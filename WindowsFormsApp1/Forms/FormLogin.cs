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
using WindowsFormsApp1.Security;
using WindowsFormsApp1.Utils;

namespace WindowsFormsApp1.Forms
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            this.AcceptButton = btnIngresar;   // Enter = click en btnIngresar
            this.CancelButton = btnCancelar;   // Esc = click en btnCancelar (opcional)
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

            var h = HashHelper.Sha256(p); // hex en minúsculas

            try
            {
                using (var cn = Db.Open())
                {
                    // 1) ¿a qué usuario Oracle nos conectamos?
                    using (var cmdWho = new OracleCommand("SELECT USER FROM dual", cn))
                    {
                        var who = (string)cmdWho.ExecuteScalar();
                        if (!string.Equals(who, "APP_USR", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("Conectado como: " + who + " (debe ser APP_USR)");
                        }
                    }

                    // 2) Trae el hash guardado para el login digitado (si existe)
                    string hashBD = null;
                    using (var cmd1 = new OracleCommand(
                        "SELECT clave_hash FROM USUARIO WHERE UPPER(login)=UPPER(:u)", cn))
                    {
                        cmd1.BindByName = true;
                        cmd1.Parameters.Add(":u", u);
                        var v = cmd1.ExecuteScalar();
                        hashBD = v == null ? null : v.ToString();
                    }

                    // 3) Validación real (login case-insensitive, hash exacto)
                    using (var cmd = new OracleCommand(@"
                SELECT id, nivel
                FROM USUARIO
                WHERE UPPER(login)=UPPER(:u) AND UPPER(clave_hash)=UPPER(:h)", cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":u", u);
                        cmd.Parameters.Add(":h", h);

                        using (var dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                var userId = dr.GetInt32(0);
                                var nivel = dr.GetInt32(1);

                                Session.Set(userId, u, nivel);

                                using (var bit = new OracleCommand(
                                    "INSERT INTO BITACORA (USUARIO_ID, EVENTO) VALUES (:id,'LOGIN')", cn))
                                {
                                    bit.BindByName = true;
                                    bit.Parameters.Add(":id", userId);
                                    bit.ExecuteNonQuery();
                                }

                                this.DialogResult = DialogResult.OK;
                                this.Close();
                                return;
                            }
                        }
                    }

                    // Si no pasó, mostramos comparación para ver qué está mal
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