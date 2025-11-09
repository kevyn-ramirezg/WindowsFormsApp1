using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Forms;
using WindowsFormsApp1.Security;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            

            // Opcional: aplica permisos por rol apenas abra
            //AplicarPermisosPorRol();
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

        private void Form1_Load(object sender, EventArgs e)
        {
            // Si quieres mostrar quién inició sesión, descomenta si tienes un Label:
            //lblUsuario.Text = $"Usuario: {Session.Username} (Nivel {Session.Nivel})";

            switch (Session.Nivel)
            {
                case 1: // Admin: todo habilitado
                    btnCategorias.Enabled = true;
                    btnProductos.Enabled = true;
                    btnClientes.Enabled = true;
                    btnVentas.Enabled = true;
                    btnProbar.Enabled = true;
                    break;

                case 2: // Paramétrico: sin usuarios/bitácora (en tu menú no hay, así que deja lo común)
                    btnCategorias.Enabled = true;
                    btnProductos.Enabled = true;
                    btnClientes.Enabled = true;
                    btnVentas.Enabled = true;
                    btnProbar.Enabled = true;
                    break;

                case 3: // Esporádico: solo “consultas” (en tu UI, podemos dejar solo Probar o lo que definas)
                    btnCategorias.Enabled = false;
                    btnProductos.Enabled = false;
                    btnClientes.Enabled = false;
                    btnVentas.Enabled = false;
                    btnProbar.Enabled = true;   // o false si no lo necesitas
                    break;
            }
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                using (var cn = Db.Open())
                using (var cmd = new OracleCommand(
                    "INSERT INTO BITACORA (USUARIO_ID, EVENTO) VALUES (:id,'LOGOUT')", cn))
                {
                    cmd.BindByName = true;
                    cmd.Parameters.Add(":id", Session.UserId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { /* opcional: log */ }

            Application.Restart();
        }

        private void btnFactura_Click(object sender, EventArgs e)
        {
            using (var f = new WindowsFormsApp1.Forms.FormRptFactura())
                f.ShowDialog(this);
        }

        private void btnVentasMes_Click(object sender, EventArgs e)
        {
            using (var f = new WindowsFormsApp1.Forms.FormReporteVentas())
                f.ShowDialog(this);
        }


        private void btnIvaTrimestre_Click(object sender, EventArgs e)
        {
            using (var f = new WindowsFormsApp1.Forms.FormRptIvaTrimestre())
                f.ShowDialog(this);
        }

        private void btnCreditos_Click(object sender, EventArgs e)
        {
            using (var f = new WindowsFormsApp1.Forms.FormCreditos())
                f.ShowDialog(this);
        }
    }
}
