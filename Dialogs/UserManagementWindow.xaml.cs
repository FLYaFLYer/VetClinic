using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using VetClinic.Data;
using VetClinic.Models;

namespace VetClinic.Dialogs
{
    public partial class UserManagementWindow : Window
    {
        private readonly VeterContext _context = new VeterContext();

        public UserManagementWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                _context.Users
                    .Include(u => u.Role)
                    .Load();
                dataGrid.ItemsSource = _context.Users.Local;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserEditDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _context.Users.Add(dialog.EditedUser);
                    _context.SaveChanges();
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is User selectedUser)
            {
                var dialog = new UserEditDialog(selectedUser);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        _context.SaveChanges();
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя");
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is User selectedUser)
            {
                if (selectedUser.Role.Name == App.AdminRole)
                {
                    int adminCount = _context.Users.Count(u => u.Role.Name == App.AdminRole);
                    if (adminCount <= 1)
                    {
                        MessageBox.Show("Нельзя удалить последнего администратора!",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (MessageBox.Show($"Удалить пользователя {selectedUser.FullName}?",
                                   "Подтверждение",
                                   MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Users.Remove(selectedUser);
                        _context.SaveChanges();
                        LoadUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is User selectedUser)
            {
                var dialog = new ChangePasswordDialog(selectedUser);
                dialog.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите пользователя");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
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