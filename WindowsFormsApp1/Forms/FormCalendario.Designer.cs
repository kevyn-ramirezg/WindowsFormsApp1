namespace WindowsFormsApp1.Forms
{
    partial class FormCalendario
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cal = new System.Windows.Forms.MonthCalendar();
            this.btnReporteVentas = new System.Windows.Forms.Button();
            this.btnCargarGrilla = new System.Windows.Forms.Button();
            this.btnHoy = new System.Windows.Forms.Button();
            this.grid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // cal
            // 
            this.cal.CalendarDimensions = new System.Drawing.Size(6, 1);
            this.cal.Location = new System.Drawing.Point(18, 18);
            this.cal.MaxSelectionCount = 31;
            this.cal.Name = "cal";
            this.cal.TabIndex = 0;
            // 
            // btnReporteVentas
            // 
            this.btnReporteVentas.Location = new System.Drawing.Point(46, 269);
            this.btnReporteVentas.Name = "btnReporteVentas";
            this.btnReporteVentas.Size = new System.Drawing.Size(136, 54);
            this.btnReporteVentas.TabIndex = 1;
            this.btnReporteVentas.Text = "Reporte de Ventas";
            this.btnReporteVentas.UseVisualStyleBackColor = true;
            // 
            // btnCargarGrilla
            // 
            this.btnCargarGrilla.Location = new System.Drawing.Point(301, 269);
            this.btnCargarGrilla.Name = "btnCargarGrilla";
            this.btnCargarGrilla.Size = new System.Drawing.Size(136, 54);
            this.btnCargarGrilla.TabIndex = 2;
            this.btnCargarGrilla.Text = "Cargar Grid";
            this.btnCargarGrilla.UseVisualStyleBackColor = true;
            // 
            // btnHoy
            // 
            this.btnHoy.Location = new System.Drawing.Point(566, 269);
            this.btnHoy.Name = "btnHoy";
            this.btnHoy.Size = new System.Drawing.Size(136, 54);
            this.btnHoy.TabIndex = 3;
            this.btnHoy.Text = "Hoy";
            this.btnHoy.UseVisualStyleBackColor = true;
            // 
            // grid
            // 
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Location = new System.Drawing.Point(12, 383);
            this.grid.Name = "grid";
            this.grid.RowHeadersWidth = 51;
            this.grid.RowTemplate.Height = 24;
            this.grid.Size = new System.Drawing.Size(1622, 150);
            this.grid.TabIndex = 4;
            // 
            // FormCalendario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1646, 737);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.btnHoy);
            this.Controls.Add(this.btnCargarGrilla);
            this.Controls.Add(this.btnReporteVentas);
            this.Controls.Add(this.cal);
            this.Name = "FormCalendario";
            this.Text = "FormCalendario";
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MonthCalendar cal;
        private System.Windows.Forms.Button btnReporteVentas;
        private System.Windows.Forms.Button btnCargarGrilla;
        private System.Windows.Forms.Button btnHoy;
        private System.Windows.Forms.DataGridView grid;
    }
}