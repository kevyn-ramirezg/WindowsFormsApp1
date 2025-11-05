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
            this.SuspendLayout();
            // 
            // btnCategorias
            // 
            this.btnCategorias.Location = new System.Drawing.Point(247, 278);
            this.btnCategorias.Name = "btnCategorias";
            this.btnCategorias.Size = new System.Drawing.Size(130, 60);
            this.btnCategorias.TabIndex = 0;
            this.btnCategorias.Text = "Categorías";
            this.btnCategorias.UseVisualStyleBackColor = true;
            this.btnCategorias.Click += new System.EventHandler(this.btnCategorias_Click);
            // 
            // btnProductos
            // 
            this.btnProductos.Location = new System.Drawing.Point(54, 278);
            this.btnProductos.Name = "btnProductos";
            this.btnProductos.Size = new System.Drawing.Size(123, 60);
            this.btnProductos.TabIndex = 1;
            this.btnProductos.Text = "Productos";
            this.btnProductos.UseVisualStyleBackColor = true;
            this.btnProductos.Click += new System.EventHandler(this.btnProductos_Click);
            // 
            // btnClientes
            // 
            this.btnClientes.Location = new System.Drawing.Point(438, 278);
            this.btnClientes.Name = "btnClientes";
            this.btnClientes.Size = new System.Drawing.Size(128, 60);
            this.btnClientes.TabIndex = 2;
            this.btnClientes.Text = "Clientes";
            this.btnClientes.UseVisualStyleBackColor = true;
            this.btnClientes.Click += new System.EventHandler(this.btnClientes_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
            this.label1.Location = new System.Drawing.Point(260, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(237, 33);
            this.label1.TabIndex = 3;
            this.label1.Text = "Productos ORLO";
            // 
            // btnProbar
            // 
            this.btnProbar.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnProbar.Location = new System.Drawing.Point(12, 398);
            this.btnProbar.Name = "btnProbar";
            this.btnProbar.Size = new System.Drawing.Size(130, 41);
            this.btnProbar.TabIndex = 4;
            this.btnProbar.Text = "Probar conexion";
            this.btnProbar.UseVisualStyleBackColor = false;
            this.btnProbar.Click += new System.EventHandler(this.btnProbar_Click);
            // 
            // btnVentas
            // 
            this.btnVentas.Location = new System.Drawing.Point(616, 278);
            this.btnVentas.Name = "btnVentas";
            this.btnVentas.Size = new System.Drawing.Size(122, 60);
            this.btnVentas.TabIndex = 5;
            this.btnVentas.Text = "Ventas";
            this.btnVentas.UseVisualStyleBackColor = true;
            this.btnVentas.Click += new System.EventHandler(this.btnVentas_Click);
            // 
            // btnCerrarSesion
            // 
            this.btnCerrarSesion.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.btnCerrarSesion.Location = new System.Drawing.Point(678, 398);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(110, 40);
            this.btnCerrarSesion.TabIndex = 6;
            this.btnCerrarSesion.Text = "Cerrar Sesión";
            this.btnCerrarSesion.UseVisualStyleBackColor = false;
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(800, 450);
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
    }
}

