using System.Collections.Generic;
using System.Linq;

namespace Gardian.Managers
{
    // Модель учетной записи
    public class UserAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
        

        // Список контактов, доступных этому пользователю (для Skype и будущих приложений)
        public List<string> AllowedContacts { get; set; } = new();
    }

    public static class UserSessionManager
    {
        // Текущий авторизованный пользователь (null если не авторизован)
        public static UserAccount CurrentUser { get; private set; } = null;
        public static bool IsLoggedIn => CurrentUser != null;
        // База пользователей (в будущем можно заменить на JSON/файл)
        private static readonly List<UserAccount> _accounts = new()
        {
            new UserAccount
            {
                Username = "GardyM",
                Password = "blackhole",
                AllowedContacts = new List<string> { "Echo-bot", "Anatoly" }
            },
            new UserAccount
            {
                Username = "GM",
                Password = "eden//",
                AllowedContacts = new List<string> { "Echo-bot", "Anatoly" }
            },
            new UserAccount
            {
                Username = "Apsis",
                Password = "tenebrium",
                AllowedContacts = new List<string> { "Admin", "TechSupport" }
            }
        };

        // Попытка авторизации
        public static bool TryLogin(string username, string password)
        {
            var user = _accounts.FirstOrDefault(u =>
                u.Username == username && u.Password == password);

            if (user != null)
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        // Выход из системы
        public static void Logout()
        {
            CurrentUser = null;
        }

        // Получить список разрешенных контактов для текущего пользователя
        public static List<string> GetAllowedContacts()
        {
            return CurrentUser?.AllowedContacts ?? new List<string>();
        }
    }
}