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
            this.label1 = new System.Windows.Forms.Label();
            this.bOK = new System.Windows.Forms.Button();
            this.bAnuluj = new System.Windows.Forms.Button();
            this.cUser = new System.Windows.Forms.ComboBox();
            this.linkLabelInstalatorGdrive = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
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
            // bOK
            // 
            this.bOK.Location = new System.Drawing.Point(27, 51);
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
            this.bAnuluj.Location = new System.Drawing.Point(108, 51);
            this.bAnuluj.Name = "bAnuluj";
            this.bAnuluj.Size = new System.Drawing.Size(75, 33);
            this.bAnuluj.TabIndex = 5;
            this.bAnuluj.Text = "&Anuluj";
            this.bAnuluj.UseVisualStyleBackColor = true;
            this.bAnuluj.Click += new System.EventHandler(this.bAnuluj_Click);
            // 
            // cUser
            // 
            this.cUser.FormattingEnabled = true;
            this.cUser.Location = new System.Drawing.Point(80, 16);
            this.cUser.Name = "cUser";
            this.cUser.Size = new System.Drawing.Size(102, 21);
            this.cUser.TabIndex = 6;
            // 
            // linkLabelInstalatorGdrive
            // 
            this.linkLabelInstalatorGdrive.AutoSize = true;
            this.linkLabelInstalatorGdrive.Location = new System.Drawing.Point(46, 87);
            this.linkLabelInstalatorGdrive.Name = "linkLabelInstalatorGdrive";
            this.linkLabelInstalatorGdrive.Size = new System.Drawing.Size(104, 13);
            this.linkLabelInstalatorGdrive.TabIndex = 8;
            this.linkLabelInstalatorGdrive.TabStop = true;
            this.linkLabelInstalatorGdrive.Text = "Instalator pakeratora";
            this.linkLabelInstalatorGdrive.Click += new System.EventHandler(this.linkLabelInstalatorGdrive_Click);
            // 
            // Login
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bAnuluj;
            this.ClientSize = new System.Drawing.Size(204, 107);
            this.Controls.Add(this.linkLabelInstalatorGdrive);
            this.Controls.Add(this.cUser);
            this.Controls.Add(this.bAnuluj);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Login";
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Login_Load);
            this.Shown += new System.EventHandler(this.Login_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bAnuluj;
        private System.Windows.Forms.ComboBox cUser;
        private System.Windows.Forms.LinkLabel linkLabelInstalatorGdrive;
    }
}