using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VetClinic.Data;
using VetClinic.Models;
using VetClinic.Utils;

namespace VetClinic
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Button _currentNavButton;
        private int _unreadCount;
        private readonly System.Threading.Timer _notificationTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        public int UnreadCount
        {
            get => _unreadCount;
            set
            {
                _unreadCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnreadCount)));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadUserInfo();
            SetupNavigationBasedOnRole();
            LoadNotificationsCount();

            // Запускаем таймер для обновления уведомлений каждые 10 секунд
            _notificationTimer = new System.Threading.Timer(
                _ => UpdateNotifications(),
                null,
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10));

            // Устанавливаем первую вкладку как активную
            SetActiveNavButton(btnPatients);
            MainFrame.Navigate(new Pages.PatientsPage());
        }

        private void LoadUserInfo()
        {
            if (App.CurrentUser != null)
            {
                tbUserInfo.Text = $"{App.CurrentUser.FullName} ({App.CurrentRole})";
            }
        }

        private void UpdateNotifications()
        {
            Dispatcher.Invoke(() =>
            {
                LoadNotificationsCount();
            });
        }

        // ИЗМЕНЕНИЕ: Сделать метод public вместо private
        public void LoadNotificationsCount()
        {
            try
            {
                using (var context = new VeterContext())
                {
                    // Получаем ID всех уведомлений
                    var allNotificationIds = context.Notifications
                        .Select(n => n.Id)
                        .ToList();

                    // Получаем ID прочитанных уведомлений для текущего пользователя
                    var readNotificationIds = context.UserNotifications
                        .Where(un => un.UserId == App.CurrentUser.Id && un.IsRead)
                        .Select(un => un.NotificationId)
                        .ToList();

                    // Считаем непрочитанные уведомления
                    int unreadCount = allNotificationIds.Count(id => !readNotificationIds.Contains(id));

                    UnreadCount = unreadCount;

                    btnNotifications.ToolTip = UnreadCount > 0
                        ? $"Уведомлений: {UnreadCount} непрочитанных"
                        : "Нет новых уведомлений";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки уведомлений: {ex.Message}");
                UnreadCount = 0;
            }
        }

        private void SetupNavigationBasedOnRole()
        {
            btnUsers.Visibility = App.CurrentRole == App.AdminRole ?
                Visibility.Visible : Visibility.Collapsed;
        }

        private void SetActiveNavButton(Button button)
        {
            // Сбрасываем стиль предыдущей активной кнопки
            if (_currentNavButton != null)
            {
                _currentNavButton.Style = (Style)FindResource("SidebarButton");
            }

            // Устанавливаем новый стиль для активной кнопки
            if (button != null)
            {
                button.Style = (Style)FindResource("SidebarButtonActive");
                _currentNavButton = button;
            }
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string pageName = button.Name.Replace("btn", "");

            if (pageName == "Users")
            {
                SetActiveNavButton(button);
                var userManagement = new Dialogs.UserManagementWindow();
                userManagement.ShowDialog();
                // После закрытия диалога возвращаем выделение на предыдущую страницу
                if (_currentNavButton == button)
                {
                    SetActiveNavButton(btnPatients);
                }
                return;
            }

            // Устанавливаем активную кнопку
            SetActiveNavButton(button);

            System.Windows.Controls.Page page = null;

            switch (pageName)
            {
                case "Patients":
                    page = new Pages.PatientsPage();
                    break;
                case "Medicines":
                    page = new Pages.MedicinesPage();
                    break;
                case "Visits":
                    page = new Pages.VisitsPage();
                    break;
                case "Owners":
                    page = new Pages.OwnersPage();
                    break;
                case "Reports":
                    page = new Pages.ReportsPage();
                    break;
                default:
                    page = new Pages.PatientsPage();
                    break;
            }

            MainFrame.Navigate(page);
        }

        private void BtnNotifications_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Dialogs.NotificationsDialog();
            dialog.Owner = this;
            dialog.Closed += (s, args) =>
            {
                LoadNotificationsCount(); // Обновляем счетчик после закрытия окна
            };
            dialog.ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            _notificationTimer?.Dispose();
            App.CurrentRole = null;
            App.CurrentUser = null;

            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void BtnAutoRefresh_Click(object sender, RoutedEventArgs e)
        {
            AutoRefreshHelper.AutoRefreshEnabled = !AutoRefreshHelper.AutoRefreshEnabled;

            if (AutoRefreshHelper.AutoRefreshEnabled)
            {
                AutoRefreshHelper.StartAutoRefresh(RefreshCurrentPage);
                MessageBox.Show("Автообновление включено (30 сек)", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                AutoRefreshHelper.StopAutoRefresh();
                MessageBox.Show("Автообновление выключено", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RefreshCurrentPage()
        {
            Dispatcher.Invoke(() =>
            {
                // Обновляем текущую страницу
                var currentPage = MainFrame.Content;

                if (currentPage is Pages.PatientsPage patientsPage)
                {
                    patientsPage.LoadData();
                }
                else if (currentPage is Pages.MedicinesPage medicinesPage)
                {
                    medicinesPage.LoadData();
                }
                else if (currentPage is Pages.VisitsPage visitsPage)
                {
                    visitsPage.LoadData(null);
                }
                else if (currentPage is Pages.OwnersPage ownersPage)
                {
                    ownersPage.LoadData();
                }
                else if (currentPage is Pages.ReportsPage reportsPage)
                {
                    // Для ReportsPage нет метода LoadData, просто обновим
                    reportsPage = new Pages.ReportsPage();
                    MainFrame.Navigate(reportsPage);
                }
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _notificationTimer?.Dispose();
            AutoRefreshHelper.StopAutoRefresh();
        }
    }
}