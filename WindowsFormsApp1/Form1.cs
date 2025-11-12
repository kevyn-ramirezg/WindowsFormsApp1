using Oracle.ManagedDataAccess.Client;
using System;
using System.Linq;
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
            WireMenuHandlers();
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;
            this.Load += Form1_Load;
        }

        // Abre un hijo MDI y evita duplicados
        private void OpenChild<T>() where T : Form, new()
        {
            var open = this.MdiChildren.FirstOrDefault(f => f is T);
            if (open != null) { open.BringToFront(); open.WindowState = FormWindowState.Normal; return; }

            var frm = new T { MdiParent = this, StartPosition = FormStartPosition.CenterScreen, WindowState = FormWindowState.Maximized };
            frm.Show();
        }

        private void LaunchCalc()
        {
            try { System.Diagnostics.Process.Start("calc.exe"); }
            catch { MessageBox.Show("No se pudo abrir la calculadora."); }
        }

        private void WireMenuHandlers()
        {
            // ENTIDADES
            clientesToolStripMenuItem.Click += (_, __) => OpenChild<FormClientes>();
            categoriasToolStripMenuItem.Click += (_, __) => OpenChild<FormCategorias>();
            productosToolStripMenuItem.Click += (_, __) => OpenChild<FormProductos>();
            usuariosToolStripMenuItem.Click += (_, __) => OpenChild<FormUsuarios>();

            // TRANSACCIONES
            ventasToolStripMenuItem.Click += (_, __) => OpenChild<FormVenta>();
            creditosToolStripMenuItem.Click += (_, __) => OpenChild<FormCreditos>();

            // REPORTES
            facturaToolStripMenuItem.Click += (_, __) => OpenChild<FormRptFactura>();
            ventasPorRangoToolStripMenuItem.Click += (_, __) => OpenChild<FormReporteVentas>();
            morososToolStripMenuItem.Click += (_, __) => OpenChild<FormMorosos>();
            topProductosToolStripMenuItem.Click += (_, __) => OpenChild<FormTopProductos>();
            existenciasBajasToolStripMenuItem.Click += (_, __) => OpenChild<FormExistenciasBajas>();

            // UTILIDADES
            calculadoraToolStripMenuItem.Click += (_, __) => LaunchCalc();
            calendarioToolStripMenuItem.Click += (_, __) => OpenChild<FormCalendario>();
            exportarCsvToolStripMenuItem.Click += (_, __) => OpenChild<FormExportadorCsv>();
            bitacoraToolStripMenuItem.Click += (_, __) => OpenChild<FormBitacora>();

            // AYUDA
            acercaDeToolStripMenuItem.Click += (_, __) => OpenChild<FormAcercaDe>();
            salirToolStripMenuItem.Click += (_, __) => Close();
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
            {
                f.ShowDialog(this);
            }
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            using (var f = new FormClientes())
            {
                f.ShowDialog(this);
            }
        }

        private void btnProbar_Click(object sender, EventArgs e)
        {
            try
            {
                using (var cn = Db.Open())
                {
                    using (var cmd = new OracleCommand("SELECT USER FROM dual", cn))
                    {
                        var who = (string)cmd.ExecuteScalar();
                        MessageBox.Show("Conectado como: " + who);
                    }
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
            Text = string.Format("Principal - {0} (Nivel {1})", Session.Username, Session.Nivel);

            if (Session.Nivel == 1) // Admin
            {
                Habilitar(true, true, true, true, true, true, true, true, true);
            }
            else if (Session.Nivel == 2) // Paramétrico
            {
                Habilitar(true, true, true, true, true, true, true, true, true);
            }
            else // Esporádico
            {
                // Deja sólo lo que quieras habilitar para consulta
                Habilitar(false, false, false, false, true, false, false, false, true);
            }
        }

        private void Habilitar(bool categorias, bool productos, bool clientes, bool ventas,
            bool probar, bool creditos, bool factura, bool ivaTrimestre, bool ventasMensuales)
        {
            // Si alguno de estos botones NO existe en tu diseñador,
            // comenta su línea correspondiente.
            btnCategorias.Enabled = categorias;
            btnProductos.Enabled = productos;
            btnClientes.Enabled = clientes;
            btnVentas.Enabled = ventas;
            btnProbar.Enabled = probar;
            if (btnCreditos != null) btnCreditos.Enabled = creditos;
            if (btnFactura != null) btnFactura.Enabled = factura;
            if (btnIvaTrimestre != null) btnIvaTrimestre.Enabled = ivaTrimestre;
            if (btnVentasMes != null) btnVentasMes.Enabled = ventasMensuales;
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                using (var cn = Db.Open())
                {
                    using (var cmd = new OracleCommand(
                        "INSERT INTO BITACORA (USUARIO_ID, EVENTO) VALUES (:id,'LOGOUT')", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":id", Session.UserId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { /* opcional log */ }

            Session.Clear();
            Application.Restart();
        }

        private void btnFactura_Click(object sender, EventArgs e)
        {
            using (var f = new WindowsFormsApp1.Forms.FormRptFactura())
            {
                f.ShowDialog(this);
            }
        }

        private void btnVentasMes_Click(object sender, EventArgs e)
        {
            using (var f = new WindowsFormsApp1.Forms.FormReporteVentas())
            {
                f.ShowDialog(this);
            }
        }

        private void btnIvaTrimestre_Click(object sender, EventArgs e)
        {
            using (var f = new WindowsFormsApp1.Forms.FormRptIvaTrimestre())
            {
                f.ShowDialog(this);
            }
        }

        private void btnCreditos_Click(object sender, EventArgs e)
        {
            using (var f = new WindowsFormsApp1.Forms.FormCreditos())
            {
                f.ShowDialog(this);
            }
        }
    }
}
