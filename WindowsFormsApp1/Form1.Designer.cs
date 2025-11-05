namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCategorias = new System.Windows.Forms.Button();
            this.btnProductos = new System.Windows.Forms.Button();
            this.btnClientes = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnProbar = new System.Windows.Forms.Button();
            this.btnVentas = new System.Windows.Forms.Button();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.btnFactura = new System.Windows.Forms.Button();
            this.btnVentasMes = new System.Windows.Forms.Button();
            this.btnIvaTrimestre = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCategorias
            // 
            this.btnCategorias.BackColor = System.Drawing.Color.SkyBlue;
            this.btnCategorias.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnCategorias.Location = new System.Drawing.Point(240, 152);
            this.btnCategorias.Name = "btnCategorias";
            this.btnCategorias.Size = new System.Drawing.Size(146, 80);
            this.btnCategorias.TabIndex = 0;
            this.btnCategorias.Text = "Categorías";
            this.btnCategorias.UseVisualStyleBackColor = false;
            this.btnCategorias.Click += new System.EventHandler(this.btnCategorias_Click);
            // 
            // btnProductos
            // 
            this.btnProductos.BackColor = System.Drawing.Color.SkyBlue;
            this.btnProductos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnProductos.Location = new System.Drawing.Point(47, 152);
            this.btnProductos.Name = "btnProductos";
            this.btnProductos.Size = new System.Drawing.Size(139, 80);
            this.btnProductos.TabIndex = 1;
            this.btnProductos.Text = "Productos";
            this.btnProductos.UseVisualStyleBackColor = false;
            this.btnProductos.Click += new System.EventHandler(this.btnProductos_Click);
            // 
            // btnClientes
            // 
            this.btnClientes.BackColor = System.Drawing.Color.SkyBlue;
            this.btnClientes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnClientes.Location = new System.Drawing.Point(431, 152);
            this.btnClientes.Name = "btnClientes";
            this.btnClientes.Size = new System.Drawing.Size(144, 80);
            this.btnClientes.TabIndex = 2;
            this.btnClientes.Text = "Clientes";
            this.btnClientes.UseVisualStyleBackColor = false;
            this.btnClientes.Click += new System.EventHandler(this.btnClientes_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Modern No. 20", 25.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(223, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(325, 44);
            this.label1.TabIndex = 3;
            this.label1.Text = "Productos ORLO";
            // 
            // btnProbar
            // 
            this.btnProbar.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnProbar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnProbar.Location = new System.Drawing.Point(12, 384);
            this.btnProbar.Name = "btnProbar";
            this.btnProbar.Size = new System.Drawing.Size(146, 55);
            this.btnProbar.TabIndex = 4;
            this.btnProbar.Text = "Probar conexion";
            this.btnProbar.UseVisualStyleBackColor = false;
            this.btnProbar.Click += new System.EventHandler(this.btnProbar_Click);
            // 
            // btnVentas
            // 
            this.btnVentas.BackColor = System.Drawing.Color.SkyBlue;
            this.btnVentas.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnVentas.Location = new System.Drawing.Point(609, 152);
            this.btnVentas.Name = "btnVentas";
            this.btnVentas.Size = new System.Drawing.Size(138, 80);
            this.btnVentas.TabIndex = 5;
            this.btnVentas.Text = "Ventas";
            this.btnVentas.UseVisualStyleBackColor = false;
            this.btnVentas.Click += new System.EventHandler(this.btnVentas_Click);
            // 
            // btnCerrarSesion
            // 
            this.btnCerrarSesion.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnCerrarSesion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnCerrarSesion.Location = new System.Drawing.Point(678, 384);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(110, 54);
            this.btnCerrarSesion.TabIndex = 6;
            this.btnCerrarSesion.Text = "Cerrar Sesión";
            this.btnCerrarSesion.UseVisualStyleBackColor = false;
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            // 
            // btnFactura
            // 
            this.btnFactura.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnFactura.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnFactura.Location = new System.Drawing.Point(231, 385);
            this.btnFactura.Name = "btnFactura";
            this.btnFactura.Size = new System.Drawing.Size(171, 54);
            this.btnFactura.TabIndex = 7;
            this.btnFactura.Text = "Factura";
            this.btnFactura.UseVisualStyleBackColor = false;
            this.btnFactura.Click += new System.EventHandler(this.btnFactura_Click);
            // 
            // btnVentasMes
            // 
            this.btnVentasMes.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnVentasMes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnVentasMes.Location = new System.Drawing.Point(438, 384);
            this.btnVentasMes.Name = "btnVentasMes";
            this.btnVentasMes.Size = new System.Drawing.Size(171, 55);
            this.btnVentasMes.TabIndex = 8;
            this.btnVentasMes.Text = "Ventas Mensuales";
            this.btnVentasMes.UseVisualStyleBackColor = false;
            this.btnVentasMes.Click += new System.EventHandler(this.btnVentasMes_Click);
            // 
            // btnIvaTrimestre
            // 
            this.btnIvaTrimestre.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnIvaTrimestre.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnIvaTrimestre.Location = new System.Drawing.Point(333, 298);
            this.btnIvaTrimestre.Name = "btnIvaTrimestre";
            this.btnIvaTrimestre.Size = new System.Drawing.Size(171, 54);
            this.btnIvaTrimestre.TabIndex = 9;
            this.btnIvaTrimestre.Text = "IVA Trimestre";
            this.btnIvaTrimestre.UseVisualStyleBackColor = false;
            this.btnIvaTrimestre.Click += new System.EventHandler(this.btnIvaTrimestre_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnIvaTrimestre);
            this.Controls.Add(this.btnVentasMes);
            this.Controls.Add(this.btnFactura);
            this.Controls.Add(this.btnCerrarSesion);
            this.Controls.Add(this.btnVentas);
            this.Controls.Add(this.btnProbar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClientes);
            this.Controls.Add(this.btnProductos);
            this.Controls.Add(this.btnCategorias);
            this.Name = "Form1";
            this.Text = "Principal";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCategorias;
        private System.Windows.Forms.Button btnProductos;
        private System.Windows.Forms.Button btnClientes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnProbar;
        private System.Windows.Forms.Button btnVentas;
        private System.Windows.Forms.Button btnCerrarSesion;
        private System.Windows.Forms.Button btnFactura;
        private System.Windows.Forms.Button btnVentasMes;
        private System.Windows.Forms.Button btnIvaTrimestre;
    }
}

