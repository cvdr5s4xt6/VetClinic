using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для AppointmentDetailsWindow.xaml
    /// </summary>
    public partial class AppointmentDetailsWindow : Window
    {
        private List<Appointment> _appointments; // Храним список записей
        private Animal _selectedAnimal;
        private DateTime _selectedDate; // Поле для хранения выбранной даты

        // Конструктор для установки выбранного животного и даты
        public AppointmentDetailsWindow(Animal selectedAnimal, DateTime selectedDate)
        {
            InitializeComponent();
            _selectedAnimal = selectedAnimal;
            _selectedDate = selectedDate;

            // Получаем записи для выбранного животного на выбранную дату и на следующий день
            _appointments = GetAppointmentsForAnimalOnDate(_selectedAnimal.animal_id, _selectedDate);
            var tomorrowAppointments = GetAppointmentsForAnimalOnDate(_selectedAnimal.animal_id, _selectedDate.AddDays(1));

            // Проверка количества записей на текущую и следующую дату
            Debug.WriteLine($"Сегодня: {_appointments.Count}, Завтра: {tomorrowAppointments.Count}");

            // Объединяем списки записей
            _appointments.AddRange(tomorrowAppointments);
            Debug.WriteLine($"Общее количество записей: {_appointments.Count}"); // Проверка общего количества

            // Отобразим записи в UI
            DisplayAppointments();
        }

        private List<Appointment> GetAppointmentsForAnimalOnDate(int animalId, DateTime date)
        {
            using (var context = new VetClinica1Entities())
            {
                DateTime startOfDay = date.Date;
                DateTime endOfDay = startOfDay.AddDays(1);

                var appointments = context.Appointment
                    .Where(a => a.animal_id == animalId &&
                                a.appointment_date >= startOfDay &&
                                a.appointment_date < endOfDay)
                    .ToList();

                // Выводим количество записей в лог
                Debug.WriteLine($"Записей найдено: {appointments.Count}");

                return appointments;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Метод для отображения записей в UI
        private void DisplayAppointments()
        {
            AppointmentsListBox.Items.Clear(); // Очистка списка перед заполнением

            if (_appointments != null && _appointments.Any())
            {
                foreach (var appointment in _appointments)
                {
                    // Получаем владельца и ветеринара, связанного с записью
                    var owner = GetOwnerById(appointment.owner_id);
                    var veterinarian = GetVeterinarianById(appointment.veterenarian_id);

                    // Проверка на null для владельца и ветеринара
                    if (owner == null || veterinarian == null)
                    {
                        Debug.WriteLine($"Владелец или ветеринар не найден для записи ID: {appointment.appointment_id}");
                        continue; // Пропустить эту запись, если данные отсутствуют
                    }

                    // Создаем элемент для отображения записи
                    var item = new StackPanel { Orientation = Orientation.Horizontal };
                    var appointmentInfo = new TextBlock
                    {
                        Text = $"Питомец: {_selectedAnimal.name}, Владелец: {owner.first_name} {owner.last_name}, " +
                               $"Ветеринар: {veterinarian.first_name} {veterinarian.last_name}, " +
                               $"Время: {appointment.appointment_date.ToShortTimeString()}",
                        Margin = new Thickness(0, 0, 5, 0)
                    };
                    var startButton = new Button
                    {
                        Content = "Начать прием",
                        Margin = new Thickness(5, 0, 0, 0),
                        Tag = appointment // Сохраняем объект записи в теге кнопки
                    };
                    startButton.Click += StartAppointmentButton_Click;

                    item.Children.Add(appointmentInfo);
                    item.Children.Add(startButton);
                    AppointmentsListBox.Items.Add(item);
                }

                Debug.WriteLine($"Общее количество записей в ListBox: {AppointmentsListBox.Items.Count}"); // Проверка количества записей в ListBox
            }
            else
            {
                AppointmentsListBox.Items.Add(new TextBlock { Text = "Нет записей на этот день." });
            }
        }


        // Обработчик нажатия кнопки "Начать прием"
        private void StartAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var appointment = button.Tag as Appointment; // Получаем объект записи

            if (appointment != null)
            {
                // Логика для открытия страницы приема с использованием информации о записи
                var appointmentPage = new AddAppointmentPage(appointment.owner_id.ToString());

                // Проверьте, есть ли NavigationService
                var navigationService = NavigationService.GetNavigationService(this);
                if (navigationService != null)
                {
                    navigationService.Navigate(appointmentPage);
                }
                else
                {
                    MessageBox.Show("Не удалось перейти на страницу приема.");
                }

                this.Close(); // Закрываем текущее окно
            }
            else
            {
                MessageBox.Show("Не удалось получить данные о записи.");
            }
        }

        // Метод для получения владельца по его ID
        private Owner GetOwnerById(int ownerId)
        {
            using (var context = new VetClinica1Entities())
            {
                return context.Owner.FirstOrDefault(o => o.owner_id == ownerId);
            }
        }

        // Метод для получения ветеринара по его ID
        private Veterenarian GetVeterinarianById(int veterinarianId)
        {
            using (var context = new VetClinica1Entities())
            {
                return context.Veterenarian.FirstOrDefault(v => v.veterenarian_id == veterinarianId);
            }
        }
    }
}