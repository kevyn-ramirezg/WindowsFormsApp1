using Oracle.ManagedDataAccess.Client;
using WindowsFormsApp1.Data;

namespace WindowsFormsApp1.DAO
{
    public static class BitacoraDAO
    {
        public static void RegistrarIngreso(int usuarioId)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                "INSERT INTO BITACORA (USUARIO_ID, EVENTO, FECHA) VALUES (:id, 'LOGIN', SYSDATE)", cn))
            {
                cmd.BindByName = true;
                cmd.Parameters.Add(":id", OracleDbType.Int32).Value = usuarioId;
                cmd.ExecuteNonQuery();
            }
        }

        public static void RegistrarSalida(int usuarioId)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                "INSERT INTO BITACORA (USUARIO_ID, EVENTO, FECHA) VALUES (:id, 'LOGOUT', SYSDATE)", cn))
            {
                cmd.BindByName = true;
                cmd.Parameters.Add(":id", OracleDbType.Int32).Value = usuarioId;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
