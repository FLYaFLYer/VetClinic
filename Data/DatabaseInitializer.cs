using System.Data.Entity;

namespace VetClinic.Data
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<VeterContext>
    {
        protected override void Seed(VeterContext context)
        {
            // Инициализация данных при создании БД

            // Добавляем роли
            context.Roles.Add(new Models.Role { Name = "Ветеринар" });
            context.Roles.Add(new Models.Role { Name = "Администратор" });

            context.SaveChanges();
        }
    }
}