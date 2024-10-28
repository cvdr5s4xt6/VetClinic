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

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для NavigationAdminPage.xaml
    /// </summary>
    public partial class NavigationAdminPage : Page
    {
        public NavigationAdminPage()
        {
            InitializeComponent();
        }

        private void ViewReports_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminReportsPage());
        }

        private void RegisterVeterinarian_Click(object sender, RoutedEventArgs e)
        {
            var registerPage = new RegisterUserPage();
            registerPage.IsAdminMode = true;
            NavigationService.Navigate(new RegisterUserPage());
        }
    }
}
