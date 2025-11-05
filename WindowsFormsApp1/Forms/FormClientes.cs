using System;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Forms
{
    public partial class FormClientes : Form
    {
        private Cliente seleccion; // puede quedar null

        public FormClientes()
        {
            InitializeComponent();
        }

        private void FormClientes_Load(object sender, EventArgs e)
        {
            grid.AutoGenerateColumns = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;

            Cargar();              // tu método que llena el grid
            SeleccionarPrimera();  // para que siempre haya una fila seleccionada
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
            g.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            g.AllowUserToResizeRows = false;
            g.RowHeadersVisible = false; // oculta el borde/triángulo de fila
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;

            if (g.Columns["Id"] != null)
            {
                g.Columns["Id"].FillWeight = 10;
                g.Columns["Id"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (g.Columns["Nombre"] != null)
            {
                g.Columns["Nombre"].FillWeight = 35;
            }
            if (g.Columns["Telefono"] != null || g.Columns["Teléfono"] != null)
            {
                var col = g.Columns["Telefono"] ?? g.Columns["Teléfono"];
                col.FillWeight = 20;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (g.Columns["Correo"] != null)
            {
                g.Columns["Correo"].FillWeight = 35;
            }
        }


        private void Limpiar()
        {
            seleccion = null;
            txtNombre.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            grid.ClearSelection();
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            seleccion = grid.CurrentRow?.DataBoundItem as Cliente;
            // Rellena los TextBox si quieres:
            if (seleccion != null)
            {
                txtNombre.Text = seleccion.Nombre;
                txtTelefono.Text = seleccion.Telefono;
                txtCorreo.Text = seleccion.Correo;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e) => Limpiar();

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Nombre es obligatorio"); return;
            }

            var c = new Cliente();
            c.Nombre = txtNombre.Text.Trim();
            c.Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? "" : txtTelefono.Text.Trim();
            c.Correo = string.IsNullOrWhiteSpace(txtCorreo.Text) ? "" : txtCorreo.Text.Trim();

            if (seleccion == null)  // insertar
                ClienteDAO.Insertar(c);
            else                    // actualizar
            {
                c.Id = seleccion.Id;
                ClienteDAO.Actualizar(c);
            }

            Cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // 1) Obtén el objeto desde la selección actual del grid
            var cli = (seleccion ??
                       grid.CurrentRow?.DataBoundItem as Cliente ??
                       (grid.SelectedRows.Count > 0 ? grid.SelectedRows[0].DataBoundItem as Cliente : null));

            if (cli == null)
            {
                MessageBox.Show("Selecciona un registro");
                return;
            }

            // 2) Confirma
            var ok = MessageBox.Show($"¿Eliminar a {cli.Nombre} (ID {cli.Id})?",
                                     "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ok != DialogResult.Yes) return;

            // 3) Borra en BD
            ClienteDAO.Eliminar(cli.Id);

            // 4) Recarga y deja una fila seleccionada
            Cargar();
            SeleccionarPrimera();
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

    }
}
