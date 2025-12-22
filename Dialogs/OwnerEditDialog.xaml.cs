using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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
                Phone = FormatPhoneNumber(owner.Phone);
                Email = owner.Email;
                Address = owner.Address;
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

            return phone;
        }

        private void TxtPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPhone.IsFocused)
            {
                string text = txtPhone.Text;
                int caretIndex = txtPhone.CaretIndex;

                string formatted = FormatPhoneNumber(text);

                if (formatted != text)
                {
                    txtPhone.Text = formatted;
                    txtPhone.CaretIndex = caretIndex + (formatted.Length - text.Length);
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
                        if (string.IsNullOrWhiteSpace(Phone))
                            error = "Телефон обязателен";
                        else
                        {
                            string digits = Regex.Replace(Phone, @"[^\d]", "");
                            if (digits.Length < 10 || digits.Length > 15)
                                error = "Введите корректный номер телефона (10-15 цифр)";
                        }
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
            // Принудительно обновляем привязки для срабатывания валидации
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

            // Форматируем телефон перед сохранением
            Phone = FormatPhoneNumber(Phone);

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