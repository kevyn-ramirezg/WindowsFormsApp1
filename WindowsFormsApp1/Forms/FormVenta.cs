using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1
{
    public partial class FormVenta : Form
    {
        // -----------------------------
        // Modelo de fila para el grid
        // -----------------------------
        private class ItemGrid
        {
            public decimal ProductoId { get; set; }
            public string Nombre { get; set; }
            public decimal Cantidad { get; set; }   // guardamos como decimal para pasar a DAO sin cast raros
            public decimal PrecioUnit { get; set; }
            public decimal Subtotal { get { return PrecioUnit * Cantidad; } }
        }

        private BindingList<ItemGrid> carrito = new BindingList<ItemGrid>();
        private List<Producto> productosCache = new List<Producto>();

        public FormVenta()
        {
            InitializeComponent();

            // Asegurar columnas del grid por código si no existen
            gridCarrito.AutoGenerateColumns = false;
            if (gridCarrito.Columns.Count == 0)
            {
                gridCarrito.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "ProductoId",
                    DataPropertyName = "ProductoId",
                    Width = 90
                });
                gridCarrito.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Nombre",
                    DataPropertyName = "Nombre",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });
                gridCarrito.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Cantidad",
                    DataPropertyName = "Cantidad",
                    Width = 90
                });
                gridCarrito.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "PrecioUnit",
                    DataPropertyName = "PrecioUnit",
                    Width = 100
                });
                gridCarrito.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Subtotal",
                    DataPropertyName = "Subtotal",
                    Width = 100,
                    ReadOnly = true
                });
            }
            gridCarrito.DataSource = carrito;


            // eventos
            btnAgregar.Click += btnAgregar_Click;
            btnGuardar.Click += btnGuardar_Click;
            cboProducto.SelectedIndexChanged += cboProducto_SelectedIndexChanged;
            numCantidad.ValueChanged += delegate { Recalcular(); };
        }

        private void FormVenta_Load(object sender, EventArgs e)
        {
            // clientes
            var clientes = VentaDAO.ListarClientes();
            cboCliente.DataSource = clientes;
            cboCliente.DisplayMember = "Nombre";
            cboCliente.ValueMember = "Id";
            cboCliente.SelectedIndex = clientes.Count > 0 ? 0 : -1;

            // productos
            productosCache = VentaDAO.ListarProductos();
            cboProducto.DataSource = productosCache;
            cboProducto.DisplayMember = "Nombre";
            cboProducto.ValueMember = "Id";
            cboProducto.SelectedIndex = productosCache.Count > 0 ? 0 : -1;

            // precio inicial
            ActualizarPrecioDeProducto();
            Recalcular();
        }

        // -----------------------------
        // Utilidades de UI
        // -----------------------------
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
            decimal total = 0m;
            for (int i = 0; i < carrito.Count; i++)
                total += carrito[i].Subtotal;

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
            var p = cboProducto.SelectedItem as Producto;
            if (p == null)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            // cantidad
            int cantidadEntera = (int)numCantidad.Value;
            if (cantidadEntera <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor a cero.");
                return;
            }

            // precio
            decimal precio;
            if (!decimal.TryParse(txtPrecio.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out precio))
            {
                MessageBox.Show("Precio inválido.");
                return;
            }

            // agrega o acumula (si ya existe el mismo producto en el carrito)
            var existente = carrito.FirstOrDefault(x => x.ProductoId == p.Id);
            if (existente != null)
            {
                existente.Cantidad += cantidadEntera;
                // notificar al grid
                var idx = carrito.IndexOf(existente);
                carrito.ResetItem(idx);
            }
            else
            {
                carrito.Add(new ItemGrid
                {
                    ProductoId = p.Id,
                    Nombre = p.Nombre,
                    Cantidad = cantidadEntera,
                    PrecioUnit = precio
                });
            }

            Recalcular();
        }

        // -----------------------------
        // Botón Guardar
        // -----------------------------
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var c = cboCliente.SelectedItem as Cliente;
            if (c == null)
            {
                MessageBox.Show("Seleccione un cliente.");
                return;
            }

            if (carrito.Count == 0)
            {
                MessageBox.Show("Agregue al menos un ítem.");
                return;
            }

            // armar listas DECIMAL para el DAO
            var listProd = carrito.Select(i => i.ProductoId).ToList();                 // ya es decimal
            var listCant = carrito.Select(i => i.Cantidad).ToList();                   // ya es decimal
            var listPrec = carrito.Select(i => i.PrecioUnit).ToList();                 // ya es decimal

            try
            {
                var ventaId = VentaDAO.RegistrarContado(c.Id, listProd, listCant, listPrec);

                MessageBox.Show("Venta registrada. ID: " + ventaId);

                // limpiar carrito
                carrito.Clear();
                Recalcular();

                // refrescar productos (para ver stock actualizado)
                productosCache = VentaDAO.ListarProductos();
                cboProducto.DataSource = null;
                cboProducto.DataSource = productosCache;
                cboProducto.DisplayMember = "Nombre";
                cboProducto.ValueMember = "Id";
                ActualizarPrecioDeProducto();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la venta: " + ex.Message);
            }
        }
    }
}
