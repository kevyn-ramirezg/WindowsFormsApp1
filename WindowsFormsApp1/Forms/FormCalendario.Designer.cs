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
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // cal
            // 
            this.cal.BackColor = System.Drawing.SystemColors.Info;
            this.cal.CalendarDimensions = new System.Drawing.Size(6, 1);
            this.cal.Location = new System.Drawing.Point(18, 85);
            this.cal.MaxSelectionCount = 31;
            this.cal.Name = "cal";
            this.cal.TabIndex = 0;
            // 
            // btnReporteVentas
            // 
            this.btnReporteVentas.BackColor = System.Drawing.Color.SkyBlue;
            this.btnReporteVentas.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnReporteVentas.Location = new System.Drawing.Point(46, 336);
            this.btnReporteVentas.Name = "btnReporteVentas";
            this.btnReporteVentas.Size = new System.Drawing.Size(136, 54);
            this.btnReporteVentas.TabIndex = 1;
            this.btnReporteVentas.Text = "Reporte de Ventas";
            this.btnReporteVentas.UseVisualStyleBackColor = false;
            // 
            // btnCargarGrilla
            // 
            this.btnCargarGrilla.BackColor = System.Drawing.Color.SkyBlue;
            this.btnCargarGrilla.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnCargarGrilla.Location = new System.Drawing.Point(279, 336);
            this.btnCargarGrilla.Name = "btnCargarGrilla";
            this.btnCargarGrilla.Size = new System.Drawing.Size(136, 54);
            this.btnCargarGrilla.TabIndex = 2;
            this.btnCargarGrilla.Text = "Cargar Grid";
            this.btnCargarGrilla.UseVisualStyleBackColor = false;
            // 
            // btnHoy
            // 
            this.btnHoy.BackColor = System.Drawing.Color.SkyBlue;
            this.btnHoy.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnHoy.Location = new System.Drawing.Point(520, 336);
            this.btnHoy.Name = "btnHoy";
            this.btnHoy.Size = new System.Drawing.Size(136, 54);
            this.btnHoy.TabIndex = 3;
            this.btnHoy.Text = "Hoy";
            this.btnHoy.UseVisualStyleBackColor = false;
            // 
            // grid
            // 
            this.grid.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Location = new System.Drawing.Point(12, 450);
            this.grid.Name = "grid";
            this.grid.RowHeadersWidth = 51;
            this.grid.RowTemplate.Height = 24;
            this.grid.Size = new System.Drawing.Size(1622, 275);
            this.grid.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Modern No. 20", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(239, 34);
            this.label1.TabIndex = 5;
            this.label1.Text = "CALENDARIO";
            // 
            // FormCalendario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(1646, 737);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.btnHoy);
            this.Controls.Add(this.btnCargarGrilla);
            this.Controls.Add(this.btnReporteVentas);
            this.Controls.Add(this.cal);
            this.Name = "FormCalendario";
            this.Text = "FormCalendario";
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MonthCalendar cal;
        private System.Windows.Forms.Button btnReporteVentas;
        private System.Windows.Forms.Button btnCargarGrilla;
        private System.Windows.Forms.Button btnHoy;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.Label label1;
    }
}