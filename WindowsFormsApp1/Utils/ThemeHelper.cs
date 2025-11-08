using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public static class ThemeHelper
    {
        public static void AutoSizeGrid(DataGridView grid)
        {
            grid.AutoGenerateColumns = false;
            grid.AllowUserToResizeRows = false;
            grid.RowHeadersVisible = false;

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font(grid.Font, FontStyle.Bold);

            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.DefaultCellStyle.NullValue = "";
            grid.DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("es-CO");

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 236, 245);
        }

        public static void SetMoneyColumn(DataGridView grid, string columnName)
        {
            if (!grid.Columns.Contains(columnName)) return;
            var col = grid.Columns[columnName];
            col.DefaultCellStyle.Format = "N0"; // 1.234.567
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            col.FillWeight = 22; // distribución de ancho
        }

        public static void SetTextColumn(DataGridView grid, string columnName, float fillWeight = 18)
        {
            if (!grid.Columns.Contains(columnName)) return;
            var col = grid.Columns[columnName];
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            col.FillWeight = fillWeight;
        }
    }
}
