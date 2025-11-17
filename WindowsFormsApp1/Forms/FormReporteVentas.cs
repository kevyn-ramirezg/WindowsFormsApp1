using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using Microsoft.Reporting.WinForms;
using Oracle.ManagedDataAccess.Client;



namespace WindowsFormsApp1.Forms
{

    public partial class FormReporteVentas : Form
    {
        private DataTable _ventasMes;  // lo reutilizamos para ReportViewer y CSV si lo tienes

        public FormReporteVentas()
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";

            InitializeComponent();

            // Eventos
            btnConsultar.Click += btnConsultar_Click;
            btnExportCsv.Click += btnExportCsv_Click;

            this.Load += FormReporteVentas_Load;
        }

        private void FormReporteVentas_Load(object sender, EventArgs e)
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";

            // Rango por defecto: mes actual
            var ini = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var fin = ini.AddMonths(1).AddDays(-1);

            dtpIni.Value = ini;
            dtpFin.Value = fin;

            EstilarGrid(gridVentas);
            EstilarGrid(gridTop);

            // Consulta inicial
            Consultar();
        }

        private void EstilarGrid(DataGridView g)
        {
            g.ReadOnly = true;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.RowHeadersVisible = false;
            g.AllowUserToAddRows = false;
            g.AllowUserToResizeRows = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.BackgroundColor = Color.AliceBlue;

            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 235, 245);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Elephant", 10, FontStyle.Bold);
            g.DefaultCellStyle.Font = new Font("Elephant", 10, FontStyle.Regular);
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
            g.RowTemplate.Height = 28;
        }

        private void btnConsultar_Click(object sender, EventArgs e) => Consultar();

        private void RenderizarReporteVentas(DataTable dt, DateTime ini, DateTime fin)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;

            // Opción recomendada: RDLC como Embedded Resource
            // Si tu proyecto se llama distinto o el RDLC está en carpeta, cambia el namespace:
            reportViewer1.LocalReport.ReportEmbeddedResource = "WindowsFormsApp1.ReporteVentasMes.rdlc";
            // Si prefieres ruta en disco:
            // var ruta = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReporteVentasMes.rdlc");
            // reportViewer1.LocalReport.ReportPath = ruta;

            reportViewer1.LocalReport.DataSources.Clear();
            // IMPORTANTE: "VentasMes" debe ser EXACTAMENTE el nombre del Dataset del RDLC
            reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("VentasMes", dt));

            // Parámetros del encabezado (si los creaste en el RDLC)
            var ps = new[]
            {
        new ReportParameter("FECHA_INICIO", ini.ToString("dd/MM/yyyy")),
        new ReportParameter("FECHA_FIN",    fin.ToString("dd/MM/yyyy"))
    };
            reportViewer1.LocalReport.SetParameters(ps);

            reportViewer1.RefreshReport();
        }



        private void Consultar()
        {
            var ini = dtpIni.Value.Date;
            var fin = dtpFin.Value.Date;


            if (fin < ini)
            {
                MessageBox.Show("La fecha fin no puede ser menor a la fecha inicio.");
                return;
            }

            // 1) Ventas del rango
            DataTable dtVentas = ReporteDAO.VentasCab(ini, fin);
            gridVentas.DataSource = dtVentas;
            FormatearGridVentas();

            // 2) Totales del rango
            var tot = ReporteDAO.VentasTotales(ini, fin);
            lblSubtotal.Text = tot.Subtotal.ToString("N0");
            lblIva.Text = tot.IvaTotal.ToString("N0");
            lblTotal.Text = tot.Total.ToString("N0");

            // 3) Top productos (TOP 10)
            DataTable dtTop = ReporteDAO.TopProductos(ini, fin, 10);
            gridTop.DataSource = dtTop;
            FormatearGridTop();

            _ventasMes = dtVentas;

            // Si tus nombres de columnas NO coinciden con el RDLC, mapea:
            // var dtParaReporte = MapearParaReporte(dtVentas);
            // _ventasMes = dtParaReporte;

            RenderizarReporteVentas(_ventasMes, ini, fin);
        }

        private void FormatearGridVentas()
        {
            var g = gridVentas;
            if (g.Columns.Count == 0) return;

            void W(string name, float weight, string fmt = null, DataGridViewContentAlignment? align = null)
            {
                if (g.Columns[name] == null) return;
                var c = g.Columns[name];
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                c.FillWeight = weight;
                if (!string.IsNullOrEmpty(fmt)) c.DefaultCellStyle.Format = fmt;
                if (align.HasValue) c.DefaultCellStyle.Alignment = align.Value;
            }

            // Ajusta según los alias que retorna ReporteDAO.VentasCab
            // Esperados: ID, FECHA, CLIENTE, TIPO, SUBTOTAL, IVA_TOTAL, TOTAL
            W("ID", 8, null, DataGridViewContentAlignment.MiddleCenter);
            W("FECHA", 12, "dd/MM/yyyy", DataGridViewContentAlignment.MiddleCenter);
            W("CLIENTE", 30, null, DataGridViewContentAlignment.MiddleLeft);
            W("TIPO", 10, null, DataGridViewContentAlignment.MiddleCenter);
            W("SUBTOTAL", 13, "N0", DataGridViewContentAlignment.MiddleRight);
            W("IVA_TOTAL", 13, "N0", DataGridViewContentAlignment.MiddleRight);
            W("TOTAL", 14, "N0", DataGridViewContentAlignment.MiddleRight);
        }

        private void FormatearGridTop()
        {
            var g = gridTop;
            if (g.Columns.Count == 0) return;

            void W(string name, float weight, string fmt = null, DataGridViewContentAlignment? align = null)
            {
                if (g.Columns[name] == null) return;
                var c = g.Columns[name];
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                c.FillWeight = weight;
                if (!string.IsNullOrEmpty(fmt)) c.DefaultCellStyle.Format = fmt;
                if (align.HasValue) c.DefaultCellStyle.Alignment = align.Value;
            }

            // Esperados: PRODUCTO, CANTIDAD, VENTAS
            W("PRODUCTO", 60, null, DataGridViewContentAlignment.MiddleLeft);
            W("CANTIDAD", 20, "N0", DataGridViewContentAlignment.MiddleRight);
            W("VENTAS", 20, "N0", DataGridViewContentAlignment.MiddleRight);
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (gridVentas.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            var sfd = new SaveFileDialog
            {
                Title = "Exportar a CSV",
                Filter = "CSV (*.csv)|*.csv",
                FileName = $"reporte_ventas_{DateTime.Now:yyyyMMdd_HHmm}.csv",
                OverwritePrompt = true
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                var sb = new System.Text.StringBuilder();

                // Encabezados
                for (int i = 0; i < gridVentas.Columns.Count; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(gridVentas.Columns[i].HeaderText.Replace(',', ' '));
                }
                sb.AppendLine();

                // Filas
                foreach (DataGridViewRow row in gridVentas.Rows)
                {
                    if (row.IsNewRow) continue;
                    for (int c = 0; c < gridVentas.Columns.Count; c++)
                    {
                        if (c > 0) sb.Append(',');
                        var v = row.Cells[c].Value?.ToString() ?? "";
                        // escapado básico
                        v = v.Replace("\"", "\"\"");
                        if (v.Contains(",") || v.Contains("\"")) v = $"\"{v}\"";
                        sb.Append(v);
                    }
                    sb.AppendLine();
                }

                System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                MessageBox.Show("Archivo exportado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo exportar: " + ex.Message);
            }
        }


        private void ExportToCsv(DataTable dt, string path)
        {
            var sb = new StringBuilder();

            // Encabezados
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(Escape(dt.Columns[i].ColumnName));
            }
            sb.AppendLine();

            // Filas
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    var val = row[i]?.ToString() ?? "";
                    sb.Append(Escape(val));
                }
                sb.AppendLine();
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);

            string Escape(string s)
            {
                // Encierra en comillas si trae coma o comillas
                if (s.Contains("\"")) s = s.Replace("\"", "\"\"");
                if (s.Contains(",") || s.Contains("\"") || s.Contains("\n")) s = $"\"{s}\"";
                return s;
            }
        }

        private void FormReporteVentas_Load_1(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }
    }
}
