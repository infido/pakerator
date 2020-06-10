using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;

namespace Pakerator
{
    public partial class Login : Form
    {
        ConnectionDB polaczenie;
        Dictionary<int, string> listMagazyny;

        public string userName;
 
        public Login(ConnectionDB polaczenie)
        {
            InitializeComponent();
            this.polaczenie = polaczenie;
        }

        private void bAnuluj_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Program zostanie zamkniety!","UWAGA!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            Application.Exit();
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            userName = tUser.Text;

            if (userName != null && userName.Trim().Length > 0)
            {
                this.Visible = false;
            }
            else
            {
                MessageBox.Show("Nie wypełniono poprawnie danych do logowania!","Bład logowania",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }


    }
}
