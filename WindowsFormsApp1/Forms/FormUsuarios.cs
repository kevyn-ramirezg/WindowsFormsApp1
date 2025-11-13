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

            var id = ObtenerIdParaGuardar();

            using (var cn = Db.Open())
            {
                if (id == null)
                {
                    // INSERT
                    using (var cmd = new OracleCommand(
                        @"INSERT INTO usuario (login, nombre, email, clave_hash, nivel, activo)
                  VALUES (:lg, :nm, :em, :ph, :nv, 1)", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":lg", login);
                        cmd.Parameters.Add(":nm", DBNull.Value); // si tienes txtNombre úsalo aquí
                        cmd.Parameters.Add(":em", DBNull.Value); // si tienes txtEmail úsalo aquí
                        cmd.Parameters.Add(":ph", string.IsNullOrWhiteSpace(txtPassword.Text)
                                                ? (object)DBNull.Value
                                                : HashHelper.Sha256(txtPassword.Text));
                        cmd.Parameters.Add(":nv", (int)numNivel.Value);

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
                    // UPDATE
                    using (var cmd = new OracleCommand(
                        @"UPDATE usuario 
                     SET login = :lg, nivel = :nv
                   WHERE id = :id", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":lg", login);
                        cmd.Parameters.Add(":nv", (int)numNivel.Value);
                        cmd.Parameters.Add(":id", id);
                        cmd.ExecuteNonQuery();
                    }

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
            txtPassword.Clear();
            numNivel.Value = 1;
            // Si tienes txtNombre / txtEmail: límpialos también.
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
            // Al seleccionar, sales de modo creación
            creando = false;

            txtUsername.Text = grid.CurrentRow.Cells["LOGIN"]?.Value?.ToString() ?? "";
            var val = grid.CurrentRow.Cells["NIVEL"]?.Value;
            numNivel.Value = (val == null || val == DBNull.Value) ? 1 : Convert.ToDecimal(val);
            txtPassword.Clear();
        }

    }
}
