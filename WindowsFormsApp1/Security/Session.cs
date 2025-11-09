namespace WindowsFormsApp1.Security
{
    public static class Session
    {
        public static int? UserId { get; private set; }
        public static string Login { get; private set; }
        public static int Nivel { get; private set; }

        public static bool IsActive => UserId.HasValue;

        public static void Set(int userId, string login, int nivel)
        {
            UserId = userId;
            Login = login;
            Nivel = nivel;
        }

        public static void Clear()
        {
            UserId = null;
            Login = null;
            Nivel = 0;
        }
    }
}
