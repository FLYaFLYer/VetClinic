using System.Windows;
using System.Windows.Controls;
using VetClinic.Pages;

namespace VetClinic
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadUserInfo();
            SetupNavigationBasedOnRole();
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

            if (App.CurrentRole == App.VetRole)
            {
                // Все кнопки доступны ветеринару
            }
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string pageName = button.Name.Replace("btn", "");

            if (pageName == "Users")
            {
                var userManagement = new Dialogs.UserManagementWindow();
                userManagement.ShowDialog();
                return;
            }

            System.Windows.Controls.Page page = null;

            if (pageName == "Patients")
                page = new PatientsPage();
            else if (pageName == "Medicines")
                page = new MedicinesPage();
            else if (pageName == "Visits")
                page = new VisitsPage();
            else if (pageName == "Owners")
                page = new OwnersPage();
            else if (pageName == "Reports")
                page = new ReportsPage();
            else
                page = new PatientsPage();

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