using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using VetClinic.Utils;

namespace VetClinic.Dialogs
{
    public partial class OwnerEditDialog : Window, IDataErrorInfo
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public OwnerEditDialog(Models.Owner owner = null)
        {
            InitializeComponent();

            if (owner != null)
            {
                LastName = owner.LastName;
                FirstName = owner.FirstName;
                Phone = PhoneNumberHelper.FormatPhoneNumber(owner.Phone);
                Email = owner.Email;
                Address = owner.Address;
            }
            else
            {
                // Пример номера (59 проблема)
                Phone = "+7 (___) ___-__-__";
            }

            DataContext = this;

            txtPhone.GotFocus += TxtPhone_GotFocus;
            txtPhone.LostFocus += TxtPhone_LostFocus;
        }

        private string FormatPhoneNumber(string phone)
        {
            return PhoneNumberHelper.FormatPhoneNumber(phone);
        }

        private void TxtPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPhone.IsFocused)
            {
                string text = txtPhone.Text;
                int caretIndex = txtPhone.CaretIndex;

                // Форматируем текст
                string formatted = PhoneNumberHelper.FormatPhoneNumber(text);

                if (formatted != text)
                {
                    txtPhone.Text = formatted;
                    // Пытаемся сохранить позицию курсора
                    txtPhone.CaretIndex = caretIndex + (formatted.Length - text.Length);
                }
            }
        }

        private void TxtPhone_GotFocus(object sender, RoutedEventArgs e)
        {
            // Если стоит пример, очищаем при фокусе
            if (txtPhone.Text == "+7 (___) ___-__-__")
            {
                txtPhone.Text = "+7 (";
                txtPhone.CaretIndex = 4;
            }
        }

        private void TxtPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            // Если поле пустое, ставим пример
            if (string.IsNullOrWhiteSpace(txtPhone.Text) || txtPhone.Text == "+7 (")
            {
                Phone = "+7 (___) ___-__-__";
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

                    case nameof(Phone):
                        if (string.IsNullOrWhiteSpace(Phone) || Phone == "+7 (___) ___-__-__")
                            error = "Телефон обязателен";
                        else if (!PhoneNumberHelper.IsValidPhoneNumber(Phone))
                            error = "Введите корректный номер телефона (10 или 11 цифр)";
                        break;

                    case nameof(Email):
                        if (!string.IsNullOrWhiteSpace(Email))
                        {
                            if (Email.Length > 100)
                                error = "Email не должен превышать 100 символов";
                            else if (!IsValidEmail(Email))
                                error = "Введите корректный email адрес";
                        }
                        break;

                    case nameof(Address):
                        if (!string.IsNullOrWhiteSpace(Address) && Address.Length > 200)
                            error = "Адрес не должен превышать 200 символов";
                        break;
                }

                return error;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Принудительно обновляем привязки
            txtLastName.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtFirstName.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtPhone.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtEmail.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtAddress.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            // Проверяем ошибки валидации
            if (Validation.GetHasError(txtLastName) ||
                Validation.GetHasError(txtFirstName) ||
                Validation.GetHasError(txtPhone) ||
                Validation.GetHasError(txtEmail) ||
                Validation.GetHasError(txtAddress))
            {
                MessageBox.Show("Исправьте ошибки в форме перед сохранением",
                    "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Нормализуем телефон перед сохранением (59 проблема)
            Phone = PhoneNumberHelper.NormalizePhoneNumber(Phone);

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}