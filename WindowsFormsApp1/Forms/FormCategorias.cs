using System;
using System.Windows.Forms;
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Forms
{
    public partial class FormCategorias : Form
    {
        private Categoria seleccion; // puede quedar null

        public FormCategorias()
        {
            InitializeComponent();
        }

        private void FormCategorias_Load(object sender, EventArgs e)
        {
            // Config extra del grid
            grid.AutoGenerateColumns = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;

            Cargar();
        }

        private void Cargar()
        {
            grid.DataSource = CategoriaDAO.Listar();

            if (grid.Columns["Id"] != null) grid.Columns["Id"].HeaderText = "ID";
            if (grid.Columns["Nombre"] != null) grid.Columns["Nombre"].HeaderText = "Nombre";
            if (grid.Columns["Iva"] != null) grid.Columns["Iva"].HeaderText = "IVA (%)";
            if (grid.Columns["UtilidadPct"] != null) grid.Columns["UtilidadPct"].HeaderText = "Utilidad (%)";

            Limpiar();
        }

        private void Limpiar()
        {
            seleccion = null;
            txtNombre.Text = "";
            numIva.Value = 0;
            numUtilidad.Value = 0;
            grid.ClearSelection();
        }

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow != null && grid.CurrentRow.DataBoundItem is Categoria c)
            {
                seleccion = c;
                txtNombre.Text = c.Nombre;
                numIva.Value = c.Iva;
                numUtilidad.Value = c.UtilidadPct;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Nombre es obligatorio");
                return;
            }

            var c = new Categoria
            {
                Nombre = txtNombre.Text.Trim(),
                Iva = numIva.Value,
                UtilidadPct = numUtilidad.Value
            };

            if (seleccion == null)
            {
                CategoriaDAO.Insertar(c);
            }
            else
            {
                c.Id = seleccion.Id;
                CategoriaDAO.Actualizar(c);
            }

            Cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (seleccion == null)
            {
                MessageBox.Show("Selecciona un registro");
                return;
            }

            if (MessageBox.Show("¿Eliminar la categoría seleccionada?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CategoriaDAO.Eliminar(seleccion.Id);
                Cargar();
            }
        }

        private void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
