using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using WindowsFormsApp1.Data;

public static class CreditoDAO
{
    // Lista de cuotas de una venta (generadas por SP_CONFIGURAR_CREDITO)
    public static List<Cuota> ListarCuotasPorVenta(int ventaId)
    {
        var lista = new List<Cuota>();
        using (var cn = Db.Open())
        using (var cmd = new OracleCommand(@"
        SELECT
          N_CUOTA        AS NUM_CUOTA,
          FECHA_VENC     AS FECHA_VENC,
          VALOR_CUOTA    AS MONTO_CUOTA,
          VALOR_PAGADO   AS MONTO_PAGADO,
          ESTADO
        FROM CREDITO_CUOTA
        WHERE VENTA_ID = :id
        ORDER BY N_CUOTA", cn))
        {
            cmd.BindByName = true;
            cmd.Parameters.Add(":id", ventaId);

            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    lista.Add(new Cuota
                    {
                        NumCuota = rd.GetInt32(0),
                        FechaVenc = rd.GetDateTime(1),
                        ValorCuota = rd.GetDecimal(2),
                        ValorPagado = rd.IsDBNull(3) ? 0m : rd.GetDecimal(3),
                        Estado = rd.GetString(4)
                    });
                }
            }
        }
        return lista;
    }



    // Registrar pago de una cuota (puede llamar a un SP si ya lo tienes)
    // using System.Data;
    public static bool PagarCuota(int ventaId, int numCuota, decimal monto, DateTime fecha)
    {
        using (var cn = Db.Open())
        using (var tx = cn.BeginTransaction())
        {
            try
            {
                // 1) Validar que la cuota exista y no esté pagada
                using (var chk = new OracleCommand(@"
                    SELECT ESTADO, MONTO_CUOTA
                      FROM CREDITO_CUOTA
                     WHERE VENTA_ID = :v AND NUM_CUOTA = :n
                    ", cn))
                {
                    chk.BindByName = true;
                    chk.Transaction = tx;
                    chk.Parameters.Add(":v", ventaId);
                    chk.Parameters.Add(":n", numCuota);

                    using (var dr = chk.ExecuteReader())
                    {
                        if (!dr.Read()) { tx.Rollback(); return false; }
                        var estado = dr.GetString(0);
                        var montoCuota = dr.GetDecimal(1);
                        if (estado.Equals("PAGADA", StringComparison.OrdinalIgnoreCase)) { tx.Rollback(); return false; }
                        // Si quieres validar exactitud del monto:
                        // if (monto != montoCuota) { tx.Rollback(); return false; }
                    }
                }

                // 2) Registrar pago (opcional si no llevas tabla de pagos)
                using (var ins = new OracleCommand(@"
                    INSERT INTO CREDITO_PAGO (VENTA_ID, NUM_CUOTA, MONTO, FECHA_PAGO)
                    VALUES (:v, :n, :m, :f)
                ", cn))
                {
                    ins.BindByName = true;
                    ins.Transaction = tx;
                    ins.Parameters.Add(":v", ventaId);
                    ins.Parameters.Add(":n", numCuota);
                    ins.Parameters.Add(":m", monto);
                    ins.Parameters.Add(":f", fecha);
                    ins.ExecuteNonQuery();
                }

                // 3) Marcar cuota como pagada
                using (var upd = new OracleCommand(@"
                    UPDATE CREDITO_CUOTA
                       SET ESTADO = 'PAGADA',
                           MONTO_PAGADO = :m,
                           FECHA_PAGO = :f
                     WHERE VENTA_ID = :v AND NUM_CUOTA = :n
                ", cn))
                {
                    upd.BindByName = true;
                    upd.Transaction = tx;
                    upd.Parameters.Add(":m", monto);
                    upd.Parameters.Add(":f", fecha);
                    upd.Parameters.Add(":v", ventaId);
                    upd.Parameters.Add(":n", numCuota);
                    upd.ExecuteNonQuery();
                }

                // 4) Recalcular totales del crédito (si llevas acumulados en otra tabla)
                using (var recalc = new OracleCommand("SP_RECALCULAR_TOTALES", cn))
                {
                    recalc.CommandType = CommandType.StoredProcedure;
                    recalc.BindByName = true;
                    recalc.Transaction = tx;
                    recalc.Parameters.Add("P_VENTA_ID", OracleDbType.Int32).Value = ventaId;
                    recalc.ExecuteNonQuery();
                }

                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                return false;
            }
        }
    }

}
