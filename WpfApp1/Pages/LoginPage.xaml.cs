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
        private VetClinica1Entities _context = new VetClinica1Entities();
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

            var owner = _context.Owner.FirstOrDefault(o => o.login == username);
            if (owner != null && VerifyPassword(enteredPassword, owner.password)) 
            {
                CurrentUserClient.OwnerId = owner.owner_id;
                MessageBox.Show($"Добро пожаловать, {owner.last_name} {owner.first_name}!", "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new NavigationOwnerPage(owner));
                return;
            }

            var veterinarian = _context.Veterenarian.FirstOrDefault(v => v.login == username);
            if (veterinarian != null && VerifyPassword(enteredPassword, veterinarian.password)) 
            {
                CurrentUser.VeterinarianId = veterinarian.veterenarian_id;
                MessageBox.Show($"Добро пожаловать, {veterinarian.last_name} {veterinarian.first_name}!", "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationVeterenarianPage navPage = new NavigationVeterenarianPage(veterinarian);
                NavigationService.Navigate(navPage);

                return;
            }

            var admin = _context.Admin.FirstOrDefault(a => a.login == username);
            if (admin != null && VerifyPassword(enteredPassword, admin.password))
            {
                NavigationService.Navigate(new NavigationAdminPage());
                return;
            }


            MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return enteredPassword == storedPassword;
        }


        private void regBtn_Click(object sender, RoutedEventArgs e)
        {
            LogPassPanel.Visibility = Visibility.Collapsed;
            var registerPage = new RegisterUserPage();
            registerPage.IsAdminMode = false;  // Устанавливаем режим для клиента
            NavigationService.Navigate(registerPage);
        }
    }
}