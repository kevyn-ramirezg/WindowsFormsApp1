using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Utils;

namespace WindowsFormsApp1.Forms
{
    public partial class FormUsuarios : Form
    {
        private bool creando = false;   // modo creación (ignora selección del grid)

        public FormUsuarios()
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";

            InitializeComponent();

            // Conexión explícita de eventos (por si el diseñador no los generó)
            this.Load += (_, __) => Listar();
            btnGuardar.Click += btnGuardar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnResetPwd.Click += btnResetPwd_Click;
            grid.SelectionChanged += grid_SelectionChanged;
            btnNuevo.Click += btnNuevo_Click;
            btnBuscar.Click += btnBuscar_Click;

            // Opcional: configuración de grid para una mejor UX
            grid.ReadOnly = true;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Rango válido para nivel (1,2,3)
            numNivel.Minimum = 1;
            numNivel.Maximum = 3;
        }

        private void Listar()
        {
            using (var cn = Db.Open())
            using (var da = new OracleDataAdapter(
                @"SELECT id, login, nombre, email, activo, nivel 
            FROM usuario 
        ORDER BY login", cn))
            {
                var dt = new DataTable();
                da.Fill(dt);
                grid.DataSource = dt;

                grid.ReadOnly = true;
                grid.MultiSelect = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                if (grid.Columns["LOGIN"] != null) grid.Columns["LOGIN"].HeaderText = "Usuario";
                if (grid.Columns["NOMBRE"] != null) grid.Columns["NOMBRE"].HeaderText = "Nombre";
                if (grid.Columns["EMAIL"] != null) grid.Columns["EMAIL"].HeaderText = "Email";
                if (grid.Columns["ACTIVO"] != null) grid.Columns["ACTIVO"].HeaderText = "Activo";
                if (grid.Columns["NIVEL"] != null) grid.Columns["NIVEL"].HeaderText = "Nivel";
            }

            // al listar, sal de modo creación
            creando = false;
            txtPassword.Clear();
        }


        private decimal? ObtenerIdSeleccionado()
        {
            if (grid.CurrentRow == null) return null;
            var cell = grid.CurrentRow.Cells["ID"]?.Value;
            if (cell == null || cell == DBNull.Value) return null;
            return Convert.ToDecimal(cell);
        }

        

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var q = (txtBuscar.Text ?? "").Trim();
            if (q.Length == 0) { Listar(); return; }

            using (var cn = Db.Open())
            using (var da = new OracleDataAdapter(
                @"SELECT id, login, nombre, email, activo, nivel
            FROM usuario
           WHERE UPPER(login)  LIKE '%'||UPPER(:q)||'%'
              OR UPPER(nombre) LIKE '%'||UPPER(:q)||'%'
              OR UPPER(email)  LIKE '%'||UPPER(:q)||'%'
        ORDER BY login", cn))
            {
                da.SelectCommand.BindByName = true;
                da.SelectCommand.Parameters.Add(":q", q);
                var dt = new DataTable();
                da.Fill(dt);
                grid.DataSource = dt;
            }
        }


        private void btnNuevo_Click(object sender, EventArgs e)
        {
            EntrarEnModoCreacion();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var login = (txtUsername.Text ?? "").Trim();
            if (login.Length == 0) { MessageBox.Show("Ingresa el usuario (login)."); return; }

            var nombre = string.IsNullOrWhiteSpace(txtNombre.Text) ? null : txtNombre.Text.Trim();
            var email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
            var nivel = (int)numNivel.Value;
            var activo = chkActivo.Checked ? 1 : 0;

            var id = ObtenerIdParaGuardar();

            using (var cn = Db.Open())
            {
                if (id == null)
                {
                    // INSERT
                    using (var cmd = new OracleCommand(
                        @"INSERT INTO usuario (login, nombre, email, clave_hash, nivel, activo)
                  VALUES (:lg, :nm, :em, :ph, :nv, :ac)", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":lg", login);
                        cmd.Parameters.Add(":nm", nombre == null ? (object)DBNull.Value : nombre);
                        cmd.Parameters.Add(":em", email == null ? (object)DBNull.Value : email);
                        cmd.Parameters.Add(":ph", string.IsNullOrWhiteSpace(txtPassword.Text)
                                                ? (object)DBNull.Value
                                                : HashHelper.Sha256(txtPassword.Text));
                        cmd.Parameters.Add(":nv", nivel);
                        cmd.Parameters.Add(":ac", activo);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Usuario creado.");
                        }
                        catch (OracleException ox) when (ox.Number == 1) // unique constraint
                        {
                            MessageBox.Show("Ese login o email ya existe. Elige otro.");
                            return;
                        }
                    }
                }
                else
                {
                    // UPDATE (sin tocar hash)
                    using (var cmd = new OracleCommand(
                        @"UPDATE usuario 
                     SET login  = :lg,
                         nombre = :nm,
                         email  = :em,
                         nivel  = :nv,
                         activo = :ac
                   WHERE id = :id", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":lg", login);
                        cmd.Parameters.Add(":nm", nombre == null ? (object)DBNull.Value : nombre);
                        cmd.Parameters.Add(":em", email == null ? (object)DBNull.Value : email);
                        cmd.Parameters.Add(":nv", nivel);
                        cmd.Parameters.Add(":ac", activo);
                        cmd.Parameters.Add(":id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // Actualiza la clave solo si escribiste algo
                    if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        using (var cmd = new OracleCommand(
                            "UPDATE usuario SET clave_hash = :ph WHERE id = :id", cn))
                        {
                            cmd.BindByName = true;
                            cmd.Parameters.Add(":ph", HashHelper.Sha256(txtPassword.Text));
                            cmd.Parameters.Add(":id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Usuario actualizado.");
                }
            }

            // Salir de modo creación y refrescar
            creando = false;
            txtPassword.Clear();
            Listar();
        }



        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var id = ObtenerIdSeleccionado();
            if (id == null) { MessageBox.Show("Selecciona un usuario."); return; }

            if (MessageBox.Show("¿Eliminar el usuario seleccionado?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            using (var cn = Db.Open())
            {
                try
                {
                    using (var cmd = new OracleCommand("DELETE FROM usuario WHERE id = :id", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":id", id);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Usuario eliminado.");
                }
                catch (OracleException ox) when (ox.Number == 2292) // ORA-02292
                {
                    // Borrado lógico si está referenciado
                    using (var cmd = new OracleCommand(
                        "UPDATE usuario SET activo = 0 WHERE id = :id", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":id", id);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("El usuario tiene registros asociados. Se desactivó (ACTIVO=0) en lugar de eliminarse.");
                }
            }

            Listar();
        }


        private void btnResetPwd_Click(object sender, EventArgs e)
        {
            var id = ObtenerIdSeleccionado();
            if (id == null) { MessageBox.Show("Selecciona un usuario."); return; }

            var nueva = (txtPassword.Text ?? "").Trim();
            if (nueva == "") { MessageBox.Show("Escribe la nueva contraseña en el campo."); return; }

            using (var cn = Db.Open())
            using (var cmd = new OracleCommand("UPDATE usuario SET clave_hash = :h WHERE id = :id", cn))
            {
                cmd.BindByName = true;
                cmd.Parameters.Add(":h", HashHelper.Sha256(nueva));
                cmd.Parameters.Add(":id", id);
                cmd.ExecuteNonQuery();
            }

            txtPassword.Clear();
            MessageBox.Show("Contraseña actualizada.");
        }


        private void LimpiarFormulario()
        {
            txtUsername.Clear();
            txtNombre.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            numNivel.Value = 1;
            chkActivo.Checked = true; // por defecto activo
        }


        private void EntrarEnModoCreacion()
        {
            creando = true;
            grid.ClearSelection();
            grid.CurrentCell = null;
            LimpiarFormulario();
            txtUsername.Focus();
        }

        // Si estás en modo creación, finge que no hay ID seleccionado
        private decimal? ObtenerIdParaGuardar()
        {
            if (creando) return null;
            return ObtenerIdSeleccionado();
        }


        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) return;

            // Al seleccionar, salimos de modo creación
            creando = false;

            txtUsername.Text = grid.CurrentRow.Cells["LOGIN"]?.Value?.ToString() ?? "";
            txtNombre.Text = grid.CurrentRow.Cells["NOMBRE"]?.Value?.ToString() ?? "";
            txtEmail.Text = grid.CurrentRow.Cells["EMAIL"]?.Value?.ToString() ?? "";

            var vNivel = grid.CurrentRow.Cells["NIVEL"]?.Value;
            numNivel.Value = (vNivel == null || vNivel == DBNull.Value) ? 1 : Convert.ToDecimal(vNivel);

            var vActivo = grid.CurrentRow.Cells["ACTIVO"]?.Value;
            chkActivo.Checked = (vActivo != null && vActivo != DBNull.Value && Convert.ToInt32(vActivo) == 1);

            txtPassword.Clear(); // nunca mostramos hashes
        }


    }
}
