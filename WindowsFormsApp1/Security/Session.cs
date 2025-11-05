namespace WindowsFormsApp1.Security
{
    public static class Session
    {
        public static int UsuarioId { get; private set; }
        public static string Username { get; private set; }
        public static int Nivel { get; private set; } // 1=Admin,2=Paramétrico,3=Esporádico
        public static bool IsLogged => UsuarioId > 0;

        public static void Set(int id, string user, int nivel)
        {
            UsuarioId = id; Username = user; Nivel = nivel;
        }

        public static void Clear()
        {
            UsuarioId = 0; Username = null; Nivel = 0;
        }
    }
}
