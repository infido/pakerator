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
                if (logList.Length != 0)
                {
                    String[] strlist = logList.Split(',');
                    cUser.Items.AddRange(strlist);
                }
            }
            catch (Exception er)
            {
                MessageBox.Show("Błąd odczytu z rejestru listy podpowiedzi użytkowników: " + er.Message);
                throw;
            }
        } 

        private void setUserListReg(String strLogin)
        {
            if (!logList.Contains(strLogin))
            {
                RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator",true);
                try
                {
                    if (logList.Length == 0)
                        logList = strLogin;
                    else
                        logList =  strLogin + "," + logList;

                    rejestr.SetValue("LoginList", logList);
                }
                catch (Exception er)
                {
                    MessageBox.Show("Błąd zapisu do rejestru listy podpowiedzi użytkowników: " + er.Message);
                    throw;
                }
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            cUser.Focus();
            cUser.Select();
        }
    }
}
