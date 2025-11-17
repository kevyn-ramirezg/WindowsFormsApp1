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
using Microsoft.Reporting.WinForms;



namespace WindowsFormsApp1.Forms
{


    public partial class FormTopProductos : Form
    {
        private DataTable _lastTop;

        public FormTopProductos()
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";

            InitializeComponent();

            Load += (_, __) =>
            {
                dtpIni.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dtpFin.Value = DateTime.Today;
                numTop.Value = 10;
                Consultar(); // <-- ahora sí existe
            };

            btnConsultar.Click += (_, __) => Consultar();
            btnImprimir.Click += (_, __) => Imprimir();
            this.Controls.Add(btnImprimir);
        }

        private void Consultar()
        {
            int topN = (int)numTop.Value;
            var dt = ConsultarTop(dtpIni.Value, dtpFin.Value, topN);
            grid.DataSource = dt;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true;

            _lastTop = dt; // <-- guarda para imprimir
        }

        private void Imprimir()
        {
            if (_lastTop == null || _lastTop.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para imprimir.");
                return;
            }

            var rdlc = "WindowsFormsApp1.Reports.RptTopProductos.rdlc"; // Embedded
            var dsName = "DsTopProductos"; // Debe coincidir con el RDLC

            var prms = new[]
            {
        new ReportParameter("pFechaIni", dtpIni.Value.ToString("dd/MM/yyyy")),
        new ReportParameter("pFechaFin", dtpFin.Value.ToString("dd/MM/yyyy")),
        new ReportParameter("pTopN", ((int)numTop.Value).ToString()),
        new ReportParameter("pUsuario", WindowsFormsApp1.Security.Session.Username ?? "Desconocido")
    };

            ShowReport(rdlc, dsName, _lastTop, prms, "Top Productos");
        }

        private void ShowReport(string embeddedReportPath, string dataSetName, DataTable table, ReportParameter[] parameters, string tituloVentana)
        {
            var rv = new ReportViewer
            {
                Dock = DockStyle.Fill,
                ProcessingMode = ProcessingMode.Local
            };
            rv.LocalReport.DataSources.Clear();
            rv.LocalReport.ReportEmbeddedResource = embeddedReportPath;
            rv.LocalReport.DataSources.Add(new ReportDataSource(dataSetName, table));
            if (parameters != null && parameters.Length > 0)
                rv.LocalReport.SetParameters(parameters);
            rv.RefreshReport();

            var frm = new Form
            {
                Text = $"Vista previa - {tituloVentana}",
                StartPosition = FormStartPosition.CenterParent,
                Width = 1000,
                Height = 700
            };
            frm.Controls.Add(rv);
            frm.ShowDialog(this);
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
