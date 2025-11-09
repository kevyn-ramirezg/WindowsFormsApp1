namespace WindowsFormsApp1.Security
{
    public static class Session
    {
        public static int UserId { get; private set; }
        public static string Username { get; private set; }
        public static int Nivel { get; private set; } // 1=Admin, 2=Paramétrico, 3=Esporádico

        public static void Set(int userId, string username, int nivel)
        {
            UserId = userId;
            Username = username ?? string.Empty;
            Nivel = nivel;
        }

        public static void Clear()
        {
            UserId = 0;
            Username = string.Empty;
            Nivel = 0;
        }
    }
}
