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
using System.Xml.Linq;
using WpfApp1.BD;
using System.IO;
using System.Globalization;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для MakePetPage.xaml
    /// </summary>
    public partial class MakePetPage : Page
    {

        private VetClinicaEntities _context;
        private int _ownerId;
        public MakePetPage(int ownerId)
        {
            InitializeComponent();
            _ownerId = ownerId;
            _context = new VetClinicaEntities();
            LoadAnimals();
            LoadVeterinarians();
        }
        private void LoadAnimals()
        {
            var animals = _context.Animal.Where(a => a.owner_id == _ownerId).Select(a => new { a.animal_id, a.name }).ToList();

            AnimalComboBox.ItemsSource = animals;
            AnimalComboBox.DisplayMemberPath = "name";
            AnimalComboBox.SelectedValuePath = "animal_id";
        }

        private void LoadVeterinarians()
        {
            var veterinarians = _context.Veterenarian.Select(v => new { v.veterenarian_id, FullName = v.first_name + " " + v.last_name }).ToList();

            VeterinarianComboBox.ItemsSource = veterinarians;
            VeterinarianComboBox.DisplayMemberPath = "FullName";
            VeterinarianComboBox.SelectedValuePath = "veterenarian_id";
        }

        private List<byte[]> _selectedImages = new List<byte[]>();


        private void OnAppointmentButtonClick(object sender, RoutedEventArgs e)
        {
            if (AnimalComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите животное.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (VeterinarianComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите ветеринара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            if (AppointmentDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите дату приема.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string selectedTime = (TimeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(selectedTime))
            {
                MessageBox.Show("Пожалуйста, выберите время приема.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int selectedAnimalId = (int)AnimalComboBox.SelectedValue;
            int selectedVeterinarianId = (int)VeterinarianComboBox.SelectedValue;

            DateTime selectedDate = AppointmentDatePicker.SelectedDate.Value;

            DateTime appointmentDateTime;

            try
            {
                appointmentDateTime = DateTime.Parse($"{selectedDate.ToShortDateString()} {selectedTime}");
            }
            catch (FormatException)
            {
                MessageBox.Show("Неверный формат времени.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selectedDate < DateTime.Now.Date)
            {
                MessageBox.Show("Нельзя записаться на прошедшую дату.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selectedDate < new DateTime(1753, 1, 1) || selectedDate > new DateTime(9999, 12, 31))
            {
                MessageBox.Show("Дата приема выходит за пределы допустимого диапазона.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            // Проверка 1: Запрещаем запись одного и того же питомца к одному и тому же врачу на одно и то же время
            var existingAppointmentForAnimal = _context.Appointment
                .FirstOrDefault(a => a.animal_id == selectedAnimalId && a.veterenarian_id == selectedVeterinarianId && a.appointment_date == appointmentDateTime);

            if (existingAppointmentForAnimal != null)
            {
                MessageBox.Show("Этот питомец уже записан к данному врачу на это время.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка 2: Запрещаем запись другого питомца (другого владельца) к этому же врачу на это же время
            var existingAppointmentForVeterinarian = _context.Appointment
                .FirstOrDefault(a => a.veterenarian_id == selectedVeterinarianId && a.appointment_date == appointmentDateTime);

            if (existingAppointmentForVeterinarian != null)
            {
                MessageBox.Show("У данного врача уже есть запись на это время.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }



            foreach (var image in _selectedImages)
            {
                var animalImage = new PetImages
                {
                    animal_id = selectedAnimalId,
                    image = image,
                    created_at = DateTime.Now  // Явно указываем текущую дату
                    // Можно добавить описание, если нужно
                };

                _context.PetImages.Add(animalImage);
            }


            string comment = AnalysisTextBox.Text;

            var appointment = new Appointment
            {
                animal_id = selectedAnimalId,
                veterenarian_id = selectedVeterinarianId,
                appointment_date = appointmentDateTime,
                reason = " ",
                comment = comment
            };

            _context.Appointment.Add(appointment);
            _context.SaveChanges();

            MessageBox.Show("Животное успешно записано на прием!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            AnalysisTextBox.Clear();
        }

        private void OnPetAdded()
        {
            LoadAnimals();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            AddPetPage addPetPage = new AddPetPage();
            addPetPage.PetAdded += OnPetAdded;
            NavigationService.Navigate(addPetPage);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CurrentUserClient.OwnerId = 0;
            NavigationService.GoBack();
        }


        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    // Создаем StackPanel для изображения и кнопки удаления
                    StackPanel imagePanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };

                    var bitmap = new BitmapImage(new Uri(fileName));
                    Image imageControl = new Image
                    {
                        Source = bitmap,
                        Width = 100,
                        Height = 100,
                        Margin = new Thickness(5)
                    };

                    Button deleteButton = new Button
                    {
                        Content = "✖",
                        Width = 20,
                        Height = 20,
                        Margin = new Thickness(5),
                        Tag = fileName
                    };
                    deleteButton.Click += DeleteImageButton_Click;
   
                    imagePanel.Children.Add(imageControl);
                    imagePanel.Children.Add(deleteButton);

                    ImageItemsControl.Items.Add(imagePanel);
                }
            }
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = sender as Button;
            if (deleteButton != null)
            {
                string fileName = deleteButton.Tag as string;

                // Удаляем изображение из списка _selectedImages, если оно есть
                var image = _selectedImages.FirstOrDefault(img => img.SequenceEqual(File.ReadAllBytes(fileName)));
                if (image != null)
                {
                    _selectedImages.Remove(image);
                }

                // Удаляем StackPanel с изображением и кнопкой из ItemsControl
                var parentPanel = deleteButton.Parent as StackPanel;
                if (parentPanel != null)
                {
                    ImageItemsControl.Items.Remove(parentPanel);
                }
            }
        }
    }
}