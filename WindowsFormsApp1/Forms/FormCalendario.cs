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
    public partial class FormCalendario : Form
    {
        public FormCalendario()
        {
            InitializeComponent();
            Load += (_, __) => { cal.SetSelectionRange(DateTime.Today, DateTime.Today); };
            btnHoy.Click += (_, __) => cal.SetSelectionRange(DateTime.Today, DateTime.Today);
            btnCargarGrilla.Click += (_, __) => CargarVentas();
            btnReporteVentas.Click += (_, __) =>
            {
                var f = new Forms.FormReporteVentas();
                f.Show(); // si quieres pasar fechas, expón un método público en FormReporteVentas
            };
        }

        private void CargarVentas()
        {
            var ini = cal.SelectionStart.Date;
            var fin = cal.SelectionEnd.Date;

            using (var cn = Data.Db.Open())
            using (var da = new OracleDataAdapter(@"
        SELECT v.id AS VENTA_ID, TRUNC(v.fecha) AS FECHA, c.nombre AS CLIENTE, v.tipo,
               v.subtotal, v.iva_total, v.total
        FROM venta v JOIN cliente c ON c.id = v.cliente_id
        WHERE TRUNC(v.fecha) BETWEEN :ini AND :fin
        ORDER BY v.fecha", cn))
            {
                da.SelectCommand.BindByName = true;
                da.SelectCommand.Parameters.Add(":ini", ini);
                da.SelectCommand.Parameters.Add(":fin", fin);
                var dt = new DataTable();
                da.Fill(dt);
                grid.DataSource = dt;
            }

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ReadOnly = true;
        }

    }

}
