namespace WindowsFormsApp1.Forms
{
    partial class FormCreditos
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
            this.label2 = new System.Windows.Forms.Label();
            this.numVentaId = new System.Windows.Forms.NumericUpDown();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.gridCuotas = new System.Windows.Forms.DataGridView();
            this.grpTotales = new System.Windows.Forms.GroupBox();
            this.lblPendiente = new System.Windows.Forms.Label();
            this.lblPagado = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblPendienteTitle = new System.Windows.Forms.Label();
            this.lblPagadoTitle = new System.Windows.Forms.Label();
            this.lblTotalTitle = new System.Windows.Forms.Label();
            this.grpPago = new System.Windows.Forms.GroupBox();
            this.btnPagar = new System.Windows.Forms.Button();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lblFecha = new System.Windows.Forms.Label();
            this.numMonto = new System.Windows.Forms.NumericUpDown();
            this.lblMonto = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numVentaId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCuotas)).BeginInit();
            this.grpTotales.SuspendLayout();
            this.grpPago.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMonto)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Font = new System.Drawing.Font("Elephant", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(14, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(425, 44);
            this.label1.TabIndex = 0;
            this.label1.Text = "CRÉDITOS Y PAGOS";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Elephant", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(18, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "Venta #:";
            // 
            // numVentaId
            // 
            this.numVentaId.BackColor = System.Drawing.Color.White;
            this.numVentaId.Font = new System.Drawing.Font("Elephant", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numVentaId.Location = new System.Drawing.Point(131, 70);
            this.numVentaId.Maximum = new decimal(new int[] {
            1661992959,
            1808227885,
            5,
            0});
            this.numVentaId.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numVentaId.Name = "numVentaId";
            this.numVentaId.Size = new System.Drawing.Size(135, 29);
            this.numVentaId.TabIndex = 2;
            this.numVentaId.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnBuscar
            // 
            this.btnBuscar.BackColor = System.Drawing.Color.White;
            this.btnBuscar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscar.Font = new System.Drawing.Font("Elephant", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.Location = new System.Drawing.Point(346, 62);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(112, 44);
            this.btnBuscar.TabIndex = 3;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = false;
            // 
            // gridCuotas
            // 
            this.gridCuotas.AllowUserToAddRows = false;
            this.gridCuotas.AllowUserToDeleteRows = false;
            this.gridCuotas.BackgroundColor = System.Drawing.Color.DarkSeaGreen;
            this.gridCuotas.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.gridCuotas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridCuotas.Location = new System.Drawing.Point(12, 126);
            this.gridCuotas.MultiSelect = false;
            this.gridCuotas.Name = "gridCuotas";
            this.gridCuotas.ReadOnly = true;
            this.gridCuotas.RowHeadersVisible = false;
            this.gridCuotas.RowHeadersWidth = 51;
            this.gridCuotas.RowTemplate.Height = 24;
            this.gridCuotas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridCuotas.Size = new System.Drawing.Size(1338, 327);
            this.gridCuotas.TabIndex = 4;
            // 
            // grpTotales
            // 
            this.grpTotales.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.grpTotales.Controls.Add(this.lblPendiente);
            this.grpTotales.Controls.Add(this.lblPagado);
            this.grpTotales.Controls.Add(this.lblTotal);
            this.grpTotales.Controls.Add(this.lblPendienteTitle);
            this.grpTotales.Controls.Add(this.lblPagadoTitle);
            this.grpTotales.Controls.Add(this.lblTotalTitle);
            this.grpTotales.Font = new System.Drawing.Font("Elephant", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpTotales.ForeColor = System.Drawing.Color.White;
            this.grpTotales.Location = new System.Drawing.Point(131, 480);
            this.grpTotales.Name = "grpTotales";
            this.grpTotales.Size = new System.Drawing.Size(451, 276);
            this.grpTotales.TabIndex = 5;
            this.grpTotales.TabStop = false;
            this.grpTotales.Text = "Totales";
            // 
            // lblPendiente
            // 
            this.lblPendiente.AutoSize = true;
            this.lblPendiente.Location = new System.Drawing.Point(190, 193);
            this.lblPendiente.Name = "lblPendiente";
            this.lblPendiente.Size = new System.Drawing.Size(23, 22);
            this.lblPendiente.TabIndex = 5;
            this.lblPendiente.Text = "0";
            // 
            // lblPagado
            // 
            this.lblPagado.AutoSize = true;
            this.lblPagado.Location = new System.Drawing.Point(190, 110);
            this.lblPagado.Name = "lblPagado";
            this.lblPagado.Size = new System.Drawing.Size(23, 22);
            this.lblPagado.TabIndex = 4;
            this.lblPagado.Text = "0";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(190, 42);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(23, 22);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.Text = "0";
            // 
            // lblPendienteTitle
            // 
            this.lblPendienteTitle.AutoSize = true;
            this.lblPendienteTitle.Location = new System.Drawing.Point(25, 193);
            this.lblPendienteTitle.Name = "lblPendienteTitle";
            this.lblPendienteTitle.Size = new System.Drawing.Size(102, 22);
            this.lblPendienteTitle.TabIndex = 2;
            this.lblPendienteTitle.Text = "Pendiente:";
            // 
            // lblPagadoTitle
            // 
            this.lblPagadoTitle.AutoSize = true;
            this.lblPagadoTitle.Location = new System.Drawing.Point(25, 110);
            this.lblPagadoTitle.Name = "lblPagadoTitle";
            this.lblPagadoTitle.Size = new System.Drawing.Size(127, 22);
            this.lblPagadoTitle.TabIndex = 1;
            this.lblPagadoTitle.Text = "Total pagado:";
            // 
            // lblTotalTitle
            // 
            this.lblTotalTitle.AutoSize = true;
            this.lblTotalTitle.Location = new System.Drawing.Point(25, 42);
            this.lblTotalTitle.Name = "lblTotalTitle";
            this.lblTotalTitle.Size = new System.Drawing.Size(127, 22);
            this.lblTotalTitle.TabIndex = 0;
            this.lblTotalTitle.Text = "Total credito:";
            // 
            // grpPago
            // 
            this.grpPago.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.grpPago.Controls.Add(this.btnPagar);
            this.grpPago.Controls.Add(this.dtpFecha);
            this.grpPago.Controls.Add(this.lblFecha);
            this.grpPago.Controls.Add(this.numMonto);
            this.grpPago.Controls.Add(this.lblMonto);
            this.grpPago.Font = new System.Drawing.Font("Elephant", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPago.ForeColor = System.Drawing.Color.White;
            this.grpPago.Location = new System.Drawing.Point(722, 480);
            this.grpPago.Name = "grpPago";
            this.grpPago.Size = new System.Drawing.Size(473, 276);
            this.grpPago.TabIndex = 6;
            this.grpPago.TabStop = false;
            this.grpPago.Text = "Registrar Pago";
            // 
            // btnPagar
            // 
            this.btnPagar.BackColor = System.Drawing.Color.White;
            this.btnPagar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPagar.ForeColor = System.Drawing.Color.Black;
            this.btnPagar.Location = new System.Drawing.Point(123, 163);
            this.btnPagar.Name = "btnPagar";
            this.btnPagar.Size = new System.Drawing.Size(189, 61);
            this.btnPagar.TabIndex = 4;
            this.btnPagar.Text = "Registrar Pago";
            this.btnPagar.UseVisualStyleBackColor = false;
            // 
            // dtpFecha
            // 
            this.dtpFecha.CalendarTitleBackColor = System.Drawing.SystemColors.Info;
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(123, 105);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(188, 29);
            this.dtpFecha.TabIndex = 3;
            // 
            // lblFecha
            // 
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(27, 110);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(67, 22);
            this.lblFecha.TabIndex = 2;
            this.lblFecha.Text = "Fecha:";
            // 
            // numMonto
            // 
            this.numMonto.BackColor = System.Drawing.Color.White;
            this.numMonto.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMonto.Location = new System.Drawing.Point(123, 40);
            this.numMonto.Maximum = new decimal(new int[] {
            -279969792,
            451204834,
            27,
            0});
            this.numMonto.Name = "numMonto";
            this.numMonto.Size = new System.Drawing.Size(189, 29);
            this.numMonto.TabIndex = 1;
            // 
            // lblMonto
            // 
            this.lblMonto.AutoSize = true;
            this.lblMonto.Location = new System.Drawing.Point(27, 46);
            this.lblMonto.Name = "lblMonto";
            this.lblMonto.Size = new System.Drawing.Size(71, 22);
            this.lblMonto.TabIndex = 0;
            this.lblMonto.Text = "Monto:";
            // 
            // FormCreditos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGreen;
            this.ClientSize = new System.Drawing.Size(1365, 761);
            this.Controls.Add(this.grpPago);
            this.Controls.Add(this.grpTotales);
            this.Controls.Add(this.gridCuotas);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.numVentaId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Elephant", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FormCreditos";
            this.Text = "Creditos/Pagos";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.numVentaId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridCuotas)).EndInit();
            this.grpTotales.ResumeLayout(false);
            this.grpTotales.PerformLayout();
            this.grpPago.ResumeLayout(false);
            this.grpPago.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMonto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numVentaId;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.DataGridView gridCuotas;

        private System.Windows.Forms.GroupBox grpTotales;
        private System.Windows.Forms.Label lblTotalTitle;
        private System.Windows.Forms.Label lblPagadoTitle;
        private System.Windows.Forms.Label lblPendienteTitle;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblPagado;
        private System.Windows.Forms.Label lblPendiente;

        private System.Windows.Forms.GroupBox grpPago;
        private System.Windows.Forms.Label lblMonto;
        private System.Windows.Forms.NumericUpDown numMonto;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Button btnPagar;

    }
}