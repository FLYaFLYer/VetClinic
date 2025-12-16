using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using VetClinic.Data;
using VetClinic.Models;

namespace VetClinic.Dialogs
{
    public partial class OwnerEditDialog : Window
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        private readonly VeterContext _context = new VeterContext();

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

            // Если номер не соответствует формату, возвращаем как есть
            return phone;
        }

        // Обработчик для форматирования телефона при вводе
        private void TxtPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPhone.IsFocused)
            {
                string text = txtPhone.Text;
                int caretIndex = txtPhone.CaretIndex;

                // Форматируем текст
                string formatted = FormatPhoneNumber(text);

                if (formatted != text)
                {
                    txtPhone.Text = formatted;
                    // Пытаемся сохранить позицию курсора
                    txtPhone.CaretIndex = caretIndex + (formatted.Length - text.Length);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastName))
            {
                MessageBox.Show("Введите фамилию владельца");
                return;
            }

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                MessageBox.Show("Введите имя владельца");
                return;
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                MessageBox.Show("Введите телефон владельца");
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