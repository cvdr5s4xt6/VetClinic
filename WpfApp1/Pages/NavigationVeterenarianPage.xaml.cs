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
    /// Логика взаимодействия для NavigationVeterenarianPage.xaml
    /// </summary>
    public partial class NavigationVeterenarianPage : Page
    {

        private VetClinica1Entities _context;
        private Veterenarian _veterinarian;

        public NavigationVeterenarianPage(Veterenarian veterinarian)
        {

            InitializeComponent();
            this._veterinarian = veterinarian;
            _context = new VetClinica1Entities();

            // Подписка на событие Loaded для инициализации календаря после загрузки страницы
            this.Loaded += NavigationVeterenarianPage_Loaded;
        }
        

        private void NavigationVeterenarianPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Устанавливаем диапазон дат для календаря
            AppointmentsCalendar.DisplayDateStart = DateTime.Now;
            AppointmentsCalendar.DisplayDateEnd = DateTime.MaxValue;
        }

        //private void AddAppointment_Click(object sender, RoutedEventArgs e)
        //{
        //    AddAppointmentPage appointmentPage = new AddAppointmentPage(_veterinarian.login);
        //    NavigationService.Navigate(appointmentPage);
        //    NavigateToAddAppointmentPage();

        //}

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            ChatPage chatPage = new ChatPage("Veterinarian", _veterinarian.veterenarian_id, _veterinarian.login);

            CurrentUser.VeterinarianId = _veterinarian.veterenarian_id;

            NavigationService.Navigate(chatPage); 
            return;
        }
        private void AppointmentsCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppointmentsCalendar.SelectedDate is DateTime selectedDate && selectedDate >= DateTime.Today)
            {
                DateTime startOfDay = selectedDate.Date;
                DateTime endOfDay = startOfDay.AddDays(1); // Добавляем 1 день

                // Загружаем записи на выбранный день
                var appointments = _context.MedicalRecord
                    .Where(record => record.created_at >= startOfDay && record.created_at < endOfDay)
                    .ToList();

                // Проверка на наличие записей
                if (appointments.Any())
                {
                    // Показать выбор питомца для конкретного хозяина
                    var ownerAppointments = appointments; // Можно изменить на фильтрацию по владельцу, если это нужно

                    // Отладочный вывод
                    Console.WriteLine($"Найдено записей: {appointments.Count}");
                    Console.WriteLine($"Записей у владельца: {ownerAppointments.Count}");

                    if (ownerAppointments.Any())
                    {
                        // Открываем кастомное окно с деталями приема, передавая животное и выбранную дату
                        var detailsWindow = new AppointmentDetailsPage(selectedDate);
                        NavigationService.Navigate(detailsWindow); // Открываем окно как диалог
                    }
                    else
                    {
                        MessageBox.Show("Записей на этот день у вас нет.");
                    }
                }
                else
                {
                    MessageBox.Show("Записей на этот день нет.");
                }
            }
            else
            {
                MessageBox.Show("Записи на выбранную дату не доступны.");
            }
        }
    }

}