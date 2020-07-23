using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;

namespace Pakerator
{
    public partial class Login : Form
    {
        ConnectionDB polaczenie;
        public string userName;
        string logList = "";
        
        public Login(ConnectionDB polaczenie)
        {
            InitializeComponent();
            this.polaczenie = polaczenie;
            getUsersListReg();
        }

        private void bAnuluj_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Program zostanie zamkniety!","UWAGA!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            Application.Exit();
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            userName = cUser.Text.ToUpper();

            if (userName != null && userName.Trim().Length > 0)
            {
                this.Visible = false;
                setUserListReg(userName);
            }
            else
            {
                MessageBox.Show("Nie wypełniono poprawnie danych do logowania!","Bład logowania",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void getUsersListReg()
        {
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            try
            {
                logList = (String)rejestr.GetValue("LoginList");
                if (logList!=null)
                {
                    String[] strlist = logList.Split(',');
                    cUser.Items.AddRange(strlist);
                }
            }
            catch (Exception er)
            {
                MessageBox.Show("Błąd odczytu z rejestru listy podpowiedzi użytkowników. Jeżeli błąd się powtarza skontaktuj się z Administratorem. " + System.Environment.NewLine + er.Message);
                //throw;
            }
        } 

        private void setUserListReg(String strLogin)
        {
            if ( (logList!=null && !logList.Contains(strLogin)) || (logList == null && strLogin.Length>0))
            {
                RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator",true);
                try
                {
                    if (logList == null)
                        logList = strLogin;
                    else
                        logList =  strLogin + "," + logList;

                    rejestr.SetValue("LoginList", logList);
                }
                catch (Exception er)
                {
                    MessageBox.Show("Błąd zapisu do rejestru listy podpowiedzi użytkowników: " + er.Message);
                    //throw;
                }
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
        }

        private void Login_Shown(object sender, EventArgs e)
        {
            cUser.Select();
        }

        private void linkLabelInstalatorGdrive_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://drive.google.com/drive/u/0/folders/187lxq5W6cFT7s2bkd1_A10PziIgzbXzF");
            Application.Exit();
        }
    }
}
