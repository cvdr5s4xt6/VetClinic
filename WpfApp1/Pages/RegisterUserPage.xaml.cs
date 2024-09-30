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
    /// Логика взаимодействия для RegisterUserPage.xaml
    /// </summary>
    public partial class RegisterUserPage : Page
    {
        private bool isPasswordVisible = false;
        public RegisterUserPage()
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


        private void regBtn_Click(object sender, RoutedEventArgs e)
        {
            string surname = Surname.Text.Trim();
            string name = Name.Text.Trim();
            string phone = Phone.Text.Trim();
            string email = Email.Text.Trim();
            string password = PasswordTb.Password;

            if (string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!(phone.StartsWith("8") || phone.StartsWith("+7")))
            {
                MessageBox.Show("Номер телефона должен начинаться с 8 или +7.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string selectedRole = ((ComboBoxItem)RoleComboBox.SelectedItem)?.Content.ToString();

            using (var context = new VetClinicaEntities()) 
            {
                if (selectedRole == "Owner")
                {
                    var owner = new Owner
                    {
                        last_name = surname,
                        first_name = name,
                        phone_number = phone,
                        email = email,
                        password = password 
                    };
                    context.Owner.Add(owner);
                }
                else if (selectedRole == "Veterinarian")
                {
                    var veterinarian = new Veterenarian

                    {
                        last_name = surname,
                        first_name = name,
                        phone_number = phone,
                        email = email,
                        password = password
                    };
                    context.Veterenarian.Add(veterinarian);
                }

                context.SaveChanges();
            }

            MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            Surname.Text = string.Empty;
            Name.Text = string.Empty;
            Phone.Text = string.Empty;
            Email.Text = string.Empty;
            PasswordTb.Password = string.Empty;
            RoleComboBox.SelectedIndex = -1;

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.LogPassPanel.Visibility = Visibility.Visible;
            mainWindow.MainFrame.Visibility = Visibility.Collapsed; /*короче я не знаю хули еще раз когда тыкаешь зарегаться не работает*/
        }
    }
}
