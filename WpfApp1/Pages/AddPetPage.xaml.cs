using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для AddPetPage.xaml
    /// </summary>
    public partial class AddPetPage : Page
    {
        public AddPetPage()
        {
            InitializeComponent();
            LoadAnimalTypes();
        }

        private void LoadAnimalTypes()
        {
            try
            {
                using (var context = new VetClinicaEntities2())
                {
                    var animalTypes = context.AnimalType.ToList();
                    AnimalTypeComboBox.ItemsSource = animalTypes;
                    AnimalTypeComboBox.DisplayMemberPath = "animal_type_name";
                    AnimalTypeComboBox.SelectedValuePath = "animal_type_id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов животных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AnimalNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(AnimalBreedTextBox.Text) ||
                string.IsNullOrWhiteSpace(AnimalAgeTextBox.Text) ||
                AnimalTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new VetClinicaEntities2())
            {
                    var selectedType = (AnimalType)AnimalTypeComboBox.SelectedItem;

                    var newAnimal = new Animal
                    {
                        name = AnimalNameTextBox.Text.Trim(),
                        animal_type_id = selectedType.animal_type_id,
                        breed = AnimalBreedTextBox.Text.Trim(),
                        age = AnimalAgeTextBox.Text.Trim(),
                        owner_id = CurrentUserClient.OwnerId 
                    };

             var existingAnimal = context.Animal.FirstOrDefault(a => a.name == newAnimal.name && a.owner_id == newAnimal.owner_id);

                    context.Animal.Add(newAnimal);
                    context.SaveChanges();

                    MessageBox.Show("Животное успешно зарегистрировано.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearInputs();
            }
        }

        private void ClearInputs()
        {
            AnimalNameTextBox.Clear();
            AnimalBreedTextBox.Clear();
            AnimalAgeTextBox.Clear();
            AnimalTypeComboBox.SelectedIndex = -1;
        }

        private void ClearAnimalTypeComboBoxButton_Click(object sender, RoutedEventArgs e)
        {
            AnimalTypeComboBox.SelectedItem = null;
        }
    }
}