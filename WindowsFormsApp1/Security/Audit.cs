using Oracle.ManagedDataAccess.Client;
using WindowsFormsApp1.Data;

namespace WindowsFormsApp1.Security
{
    public static class Audit
    {
        public static void Log(string evento)
        {
            try
            {
                using (var cn = Db.Open())
                {
                    using (var cmd = new OracleCommand(
                        "INSERT INTO BITACORA (USUARIO_ID, EVENTO) VALUES (:u,:e)", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":u", Session.UserId);
                        cmd.Parameters.Add(":e", evento);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Silenciar errores de auditoría para no romper el flujo de la app
            }
        }
    }
}
