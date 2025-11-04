using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCategorias_Click(object sender, EventArgs e)
        {
            using (var f = new FormCategorias())
            {
                f.ShowDialog(this);
            }
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {

            using (var f = new FormProductos())
                f.ShowDialog(this);
        }

        // Clientes
        private void btnClientes_Click(object sender, EventArgs e)
        {
            using (var f = new FormClientes())
                f.ShowDialog(this);
        }

        private void btnProbar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var cn = Db.Open())
                using (var cmd = new OracleCommand("SELECT USER FROM dual", cn))
                {
                    var who = (string)cmd.ExecuteScalar();
                    MessageBox.Show("Conectado como: " + who);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
            }
        }

        private void btnVentas_Click(object sender, EventArgs e)
        {
            using (var f = new FormVenta())
            {
                f.ShowDialog(this);
            }
        }

    }
}
