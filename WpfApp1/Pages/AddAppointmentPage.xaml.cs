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
        private VetClinicaEntities _context = new VetClinicaEntities();
        public AddAppointmentPage(string username)
        {

            _username = username;
            InitializeComponent();
            LoadPets();
            LoadVeterinarianData();
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
                using (var context = new VetClinicaEntities())
                {

                    var existingRecord = context.MedicalRecord.FirstOrDefault(m => m.animal_id == selectedPet.animal_id && m.diagnosis == DiagnosisTextBox.Text);

                    if (existingRecord != null)
                    {
                        MessageBox.Show("Запись о приеме уже существует для этого питомца.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(AnalysisTextBox.Text))
                    {
                        var testType = new TestTypes
                        {
                            test_type_name = AnalysisTextBox.Text
                        };

                        context.TestTypes.Add(testType);
                    }

                    var medicalRecord = new MedicalRecord
                    {
                        animal_id = selectedPet.animal_id,
                        diagnosis = DiagnosisTextBox.Text,
                        treatment = PrescriptionsTextBox.Text,

                        veterenarian_id = CurrentUser.VeterinarianId
                    };

                    context.MedicalRecord.Add(medicalRecord);
                    context.SaveChanges();

                    MessageBox.Show("Прием успешно сохранен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterUserPage());
        }


        private Veterenarian GetVeterinarianByLogin(string login)
        {
            using (var context = new VetClinicaEntities())
            {
                return context.Veterenarian.FirstOrDefault(v => v.login == login);
            }
        }

        private void LoadVeterinarianData()
        {
            var veterinarian = _context.Veterenarian.FirstOrDefault(v => v.login == _username);

            if (veterinarian != null)
            {
                MessageBox.Show($"Добро пожаловать, {veterinarian.first_name} {veterinarian.last_name}!");
                LoggedInVeterinarianTextBlock.Text = $"Врач: {veterinarian.first_name} {veterinarian.last_name}";

            }
            else
            {
                MessageBox.Show("Ветеринар не найден.");
                LoggedInVeterinarianTextBlock.Text = "Ветеринар не найден.";
            }
        
        }
    }
}