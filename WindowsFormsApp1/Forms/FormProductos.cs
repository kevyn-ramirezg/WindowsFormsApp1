using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Forms
{
    public partial class FormProductos : Form
    {
        private Producto seleccion; // puede quedar null

        public FormProductos()
        {
            InitializeComponent();
        }

        private void FormProductos_Load(object sender, EventArgs e)
        {
            // grid
            grid.AutoGenerateColumns = false;
            if (grid.Columns.Count == 0)
            {
                grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", DataPropertyName = "Id", Width = 40 });
                grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Categoria", HeaderText = "Categoría", DataPropertyName = "Categoria", Width = 80 });
                grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CategoriaId", HeaderText = "CategoríaId", DataPropertyName = "CategoriaId", Width = 80 });
                grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre", DataPropertyName = "Nombre", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Costo", HeaderText = "Costo", DataPropertyName = "Costo", Width = 80 });
                grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrecioVenta", HeaderText = "Precio Venta", DataPropertyName = "PrecioVenta", Width = 100 });
                grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", DataPropertyName = "Stock", Width = 90 });
            }

            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;

            ConfigurarNumericos();

            // combos y datos
            CargarCategoriasCombo();
            Cargar();
        }

        private void CargarCategoriasCombo()
        {
            var lista = ProductoDAO.ListarCategoriasCombo();
            cboCategoria.DataSource = lista;
            cboCategoria.DisplayMember = "Nombre";
            cboCategoria.ValueMember = "Id";
        }

        private void Cargar()
        {
            grid.DataSource = ProductoDAO.Listar();
            Limpiar();
        }


        private void Limpiar()
        {
            seleccion = null;
            if (cboCategoria.Items.Count > 0) cboCategoria.SelectedIndex = 0;
            txtNombre.Text = "";
            numCosto.Value = 0;
            numPrecio.Value = 0;
            numStock.Value = 0;
            grid.ClearSelection();
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow != null && grid.CurrentRow.DataBoundItem is Producto p)
            {
                seleccion = p;
                cboCategoria.SelectedValue = p.CategoriaId;
                txtNombre.Text = p.Nombre;

                SetNumericValue(numCosto, p.Costo);
                SetNumericValue(numPrecio, p.PrecioVenta);
                SetNumericValue(numStock, p.Stock);
            }
        }

        private void SetNumericValue(NumericUpDown ctrl, decimal value)
        {
            if (value < ctrl.Minimum) ctrl.Minimum = value;
            if (value > ctrl.Maximum) ctrl.Maximum = value;
            ctrl.Value = value;
        }

        private void ConfigurarNumericos()
        {
            // Costo
            numCosto.DecimalPlaces = 2;
            numCosto.Minimum = 0;
            numCosto.Maximum = 1000000000m; // 1,000,000,000

            // Precio
            numPrecio.DecimalPlaces = 2;
            numPrecio.Minimum = 0;
            numPrecio.Maximum = 1000000000m;

            // Stock (si manejas enteros, deja DecimalPlaces = 0)
            numStock.DecimalPlaces = 2; // o 0 si es entero
            numStock.Minimum = 0;
            numStock.Maximum = 1000000000m;
        }

        private void btnNuevo_Click(object sender, EventArgs e) => Limpiar();

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cboCategoria.SelectedValue == null)
            {
                MessageBox.Show("Selecciona una categoría"); return;
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Nombre es obligatorio"); return;
            }

            var p = new Producto();
            p.CategoriaId = (decimal)cboCategoria.SelectedValue;
            p.Nombre = txtNombre.Text.Trim();
            p.Costo = numCosto.Value;
            p.PrecioVenta = numPrecio.Value;
            p.Stock = numStock.Value;

            if (seleccion == null)  // insertar
                ProductoDAO.Insertar(p);
            else                    // actualizar
            {
                p.Id = seleccion.Id;
                ProductoDAO.Actualizar(p);
            }

            Cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (seleccion == null)
            {
                MessageBox.Show("Selecciona un registro"); return;
            }
            if (MessageBox.Show("¿Eliminar el producto seleccionado?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ProductoDAO.Eliminar(seleccion.Id);
                Cargar();
            }
        }
    }
}
