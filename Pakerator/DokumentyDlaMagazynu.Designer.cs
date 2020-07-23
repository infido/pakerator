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
            this.dataGridViewDokSP = new Pakerator.DataGridViewGroup();
            this.panelDol = new System.Windows.Forms.Panel();
            this.cCzyPokazywacZaliczki = new System.Windows.Forms.CheckBox();
            this.cBRAK = new System.Windows.Forms.CheckBox();
            this.cForMove = new System.Windows.Forms.CheckBox();
            this.cOK = new System.Windows.Forms.CheckBox();
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
            this.panelDol.Controls.Add(this.cCzyPokazywacZaliczki);
            this.panelDol.Controls.Add(this.cBRAK);
            this.panelDol.Controls.Add(this.cForMove);
            this.panelDol.Controls.Add(this.cOK);
            this.panelDol.Controls.Add(this.bWybierz);
            this.panelDol.Controls.Add(this.bAnuluj);
            this.panelDol.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDol.Location = new System.Drawing.Point(0, 549);
            this.panelDol.Name = "panelDol";
            this.panelDol.Size = new System.Drawing.Size(1233, 53);
            this.panelDol.TabIndex = 1;
            // 
            // cCzyPokazywacZaliczki
            // 
            this.cCzyPokazywacZaliczki.AutoSize = true;
            this.cCzyPokazywacZaliczki.Location = new System.Drawing.Point(380, 19);
            this.cCzyPokazywacZaliczki.Name = "cCzyPokazywacZaliczki";
            this.cCzyPokazywacZaliczki.Size = new System.Drawing.Size(105, 17);
            this.cCzyPokazywacZaliczki.TabIndex = 5;
            this.cCzyPokazywacZaliczki.Text = "Wyświetl zaliczki";
            this.cCzyPokazywacZaliczki.UseVisualStyleBackColor = true;
            this.cCzyPokazywacZaliczki.CheckedChanged += new System.EventHandler(this.cCzyPokazywacZaliczki_CheckedChanged);
            // 
            // cBRAK
            // 
            this.cBRAK.AutoSize = true;
            this.cBRAK.Location = new System.Drawing.Point(276, 19);
            this.cBRAK.Name = "cBRAK";
            this.cBRAK.Size = new System.Drawing.Size(98, 17);
            this.cBRAK.TabIndex = 4;
            this.cBRAK.Text = "Status \"BRAK\"";
            this.cBRAK.UseVisualStyleBackColor = true;
            this.cBRAK.CheckedChanged += new System.EventHandler(this.cBRAK_CheckedChanged);
            // 
            // cForMove
            // 
            this.cForMove.AutoSize = true;
            this.cForMove.Checked = true;
            this.cForMove.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cForMove.Location = new System.Drawing.Point(103, 20);
            this.cForMove.Name = "cForMove";
            this.cForMove.Size = new System.Drawing.Size(167, 17);
            this.cForMove.TabIndex = 3;
            this.cForMove.Text = "Status \"DO PRZESUNIĘCIA\"";
            this.cForMove.UseVisualStyleBackColor = true;
            this.cForMove.CheckedChanged += new System.EventHandler(this.cForMove_CheckedChanged);
            // 
            // cOK
            // 
            this.cOK.AutoSize = true;
            this.cOK.Checked = true;
            this.cOK.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cOK.Location = new System.Drawing.Point(13, 20);
            this.cOK.Name = "cOK";
            this.cOK.Size = new System.Drawing.Size(84, 17);
            this.cOK.TabIndex = 2;
            this.cOK.Text = "Status \"OK\"";
            this.cOK.UseVisualStyleBackColor = true;
            this.cOK.CheckedChanged += new System.EventHandler(this.cOK_CheckedChanged);
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
            this.panelDol.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelGora;
        //private System.Windows.Forms.DataGridView dataGridViewDokSP;
        private DataGridViewGroup dataGridViewDokSP;
        private System.Windows.Forms.Panel panelDol;
        private System.Windows.Forms.Button bWybierz;
        private System.Windows.Forms.Button bAnuluj;
        private System.Windows.Forms.CheckBox cBRAK;
        private System.Windows.Forms.CheckBox cForMove;
        private System.Windows.Forms.CheckBox cOK;
        private System.Windows.Forms.CheckBox cCzyPokazywacZaliczki;
    }
}