using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    public partial class FormExportadorCsv : Form
    {
        private DataTable _dt;

        public FormExportadorCsv()
        {
            this.Text += $"  | Nivel={WindowsFormsApp1.Security.Session.Nivel}";

            InitializeComponent();
            Load += (_, __) =>
            {
                cmbOrigen.Items.AddRange(new[] { "Ventas (rango)", "Top productos (rango)", "Morosos", "Existencias bajas" });
                cmbOrigen.SelectedIndex = 0;
                dtpIni.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dtpFin.Value = DateTime.Today;
            };
            cmbOrigen.SelectedIndexChanged += (_, __) => ToggleInputs();
            btnGenerar.Click += (_, __) => Generar();
            btnGuardar.Click += (_, __) => GuardarCsv();
        }

        private void ToggleInputs()
        {
            var o = cmbOrigen.SelectedItem?.ToString();
            bool usaRango = o.Contains("(rango)");
            dtpIni.Enabled = dtpFin.Enabled = usaRango;
            numTop.Enabled = o.StartsWith("Top productos");
        }

        private void Generar()
        {
            switch (cmbOrigen.SelectedItem?.ToString())
            {
                case "Ventas (rango)": _dt = QVentas(); break;
                case "Top productos (rango)": _dt = QTop(); break;
                case "Morosos": _dt = QMorosos(); break;
                case "Existencias bajas": _dt = QExistencias(); break;
            }
            grid.DataSource = _dt;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private DataTable QVentas()
        {
            using (var cn = Data.Db.Open())
            using (var da = new OracleDataAdapter(@"
        SELECT v.id AS VENTA_ID, TRUNC(v.fecha) AS FECHA, c.nombre AS CLIENTE, v.tipo,
               v.subtotal, v.iva_total, v.total
        FROM venta v JOIN cliente c ON c.id = v.cliente_id
        WHERE TRUNC(v.fecha) BETWEEN :ini AND :fin
        ORDER BY v.fecha", cn))
            {
                da.SelectCommand.BindByName = true;
                da.SelectCommand.Parameters.Add(":ini", dtpIni.Value.Date);
                da.SelectCommand.Parameters.Add(":fin", dtpFin.Value.Date);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private DataTable QTop()
        {
            using (var cn = Data.Db.Open())
            using (var da = new OracleDataAdapter(@"
        SELECT p.nombre AS PRODUCTO,
               SUM(d.cantidad) AS CANTIDAD,
               SUM(ROUND(d.cantidad*d.precio_unit*(1+p.iva/100),0)) AS VENTAS
        FROM venta v
        JOIN detalleventa d ON d.venta_id = v.id
        JOIN producto p     ON p.id = d.producto_id
        WHERE TRUNC(v.fecha) BETWEEN :ini AND :fin
        GROUP BY p.nombre
        ORDER BY VENTAS DESC", cn))
            {
                da.SelectCommand.BindByName = true;
                da.SelectCommand.Parameters.Add(":ini", dtpIni.Value.Date);
                da.SelectCommand.Parameters.Add(":fin", dtpFin.Value.Date);

                var dt = new DataTable();
                da.Fill(dt);

                var top = dt.AsEnumerable().Take((int)numTop.Value);
                return top.Any() ? top.CopyToDataTable() : dt.Clone();
            }
        }

        private DataTable QMorosos()
        {
            using (var cn = Data.Db.Open())
            using (var da = new OracleDataAdapter("SELECT * FROM vw_morosos ORDER BY cliente", cn))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private DataTable QExistencias()
        {
            using (var cn = Data.Db.Open())
            using (var da = new OracleDataAdapter(@"
        SELECT id, nombre, stock, precio_venta
        FROM producto
        WHERE stock <= 5
        ORDER BY stock ASC", cn))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private void GuardarCsv()
        {
            if (_dt == null || _dt.Rows.Count == 0) { MessageBox.Show("No hay datos."); return; }

            sfd.Filter = "CSV (*.csv)|*.csv";
            sfd.FileName = "export.csv";
            if (sfd.ShowDialog() != DialogResult.OK) return;

            using (var sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
            {
                // encabezados
                sw.WriteLine(string.Join(",", _dt.Columns.Cast<DataColumn>().Select(c => Escape(c.ColumnName))));
                // filas
                foreach (DataRow r in _dt.Rows)
                    sw.WriteLine(string.Join(",", _dt.Columns.Cast<DataColumn>().Select(c => Escape(r[c]))));
            }
            MessageBox.Show("CSV guardado.");
        }

        private string Escape(object v)
        {
            var s = v == null ? "" : v.ToString();
            s = s.Replace("\"", "\"\"");
            return "\"" + s + "\"";
        }
    }

}
