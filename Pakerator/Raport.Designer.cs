namespace Pakerator
{
    partial class Raport
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
            this.panel1Gora = new System.Windows.Forms.Panel();
            this.dataGridViewRaport = new System.Windows.Forms.DataGridView();
            this.panel2Dol = new System.Windows.Forms.Panel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.panel1Gora.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRaport)).BeginInit();
            this.panel2Dol.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1Gora
            // 
            this.panel1Gora.Controls.Add(this.dataGridViewRaport);
            this.panel1Gora.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1Gora.Location = new System.Drawing.Point(0, 0);
            this.panel1Gora.Name = "panel1Gora";
            this.panel1Gora.Size = new System.Drawing.Size(1186, 579);
            this.panel1Gora.TabIndex = 0;
            // 
            // dataGridViewRaport
            // 
            this.dataGridViewRaport.AllowUserToAddRows = false;
            this.dataGridViewRaport.AllowUserToDeleteRows = false;
            this.dataGridViewRaport.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewRaport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRaport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRaport.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewRaport.Name = "dataGridViewRaport";
            this.dataGridViewRaport.ReadOnly = true;
            this.dataGridViewRaport.Size = new System.Drawing.Size(1186, 579);
            this.dataGridViewRaport.TabIndex = 0;
            // 
            // panel2Dol
            // 
            this.panel2Dol.Controls.Add(this.buttonClose);
            this.panel2Dol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2Dol.Location = new System.Drawing.Point(0, 579);
            this.panel2Dol.Name = "panel2Dol";
            this.panel2Dol.Size = new System.Drawing.Size(1186, 39);
            this.panel2Dol.TabIndex = 1;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(1099, 6);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "&Zamknij";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // Raport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1186, 618);
            this.Controls.Add(this.panel2Dol);
            this.Controls.Add(this.panel1Gora);
            this.Name = "Raport";
            this.ShowIcon = false;
            this.Text = "Raport";
            this.panel1Gora.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRaport)).EndInit();
            this.panel2Dol.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1Gora;
        private System.Windows.Forms.DataGridView dataGridViewRaport;
        private System.Windows.Forms.Panel panel2Dol;
        private System.Windows.Forms.Button buttonClose;
    }
}