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
        private VetClinicaEntities _context;
        private Veterenarian veterenarian; // Поле для хранения данных о владельце
        public NavigationVeterenarianPage()
        {
            InitializeComponent();
            this.veterenarian = veterenarian; // Сохраняем переданного владельца
        }

        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            // Создаем страницы с передачей необходимых параметров
            MakePetPage makePetPage = new MakePetPage(veterenarian.veterenarian_id);
            //NavigationService.Navigate(new AddAppointmentPage(username));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            ChatPage chatPage = new ChatPage("Veterinarian", veterenarian.veterenarian_id, veterenarian.login);

            // Сохраняем ID владельца
            CurrentUser.VeterinarianId = veterenarian.veterenarian_id;

            NavigationService.Navigate(chatPage); // Переход на ChatPage, если нужно
            return;
        }

        private void A_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
