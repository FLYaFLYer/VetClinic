using System;
using System.Data.Entity;
using System.Linq;

namespace VetClinic.Data
{
    public static class DatabaseHelper
    {
        public static void InitializeDatabase()
        {
            try
            {
                using (var context = new VeterContext())
                {
                    // Проверяем подключение к БД
                    context.Database.Connection.Open();
                    context.Database.Connection.Close();

                    // Проверяем наличие данных
                    if (!context.Roles.Any())
                    {
                        // Если таблица пустая, добавляем базовые роли
                        context.Roles.Add(new Models.Role { Name = "Ветеринар" });
                        context.Roles.Add(new Models.Role { Name = "Администратор" });
                        context.SaveChanges();
                    }

                    // Проверяем наличие тестовых пользователей
                    if (!context.Users.Any())
                    {
                        var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Администратор");
                        var vetRole = context.Roles.FirstOrDefault(r => r.Name == "Ветеринар");

                        if (adminRole != null && vetRole != null)
                        {
                            var hashedPassword = Utils.SecurityHelper.HashPassword("admin123");

                            context.Users.Add(new Models.User
                            {
                                FirstName = "Мария",
                                MiddleName = "Игоревна",
                                LastName = "Сидорова",
                                DateOfBirth = new DateTime(1995, 11, 30),
                                PhoneNumber = "+7(911)222-3344",
                                DateOfHire = new DateTime(2021, 5, 1),
                                RoleId = adminRole.Id,
                                Login = "admin",
                                Password = hashedPassword
                            });

                            hashedPassword = Utils.SecurityHelper.HashPassword("vet123");

                            context.Users.Add(new Models.User
                            {
                                FirstName = "Иван",
                                MiddleName = "Иванович",
                                LastName = "Петров",
                                DateOfBirth = new DateTime(1985, 5, 15),
                                PhoneNumber = "+7(123)456-7890",
                                DateOfHire = new DateTime(2020, 1, 10),
                                RoleId = vetRole.Id,
                                Login = "vet",
                                Password = hashedPassword
                            });

                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка инициализации БД: {ex.Message}\n\nУбедитесь, что база данных 'veterclinic' создана на (localdb)\\mssqllocaldb");
            }
        }
    }
}