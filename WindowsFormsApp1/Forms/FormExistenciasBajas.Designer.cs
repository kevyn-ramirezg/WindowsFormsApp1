namespace WindowsFormsApp1.Forms
{
    partial class FormExistenciasBajas
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
            this.numUmbral = new System.Windows.Forms.NumericUpDown();
            this.btnConsultar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.grid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.numUmbral)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // numUmbral
            // 
            this.numUmbral.Location = new System.Drawing.Point(99, 61);
            this.numUmbral.Name = "numUmbral";
            this.numUmbral.Size = new System.Drawing.Size(120, 22);
            this.numUmbral.TabIndex = 0;
            this.numUmbral.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // btnConsultar
            // 
            this.btnConsultar.Location = new System.Drawing.Point(365, 43);
            this.btnConsultar.Name = "btnConsultar";
            this.btnConsultar.Size = new System.Drawing.Size(131, 57);
            this.btnConsultar.TabIndex = 1;
            this.btnConsultar.Text = "Consultar";
            this.btnConsultar.UseVisualStyleBackColor = true;
            // 
            // btnExportar
            // 
            this.btnExportar.Location = new System.Drawing.Point(622, 43);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(131, 57);
            this.btnExportar.TabIndex = 2;
            this.btnExportar.Text = "Exportar";
            this.btnExportar.UseVisualStyleBackColor = true;
            // 
            // grid
            // 
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Location = new System.Drawing.Point(12, 242);
            this.grid.Name = "grid";
            this.grid.RowHeadersWidth = 51;
            this.grid.RowTemplate.Height = 24;
            this.grid.Size = new System.Drawing.Size(1383, 150);
            this.grid.TabIndex = 3;
            // 
            // FormExistenciasBajas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1407, 623);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnConsultar);
            this.Controls.Add(this.numUmbral);
            this.Name = "FormExistenciasBajas";
            this.Text = "FormExistenciasBajas";
            ((System.ComponentModel.ISupportInitialize)(this.numUmbral)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numUmbral;
        private System.Windows.Forms.Button btnConsultar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.DataGridView grid;
    }
}