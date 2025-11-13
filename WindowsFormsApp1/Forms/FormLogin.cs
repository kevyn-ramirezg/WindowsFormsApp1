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
using WindowsFormsApp1.DAO;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Security;
using WindowsFormsApp1.Utils;

namespace WindowsFormsApp1.Forms
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            // Asegura que el click esté conectado por código:
            btnIngresar.Click += btnIngresar_Click;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            btnCrearCuenta.Click += (_, __) =>
            {
                using (var frm = new Forms.FormRegistroUsuario())
                {
                    if (frm.ShowDialog(this) == DialogResult.OK)
                    {
                        txtUsuario.Text = frm.UsernameCreado;
                        txtClave.Focus();
                    }
                }
            };
            ;

            // Enter = Ingresar, Esc = Cancelar
            this.AcceptButton = btnIngresar;
            this.CancelButton = btnCancelar;
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            var u = (txtUsuario.Text ?? "").Trim();
            var p = (txtClave.Text ?? "");

            if (u.Length == 0 || p.Length == 0)
            {
                MessageBox.Show("Ingresa usuario y clave.");
                return;
            }

            var h = HashHelper.Sha256(p); // hex minúsculas

            try
            {
                using (var cn = Db.Open())
                {
                    // (Opcional) Ver usuario de BD
                    using (var cmdWho = new OracleCommand("SELECT USER FROM dual", cn))
                    {
                        var who = (string)cmdWho.ExecuteScalar();
                        // Puedes comentar esto en producción
                        // MessageBox.Show("Conectado como: " + who);
                    }

                    // 1) Buscar el usuario por LOGIN y hash (CLAVE_HASH)
                    using (var cmd = new OracleCommand(@"
            SELECT id, nombre, nivel, activo
              FROM USUARIO
             WHERE UPPER(login)=UPPER(:u)
               AND UPPER(clave_hash)=UPPER(:h)", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":u", u);
                        cmd.Parameters.Add(":h", h);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                var userId = Convert.ToInt32(dr["ID"]);
                                var nombre = dr["NOMBRE"] == DBNull.Value ? u : dr["NOMBRE"].ToString();
                                var nivel = Convert.ToInt32(dr["NIVEL"]);
                                var activo = Convert.ToInt32(dr["ACTIVO"]) == 1;

                                if (!activo)
                                {
                                    MessageBox.Show("Cuenta inactiva. Contacte al administrador.");
                                    return;
                                }

                                // 2) Cargar sesión
                                Session.Set(userId, u, nivel); // ya la tienes; usa tu implementación

                                // 3) Registrar en bitácora
                                using (var bit = new OracleCommand(
                                    "INSERT INTO BITACORA (USUARIO_ID, EVENTO) VALUES (:id,'LOGIN')", cn))
                                {
                                    bit.BindByName = true;
                                    bit.Parameters.Add(":id", userId);
                                    bit.ExecuteNonQuery();
                                }

                                // 4) Cerrar login y continuar
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                                return;
                            }
                        }
                    }

                    // 5) Diagnóstico cuando falla (útil en desarrollo)
                    string hashBD = null;
                    using (var cmd1 = new OracleCommand(
                        "SELECT clave_hash FROM USUARIO WHERE UPPER(login)=UPPER(:u)", cn))
                    {
                        cmd1.BindByName = true;
                        cmd1.Parameters.Add(":u", u);
                        var v = cmd1.ExecuteScalar();
                        hashBD = v == null ? null : v.ToString();
                    }

                    MessageBox.Show(
                        "Credenciales inválidas.\n\n" +
                        $"Usuario digitado : {u}\n" +
                        $"Hash calculado   : {h}\n" +
                        $"Hash en BD       : {(hashBD ?? "(no existe login)")}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar credenciales: " + ex.Message);
            }

        }

    }
}