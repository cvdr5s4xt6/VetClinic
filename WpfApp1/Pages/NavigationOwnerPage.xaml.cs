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
    /// Логика взаимодействия для NavigationOwnerPage.xaml
    /// </summary>
    public partial class NavigationOwnerPage : Page
    {
        private VetClinicaEntities _context;
        private Owner owner; // Поле для хранения данных о владельце

        public NavigationOwnerPage(Owner owner)
        {
            InitializeComponent();
            this.owner = owner; // Сохраняем переданного владельца
        }

        private void MakePet_Click(object sender, RoutedEventArgs e)
        {
            // Создаем страницы с передачей необходимых параметров
            MakePetPage makePetPage = new MakePetPage(owner.owner_id);
            NavigationService.Navigate(makePetPage); 

        }

        private void RegisterVeterinarianPet_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddPetPage());
        }

        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            
            ChatPage chatPage = new ChatPage("Owner", owner.owner_id, owner.login);

            // Сохраняем ID владельца
            CurrentUserClient.OwnerId = owner.owner_id;

            NavigationService.Navigate(chatPage); // Переход на ChatPage, если нужно
            return;
        }

        private void PetImage_Click(object sender, RoutedEventArgs e)
        {
            int imageId = 5;
            NavigationService.Navigate(new PetImagePage(imageId));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
