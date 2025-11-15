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

namespace WindowsFormsApp1.Forms
{
    public partial class FormBitacora : Form
    {
        public FormBitacora()
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";

            InitializeComponent();
            Load += (_, __) =>
            {
                dtpIni.Value = DateTime.Today.AddDays(-7);
                dtpFin.Value = DateTime.Today;
                Consultar();
            };
            btnBuscar.Click += (_, __) => Consultar();
        }

        private void Consultar()
        {
            using (var cn = Data.Db.Open())
            using (var da = new OracleDataAdapter(
                @"SELECT b.id,
             u.login       AS usuario,
             b.evento      AS evento,
             b.fecha_hora  AS fecha
      FROM bitacora b
      LEFT JOIN usuario u ON u.id = b.usuario_id
      WHERE TRUNC(b.fecha_hora) BETWEEN :ini AND :fin
        AND (:usr IS NULL OR UPPER(u.login) LIKE '%'||UPPER(:usr)||'%')
      ORDER BY b.fecha_hora DESC", cn))
            {
                da.SelectCommand.BindByName = true;
                da.SelectCommand.Parameters.Add(":ini", dtpIni.Value.Date);
                da.SelectCommand.Parameters.Add(":fin", dtpFin.Value.Date);
                var filtro = txtUsuario.Text?.Trim();
                da.SelectCommand.Parameters.Add(":usr", string.IsNullOrEmpty(filtro) ? (object)DBNull.Value : filtro);

                var dt = new DataTable();
                da.Fill(dt);
                grid.DataSource = dt;
            }

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true;
            lblResumen.Text = $"Registros: {((DataTable)grid.DataSource).Rows.Count:N0}";
        }

    }

}
