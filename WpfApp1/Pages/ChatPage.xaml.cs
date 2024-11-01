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
        private static VetClinica1Entities db = new VetClinica1Entities();
        private string userRole; // Роль пользователя, может быть "Veterinarian" или "Owner"
        private int? userId; // ID авторизованного пользователя
        private string loggedInUsername;
        private Messages selectedMessage;

        public ChatPage(string userRole, int userId, string login)
        {
            InitializeComponent();
            messages = new ObservableCollection<Messages>();
            MessagesListBox.ItemsSource = messages;

            this.userRole = userRole; // Сохраняем роль пользователя
            this.userId = userId; // Сохраняем ID пользователя
            loggedInUsername = login; // Сохраняем имя пользователя
            AuthenticateUser(login);
            SetupPage();
        }

        private void AuthenticateUser(string login)
        {
            var owner = db.Owner.FirstOrDefault(o => o.login == login);
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
        }

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

        private void LoadClients()
        {
            UserComboBox.ItemsSource = db.Owner.ToList();
            UserComboBox.DisplayMemberPath = "first_name";
            UserComboBox.SelectedValuePath = "owner_id";
        }

        private void LoadVeterinarians()
        {
            UserComboBox.ItemsSource = db.Veterenarian.ToList();
            UserComboBox.DisplayMemberPath = "first_name";
            UserComboBox.SelectedValuePath = "veterenarian_id";
        }

        private void UserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedReceiverId = (int?)UserComboBox.SelectedValue;
            LoadMessages();
        }

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

            if (selectedMessage != null) // Проверяем, редактируем ли мы сообщение
            {
                // Проверяем, является ли текущий пользователь отправителем сообщения
                if (selectedMessage.SenderId == userId)
                {
                    selectedMessage.Content = content; // Обновляем содержимое сообщения
                    db.SaveChanges(); // Сохраняем изменения в базе данных
                    MessageTextBox.Clear(); // Очищаем текстовое поле
                    selectedMessage = null; // Сбрасываем выбранное сообщение
                }
                else
                {
                    MessageBox.Show("Вы не можете редактировать это сообщение.");
                }
            }
            else
            {

                var newMessage = new Messages
                {
                    SenderId = (int)userId,
                    ReceiverId = (int)selectedReceiverId,
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
            LoadMessages();
        }

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

        // Метод для удаления конкретного сообщения
        private void DeleteMessage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int MessageId) // Получаем ID сообщения из кнопки
            {
                var messageToDelete = db.Messages.Find(MessageId); // Ищем сообщение в базе данных
                if (messageToDelete != null)
                {
                    // Проверяем, является ли текущий пользователь отправителем сообщения
                    if (messageToDelete.SenderId == userId)
                    {
                        db.Messages.Remove(messageToDelete); // Удаляем сообщение
                        db.SaveChanges(); // Сохраняем изменения
                        messages.Remove(messageToDelete); // Удаляем сообщение из коллекции
                    }
                    else
                    {
                        MessageBox.Show("Вы не можете удалить это сообщение.");
                    }
                }
            }
        }


        // Метод для редактирования сообщения
        private void EditMessage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int MessageId) // Получаем ID сообщения из кнопки
            {
                var messageToEdit = db.Messages.Find(MessageId); // Ищем сообщение в базе данных
                if (messageToEdit != null)
                {
                    // Проверяем, является ли текущий пользователь отправителем сообщения
                    if (messageToEdit.SenderId == userId)
                    {
                        selectedMessage = messageToEdit; // Устанавливаем выбранное сообщение для редактирования
                        MessageTextBox.Text = selectedMessage.Content; // Загружаем содержимое сообщения в текстовое поле для редактирования
                       
                    }
                    else
                    {
                        MessageBox.Show("Вы не можете редактировать это сообщение.");
                    }
                }
            }
        }




    }
}