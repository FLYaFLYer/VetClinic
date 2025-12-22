using System.Linq;
using System.Text.RegularExpressions;

namespace VetClinic.Utils
{
    public static class PhoneNumberHelper
    {
        public static string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            // Удаляем все нецифровые символы
            string digits = Regex.Replace(phone, @"[^\d]", "");

            // Преобразуем 8 в начале в +7
            if (digits.Length == 11 && digits.StartsWith("8"))
            {
                digits = "7" + digits.Substring(1);
            }

            // Форматируем номер
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

        public static string NormalizePhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            // Удаляем все нецифровые символы
            string digits = Regex.Replace(phone, @"[^\d]", "");

            // Преобразуем 8 в начале в 7
            if (digits.Length == 11 && digits.StartsWith("8"))
            {
                digits = "7" + digits.Substring(1);
            }

            // Если 10 цифр, добавляем 7 в начало
            if (digits.Length == 10)
            {
                digits = "7" + digits;
            }

            return digits;
        }

        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            string digits = Regex.Replace(phone, @"[^\d]", "");
            return (digits.Length == 10 || digits.Length == 11) && digits.All(char.IsDigit);
        }
    }
}