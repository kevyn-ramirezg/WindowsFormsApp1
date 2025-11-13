using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Utils;

namespace WindowsFormsApp1.Forms
{
    public partial class FormRegistroUsuario : Form
    {
        public string UsernameCreado { get; private set; } = "";

        public FormRegistroUsuario()
        {
            InitializeComponent();

            cbNivel.DropDownStyle = ComboBoxStyle.DropDownList;
            cbNivel.Items.Clear();
            cbNivel.Items.Add(new NivelItem("Administrador (3)", 3));
            cbNivel.Items.Add(new NivelItem("Paramétrico (2)", 2));
            cbNivel.Items.Add(new NivelItem("Esporádico (1)", 1));
            cbNivel.SelectedIndex = 2; // por defecto Esporádico (1)

            chkActivo.Checked = true;

            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            var login = txtLogin.Text.Trim();
            var nombre = txtNombre.Text.Trim();
            var email = txtEmail.Text.Trim();
            var pass1 = txtPassword.Text;
            var pass2 = txtConfirm.Text;
            var nivel = ((NivelItem)cbNivel.SelectedItem).Valor;
            var activo = chkActivo.Checked ? 1 : 0;

            var hash = HashHelper.Sha256(pass1);   // el mismo helper que usas en login


            // Validaciones básicas
            if (login == "") { MessageBox.Show("Login es obligatorio."); return; }
            if (nombre == "") { MessageBox.Show("Nombre es obligatorio."); return; }
            if (pass1.Length < 6) { MessageBox.Show("La contraseña debe tener al menos 6 caracteres."); return; }
            if (pass1 != pass2) { MessageBox.Show("Las contraseñas no coinciden."); return; }
            if (email != "" && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { MessageBox.Show("Correo no válido."); return; }

            // Verificar unicidad de LOGIN
            using (var cn = Db.Open())
            using (var cmdChk = new OracleCommand("SELECT COUNT(*) FROM USUARIO WHERE UPPER(LOGIN)=UPPER(:l)", cn))
            {
                cmdChk.BindByName = true;
                cmdChk.Parameters.Add(":l", login);
                var exists = Convert.ToInt32(cmdChk.ExecuteScalar()) > 0;
                if (exists)
                {
                    MessageBox.Show("Ese login ya existe. Elige otro.");
                    return;
                }
            }

            // Insertar
            using (var cn = Db.Open())
            using (var cmd = new OracleCommand(@"
    INSERT INTO USUARIO (LOGIN, NOMBRE, EMAIL, CLAVE_HASH, NIVEL, ACTIVO)
    VALUES (:login, :nombre, :email, :hash, :nivel, :activo)", cn))
            {
                cmd.BindByName = true;
                cmd.Parameters.Add(":login", login);
                cmd.Parameters.Add(":nombre", nombre);
                cmd.Parameters.Add(":email", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);
                cmd.Parameters.Add(":hash", hash);
                cmd.Parameters.Add(":nivel", nivel);
                cmd.Parameters.Add(":activo", activo);

                try { cmd.ExecuteNonQuery(); }
                catch (OracleException ox) when (ox.Number == 1) // unique constraint
                {
                    MessageBox.Show("Ese login o email ya existe. Elige otros valores.");
                    return;
                }
            }

            UsernameCreado = login;   // para precargar en el FormLogin
            DialogResult = DialogResult.OK;
            Close();
        }

        private class NivelItem
        {
            public string Texto { get; }
            public int Valor { get; }
            public NivelItem(string texto, int valor) { Texto = texto; Valor = valor; }
            public override string ToString() => Texto;
        }
    }
}
