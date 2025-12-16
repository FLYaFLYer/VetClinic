using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using VetClinic.Data;
using VetClinic.Models;
using VetClinic.Utils;

namespace VetClinic
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            txtLogin.Focus();
        }

        // Метод для форматирования номера телефона
        private string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            // Удаляем все нецифровые символы
            string digits = Regex.Replace(phone, @"[^\d]", "");

            if (digits.Length == 11)
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

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            try
            {
                using (var context = new VeterContext())
                {
                    var user = context.Users
                        .Include(u => u.Role)
                        .FirstOrDefault(u => u.Login == login);

                    if (user != null && SecurityHelper.VerifyPassword(password, user.Password))
                    {
                        App.CurrentUser = user;
                        App.CurrentRole = user.Role.Name;

                        var mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль");
                        txtPassword.Password = "";
                        txtPassword.Focus();
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}\n\n" +
                    "Убедитесь, что:\n" +
                    "1. SQL Server LocalDB установлен\n" +
                    "2. База данных 'veterclinic' создана\n" +
                    "3. Строка подключения в App.config правильная");
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Dialogs.ChangePasswordDialog();
            dialog.ShowDialog();
        }
    }
}