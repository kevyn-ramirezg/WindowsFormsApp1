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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.cal.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cal.Location = new System.Drawing.Point(19, 85);
            this.cal.MaxSelectionCount = 31;
            this.cal.Name = "cal";
            this.cal.TabIndex = 0;
            // 
            // btnReporteVentas
            // 
            this.btnReporteVentas.BackColor = System.Drawing.Color.SkyBlue;
            this.btnReporteVentas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReporteVentas.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReporteVentas.Location = new System.Drawing.Point(45, 336);
            this.btnReporteVentas.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnReporteVentas.Name = "btnReporteVentas";
            this.btnReporteVentas.Size = new System.Drawing.Size(175, 54);
            this.btnReporteVentas.TabIndex = 1;
            this.btnReporteVentas.Text = "Reporte de Ventas";
            this.btnReporteVentas.UseVisualStyleBackColor = false;
            // 
            // btnCargarGrilla
            // 
            this.btnCargarGrilla.BackColor = System.Drawing.Color.SkyBlue;
            this.btnCargarGrilla.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCargarGrilla.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCargarGrilla.Location = new System.Drawing.Point(279, 336);
            this.btnCargarGrilla.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCargarGrilla.Name = "btnCargarGrilla";
            this.btnCargarGrilla.Size = new System.Drawing.Size(175, 54);
            this.btnCargarGrilla.TabIndex = 2;
            this.btnCargarGrilla.Text = "Cargar Grid";
            this.btnCargarGrilla.UseVisualStyleBackColor = false;
            // 
            // btnHoy
            // 
            this.btnHoy.BackColor = System.Drawing.Color.SkyBlue;
            this.btnHoy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHoy.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHoy.Location = new System.Drawing.Point(520, 336);
            this.btnHoy.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnHoy.Name = "btnHoy";
            this.btnHoy.Size = new System.Drawing.Size(175, 54);
            this.btnHoy.TabIndex = 3;
            this.btnHoy.Text = "Hoy";
            this.btnHoy.UseVisualStyleBackColor = false;
            // 
            // grid
            // 
            this.grid.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Location = new System.Drawing.Point(12, 450);
            this.grid.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grid.Name = "grid";
            this.grid.RowHeadersWidth = 51;
            this.grid.RowTemplate.Height = 24;
            this.grid.Size = new System.Drawing.Size(1621, 274);
            this.grid.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Modern No. 20", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 26);
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
            this.ClientSize = new System.Drawing.Size(1645, 737);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.btnHoy);
            this.Controls.Add(this.btnCargarGrilla);
            this.Controls.Add(this.btnReporteVentas);
            this.Controls.Add(this.cal);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormCalendario";
            this.Text = "FormCalendario";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
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