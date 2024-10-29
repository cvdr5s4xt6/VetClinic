using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public event Action PetAdded; // Событие для уведомления о добавлении питомца
        private List<string> animalTypeSuggestions = new List<string>();

        public AddPetPage()
        {
            InitializeComponent();
        }

        private void AnimalTypeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = AnimalTypeTextBox.Text.Trim();
            if (input.Length > 0)
            {
                LoadAnimalTypeSuggestions(input);
            }
            else
            {
                animalTypeSuggestions.Clear();
            }
        }

        private void LoadAnimalTypeSuggestions(string query)
        {
            using (var context = new VetClinicaEntities())
            {
                animalTypeSuggestions = context.AnimalType
                    .Where(at => at.animal_type_name.StartsWith(query))
                    .Select(at => at.animal_type_name)
                    .ToList();
            }

            // Показать первую подсказку, если она есть
            if (animalTypeSuggestions.Any())
            {
                AnimalTypeTextBox.Text = animalTypeSuggestions.First();
                AnimalTypeTextBox.SelectionStart = query.Length;
                AnimalTypeTextBox.SelectionLength = animalTypeSuggestions.First().Length - query.Length;
            }
        }

        private void AnimalTypeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && AnimalTypeTextBox.SelectionLength > 0)
            {
                AnimalTypeTextBox.SelectionStart = AnimalTypeTextBox.Text.Length;
                AnimalTypeTextBox.SelectionLength = 0;
                e.Handled = true;
            }
        }

        private void SaveAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AnimalNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(AnimalBreedTextBox.Text) ||
                string.IsNullOrWhiteSpace(AnimalAgeTextBox.Text) ||
                string.IsNullOrWhiteSpace(AnimalTypeTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new VetClinicaEntities())
            {
                // Проверяем наличие типа животного
                string animalTypeName = AnimalTypeTextBox.Text.Trim();
                var animalType = context.AnimalType.FirstOrDefault(at => at.animal_type_name == animalTypeName);

                if (animalType == null)
                {
                    // Если тип не найден, создаем новый
                    animalType = new AnimalType { animal_type_name = animalTypeName };
                    context.AnimalType.Add(animalType);
                    context.SaveChanges();
                }

                var newAnimal = new Animal
                {
                    name = AnimalNameTextBox.Text.Trim(),
                    animal_type_id = animalType.animal_type_id,
                    breed = AnimalBreedTextBox.Text.Trim(),
                    age = AnimalAgeTextBox.Text.Trim(),
                    owner_id = CurrentUserClient.OwnerId
                };

                context.Animal.Add(newAnimal);
                context.SaveChanges();

                PetAdded?.Invoke(); // Вызываем событие
                NavigationService.GoBack(); // Возврат на предыдущую страницу

                MessageBox.Show("Животное успешно зарегистрировано.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearInputs();
            }
        }

        private void ClearInputs()
        {
            AnimalNameTextBox.Clear();
            AnimalBreedTextBox.Clear();
            AnimalAgeTextBox.Clear();
            AnimalTypeTextBox.Clear();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[а-яА-Яa-zA-Z]+$"))
            {
                e.Handled = true;
                MessageBox.Show("Вводить можно только буквы.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AgeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^\d+$"))
            {
                e.Handled = true;
                MessageBox.Show("Вводить можно только цифры.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClearAnimalTypeComboBoxButton(object sender, RoutedEventArgs e)
        {
            ClearInputs();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}