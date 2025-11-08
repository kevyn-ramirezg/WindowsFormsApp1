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
                using (var cmd = new OracleCommand("SP_PAGAR_CUOTA", cn))
                {
                    cmd.Transaction = tx;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("p_venta_id", OracleDbType.Int32).Value = ventaId;
                    cmd.Parameters.Add("p_n_cuota", OracleDbType.Int32).Value = numCuota;
                    cmd.Parameters.Add("p_monto", OracleDbType.Decimal).Value = monto;
                    cmd.Parameters.Add("p_fecha", OracleDbType.Date).Value = fecha;

                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                return true;
            }
            catch (OracleException ex)
            {
                // Te muestra la causa real si algo falla (FK, NO_DATA_FOUND, etc.)
                System.Windows.Forms.MessageBox.Show("Oracle: " + ex.Message);
                try { tx.Rollback(); } catch { }
                return false;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                try { tx.Rollback(); } catch { }
                return false;
            }
        }
    }
}