namespace Pakerator
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.tUser = new System.Windows.Forms.TextBox();
            this.cMagazyn = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.bOK = new System.Windows.Forms.Button();
            this.bAnuluj = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tUser
            // 
            this.tUser.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tUser.Location = new System.Drawing.Point(80, 16);
            this.tUser.Name = "tUser";
            this.tUser.Size = new System.Drawing.Size(103, 20);
            this.tUser.TabIndex = 0;
            // 
            // cMagazyn
            // 
            this.cMagazyn.FormattingEnabled = true;
            this.cMagazyn.Location = new System.Drawing.Point(80, 48);
            this.cMagazyn.Name = "cMagazyn";
            this.cMagazyn.Size = new System.Drawing.Size(253, 21);
            this.cMagazyn.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Użytkownik";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Magazyn";
            // 
            // bOK
            // 
            this.bOK.Location = new System.Drawing.Point(98, 87);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 33);
            this.bOK.TabIndex = 4;
            this.bOK.Text = "&OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // bAnuluj
            // 
            this.bAnuluj.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bAnuluj.Location = new System.Drawing.Point(179, 87);
            this.bAnuluj.Name = "bAnuluj";
            this.bAnuluj.Size = new System.Drawing.Size(75, 33);
            this.bAnuluj.TabIndex = 5;
            this.bAnuluj.Text = "&Anuluj";
            this.bAnuluj.UseVisualStyleBackColor = true;
            this.bAnuluj.Click += new System.EventHandler(this.bAnuluj_Click);
            // 
            // Login
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bAnuluj;
            this.ClientSize = new System.Drawing.Size(345, 132);
            this.Controls.Add(this.bAnuluj);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cMagazyn);
            this.Controls.Add(this.tUser);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Login";
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tUser;
        private System.Windows.Forms.ComboBox cMagazyn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bAnuluj;
    }
}