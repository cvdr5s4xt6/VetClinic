using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для ChatPage.xaml
    /// </summary>
    public partial class ChatPage : Page
    {
        private ObservableCollection<Messages> messages;
        private int? selectedReceiverId;
        private static VetClinicaEntities db = new VetClinicaEntities();
        private string userRole; // Роль пользователя, может быть "Veterinarian" или "Owner"
        private int? userId; // ID авторизованного пользователя

        public ChatPage(string userRole, int userId, string login)
        {
            InitializeComponent();
            messages = new ObservableCollection<Messages>();
            MessagesListBox.ItemsSource = messages;

            AuthenticateUser(login);
            SetupPage();
        }

        // Метод для аутентификации пользователя
        private void AuthenticateUser(string login)
        {
            var owner = db.Owner.FirstOrDefault(o => o.login == login );
            if (owner != null)
            {
                userRole = "Owner";
                userId = owner.owner_id;
                return;
            }

            var veterinarian = db.Veterenarian.FirstOrDefault(v => v.login == login);
            if (veterinarian != null)
            {
                userRole = "Veterinarian";
                userId = veterinarian.veterenarian_id;
                return;
            }

            MessageBox.Show("Неверный логин или пароль.");
            // Закрыть или перенаправить пользователя при ошибке аутентификации
        }

        // Настройка страницы в зависимости от роли пользователя
        private void SetupPage()
        {
            if (userRole == "Owner")
            {
                LoadVeterinarians();
            }
            else if (userRole == "Veterinarian")
            {
                LoadClients();
            }
        }

        // Метод для загрузки клиентов для выбора в ComboBox
        private void LoadClients()
        {
            UserComboBox.ItemsSource = db.Owner.ToList();
            UserComboBox.DisplayMemberPath = "first_name"; // Пример отображения имени клиента
            UserComboBox.SelectedValuePath = "owner_id";
        }

        // Метод для загрузки ветеринаров для выбора в ComboBox
        private void LoadVeterinarians()
        {
            UserComboBox.ItemsSource = db.Veterenarian.ToList();
            UserComboBox.DisplayMemberPath = "first_name"; // Пример отображения имени ветеринара
            UserComboBox.SelectedValuePath = "veterenarian_id";
        }

        // Обработчик выбора собеседника
        private void UserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedReceiverId = (int?)UserComboBox.SelectedValue;
            LoadMessages();
        }

        // Метод для загрузки сообщений
        private void LoadMessages()
        {
            if (selectedReceiverId.HasValue)
            {
                var chatMessages = db.Messages
                    .Where(m => (m.SenderId == userId && m.ReceiverId == selectedReceiverId) ||
                                (m.SenderId == selectedReceiverId && m.ReceiverId == userId))
                    .OrderBy(m => m.Timestamp)
                    .ToList();

                messages.Clear();
                foreach (var msg in chatMessages)
                {
                    messages.Add(msg);
                }
            }
        }

        // Отправка сообщения
        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            if (!selectedReceiverId.HasValue)
            {
                MessageBox.Show("Выберите собеседника.");
                return;
            }

            var content = MessageTextBox.Text;
            if (string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Нельзя отправить пустое сообщение.");
                return;
            }

            var newMessage = new Messages
            {
                SenderId = userId,
                ReceiverId = selectedReceiverId,
                Content = content,
                Timestamp = DateTime.Now,
                SenderRole = userRole,
                ReceiverRole = userRole == "Owner" ? "Veterinarian" : "Owner"
            };

            db.Messages.Add(newMessage);
            db.SaveChanges();
            messages.Add(newMessage);

            MessageTextBox.Clear();
        }

        // Удаление истории чата
        private void DeleteChatHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!selectedReceiverId.HasValue)
            {
                MessageBox.Show("Выберите собеседника.");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить историю чата?", "Удаление", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var chatMessages = db.Messages
                    .Where(m => (m.SenderId == userId && m.ReceiverId == selectedReceiverId) ||
                                (m.SenderId == selectedReceiverId && m.ReceiverId == userId))
                    .ToList();

                db.Messages.RemoveRange(chatMessages);
                db.SaveChanges();
                messages.Clear();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}