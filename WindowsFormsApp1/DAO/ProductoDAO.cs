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
            using (var cmd = new OracleCommand(
                "SELECT id, categoria_id, nombre, costo, precio_venta, stock FROM producto ORDER BY nombre", cn))
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    list.Add(new Producto
                    {
                        Id = dr.GetInt32(0),
                        CategoriaId = dr.GetInt32(1),
                        Nombre = dr.GetString(2),
                        Costo = dr.GetDecimal(3),
                        PrecioVenta = dr.GetDecimal(4),
                        Stock = dr.GetDecimal(5)
                    });
                }
            }
            return list;
        }

        public static List<ProductoForVenta> ListarParaVenta()
        {
            var list = new List<ProductoForVenta>();
            using (var cn = Data.Db.Open())
            using (var cmd = new OracleCommand(@"
                SELECT p.id, p.nombre, p.precio_venta, p.stock,
                       c.iva, c.utilidad_pct
                FROM producto p
                JOIN categoria c ON c.id = p.categoria_id
                ORDER BY p.nombre", cn))
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    list.Add(new ProductoForVenta
                    {
                        Id = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        PrecioVenta = dr.GetDecimal(2),
                        Stock = dr.GetDecimal(3),
                        IvaPct = dr.GetDecimal(4),
                        UtilidadPct = dr.GetDecimal(5)
                    });
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
