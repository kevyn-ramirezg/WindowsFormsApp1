// Utils/ReportUtils.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace WindowsFormsApp1.Utils
{
    public static class ReportUtils
    {
        public static void ShowLocalReport(IWin32Window owner,
                                           string rdlcEmbeddedPath,
                                           string dataSourceName,
                                           DataTable table,
                                           Dictionary<string, string> parameters = null,
                                           string tituloVentana = "Reporte")
        {
            var frm = new Form();  // o crea tu propio FormReportViewer si ya lo tienes
            frm.Text = tituloVentana;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.Width = 1000; frm.Height = 700;

            var viewer = new ReportViewer
            {
                Dock = DockStyle.Fill,
                ProcessingMode = ProcessingMode.Local
            };
            frm.Controls.Add(viewer);

            // RDLC embebido como recurso
            viewer.LocalReport.DataSources.Clear();
            viewer.LocalReport.ReportEmbeddedResource = rdlcEmbeddedPath;
            viewer.LocalReport.DataSources.Add(new ReportDataSource(dataSourceName, table));

            if (parameters != null)
            {
                var list = new List<ReportParameter>();
                foreach (var kv in parameters)
                    list.Add(new ReportParameter(kv.Key, kv.Value ?? ""));
                viewer.LocalReport.SetParameters(list);
            }

            viewer.RefreshReport();
            frm.ShowDialog(owner);
        }
    }
}
