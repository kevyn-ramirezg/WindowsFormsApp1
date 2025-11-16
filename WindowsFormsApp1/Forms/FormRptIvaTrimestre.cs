using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Oracle.ManagedDataAccess.Client;
using System.Windows.Forms.DataVisualization.Charting;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Security;

namespace WindowsFormsApp1.Forms
{
    public partial class FormRptIvaTrimestre : Form
    {
        private Chart chart; // gráfico IVA x trimestre

        public FormRptIvaTrimestre()
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";
            InitializeComponent();

            Load += (_, __) =>
            {
                try
                {
                    Acl.Require(Feature.ReporteIvaTrimestre);
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show(ex.Message, "Acceso restringido",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }

                // ...tu lógica actual de inicialización (estilos, Cargar(), etc.)
            };


            // --- Seguridad: esta pantalla NO la ve el nivel esporádico ---
            this.Load += SecureLoad;

            // UI/UX
            this.Load += FormRptIvaTrimestre_Load;
            btnBuscar.Click += (_, __) => Cargar();
            numAnio.ValueChanged += (_, __) => { /*si quieres auto-refrescar: Cargar();*/ };
            numAnio.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { Cargar(); e.Handled = true; e.SuppressKeyPress = true; } };
        }

        // Bloqueo por permisos (usa Feature.ReporteIvaTrimestre)
        private void SecureLoad(object sender, EventArgs e)
        {
            try { Acl.Require(Feature.ReporteIvaTrimestre); }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Acceso restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
        }

        private void FormRptIvaTrimestre_Load(object sender, EventArgs e)
        {
            // Año por defecto = actual
            if (numAnio.Minimum > 1900) numAnio.Minimum = 1900;
            if (numAnio.Maximum < 2100) numAnio.Maximum = 2100;
            numAnio.Value = DateTime.Now.Year;

            EstilarGrid();
            CrearGrafico();
            Cargar();
        }

        private void EstilarGrid()
        {
            var g = grid;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.MultiSelect = false;
            g.RowHeadersVisible = false;

            g.AutoGenerateColumns = true;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 235, 245);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            g.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
        }

        private void CrearGrafico()
        {
            // crea un Chart debajo de la grilla, ocupando el panel grande
            if (chart != null) return;

            chart = new Chart
            {
                Dock = DockStyle.Bottom,
                Height = Math.Max(220, this.ClientSize.Height / 3)
            };

            var area = new ChartArea("ivaArea");
            area.AxisX.Title = "Trimestre";
            area.AxisY.Title = "IVA";
            area.AxisY.LabelStyle.Format = "N0";
            area.AxisX.Interval = 1;
            chart.ChartAreas.Add(area);

            var series = new Series("IVA")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double,
                IsValueShownAsLabel = true,
                LabelFormat = "N0"
            };
            chart.Series.Add(series);

            // Insertamos el chart al final para que quede debajo de la grilla
            this.Controls.Add(chart);
            chart.BringToFront();
        }

        private void Cargar()
        {
            try
            {
                using (var cn = Db.Open())
                using (var da = new OracleDataAdapter(@"
                    SELECT anio, trimestre, iva_trimestre 
                    FROM VW_IVA_TRIMESTRE 
                    WHERE anio = :a 
                    ORDER BY trimestre", cn))
                {
                    da.SelectCommand.BindByName = true;
                    da.SelectCommand.Parameters.Add(":a", (int)numAnio.Value);

                    var dt = new DataTable();
                    da.Fill(dt);

                    // Grid
                    grid.DataSource = dt;
                    if (grid.Columns["ANIO"] != null) grid.Columns["ANIO"].HeaderText = "Año";
                    if (grid.Columns["anio"] != null) grid.Columns["anio"].HeaderText = "Año"; // por si viene en minúscula
                    var colTri = grid.Columns["trimestre"] ?? grid.Columns["TRIMESTRE"];
                    var colIva = grid.Columns["iva_trimestre"] ?? grid.Columns["IVA_TRIMESTRE"];
                    if (colTri != null) colTri.HeaderText = "Trimestre";
                    if (colIva != null)
                    {
                        colIva.HeaderText = "IVA Trimestre";
                        colIva.DefaultCellStyle.Format = "N0";
                        colIva.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        colIva.FillWeight = 50;
                    }

                    // Total
                    decimal ivaAnual = dt.AsEnumerable().Sum(r => Convert.ToDecimal(r["IVA_TRIMESTRE"] is DBNull ? r["iva_trimestre"] : r["IVA_TRIMESTRE"]));
                    lblTotalIVA.Text = $"IVA total {numAnio.Value}: {ivaAnual:N0}";

                    // Chart
                    PoblarGrafico(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar IVA por trimestre: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PoblarGrafico(DataTable dt)
        {
            if (chart == null || chart.Series.Count == 0) return;

            var s = chart.Series[0];
            s.Points.Clear();

            // Asegura 4 barras (Q1..Q4) aunque no haya filas
            decimal[] valores = new decimal[4];
            foreach (DataRow r in dt.Rows)
            {
                int tri = Convert.ToInt32(r["trimestre"] is DBNull ? r["TRIMESTRE"] : r["trimestre"]);
                decimal iva = Convert.ToDecimal(r["IVA_TRIMESTRE"] is DBNull ? r["iva_trimestre"] : r["IVA_TRIMESTRE"]);
                if (tri >= 1 && tri <= 4) valores[tri - 1] = iva;
            }

            string[] labels = { "T1", "T2", "T3", "T4" };
            for (int i = 0; i < 4; i++)
            {
                s.Points.AddXY(labels[i], valores[i]);
            }
        }
    }
}
