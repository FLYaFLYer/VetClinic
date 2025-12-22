using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VetClinic.Data;
using VetClinic.Models;

namespace VetClinic
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Button _currentNavButton;
        private int _unreadCount;
        private readonly VeterContext _context = new VeterContext();

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

        private void LoadNotificationsCount()
        {
            try
            {
                UnreadCount = _context.Notifications.Count(n => !n.IsRead);
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
            dialog.Closed += (s, args) => LoadNotificationsCount();
            dialog.ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentRole = null;
            App.CurrentUser = null;

            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _context?.Dispose();
        }
    }
}