using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.BD;


namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddAppointmentPage.xaml
    /// </summary>
    public partial class AddAppointmentPage : Page
    {

        private string _username;
        private VetClinica1Entities _context = new VetClinica1Entities();
        public AddAppointmentPage(string username, Animal selectedAnimal = null)
        {

            _username = username;
            InitializeComponent();
            LoadPets();
            LoadVeterinarianData();
            if (selectedAnimal != null)
            {
                PetComboBox.SelectedItem = selectedAnimal;
            }

        }
    


        private void LoadPets()
        {
            try
            {
                var pets = App.bd.Animal.ToList();
                PetComboBox.ItemsSource = pets;
                PetComboBox.DisplayMemberPath = "name";
                PetComboBox.SelectedValuePath = "animal_id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки питомцев: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveVisitButton_Click(object sender, RoutedEventArgs e)
        {
            if (PetComboBox.SelectedItem is Animal selectedPet)
            {
                using (var context = new VetClinica1Entities())
                {
                    // Проверка существования записи
                    var existingRecord = context.MedicalRecord
                        .FirstOrDefault(m => m.animal_id == selectedPet.animal_id && m.diagnosis == DiagnosisTextBox.Text);

                    if (existingRecord != null)
                    {
                        MessageBox.Show("Запись о приеме уже существует для этого питомца.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Добавление теста, если он есть
                    if (!string.IsNullOrWhiteSpace(AnalysisTextBox.Text))
                    {
                        var testType = new TestTypes
                        {
                            test_type_name = AnalysisTextBox.Text
                        };
                        context.TestTypes.Add(testType);
                    }

                    // Проверка существования ветеринара
                    var veterenarianExists = _currentVeterinarian != null;

                    if (veterenarianExists)
                    {
                        var medicalRecord = new MedicalRecord
                        {
                            animal_id = selectedPet.animal_id,
                            diagnosis = DiagnosisTextBox.Text,
                            treatment = PrescriptionsTextBox.Text,
                            created_at = DateTime.Now,
                            veterenarian_id = _currentVeterinarian.veterenarian_id // используем ID из _currentVeterinarian
                        };

                        context.MedicalRecord.Add(medicalRecord);
                        context.SaveChanges();
                        MessageBox.Show("Запись о приеме успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    else
                    {
                        Console.WriteLine("Ошибка: Ветеринар с указанным ID не найден.");
                    }
                }
            }

            else
            {
                MessageBox.Show("Пожалуйста, выберите питомца.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputs()
        {
            DiagnosisTextBox.Clear();
            PrescriptionsTextBox.Clear();
            AnalysisTextBox.Clear();
            PetComboBox.SelectedIndex = -1;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
            ClearInputs();

        }

        private void ClearDiagnosisButton_Click(object sender, RoutedEventArgs e)
        {
            DiagnosisTextBox.Clear();
        }

        private void ClearPrescriptionsButton_Click(object sender, RoutedEventArgs e)
        {
            PrescriptionsTextBox.Clear();
        }

        private void ClearAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            AnalysisTextBox.Clear();
        }

        private void ClearPetComboBoxButton_Click(object sender, RoutedEventArgs e)
        {
            PetComboBox.SelectedItem = null;
        }


        private Veterenarian GetVeterinarianByLogin(string login)
        {
            using (var context = new VetClinica1Entities())
            {
                return context.Veterenarian.FirstOrDefault(v => v.login == login);
            }
        }

        private Veterenarian _currentVeterinarian; // поле класса для хранения текущего ветеринара

        private void LoadVeterinarianData()
        {
            _currentVeterinarian = _context.Veterenarian.FirstOrDefault(v => v.login == _username);

            if (_currentVeterinarian != null)
            {
                MessageBox.Show($"Добро пожаловать, {_currentVeterinarian.first_name} {_currentVeterinarian.last_name}!");
                LoggedInVeterinarianTextBlock.Text = $"Врач: {_currentVeterinarian.first_name} {_currentVeterinarian.last_name}";
            }
            else
            {
                MessageBox.Show("Ветеринар не найден.");
                LoggedInVeterinarianTextBlock.Text = "Ветеринар не найден.";
            }
        }
    }
}