using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.Models;
using Oracle.ManagedDataAccess.Client;
using WindowsFormsApp1.Data;

namespace WindowsFormsApp1
{
    public partial class FormVenta : Form
    {
        // -----------------------------
        // Modelo de fila para el grid
        // -----------------------------
       /* private class ItemGrid
        {
            public decimal ProductoId { get; set; }
            public string Nombre { get; set; }
            public decimal Cantidad { get; set; }   // guardamos como decimal para pasar a DAO sin cast raros
            public decimal PrecioUnit { get; set; }
            public decimal Subtotal { get { return PrecioUnit * Cantidad; } }
        }*/

        //private BindingList<ItemGrid> carrito = new BindingList<ItemGrid>();

        private List<Producto> productosCache = new List<Producto>();
        // Lista en memoria para el carrito
        private readonly List<ItemCarrito> _items = new List<ItemCarrito>();
    

        public FormVenta()
        {
            InitializeComponent();
            this.Load += FormVenta_Load;
            // opcional si tienes un botón “Quitar”
            cboProducto.SelectedIndexChanged += (s, e) => ActualizarPrecioDeProducto();

            rbContado.CheckedChanged += (_, __) => ToggleCredito();
            rbCredito.CheckedChanged += (_, __) => ToggleCredito();

            cboMeses.Items.Clear();
            cboMeses.Items.AddRange(new object[] { 12, 18, 24 });
            if (cboMeses.Items.Count > 0) cboMeses.SelectedIndex = 0;

            // Asegurar columnas del grid (una sola vez)
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

            // eventos
            numCantidad.ValueChanged += delegate { Recalcular(); };
        }

        private void FormVenta_Load(object sender, EventArgs e)
        {
            // clientes
            var clientes = ClienteDAO.Listar();          // <- usa tu ClienteDAO
            cboCliente.DataSource = clientes;
            cboCliente.DisplayMember = "Nombre";
            cboCliente.ValueMember = "Id";
            cboCliente.SelectedIndex = clientes.Count > 0 ? 0 : -1;

            // productos
            productosCache = ProductoDAO.Listar();       // <- usa tu ProductoDAO
            cboProducto.DataSource = productosCache;
            cboProducto.DisplayMember = "Nombre";
            cboProducto.ValueMember = "Id";
            cboProducto.SelectedIndex = productosCache.Count > 0 ? 0 : -1;

            cboMeses.SelectedIndexChanged += (_, __) => { if (rbCredito.Checked) PrevisualizarCuotas(); };

            ToggleCredito();

            // precio inicial
            ActualizarPrecioDeProducto();
            Recalcular();
        }

        // -----------------------------
        // Utilidades de UI
        // -----------------------------

        private void ToggleCredito()
        {
            bool esCred = rbCredito.Checked;
            cboMeses.Enabled = esCred;
            lblCuotaInicial.Visible = esCred;
            lblCuotaMensual.Visible = esCred;

            if (esCred) PrevisualizarCuotas();
        }

        private void PrevisualizarCuotas()
        {
            // Total actual (ya lo calculas con _items)
            decimal total = _items.Sum(i => i.Total);

            // Reglas del enunciado:
            //  - cuota inicial = 30% del total
            //  - se financia el 70% + 5% de interés (simple para el total financiado)
            //  - meses: 12/18/24 según combo
            if (total <= 0 || cboMeses.SelectedItem == null)
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

        private void ActualizarPrecioDeProducto()
        {
            var p = cboProducto.SelectedItem as Producto;
            if (p != null)
            {
                // muestra el precio del producto. Si prefieres permitir editarlo, deja txtPrecio editable.
                txtPrecio.Text = p.PrecioVenta.ToString("0.##", CultureInfo.CurrentCulture);
            }
        }

        private void Recalcular()
        {
            decimal total = _items.Sum(i => i.Total);   // si prefieres solo Subtotal: i.Subtotal
            lblTotal.Text = "Total: " + total.ToString("C", CultureInfo.CurrentCulture);
        }


        private void cboProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarPrecioDeProducto();
        }

        // -----------------------------
        // Botón Agregar
        // -----------------------------
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

            // productosCache es tu lista actual. Puede ser tipada o DataTable.
            // Usamos dynamic para no romper tus tipos existentes.
            dynamic prod = cboProducto.SelectedItem;

            decimal prodId = (decimal)prod.Id;
            string nombre = (string)prod.Nombre;
            decimal precio = (decimal)prod.PrecioVenta;
            decimal stock = (decimal)prod.Stock;

            // IVA: si tu objeto no trae IvaPct, lo consultamos a BD por la categoría.
            decimal ivaPct = 0m;
            try { ivaPct = (decimal)prod.IvaPct; } catch { ivaPct = ObtenerIvaDeProducto(prodId); }

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



        // -----------------------------
        // Botón Guardar
        // -----------------------------
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_items.Count == 0) { MessageBox.Show("Carrito vacío."); return; }
            if (cboCliente.SelectedItem == null) { MessageBox.Show("Selecciona un cliente."); return; }

            // 1) Cliente
            dynamic cli = cboCliente.SelectedItem;
            int clienteId = Convert.ToInt32(cli.Id);

            // 2) Cabecera: tipo según radio
            var cab = new VentaCab
            {
                ClienteId = clienteId,
                Tipo = rbCredito.Checked ? "CREDITO" : "CONTADO"   // ⬅️ aquí se define el tipo
            };

            // 3) Detalles (como ya los tienes)
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

            // 4) Plan de crédito solo si aplica
            PlanCreditoInfo plan = null;
            if (rbCredito.Checked)
            {
                if (cboMeses.SelectedItem == null)
                {
                    MessageBox.Show("Selecciona meses (12 / 18 / 24).");
                    return;
                }
                plan = new PlanCreditoInfo
                {
                    Meses = Convert.ToInt32(cboMeses.SelectedItem)
                };
            }

            // 5) Guardar (llama SP_RECALCULAR_TOTALES y, si hay plan, SP_CONFIGURAR_CREDITO)
            var ventaId = VentaDAO.Guardar(cab, detalles, plan);

            MessageBox.Show($"Venta #{ventaId} guardada correctamente.");

            // 6) Limpiar UI
            _items.Clear();
            RefrescarGridYTotales();
            rbContado.Checked = true;          // vuelve a contado por defecto
                                               // si quieres, también: cboMeses.SelectedIndex = 0;
        }




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
            gridCarrito.AutoGenerateColumns = false; // usamos tus columnas manuales
            gridCarrito.DataSource = null;
            gridCarrito.DataSource = _items.Select(x => new
            {
                x.ProductoId,
                x.Nombre,
                x.Cantidad,
                x.PrecioUnit,
                x.Subtotal
            }).ToList();

            var total = _items.Sum(i => i.Total);   // si solo quieres Subtotal, usa i.Subtotal
            lblTotal.Text = "Total: " + total.ToString("C", CultureInfo.CurrentCulture);

            if (rbCredito.Checked) PrevisualizarCuotas();
        }

        // Saca IVA de la categoría cuando el producto de tu lista no lo trae
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
