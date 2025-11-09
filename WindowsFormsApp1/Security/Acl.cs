using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using WindowsFormsApp1.Data;

namespace WindowsFormsApp1.Security
{
    public static class Acl
    {
        // Devuelve true si el nivel actual tiene permiso para "accion".
        public static bool Permite(string accion)
        {
            try
            {
                using (var cn = Db.Open())
                {
                    using (var cmd = new OracleCommand(
                        "SELECT COUNT(*) FROM PERMISO WHERE NIVEL = :niv AND ACCION = :a", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":niv", Session.Nivel);
                        cmd.Parameters.Add(":a", accion);
                        var n = Convert.ToInt32(cmd.ExecuteScalar());
                        return n > 0;
                    }
                }
            }
            catch
            {
                // Si algo falla, por seguridad retorna false
                return false;
            }
        }

        // Azúcar sintáctico para los forms: muestra mensaje si no tiene permiso
        public static bool Require(string accion)
        {
            if (Permite(accion)) return true;
            MessageBox.Show("No tienes permisos para realizar esta acción: " + accion,
                            "Permisos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        public static bool Require(Feature feature)
        {
            // Usa el nombre del enum como texto de acción en PERMISO.ACCION
            return Require(feature.ToString());
        }
    }
}
