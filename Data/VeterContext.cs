using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using VetClinic.Models;

namespace VetClinic.Data
{
    public class VeterContext : DbContext
    {
        public VeterContext() : base("name=VeterContext")
        {
            // Отключаем автоматическое создание/миграции БД
            Database.SetInitializer<VeterContext>(null);

            // Для отладки - создаем БД если ее нет
            try
            {
                Database.CreateIfNotExists();
            }
            catch
            {
                // Игнорируем ошибку, если БД уже существует
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<Breed> Breeds { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<MedicineStock> MedicineStocks { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Указываем, что мы используем существующие таблицы
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<Owner>().ToTable("owners");
            modelBuilder.Entity<Patient>().ToTable("patients");
            modelBuilder.Entity<AnimalType>().ToTable("animal_types");
            modelBuilder.Entity<Breed>().ToTable("breeds");
            modelBuilder.Entity<Medicine>().ToTable("medicines");
            modelBuilder.Entity<MedicineStock>().ToTable("medicine_stocks");
            modelBuilder.Entity<Visit>().ToTable("visits");
            modelBuilder.Entity<Prescription>().ToTable("prescriptions");
            modelBuilder.Entity<Notification>().ToTable("notifications");
            modelBuilder.Entity<UserNotification>().ToTable("user_notifications");

            modelBuilder.Entity<Medicine>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);
        }
    }
}