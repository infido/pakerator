namespace Pakerator
{
    partial class RaportKontrolaIndeksow
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
            this.bClose = new System.Windows.Forms.Button();
            this.bFind = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tKodDoZnalezienia = new System.Windows.Forms.TextBox();
            this.panelView = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1Gora.SuspendLayout();
            this.panelView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1Gora
            // 
            this.panel1Gora.Controls.Add(this.bClose);
            this.panel1Gora.Controls.Add(this.bFind);
            this.panel1Gora.Controls.Add(this.label1);
            this.panel1Gora.Controls.Add(this.tKodDoZnalezienia);
            this.panel1Gora.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1Gora.Location = new System.Drawing.Point(0, 0);
            this.panel1Gora.Name = "panel1Gora";
            this.panel1Gora.Size = new System.Drawing.Size(800, 46);
            this.panel1Gora.TabIndex = 0;
            // 
            // bClose
            // 
            this.bClose.Location = new System.Drawing.Point(713, 11);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(75, 23);
            this.bClose.TabIndex = 3;
            this.bClose.Text = "Zamknij";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // bFind
            // 
            this.bFind.Location = new System.Drawing.Point(286, 14);
            this.bFind.Name = "bFind";
            this.bFind.Size = new System.Drawing.Size(75, 23);
            this.bFind.TabIndex = 2;
            this.bFind.Text = "Szukaj";
            this.bFind.UseVisualStyleBackColor = true;
            this.bFind.Click += new System.EventHandler(this.bFind_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Kod towaru do sprawdzenia";
            // 
            // tKodDoZnalezienia
            // 
            this.tKodDoZnalezienia.Location = new System.Drawing.Point(157, 16);
            this.tKodDoZnalezienia.Name = "tKodDoZnalezienia";
            this.tKodDoZnalezienia.Size = new System.Drawing.Size(123, 20);
            this.tKodDoZnalezienia.TabIndex = 0;
            this.tKodDoZnalezienia.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tKodDoZnalezienia_KeyPress);
            // 
            // panelView
            // 
            this.panelView.Controls.Add(this.dataGridView1);
            this.panelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelView.Location = new System.Drawing.Point(0, 46);
            this.panelView.Name = "panelView";
            this.panelView.Size = new System.Drawing.Size(800, 404);
            this.panelView.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(800, 404);
            this.dataGridView1.TabIndex = 0;
            // 
            // RaportKontrolaIndeksow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelView);
            this.Controls.Add(this.panel1Gora);
            this.Name = "RaportKontrolaIndeksow";
            this.ShowIcon = false;
            this.Text = "Raport kontrola indeksów w dokumentach sprzedaży";
            this.panel1Gora.ResumeLayout(false);
            this.panel1Gora.PerformLayout();
            this.panelView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1Gora;
        private System.Windows.Forms.Panel panelView;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button bClose;
        private System.Windows.Forms.Button bFind;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tKodDoZnalezienia;
    }
}