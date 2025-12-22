using VetClinic.Data;
using VetClinic.Models;
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
                _context.Notifications
                    .Include(n => n.Medicine)
                    .OrderByDescending(n => n.CreatedDate)
                    .Load();

                dataGrid.ItemsSource = _context.Notifications.Local;

                // Обновляем счетчик
                int unreadCount = _context.Notifications.Local.Count(n => !n.IsRead);
                int totalCount = _context.Notifications.Local.Count;

                tbNotificationCount.Text = $"Уведомлений: {totalCount} (новых: {unreadCount})";
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки уведомлений: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnMarkAsRead_Click(object sender, RoutedEventArgs e)
        {
            var unreadNotifications = _context.Notifications.Local.Where(n => !n.IsRead).ToList();

            if (unreadNotifications.Count == 0)
            {
                MessageBox.Show("Нет непрочитанных уведомлений",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Пометить все {unreadNotifications.Count} непрочитанных уведомлений как прочитанные?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                }

                _context.SaveChanges();
                LoadNotifications();

                MessageBox.Show("Все уведомления помечены как прочитанные",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            _context?.Dispose();
        }
    }
}