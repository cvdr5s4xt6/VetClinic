using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для AdminReportsPage.xaml
    /// </summary>
    public partial class AdminReportsPage : Page
    {
        private VetClinica1Entities _context;

        public AdminReportsPage()
        {
            InitializeComponent();
            _context = new VetClinica1Entities();
            LoadAppointmentsData();
            LoadMedicalTestsData();
            LoadAnimalInfoData();
        }

        private void LoadAppointmentsData()
        {
            try
            {
             var appointmentsReport = _context.Appointment.GroupBy(a => new { AppointmentDate = a.appointment_date, VeterinarianName = a.Veterenarian.first_name + " " + a.Veterenarian.last_name})  .Select(g => new {g.Key.AppointmentDate,g.Key.VeterinarianName, AppointmentCount = g.Count() }).OrderBy(r => r.AppointmentDate).ToList();

                AppointmentsDataGrid.ItemsSource = appointmentsReport;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных по приемам: {ex.Message}");
            }
        }

        private void LoadMedicalTestsData()
        {
            try
            {
             var medicalTestsReport = _context.MedicalRecord.Select(record => new{AnimalName = record.Animal.name,record.diagnosis,record.treatment,}).ToList();

                MedicalTestsDataGrid.ItemsSource = medicalTestsReport;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных по медицинским тестам: {ex.Message}");
            }
        }

        private void LoadAnimalInfoData()
        {
            try
            {

             var animalInfo = _context.Animal.Select(a => new{a.name,a.breed,a.age,OwnerName = a.Owner.first_name + " " + a.Owner.last_name}).ToList();

                AnimalInfoDataGrid.ItemsSource = animalInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных по животным: {ex.Message}");
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new NavigationAdminPage());
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

    }
}