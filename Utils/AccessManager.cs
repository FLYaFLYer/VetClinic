namespace VetClinic.Utils
{
    public static class AccessManager
    {
        public static bool CanEditPatients(string role)
        {
            return role == App.VetRole || role == App.AdminRole;
        }

        public static bool CanEditMedicines(string role)
        {
            return role == App.VetRole || role == App.AdminRole;
        }

        public static bool CanEditUsers(string role)
        {
            return role == App.AdminRole;
        }

        public static bool CanViewReports(string role)
        {
            return role == App.VetRole || role == App.AdminRole;
        }

        public static bool CanEditVisits(string role)
        {
            return role == App.VetRole || role == App.AdminRole;
        }

        public static bool CanEditOwners(string role)
        {
            return role == App.VetRole || role == App.AdminRole;
        }

        public static string GetNoPermissionMessage(string action)
        {
            return $"Нет прав для {action}. Обратитесь к администратору.";
        }
    }
}