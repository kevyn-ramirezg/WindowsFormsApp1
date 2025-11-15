using System;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Security;

namespace WindowsFormsApp1.Forms
{
    public partial class FormCategorias : Form
    {
        private Categoria _seleccion;

        public FormCategorias()
        {
            InitializeComponent();
            // NO enganches Load aquí; el Designer llama a FormCategorias_Load
            // grid_SelectionChanged sí lo engancha el Designer
        }

        // ====== LO QUE PIDE EL DESIGNER ======
        private void FormCategorias_Load(object sender, EventArgs e)
        {
            try { Acl.Require(Feature.Categorias); }
            catch (UnauthorizedAccessException ex) { MessageBox.Show(ex.Message); Close(); return; }

            ConfigurarGrid();
            ApplyLocalPermissions();
            Cargar();
        }
        private void btnNuevo_Click(object sender, EventArgs e) => Limpiar();
        private void btnGuardar_Click(object sender, EventArgs e) => Guardar();
        private void btnEliminar_Click(object sender, EventArgs e) => Eliminar();
        private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e) { /* opcional */ }
        // =====================================

        private void ConfigurarGrid()
        {
            grid.AutoGenerateColumns = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
        }

        private void ApplyLocalPermissions()
        {
            bool canCreate = Acl.Can(Feature.CategoriasCreate);
            bool canUpdate = Acl.Can(Feature.CategoriasUpdate);
            bool canDelete = Acl.Can(Feature.CategoriasDelete);
            bool canEdit = canCreate || canUpdate;

            btnNuevo.Enabled = canCreate;
            btnGuardar.Enabled = canEdit;
            btnEliminar.Enabled = canDelete;

            bool ro = !canEdit;
            txtNombre.ReadOnly = ro;
            numIva.Enabled = !ro;
            numUtilidad.Enabled = !ro;
        }

        private void Cargar()
        {
            grid.DataSource = CategoriaDAO.Listar();

            if (grid.Columns["Id"] != null) grid.Columns["Id"].HeaderText = "ID";
            if (grid.Columns["Nombre"] != null) grid.Columns["Nombre"].HeaderText = "Nombre";
            if (grid.Columns["Iva"] != null) grid.Columns["Iva"].HeaderText = "IVA (%)";
            if (grid.Columns["UtilidadPct"] != null) grid.Columns["UtilidadPct"].HeaderText = "Utilidad (%)";

            AjustarGrid();
            Limpiar();
        }

        private void AjustarGrid()
        {
            var g = grid;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.AllowUserToResizeRows = false;
            g.RowHeadersVisible = false;

            if (g.Columns["Id"] != null)
            {
                g.Columns["Id"].FillWeight = 12;
                g.Columns["Id"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (g.Columns["Nombre"] != null) g.Columns["Nombre"].FillWeight = 55;

            if (g.Columns["Iva"] != null)
            {
                var c = g.Columns["Iva"];
                c.HeaderText = "IVA (%)";
                c.FillWeight = 16;
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                c.DefaultCellStyle.Format = "N0";
            }
            if (g.Columns["UtilidadPct"] != null)
            {
                var c = g.Columns["UtilidadPct"];
                c.HeaderText = "Utilidad (%)";
                c.FillWeight = 17;
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                c.DefaultCellStyle.Format = "N0";
            }
        }

        private void Limpiar()
        {
            _seleccion = null;
            txtNombre.Text = "";
            numIva.Value = 0;
            numUtilidad.Value = 0;
            grid.ClearSelection();
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow != null && grid.CurrentRow.DataBoundItem is Categoria c)
            {
                _seleccion = c;
                txtNombre.Text = c.Nombre;
                numIva.Value = c.Iva;
                numUtilidad.Value = c.UtilidadPct;
            }
        }

        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("Nombre es obligatorio"); return; }

            var c = new Categoria
            {
                Nombre = txtNombre.Text.Trim(),
                Iva = numIva.Value,
                UtilidadPct = numUtilidad.Value
            };

            if (_seleccion == null)
            {
                if (!Acl.Can(Feature.CategoriasCreate)) { MessageBox.Show("Sin permiso para crear."); return; }
                CategoriaDAO.Insertar(c);
            }
            else
            {
                if (!Acl.Can(Feature.CategoriasUpdate)) { MessageBox.Show("Sin permiso para actualizar."); return; }
                c.Id = _seleccion.Id;
                CategoriaDAO.Actualizar(c);
            }

            Cargar();
        }

        private void Eliminar()
        {
            if (!Acl.Can(Feature.CategoriasDelete)) { MessageBox.Show("Sin permiso para eliminar."); return; }
            if (_seleccion == null) { MessageBox.Show("Selecciona un registro"); return; }

            if (MessageBox.Show("¿Eliminar la categoría seleccionada?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            CategoriaDAO.Eliminar(_seleccion.Id);
            Cargar();
        }
    }
}
