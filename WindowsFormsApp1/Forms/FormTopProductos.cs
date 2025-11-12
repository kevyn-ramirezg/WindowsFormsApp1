using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Data;


namespace WindowsFormsApp1.Forms
{


    public partial class FormTopProductos : Form
    {
        public FormTopProductos()
        {
            InitializeComponent();

            Load += (_, __) =>
            {
                dtpIni.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dtpFin.Value = DateTime.Today;
                numTop.Value = 10;
                Consultar(); // <-- ahora sí existe
            };

            btnConsultar.Click += (_, __) => Consultar();
        }

        private void Consultar()
        {
            int topN = (int)numTop.Value; // o 10
            var dt = ConsultarTop(dtpIni.Value, dtpFin.Value, topN);
            grid.DataSource = dt;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true;
        }


        // using System.Data; using Oracle.ManagedDataAccess.Client;

        private DataTable ConsultarTop(DateTime ini, DateTime fin, int topN)
        {
            using (var cn = Data.Db.Open())
            using (var da = new OracleDataAdapter(
                @"SELECT p.nombre AS PRODUCTO,
                 SUM(d.cantidad) AS CANTIDAD,
                 SUM(ROUND(d.cantidad*d.precio_unit,2)) AS SUBTOTAL,
                 SUM(d.linea_iva) AS IVA,
                 SUM(d.linea_total) AS VENTAS
          FROM venta v
          JOIN detalleventa d ON d.venta_id = v.id
          JOIN producto     p ON p.id       = d.producto_id
          WHERE TRUNC(v.fecha) BETWEEN :ini AND :fin
          GROUP BY p.nombre
          ORDER BY VENTAS DESC", cn))
            {
                da.SelectCommand.BindByName = true;
                da.SelectCommand.Parameters.Add(":ini", ini.Date);
                da.SelectCommand.Parameters.Add(":fin", fin.Date);

                var dt = new DataTable();
                da.Fill(dt);

                var top = dt.AsEnumerable().Take(topN);
                return top.Any() ? top.CopyToDataTable() : dt.Clone();
            }
        }


    }
}
