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

        private void printReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                // Создаем объект Visual для печати
                var printVisual = new StackPanel();

                // Заголовок страницы печати
                var headerTextBlock = new TextBlock
                {
                    Text = "Отчеты о работе ветеринара",
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 10)
                };
                printVisual.Children.Add(headerTextBlock);

                // Находим TabControl в иерархии визуальных элементов
                TabControl tabControl = FindVisualChild<TabControl>(this);

                if (tabControl != null)
                {
                    // Копируем содержимое DataGrid в StackPanel для печати
                    foreach (TabItem tab in tabControl.Items)
                    {
                        DataGrid dataGrid = tab.Content as DataGrid;
                        if (dataGrid != null)
                        {
                            var tabHeader = new TextBlock
                            {
                                Text = tab.Header.ToString(),
                                FontSize = 16,
                                FontWeight = FontWeights.SemiBold,
                                Margin = new Thickness(0, 10, 0, 5)
                            };
                            printVisual.Children.Add(tabHeader);

                            DataGrid printDataGrid = new DataGrid
                            {
                                AutoGenerateColumns = dataGrid.AutoGenerateColumns,
                                HeadersVisibility = dataGrid.HeadersVisibility,
                                RowHeight = dataGrid.RowHeight,
                                ItemsSource = dataGrid.ItemsSource,
                                FontSize = dataGrid.FontSize,
                                Width = dataGrid.Width,
                                Margin = dataGrid.Margin,
                                IsReadOnly = true
                            };
                            foreach (var column in dataGrid.Columns)
                            {
                                printDataGrid.Columns.Add(new DataGridTextColumn
                                {
                                    Header = column.Header,
                                    Binding = ((DataGridTextColumn)column).Binding
                                });
                            }
                            printVisual.Children.Add(printDataGrid);
                        }
                    }

                    // Печать созданного Visual
                    printDialog.PrintVisual(printVisual, "Отчеты о работе ветеринара");
                }
            }
        }

        // Вспомогательный метод для поиска элемента в визуальной иерархии
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T foundChild)
                    return foundChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}