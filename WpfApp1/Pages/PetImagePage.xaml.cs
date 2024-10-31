using System;
using System.Collections.Generic;
using System.IO;
using System.Data.Entity;
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
using System.Linq;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для PetImagePage.xaml
    /// </summary>
    public partial class PetImagePage : Page
    {
        private VetClinica1Entities _context;

        public PetImagePage(int imageId)
        {
            InitializeComponent();
            _context = new VetClinica1Entities();
            LoadImagesByDate(DateTime.Today);
        }

        private void LoadImagesByDate(DateTime? selectedDate)
        {
            var petImages = _context.PetImages
                .AsEnumerable() // Переключаемся на LINQ to Objects после загрузки данных
                .Where(img => selectedDate == null
                           || img.created_at.Date == selectedDate.Value.Date)
                .ToList();

            var imageSources = new List<BitmapImage>();

            foreach (var petImage in petImages)
            {
                if (petImage.image != null)
                {
                    using (var ms = new MemoryStream(petImage.image))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = ms;
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.EndInit();
                        imageSources.Add(image);
                    }
                }
            }

            ImagesItemsControl.ItemsSource = imageSources;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = DatePicker.SelectedDate;
            LoadImagesByDate(selectedDate);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}