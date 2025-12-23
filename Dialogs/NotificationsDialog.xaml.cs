using VetClinic.Data;
using VetClinic.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace VetClinic.Dialogs
{
    public partial class NotificationsDialog : Window
    {
        private readonly VeterContext _context = new VeterContext();

        public NotificationsDialog()
        {
            InitializeComponent();
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            try
            {
                // Загружаем все уведомления
                var notifications = _context.Notifications
                    .Include(n => n.Medicine)
                    .OrderByDescending(n => n.CreatedDate)
                    .ToList();

                // Получаем прочитанные уведомления для текущего пользователя
                var readNotifications = _context.UserNotifications
                    .Where(un => un.UserId == App.CurrentUser.Id && un.IsRead)
                    .Select(un => un.NotificationId)
                    .ToList();

                // Помечаем уведомления как прочитанные/непрочитанные
                foreach (var notification in notifications)
                {
                    notification.IsRead = readNotifications.Contains(notification.Id);
                }

                dataGrid.ItemsSource = notifications;

                // Обновляем счетчик
                int unreadCount = notifications.Count(n => !n.IsRead);
                int totalCount = notifications.Count;

                tbNotificationCount.Text = $"Уведомлений: {totalCount} (новых: {unreadCount})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки уведомлений: {ex.InnerException?.Message ?? ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MarkNotificationAsRead(int notificationId)
        {
            try
            {
                // Проверяем, есть ли уже запись
                var userNotification = _context.UserNotifications
                    .FirstOrDefault(un => un.UserId == App.CurrentUser.Id &&
                                         un.NotificationId == notificationId);

                if (userNotification != null)
                {
                    // Обновляем существующую запись
                    userNotification.IsRead = true;
                    userNotification.ReadDate = DateTime.Now;
                }
                else
                {
                    // Создаем новую запись
                    userNotification = new UserNotification
                    {
                        UserId = App.CurrentUser.Id,
                        NotificationId = notificationId,
                        IsRead = true,
                        ReadDate = DateTime.Now
                    };
                    _context.UserNotifications.Add(userNotification);
                }

                _context.SaveChanges();

                // Обновляем отображение
                var notification = (dataGrid.ItemsSource as System.Collections.IEnumerable)
                    .Cast<Notification>()
                    .FirstOrDefault(n => n.Id == notificationId);

                if (notification != null)
                {
                    notification.IsRead = true;
                    dataGrid.Items.Refresh();
                }

                // Обновляем счетчик
                int unreadCount = (dataGrid.ItemsSource as System.Collections.IEnumerable)
                    .Cast<Notification>()
                    .Count(n => !n.IsRead);
                int totalCount = (dataGrid.ItemsSource as System.Collections.IEnumerable)
                    .Cast<Notification>()
                    .Count();

                tbNotificationCount.Text = $"Уведомлений: {totalCount} (новых: {unreadCount})";

                // Обновляем счетчик в главном окне
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.LoadNotificationsCount(); // Теперь метод public
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отметке уведомления: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MarkAllNotificationsAsRead()
        {
            try
            {
                var notifications = (dataGrid.ItemsSource as System.Collections.IEnumerable)
                    .Cast<Notification>()
                    .Where(n => !n.IsRead)
                    .ToList();

                foreach (var notification in notifications)
                {
                    var userNotification = _context.UserNotifications
                        .FirstOrDefault(un => un.UserId == App.CurrentUser.Id &&
                                             un.NotificationId == notification.Id);

                    if (userNotification != null)
                    {
                        userNotification.IsRead = true;
                        userNotification.ReadDate = DateTime.Now;
                    }
                    else
                    {
                        userNotification = new UserNotification
                        {
                            UserId = App.CurrentUser.Id,
                            NotificationId = notification.Id,
                            IsRead = true,
                            ReadDate = DateTime.Now
                        };
                        _context.UserNotifications.Add(userNotification);
                    }
                }

                _context.SaveChanges();

                // Обновляем отображение
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }
                dataGrid.Items.Refresh();

                // Обновляем счетчик
                tbNotificationCount.Text = $"Уведомлений: {notifications.Count} (новых: 0)";

                // Обновляем счетчик в главном окне
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.LoadNotificationsCount(); // Теперь метод public
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отметке всех уведомлений: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnMarkAsRead_Click(object sender, RoutedEventArgs e)
        {
            MarkAllNotificationsAsRead();
            MessageBox.Show("Все уведомления отмечены как прочитанные",
                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnMarkSelectedAsRead_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Notification selectedNotification)
            {
                if (!selectedNotification.IsRead)
                {
                    MarkNotificationAsRead(selectedNotification.Id);
                    MessageBox.Show("Уведомление отмечено как прочитанное",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Уведомление уже прочитано",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Выберите уведомление",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem is Notification selectedNotification)
            {
                if (!selectedNotification.IsRead)
                {
                    MarkNotificationAsRead(selectedNotification.Id);
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadNotifications();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _context?.Dispose();
        }
    }
}