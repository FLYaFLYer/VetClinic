using VetClinic.Data;
using VetClinic.Models;
using VetClinic.Utils;
using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace VetClinic.Dialogs
{
    public partial class UserEditDialog : Window
    {
        private readonly VeterContext _context = new VeterContext();

        public User EditedUser { get; private set; }
        public string PasswordPlainText { get; set; }

        // Свойства для привязки
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

            // Загружаем роли
            _context.Roles.Load();
            cmbRoles.ItemsSource = _context.Roles.Local;

            if (user != null)
            {
                // Редактирование существующего пользователя
                EditedUser = user;
                Login = user.Login;
                LastName = user.LastName;
                FirstName = user.FirstName;
                MiddleName = user.MiddleName;
                PhoneNumber = FormatPhoneNumber(user.PhoneNumber);
                DateOfBirth = user.DateOfBirth;
                DateOfHire = user.DateOfHire;
                RoleId = user.RoleId;

                txtLogin.IsEnabled = false; // Не позволяем менять логин при редактировании
                PasswordPlainText = ""; // Пустой пароль, нужно ввести новый
            }
            else
            {
                // Создание нового пользователя
                EditedUser = new User
                {
                    DateOfBirth = DateTime.Now.AddYears(-20),
                    DateOfHire = DateTime.Now,
                    RoleId = 2 // По умолчанию Администратор
                };

                Login = "";
                LastName = "";
                FirstName = "";
                MiddleName = "";
                PhoneNumber = "";
                DateOfBirth = EditedUser.DateOfBirth;
                DateOfHire = EditedUser.DateOfHire;
                RoleId = EditedUser.RoleId;

                // Не генерируем пароль автоматически!
            }

            // Устанавливаем DataContext
            DataContext = this;
        }

        // Метод для форматирования номера телефона
        private string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            // Удаляем все нецифровые символы
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

            // Если номер не соответствует формату, возвращаем как есть
            return phone;
        }

        // Обработчик для форматирования телефона при вводе
        private void TxtPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPhoneNumber.IsFocused)
            {
                string text = txtPhoneNumber.Text;
                int caretIndex = txtPhoneNumber.CaretIndex;

                // Форматируем текст
                string formatted = FormatPhoneNumber(text);

                if (formatted != text)
                {
                    txtPhoneNumber.Text = formatted;
                    // Пытаемся сохранить позицию курсора
                    txtPhoneNumber.CaretIndex = caretIndex + (formatted.Length - text.Length);
                }
            }
        }

        private void GenerateAndSetPassword()
        {
            var randomPassword = SecurityHelper.GenerateRandomPassword(8);
            PasswordPlainText = randomPassword;
            EditedUser.Password = SecurityHelper.HashPassword(randomPassword);

            // Обновляем отображение пароля
            pwdPassword.Password = PasswordPlainText;
            txtPasswordVisible.Text = PasswordPlainText;

            // Показываем пароль пользователю
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
            // Проверка логина
            if (string.IsNullOrWhiteSpace(Login))
            {
                MessageBox.Show("Введите логин пользователя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                txtLogin.Focus();
                return;
            }

            // Проверка фамилии
            if (string.IsNullOrWhiteSpace(LastName))
            {
                MessageBox.Show("Введите фамилию пользователя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                txtLastName.Focus();
                return;
            }

            // Проверка имени
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                MessageBox.Show("Введите имя пользователя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                txtFirstName.Focus();
                return;
            }

            // Проверка телефона
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                MessageBox.Show("Введите телефон пользователя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                txtPhoneNumber.Focus();
                return;
            }

            // Проверка формата телефона
            string phoneDigits = Regex.Replace(PhoneNumber, @"[^\d]", "");
            if (phoneDigits.Length != 11 && phoneDigits.Length != 10)
            {
                MessageBox.Show("Введите корректный номер телефона (10 или 11 цифр)", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                txtPhoneNumber.Focus();
                return;
            }

            // Проверка роли
            if (cmbRoles.SelectedItem == null)
            {
                MessageBox.Show("Выберите роль пользователя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                cmbRoles.Focus();
                return;
            }

            // Проверка пароля для нового пользователя
            if (EditedUser.Id == 0 && string.IsNullOrEmpty(PasswordPlainText))
            {
                MessageBox.Show("Введите пароль или сгенерируйте его", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                pwdPassword.Focus();
                return;
            }

            // Проверка дат
            if (!DateOfBirth.HasValue || !DateOfHire.HasValue)
            {
                MessageBox.Show("Заполните все даты", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Обновляем EditedUser из полей ввода
            EditedUser.Login = Login;
            EditedUser.LastName = LastName;
            EditedUser.FirstName = FirstName;
            EditedUser.MiddleName = MiddleName;
            EditedUser.PhoneNumber = FormatPhoneNumber(PhoneNumber);
            EditedUser.DateOfBirth = DateOfBirth.Value;
            EditedUser.DateOfHire = DateOfHire.Value;
            EditedUser.RoleId = RoleId ?? 2;

            // Проверяем, был ли изменен пароль
            if (EditedUser.Id == 0 || !string.IsNullOrEmpty(PasswordPlainText))
            {
                // Пароль уже установлен в обработчиках PasswordChanged
            }
            else if (EditedUser.Id > 0)
            {
                // Для существующего пользователя, если пароль не менялся, сохраняем старый
                var existingUser = _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == EditedUser.Id);
                if (existingUser != null)
                {
                    EditedUser.Password = existingUser.Password;
                }
            }

            // Валидация модели
            var validationResults = new System.Collections.Generic.List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(EditedUser);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(EditedUser, validationContext, validationResults, true);

            if (!isValid)
            {
                string errorMessage = "Ошибки валидации:\n";
                foreach (var validationResult in validationResults)
                {
                    errorMessage += $"- {validationResult.ErrorMessage}\n";
                }
                MessageBox.Show(errorMessage, "Ошибка валидации",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
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