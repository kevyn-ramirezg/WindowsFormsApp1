using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Utils;

namespace WindowsFormsApp1.Forms
{
    public partial class FormUsuarios : Form
    {
        public FormUsuarios() { 
            InitializeComponent(); 
            Load += (_, __) => { Listar(); }; 
        }

        private void Listar()
        {
            using (var cn = Db.Open())
            using (var da = new OracleDataAdapter(
                // columnas reales del esquema
                "SELECT id, login, nivel FROM usuario ORDER BY login", cn))
            {
                var dt = new DataTable();
                da.Fill(dt);

                grid.DataSource = dt;
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                grid.ReadOnly = true;

                // encabezados bonitos
                if (grid.Columns["LOGIN"] != null) grid.Columns["LOGIN"].HeaderText = "Usuario";
                if (grid.Columns["NIVEL"] != null) grid.Columns["NIVEL"].HeaderText = "Nivel";
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // Nota: USUARIO.ID es identidad -> no uses secuencia en INSERT
            var id = ObtenerIdSeleccionado();

            using (var cn = Db.Open())
            {
                if (id == null)
                {
                    // INSERT
                    using (var cmd = new OracleCommand(
                        "INSERT INTO usuario (login, clave_hash, nivel) VALUES (:lg, :ph, :nv)", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":lg", txtUsername.Text.Trim());
                        cmd.Parameters.Add(":ph", string.IsNullOrWhiteSpace(txtPassword.Text)
                            ? (object)DBNull.Value
                            : HashHelper.Sha256(txtPassword.Text));   // usa Sha256
                        cmd.Parameters.Add(":nv", (int)numNivel.Value);
                        cmd.ExecuteNonQuery();
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
                        cmd.Parameters.Add(":lg", txtUsername.Text.Trim());
                        cmd.Parameters.Add(":nv", (int)numNivel.Value);
                        cmd.Parameters.Add(":id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // Actualizar la clave solo si escribiste algo
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
                }
            }

            txtPassword.Clear();
            Listar();
            MessageBox.Show("Usuario guardado/actualizado.");
        }


        private decimal? ObtenerIdSeleccionado()
        {
            if (grid.CurrentRow == null) return null;
            return Convert.ToDecimal(grid.CurrentRow.Cells["ID"].Value);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var id = ObtenerIdSeleccionado();
            if (id == null) return;

            if (MessageBox.Show("¿Eliminar el usuario seleccionado?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            using (var cn = Db.Open())
            using (var cmd = new OracleCommand("DELETE FROM usuario WHERE id = :id", cn))
            {
                cmd.BindByName = true;
                cmd.Parameters.Add(":id", id);
                cmd.ExecuteNonQuery();
            }

            Listar();
        }

        private void btnResetPwd_Click(object sender, EventArgs e)
        {
            var id = ObtenerIdSeleccionado();
            if (id == null) return;

            var nueva = txtPassword.Text.Trim();
            if (nueva == "")
            {
                MessageBox.Show("Ingrese nueva contraseña.");
                return;
            }

            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(
                "UPDATE usuario SET password_hash=:h WHERE id=:id", cn))
            {
                // Unificado: usa HashHelper.Hash
                cmd.Parameters.Add(":h", HashHelper.Sha256(nueva));
                cmd.Parameters.Add(":id", id);
                cmd.ExecuteNonQuery();
            }

            txtPassword.Clear();
            MessageBox.Show("Contraseña actualizada.");
        }

        // grid.SelectionChanged -> cargar datos en el panel de edición
        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null) return;
            txtUsername.Text = grid.CurrentRow.Cells["LOGIN"]?.Value?.ToString() ?? "";
            numNivel.Value = grid.CurrentRow.Cells["NIVEL"] != null
                ? Convert.ToDecimal(grid.CurrentRow.Cells["NIVEL"].Value)
                : 1;
            txtPassword.Clear(); // nunca mostramos hashes
        }
    }

}
