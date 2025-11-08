using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;

namespace WindowsFormsApp1.Forms
{
    public partial class FormReporteVentas : Form
    {
        public FormReporteVentas()
        {
            InitializeComponent();

            // Eventos
            btnConsultar.Click += btnConsultar_Click;
            btnExportCsv.Click += btnExportCsv_Click;

            this.Load += FormReporteVentas_Load;
        }

        private void FormReporteVentas_Load(object sender, EventArgs e)
        {
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
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            g.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
            g.RowTemplate.Height = 28;
        }

        private void btnConsultar_Click(object sender, EventArgs e) => Consultar();

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
            var dt = gridVentas.DataSource as DataTable; // C# 7.3 compatible
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            using (var sfd = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = $"ventas_{dtpIni.Value:yyyyMMdd}_{dtpFin.Value:yyyyMMdd}.csv"
            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        ExportToCsv(dt, sfd.FileName);
                        MessageBox.Show("Exportado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error exportando: " + ex.Message);
                    }
                }
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
    }
}
