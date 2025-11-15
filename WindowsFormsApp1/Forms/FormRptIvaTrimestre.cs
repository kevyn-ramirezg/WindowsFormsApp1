using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using WindowsFormsApp1.Data;

namespace WindowsFormsApp1.Forms
{
    public partial class FormRptIvaTrimestre : Form
    {
        public FormRptIvaTrimestre()
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";

            InitializeComponent();
            // eventos de UI
            this.Load += FormRptIvaTrimestre_Load;
            btnBuscar.Click += btnBuscar_Click;
            numAnio.ValueChanged += (_, __) => { /* si quieres autorefrescar al cambiar el año: */ /*Cargar();*/ };
        }

        private void FormRptIvaTrimestre_Load(object sender, EventArgs e)
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";

            // Año por defecto = actual
            numAnio.Value = DateTime.Now.Year;

            // Ajustes visuales de la grilla
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AutoGenerateColumns = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;

            // Primera carga
            Cargar();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                using (var cn = Db.Open())
                using (var da = new OracleDataAdapter(
                    "SELECT anio, trimestre, iva_trimestre FROM VW_IVA_TRIMESTRE WHERE anio=:a ORDER BY trimestre", cn))
                {
                    da.SelectCommand.BindByName = true;
                    da.SelectCommand.Parameters.Add(":a", (int)numAnio.Value);

                    var dt = new DataTable();
                    da.Fill(dt);
                    grid.DataSource = dt;

                    if (grid.Columns["anio"] != null) grid.Columns["anio"].HeaderText = "Año";
                    if (grid.Columns["trimestre"] != null) grid.Columns["trimestre"].HeaderText = "Trimestre";
                    if (grid.Columns["iva_trimestre"] != null)
                    {
                        grid.Columns["iva_trimestre"].HeaderText = "IVA Trimestre";
                        grid.Columns["iva_trimestre"].DefaultCellStyle.Format = "N0";
                        grid.Columns["iva_trimestre"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }

                    decimal ivaAnual = dt.AsEnumerable().Sum(r => Convert.ToDecimal(r["iva_trimestre"]));
                    lblTotalIVA.Text = $"IVA total {numAnio.Value}: {ivaAnual:N0}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar IVA por trimestre: " + ex.Message);
            }
        }
    }
}
