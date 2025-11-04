using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using WindowsFormsApp1.Data;   // Db.Open()
using WindowsFormsApp1.Models; // Cliente, Producto

namespace WindowsFormsApp1.DAO
{
    public static class VentaDAO
    {
        // --------------------------------------------------------------------
        // Utilidades de carga (si ya las tienes en otro DAO, puedes omitirlas)
        // --------------------------------------------------------------------
        public static List<Cliente> ListarClientes()
        {
            var list = new List<Cliente>();
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                @"SELECT id, nombre, telefono, correo 
                  FROM APP_USR.Cliente 
                  ORDER BY nombre", cn))
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    list.Add(new Cliente
                    {
                        Id = dr.GetDecimal(0),
                        Nombre = dr.GetString(1),
                        Telefono = dr.IsDBNull(2) ? null : dr.GetString(2),
                        Correo = dr.IsDBNull(3) ? null : dr.GetString(3)
                    });
                }
            }
            return list;
        }

        public static List<Producto> ListarProductos()
        {
            var list = new List<Producto>();
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                @"SELECT id, categoria_id, nombre, costo, precio_venta, stock 
                  FROM APP_USR.Producto 
                  ORDER BY nombre", cn))
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    list.Add(new Producto
                    {
                        Id = dr.GetDecimal(0),
                        CategoriaId = dr.GetDecimal(1),
                        Nombre = dr.GetString(2),
                        Costo = dr.GetDecimal(3),
                        PrecioVenta = dr.GetDecimal(4),
                        Stock = dr.GetDecimal(5) // si es entero en DB, igual llega como decimal
                    });
                }
            }
            return list;
        }

        // --------------------------------------------------------------------
        // Registrar venta al contado
        //   - clienteId: id del cliente
        //   - productoId/cantidad/precioUnit: listas paralelas
        // Retorna: id de la venta generada
        // --------------------------------------------------------------------
        public static decimal RegistrarContado(
            decimal clienteId,
            List<decimal> productoId,
            List<decimal> cantidad,
            List<decimal> precioUnit)
        {
            if (productoId == null || cantidad == null || precioUnit == null)
                throw new ArgumentException("Las listas no pueden ser nulas.");

            if (productoId.Count == 0 ||
                productoId.Count != cantidad.Count ||
                productoId.Count != precioUnit.Count)
                throw new ArgumentException("Las listas deben tener el mismo tamaño y al menos un elemento.");

            using (var cn = Db.Open())
            using (var tx = cn.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    // 1) Insertar cabecera y obtener ID con RETURNING
                    decimal ventaId;

                    using (var cmdVenta = new OracleCommand(
                        @"INSERT INTO APP_USR.Venta (cliente_id, tipo, total) 
                          VALUES (:c, 'C', 0) 
                          RETURNING id INTO :id", cn))
                    {
                        cmdVenta.BindByName = true;
                        cmdVenta.Transaction = tx;

                        cmdVenta.Parameters.Add(":c", OracleDbType.Decimal).Value = clienteId;

                        var pId = new OracleParameter(":id", OracleDbType.Decimal)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        cmdVenta.Parameters.Add(pId);

                        cmdVenta.ExecuteNonQuery();

                        // ¡IMPORTANTE! Leer el RETURNING como OracleDecimal
                        var outId = (OracleDecimal)pId.Value;
                        ventaId = outId.Value; // decimal nativo
                    }

                    // Comandos reusables para detalle y stock (con tipos y escalas correctas)
                    var cmdDetalle = new OracleCommand(
                        @"INSERT INTO APP_USR.DetalleVenta
                          (venta_id, producto_id, cantidad, precio_unit)
                          VALUES (:v, :p, :c, :u)", cn)
                    {
                        BindByName = true,
                        Transaction = tx
                    };

                    // :v (venta_id)
                    cmdDetalle.Parameters.Add(":v", OracleDbType.Decimal);
                    // :p (producto_id)
                    cmdDetalle.Parameters.Add(":p", OracleDbType.Decimal);
                    // :c (cantidad) -> entero (Scale = 0) o Int32
                    var parCant = cmdDetalle.Parameters.Add(":c", OracleDbType.Int32);
                    // Si tu columna es NUMBER(12,2) para cantidad, usa Decimal + Scale=0:
                    // var parCant = cmdDetalle.Parameters.Add(":c", OracleDbType.Decimal); parCant.Scale = 0;

                    // :u (precio unitario) -> NUMBER(12,2) típico
                    var parPrecio = cmdDetalle.Parameters.Add(":u", OracleDbType.Decimal);
                    parPrecio.Precision = 18; // ajusta si quieres
                    parPrecio.Scale = 2;

                    var cmdStock = new OracleCommand(
                        @"UPDATE APP_USR.Producto SET stock = stock - :c 
                          WHERE id = :p", cn)
                    {
                        BindByName = true,
                        Transaction = tx
                    };
                    var parDesc = cmdStock.Parameters.Add(":c", OracleDbType.Int32);
                    cmdStock.Parameters.Add(":p", OracleDbType.Decimal);

                    // Consulta de stock con bloqueo
                    var cmdStockCheck = new OracleCommand(
                        @"SELECT stock 
                          FROM APP_USR.Producto 
                          WHERE id = :p 
                          FOR UPDATE", cn)
                    {
                        BindByName = true,
                        Transaction = tx
                    };
                    cmdStockCheck.Parameters.Add(":p", OracleDbType.Decimal);

                    // 2) Recorrer items, validar stock, insertar detalle y descontar
                    decimal total = 0m;

                    for (int i = 0; i < productoId.Count; i++)
                    {
                        var pid = productoId[i];
                        var qty = Convert.ToInt32(cantidad[i]); // cantidad entera
                        var unit = precioUnit[i];

                        // Validación de stock con FOR UPDATE
                        cmdStockCheck.Parameters[0].Value = pid;
                        object raw = cmdStockCheck.ExecuteScalar();
                        if (raw == null)
                            throw new InvalidOperationException("Producto no encontrado: " + pid);

                        var currentStock = ((OracleDecimal)raw).Value; // decimal
                        if (qty > currentStock)
                            throw new InvalidOperationException($"Stock insuficiente para producto {pid}. Disponible: {currentStock}, solicitado: {qty}.");

                        // Insertar detalle
                        cmdDetalle.Parameters[0].Value = ventaId; // :v
                        cmdDetalle.Parameters[1].Value = pid;     // :p
                        parCant.Value = qty;                      // :c
                        parPrecio.Value = unit;                   // :u
                        cmdDetalle.ExecuteNonQuery();

                        // Descontar stock
                        parDesc.Value = qty;                      // :c
                        cmdStock.Parameters[1].Value = pid;       // :p
                        cmdStock.ExecuteNonQuery();

                        total += unit * qty;
                    }

                    // 3) Actualizar total en cabecera
                    using (var cmdTot = new OracleCommand(
                        @"UPDATE APP_USR.Venta SET total = :t WHERE id = :id", cn))
                    {
                        cmdTot.BindByName = true;
                        cmdTot.Transaction = tx;

                        var pTot = cmdTot.Parameters.Add(":t", OracleDbType.Decimal);
                        pTot.Precision = 18;
                        pTot.Scale = 2;
                        pTot.Value = total;

                        cmdTot.Parameters.Add(":id", OracleDbType.Decimal).Value = ventaId;

                        cmdTot.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return ventaId;
                }
                catch
                {
                    try { tx.Rollback(); } catch { /* noop */ }
                    throw;
                }
            }
        }
    }
}
