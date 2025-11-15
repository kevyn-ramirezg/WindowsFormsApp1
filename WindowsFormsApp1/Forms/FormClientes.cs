using System;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Security;

namespace WindowsFormsApp1.Forms
{
    public partial class FormClientes : Form
    {
        private Cliente _seleccion; // puede quedar null

        public FormClientes()
        {
            InitializeComponent();
            // NO enganches eventos Load aquí; el Designer ya llama a FormClientes_Load
            grid.SelectionChanged += grid_SelectionChanged; // este sí no lo engancha el Designer
        }

        // ====== LO QUE PIDE EL DESIGNER ======
        private void FormClientes_Load(object sender, EventArgs e)
        {
            try { Acl.Require(Feature.Clientes); }
            catch (UnauthorizedAccessException ex) { MessageBox.Show(ex.Message); Close(); return; }

            ConfigurarGrid();
            ApplyLocalPermissions();
            Cargar();
            SeleccionarPrimera();
        }
        private void btnNuevo_Click(object sender, EventArgs e) => Limpiar();
        private void btnGuardar_Click(object sender, EventArgs e) => Guardar();
        private void btnEliminar_Click(object sender, EventArgs e) => Eliminar();
        // =====================================

        private void ConfigurarGrid()
        {
            grid.AutoGenerateColumns = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
        }

        private void ApplyLocalPermissions()
        {
            bool canCreate = Acl.Can(Feature.ClientesCreate);
            bool canUpdate = Acl.Can(Feature.ClientesUpdate);
            bool canDelete = Acl.Can(Feature.ClientesDelete);
            bool canEdit = canCreate || canUpdate;

            btnNuevo.Enabled = canCreate;
            btnGuardar.Enabled = canEdit;
            btnEliminar.Enabled = canDelete;

            bool ro = !canEdit;
            txtNombre.ReadOnly = ro;
            txtTelefono.ReadOnly = ro;
            txtCorreo.ReadOnly = ro;
        }

        private void Cargar()
        {
            grid.DataSource = ClienteDAO.Listar();

            if (grid.Columns["Id"] != null) grid.Columns["Id"].HeaderText = "ID";
            if (grid.Columns["Nombre"] != null) grid.Columns["Nombre"].HeaderText = "Nombre";
            if (grid.Columns["Telefono"] != null) grid.Columns["Telefono"].HeaderText = "Teléfono";
            if (grid.Columns["Correo"] != null) grid.Columns["Correo"].HeaderText = "Correo";

            AjustarGridClientes();
            Limpiar();
        }

        private void AjustarGridClientes()
        {
            var g = grid;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.AllowUserToResizeRows = false;
            g.RowHeadersVisible = false;

            if (g.Columns["Id"] != null)
            {
                g.Columns["Id"].FillWeight = 10;
                g.Columns["Id"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (g.Columns["Nombre"] != null) g.Columns["Nombre"].FillWeight = 35;
            if (g.Columns["Telefono"] != null)
            {
                var c = g.Columns["Telefono"];
                c.FillWeight = 20;
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (g.Columns["Correo"] != null) g.Columns["Correo"].FillWeight = 35;
        }

        private void Limpiar()
        {
            _seleccion = null;
            txtNombre.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            grid.ClearSelection();
        }

        private void SeleccionarPrimera()
        {
            if (grid.Rows.Count > 0)
            {
                grid.ClearSelection();
                grid.Rows[0].Selected = true;
                grid.CurrentCell = grid.Rows[0].Cells[0];
            }
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            _seleccion = grid.CurrentRow?.DataBoundItem as Cliente;
            if (_seleccion != null)
            {
                txtNombre.Text = _seleccion.Nombre;
                txtTelefono.Text = _seleccion.Telefono;
                txtCorreo.Text = _seleccion.Correo;
            }
        }

        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MessageBox.Show("Nombre es obligatorio"); return; }

            var c = new Cliente
            {
                Nombre = txtNombre.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? "" : txtTelefono.Text.Trim(),
                Correo = string.IsNullOrWhiteSpace(txtCorreo.Text) ? "" : txtCorreo.Text.Trim()
            };

            if (_seleccion == null)
            {
                if (!Acl.Can(Feature.ClientesCreate)) { MessageBox.Show("Sin permiso para crear."); return; }
                ClienteDAO.Insertar(c);
            }
            else
            {
                if (!Acl.Can(Feature.ClientesUpdate)) { MessageBox.Show("Sin permiso para actualizar."); return; }
                c.Id = _seleccion.Id;
                ClienteDAO.Actualizar(c);
            }

            Cargar();
            SeleccionarPrimera();
        }

        private void Eliminar()
        {
            if (!Acl.Can(Feature.ClientesDelete)) { MessageBox.Show("Sin permiso para eliminar."); return; }

            var cli = _seleccion ??
                      grid.CurrentRow?.DataBoundItem as Cliente ??
                      (grid.SelectedRows.Count > 0 ? grid.SelectedRows[0].DataBoundItem as Cliente : null);

            if (cli == null) { MessageBox.Show("Selecciona un registro"); return; }

            if (MessageBox.Show($"¿Eliminar a {cli.Nombre} (ID {cli.Id})?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            ClienteDAO.Eliminar(cli.Id);
            Cargar();
            SeleccionarPrimera();
        }
    }
}
