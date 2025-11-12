namespace WindowsFormsApp1.Forms
{
    partial class FormReporteVentas
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
            this.label1 = new System.Windows.Forms.Label();
            this.dtpIni = new System.Windows.Forms.DateTimePicker();
            this.dtpFin = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnConsultar = new System.Windows.Forms.Button();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.gridVentas = new System.Windows.Forms.DataGridView();
            this.gridTop = new System.Windows.Forms.DataGridView();
            this.lblSubtotalTitle = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblIva = new System.Windows.Forms.Label();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblTotalTitle = new System.Windows.Forms.Label();
            this.lblIvaTitle = new System.Windows.Forms.Label();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            ((System.ComponentModel.ISupportInitialize)(this.gridVentas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTop)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Modern No. 20", 22.2F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(307, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "Reporte de Ventas";
            // 
            // dtpIni
            // 
            this.dtpIni.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dtpIni.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpIni.Location = new System.Drawing.Point(147, 93);
            this.dtpIni.Margin = new System.Windows.Forms.Padding(4);
            this.dtpIni.Name = "dtpIni";
            this.dtpIni.Size = new System.Drawing.Size(249, 26);
            this.dtpIni.TabIndex = 1;
            // 
            // dtpFin
            // 
            this.dtpFin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dtpFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFin.Location = new System.Drawing.Point(147, 174);
            this.dtpFin.Margin = new System.Windows.Forms.Padding(4);
            this.dtpFin.Name = "dtpFin";
            this.dtpFin.Size = new System.Drawing.Size(249, 26);
            this.dtpFin.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label2.Location = new System.Drawing.Point(19, 99);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Fecha Inicio: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label3.Location = new System.Drawing.Point(19, 179);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Fecha Fin: ";
            // 
            // btnConsultar
            // 
            this.btnConsultar.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnConsultar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnConsultar.Location = new System.Drawing.Point(483, 88);
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Size = new System.Drawing.Size(132, 41);
            this.btnConsultar.TabIndex = 5;
            this.btnConsultar.Text = "Consultar";
            this.btnConsultar.UseVisualStyleBackColor = false;
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnExportCsv.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnExportCsv.Location = new System.Drawing.Point(483, 158);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(132, 41);
            this.btnExportCsv.TabIndex = 6;
            this.btnExportCsv.Text = "Exportar CSV";
            this.btnExportCsv.UseVisualStyleBackColor = false;
            // 
            // gridVentas
            // 
            this.gridVentas.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.gridVentas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridVentas.Location = new System.Drawing.Point(12, 250);
            this.gridVentas.Name = "gridVentas";
            this.gridVentas.RowHeadersWidth = 51;
            this.gridVentas.RowTemplate.Height = 24;
            this.gridVentas.Size = new System.Drawing.Size(1432, 244);
            this.gridVentas.TabIndex = 7;
            // 
            // gridTop
            // 
            this.gridTop.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.gridTop.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTop.Location = new System.Drawing.Point(12, 515);
            this.gridTop.Name = "gridTop";
            this.gridTop.RowHeadersWidth = 51;
            this.gridTop.RowTemplate.Height = 24;
            this.gridTop.Size = new System.Drawing.Size(674, 168);
            this.gridTop.TabIndex = 8;
            // 
            // lblSubtotalTitle
            // 
            this.lblSubtotalTitle.AutoSize = true;
            this.lblSubtotalTitle.Font = new System.Drawing.Font("Modern No. 20", 20F, System.Drawing.FontStyle.Bold);
            this.lblSubtotalTitle.Location = new System.Drawing.Point(775, 537);
            this.lblSubtotalTitle.Name = "lblSubtotalTitle";
            this.lblSubtotalTitle.Size = new System.Drawing.Size(148, 35);
            this.lblSubtotalTitle.TabIndex = 9;
            this.lblSubtotalTitle.Text = "Subtotal:";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Modern No. 20", 20F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(1198, 596);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(31, 35);
            this.lblTotal.TabIndex = 10;
            this.lblTotal.Text = "0";
            // 
            // lblIva
            // 
            this.lblIva.AutoSize = true;
            this.lblIva.Font = new System.Drawing.Font("Modern No. 20", 20F, System.Drawing.FontStyle.Bold);
            this.lblIva.Location = new System.Drawing.Point(929, 637);
            this.lblIva.Name = "lblIva";
            this.lblIva.Size = new System.Drawing.Size(31, 35);
            this.lblIva.TabIndex = 11;
            this.lblIva.Text = "0";
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.AutoSize = true;
            this.lblSubtotal.Font = new System.Drawing.Font("Modern No. 20", 20F, System.Drawing.FontStyle.Bold);
            this.lblSubtotal.Location = new System.Drawing.Point(929, 537);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(31, 35);
            this.lblSubtotal.TabIndex = 12;
            this.lblSubtotal.Text = "0";
            // 
            // lblTotalTitle
            // 
            this.lblTotalTitle.AutoSize = true;
            this.lblTotalTitle.Font = new System.Drawing.Font("Modern No. 20", 20F, System.Drawing.FontStyle.Bold);
            this.lblTotalTitle.Location = new System.Drawing.Point(1089, 596);
            this.lblTotalTitle.Name = "lblTotalTitle";
            this.lblTotalTitle.Size = new System.Drawing.Size(103, 35);
            this.lblTotalTitle.TabIndex = 13;
            this.lblTotalTitle.Text = "Total:";
            // 
            // lblIvaTitle
            // 
            this.lblIvaTitle.AutoSize = true;
            this.lblIvaTitle.Font = new System.Drawing.Font("Modern No. 20", 20F, System.Drawing.FontStyle.Bold);
            this.lblIvaTitle.Location = new System.Drawing.Point(775, 637);
            this.lblIvaTitle.Name = "lblIvaTitle";
            this.lblIvaTitle.Size = new System.Drawing.Size(87, 35);
            this.lblIvaTitle.TabIndex = 14;
            this.lblIvaTitle.Text = "IVA:";
            // 
            // reportViewer1
            // 
            this.reportViewer1.Location = new System.Drawing.Point(711, 16);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ServerReport.BearerToken = null;
            this.reportViewer1.Size = new System.Drawing.Size(691, 218);
            this.reportViewer1.TabIndex = 15;
            // 
            // FormReporteVentas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(1456, 706);
            this.Controls.Add(this.reportViewer1);
            this.Controls.Add(this.lblIvaTitle);
            this.Controls.Add(this.lblTotalTitle);
            this.Controls.Add(this.lblSubtotal);
            this.Controls.Add(this.lblIva);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblSubtotalTitle);
            this.Controls.Add(this.gridTop);
            this.Controls.Add(this.gridVentas);
            this.Controls.Add(this.btnExportCsv);
            this.Controls.Add(this.btnConsultar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtpFin);
            this.Controls.Add(this.dtpIni);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormReporteVentas";
            this.Text = "FormReporteVentas";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormReporteVentas_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.gridVentas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridTop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpIni;
        private System.Windows.Forms.DateTimePicker dtpFin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnConsultar;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.DataGridView gridVentas;
        private System.Windows.Forms.DataGridView gridTop;
        private System.Windows.Forms.Label lblSubtotalTitle;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblIva;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblTotalTitle;
        private System.Windows.Forms.Label lblIvaTitle;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    }
}