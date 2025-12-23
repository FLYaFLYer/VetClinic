using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
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

        private string GetFullExceptionDetails(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Тип: {ex.GetType().FullName}");
            sb.AppendLine($"Сообщение: {ex.Message}");

            Exception inner = ex.InnerException;
            int level = 1;
            while (inner != null)
            {
                sb.AppendLine();
                sb.AppendLine($"Внутренняя ошибка #{level}:");
                sb.AppendLine($"Тип: {inner.GetType().FullName}");
                sb.AppendLine($"Сообщение: {inner.Message}");

                // Если это исключение базы данных, попробуем получить больше информации
                if (inner is System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    sb.AppendLine($"Тип исключения БД: DbUpdateException");
                    if (dbEx.Entries != null)
                    {
                        sb.AppendLine($"Количество записей: {dbEx.Entries.Count()}");
                        int entryIndex = 1;
                        foreach (var entry in dbEx.Entries)
                        {
                            sb.AppendLine($"Запись #{entryIndex}:");
                            sb.AppendLine($"  Сущность: {entry.Entity?.GetType()?.Name}");
                            sb.AppendLine($"  Состояние: {entry.State}");
                            entryIndex++;
                        }
                    }
                    else
                    {
                        sb.AppendLine("Записи отсутствуют");
                    }
                }
                else if (inner is System.Data.SqlClient.SqlException sqlEx)
                {
                    sb.AppendLine($"Ошибка SQL Server: Номер ошибки {sqlEx.Number}");
                    sb.AppendLine($"Сообщение SQL: {sqlEx.Message}");
                    sb.AppendLine($"Сервер: {sqlEx.Server}");
                }

                inner = inner.InnerException;
                level++;
            }

            return sb.ToString();
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserEditDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (var newContext = new VeterContext())
                    {
                        bool loginExists = newContext.Users.Any(u => u.Login == dialog.EditedUser.Login);
                        if (loginExists)
                        {
                            MessageBox.Show($"Пользователь с логином '{dialog.EditedUser.Login}' уже существует", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        newContext.Users.Add(dialog.EditedUser);
                        newContext.SaveChanges();
                    }

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
                    string errorDetails = GetFullExceptionDetails(ex);

                    string userMessage = "Ошибка сохранения пользователя.\n\n" +
                        $"Детали ошибки:\n{errorDetails}";

                    MessageBox.Show(userMessage, "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);

                    Console.WriteLine("=== ОШИБКА В BtnAddUser_Click ===");
                    Console.WriteLine(errorDetails);
                    Console.WriteLine("==================================");
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
                        using (var newContext = new VeterContext())
                        {
                            var userToUpdate = newContext.Users.Find(selectedUser.Id);
                            if (userToUpdate != null)
                            {
                                userToUpdate.LastName = dialog.EditedUser.LastName;
                                userToUpdate.FirstName = dialog.EditedUser.FirstName;
                                userToUpdate.MiddleName = dialog.EditedUser.MiddleName;
                                userToUpdate.PhoneNumber = dialog.EditedUser.PhoneNumber;
                                userToUpdate.DateOfBirth = dialog.EditedUser.DateOfBirth;
                                userToUpdate.DateOfHire = dialog.EditedUser.DateOfHire;
                                userToUpdate.RoleId = dialog.EditedUser.RoleId;

                                if (!string.IsNullOrEmpty(dialog.PasswordPlainText))
                                {
                                    userToUpdate.Password = dialog.EditedUser.Password;
                                }

                                newContext.SaveChanges();
                                LoadUsers();

                                MessageBox.Show("Пользователь успешно обновлен!", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                            }
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
                        string errorDetails = GetFullExceptionDetails(ex);

                        string userMessage = "Ошибка обновления пользователя.\n\n" +
                            $"Детали ошибки:\n{errorDetails}";

                        MessageBox.Show(userMessage, "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);

                        Console.WriteLine("=== ОШИБКА В BtnEditUser_Click ===");
                        Console.WriteLine(errorDetails);
                        Console.WriteLine("==================================");
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
                try
                {
                    using (var checkContext = new VeterContext())
                    {
                        bool hasVisits = checkContext.Visits.Any(v => v.UserId == selectedUser.Id);
                        bool hasUserNotifications = checkContext.UserNotifications.Any(un => un.UserId == selectedUser.Id);

                        if (hasVisits || hasUserNotifications)
                        {
                            string message = "Нельзя удалить пользователя, у которого есть:";
                            if (hasVisits) message += "\n• Приёмы";
                            if (hasUserNotifications) message += "\n• Уведомления";
                            message += "\n\nСначала удалите или переназначьте связанные записи.";

                            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (selectedUser.Role?.Name == App.AdminRole)
                        {
                            int adminCount = checkContext.Users.Count(u => u.Role.Name == App.AdminRole);
                            if (adminCount <= 1)
                            {
                                MessageBox.Show("Нельзя удалить последнего администратора!",
                                               "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                    }

                    if (MessageBox.Show($"Вы уверены, что хотите удалить пользователя:\n\n{selectedUser.FullName}\nЛогин: {selectedUser.Login}",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            using (var deleteContext = new VeterContext())
                            {
                                var userToDelete = deleteContext.Users.Find(selectedUser.Id);
                                if (userToDelete != null)
                                {
                                    deleteContext.Users.Remove(userToDelete);
                                    deleteContext.SaveChanges();
                                    LoadUsers();

                                    MessageBox.Show("Пользователь успешно удален!", "Успех",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Пользователь не найден в базе данных", "Ошибка",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string errorDetails = GetFullExceptionDetails(ex);

                            string userMessage = "Ошибка удаления пользователя.\n\n" +
                                "Возможные причины:\n" +
                                "1. Пользователь связан с другими записями в базе данных\n" +
                                "2. Нарушение ограничений внешних ключей\n" +
                                "3. Проблемы с правами доступа к БД\n\n" +
                                $"Детали ошибки:\n{errorDetails}";

                            MessageBox.Show(userMessage, "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);

                            Console.WriteLine("=== ОШИБКА В BtnDeleteUser_Click (удаление) ===");
                            Console.WriteLine(errorDetails);
                            Console.WriteLine("==============================================");
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errorDetails = GetFullExceptionDetails(ex);

                    MessageBox.Show($"Ошибка при проверке пользователя:\n\n{errorDetails}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
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