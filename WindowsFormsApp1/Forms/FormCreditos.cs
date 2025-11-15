using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Security;

namespace WindowsFormsApp1.Forms
{
    public partial class FormCreditos : Form
    {
        public FormCreditos()
        {
            InitializeComponent();

            this.Load += (s, e) =>
            {
                try { Acl.Require(Feature.Creditos); }
                catch (UnauthorizedAccessException ex) { MessageBox.Show(ex.Message); Close(); return; }

                // Estética y básicos
                EstilarGrid();
                ConfigurarGridCuotas();
                LimpiarTotales();

                // Numeric monto
                numMonto.DecimalPlaces = 0;
                numMonto.ThousandsSeparator = true;
                numMonto.Maximum = 999_999_999;

                // Permisos locales (pagar cuota = acción granular)
                btnPagar.Enabled = Acl.Can(Feature.CreditosPagar);
            };

            btnBuscar.Click += btnBuscar_Click;
            btnPagar.Click += btnPagar_Click;
            gridCuotas.CellDoubleClick += gridCuotas_CellDoubleClick;
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

        private void ConfigurarGridCuotas()
        {
            var g = gridCuotas;

            g.AutoGenerateColumns = false;
            g.Columns.Clear();

            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 235, 245);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            g.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            g.RowTemplate.Height = 28;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);

            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.ReadOnly = true;
            g.RowHeadersVisible = false;
            g.AllowUserToResizeRows = false;
            g.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // DataPropertyName = nombres esperados en el modelo Cuota o DataTable devuelto
            var colNum = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NumCuota",
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
                DefaultCellStyle = { Format = "dd/MM/yyyy", Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            var colCuota = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ValorCuota",
                HeaderText = "Valor cuota",
                MinimumWidth = 120,
                FillWeight = 30,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            };

            var colPagado = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ValorPagado",
                HeaderText = "Pagado",
                MinimumWidth = 120,
                FillWeight = 30,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
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

            foreach (DataGridViewColumn c in g.Columns)
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void AjustarColumnas()
        {
            var g = gridCuotas;
            if (g.Columns.Count == 0) return;

            void W(string name, float weight, string fmt = null, DataGridViewContentAlignment? align = null)
            {
                if (g.Columns[name] == null) return;
                var c = g.Columns[name];
                c.FillWeight = weight;
                if (!string.IsNullOrEmpty(fmt)) c.DefaultCellStyle.Format = fmt;
                if (align.HasValue) c.DefaultCellStyle.Alignment = align.Value;
            }

            W("NumCuota", 12, null, DataGridViewContentAlignment.MiddleCenter);
            W("FechaVenc", 18, "dd/MM/yyyy", DataGridViewContentAlignment.MiddleCenter);
            W("ValorCuota", 30, "N0", DataGridViewContentAlignment.MiddleRight);
            W("ValorPagado", 30, "N0", DataGridViewContentAlignment.MiddleRight);
            W("Estado", 10, null, DataGridViewContentAlignment.MiddleCenter);
        }

        private void LimpiarTotales()
        {
            lblTotal.Text = "0";
            lblPagado.Text = "0";
            lblPendiente.Text = "0";
        }

        private void RecalcularTotales()
        {
            decimal total = 0, pagado = 0;

            if (gridCuotas.DataSource is System.Collections.IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    if (item is Cuota c) // tu modelo Cuota
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

        // ------------ Eventos ------------

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var ventaId = (int)numVentaId.Value;

            var cuotas = CreditoDAO.ListarCuotasPorVenta(ventaId); // List<Cuota> o DataTable compatible
            gridCuotas.DataSource = cuotas;

            AjustarColumnas();
            RecalcularTotales();

            // Selecciona primera PENDIENTE
            foreach (DataGridViewRow r in gridCuotas.Rows)
            {
                var c = r.DataBoundItem as Cuota;
                if (c != null && string.Equals(c.Estado, "PENDIENTE", StringComparison.OrdinalIgnoreCase))
                {
                    r.Selected = true;
                    gridCuotas.CurrentCell = r.Cells[0];
                    break;
                }
            }
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            if (!Acl.Can(Feature.CreditosPagar)) { MessageBox.Show("Sin permiso para pagar cuotas."); return; }

            if (gridCuotas.CurrentRow == null) { MessageBox.Show("Selecciona una cuota."); return; }

            var sel = gridCuotas.CurrentRow.DataBoundItem as Cuota;
            if (sel == null) { MessageBox.Show("No se pudo leer la cuota."); return; }

            int ventaId = (int)numVentaId.Value;
            int numCuota = sel.NumCuota;
            decimal monto = numMonto.Value;
            DateTime fecha = dtpFecha.Value.Date;

            if (monto <= 0) { MessageBox.Show("Ingresa un monto mayor que cero."); return; }

            var ok = CreditoDAO.PagarCuota(ventaId, numCuota, monto, fecha);
            if (!ok) return;

            btnBuscar_Click(null, null);
        }

        private void gridCuotas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = gridCuotas.Rows[e.RowIndex];
            var sel = row.DataBoundItem as Cuota;
            if (sel == null) return;

            decimal resto = Math.Max(0m, sel.ValorCuota - sel.ValorPagado);

            numMonto.ThousandsSeparator = true;
            numMonto.DecimalPlaces = 0;

            if (resto > 0m)
            {
                if (resto > numMonto.Maximum) numMonto.Maximum = resto;
                if (numMonto.Minimum > 0m) numMonto.Minimum = 0m;
                numMonto.Value = resto;
            }
            else
            {
                numMonto.Value = 0m;
            }

            if (dtpFecha.Value.Date < DateTime.Today)
                dtpFecha.Value = DateTime.Today;

            gridCuotas.ClearSelection();
            row.Selected = true;
            gridCuotas.CurrentCell = row.Cells[0];
        }
    }
}
