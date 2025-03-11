using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VetClinic
{
    public partial class LoginPage : Window
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private string GetUserRole(string username, string password)
        {
            // Обновленный словарь пользователей с новыми учетными данными
            var users = new Dictionary<string, (string Password, string Role)>
            {
                { "manager", ("managerPassword", "manager") },
                { "user", ("userPassword", "user") },
                { "aboba", ("000", "manager") },
                { "chel", ("123", "user") }
            };

            // Проверяем, существует ли пользователь и правильный ли пароль
            if (users.TryGetValue(username, out var userInfo))
            {
                if (userInfo.Password == password)
                {
                    return userInfo.Role; // Возвращаем роль пользователя
                }
            }

            return "unknown"; // Если пользователь не найден или пароль неверный
        }

        private void Autorized_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(LoginText.Text))
            {
                errors.AppendLine("Укажите логин");
            }
            if (string.IsNullOrWhiteSpace(PasswordText.Text))
            {
                errors.AppendLine("Укажите пароль");
            }
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            string username = LoginText.Text;
            string password = PasswordText.Text;

            // Получаем роль пользователя
            string userRole = GetUserRole(username, password);
             
            if (userRole != "unknown")
            {
                MessageBox.Show("Пользователь авторизован");

                // Здесь создаем и показываем главное окно
                MainWindow mainWindow = new MainWindow();

                // Переход на соответствующую страницу в зависимости от роли
                if (userRole == "admin")
                {
                    mainWindow.MainFrame.Navigate(new AdminPage());
                }
                else if (userRole == "user")
                {
                    mainWindow.MainFrame.Navigate(new MainPage());
                }

                mainWindow.Show(); // Открываем главное окно
                this.Close(); // Закрываем окно логина
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль");
            }
        }
    }
}