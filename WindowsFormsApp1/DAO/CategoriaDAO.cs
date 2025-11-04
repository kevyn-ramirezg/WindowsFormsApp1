using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.DAO
{
    public static class CategoriaDAO
    {
        public static List<Categoria> Listar()
        {
            var list = new List<Categoria>();
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                "SELECT id, nombre, iva, utilidad_pct FROM Categoria ORDER BY nombre", cn))
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    var c = new Categoria();
                    c.Id = dr.GetDecimal(0);
                    c.Nombre = dr.GetString(1);
                    c.Iva = dr.GetDecimal(2);
                    c.UtilidadPct = dr.GetDecimal(3);
                    list.Add(c);
                }
            }
            return list;
        }

        public static void Insertar(Categoria c)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                "INSERT INTO Categoria(nombre, iva, utilidad_pct) VALUES(:n,:i,:u)", cn))
            {
                cmd.Parameters.Add(":n", c.Nombre);
                cmd.Parameters.Add(":i", c.Iva);
                cmd.Parameters.Add(":u", c.UtilidadPct);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Actualizar(Categoria c)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                "UPDATE Categoria SET nombre=:n, iva=:i, utilidad_pct=:u WHERE id=:id", cn))
            {
                cmd.Parameters.Add(":n", c.Nombre);
                cmd.Parameters.Add(":i", c.Iva);
                cmd.Parameters.Add(":u", c.UtilidadPct);
                cmd.Parameters.Add(":id", c.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Eliminar(decimal id)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand("DELETE FROM Categoria WHERE id=:id", cn))
            {
                cmd.Parameters.Add(":id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
