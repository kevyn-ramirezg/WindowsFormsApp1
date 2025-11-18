using Microsoft.Reporting.WinForms;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    public partial class FormMorosos : Form
    {
        public FormMorosos()
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";
            InitializeComponent();

            // Estética mínima sugerida (opcional)
            TryApplyElephant();

            // Enganchar eventos de UI
            Load += (s, e) => Consultar();
            btnImprimir.Click += (s, e) => ImprimirMorosos();
            btnActualizar.Click += (s, e) => Consultar();
            btnExportar.Click += (s, e) => ExportarCsv();
            txtBuscar.TextChanged += (s, e) => Consultar();
            numMinDias.ValueChanged += (s, e) => Consultar();

            // Formateo del grid
            grid.CellFormatting += Grid_CellFormatting; // para colorear por días vencidos
        }

        // ===== Botón imprimir =====
        private void FormMorosos_Load(object sender, EventArgs e)
        {
           
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            var dt = grid?.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para imprimir.");
                return;
            }

            MostrarReporteMorosos(dt);
        }

        private void MostrarReporteMorosos(DataTable table)
        {
            var rv = new ReportViewer
            {
                Dock = DockStyle.Fill,
                ProcessingMode = ProcessingMode.Local
            };

            rv.LocalReport.DataSources.Clear();

            // Carga por recurso embebido (NO mezclar con ReportPath)
            rv.LocalReport.ReportEmbeddedResource = "WindowsFormsApp1.Reports.RptMorosos.rdlc";

            // 👇 OJO: nombre EXACTO del DataSet como está en el RDLC
            rv.LocalReport.DataSources.Add(new ReportDataSource("DsMorosos", table));

            // Parámetros (seguro)
            var definidos = rv.LocalReport.GetParameters()
                                          .Select(p => p.Name)
                                          .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var plist = new List<ReportParameter>();
            string usuario = WindowsFormsApp1.Security.Session.Username ?? "Desconocido";
            var filtro = $"Cliente: {(txtBuscar.Text?.Trim().Length > 0 ? txtBuscar.Text.Trim() : "(todos)")}" +
             $"   |   Mín. días vencido: {numMinDias.Value}";

            if (definidos.Contains("pFiltro"))
                plist.Add(new ReportParameter("pFiltro", filtro));


            if (definidos.Contains("ptitulo"))
                plist.Add(new ReportParameter("ptitulo", "Reporte de morosos"));
            if (definidos.Contains("pusuario"))
                plist.Add(new ReportParameter("pusuario", usuario));
            if (definidos.Contains("pfecha"))
                plist.Add(new ReportParameter("pfecha", DateTime.Now.ToString("dd/MM/yyyy HH:mm")));

            if (plist.Count > 0)
                rv.LocalReport.SetParameters(plist);

            rv.RefreshReport();

            var frm = new Form
            {
                Text = "Vista previa - Morosos",
                StartPosition = FormStartPosition.CenterParent,
                WindowState = FormWindowState.Maximized
            };
            frm.Controls.Add(rv);
            frm.ShowDialog(this);
        }




        private void ImprimirMorosos()
{
    var dt = grid?.DataSource as DataTable;
    if (dt == null || dt.Rows.Count == 0)
    {
        MessageBox.Show("No hay datos para imprimir.");
        return;
    }

    var filtro = $"Cliente: {(txtBuscar.Text?.Trim().Length > 0 ? txtBuscar.Text.Trim() : "(todos)")}   |   Mín. días vencido: {numMinDias.Value}";

    var pars = new[]
    {
        new ReportParameter("ptitulo",  "Reporte de morosos"),
        new ReportParameter("pusuario", WindowsFormsApp1.Security.Session.Username ?? "Desconocido"),
        new ReportParameter("pfecha",   DateTime.Now.ToString("dd/MM/yyyy HH:mm")),
        new ReportParameter("pFiltro",  filtro)
    };

    ShowReport(
        embeddedReportPath: "WindowsFormsApp1.Reports.RptMorosos.rdlc",
        dataSetName:        "DsMorosos",
        table:              dt.Copy(),
        parameters:         pars,
        tituloVentana:      "Morosos - Vista previa"
    );
}


        private void ShowReport(
            string embeddedReportPath,
            string dataSetName,
            DataTable table,
            ReportParameter[] parameters = null,
            string tituloVentana = "Vista previa")
        {
            var rv = new ReportViewer
            {
                Dock = DockStyle.Fill,
                ProcessingMode = ProcessingMode.Local
            };

            rv.LocalReport.DataSources.Clear();
            rv.LocalReport.ReportEmbeddedResource = embeddedReportPath;  // embebido

            // 👇 nombre EXACTO del DataSet del RDLC
            rv.LocalReport.DataSources.Add(new ReportDataSource(dataSetName, table));

            // ✅ Set de parámetros seguro (solo los que existan)
            if (parameters != null && parameters.Length > 0)
            {
                var definidos = rv.LocalReport.GetParameters()
                                              .Select(p => p.Name)
                                              .ToHashSet(StringComparer.OrdinalIgnoreCase);
                var apply = parameters.Where(p => definidos.Contains(p.Name)).ToArray();
                if (apply.Length > 0)
                    rv.LocalReport.SetParameters(apply);
            }

            rv.RefreshReport();

            var frm = new Form
            {
                Text = tituloVentana,
                StartPosition = FormStartPosition.CenterParent,
                WindowState = FormWindowState.Maximized
            };
            frm.Controls.Add(rv);
            frm.ShowDialog(this);
        }


        private void TryApplyElephant()
        {
            try
            {
                var uiFont = new Font("Elephant", 10f, FontStyle.Regular);
                this.Font = uiFont;
                txtBuscar.Font = uiFont;
                numMinDias.Font = uiFont;
                btnActualizar.Font = uiFont;
                btnExportar.Font = uiFont;
                lblResumen.Font = uiFont;

                // Botones blancos
                StyleWhiteButton(btnActualizar);
                StyleWhiteButton(btnExportar);
            }
            catch { /* si no está la fuente, ignora */ }
        }

        private void StyleWhiteButton(Button b)
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = Color.DarkGreen;
            b.BackColor = Color.White;
            b.ForeColor = Color.Black;
            b.Padding = new Padding(14, 6, 14, 6);
            b.UseVisualStyleBackColor = false;
            b.AutoSize = true;
        }

        private void Consultar()
        {
            // 1) Abrir conexión y preparar consulta
            using (var cn = Data.Db.Open())
            using (var cmd = new OracleCommand(@"
                SELECT cliente_id, cliente, venta_id, nro_cuota, fecha_venc,
                       valor, estado, dias_vencidos
                FROM vw_morosos
                WHERE (:pNombre IS NULL OR UPPER(cliente) LIKE '%'||UPPER(:pNombre)||'%')
                  AND dias_vencidos >= :pDias
                ORDER BY cliente, venta_id, nro_cuota", cn))
            {
                cmd.BindByName = true;

                // 2) Parámetros
                var nombre = (txtBuscar.Text ?? "").Trim();
                if (nombre.Length == 0)
                    cmd.Parameters.Add(":pNombre", OracleDbType.Varchar2).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(":pNombre", OracleDbType.Varchar2).Value = nombre;

                cmd.Parameters.Add(":pDias", OracleDbType.Int32).Value = (int)numMinDias.Value;

                // 3) Llenar DataTable
                using (var da = new OracleDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    grid.DataSource = dt;
                    FormatearGrid(dt);
                    ActualizarResumen(dt);
                }
            }
        }

        private void FormatearGrid(DataTable dt)
        {
            var g = grid;

            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.ReadOnly = true;
            g.RowHeadersVisible = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.MultiSelect = false;
            g.BorderStyle = BorderStyle.FixedSingle;

            // Encabezados/formatos si existen esas columnas
            SetHeader("cliente_id", "ID Cliente");
            SetHeader("cliente", "Cliente");
            SetHeader("venta_id", "Venta");
            SetHeader("nro_cuota", "Cuota #");
            SetHeader("fecha_venc", "Fecha venc.");
            SetHeader("valor", "Valor");
            SetHeader("estado", "Estado");
            SetHeader("dias_vencidos", "Días vencidos");

            FormatDate("fecha_venc");
            FormatMoneyRight("valor");
            AlignCenter("dias_vencidos");

            void SetHeader(string name, string header)
            {
                var c = g.Columns[name] ?? g.Columns[name.ToUpper()] ?? g.Columns[name.ToLower()];
                if (c != null) c.HeaderText = header;
            }
            void FormatDate(string name)
            {
                var c = g.Columns[name] ?? g.Columns[name.ToUpper()] ?? g.Columns[name.ToLower()];
                if (c != null) c.DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            void FormatMoneyRight(string name)
            {
                var c = g.Columns[name] ?? g.Columns[name.ToUpper()] ?? g.Columns[name.ToLower()];
                if (c != null)
                {
                    c.DefaultCellStyle.Format = "N0";
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    c.FillWeight = 60;
                }
            }
            void AlignCenter(string name)
            {
                var c = g.Columns[name] ?? g.Columns[name.ToUpper()] ?? g.Columns[name.ToLower()];
                if (c != null)
                {
                    c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    c.FillWeight = 50;
                }
            }
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var g = (DataGridView)sender;
            var col = g.Columns["dias_vencidos"] ?? g.Columns["DIAS_VENCIDOS"];
            if (col == null || e.RowIndex < 0) return;

            var cell = g.Rows[e.RowIndex].Cells[col.Index];
            if (cell.Value is IConvertible conv)
            {
                int d = 0;
                try { d = conv.ToInt32(System.Globalization.CultureInfo.InvariantCulture); } catch { }

                // 0-29: amarillo suave, 30-89: naranja, 90+: rojo suave
                Color back = Color.White;
                if (d >= 90) back = Color.FromArgb(255, 235, 235);
                else if (d >= 30) back = Color.FromArgb(255, 245, 230);
                else if (d >= 1) back = Color.FromArgb(255, 250, 220);

                g.Rows[e.RowIndex].DefaultCellStyle.BackColor = back;
            }
        }

        private void ActualizarResumen(DataTable dt)
        {
            if (lblResumen == null) return;

            if (dt == null || dt.Rows.Count == 0)
            {
                lblResumen.Text = "Morosos: 0";
                return;
            }

            int n = dt.Rows.Count;
            decimal total = 0m;

            if (dt.Columns.Contains("valor"))
            {
                foreach (DataRow r in dt.Rows)
                    if (r["valor"] != DBNull.Value) total += Convert.ToDecimal(r["valor"]);
            }

            lblResumen.Text = $"Morosos: {n:N0}   |   Total adeudado: {total:N0}";
        }

        private void ExportarCsv()
        {
            var dt = grid.DataSource as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            using (var sfd = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "morosos.csv" })
            {
                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    var cols = dt.Columns.Cast<DataColumn>().ToList();
                    using (var sw = new System.IO.StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        // encabezado
                        sw.WriteLine(string.Join(",", cols.Select(c => "\"" + c.ColumnName.Replace("\"", "\"\"") + "\"")));
                        // filas
                        foreach (DataRow r in dt.Rows)
                        {
                            var vals = cols.Select(c =>
                            {
                                var v = r[c] == DBNull.Value ? "" : r[c].ToString();
                                return "\"" + (v ?? "").Replace("\"", "\"\"") + "\"";
                            });
                            sw.WriteLine(string.Join(",", vals));
                        }
                    }
                    MessageBox.Show("Exportado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message);
                }
            }
        }

        // (Opcional) doble clic en la fila para mostrar detalle rápido
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            grid.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex < 0) return;
                var r = grid.Rows[ev.RowIndex];
                var ventaId = (r.Cells["venta_id"] ?? r.Cells["VENTA_ID"]).Value;
                var cuota = (r.Cells["nro_cuota"] ?? r.Cells["NRO_CUOTA"]).Value;
                var cliente = (r.Cells["cliente"] ?? r.Cells["CLIENTE"]).Value;
                MessageBox.Show($"Cliente: {cliente}\nVenta: {ventaId}\nCuota: {cuota}", "Detalle moroso");
            };
        }
    }
}
