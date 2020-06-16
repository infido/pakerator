﻿namespace Pakerator
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
            this.panel2Dol = new System.Windows.Forms.Panel();
            this.panel3Srodek = new System.Windows.Forms.Panel();
            this.dataGridView1Naglowki = new System.Windows.Forms.DataGridView();
            this.dataGridView2Pozycje = new System.Windows.Forms.DataGridView();
            this.bClose = new System.Windows.Forms.Button();
            this.bRefresh = new System.Windows.Forms.Button();
            this.lkomunikat = new System.Windows.Forms.Label();
            this.panel1Gora.SuspendLayout();
            this.panel2Dol.SuspendLayout();
            this.panel3Srodek.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1Naglowki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2Pozycje)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1Gora
            // 
            this.panel1Gora.Controls.Add(this.lkomunikat);
            this.panel1Gora.Controls.Add(this.bRefresh);
            this.panel1Gora.Controls.Add(this.bClose);
            this.panel1Gora.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1Gora.Location = new System.Drawing.Point(0, 0);
            this.panel1Gora.Name = "panel1Gora";
            this.panel1Gora.Size = new System.Drawing.Size(800, 53);
            this.panel1Gora.TabIndex = 0;
            // 
            // panel2Dol
            // 
            this.panel2Dol.Controls.Add(this.dataGridView2Pozycje);
            this.panel2Dol.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2Dol.Location = new System.Drawing.Point(0, 354);
            this.panel2Dol.Name = "panel2Dol";
            this.panel2Dol.Size = new System.Drawing.Size(800, 150);
            this.panel2Dol.TabIndex = 1;
            // 
            // panel3Srodek
            // 
            this.panel3Srodek.Controls.Add(this.dataGridView1Naglowki);
            this.panel3Srodek.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3Srodek.Location = new System.Drawing.Point(0, 53);
            this.panel3Srodek.Name = "panel3Srodek";
            this.panel3Srodek.Size = new System.Drawing.Size(800, 301);
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
            this.dataGridView1Naglowki.Size = new System.Drawing.Size(800, 301);
            this.dataGridView1Naglowki.TabIndex = 0;
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
            this.dataGridView2Pozycje.Size = new System.Drawing.Size(800, 150);
            this.dataGridView2Pozycje.TabIndex = 0;
            // 
            // bClose
            // 
            this.bClose.Location = new System.Drawing.Point(714, 13);
            this.bClose.Name = "bClose";
            this.bClose.Size = new System.Drawing.Size(75, 23);
            this.bClose.TabIndex = 0;
            this.bClose.Text = "Zamknij";
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Click += new System.EventHandler(this.bClose_Click);
            // 
            // bRefresh
            // 
            this.bRefresh.Location = new System.Drawing.Point(13, 13);
            this.bRefresh.Name = "bRefresh";
            this.bRefresh.Size = new System.Drawing.Size(75, 23);
            this.bRefresh.TabIndex = 1;
            this.bRefresh.Text = "Odśwież";
            this.bRefresh.UseVisualStyleBackColor = true;
            this.bRefresh.Click += new System.EventHandler(this.bRefresh_Click);
            // 
            // lkomunikat
            // 
            this.lkomunikat.AutoSize = true;
            this.lkomunikat.Location = new System.Drawing.Point(115, 18);
            this.lkomunikat.Name = "lkomunikat";
            this.lkomunikat.Size = new System.Drawing.Size(35, 13);
            this.lkomunikat.TabIndex = 2;
            this.lkomunikat.Text = "label1";
            this.lkomunikat.Visible = false;
            // 
            // OrdersView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 504);
            this.Controls.Add(this.panel3Srodek);
            this.Controls.Add(this.panel2Dol);
            this.Controls.Add(this.panel1Gora);
            this.Name = "OrdersView";
            this.ShowIcon = false;
            this.Text = "Zamówienia";
            this.panel1Gora.ResumeLayout(false);
            this.panel1Gora.PerformLayout();
            this.panel2Dol.ResumeLayout(false);
            this.panel3Srodek.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1Naglowki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2Pozycje)).EndInit();
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
    }
}