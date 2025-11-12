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
    public partial class FormExistenciasBajas : Form
    {
        public FormExistenciasBajas()
        {
            InitializeComponent();
            Load += (_, __) => { numUmbral.Value = 5; Consultar(); };
            btnConsultar.Click += (_, __) => Consultar();
        }

        private void Consultar()
        {
            using (var cn = Data.Db.Open())
            using (var da = new OracleDataAdapter(@"
        SELECT id, nombre, stock, precio_venta
        FROM producto
        WHERE stock <= :u
        ORDER BY stock ASC, nombre", cn))
            {
                da.SelectCommand.BindByName = true;
                da.SelectCommand.Parameters.Add(":u", (int)numUmbral.Value);
                var dt = new DataTable();
                da.Fill(dt);
                grid.DataSource = dt;
            }
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true;
        }

    }

}
