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
    /// Логика взаимодействия для RegisterAdminPage.xaml
    /// </summary>
    public partial class RegisterAdminPage : Page
    {
        public RegisterAdminPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private bool isPasswordVisible = false;

        //public bool IsReturnButtonVisible
        //{
        //    get => ReturnToAppointmentButton.Visibility == Visibility.Visible;
        //    set => ReturnToAppointmentButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        //}


        private bool _isAdminMode;

        public bool IsAdminMode
        {
            get => _isAdminMode;
            set
            {
                _isAdminMode = value;
                if (_isAdminMode)
                {
                    HideRole("Клиент");  // Скрываем роль "Клиент" для администратора
                }
                else
                {
                    HideRole("Ветеринар");  // Скрываем роль "Ветеринар" для клиента
                }
            }
        }

        private void HideRole(string role)
        {
            var item = RoleComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == role);
            if (item != null)
            {
                RoleComboBox.Items.Remove(item);
            }
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

            CurrentUserClient.OwnerId = 1;

            string surname = Surname.Text.Trim();
            string name = Name.Text.Trim();
            string phone = Phone.Text.Trim();
            string email = Email.Text.Trim();
            string password = PasswordTb.Password;
            string login = Login.Text.Trim();

            if (string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string selectedRole = ((ComboBoxItem)RoleComboBox.SelectedItem)?.Content.ToString();
            if (string.IsNullOrEmpty(selectedRole))
            {
                MessageBox.Show("Пожалуйста, выберите роль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var context = new VetClinicaEntities();

            bool loginExists = context.Owner.Any(o => o.login == login) ||
                               context.Veterenarian.Any(v => v.login == login);

            if (loginExists)
            {
                MessageBox.Show("Такой логин уже существует, придумайте другой логин.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool passwordExists = context.Owner.Any(o => o.password == password) ||
                                  context.Veterenarian.Any(v => v.password == password);

            if (passwordExists)
            {
                MessageBox.Show("Такой пароль уже существует, придумайте другой пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool emailExists = context.Owner.Any(o => o.email == email) ||
                               context.Veterenarian.Any(v => v.email == email);

            if (emailExists)
            {
                MessageBox.Show("Такая почта уже существует, придумайте другую почту.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool phoneExists = context.Owner.Any(o => o.phone_number == phone) ||
                   context.Veterenarian.Any(v => v.phone_number == phone);

            if (phoneExists)
            {
                MessageBox.Show("Этот номер телефона уже зарегистрирован, используйте другой номер.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

                var veterenarian = new Veterenarian
                {
                    last_name = surname,
                    first_name = name,
                    phone_number = phone,
                    email = email,
                    login = login,
                    password = password
                };
                context.Veterenarian.Add(veterenarian);
            

            context.SaveChanges();

            MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
NavigationService.GoBack();
        }
        private void Phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsPhoneInputValid(Phone.Text + e.Text))
            {
                MessageBox.Show("Введите только цифры, а также один из префиксов +7 или 8.");
                e.Handled = true;
            }
        }

        private bool IsPhoneInputValid(string input)
        {
            if (input.Length == 0) return true;

            if (input.Length == 1)
            {
                return input == "8" || input == "+";
            }

            if (input.StartsWith("+"))
            {
                return input.Length == 2 && input[1] == '7' || input.Substring(0, 2) == "+7" && input.Substring(2).All(char.IsDigit);
            }
            else if (input.StartsWith("8"))
            {
                return input.Substring(1).All(char.IsDigit);
            }

            return input.All(char.IsDigit);
        }


        private bool hasShownMessage = false;
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsRussianLetter(e.Text))
            {
                if (!hasShownMessage)
                {
                    MessageBox.Show("Используйте русскую раскладку для ввода данных.");
                    hasShownMessage = true;
                }
                e.Handled = true;
            }
            else
            {
                hasShownMessage = false;
            }
        }

        private bool IsRussianLetter(string input)
        {
            foreach (char c in input)
            {
                if (!((c >= 'А' && c <= 'я') || c == 'ё' || c == 'Ё'))
                {
                    return false;
                }
            }
            return true;
        }


        private void TextBox_PreviewTextInputEmail(object sender, TextCompositionEventArgs e)
        {
            if (!IsEnglishLetterOrSpecialChar(e.Text))
            {
                MessageBox.Show("Введите английские буквы и специальные символы, такие как @, #, $, %, &, и т.д.");
                e.Handled = true;
            }
        }

        private bool IsEnglishLetterOrSpecialChar(string input)
        {
            foreach (char c in input)
            {
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') ||
                      (c == '@' || c == '.' || c == '-' || c == '_' || c == '#' || c == '$' || c == '%' || c == '&')))
                {
                    return false;
                }
            }
            return true;
        }

        private void Email_LostFocus(object sender, RoutedEventArgs e)
        {
            string email = Email.Text.Trim();

            if (!email.Contains("@"))
            {
                MessageBox.Show("Пожалуйста, укажите домен почты (например, @gmail.com или @mail.ru).");
                return;
            }

            if (!(email.EndsWith("@gmail.com") || email.EndsWith("@mail.ru")))
            {
                MessageBox.Show("Введите корректный домен почты: @gmail.com или @mail.ru.");
            }
        }
        //private void ReturnToAppointmentButton_Click(object sender, RoutedEventArgs e)
        //{
        //    //NavigationService.Navigate(new AddAppointmentPage(username));
        //}

    }
}
