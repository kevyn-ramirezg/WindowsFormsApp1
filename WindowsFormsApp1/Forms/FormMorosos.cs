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
    public partial class FormMorosos : Form
    {
        public FormMorosos()
        {
            InitializeComponent();
            Load += (_, __) => Consultar();
            btnActualizar.Click += (_, __) => Consultar();
        }

        private void Consultar()
        {
            using (var cn = Data.Db.Open())
            using (var da = new OracleDataAdapter("SELECT * FROM vw_morosos ORDER BY cliente", cn))
            {
                var dt = new DataTable();
                da.Fill(dt);
                grid.DataSource = dt;
            }
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true;
        }

    }

}
