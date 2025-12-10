using VetClinic.Data;
using VetClinic.Models;
using VetClinic.Utils;
using System;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;

namespace VetClinic.Dialogs
{
	public partial class UserEditDialog : Window
	{
		private readonly VeterContext _context = new VeterContext();

		public User EditedUser { get; private set; }
		public string PasswordPlainText { get; set; }

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
					RoleId = 2 // По умолчанию Администратор (нужно проверить ID роли в БД)
				};

				// Генерируем начальный пароль
				GenerateAndSetPassword();
			}

			// Устанавливаем DataContext
			DataContext = this;
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
			if (string.IsNullOrWhiteSpace(EditedUser.Login))
			{
				MessageBox.Show("Введите логин пользователя", "Ошибка",
							  MessageBoxButton.OK, MessageBoxImage.Error);
				txtLogin.Focus();
				return;
			}

			// Проверка фамилии
			if (string.IsNullOrWhiteSpace(EditedUser.LastName))
			{
				MessageBox.Show("Введите фамилию пользователя", "Ошибка",
							  MessageBoxButton.OK, MessageBoxImage.Error);
				txtLastName.Focus();
				return;
			}

			// Проверка имени
			if (string.IsNullOrWhiteSpace(EditedUser.FirstName))
			{
				MessageBox.Show("Введите имя пользователя", "Ошибка",
							  MessageBoxButton.OK, MessageBoxImage.Error);
				txtFirstName.Focus();
				return;
			}

			// Проверка телефона
			if (string.IsNullOrWhiteSpace(EditedUser.PhoneNumber))
			{
				MessageBox.Show("Введите телефон пользователя", "Ошибка",
							  MessageBoxButton.OK, MessageBoxImage.Error);
				txtPhoneNumber.Focus();
				return;
			}

			// Проверка роли
			if (EditedUser.RoleId <= 0)
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

			// Если пароль пустой при создании нового пользователя, генерируем его
			if (EditedUser.Id == 0 && string.IsNullOrEmpty(EditedUser.Password))
			{
				GenerateAndSetPassword();
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