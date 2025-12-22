using System.Data.Entity;
using System.Linq;
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

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login))
            {
                MessageBox.Show("Введите логин", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtLogin.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtPassword.Focus();
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
                        MessageBox.Show("Неверный логин или пароль", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
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
                    "3. Строка подключения в App.config правильная",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Dialogs.ChangePasswordDialog();
            dialog.ShowDialog();
        }

        private void TxtLogin_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }

        private void TxtPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }
    }
}