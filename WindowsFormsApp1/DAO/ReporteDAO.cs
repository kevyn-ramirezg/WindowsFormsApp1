using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using WindowsFormsApp1.Data;

public static class ReporteDAO
{
    // 1) Cabecera de ventas por rango
    public static DataTable VentasCab(DateTime ini, DateTime fin)
    {
        using (var cn = Db.Open())
        using (var da = new OracleDataAdapter(@"
            SELECT
              v.id                           AS ID,
              TRUNC(v.fecha)                 AS FECHA,
              c.nombre                       AS CLIENTE,
              v.tipo                         AS TIPO,
              v.subtotal                     AS SUBTOTAL,
              v.iva_total                    AS IVA_TOTAL,
              v.total                        AS TOTAL
            FROM venta v
            JOIN cliente c ON c.id = v.cliente_id
            WHERE TRUNC(v.fecha) BETWEEN :ini AND :fin
            ORDER BY v.fecha, v.id", cn))
        {
            da.SelectCommand.BindByName = true;
            da.SelectCommand.Parameters.Add(":ini", OracleDbType.Date).Value = ini;
            da.SelectCommand.Parameters.Add(":fin", OracleDbType.Date).Value = fin;
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }

    // 2) Totales del rango
    public struct Totales
    {
        public decimal Subtotal;
        public decimal IvaTotal;
        public decimal Total;
    }

    public static Totales VentasTotales(DateTime ini, DateTime fin)
    {
        using (var cn = Db.Open())
        using (var cmd = new OracleCommand(@"
            SELECT
              NVL(SUM(v.subtotal),0),
              NVL(SUM(v.iva_total),0),
              NVL(SUM(v.total),0)
            FROM venta v
            WHERE TRUNC(v.fecha) BETWEEN :ini AND :fin", cn))
        {
            cmd.BindByName = true;
            cmd.Parameters.Add(":ini", OracleDbType.Date).Value = ini;
            cmd.Parameters.Add(":fin", OracleDbType.Date).Value = fin;

            using (var rd = cmd.ExecuteReader())
            {
                rd.Read();
                return new Totales
                {
                    Subtotal = rd.IsDBNull(0) ? 0 : rd.GetDecimal(0),
                    IvaTotal = rd.IsDBNull(1) ? 0 : rd.GetDecimal(1),
                    Total = rd.IsDBNull(2) ? 0 : rd.GetDecimal(2),
                };
            }
        }
    }

    // 3) Top productos por rango
    public static DataTable TopProductos(DateTime ini, DateTime fin, int topN)
    {
        using (var cn = Db.Open())
        using (var da = new OracleDataAdapter(@"
            SELECT *
            FROM (
              SELECT
                p.nombre                       AS PRODUCTO,
                SUM(d.cantidad)                AS CANTIDAD,
                SUM(d.linea_total)             AS VENTAS
              FROM detalleventa d
              JOIN venta v   ON v.id   = d.venta_id
              JOIN producto p ON p.id  = d.producto_id
              WHERE TRUNC(v.fecha) BETWEEN :ini AND :fin
              GROUP BY p.nombre
              ORDER BY VENTAS DESC
            ) 
            WHERE ROWNUM <= :topN", cn))
        {
            da.SelectCommand.BindByName = true;
            da.SelectCommand.Parameters.Add(":ini", OracleDbType.Date).Value = ini;
            da.SelectCommand.Parameters.Add(":fin", OracleDbType.Date).Value = fin;
            da.SelectCommand.Parameters.Add(":topN", OracleDbType.Int32).Value = topN;

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
