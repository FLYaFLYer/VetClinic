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
            _context.Notifications
                .Include(n => n.Medicine)
                .OrderByDescending(n => n.CreatedDate)
                .Load();

            dataGrid.ItemsSource = _context.Notifications.Local;
        }

        private void BtnMarkAsRead_Click(object sender, RoutedEventArgs e)
        {
            foreach (var notification in _context.Notifications.Local.Where(n => !n.IsRead))
            {
                notification.IsRead = true;
            }
            _context.SaveChanges();
            LoadNotifications();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}