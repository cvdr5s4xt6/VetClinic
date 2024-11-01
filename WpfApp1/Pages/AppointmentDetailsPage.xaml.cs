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
    /// Логика взаимодействия для AppointmentDetailsPage.xaml
    /// </summary>
    public partial class AppointmentDetailsPage : Page
    {
        private List<Appointment> _appointments; 
        private DateTime _selectedDate; 

        // Конструктор для установки выбранного животного и даты
        public AppointmentDetailsPage(DateTime selectedDate)
        {
            InitializeComponent();
            _selectedDate = selectedDate;

            DateTime startOfDay = _selectedDate.Date;
            DateTime middleDate = _selectedDate.AddDays(1);
            DateTime endOfDay = startOfDay.AddDays(2);

            _appointments = App.bd.Appointment.Where(
                a => a.appointment_date >= startOfDay &&
                            a.appointment_date < endOfDay &&
                            a.veterenarian_id == CurrentUser.VeterinarianId).ToList();
            int todayAppointmentsCount = 0;
            int tommorowAppointmentsCount = 0;
            foreach (Appointment appointment in _appointments)
            {
                if(appointment.appointment_date < middleDate)
                {
                    todayAppointmentsCount++;
                }
                else
                {
                    tommorowAppointmentsCount++;
                }
            }

            Debug.WriteLine($"Сегодня: {todayAppointmentsCount}, Завтра: {tommorowAppointmentsCount}");
            Debug.WriteLine($"Общее количество записей: {_appointments.Count}"); 
            DisplayAppointments();
        }

        private void DisplayAppointments()
        {
            AppointmentsListBox.Items.Clear(); 

            if (_appointments != null && _appointments.Any())
            {
                foreach (var appointment in _appointments)
                {
                    var owner = GetOwnerById(appointment.owner_id);
                    var veterinarian = GetVeterinarianById(appointment.veterenarian_id);

                    if (owner == null || veterinarian == null)
                    {
                        Debug.WriteLine($"Владелец или ветеринар не найден для записи ID: {appointment.appointment_id}");
                        continue; 
                    }

                    // Создаем элемент для отображения записи
                    var item = new StackPanel { Orientation = Orientation.Horizontal };
                    var appointmentInfo = new TextBlock
                    {
                        Text = $"Питомец: {appointment.Animal.name}, Владелец: {owner.first_name} {owner.last_name}, " +
                               $"Ветеринар: {veterinarian.first_name} {veterinarian.last_name}, " +
                               $"Время: {appointment.appointment_date.ToShortTimeString()}",
                        Margin = new Thickness(0, 0, 5, 0)
                    };
                    var startButton = new Button
                    {
                        Content = "Начать прием",
                        Margin = new Thickness(5, 0, 0, 0),
                        Tag = appointment, 

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

        private void StartAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var appointment = button.Tag as Appointment;

            if (appointment != null)
            {
                App.mainFrame.Navigate(new AddAppointmentPage(appointment.Veterenarian.login, appointment.Animal));
            }
            else
            {
                MessageBox.Show("Не удалось получить данные о записи.");
            }
        }

        private Owner GetOwnerById(int ownerId)
        {
            using (var context = new VetClinica1Entities())
            {
                return context.Owner.FirstOrDefault(o => o.owner_id == ownerId);
            }
        }

        private Veterenarian GetVeterinarianById(int veterinarianId)
        {
            using (var context = new VetClinica1Entities())
            {
                return context.Veterenarian.FirstOrDefault(v => v.veterenarian_id == veterinarianId);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
