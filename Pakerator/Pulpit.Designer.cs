namespace Pakerator
{
    partial class Pulpit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Pulpit));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.konfiguracjaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lKontekstPracyMagazyn = new System.Windows.Forms.Label();
            this.tToSkan = new System.Windows.Forms.TextBox();
            this.lCoSkanowac = new System.Windows.Forms.Label();
            this.lDokument = new System.Windows.Forms.Label();
            this.lOdbiorcaLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lnabywcaLabel = new System.Windows.Forms.Label();
            this.lOdbiorcaTresc = new System.Windows.Forms.Label();
            this.lNabywcaTresc = new System.Windows.Forms.Label();
            this.wyczyśćToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lListPrzewozowy = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelHistoria = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.textHistoria = new System.Windows.Forms.TextBox();
            this.dataGridViewPozycje = new System.Windows.Forms.DataGridView();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelHistoria.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPozycje)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wyczyśćToolStripMenuItem,
            this.konfiguracjaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(829, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // konfiguracjaToolStripMenuItem
            // 
            this.konfiguracjaToolStripMenuItem.Name = "konfiguracjaToolStripMenuItem";
            this.konfiguracjaToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.konfiguracjaToolStripMenuItem.Text = "&Konfiguracja";
            this.konfiguracjaToolStripMenuItem.Click += new System.EventHandler(this.konfiguracjaToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lListPrzewozowy);
            this.panel1.Controls.Add(this.lNabywcaTresc);
            this.panel1.Controls.Add(this.lOdbiorcaTresc);
            this.panel1.Controls.Add(this.lnabywcaLabel);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lOdbiorcaLabel);
            this.panel1.Controls.Add(this.lDokument);
            this.panel1.Controls.Add(this.lCoSkanowac);
            this.panel1.Controls.Add(this.tToSkan);
            this.panel1.Controls.Add(this.lKontekstPracyMagazyn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(829, 100);
            this.panel1.TabIndex = 1;
            // 
            // lKontekstPracyMagazyn
            // 
            this.lKontekstPracyMagazyn.AutoSize = true;
            this.lKontekstPracyMagazyn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lKontekstPracyMagazyn.Location = new System.Drawing.Point(12, 10);
            this.lKontekstPracyMagazyn.Name = "lKontekstPracyMagazyn";
            this.lKontekstPracyMagazyn.Size = new System.Drawing.Size(762, 20);
            this.lKontekstPracyMagazyn.TabIndex = 0;
            this.lKontekstPracyMagazyn.Text = "Magazyn: 1234567890 1234567890 1234567890 1234567890 1234567890 TO jest przykład " +
    "długiej nazwy";
            // 
            // tToSkan
            // 
            this.tToSkan.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tToSkan.Location = new System.Drawing.Point(16, 63);
            this.tToSkan.Name = "tToSkan";
            this.tToSkan.Size = new System.Drawing.Size(164, 22);
            this.tToSkan.TabIndex = 1;
            this.tToSkan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tToSkan_KeyPress);
            // 
            // lCoSkanowac
            // 
            this.lCoSkanowac.AutoSize = true;
            this.lCoSkanowac.Location = new System.Drawing.Point(16, 44);
            this.lCoSkanowac.Name = "lCoSkanowac";
            this.lCoSkanowac.Size = new System.Drawing.Size(144, 13);
            this.lCoSkanowac.TabIndex = 2;
            this.lCoSkanowac.Text = "Skanuj listprzewozowy/towar";
            // 
            // lDokument
            // 
            this.lDokument.AutoSize = true;
            this.lDokument.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lDokument.Location = new System.Drawing.Point(197, 49);
            this.lDokument.Name = "lDokument";
            this.lDokument.Size = new System.Drawing.Size(83, 20);
            this.lDokument.TabIndex = 3;
            this.lDokument.Text = "Dokument";
            // 
            // lOdbiorcaLabel
            // 
            this.lOdbiorcaLabel.AutoSize = true;
            this.lOdbiorcaLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lOdbiorcaLabel.Location = new System.Drawing.Point(381, 49);
            this.lOdbiorcaLabel.Name = "lOdbiorcaLabel";
            this.lOdbiorcaLabel.Size = new System.Drawing.Size(62, 13);
            this.lOdbiorcaLabel.TabIndex = 4;
            this.lOdbiorcaLabel.Text = "Odbiorca:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(197, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Nr dokumentu i listu:";
            // 
            // lnabywcaLabel
            // 
            this.lnabywcaLabel.AutoSize = true;
            this.lnabywcaLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lnabywcaLabel.Location = new System.Drawing.Point(381, 33);
            this.lnabywcaLabel.Name = "lnabywcaLabel";
            this.lnabywcaLabel.Size = new System.Drawing.Size(112, 13);
            this.lnabywcaLabel.TabIndex = 6;
            this.lnabywcaLabel.Text = "Nabywca(Płatnik):";
            // 
            // lOdbiorcaTresc
            // 
            this.lOdbiorcaTresc.AutoSize = true;
            this.lOdbiorcaTresc.Location = new System.Drawing.Point(499, 49);
            this.lOdbiorcaTresc.Name = "lOdbiorcaTresc";
            this.lOdbiorcaTresc.Size = new System.Drawing.Size(16, 13);
            this.lOdbiorcaTresc.TabIndex = 7;
            this.lOdbiorcaTresc.Text = "...";
            // 
            // lNabywcaTresc
            // 
            this.lNabywcaTresc.AutoSize = true;
            this.lNabywcaTresc.Location = new System.Drawing.Point(499, 33);
            this.lNabywcaTresc.Name = "lNabywcaTresc";
            this.lNabywcaTresc.Size = new System.Drawing.Size(16, 13);
            this.lNabywcaTresc.TabIndex = 8;
            this.lNabywcaTresc.Text = "...";
            // 
            // wyczyśćToolStripMenuItem
            // 
            this.wyczyśćToolStripMenuItem.Name = "wyczyśćToolStripMenuItem";
            this.wyczyśćToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.wyczyśćToolStripMenuItem.Text = "&Wyczyść";
            this.wyczyśćToolStripMenuItem.Click += new System.EventHandler(this.wyczyśćToolStripMenuItem_Click);
            // 
            // lListPrzewozowy
            // 
            this.lListPrzewozowy.AutoSize = true;
            this.lListPrzewozowy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lListPrzewozowy.Location = new System.Drawing.Point(197, 71);
            this.lListPrzewozowy.Name = "lListPrzewozowy";
            this.lListPrzewozowy.Size = new System.Drawing.Size(125, 20);
            this.lListPrzewozowy.TabIndex = 9;
            this.lListPrzewozowy.Text = "List Przewozowy";
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.dataGridViewPozycje);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 124);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(829, 399);
            this.panel2.TabIndex = 2;
            // 
            // panelHistoria
            // 
            this.panelHistoria.Controls.Add(this.panel4);
            this.panelHistoria.Controls.Add(this.panel3);
            this.panelHistoria.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelHistoria.Location = new System.Drawing.Point(0, 439);
            this.panelHistoria.Name = "panelHistoria";
            this.panelHistoria.Size = new System.Drawing.Size(829, 84);
            this.panelHistoria.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(829, 25);
            this.panel3.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Historia:";
            // 
            // panel4
            // 
            this.panel4.AutoScroll = true;
            this.panel4.Controls.Add(this.textHistoria);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 25);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(829, 59);
            this.panel4.TabIndex = 1;
            // 
            // textHistoria
            // 
            this.textHistoria.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textHistoria.Location = new System.Drawing.Point(0, 0);
            this.textHistoria.Multiline = true;
            this.textHistoria.Name = "textHistoria";
            this.textHistoria.ReadOnly = true;
            this.textHistoria.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textHistoria.Size = new System.Drawing.Size(829, 59);
            this.textHistoria.TabIndex = 0;
            // 
            // dataGridViewPozycje
            // 
            this.dataGridViewPozycje.AllowUserToAddRows = false;
            this.dataGridViewPozycje.AllowUserToDeleteRows = false;
            this.dataGridViewPozycje.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewPozycje.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPozycje.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPozycje.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewPozycje.Name = "dataGridViewPozycje";
            this.dataGridViewPozycje.ReadOnly = true;
            this.dataGridViewPozycje.RowHeadersVisible = false;
            this.dataGridViewPozycje.Size = new System.Drawing.Size(829, 399);
            this.dataGridViewPozycje.TabIndex = 0;
            // 
            // Pulpit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 523);
            this.Controls.Add(this.panelHistoria);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Pulpit";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Pulpit_FormClosed);
            this.Load += new System.EventHandler(this.Pulpit_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panelHistoria.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPozycje)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem konfiguracjaToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lKontekstPracyMagazyn;
        private System.Windows.Forms.Label lCoSkanowac;
        private System.Windows.Forms.TextBox tToSkan;
        private System.Windows.Forms.Label lDokument;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lOdbiorcaLabel;
        private System.Windows.Forms.Label lnabywcaLabel;
        private System.Windows.Forms.Label lNabywcaTresc;
        private System.Windows.Forms.Label lOdbiorcaTresc;
        private System.Windows.Forms.ToolStripMenuItem wyczyśćToolStripMenuItem;
        private System.Windows.Forms.Label lListPrzewozowy;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panelHistoria;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox textHistoria;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridViewPozycje;
    }
}

