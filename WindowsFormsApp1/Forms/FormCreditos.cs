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

            // ⇩ Ajustes del NumericUpDown de monto
            numMonto.DecimalPlaces = 0;     
            numMonto.ThousandsSeparator = true;
            numMonto.Maximum = 999_999_999;

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

        // Crea las columnas 1 sola vez con los nombres que usamos en el código
        /*private void EnsureGridColumns()
        {
            if (gridCuotas.Columns.Count > 0) return;

            gridCuotas.AutoGenerateColumns = false;

            gridCuotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NUM_CUOTA",            // <- este Name es el que usas para leer la celda al pagar
                HeaderText = "Cuota",
                DataPropertyName = "NUM_CUOTA"
            });
            gridCuotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FECHA_VENC",
                HeaderText = "Vence",
                DataPropertyName = "FECHA_VENC"
            });
            gridCuotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VALOR_CUOTA",
                HeaderText = "Valor cuota",
                DataPropertyName = "VALOR_CUOTA"
            });
            gridCuotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VALOR_PAGADO",
                HeaderText = "Pagado",
                DataPropertyName = "VALOR_PAGADO"
            });
            gridCuotas.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ESTADO",
                HeaderText = "Estado",
                DataPropertyName = "ESTADO"
            });
        }*/


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

            // OJO: DataPropertyName = nombres de columna del DataTable
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

            // Ajuste fino uniforme
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

            var cuotas = CreditoDAO.ListarCuotasPorVenta(ventaId); // List<Cuota>
            gridCuotas.DataSource = cuotas;

            AjustarColumnas();
            RecalcularTotales();

            // Selecciona la primera PENDIENTE
            foreach (DataGridViewRow r in gridCuotas.Rows)
            {
                if (r.DataBoundItem is Cuota c &&
                    string.Equals(c.Estado, "PENDIENTE", StringComparison.OrdinalIgnoreCase))
                {
                    r.Selected = true;
                    gridCuotas.CurrentCell = r.Cells[0];
                    break;
                }
            }
        }



        private void btnPagar_Click(object sender, EventArgs e)
        {
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

            // Fila y objeto enlazado
            var row = gridCuotas.Rows[e.RowIndex];
            var sel = row.DataBoundItem as Cuota;
            if (sel == null) return;

            // Resto pendiente calculado desde el modelo
            decimal resto = Math.Max(0m, sel.ValorCuota - sel.ValorPagado);

            // Sugerencias de UI para el NumericUpDown
            numMonto.ThousandsSeparator = true;   // ya lo tienes en Load, por si acaso
            numMonto.DecimalPlaces = 0;

            // Ajusta límites y propone el resto pendiente como valor por defecto
            if (resto > 0m)
            {
                if (resto > numMonto.Maximum) numMonto.Maximum = resto; // asegura tope
                if (numMonto.Minimum > 0m) numMonto.Minimum = 0m;   // permitir abonos parciales si quieres
                numMonto.Value = resto;                                   // propone pagar el restante
            }
            else
            {
                numMonto.Value = 0m;
            }

            // Fecha sugerida: hoy (si tu DateTimePicker quedó en una fecha pasada)
            if (dtpFecha.Value.Date < DateTime.Today)
                dtpFecha.Value = DateTime.Today;

            // Mantén la fila seleccionada
            gridCuotas.ClearSelection();
            row.Selected = true;
            gridCuotas.CurrentCell = row.Cells[0];
        }

    }
}
