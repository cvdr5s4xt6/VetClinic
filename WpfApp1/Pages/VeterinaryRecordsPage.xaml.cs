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
    /// Логика взаимодействия для VeterinaryRecordsPage.xaml
    /// </summary>
    public partial class VeterinaryRecordsPage : Page
    {
        private VetClinicaEntities _context;
        private int _veterinarianId;
        public VeterinaryRecordsPage()
        {
            InitializeComponent();
        }

        public VeterinaryRecordsPage(int veterinarianId)
        {
            InitializeComponent();
            _context = new VetClinicaEntities();
            _veterinarianId = veterinarianId;

            LoadRecords();
        }

        private void LoadRecords()
        {
            var records = from a in _context.Appointment
                          join an in _context.Animal on a.nimal_id equals an.AnimalId
                          join o in _context.Owner on an.OwnerId equals o.OwnerId
                          where a.VeterinarianId == _veterinarianId
                          select new VeterinaryRecord
                          {
                              RecordID = a.AppointmentId,
                              Date = a.AppointmentDate,
                              OwnerName = o.FirstName + " " + o.LastName,
                              PetName = an.Name,
                              Status = a.Status
                          };


            var recordList = records.ToList();

            // Отладка: проверка, есть ли записи
            if (recordList.Count == 0)
            {
                MessageBox.Show("Нет записей для данного ветеринара.");
            }
            else
            {
                RecordsListView.ItemsSource = recordList;
            }
        
    }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void RecordsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Здесь можно добавить логику для обработки выбранного элемента
        }
    }
}
    

