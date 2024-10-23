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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
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

        public void logBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text.Trim();
            string enteredPassword = isPasswordVisible ? PasswordVisibleTb.Text.Trim() : PasswordTb.Password.Trim();

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(enteredPassword))
            {
                MessageBox.Show("Оба поля пустые. Пожалуйста, заполните все поля.");
                return;
            }
            else if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Поле логина пустое. Пожалуйста, заполните поле логина.");
                return;
            }
            else if (string.IsNullOrEmpty(enteredPassword))
            {
                MessageBox.Show("Поле пароля пустое. Пожалуйста, заполните поле пароля.");
                return;
            }

            bool isLoginCorrect = App.bd.Owner.Any(x => x.login == username) ||
                      App.bd.Veterenarian.Any(x => x.login == username) ||
                      App.bd.Admin.Any(x => x.login == username); 

            bool isPasswordCorrect = App.bd.Owner.Any(x => x.password == enteredPassword) ||
                                     App.bd.Veterenarian.Any(x => x.password == enteredPassword) ||
                                     App.bd.Admin.Any(x => x.password == enteredPassword); 

            if (isLoginCorrect && isPasswordCorrect)
            {
                if (App.bd.Owner.Any(x => x.login == username && x.password == enteredPassword))
                {
                    var owner = App.bd.Owner.FirstOrDefault(x => x.login == username);
                    if (owner != null)
                        CurrentUserClient.OwnerId = owner.owner_id;
                    MessageBox.Show("Пароль верен. Вход выполнен. Клиент.");
                    var addPetPage = new AddPetPage();
                    NavigationService.Navigate(addPetPage);
                }
                else if (App.bd.Veterenarian.Any(x => x.login == username && x.password == enteredPassword))
                {
                    CurrentUser.VeterinarianId = 1;
                    MessageBox.Show("Пароль верен. Вход выполнен. Ветеринар.");
                    var appointmentPage = new AddAppointmentPage();
                    NavigationService.Navigate(appointmentPage);
                }
                else if (App.bd.Admin.Any(x => x.login == username && x.password == enteredPassword)) 
                {
                    MessageBox.Show("Пароль верен. Вход выполнен. Администратор.");
                    var adminPage = new AdminReportsPage(); 
                    NavigationService.Navigate(adminPage);
                }
            }
            else
            {
                if (!isLoginCorrect && !isPasswordCorrect)
                {
                    MessageBox.Show("Ошибка в логине и пароле. Проверьте правильность данных.");
                }
                else if (!isLoginCorrect)
                {
                    MessageBox.Show("Ошибка в логине. Проверьте логин.");
                }
                else if (!isPasswordCorrect)
                {
                    MessageBox.Show("Ошибка в пароле. Проверьте пароль.");
                }
            }
        }

            private void regBtn_Click(object sender, RoutedEventArgs e)
        {
            LogPassPanel.Visibility = Visibility.Collapsed;
            var registerPage = new RegisterUserPage();
            NavigationService.Navigate(registerPage);
        }
    }
}