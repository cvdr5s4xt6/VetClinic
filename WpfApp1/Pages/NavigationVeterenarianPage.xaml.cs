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
        private Veterenarian _veterinarian;

        public NavigationVeterenarianPage(Veterenarian veterinarian)
        {
            InitializeComponent();
            this._veterinarian = veterinarian;
        }

        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            AddAppointmentPage appointmentPage = new AddAppointmentPage(_veterinarian.login);
            NavigationService.Navigate(appointmentPage);
        }

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

        private void A_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new VeterinaryRecordsPage());
        }
    }
}