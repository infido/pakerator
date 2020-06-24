namespace Pakerator
{
    partial class DokumentyDlaMagazynu
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
            this.panelGora = new System.Windows.Forms.Panel();
            //this.dataGridViewDokSP = new System.Windows.Forms.DataGridView();
            this.dataGridViewDokSP = new DataGridViewGroup();
            this.panelDol = new System.Windows.Forms.Panel();
            this.bWybierz = new System.Windows.Forms.Button();
            this.bAnuluj = new System.Windows.Forms.Button();
            this.panelGora.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDokSP)).BeginInit();
            this.panelDol.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelGora
            // 
            this.panelGora.Controls.Add(this.dataGridViewDokSP);
            this.panelGora.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGora.Location = new System.Drawing.Point(0, 0);
            this.panelGora.Name = "panelGora";
            this.panelGora.Size = new System.Drawing.Size(1233, 602);
            this.panelGora.TabIndex = 0;
            // 
            // dataGridViewDokSP
            // 
            this.dataGridViewDokSP.AllowUserToAddRows = false;
            this.dataGridViewDokSP.AllowUserToDeleteRows = false;
            this.dataGridViewDokSP.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewDokSP.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDokSP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDokSP.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewDokSP.Name = "dataGridViewDokSP";
            this.dataGridViewDokSP.ReadOnly = true;
            this.dataGridViewDokSP.Size = new System.Drawing.Size(1233, 602);
            this.dataGridViewDokSP.TabIndex = 0;
            // 
            // panelDol
            // 
            this.panelDol.Controls.Add(this.bWybierz);
            this.panelDol.Controls.Add(this.bAnuluj);
            this.panelDol.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDol.Location = new System.Drawing.Point(0, 549);
            this.panelDol.Name = "panelDol";
            this.panelDol.Size = new System.Drawing.Size(1233, 53);
            this.panelDol.TabIndex = 1;
            // 
            // bWybierz
            // 
            this.bWybierz.Location = new System.Drawing.Point(1146, 15);
            this.bWybierz.Name = "bWybierz";
            this.bWybierz.Size = new System.Drawing.Size(75, 23);
            this.bWybierz.TabIndex = 1;
            this.bWybierz.Text = "&Wybierz";
            this.bWybierz.UseVisualStyleBackColor = true;
            this.bWybierz.Click += new System.EventHandler(this.bWybierz_Click);
            // 
            // bAnuluj
            // 
            this.bAnuluj.Location = new System.Drawing.Point(1065, 15);
            this.bAnuluj.Name = "bAnuluj";
            this.bAnuluj.Size = new System.Drawing.Size(75, 23);
            this.bAnuluj.TabIndex = 0;
            this.bAnuluj.Text = "&Anuluj";
            this.bAnuluj.UseVisualStyleBackColor = true;
            this.bAnuluj.Click += new System.EventHandler(this.bAnuluj_Click);
            // 
            // DokumentyDlaMagazynu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1233, 602);
            this.Controls.Add(this.panelDol);
            this.Controls.Add(this.panelGora);
            this.Name = "DokumentyDlaMagazynu";
            this.ShowIcon = false;
            this.Text = "Lista dokumentów sprzedaży dla magazynu ";
            this.panelGora.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDokSP)).EndInit();
            this.panelDol.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelGora;
        //private System.Windows.Forms.DataGridView dataGridViewDokSP;
        private DataGridViewGroup dataGridViewDokSP;
        private System.Windows.Forms.Panel panelDol;
        private System.Windows.Forms.Button bWybierz;
        private System.Windows.Forms.Button bAnuluj;
    }
}