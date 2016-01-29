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
        public int magID;
        public string magNazwa;
        public string userName;
 
        public Login(ConnectionDB polaczenie)
        {
            InitializeComponent();
            this.polaczenie = polaczenie;
            setDictonary();
        }

        private void bAnuluj_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Program zostanie zamkniety!","UWAGA!",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            Application.Exit();
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            userName = tUser.Text;
            magID = ((KeyValuePair<int, string>)cMagazyn.SelectedItem).Key;
            magNazwa = ((KeyValuePair<int, string>)cMagazyn.SelectedItem).Value;

            if (userName != null && userName.Trim().Length > 0 && magID != 0)
            {
                this.Visible = false;
            }
            else
            {
                MessageBox.Show("Nie wypełniono poprawnie danych do logowania!","Bład logowania",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void setDictonary()
        {
            listMagazyny = new Dictionary<int, string>();
            FbCommand cdk = new FbCommand("SELECT ID, NUMER || ' ' || NAZWA AS NUMER FROM GM_MAGAZYNY WHERE ARCHIWALNY=0", polaczenie.getConnection());
            try
            {
                FbDataReader fdk = cdk.ExecuteReader();
                while (fdk.Read())
                {
                    listMagazyny.Add((int)fdk["ID"], (string)fdk["NUMER"]);
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("Błąd wczytywania listy magazynów: " + ex.Message);
            }

            cMagazyn.DataSource = new BindingSource(listMagazyny, null);
            cMagazyn.DisplayMember = "Value";
            cMagazyn.ValueMember = "Key";
        }

        public string getIdMagazynAsString()
        {
            return magID.ToString();
        }

    }
}
