using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.DAO
{
    public class ClienteMin
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public override string ToString() => Nombre;
    }
    public static class ClienteDAO
    {
        public static List<Cliente> Listar()
        {
            var list = new List<Cliente>();
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                "SELECT id, nombre, telefono, correo FROM cliente ORDER BY nombre", cn))
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    list.Add(new Cliente
                    {
                        Id = dr.GetInt32(0),
                        Nombre = dr.GetString(1),
                        Telefono = dr.IsDBNull(2) ? null : dr.GetString(2),
                        Correo = dr.IsDBNull(3) ? null : dr.GetString(3)
                    });
                }
            }
            return list;
        }

        public static List<ClienteMin> ListarMin()
        {
            var list = new List<ClienteMin>();
            using (var cn = Data.Db.Open())
            using (var cmd = new OracleCommand(
                "SELECT id, nombre FROM cliente ORDER BY nombre", cn))
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    list.Add(new ClienteMin { Id = dr.GetInt32(0), Nombre = dr.GetString(1) });
                }
            }
            return list;
        }

        public static void Insertar(Cliente c)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                "INSERT INTO Cliente(nombre, telefono, correo) VALUES(:n,:t,:e)", cn))
            {
                cmd.Parameters.Add(":n", c.Nombre);
                cmd.Parameters.Add(":t", string.IsNullOrEmpty(c.Telefono) ? (object)DBNull.Value : c.Telefono);
                cmd.Parameters.Add(":e", string.IsNullOrEmpty(c.Correo) ? (object)DBNull.Value : c.Correo);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Actualizar(Cliente c)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                "UPDATE Cliente SET nombre=:n, telefono=:t, correo=:e WHERE id=:id", cn))
            {
                cmd.Parameters.Add(":n", c.Nombre);
                cmd.Parameters.Add(":t", string.IsNullOrEmpty(c.Telefono) ? (object)DBNull.Value : c.Telefono);
                cmd.Parameters.Add(":e", string.IsNullOrEmpty(c.Correo) ? (object)DBNull.Value : c.Correo);
                cmd.Parameters.Add(":id", c.Id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void Eliminar(decimal id)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand("DELETE FROM APP_USR.CLIENTE WHERE ID = :id", cn))
            {
                cmd.Parameters.Add(":id", OracleDbType.Decimal).Value = id;
                cmd.ExecuteNonQuery();
            }
        }

    }
}
