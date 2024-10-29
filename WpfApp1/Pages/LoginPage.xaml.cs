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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private VetClinicaEntities _context = new VetClinicaEntities();
        private bool isPasswordVisible = false;

        public LoginPage()
        {
            InitializeComponent();
        }

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordTb.Password.Length == 0 && PasswordVisibleTb.Text.Length == 0)
            {
                MessageBox.Show("Введите хотя бы один символ.");
                return;
            }

            if (isPasswordVisible)
            {
                PasswordVisibleTb.Visibility = Visibility.Collapsed;
                PasswordTb.Visibility = Visibility.Visible;
                PasswordTb.Password = PasswordVisibleTb.Text;
                ShowPasswordButton.Content = "🔒"; 
                isPasswordVisible = false;
            }
            else
            {
                PasswordVisibleTb.Visibility = Visibility.Visible;
                PasswordTb.Visibility = Visibility.Collapsed;
                PasswordVisibleTb.Text = PasswordTb.Password;
                ShowPasswordButton.Content = "🔓";
                isPasswordVisible = true;
            }
        }

        private void PasswordTb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!isPasswordVisible)
            {
                PasswordVisibleTb.Text = PasswordTb.Password;
            }
        }

        private void PasswordVisibleTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isPasswordVisible)
            {
                PasswordTb.Password = PasswordVisibleTb.Text;
            }
        }

        private void logBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text.Trim();
            string enteredPassword = isPasswordVisible ? PasswordVisibleTb.Text.Trim() : PasswordTb.Password.Trim();

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Поле логина пустое. Пожалуйста, заполните поле логина.");
                return;
            }

            if (string.IsNullOrEmpty(enteredPassword))
            {
                MessageBox.Show("Поле пароля пустое. Пожалуйста, заполните поле пароля.");
                return;
            }

            var owner = _context.Owner.FirstOrDefault(o => o.login == username && o.password == enteredPassword);
            if (owner != null)
            {
                MakePetPage makePetPage = new MakePetPage(owner.owner_id);
                CurrentUserClient.OwnerId = owner.owner_id;
                NavigationService.Navigate(makePetPage);
                return;
            }

            var veterinarian = _context.Veterenarian.FirstOrDefault(v => v.login == username && v.password == enteredPassword);
            if (veterinarian != null)
            {
                // Передача логина на страницу AddAppointmentPage при успешной авторизации ветеринара
                AddAppointmentPage appointmentPage = new AddAppointmentPage(username);
                NavigationService.Navigate(appointmentPage);
                return;
            }

            var admin = _context.Admin.FirstOrDefault(a => a.login == username && a.password == enteredPassword);
            if (admin != null)
            {
                NavigationService.Navigate(new NavigationAdminPage());
                return;
            }

            MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

        }


        private void regBtn_Click(object sender, RoutedEventArgs e)
        {
            LogPassPanel.Visibility = Visibility.Collapsed;
            var registerPage = new RegisterUserPage();
            registerPage.IsAdminMode = false;  // Устанавливаем режим для клиента
            registerPage.IsReturnButtonVisible = false; // Скрываем кнопку
            NavigationService.Navigate(registerPage);
        }
    }
}