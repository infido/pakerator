namespace Pakerator
{
    partial class OrdersView
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
            this.label1 = new System.Windows.Forms.Label();
            this.nDniWstecz = new System.Windows.Forms.NumericUpDown();
            this.lkomunikat = new System.Windows.Forms.Label();
            this.bRefresh = new System.Windows.Forms.Button();
            this.bClose = new System.Windows.Forms.Button();
            this.panel2Dol = new System.Windows.Forms.Panel();
            this.dataGridView2Pozycje = new System.Windows.Forms.DataGridView();
            this.panel3Srodek = new System.Windows.Forms.Panel();
            this.dataGridView1Naglowki = new System.Windows.Forms.DataGridView();
            this.panel1Gora.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDniWstecz)).BeginInit();
            this.panel2Dol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2Pozycje)).BeginInit();
            this.panel3Srodek.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1Naglowki)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1Gora
            // 
            this.panel1Gora.Controls.Add(this.label1);
            this.panel1Gora.Controls.Add(this.nDniWstecz);
            this.panel1Gora.Controls.Add(this.lkomunikat);
            this.panel1Gora.Controls.Add(this.bRefresh);
            this.panel1Gora.Controls.Add(this.bClose);
            this.panel1Gora.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1Gora.Location = new System.Drawing.Point(0, 0);
            this.panel1Gora.Name = "panel1Gora";
            this.panel1Gora.Size = new System.Drawing.Size(1268, 53);
            this.panel1Gora.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Ile dni wstecz";
            // 
            // nDniWstecz
            // 
            this.nDniWstecz.Location = new System.Drawing.Point(89, 16);
            this.nDniWstecz.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nDniWstecz.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nDniWstecz.Name = "nDniWstecz";
            this.nDniWstecz.Size = new System.Drawing.Size(48, 20);
            this.nDniWstecz.TabIndex = 3;
            this.nDniWstecz.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lkomunikat
            // 
            this.lkomunikat.AutoSize = true;
            this.lkomunikat.Location = new System.Drawing.Point(259, 17);
            this.lkomunikat.Name = "lkomunikat";
            this.lkomunikat.Size = new System.Drawing.Size(35, 13);
            this.lkomunikat.TabIndex = 2;
            this.lkomunikat.Text = "label1";
            this.lkomunikat.Visible = false;
            // 
            // bRefresh
            // 
            this.bRefresh.Location = new System.Drawing.Point(143, 15);
            this.bRefresh.Name = "bRefresh";
            this.bRefresh.Size = new System.Drawing.Size(75, 23);
            this.bRefresh.TabIndex = 1;
            this.bRefresh.Text = "Odśwież";
            this.bRefresh.UseVisualStyleBackColor = true;
            this.bRefresh.Click += new System.EventHandler(this.bRefresh_Click);
            // 
            // bClose
            // 
            this.bClose.Location = new System.Drawing.Point(1181, 12);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(75, 23);
            this.bClose.TabIndex = 0;
            this.bClose.Text = "Zamknij";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // panel2Dol
            // 
            this.panel2Dol.Controls.Add(this.dataGridView2Pozycje);
            this.panel2Dol.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2Dol.Location = new System.Drawing.Point(0, 354);
            this.panel2Dol.Name = "panel2Dol";
            this.panel2Dol.Size = new System.Drawing.Size(1268, 150);
            this.panel2Dol.TabIndex = 1;
            // 
            // dataGridView2Pozycje
            // 
            this.dataGridView2Pozycje.AllowUserToAddRows = false;
            this.dataGridView2Pozycje.AllowUserToDeleteRows = false;
            this.dataGridView2Pozycje.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2Pozycje.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2Pozycje.Location = new System.Drawing.Point(0, 0);
            this.dataGridView2Pozycje.Name = "dataGridView2Pozycje";
            this.dataGridView2Pozycje.ReadOnly = true;
            this.dataGridView2Pozycje.Size = new System.Drawing.Size(1268, 150);
            this.dataGridView2Pozycje.TabIndex = 0;
            // 
            // panel3Srodek
            // 
            this.panel3Srodek.Controls.Add(this.dataGridView1Naglowki);
            this.panel3Srodek.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3Srodek.Location = new System.Drawing.Point(0, 53);
            this.panel3Srodek.Name = "panel3Srodek";
            this.panel3Srodek.Size = new System.Drawing.Size(1268, 301);
            this.panel3Srodek.TabIndex = 2;
            // 
            // dataGridView1Naglowki
            // 
            this.dataGridView1Naglowki.AllowUserToAddRows = false;
            this.dataGridView1Naglowki.AllowUserToDeleteRows = false;
            this.dataGridView1Naglowki.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1Naglowki.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1Naglowki.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1Naglowki.Name = "dataGridView1Naglowki";
            this.dataGridView1Naglowki.ReadOnly = true;
            this.dataGridView1Naglowki.Size = new System.Drawing.Size(1268, 301);
            this.dataGridView1Naglowki.TabIndex = 0;
            this.dataGridView1Naglowki.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1Naglowki_CellClick);
            // 
            // OrdersView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1268, 504);
            this.Controls.Add(this.panel3Srodek);
            this.Controls.Add(this.panel2Dol);
            this.Controls.Add(this.panel1Gora);
            this.Name = "OrdersView";
            this.ShowIcon = false;
            this.Text = "Zamówienia";
            this.panel1Gora.ResumeLayout(false);
            this.panel1Gora.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nDniWstecz)).EndInit();
            this.panel2Dol.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2Pozycje)).EndInit();
            this.panel3Srodek.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1Naglowki)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1Gora;
        private System.Windows.Forms.Panel panel2Dol;
        private System.Windows.Forms.Panel panel3Srodek;
        private System.Windows.Forms.DataGridView dataGridView1Naglowki;
        private System.Windows.Forms.DataGridView dataGridView2Pozycje;
        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.Button bRefresh;
        private System.Windows.Forms.Label lkomunikat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nDniWstecz;
    }
}