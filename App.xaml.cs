using System.Windows;
using VetClinic.Data;

namespace VetClinic
{
    public partial class App : Application
    {
        public static string CurrentRole { get; set; }
        public static Models.User CurrentUser { get; set; }

        public const string VetRole = "Ветеринар";
        public const string AdminRole = "Администратор";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Инициализируем базу данных при запуске приложения
            DatabaseHelper.InitializeDatabase();
        }
    }
}