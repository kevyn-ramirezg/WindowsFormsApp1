using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using WindowsFormsApp1.Data;
using Microsoft.Reporting.WinForms;



namespace WindowsFormsApp1.Forms
{
    public partial class FormRptVentasMes : Form
    {
        private DataTable _ventasMes;  // lo reutilizamos para ReportViewer y CSV si lo tienes

        public FormRptVentasMes()
        {
            InitializeComponent();

            this.Load += FormRptVentasMes_Load;
            btnBuscar.Click += (_, __) => Cargar();
            numAnio.ValueChanged += (_, __) => { /* si quieres refrescar al cambiar: Cargar(); */ };
        }

        private void FormRptVentasMes_Load(object sender, EventArgs e)
        {
            numAnio.Value = DateTime.Now.Year;

            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AutoGenerateColumns = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Cargar();
        }

        private void Cargar()
        {
            try
            {
                using (var cn = Db.Open())
                using (var da = new OracleDataAdapter(
                    "SELECT anio, mes, num_ventas, total_mes, iva_mes FROM VW_VENTAS_POR_MES WHERE anio = :a ORDER BY mes", cn))
                {
                    da.SelectCommand.BindByName = true;
                    da.SelectCommand.Parameters.Add(":a", (int)numAnio.Value);

                    var dt = new DataTable();
                    da.Fill(dt);

                    // Agregamos una columna “MesNombre” para visual (ene, feb, …)
                    if (!dt.Columns.Contains("MesNombre"))
                        dt.Columns.Add("MesNombre", typeof(string));

                    foreach (DataRow r in dt.Rows)
                    {
                        r["MesNombre"] = NombreMes(Convert.ToInt32(r["mes"]));
                    }

                    // Reordenamos: MesNombre primero
                    grid.DataSource = null;
                    grid.DataSource = dt;

                    if (grid.Columns["MesNombre"] != null)
                    {
                        grid.Columns["MesNombre"].DisplayIndex = 0;
                        grid.Columns["MesNombre"].HeaderText = "Mes";
                        grid.Columns["MesNombre"].FillWeight = 18;
                    }

                    if (grid.Columns["anio"] != null)
                    {
                        grid.Columns["anio"].HeaderText = "Año";
                        grid.Columns["anio"].FillWeight = 12;
                    }
                    if (grid.Columns["mes"] != null)
                    {
                        grid.Columns["mes"].HeaderText = "Nº Mes";
                        grid.Columns["mes"].FillWeight = 12;
                    }
                    if (grid.Columns["num_ventas"] != null)
                    {
                        grid.Columns["num_ventas"].HeaderText = "Ventas";
                        grid.Columns["num_ventas"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        grid.Columns["num_ventas"].FillWeight = 18;
                    }
                    if (grid.Columns["total_mes"] != null)
                    {
                        grid.Columns["total_mes"].HeaderText = "Total Mes";
                        grid.Columns["total_mes"].DefaultCellStyle.Format = "N0";
                        grid.Columns["total_mes"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        grid.Columns["total_mes"].FillWeight = 25;
                    }
                    if (grid.Columns["iva_mes"] != null)
                    {
                        grid.Columns["iva_mes"].HeaderText = "IVA Mes";
                        grid.Columns["iva_mes"].DefaultCellStyle.Format = "N0";
                        grid.Columns["iva_mes"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        grid.Columns["iva_mes"].FillWeight = 15;
                    }

                    decimal totalAnio = dt.AsEnumerable().Sum(r => r.Field<decimal?>("total_mes") ?? 0m);
                    decimal ivaAnio = dt.AsEnumerable().Sum(r => r.Field<decimal?>("iva_mes") ?? 0m);
                    lblResumen.Text = $"Total año {numAnio.Value}: {totalAnio:N0}    IVA año: {ivaAnio:N0}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar Ventas por Mes: " + ex.Message);
            }
        }

        private static string NombreMes(int m)
        {
            switch (m)
            {
                case 1: return "Enero";
                case 2: return "Febrero";
                case 3: return "Marzo";
                case 4: return "Abril";
                case 5: return "Mayo";
                case 6: return "Junio";
                case 7: return "Julio";
                case 8: return "Agosto";
                case 9: return "Septiembre";
                case 10: return "Octubre";
                case 11: return "Noviembre";
                case 12: return "Diciembre";
                default: return m.ToString();
            }
        }
    }
}
