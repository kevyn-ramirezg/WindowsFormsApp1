using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;

namespace WindowsFormsApp1.Forms
{
    public partial class FormCreditos : Form
    {
        public FormCreditos()
        {
            InitializeComponent();
            // Enlaza eventos (el diseñador también puede hacerlo; así te aseguras)
            btnBuscar.Click += btnBuscar_Click;
            btnPagar.Click += btnPagar_Click;
            gridCuotas.CellDoubleClick += gridCuotas_CellDoubleClick;
            this.Load += FormCreditos_Load;
        }

        private void FormCreditos_Load(object sender, EventArgs e)
        {
            EstilarGrid();
            ConfigurarGridCuotas();
            LimpiarTotales();

        }

        private void EstilarGrid()
        {
            var g = gridCuotas;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.ReadOnly = true;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.RowHeadersVisible = false;
            g.AutoGenerateColumns = true;
            g.BackgroundColor = Color.LightSteelBlue;

            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 246, 255);
            g.ColumnHeadersDefaultCellStyle.Font = new Font(g.Font, FontStyle.Bold);
        }

        private void AjustarColumnas()
        {
            var g = gridCuotas;
            if (g.Columns.Count == 0) return;

            foreach (DataGridViewColumn c in g.Columns)
            {
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                c.FillWeight = 1;
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            void W(string name, float weight, string fmt = null, DataGridViewContentAlignment? align = null)
            {
                if (g.Columns[name] == null) return;
                var c = g.Columns[name];
                c.FillWeight = weight;
                if (!string.IsNullOrEmpty(fmt)) c.DefaultCellStyle.Format = fmt;
                if (align.HasValue) c.DefaultCellStyle.Alignment = align.Value;
            }

            // Ajusta por nombres típicos devueltos por tu consulta
            W("NUM_CUOTA", 60, null, DataGridViewContentAlignment.MiddleCenter);
            W("FECHA_VENC", 90, "d", DataGridViewContentAlignment.MiddleCenter);
            W("VALOR_CUOTA", 100, "N0", DataGridViewContentAlignment.MiddleRight);
            W("VALOR_PAGADO", 100, "N0", DataGridViewContentAlignment.MiddleRight);
            W("ESTADO", 80, null, DataGridViewContentAlignment.MiddleCenter);
        }

        private void LimpiarTotales()
        {
            lblTotal.Text = "0";
            lblPagado.Text = "0";
            lblPendiente.Text = "0";
        }

        private void ConfigurarGridCuotas()
        {
            var g = gridCuotas;

            g.AutoGenerateColumns = false;
            g.Columns.Clear();

            // Estilos generales
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(230, 235, 245);
            g.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            g.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

            g.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            g.RowTemplate.Height = 28;
            g.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 248, 255);

            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.ReadOnly = true;
            g.RowHeadersVisible = false;
            g.AllowUserToResizeRows = false;
            g.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            // Que el ancho se reparta proporcionalmente
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Columnas
            var colNum = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NumCuota",     // propiedad del modelo Cuota
                HeaderText = "Cuota",
                MinimumWidth = 70,
                FillWeight = 12,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            var colFec = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FechaVenc",
                HeaderText = "Vence",
                MinimumWidth = 110,
                FillWeight = 18,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = {
            Format    = "dd/MM/yyyy",
            Alignment = DataGridViewContentAlignment.MiddleCenter
        }
            };

            var colCuota = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ValorCuota",
                HeaderText = "Valor cuota",
                MinimumWidth = 120,
                FillWeight = 30,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = {
            Format    = "N0",                          // 1.234.567
            Alignment = DataGridViewContentAlignment.MiddleRight
        }
            };

            var colPagado = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ValorPagado",
                HeaderText = "Pagado",
                MinimumWidth = 120,
                FillWeight = 30,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = {
            Format    = "N0",
            Alignment = DataGridViewContentAlignment.MiddleRight
        }
            };

            var colEstado = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Estado",
                HeaderText = "Estado",
                MinimumWidth = 100,
                FillWeight = 10,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            g.Columns.AddRange(colNum, colFec, colCuota, colPagado, colEstado);

            // Opcional: ajuste fino para pantallas pequeñas
            foreach (DataGridViewColumn c in g.Columns)
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }


        private void RecalcularTotales()
        {
            decimal total = 0, pagado = 0;

            // Si el grid está ligado a una lista de Cuota
            if (gridCuotas.DataSource is System.Collections.IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    // Asume tu modelo:
                    // class Cuota { public int NumCuota; public DateTime FechaVenc;
                    //               public decimal ValorCuota; public decimal ValorPagado; public string Estado; }
                    if (item is Cuota c)
                    {
                        total += c.ValorCuota;
                        pagado += c.ValorPagado;
                    }
                }
            }

            var pendiente = total - pagado;
            lblTotal.Text = total.ToString("N0");
            lblPagado.Text = pagado.ToString("N0");
            lblPendiente.Text = pendiente.ToString("N0");
        }


        // ---------------- Eventos ----------------

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var ventaId = (int)numVentaId.Value;

            // Llama a tu DAO que devuelve DataTable o lista
            var dt = CreditoDAO.ListarCuotasPorVenta(ventaId); // implementado en tu DAO

            gridCuotas.DataSource = dt;
            AjustarColumnas();
            RecalcularTotales();

            // Sugerencia: posiciona la 1ª vencida
            if (gridCuotas.Columns.Contains("ESTADO"))
            {
                foreach (DataGridViewRow r in gridCuotas.Rows)
                {
                    if ((r.Cells["ESTADO"].Value?.ToString() ?? "").Equals("PENDIENTE", StringComparison.OrdinalIgnoreCase))
                    {
                        r.Selected = true;
                        gridCuotas.CurrentCell = r.Cells[0];
                        break;
                    }
                }
            }
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            if (gridCuotas.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una cuota.");
                return;
            }

            var sel = gridCuotas.CurrentRow.DataBoundItem as Cuota;
            if (sel == null)
            {
                MessageBox.Show("No se pudo obtener la cuota seleccionada.");
                return;
            }

            int ventaId = (int)numVentaId.Value;
            int numCuota = sel.NumCuota;              // <— ¡aquí está el número de cuota!
            decimal monto = numMonto.Value;
            DateTime fecha = dtpFecha.Value.Date;

            if (monto <= 0) { MessageBox.Show("Ingresa un monto mayor que cero."); return; }

            var ok = CreditoDAO.PagarCuota(ventaId, numCuota, monto, fecha);
            if (!ok) { MessageBox.Show("No fue posible registrar el pago."); return; }

            btnBuscar_Click(null, null); // refrescar
        }


        private void gridCuotas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var r = gridCuotas.Rows[e.RowIndex];

            // Si tu consulta trae RESTO_PENDIENTE, úsalo como sugerencia
            if (gridCuotas.Columns.Contains("RESTO_PENDIENTE") && r.Cells["RESTO_PENDIENTE"].Value != null)
            {
                var resto = Convert.ToDecimal(r.Cells["RESTO_PENDIENTE"].Value);
                if (resto > 0)
                {
                    if (resto < numMonto.Minimum) numMonto.Minimum = resto;
                    if (resto > numMonto.Maximum) numMonto.Maximum = resto;
                    numMonto.Value = resto;
                }
            }
        }
    }
}
