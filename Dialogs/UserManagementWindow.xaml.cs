using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
                var users = _context.Users
                    .Include(u => u.Role)
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToList();

                dataGrid.ItemsSource = users;
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

                    MessageBox.Show("Пользователь успешно добавлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (DbEntityValidationException ex)
                {
                    string errorMessage = "Ошибки валидации при сохранении:\n";
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            errorMessage += $"- {ve.PropertyName}: {ve.ErrorMessage}\n";
                        }
                    }
                    MessageBox.Show(errorMessage, "Ошибка валидации",
                        MessageBoxButton.OK, MessageBoxImage.Error);
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
                        // Находим пользователя в контексте
                        var userToUpdate = _context.Users.Find(selectedUser.Id);
                        if (userToUpdate != null)
                        {
                            // Обновляем свойства
                            userToUpdate.LastName = dialog.EditedUser.LastName;
                            userToUpdate.FirstName = dialog.EditedUser.FirstName;
                            userToUpdate.MiddleName = dialog.EditedUser.MiddleName;
                            userToUpdate.PhoneNumber = dialog.EditedUser.PhoneNumber;
                            userToUpdate.DateOfBirth = dialog.EditedUser.DateOfBirth;
                            userToUpdate.DateOfHire = dialog.EditedUser.DateOfHire;
                            userToUpdate.RoleId = dialog.EditedUser.RoleId;

                            // Обновляем пароль только если он был изменен
                            if (!string.IsNullOrEmpty(dialog.EditedUser.Password) &&
                                dialog.EditedUser.Password != selectedUser.Password)
                            {
                                userToUpdate.Password = dialog.EditedUser.Password;
                            }

                            _context.SaveChanges();
                            LoadUsers();

                            MessageBox.Show("Пользователь успешно обновлен!", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        string errorMessage = "Ошибки валидации при обновлении:\n";
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            foreach (var ve in eve.ValidationErrors)
                            {
                                errorMessage += $"- {ve.PropertyName}: {ve.ErrorMessage}\n";
                            }
                        }
                        MessageBox.Show(errorMessage, "Ошибка валидации",
                            MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Выберите пользователя для редактирования");
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is User selectedUser)
            {
                // Проверяем, есть ли у пользователя связанные визиты
                bool hasVisits = _context.Visits.Any(v => v.UserId == selectedUser.Id);
                if (hasVisits)
                {
                    MessageBox.Show("Нельзя удалить пользователя, у которого есть приёмы. Сначала удалите или переназначьте приёмы.",
                                   "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (selectedUser.Role?.Name == App.AdminRole)
                {
                    int adminCount = _context.Users.Count(u => u.Role.Name == App.AdminRole);
                    if (adminCount <= 1)
                    {
                        MessageBox.Show("Нельзя удалить последнего администратора!",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (MessageBox.Show($"Вы уверены, что хотите удалить пользователя:\n\n{selectedUser.FullName}\nЛогин: {selectedUser.Login}",
                                   "Подтверждение удаления",
                                   MessageBoxButton.YesNo,
                                   MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Загружаем пользователя снова, чтобы убедиться в отслеживании
                        var userToDelete = _context.Users.Find(selectedUser.Id);
                        if (userToDelete != null)
                        {
                            _context.Users.Remove(userToDelete);
                            _context.SaveChanges();
                            LoadUsers();

                            MessageBox.Show("Пользователь успешно удален!", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = $"Ошибка удаления: {ex.Message}";
                        if (ex.InnerException != null)
                        {
                            errorMessage += $"\n\nВнутренняя ошибка: {ex.InnerException.Message}";
                        }
                        MessageBox.Show(errorMessage, "Ошибка",
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