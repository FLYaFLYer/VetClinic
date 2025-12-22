using VetClinic.Data;
using VetClinic.Models;
using VetClinic.Utils;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace VetClinic.Dialogs
{
    public partial class UserEditDialog : Window, IDataErrorInfo
    {
        private readonly VeterContext _context = new VeterContext();

        public User EditedUser { get; private set; }
        public string PasswordPlainText { get; set; }

        public string Login { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfHire { get; set; }
        public int? RoleId { get; set; }

        public UserEditDialog(User user = null)
        {
            InitializeComponent();

            _context.Roles.Load();
            cmbRoles.ItemsSource = _context.Roles.Local;

            if (user != null)
            {
                EditedUser = user;
                Login = user.Login;
                LastName = user.LastName;
                FirstName = user.FirstName;
                MiddleName = user.MiddleName;
                PhoneNumber = FormatPhoneNumber(user.PhoneNumber);
                DateOfBirth = user.DateOfBirth;
                DateOfHire = user.DateOfHire;
                RoleId = user.RoleId;

                txtLogin.IsEnabled = false;
                PasswordPlainText = "";
            }
            else
            {
                EditedUser = new User
                {
                    DateOfBirth = DateTime.Now.AddYears(-20),
                    DateOfHire = DateTime.Now,
                    RoleId = 2
                };

                Login = "";
                LastName = "";
                FirstName = "";
                MiddleName = "";
                PhoneNumber = "";
                DateOfBirth = EditedUser.DateOfBirth;
                DateOfHire = EditedUser.DateOfHire;
                RoleId = EditedUser.RoleId;
            }

            DataContext = this;
        }

        private string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            string digits = Regex.Replace(phone, @"[^\d]", "");

            if (digits.Length == 11 && digits.StartsWith("7"))
            {
                return $"+7 ({digits.Substring(1, 3)}) {digits.Substring(4, 3)}-{digits.Substring(7, 2)}-{digits.Substring(9, 2)}";
            }
            else if (digits.Length == 10)
            {
                return $"+7 ({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6, 2)}-{digits.Substring(8, 2)}";
            }
            else if (digits.Length == 11 && digits.StartsWith("8"))
            {
                return $"+7 ({digits.Substring(1, 3)}) {digits.Substring(4, 3)}-{digits.Substring(7, 2)}-{digits.Substring(9, 2)}";
            }

            return phone;
        }

        private void TxtPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPhoneNumber.IsFocused)
            {
                string text = txtPhoneNumber.Text;
                int caretIndex = txtPhoneNumber.CaretIndex;

                string formatted = FormatPhoneNumber(text);

                if (formatted != text)
                {
                    txtPhoneNumber.Text = formatted;
                    txtPhoneNumber.CaretIndex = caretIndex + (formatted.Length - text.Length);
                }
            }
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(Login):
                        if (string.IsNullOrWhiteSpace(Login))
                            error = "Логин обязателен";
                        else if (Login.Length > 50)
                            error = "Логин не должен превышать 50 символов";
                        else if (!Regex.IsMatch(Login, @"^[a-zA-Z0-9_]+$"))
                            error = "Логин может содержать только буквы, цифры и подчеркивания";
                        break;

                    case nameof(LastName):
                        if (string.IsNullOrWhiteSpace(LastName))
                            error = "Фамилия обязательна";
                        else if (LastName.Length > 50)
                            error = "Фамилия не должна превышать 50 символов";
                        break;

                    case nameof(FirstName):
                        if (string.IsNullOrWhiteSpace(FirstName))
                            error = "Имя обязательно";
                        else if (FirstName.Length > 50)
                            error = "Имя не должно превышать 50 символов";
                        break;

                    case nameof(MiddleName):
                        if (!string.IsNullOrWhiteSpace(MiddleName) && MiddleName.Length > 50)
                            error = "Отчество не должно превышать 50 символов";
                        break;

                    case nameof(PhoneNumber):
                        if (string.IsNullOrWhiteSpace(PhoneNumber))
                            error = "Телефон обязателен";
                        else
                        {
                            string digits = Regex.Replace(PhoneNumber, @"[^\d]", "");
                            if (digits.Length < 10 || digits.Length > 15)
                                error = "Введите корректный номер телефона (10-15 цифр)";
                        }
                        break;

                    case nameof(DateOfBirth):
                        if (!DateOfBirth.HasValue)
                            error = "Дата рождения обязательна";
                        else if (DateOfBirth > DateTime.Now)
                            error = "Дата рождения не может быть в будущем";
                        else if (DateOfBirth < DateTime.Now.AddYears(-100))
                            error = "Некорректная дата рождения";
                        break;

                    case nameof(DateOfHire):
                        if (!DateOfHire.HasValue)
                            error = "Дата приема обязательна";
                        else if (DateOfHire > DateTime.Now)
                            error = "Дата приема не может быть в будущем";
                        else if (DateOfBirth.HasValue && DateOfHire < DateOfBirth)
                            error = "Дата приема не может быть раньше даты рождения";
                        break;

                    case nameof(RoleId):
                        if (!RoleId.HasValue || RoleId <= 0)
                            error = "Выберите роль пользователя";
                        break;
                }

                return error;
            }
        }

        private void GenerateAndSetPassword()
        {
            var randomPassword = SecurityHelper.GenerateRandomPassword(8);
            PasswordPlainText = randomPassword;
            EditedUser.Password = SecurityHelper.HashPassword(randomPassword);

            pwdPassword.Password = PasswordPlainText;
            txtPasswordVisible.Text = PasswordPlainText;

            MessageBox.Show($"Пароль пользователя: {randomPassword}\n\nСкопируйте его для передачи пользователю!",
                          "Пароль сгенерирован", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PwdPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlainText = pwdPassword.Password;
            if (!string.IsNullOrEmpty(PasswordPlainText))
            {
                EditedUser.Password = SecurityHelper.HashPassword(PasswordPlainText);
            }
        }

        private void TxtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPasswordVisible.Text))
            {
                PasswordPlainText = txtPasswordVisible.Text;
                EditedUser.Password = SecurityHelper.HashPassword(PasswordPlainText);
            }
        }

        private void ChkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            txtPasswordVisible.Visibility = Visibility.Visible;
            pwdPassword.Visibility = Visibility.Collapsed;
            txtPasswordVisible.Text = PasswordPlainText;
        }

        private void ChkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            pwdPassword.Visibility = Visibility.Visible;
            txtPasswordVisible.Visibility = Visibility.Collapsed;
            pwdPassword.Password = PasswordPlainText;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Принудительно обновляем привязки
            txtLogin.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtLastName.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtFirstName.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtPhoneNumber.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            dpDateOfBirth.GetBindingExpression(DatePicker.SelectedDateProperty)?.UpdateSource();
            dpDateOfHire.GetBindingExpression(DatePicker.SelectedDateProperty)?.UpdateSource();
            cmbRoles.GetBindingExpression(ComboBox.SelectedValueProperty)?.UpdateSource();

            // Проверяем ошибки валидации
            if (Validation.GetHasError(txtLogin) ||
                Validation.GetHasError(txtLastName) ||
                Validation.GetHasError(txtFirstName) ||
                Validation.GetHasError(txtPhoneNumber) ||
                Validation.GetHasError(dpDateOfBirth) ||
                Validation.GetHasError(dpDateOfHire) ||
                Validation.GetHasError(cmbRoles))
            {
                MessageBox.Show("Исправьте ошибки в форме перед сохранением",
                              "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EditedUser.Id == 0 && string.IsNullOrEmpty(PasswordPlainText))
            {
                MessageBox.Show("Введите пароль или сгенерируйте его", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                pwdPassword.Focus();
                return;
            }

            try
            {
                EditedUser.Login = Login;
                EditedUser.LastName = LastName;
                EditedUser.FirstName = FirstName;
                EditedUser.MiddleName = MiddleName;
                EditedUser.PhoneNumber = FormatPhoneNumber(PhoneNumber);
                EditedUser.DateOfBirth = DateOfBirth.Value;
                EditedUser.DateOfHire = DateOfHire.Value;
                EditedUser.RoleId = RoleId.Value;

                if (EditedUser.Id == 0)
                {
                    _context.Users.Add(EditedUser);
                }
                else
                {
                    var existingUser = _context.Users.Find(EditedUser.Id);
                    if (existingUser != null)
                    {
                        existingUser.LastName = EditedUser.LastName;
                        existingUser.FirstName = EditedUser.FirstName;
                        existingUser.MiddleName = EditedUser.MiddleName;
                        existingUser.PhoneNumber = EditedUser.PhoneNumber;
                        existingUser.DateOfBirth = EditedUser.DateOfBirth;
                        existingUser.DateOfHire = EditedUser.DateOfHire;
                        existingUser.RoleId = EditedUser.RoleId;

                        if (!string.IsNullOrEmpty(PasswordPlainText))
                        {
                            existingUser.Password = EditedUser.Password;
                        }
                    }
                }

                _context.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnGeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            GenerateAndSetPassword();
        }

        private void BtnCopyPassword_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PasswordPlainText))
            {
                try
                {
                    Clipboard.SetText(PasswordPlainText);
                    MessageBox.Show("Пароль скопирован в буфер обмена", "Успешно",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка копирования: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Сначала сгенерируйте пароль", "Предупреждение",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}