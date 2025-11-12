namespace WindowsFormsApp1.Forms
{
    partial class FormAcercaDe
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
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblAutor = new System.Windows.Forms.Label();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.lnkRepo = new System.Windows.Forms.LinkLabel();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // picLogo
            // 
            this.picLogo.Location = new System.Drawing.Point(492, 104);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(477, 175);
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(40, 25);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(44, 16);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "label1";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(43, 65);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(44, 16);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "label1";
            // 
            // lblAutor
            // 
            this.lblAutor.AutoSize = true;
            this.lblAutor.Location = new System.Drawing.Point(46, 104);
            this.lblAutor.Name = "lblAutor";
            this.lblAutor.Size = new System.Drawing.Size(44, 16);
            this.lblAutor.TabIndex = 3;
            this.lblAutor.Text = "label1";
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Location = new System.Drawing.Point(43, 146);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(44, 16);
            this.lblDescripcion.TabIndex = 4;
            this.lblDescripcion.Text = "label1";
            // 
            // lnkRepo
            // 
            this.lnkRepo.AutoSize = true;
            this.lnkRepo.Location = new System.Drawing.Point(46, 202);
            this.lnkRepo.Name = "lnkRepo";
            this.lnkRepo.Size = new System.Drawing.Size(68, 16);
            this.lnkRepo.TabIndex = 5;
            this.lnkRepo.TabStop = true;
            this.lnkRepo.Text = "linkLabel1";
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(49, 262);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 23);
            this.btnCerrar.TabIndex = 6;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            // 
            // FormAcercaDe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1348, 721);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lnkRepo);
            this.Controls.Add(this.lblDescripcion);
            this.Controls.Add(this.lblAutor);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.picLogo);
            this.Name = "FormAcercaDe";
            this.Text = "FormAcercaDe";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblAutor;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.LinkLabel lnkRepo;
        private System.Windows.Forms.Button btnCerrar;
    }
}