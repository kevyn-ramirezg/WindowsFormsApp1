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
                    // (Opcional) Aviso si no estás en APP_USR
                    using (var cmdWho = new OracleCommand("SELECT USER FROM dual", cn))
                    {
                        var who = (string)cmdWho.ExecuteScalar();
                        if (!who.Equals("APP_USR", StringComparison.OrdinalIgnoreCase))
                            MessageBox.Show("Conectado como: " + who + " (debería ser APP_USR)");
                    }

                    // ===== Validación de credenciales =====
                    using (var cmd = new OracleCommand(@"
                SELECT id, nivel
                  FROM USUARIO
                 WHERE UPPER(login)=UPPER(:u)
                   AND UPPER(clave_hash)=UPPER(:h)", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":u", OracleDbType.Varchar2, 100).Value = u;
                        cmd.Parameters.Add(":h", OracleDbType.Varchar2, 64).Value = h;

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                var userId = dr.GetInt32(0);
                                var nivel = dr.GetInt32(1);

                                // 1) Sesión viva
                                Session.Set(userId, u, nivel);

                                // 2) ⬇⬇⬇ REEMPLAZO: antes insertabas aquí en BITACORA con un OracleCommand inline
                                //    Ahora centralizamos:
                                try { BitacoraDAO.RegistrarIngreso(userId); }
                                catch { /* si falla la bitácora, no bloquea el login */ }

                                // 3) Volver al contenedor principal
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                                return;
                            }
                        }
                    }

                    // ===== Diagnóstico si no coincide =====
                    string hashBD = null;
                    using (var cmd1 = new OracleCommand(
                        "SELECT clave_hash FROM USUARIO WHERE UPPER(login)=UPPER(:u)", cn))
                    {
                        cmd1.BindByName = true;
                        cmd1.Parameters.Add(":u", OracleDbType.Varchar2, 100).Value = u;
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