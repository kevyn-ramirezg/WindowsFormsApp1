using Oracle.ManagedDataAccess.Client;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Forms;
using WindowsFormsApp1.Security;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // Panel de bienvenida
        // Bienvenida
        private Panel pWelcome;
        private TableLayoutPanel tOuter, tContent;
        private Label lblTitulo, lblSubtitulo, lblTips;
        private PictureBox picLogo;


        public Form1()
        {
            InitializeComponent();
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;

            // handlers generales
            this.MdiChildActivate += (_, __) => UpdateWelcomePanelVisibility();
            this.Resize += (_, __) => UpdateWelcomeBounds();

            WireMenuHandlers();
            this.Load += Form1_Load;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";
            Text = $"Principal - {Session.Username} (Nivel {Session.Nivel})";

            ApplyRolePermissions(Session.Nivel);

            // ⬅️ Construye la bienvenida aquí, UNA sola vez
            BuildWelcomePanel();
            UpdateWelcomePanelVisibility();
        }

        // --- arriba de BuildWelcomePanel() o al inicio del Form1 ---
        const int CONTENT_WIDTH = 1100;

        private void BuildWelcomePanel()
        {
            // 1) MDI client (zona gris)
            var mdi = this.Controls.OfType<MdiClient>().FirstOrDefault();
            if (mdi == null) return;


            // 0) Color común de fondo para todo el welcome
            var bg = Color.DarkSeaGreen; // el azul claro que ya usas

            // 2) Panel que cubre la zona gris (lo ocultamos cuando haya formularios hijos)
            pWelcome = new Panel
            {
                BackColor = bg,
                Visible = true
            };

            this.Controls.Add(pWelcome);     // ✅ agregamos al formulario padre
            pWelcome.BringToFront();

            mdi.Resize += (_, __) => UpdateWelcomeBounds();
            mdi.LocationChanged += (_, __) => UpdateWelcomeBounds();

            // 3) Outer TLP: 3x3 con celda central autosize ⇒ centra vertical y horizontal
            tOuter = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 3,
                BackColor = Color.Transparent
            };
            tOuter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tOuter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tOuter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tOuter.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            tOuter.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tOuter.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            pWelcome.Controls.Add(tOuter);

            // 4) Content TLP (apila título, subtítulo, texto y la imagen)
            tContent = new TableLayoutPanel
{
    AutoSize = true,
    BackColor = Color.Transparent,
    Anchor = AnchorStyles.None   // <-- para que el TLP completo quede centrado
};


            tContent.Anchor = AnchorStyles.None;

            tContent.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tContent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tOuter.Controls.Add(tContent, 1, 1); // celda central

            // 5) Título
            lblTitulo = new Label
            {
                AutoSize = true,
                Font = new Font("Elephant", 35, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 6),
                MinimumSize = new Size(CONTENT_WIDTH, 0),   // ⬅️ mismo ancho
                MaximumSize = new Size(CONTENT_WIDTH, 0),
                Text = $"Bienvenido {WindowsFormsApp1.Security.Session.Username}"
            };
            tContent.Controls.Add(lblTitulo);

            // 6) Subtítulo
            lblSubtitulo = new Label
            {
                AutoSize = true,
                Font = new Font("Elephant", 20, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 16),
                MinimumSize = new Size(CONTENT_WIDTH, 0),   // ⬅️ mismo ancho
                MaximumSize = new Size(CONTENT_WIDTH, 0),
                Text = "Sistema de Gestión de Productos ORLO"
            };
            tContent.Controls.Add(lblSubtitulo);

            // 7) Texto
            lblTips = new Label
            {
                AutoSize = true,
                Font = new Font("Elephant", 15, FontStyle.Regular),
                ForeColor = Color.Green,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 16),
                MinimumSize = new Size(CONTENT_WIDTH, 0),   // ⬅️ mismo ancho
                MaximumSize = new Size(CONTENT_WIDTH, 0),
                Text =
                    "¿Qué deseas hacer hoy?\n" +
                    "• Gestionar Entidades: Clientes, Categorías, Productos, Usuarios.\n" +
                    "• Transacciones: Ventas y Créditos, IVA Trimestre (según tu nivel de acceso).\n" +
                    "• Reportes: Factura, Ventas Mensuales, Morosos, Top Productos, Existencias Bajas.\n\n" +
                    $"Fecha: {DateTime.Now:dd/MM/yyyy}"
            };
            tContent.Controls.Add(lblTips);


            // 8) Imagen debajo del texto, enmarcada en un panel del mismo color que el fondo
            picLogo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill
            };
            // Si la tienes en resources:
            picLogo.Image = Properties.Resources.OrloWelcome;
            // Si por archivo: picLogo.Image = Image.FromFile(@"Assets\orlo_welcome.png");

            // Tamaño GRANDE y centrado (720x480 es buena proporción para tu imagen 1152x768)
            int imgW = 720;
            int imgH = 480;

            // Panel marco que “contiene” la imagen (mismo color que el fondo)
            var imgFrame = new Panel
            {
                BackColor = bg,
                AutoSize = false,
                Width = imgW,
                Height = imgH,
                Margin = new Padding(0, 20, 0, 0),   // espacio respecto al texto
                Padding = new Padding(10),
                Anchor = AnchorStyles.None           // MUY IMPORTANTE para centrar en el TLP
            };

            imgFrame.Controls.Add(picLogo);
            tContent.Controls.Add(imgFrame);
            UpdateWelcomePanel(); // estado inicial (visible si no hay hijos)
        }

        private void UpdateWelcomePanel()
        {
            if (pWelcome == null) return;

            // visible sólo si NO hay formularios MDI abiertos
            pWelcome.Visible = this.MdiChildren.Length == 0;

            if (pWelcome.Visible)
                pWelcome.BringToFront();
        }

        private void UpdateWelcomeBounds()
        {
            if (pWelcome == null || pWelcome.IsDisposed) return;
            var mdi = this.Controls.OfType<MdiClient>().FirstOrDefault();
            if (mdi == null) return;

            pWelcome.Bounds = mdi.Bounds;
            pWelcome.BringToFront();
        }
        private void UpdateWelcomePanelVisibility()
        {
            if (pWelcome == null || pWelcome.IsDisposed) return;

            // ¿Hay algún hijo MDI visible?
            bool hasMdiChildren = this.MdiChildren.Any(f => f.Visible);
            pWelcome.Visible = !hasMdiChildren;

            if (!hasMdiChildren)
            {
                UpdateWelcomeBounds();
                pWelcome.BringToFront();
            }
        }



        // Abre un hijo MDI en modo “una sola ventana”:
        // - Si ya hay uno del mismo tipo, lo activa.
        // - Si hay otros distintos, los cierra y abre el nuevo.

        private void OpenChild<T>() where T : Form, new()
        {
            // Si ya hay uno del mismo tipo, lo traemos al frente
            var same = this.MdiChildren.FirstOrDefault(f => f is T);
            if (same != null)
            {
                same.WindowState = FormWindowState.Maximized;
                same.BringToFront();
                same.Activate();
                UpdateWelcomePanelVisibility();
                return;
            }

            // Cierra cualquier otro hijo MDI
            foreach (var child in this.MdiChildren)
                child.Close();

            // Abre el nuevo
            var frm = new T
            {
                MdiParent = this,
                StartPosition = FormStartPosition.CenterScreen,
                WindowState = FormWindowState.Maximized
            };

            frm.FormClosed += (_, __) => UpdateWelcomePanelVisibility();
            frm.Show();

            UpdateWelcomePanelVisibility();
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

            usuariosToolStripMenuItem.Click += (_, __) =>
            {
                if (Guard(Session.Nivel < 3, "Sólo el Administrador puede gestionar usuarios.")) return;
                OpenChild<FormUsuarios>();
            };

            // TRANSACCIONES
            ventasToolStripMenuItem.Click += (_, __) =>
            {
                if (Guard(Session.Nivel < 2, "No tienes permisos para Transacciones.")) return;
                OpenChild<FormVenta>();
            };
            creditosToolStripMenuItem.Click += (_, __) =>
            {
                if (Guard(Session.Nivel < 2, "No tienes permisos para Transacciones.")) return;
                OpenChild<FormCreditos>();
            };

            ivaTrimestreToolStripMenuItem.Click += (_, __) =>
            {
                if (Guard(!Acl.Can(Feature.ReporteIvaTrimestre),
                          "Sólo Administrador o Paramétrico pueden ver IVA Trimestre.")) return;
                OpenChild<FormRptIvaTrimestre>();
            };




            // REPORTES (consultas): siempre que canReports sea true ya estarán habilitados
            facturaToolStripMenuItem.Click += (_, __) => OpenChild<FormRptFactura>();
            ventasPorRangoToolStripMenuItem.Click += (_, __) => OpenChild<FormReporteVentas>();
            morososToolStripMenuItem.Click += (_, __) => OpenChild<FormMorosos>();
            topProductosToolStripMenuItem.Click += (_, __) => OpenChild<FormTopProductos>();
            existenciasBajasToolStripMenuItem.Click += (_, __) => OpenChild<FormExistenciasBajas>();

            // UTILIDADES
            calculadoraToolStripMenuItem.Click += (_, __) => LaunchCalc();
            calendarioToolStripMenuItem.Click += (_, __) => OpenChild<FormCalendario>();
            exportarCsvToolStripMenuItem.Click += (_, __) => OpenChild<FormExportadorCsv>();
            bitacoraToolStripMenuItem.Click += (_, __) =>
            {
                if (Guard(Session.Nivel < 3, "Sólo el Administrador puede ver la bitácora.")) return;
                OpenChild<FormBitacora>();
            };

            // AYUDA
            acercaDeToolStripMenuItem.Click += (_, __) => OpenChild<FormAcercaDe>();
            salirToolStripMenuItem.Click += (_, __) => Application.Restart();
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

        private void RefreshWelcomeText()
        {
            if (lblTitulo != null)
                lblTitulo.Text = $"Bienvenido {WindowsFormsApp1.Security.Session.Username} 👋";
            if (lblTips != null)
                lblTips.Text =
                    "¿Qué deseas hacer hoy?\n" +
                    "• Gestionar Entidades: Clientes, Categorías, Productos, Usuarios.\n" +
                    "• Transacciones: Ventas y Créditos, IVA Trimestre. (según tu nivel de acceso).\n" +
                    "• Reportes: Factura, Ventas Mensuales, Morosos, Top Productos, Existencias Bajas.\n\n" +
                    $"Fecha: {DateTime.Now:dd/MM/yyyy}";
        }


        // ======= AGREGA ESTOS MÉTODOS DE APOYO =======

        // Mensaje estándar cuando no tiene permiso
        private bool Guard(bool condition, string msg)
        {
            if (condition)
            {
                MessageBox.Show(msg, "Acceso restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            return false;
        }

        // Aplica permisos por nivel: 3 Admin, 2 Paramétrico, 1 Esporádico
        private void ApplyRolePermissions(int _)
        {
            // Menú
            clientesToolStripMenuItem.Enabled = Acl.CanOpen(Feature.Clientes);
            categoriasToolStripMenuItem.Enabled = Acl.CanOpen(Feature.Categorias);
            productosToolStripMenuItem.Enabled = Acl.CanOpen(Feature.Productos);
            usuariosToolStripMenuItem.Enabled = Acl.CanOpen(Feature.Usuarios);
            ventasToolStripMenuItem.Enabled = Acl.CanOpen(Feature.Ventas);
            creditosToolStripMenuItem.Enabled = Acl.CanOpen(Feature.Creditos);
            ivaTrimestreToolStripMenuItem.Enabled = Acl.Can(Feature.ReporteIvaTrimestre);


            facturaToolStripMenuItem.Enabled = Acl.Can(Feature.ReporteFactura);
            ventasPorRangoToolStripMenuItem.Enabled = Acl.Can(Feature.ReporteVentasMes);
            morososToolStripMenuItem.Enabled = Acl.Can(Feature.ReporteMorosos);
            topProductosToolStripMenuItem.Enabled = Acl.Can(Feature.ReporteTopProductos);
            existenciasBajasToolStripMenuItem.Enabled = Acl.Can(Feature.ReporteExistenciasBajas);

            calculadoraToolStripMenuItem.Enabled = Acl.Can(Feature.Util_Calculadora);
            calendarioToolStripMenuItem.Enabled = Acl.Can(Feature.Util_Calendario);
            exportarCsvToolStripMenuItem.Enabled = Acl.Can(Feature.Util_ExportarCsv);
            bitacoraToolStripMenuItem.Enabled = Acl.CanOpen(Feature.Bitacora);

            // Botones laterales (si existen)
            btnClientes.Enabled = Acl.CanOpen(Feature.Clientes);
            btnCategorias.Enabled = Acl.CanOpen(Feature.Categorias);
            btnProductos.Enabled = Acl.CanOpen(Feature.Productos);
            btnUsuarios.Enabled = Acl.CanOpen(Feature.Usuarios);
            btnVentas.Enabled = Acl.CanOpen(Feature.Ventas);
            btnCreditos.Enabled = Acl.CanOpen(Feature.Creditos);
            btnFactura.Enabled = Acl.Can(Feature.ReporteFactura);
            btnVentasMes.Enabled = Acl.Can(Feature.ReporteVentasMes);
            btnIvaTrimestre.Enabled = Acl.Can(Feature.ReporteIvaTrimestre);
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
            if (Guard(!Acl.Can(Feature.ReporteIvaTrimestre),
                      "Sólo Administrador o Paramétrico pueden ver IVA Trimestre.")) return;

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

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
                using (var f = new WindowsFormsApp1.Forms.FormUsuarios())
                {
                    f.ShowDialog(this);
            }
        }

        private void btnMorosos_Click(object sender, EventArgs e)
        {
            using (var f = new WindowsFormsApp1.Forms.FormMorosos())
            {
                f.ShowDialog(this);
            }
        }

        private void btnTopProductos_Click(object sender, EventArgs e)
        {
            using (var f = new WindowsFormsApp1.Forms.FormTopProductos())
            {
                f.ShowDialog(this);
            }
        }

        private void btnExistenciasBajas_Click(object sender, EventArgs e)
        {
            using (var f = new WindowsFormsApp1.Forms.FormExistenciasBajas())
            {
                f.ShowDialog(this);
            }   
        }
    }
}
