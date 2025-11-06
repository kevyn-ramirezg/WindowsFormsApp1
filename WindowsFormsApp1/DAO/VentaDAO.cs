using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using WindowsFormsApp1.Data;

namespace WindowsFormsApp1.DAO
{
    // ======== MODELOS QUE USA GUARDAR =========
    public class VentaCab
    {
        public int ClienteId { get; set; }
        public string Tipo { get; set; } // "CONTADO" | "CREDITO"
    }

    public class VentaDet
    {
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnit { get; set; }
        public decimal IvaPct { get; set; }
        public decimal UtilidadPct { get; set; }
        public decimal LineaSubtotal { get; set; }
        public decimal LineaIva { get; set; }
        public decimal LineaTotal { get; set; }
    }

    public class PlanCreditoInfo
    {
        public int Meses { get; set; } // 12/18/24
    }
    // ==========================================

    public static class VentaDAO
    {
        // Si ya tienes estos, puedes conservarlos:
        // public static List<Cliente> ListarClientes() { ... }
        // public static List<Producto> ListarProductos() { ... }

        public static int Guardar(VentaCab cab, List<VentaDet> dets, PlanCreditoInfo plan)
        {
            using (var cn = Db.Open())
            using (var tx = cn.BeginTransaction())
            {
                try
                {
                    // 1) Cabecera
                    int ventaId;
                    using (var cmd = new OracleCommand(@"
                        INSERT INTO VENTA (CLIENTE_ID, TIPO, SUBTOTAL, IVA_TOTAL, TOTAL)
                        VALUES (:c, :t, 0, 0, 0)
                        RETURNING ID INTO :id", cn))
                    {
                        cmd.Transaction = tx;
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":c", cab.ClienteId);
                        cmd.Parameters.Add(":t", cab.Tipo);
                        var pOut = new OracleParameter(":id", OracleDbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(pOut);
                        cmd.ExecuteNonQuery();
                        ventaId = Convert.ToInt32(pOut.Value.ToString());
                    }

                    // 2) Detalles
                    using (var cmdD = new OracleCommand(@"
                        INSERT INTO DETALLEVENTA
                          (VENTA_ID, PRODUCTO_ID, CANTIDAD, PRECIO_UNIT, IVA_PCT, UTILIDAD_PCT,
                           LINEA_SUBTOTAL, LINEA_IVA, LINEA_TOTAL)
                        VALUES(:v,:p,:q,:pu,:iva,:uti,:ls,:li,:lt)", cn))
                    {
                        cmdD.Transaction = tx;
                        cmdD.BindByName = true;

                        cmdD.Parameters.Add(":v", OracleDbType.Int32);
                        cmdD.Parameters.Add(":p", OracleDbType.Int32);
                        cmdD.Parameters.Add(":q", OracleDbType.Decimal);
                        cmdD.Parameters.Add(":pu", OracleDbType.Decimal);
                        cmdD.Parameters.Add(":iva", OracleDbType.Decimal);
                        cmdD.Parameters.Add(":uti", OracleDbType.Decimal);
                        cmdD.Parameters.Add(":ls", OracleDbType.Decimal);
                        cmdD.Parameters.Add(":li", OracleDbType.Decimal);
                        cmdD.Parameters.Add(":lt", OracleDbType.Decimal);

                        foreach (var d in dets)
                        {
                            cmdD.Parameters[0].Value = ventaId;
                            cmdD.Parameters[1].Value = d.ProductoId;
                            cmdD.Parameters[2].Value = d.Cantidad;
                            cmdD.Parameters[3].Value = d.PrecioUnit;
                            cmdD.Parameters[4].Value = d.IvaPct;
                            cmdD.Parameters[5].Value = d.UtilidadPct;
                            cmdD.Parameters[6].Value = d.LineaSubtotal;
                            cmdD.Parameters[7].Value = d.LineaIva;
                            cmdD.Parameters[8].Value = d.LineaTotal;
                            cmdD.ExecuteNonQuery();
                        }
                    }

                    // 3) Totales por SP
                    using (var cmdTot = new OracleCommand("SP_RECALCULAR_TOTALES", cn))
                    {
                        cmdTot.Transaction = tx;
                        cmdTot.CommandType = CommandType.StoredProcedure;
                        cmdTot.Parameters.Add("p_venta_id", OracleDbType.Int32).Value = ventaId;
                        cmdTot.ExecuteNonQuery();
                    }

                    // >>> NUEVO: leer el TOTAL calculado para pasarlo al SP de crédito
                    decimal totalVenta = 0m;
                    using (var cmdGetTotal = new OracleCommand("SELECT TOTAL FROM VENTA WHERE ID = :id", cn))
                    {
                        cmdGetTotal.Transaction = tx;
                        cmdGetTotal.BindByName = true;
                        cmdGetTotal.Parameters.Add(":id", OracleDbType.Int32).Value = ventaId;

                        var v = cmdGetTotal.ExecuteScalar();
                        if (v != null && v != DBNull.Value)
                            totalVenta = Convert.ToDecimal(v);
                    }

                    // 4) Si es crédito, plan
                    if (cab.Tipo == "CREDITO" && plan != null)
                    {
                        using (var cmdCred = new OracleCommand("SP_CONFIGURAR_CREDITO", cn))
                        {
                            cmdCred.Transaction = tx;
                            cmdCred.CommandType = CommandType.StoredProcedure;
                            cmdCred.BindByName = true;

                            cmdCred.Parameters.Add("p_venta_id", OracleDbType.Int32).Value = ventaId;
                            cmdCred.Parameters.Add("p_meses", OracleDbType.Int32).Value = plan.Meses;
                            cmdCred.Parameters.Add("p_total", OracleDbType.Decimal).Value = totalVenta;

                            cmdCred.ExecuteNonQuery();
                        }
                    }


                    tx.Commit();
                    return ventaId;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
    }
}
