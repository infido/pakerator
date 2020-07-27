﻿namespace Pakerator
{
    partial class Autentykacja
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Autentykacja));
            this.tLogin = new System.Windows.Forms.TextBox();
            this.tPass = new System.Windows.Forms.TextBox();
            this.tPassToConfirmation = new System.Windows.Forms.TextBox();
            this.bLogin = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lPassToConfirmation = new System.Windows.Forms.Label();
            this.lPassWrong = new System.Windows.Forms.Label();
            this.linkLabelInstalatorGdrive = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // tLogin
            // 
            this.tLogin.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tLogin.Location = new System.Drawing.Point(115, 12);
            this.tLogin.MaxLength = 8;
            this.tLogin.Name = "tLogin";
            this.tLogin.Size = new System.Drawing.Size(100, 20);
            this.tLogin.TabIndex = 0;
            // 
            // tPass
            // 
            this.tPass.Location = new System.Drawing.Point(115, 39);
            this.tPass.MaxLength = 20;
            this.tPass.Name = "tPass";
            this.tPass.PasswordChar = '*';
            this.tPass.Size = new System.Drawing.Size(100, 20);
            this.tPass.TabIndex = 1;
            this.tPass.TextChanged += new System.EventHandler(this.tPass_TextChanged);
            this.tPass.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tPass_KeyUp);
            // 
            // tPassToConfirmation
            // 
            this.tPassToConfirmation.Location = new System.Drawing.Point(115, 66);
            this.tPassToConfirmation.MaxLength = 20;
            this.tPassToConfirmation.Name = "tPassToConfirmation";
            this.tPassToConfirmation.PasswordChar = '*';
            this.tPassToConfirmation.Size = new System.Drawing.Size(100, 20);
            this.tPassToConfirmation.TabIndex = 2;
            this.tPassToConfirmation.Visible = false;
            this.tPassToConfirmation.TextChanged += new System.EventHandler(this.tPassToConfirmation_TextChanged);
            // 
            // bLogin
            // 
            this.bLogin.Location = new System.Drawing.Point(140, 104);
            this.bLogin.Name = "bLogin";
            this.bLogin.Size = new System.Drawing.Size(75, 23);
            this.bLogin.TabIndex = 3;
            this.bLogin.Text = "&OK";
            this.bLogin.UseVisualStyleBackColor = true;
            this.bLogin.Click += new System.EventHandler(this.bLogin_Click);
            // 
            // bCancel
            // 
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(29, 104);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 4;
            this.bCancel.Text = "&Anuluj";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Użytkownik";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Hasło";
            // 
            // lPassToConfirmation
            // 
            this.lPassToConfirmation.AutoSize = true;
            this.lPassToConfirmation.Location = new System.Drawing.Point(26, 69);
            this.lPassToConfirmation.Name = "lPassToConfirmation";
            this.lPassToConfirmation.Size = new System.Drawing.Size(83, 13);
            this.lPassToConfirmation.TabIndex = 7;
            this.lPassToConfirmation.Text = "Potwierdź hasło";
            this.lPassToConfirmation.Visible = false;
            // 
            // lPassWrong
            // 
            this.lPassWrong.AutoSize = true;
            this.lPassWrong.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lPassWrong.ForeColor = System.Drawing.Color.Red;
            this.lPassWrong.Location = new System.Drawing.Point(112, 89);
            this.lPassWrong.Name = "lPassWrong";
            this.lPassWrong.Size = new System.Drawing.Size(107, 13);
            this.lPassWrong.TabIndex = 8;
            this.lPassWrong.Text = "Hasło niezgodne!";
            this.lPassWrong.Visible = false;
            // 
            // linkLabelInstalatorGdrive
            // 
            this.linkLabelInstalatorGdrive.AutoSize = true;
            this.linkLabelInstalatorGdrive.Location = new System.Drawing.Point(68, 133);
            this.linkLabelInstalatorGdrive.Name = "linkLabelInstalatorGdrive";
            this.linkLabelInstalatorGdrive.Size = new System.Drawing.Size(104, 13);
            this.linkLabelInstalatorGdrive.TabIndex = 9;
            this.linkLabelInstalatorGdrive.TabStop = true;
            this.linkLabelInstalatorGdrive.Text = "Instalator pakeratora";
            // 
            // Autentykacja
            // 
            this.AcceptButton = this.bLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(246, 155);
            this.Controls.Add(this.linkLabelInstalatorGdrive);
            this.Controls.Add(this.lPassWrong);
            this.Controls.Add(this.lPassToConfirmation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bLogin);
            this.Controls.Add(this.tPassToConfirmation);
            this.Controls.Add(this.tPass);
            this.Controls.Add(this.tLogin);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Autentykacja";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Autentykacja";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tLogin;
        private System.Windows.Forms.TextBox tPass;
        private System.Windows.Forms.TextBox tPassToConfirmation;
        private System.Windows.Forms.Button bLogin;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lPassToConfirmation;
        private System.Windows.Forms.Label lPassWrong;
        private System.Windows.Forms.LinkLabel linkLabelInstalatorGdrive;
    }
}