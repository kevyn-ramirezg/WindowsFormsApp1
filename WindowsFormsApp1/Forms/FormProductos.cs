using System;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Security;

namespace WindowsFormsApp1.Forms
{
    public partial class FormProductos : Form
    {
        private Producto _seleccion;

        public FormProductos()
        {
            InitializeComponent();
            // NO enganches Load aquí; el Designer llama a FormProductos_Load
            // grid_SelectionChanged lo llama el Designer; no es necesario re-enganchar
        }

        // ====== LO QUE PIDE EL DESIGNER ======
        private void FormProductos_Load(object sender, EventArgs e)
        {
            try { Acl.Require(Feature.Productos); }
            catch (UnauthorizedAccessException ex) { MessageBox.Show(ex.Message); Close(); return; }

            ConfigurarGrid();
            ApplyLocalPermissions();
            ConfigurarNumericos();
            CargarCategoriasCombo();
            Cargar();
        }
        private void btnNuevo_Click(object sender, EventArgs e) => Limpiar();
        private void btnGuardar_Click(object sender, EventArgs e) => Guardar();
        private void btnEliminar_Click(object sender, EventArgs e) => Eliminar();
        private void grid_SelectionChanged(object sender, EventArgs e) => RellenarDesdeSeleccion();
        // =====================================

        private void ApplyLocalPermissions()
        {
            bool canCreate = Acl.Can(Feature.ProductosCreate);
            bool canUpdate = Acl.Can(Feature.ProductosUpdate);
            bool canDelete = Acl.Can(Feature.ProductosDelete);
            bool canEdit = canCreate || canUpdate;

            btnNuevo.Enabled = canCreate;
            btnGuardar.Enabled = canEdit;
            btnEliminar.Enabled = canDelete;

            bool ro = !canEdit;
            cboCategoria.Enabled = !ro;
            txtNombre.ReadOnly = ro;
            numCosto.Enabled = !ro;
            numPrecio.Enabled = !ro;
            numStock.Enabled = !ro;
        }

        private void ConfigurarGrid()
        {
            grid.AutoGenerateColumns = false;
            grid.Columns.Clear();

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", DataPropertyName = "Id", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Categoria", HeaderText = "Categoría", DataPropertyName = "Categoria", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CategoriaId", HeaderText = "CategoríaId", DataPropertyName = "CategoriaId", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre", DataPropertyName = "Nombre", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Costo", HeaderText = "Costo", DataPropertyName = "Costo", Width = 90, DefaultCellStyle = { Format = "N2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "PrecioVenta", HeaderText = "Precio Venta", DataPropertyName = "PrecioVenta", Width = 110, DefaultCellStyle = { Format = "N2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", DataPropertyName = "Stock", Width = 90, DefaultCellStyle = { Format = "N2" } });

            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
        }

        private void ConfigurarNumericos()
        {
            numCosto.DecimalPlaces = 2; numCosto.Minimum = 0; numCosto.Maximum = 1_000_000_000m;
            numPrecio.DecimalPlaces = 2; numPrecio.Minimum = 0; numPrecio.Maximum = 1_000_000_000m;
            numStock.DecimalPlaces = 2; numStock.Minimum = 0; numStock.Maximum = 1_000_000_000m;
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
            _seleccion = null;
            if (cboCategoria.Items.Count > 0) cboCategoria.SelectedIndex = 0;
            txtNombre.Text = "";
            numCosto.Value = 0;
            numPrecio.Value = 0;
            numStock.Value = 0;
            grid.ClearSelection();
        }

        private void RellenarDesdeSeleccion()
        {
            if (grid.CurrentRow != null && grid.CurrentRow.DataBoundItem is Producto p)
            {
                _seleccion = p;
                cboCategoria.SelectedValue = p.CategoriaId;
                txtNombre.Text = p.Nombre;

                SetNumericValue(numCosto, p.Costo);
                SetNumericValue(numPrecio, p.PrecioVenta);
                SetNumericValue(numStock, p.Stock);
            }
        }

        private static void SetNumericValue(NumericUpDown ctrl, decimal value)
        {
            if (value < ctrl.Minimum) ctrl.Minimum = value;
            if (value > ctrl.Maximum) ctrl.Maximum = value;
            ctrl.Value = value;
        }

        private void Guardar()
        {
            if (cboCategoria.SelectedValue == null) { MessageBox.Show("Selecciona una categoría"); return; }
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Nombre es obligatorio"); return; }

            var p = new Producto
            {
                CategoriaId = (decimal)cboCategoria.SelectedValue,
                Nombre = txtNombre.Text.Trim(),
                Costo = numCosto.Value,
                PrecioVenta = numPrecio.Value,
                Stock = numStock.Value
            };

            if (_seleccion == null)
            {
                if (!Acl.Can(Feature.ProductosCreate)) { MessageBox.Show("Sin permiso para crear."); return; }
                ProductoDAO.Insertar(p);
            }
            else
            {
                if (!Acl.Can(Feature.ProductosUpdate)) { MessageBox.Show("Sin permiso para actualizar."); return; }
                p.Id = _seleccion.Id;
                ProductoDAO.Actualizar(p);
            }

            Cargar();
        }

        private void Eliminar()
        {
            if (!Acl.Can(Feature.ProductosDelete)) { MessageBox.Show("Sin permiso para eliminar."); return; }
            if (_seleccion == null) { MessageBox.Show("Selecciona un registro"); return; }

            if (MessageBox.Show("¿Eliminar el producto seleccionado?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            ProductoDAO.Eliminar(_seleccion.Id);
            Cargar();
        }
    }
}
