using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Security;

namespace WindowsFormsApp1
{
    public partial class FormVenta : Form
    {
        private List<Producto> productosCache = new List<Producto>();
        private readonly List<ItemCarrito> _items = new List<ItemCarrito>();

        public FormVenta()
        {
            InitializeComponent();
            Load += SecureLoad_Ventas;
            this.Load += FormVenta_Load;

            // Precio al cambiar producto
            cboProducto.SelectedIndexChanged += (s, e) => ActualizarPrecioDeProducto();

            // ====== Controles de crédito (creados por código) ======
            // rbContado
            this.rbContado = new RadioButton();
            this.rbContado.AutoSize = true;
            this.rbContado.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.rbContado.Location = new System.Drawing.Point(170, 470);
            this.rbContado.Name = "rbContado";
            this.rbContado.Size = new System.Drawing.Size(100, 21);
            this.rbContado.TabIndex = 14;
            this.rbContado.TabStop = true;
            this.rbContado.Text = "Contado";
            this.rbContado.UseVisualStyleBackColor = true;
            this.rbContado.Checked = true;

            // rbCredito
            this.rbCredito = new RadioButton();
            this.rbCredito.AutoSize = true;
            this.rbCredito.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.rbCredito.Location = new System.Drawing.Point(260, 470);
            this.rbCredito.Name = "rbCredito";
            this.rbCredito.Size = new System.Drawing.Size(78, 21);
            this.rbCredito.TabIndex = 15;
            this.rbCredito.Text = "Crédito";
            this.rbCredito.UseVisualStyleBackColor = true;

            // cboMeses
            this.cboMeses = new ComboBox();
            this.cboMeses.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboMeses.BackColor = System.Drawing.SystemColors.Info;
            this.cboMeses.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cboMeses.Location = new System.Drawing.Point(360, 470);
            this.cboMeses.Name = "cboMeses";
            this.cboMeses.Size = new System.Drawing.Size(120, 28);
            this.cboMeses.TabIndex = 16;
            this.cboMeses.Visible = false;

            // lblCuotaInicial
            this.lblCuotaInicial = new Label();
            this.lblCuotaInicial.AutoSize = true;
            this.lblCuotaInicial.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblCuotaInicial.Location = new System.Drawing.Point(515, 470);
            this.lblCuotaInicial.Name = "lblCuotaInicial";
            this.lblCuotaInicial.Size = new System.Drawing.Size(142, 20);
            this.lblCuotaInicial.TabIndex = 17;
            this.lblCuotaInicial.Text = "Cuota inicial: $0";
            this.lblCuotaInicial.Visible = false;

            // lblCuotaMensual
            this.lblCuotaMensual = new Label();
            this.lblCuotaMensual.AutoSize = true;
            this.lblCuotaMensual.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblCuotaMensual.Location = new System.Drawing.Point(700, 470);
            this.lblCuotaMensual.Name = "lblCuotaMensual";
            this.lblCuotaMensual.Size = new System.Drawing.Size(165, 20);
            this.lblCuotaMensual.TabIndex = 18;
            this.lblCuotaMensual.Text = "Cuota mensual: $0";
            this.lblCuotaMensual.Visible = false;

            // Agregar a la forma
            this.Controls.Add(this.rbContado);
            this.Controls.Add(this.rbCredito);
            this.Controls.Add(this.cboMeses);
            this.Controls.Add(this.lblCuotaInicial);
            this.Controls.Add(this.lblCuotaMensual);
            // ========================================================

            // Config columnas del grid una sola vez
            gridCarrito.AutoGenerateColumns = false;
            if (gridCarrito.Columns.Count == 0)
            {
                gridCarrito.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ProductoId",
                    HeaderText = "ProductoId",
                    DataPropertyName = "ProductoId",
                    Width = 110
                });
                gridCarrito.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Nombre",
                    HeaderText = "Nombre",
                    DataPropertyName = "Nombre",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });
                gridCarrito.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Cantidad",
                    HeaderText = "Cantidad",
                    DataPropertyName = "Cantidad",
                    Width = 100
                });
                gridCarrito.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "PrecioUnit",
                    HeaderText = "PrecioUnit",
                    DataPropertyName = "PrecioUnit",
                    Width = 110
                });
                gridCarrito.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Subtotal",
                    HeaderText = "Subtotal",
                    DataPropertyName = "Subtotal",
                    Width = 110,
                    ReadOnly = true
                });
            }

            numCantidad.ValueChanged += (s, e) => Recalcular();
        }

        private void SecureLoad_Ventas(object sender, EventArgs e)
        {
            try { Acl.Require(Feature.Ventas); }
            catch (UnauthorizedAccessException ex) { MessageBox.Show(ex.Message); Close(); }
        }

        private void FormVenta_Load(object sender, EventArgs e)
        {
            // Cargar combos
            var clientes = ClienteDAO.Listar();
            cboCliente.DataSource = clientes;
            cboCliente.DisplayMember = "Nombre";
            cboCliente.ValueMember = "Id";
            cboCliente.SelectedIndex = clientes.Count > 0 ? 0 : -1;

            productosCache = ProductoDAO.Listar();
            cboProducto.DataSource = productosCache;
            cboProducto.DisplayMember = "Nombre";
            cboProducto.ValueMember = "Id";
            cboProducto.SelectedIndex = productosCache.Count > 0 ? 0 : -1;

            // Eventos crédito
            rbCredito.CheckedChanged += rbCredito_CheckedChanged;
            cboMeses.SelectedIndexChanged += (_, __) =>
            {
                if (rbCredito.Checked) PrevisualizarCuotas();
            };

            rbContado.Checked = true; // default
            ToggleCredito();

            // Precio inicial
            ActualizarPrecioDeProducto();
            Recalcular();
        }

        // ======== Lógica de crédito (sin lblPlanInfo) ========
        private void rbCredito_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCredito();

            if (rbCredito.Checked && cboMeses.Items.Count == 0)
            {
                cboMeses.Items.Add(12);
                cboMeses.Items.Add(18);
                cboMeses.Items.Add(24);
                cboMeses.SelectedIndex = 0;
            }
        }

        private void ToggleCredito()
        {
            bool esCred = rbCredito.Checked;
            cboMeses.Visible = esCred;
            lblCuotaInicial.Visible = esCred;
            lblCuotaMensual.Visible = esCred;

            if (esCred) PrevisualizarCuotas();
            else
            {
                lblCuotaInicial.Text = "Cuota inicial: $0";
                lblCuotaMensual.Text = "Cuota mensual: $0";
            }
        }

        private void PrevisualizarCuotas()
        {
            decimal total = _items.Sum(i => i.Total);
            if (!rbCredito.Checked || total <= 0 || cboMeses.SelectedItem == null)
            {
                lblCuotaInicial.Text = "Cuota inicial: $0";
                lblCuotaMensual.Text = "Cuota mensual: $0";
                return;
            }

            int meses = Convert.ToInt32(cboMeses.SelectedItem);
            decimal cuotaInicial = Math.Round(total * 0.30m, 2);
            decimal capitalFinanciar = Math.Round(total * 0.70m * 1.05m, 2); // +5%
            decimal cuotaMensual = Math.Round(capitalFinanciar / meses, 2);

            lblCuotaInicial.Text = $"Cuota inicial: {cuotaInicial:N0}";
            lblCuotaMensual.Text = $"Cuota mensual: {cuotaMensual:N0}";
        }
        // =====================================================

        private void ActualizarPrecioDeProducto()
        {
            var p = cboProducto.SelectedItem as Producto;
            if (p != null)
                txtPrecio.Text = p.PrecioVenta.ToString("0.##", CultureInfo.CurrentCulture);
        }

        private void Recalcular()
        {
            decimal total = _items.Sum(i => i.Total);
            lblTotal.Text = "Total: " + total.ToString("C", CultureInfo.CurrentCulture);
        }

        private void cboProducto_SelectedIndexChanged(object sender, EventArgs e)
            => ActualizarPrecioDeProducto();

        // ======== Agregar ========
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cboProducto.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }
            if (numCantidad.Value <= 0)
            {
                MessageBox.Show("Cantidad inválida.");
                return;
            }

            dynamic prod = cboProducto.SelectedItem;

            decimal prodId = (decimal)prod.Id;
            string nombre = (string)prod.Nombre;
            decimal precio = (decimal)prod.PrecioVenta;
            decimal stock = (decimal)prod.Stock;

            decimal ivaPct = 0m;
            try { ivaPct = (decimal)prod.IvaPct; }
            catch { ivaPct = ObtenerIvaDeProducto(prodId); }

            decimal qty = numCantidad.Value;

            var existente = _items.FirstOrDefault(x => x.ProductoId == prodId);
            var qtyActual = existente?.Cantidad ?? 0m;

            if (qtyActual + qty > stock)
            {
                MessageBox.Show($"Stock insuficiente. Disponible: {(stock - qtyActual):N0}");
                return;
            }

            if (existente != null)
            {
                existente.Cantidad += qty;
                RecalcularLinea(existente);
            }
            else
            {
                var item = new ItemCarrito
                {
                    ProductoId = prodId,
                    Nombre = nombre,
                    Cantidad = qty,
                    PrecioUnit = precio,
                    Stock = stock,
                    IvaPct = ivaPct
                };
                RecalcularLinea(item);
                _items.Add(item);
            }

            RefrescarGridYTotales();
        }

        // ======== Guardar ========
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_items.Count == 0) { MessageBox.Show("Carrito vacío."); return; }
            if (cboCliente.SelectedItem == null) { MessageBox.Show("Selecciona un cliente."); return; }

            dynamic cli = cboCliente.SelectedItem;
            int clienteId = Convert.ToInt32(cli.Id);

            var cab = new VentaCab
            {
                ClienteId = clienteId,
                Tipo = rbCredito.Checked ? "CREDITO" : "CONTADO"
            };

            var detalles = _items.Select(x => new VentaDet
            {
                ProductoId = Convert.ToInt32(x.ProductoId),
                Cantidad = x.Cantidad,
                PrecioUnit = x.PrecioUnit,
                IvaPct = x.IvaPct,
                UtilidadPct = 0,
                LineaSubtotal = x.Subtotal,
                LineaIva = x.Iva,
                LineaTotal = x.Total
            }).ToList();

            PlanCreditoInfo plan = null;
            if (rbCredito.Checked)
            {
                if (cboMeses.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona meses (12 / 18 / 24).");
                    return;
                }
                plan = new PlanCreditoInfo { Meses = Convert.ToInt32(cboMeses.SelectedItem) };
            }

            var ventaId = VentaDAO.Guardar(cab, detalles, plan);
            MessageBox.Show($"Venta #{ventaId} guardada correctamente.");

            _items.Clear();
            RefrescarGridYTotales();
            rbContado.Checked = true; // default
        }

        // ======== Quitar ========
        private void btnQuitar_Click(object sender, EventArgs e)
        {
            if (gridCarrito.CurrentRow == null) return;

            var col = gridCarrito.Columns["ProductoId"];
            if (col == null) { MessageBox.Show("Columna ProductoId no existe en el grid."); return; }

            var idObj = gridCarrito.CurrentRow.Cells[col.Index].Value;
            if (idObj == null) return;

            var prodId = Convert.ToDecimal(idObj);
            var it = _items.FirstOrDefault(x => x.ProductoId == prodId);
            if (it != null) _items.Remove(it);

            RefrescarGridYTotales();
        }

        private void RecalcularLinea(ItemCarrito it)
        {
            it.Subtotal = Math.Round(it.PrecioUnit * it.Cantidad, 2);
            it.Iva = Math.Round(it.Subtotal * (it.IvaPct / 100m), 2);
            it.Total = it.Subtotal + it.Iva;
        }

        private void RefrescarGridYTotales()
        {
            gridCarrito.AutoGenerateColumns = false;
            gridCarrito.DataSource = null;
            gridCarrito.DataSource = _items.Select(x => new
            {
                x.ProductoId,
                x.Nombre,
                x.Cantidad,
                x.PrecioUnit,
                x.Subtotal
            }).ToList();

            var total = _items.Sum(i => i.Total);
            lblTotal.Text = "Total: " + total.ToString("C", CultureInfo.CurrentCulture);

            if (rbCredito.Checked) PrevisualizarCuotas();
        }

        private decimal ObtenerIvaDeProducto(decimal productoId)
        {
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(@"
                SELECT c.iva
                FROM producto p
                JOIN categoria c ON c.id = p.categoria_id
                WHERE p.id = :id", cn))
            {
                cmd.BindByName = true;
                cmd.Parameters.Add(":id", productoId);
                var v = cmd.ExecuteScalar();
                return v == null ? 0m : Convert.ToDecimal(v);
            }
        }
    }
}
