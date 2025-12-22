using System.Windows;
using System.Windows.Controls;
using VetClinic.Pages;

namespace VetClinic
{
    public partial class MainWindow : Window
    {
        private Button _currentNavButton;

        public MainWindow()
        {
            InitializeComponent();
            LoadUserInfo();
            SetupNavigationBasedOnRole();

            // Устанавливаем первую вкладку как активную
            SetActiveNavButton(btnPatients);
            MainFrame.Navigate(new PatientsPage());
        }

        private void LoadUserInfo()
        {
            if (App.CurrentUser != null)
            {
                tbUserInfo.Text = $"{App.CurrentUser.FullName} ({App.CurrentRole})";
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
                    page = new PatientsPage();
                    break;
                case "Medicines":
                    page = new MedicinesPage();
                    break;
                case "Visits":
                    page = new VisitsPage();
                    break;
                case "Owners":
                    page = new OwnersPage();
                    break;
                case "Reports":
                    page = new ReportsPage();
                    break;
                default:
                    page = new PatientsPage();
                    break;
            }

            MainFrame.Navigate(page);
        }

        private void BtnNotifications_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Dialogs.NotificationsDialog();
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
    }
}