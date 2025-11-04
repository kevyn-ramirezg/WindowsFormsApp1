using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.DAO
{
    public static class ProductoDAO
    {
        public static List<Producto> Listar()
        {
            var list = new List<Producto>();
            using (var cn = Db.Open())
            using (var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(@"
    SELECT p.id,
           p.categoria_id,
           p.nombre,
           p.costo,
           p.precio_venta,
           p.stock,
           c.nombre AS categoria
    FROM   APP_USR.PRODUCTO p
    JOIN   APP_USR.CATEGORIA c ON c.id = p.categoria_id
    ORDER BY p.nombre", cn))

                

            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    var p = new Producto();
                    p.Id = dr.GetDecimal(0);
                    p.CategoriaId = dr.GetDecimal(1);
                    p.Nombre = dr.GetString(2);
                    p.Costo = dr.GetDecimal(3);
                    p.PrecioVenta = dr.GetDecimal(4);
                    p.Stock = dr.GetDecimal(5);
                    p.Categoria = dr.GetString(6);
                    list.Add(p);
                }
            }
            return list;
        }

        public static List<Categoria> ListarCategoriasCombo()
        {

            var list = new List<Categoria>();
            using (var cn = Db.Open())
            using (var cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(
                "SELECT id, nombre, iva, utilidad_pct FROM APP_USR.CATEGORIA ORDER BY nombre", cn))
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

        public static void Insertar(Producto p)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                "INSERT INTO Producto(categoria_id, nombre, costo, precio_venta, stock) " +
                "VALUES(:c,:n,:co,:pv,:s)", cn))
            {
                cmd.Parameters.Add(":c", p.CategoriaId);
                cmd.Parameters.Add(":n", p.Nombre);
                cmd.Parameters.Add(":co", p.Costo);
                cmd.Parameters.Add(":pv", p.PrecioVenta);
                cmd.Parameters.Add(":s", p.Stock);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Actualizar(Producto p)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                "UPDATE Producto SET categoria_id=:c, nombre=:n, costo=:co, precio_venta=:pv, stock=:s WHERE id=:id", cn))
            {
                cmd.Parameters.Add(":c", p.CategoriaId);
                cmd.Parameters.Add(":n", p.Nombre);
                cmd.Parameters.Add(":co", p.Costo);
                cmd.Parameters.Add(":pv", p.PrecioVenta);
                cmd.Parameters.Add(":s", p.Stock);
                cmd.Parameters.Add(":id", p.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Eliminar(decimal id)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand("DELETE FROM Producto WHERE id=:id", cn))
            {
                cmd.Parameters.Add(":id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
