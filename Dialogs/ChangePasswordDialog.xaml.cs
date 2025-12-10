using VetClinic.Data;
using VetClinic.Models;
using VetClinic.Utils;
using System.Windows;

namespace VetClinic.Dialogs
{
    public partial class ChangePasswordDialog : Window
    {
        private readonly User _user;
        private readonly VeterContext _context = new VeterContext();

        public ChangePasswordDialog(User user = null)
        {
            InitializeComponent();
            _user = user;

            if (_user != null)
            {
                txtUserName.Text = _user.FullName;
            }
            else
            {
                txtUserName.Text = "Текущий пользователь";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = pwdNewPassword.Password;
            string confirmPassword = pwdConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Введите новый пароль");
                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }

            try
            {
                if (_user == null)
                {
                    // Смена пароля для текущего пользователя
                    var currentUser = _context.Users.Find(App.CurrentUser.Id);
                    if (currentUser != null)
                    {
                        currentUser.Password = SecurityHelper.HashPassword(newPassword);
                        _context.SaveChanges();
                        MessageBox.Show("Пароль успешно изменен");
                        DialogResult = true;
                    }
                }
                else
                {
                    // Смена пароля для другого пользователя (только админ)
                    if (App.CurrentRole != App.AdminRole)
                    {
                        MessageBox.Show("Только администратор может менять пароли других пользователей");
                        return;
                    }

                    var userToUpdate = _context.Users.Find(_user.Id);
                    if (userToUpdate != null)
                    {
                        userToUpdate.Password = SecurityHelper.HashPassword(newPassword);
                        _context.SaveChanges();
                        MessageBox.Show("Пароль успешно изменен");
                        DialogResult = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при смене пароля: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}