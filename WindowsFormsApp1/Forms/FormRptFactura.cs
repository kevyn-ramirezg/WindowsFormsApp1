using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using WindowsFormsApp1.Data;
using Microsoft.Reporting.WinForms; // ← importante


namespace WindowsFormsApp1.Forms
{
    public partial class FormRptFactura : Form
    {

        private DataTable _ultimaFactura;   // guardará el dt para recargar el visor
        public FormRptFactura()
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";

            InitializeComponent();

            this.Load += FormRptFactura_Load;
            btnBuscar.Click += (_, __) => Cargar();
        }

        private void FormRptFactura_Load(object sender, EventArgs e)
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";

            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AutoGenerateColumns = true;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Primera carga con el valor actual del numeric
            Cargar();
        }

        private void Cargar()
        {
            int ventaId = (int)numVentaId.Value;

            try
            {
                using (var cn = Db.Open())
                using (var da = new OracleDataAdapter(
                    "SELECT * FROM VW_FACTURA WHERE venta_id = :v ORDER BY producto_id", cn))
                {
                    da.SelectCommand.BindByName = true;
                    da.SelectCommand.Parameters.Add(":v", ventaId);

                    var dt = new DataTable();
                    da.Fill(dt);

                    // --- columnas que sí existan en tu vista ---
                    // (en Oracle, el DataAdapter trae los nombres en MAYÚSCULAS)
                    bool hasIva = dt.Columns.Contains("IVA") || dt.Columns.Contains("LINEA_IVA");
                    bool hasSubtotal = dt.Columns.Contains("SUBTOTAL") || dt.Columns.Contains("LINEA_SUBTOTAL");

                    // Columnas “canónicas” para mostrar en el grid
                    if (!dt.Columns.Contains("IVA_MOSTRAR"))
                        dt.Columns.Add("IVA_MOSTRAR", typeof(decimal));
                    if (!dt.Columns.Contains("SUBTOTAL_MOSTRAR"))
                        dt.Columns.Add("SUBTOTAL_MOSTRAR", typeof(decimal));

                    foreach (DataRow r in dt.Rows)
                    {
                        // IVA
                        decimal iva = 0m;
                        if (dt.Columns.Contains("IVA") && r["IVA"] != DBNull.Value)
                            iva = Convert.ToDecimal(r["IVA"]);
                        else if (dt.Columns.Contains("LINEA_IVA") && r["LINEA_IVA"] != DBNull.Value)
                            iva = Convert.ToDecimal(r["LINEA_IVA"]);
                        r["IVA_MOSTRAR"] = iva;

                        // SUBTOTAL
                        decimal subtotal = 0m;
                        if (dt.Columns.Contains("SUBTOTAL") && r["SUBTOTAL"] != DBNull.Value)
                            subtotal = Convert.ToDecimal(r["SUBTOTAL"]);
                        else if (dt.Columns.Contains("LINEA_SUBTOTAL") && r["LINEA_SUBTOTAL"] != DBNull.Value)
                            subtotal = Convert.ToDecimal(r["LINEA_SUBTOTAL"]);
                        else
                        {
                            // respaldo: cantidad * precio_unit
                            var cantidad = Convert.ToDecimal(r["CANTIDAD"]);
                            var precioUnit = Convert.ToDecimal(r["PRECIO_UNIT"]);
                            subtotal = Math.Round(cantidad * precioUnit, 2);
                        }
                        r["SUBTOTAL_MOSTRAR"] = subtotal;
                    }

                    var vista = BuildFacturaView(dt);   // ← proyección sin columnas duplicadas
                    grid.DataSource = vista;
                    ConfigurarGridFactura();


                    if (dt.Rows.Count == 0)
                    {
                        lblTotales.Text = "Sin datos para esa venta.";
                        return;
                    }

                    SetHeadersAndFormats();  // mapea a *_MOSTRAR

                    // Totales de cabecera (vienen repetidos por fila en la vista)
                    decimal total = dt.AsEnumerable().Max(r => Convert.ToDecimal(r["TOTAL"]));
                    decimal ivaTot = dt.AsEnumerable().Max(r => Convert.ToDecimal(r["IVA_TOTAL"]));
                    decimal sub = total - ivaTot;
                    lblTotales.Text = $"Subtotal: {sub:N0}   IVA: {ivaTot:N0}   Total: {total:N0}";
                    _ultimaFactura = dt;
                    RenderizarReporte();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar factura: " + ex.Message);
            }
        }

        private void RenderizarReporte()
        {
            // Si no hay datos, limpia el visor y sal.
            if (_ultimaFactura == null || _ultimaFactura.Rows.Count == 0)
            {
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.RefreshReport();
                return;
            }

            // 1) Modo local
            reportViewer1.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Local;

            // 2) Ruta del RDLC (ajústala si lo moviste de carpeta)
            reportViewer1.LocalReport.ReportEmbeddedResource = "WindowsFormsApp1.Reports.ReporteFactura.rdlc";
            // Si lo pusiste como Embedded Resource, usa:
            // reportViewer1.LocalReport.ReportEmbeddedResource = "WindowsFormsApp1.ReporteFactura.rdlc";

            // 3) Enlazar datos
            reportViewer1.LocalReport.DataSources.Clear();

            // OJO: este nombre "Factura" DEBE ser EXACTO al del conjunto de datos en tu RDLC
            reportViewer1.LocalReport.DataSources.Add(
                new Microsoft.Reporting.WinForms.ReportDataSource("Factura", _ultimaFactura)
            );

            // 4) Refrescar
            reportViewer1.RefreshReport();
        }


        private void SetHeadersAndFormats()
        {
            // Reordena columnas para que primero salgan datos de cabecera y luego líneas
            TryHeader("venta_id", "Venta #", displayIndex: 0, widthWeight: 10, center: true);
            TryHeader("fecha", "Fecha", displayIndex: 1, widthWeight: 16);
            TryHeader("tipo", "Tipo", displayIndex: 2, widthWeight: 10, center: true);
            TryHeader("cliente", "Cliente", displayIndex: 3, widthWeight: 30);

            TryHeader("producto_id", "Id Prod", displayIndex: 4, widthWeight: 10, center: true);
            TryHeader("producto", "Producto", displayIndex: 5, widthWeight: 40);

            TryMoney("precio_unit", "Precio Unit", displayIndex: 6, widthWeight: 16);
            TryNumber("cantidad", "Cant", displayIndex: 7, widthWeight: 10, center: true);

            // ...
            TryMoney("SUBTOTAL_MOSTRAR", "Subtotal", displayIndex: 8, widthWeight: 18);
            TryMoney("IVA_MOSTRAR", "IVA", displayIndex: 9, widthWeight: 14);
            // y deja igual IVA_TOTAL y TOTAL:
            TryMoney("IVA_TOTAL", "IVA Total", displayIndex: 10, widthWeight: 14);
            TryMoney("TOTAL", "Total Venta", displayIndex: 11, widthWeight: 18);

        }

        // Helpers para no repetir código
        private void TryHeader(string name, string header, int displayIndex, int widthWeight, bool center = false)
        {
            if (grid.Columns[name] == null) return;
            var c = grid.Columns[name];
            c.HeaderText = header;
            c.DisplayIndex = displayIndex;
            c.FillWeight = widthWeight;
            c.DefaultCellStyle.Alignment = center
                ? DataGridViewContentAlignment.MiddleCenter
                : DataGridViewContentAlignment.MiddleLeft;
        }

        // Toma la columna "preferida": si existe A usa A, de lo contrario usa B.
        private static object Pick(DataRow r, string prefer, string fallback)
        {
            return r.Table.Columns.Contains(prefer) ? r[prefer] : r[fallback];
        }

        // Construye un DataTable "vista" solo con las columnas que queremos mostrar
        private static DataTable BuildFacturaView(DataTable src)
        {
            var dt = new DataTable();
            dt.Columns.Add("Fecha", typeof(DateTime));
            dt.Columns.Add("Cliente", typeof(string));
            dt.Columns.Add("Producto", typeof(string));
            dt.Columns.Add("Precio", typeof(decimal));
            dt.Columns.Add("Cantidad", typeof(decimal));
            dt.Columns.Add("Subtotal", typeof(decimal));  // solo uno
            dt.Columns.Add("IVA", typeof(decimal));  // solo uno
            dt.Columns.Add("Total línea", typeof(decimal));

            foreach (DataRow r in src.Rows)
            {
                dt.Rows.Add(
                    r["FECHA"],
                    r["CLIENTE"],
                    r["PRODUCTO"],
                    r["PRECIO_UNIT"],
                    r["CANTIDAD"],
                    Pick(r, "LINEA_SUBTOTAL", "SUBTOTAL"),
                    Pick(r, "LINEA_IVA", "IVA"),
                    Pick(r, "LINEA_TOTAL", "TOTAL")
                );
            }
            return dt;
        }


        private void ConfigurarGridFactura()
        {
            var g = grid;

            g.RowHeadersVisible = false;
            g.AllowUserToResizeRows = false;
            g.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            // Repartimos el ancho disponible
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Formatos y pesos por columna
            void SetCol(string name, int weight, string fmt = null,
                        DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleLeft,
                        int minWidth = 60)
            {
                var c = g.Columns[name];
                if (c == null) return;
                c.FillWeight = weight;
                c.MinimumWidth = minWidth;
                c.DefaultCellStyle.Alignment = align;
                if (!string.IsNullOrEmpty(fmt)) c.DefaultCellStyle.Format = fmt;
            }

            // Pesos pensados para que los textos no se corten
            SetCol("Fecha", 12, "d"); // fecha corta
            SetCol("Cliente", 18);
            SetCol("Producto", 32, null, DataGridViewContentAlignment.MiddleLeft, 200);
            SetCol("Precio", 12, "N0", DataGridViewContentAlignment.MiddleRight);
            SetCol("Cant.", 8, "N2", DataGridViewContentAlignment.MiddleRight);
            SetCol("Subt.", 12, "N0", DataGridViewContentAlignment.MiddleRight);
            SetCol("IVA", 10, "N0", DataGridViewContentAlignment.MiddleRight);
            SetCol("Total línea", 14, "N0", DataGridViewContentAlignment.MiddleRight);

            // Encabezados sin salto
            g.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
        }



        private void TryMoney(string name, string header, int displayIndex, int widthWeight)
        {
            if (grid.Columns[name] == null) return;
            var c = grid.Columns[name];
            c.HeaderText = header;
            c.DisplayIndex = displayIndex;
            c.FillWeight = widthWeight;
            c.DefaultCellStyle.Format = "N0";
            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void TryNumber(string name, string header, int displayIndex, int widthWeight, bool center = false)
        {
            if (grid.Columns[name] == null) return;
            var c = grid.Columns[name];
            c.HeaderText = header;
            c.DisplayIndex = displayIndex;
            c.FillWeight = widthWeight;
            c.DefaultCellStyle.Format = "N2";
            c.DefaultCellStyle.Alignment = center
                ? DataGridViewContentAlignment.MiddleCenter
                : DataGridViewContentAlignment.MiddleRight;
        }
    }
}
