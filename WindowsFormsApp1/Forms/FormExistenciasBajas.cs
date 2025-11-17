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
using System.Windows.Forms.DataVisualization.Charting;
using WindowsFormsApp1.Utils;
using Microsoft.Reporting.WinForms;
using Microsoft.Reporting.WinForms;
using System.Data;


namespace WindowsFormsApp1.Forms
{
    public partial class FormExistenciasBajas : Form
    {
        private DataTable _lastExistencias;

        public FormExistenciasBajas()
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";

            InitializeComponent();

            btnImprimir.Left = btnConsultar.Right + 8;
            btnImprimir.Top = btnConsultar.Top;
            btnImprimir.Anchor = btnConsultar.Anchor;
            btnImprimir.Click += (_, __) => Imprimir();

            // Si el botón está en otra barra/panel, añade en ese contenedor:
            this.Controls.Add(btnImprimir);

            Load += (_, __) => { numUmbral.Value = 5; Consultar(); };
            btnConsultar.Click += (_, __) => Consultar();
        }


        private void Consultar()
        {
            using (var cn = Data.Db.Open())
            using (var da = new OracleDataAdapter(@"
        SELECT id, nombre, stock, precio_venta
        FROM producto
        WHERE stock <= :u
        ORDER BY stock ASC, nombre", cn))
            {
                da.SelectCommand.BindByName = true;
                da.SelectCommand.Parameters.Add(":u", (int)numUmbral.Value);
                var dt = new DataTable();
                da.Fill(dt);
                grid.DataSource = dt;

                _lastExistencias = dt; // <-- guarda para imprimir
            }
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true;
        }

        private void Imprimir()
        {
            if (_lastExistencias == null || _lastExistencias.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para imprimir.");
                return;
            }

            var rdlc = "WindowsFormsApp1.Reports.RptExistenciasBajas.rdlc"; // Embedded
            var dsName = "DsExistenciasBajas"; // Debe coincidir con el RDLC

            var prms = new[]
            {
        new ReportParameter("pUmbral", numUmbral.Value.ToString()),
        new ReportParameter("pFecha", DateTime.Now.ToString("dd/MM/yyyy HH:mm")),
        new ReportParameter("pUsuario", WindowsFormsApp1.Security.Session.Username ?? "Desconocido")
    };

            ShowReport(rdlc, dsName, _lastExistencias, prms, "Existencias Bajas");
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



        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (_lastExistencias == null || _lastExistencias.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para imprimir.");
                return;
            }

            var pars = new Dictionary<string, string>
            {
                ["pUmbral"] = numUmbral.Value.ToString(),
                ["pFecha"] = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                ["pUsuario"] = WindowsFormsApp1.Security.Session.Username
            };

            // El path debe coincidir con el Namespace + carpeta de tu RDLC embebido
            ReportUtils.ShowLocalReport(this,
                "WindowsFormsApp1.Reports.RptExistenciasBajas.rdlc", // Namespace + carpeta + nombre
                "DsExistenciasBajas",
                _lastExistencias,
                pars,
                "Reporte de Existencias Bajas");
        }
    }
}

