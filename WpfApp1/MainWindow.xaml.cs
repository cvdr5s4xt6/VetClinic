using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using WpfApp1.Pages;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        private bool isPasswordVisible = false;
        public MainWindow()
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

           
           
            if (App.bd.Owner.Any(x => x.login.ToString() == Username.Text) && App.bd.Owner.Any(x => x.password.ToString() == enteredPassword))
            {
                LogPassPanel.Visibility = Visibility.Collapsed;
                MessageBox.Show("Пароль верен. Вход выполнен. Пользователь");         
                var appointmentPage = new AddAppointmentPage();        
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(appointmentPage);

            }
            else if(App.bd.Owner.Any(x => x.password.ToString() == enteredPassword) && App.bd.Owner.Any(x => x.login.ToString() == Username.Text))
            {
                MessageBox.Show("Неправильный пароль. Пожалуйста, проверьте пароль.");
            }
            else if(App.bd.Veterenarian.Any(x => x.login.ToString() == Username.Text) && App.bd.Veterenarian.Any(x => x.password.ToString() == enteredPassword))
            {
                LogPassPanel.Visibility = Visibility.Collapsed;
                MessageBox.Show("Пароль верен. Вход выполнен. Ветеринар");
                var appointmentPage = new AddAppointmentPage();             
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(appointmentPage);

            }
        }

        private void regBtn_Click(object sender, RoutedEventArgs e)
        {
            LogPassPanel.Visibility = Visibility.Collapsed;
            var registerPage = new RegisterUserPage();  
            ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(registerPage);
        }
    }
}