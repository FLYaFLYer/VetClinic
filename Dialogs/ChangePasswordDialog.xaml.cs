using VetClinic.Data;
using VetClinic.Models;
using VetClinic.Utils;
using System;
using System.ComponentModel;
using System.Windows;

namespace VetClinic.Dialogs
{
    public partial class ChangePasswordDialog : Window, IDataErrorInfo
    {
        private readonly User _user;
        private readonly VeterContext _context = new VeterContext();

        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

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

            DataContext = this;
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(NewPassword):
                        if (string.IsNullOrWhiteSpace(NewPassword))
                            error = "Введите новый пароль";
                        else if (NewPassword.Length < 6)
                            error = "Пароль должен содержать минимум 6 символов";
                        else if (NewPassword.Length > 64)
                            error = "Пароль не должен превышать 64 символа";
                        break;

                    case nameof(ConfirmPassword):
                        if (string.IsNullOrWhiteSpace(ConfirmPassword))
                            error = "Подтвердите пароль";
                        else if (NewPassword != ConfirmPassword)
                            error = "Пароли не совпадают";
                        break;
                }

                return error;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                MessageBox.Show("Введите новый пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                pwdNewPassword.Focus();
                return;
            }

            if (NewPassword.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                pwdNewPassword.Focus();
                pwdNewPassword.SelectAll();
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                pwdConfirmPassword.Focus();
                pwdConfirmPassword.SelectAll();
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
                        currentUser.Password = SecurityHelper.HashPassword(NewPassword);
                        _context.SaveChanges();
                        MessageBox.Show("Пароль успешно изменен", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                    }
                }
                else
                {
                    // Смена пароля для другого пользователя (только админ)
                    if (App.CurrentRole != App.AdminRole)
                    {
                        MessageBox.Show("Только администратор может менять пароли других пользователей",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var userToUpdate = _context.Users.Find(_user.Id);
                    if (userToUpdate != null)
                    {
                        userToUpdate.Password = SecurityHelper.HashPassword(NewPassword);
                        _context.SaveChanges();
                        MessageBox.Show("Пароль успешно изменен", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при смене пароля: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void PwdNewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            NewPassword = pwdNewPassword.Password;
        }

        private void PwdConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ConfirmPassword = pwdConfirmPassword.Password;
        }
    }
}