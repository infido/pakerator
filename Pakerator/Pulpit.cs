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
    public partial class Pulpit : Form
    {
        public ConnectionDB polaczenie;
        private FbDataAdapter fda;
        private Login logowanie;
        int dokId = 0;
        
        public Pulpit()
        {
            InitializeComponent();
            Text = "Pakerator " + Application.ProductVersion;
            polaczenie = new ConnectionDB();
            logowanie = new Login(polaczenie);
            logowanie.ShowDialog();
            lKontekstPracyMagazyn.Text = "Praca z dokumentami w: " + logowanie.magNazwa + "   Użytkownik:" + logowanie.userName;
        }

        private void Pulpit_Load(object sender, EventArgs e)
        {
            
        }

        private void Pulpit_FormClosed(object sender, FormClosedEventArgs e)
        {
            polaczenie.Close();
        }

        private void konfiguracjaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polaczenie.Show();
        }

        private void SetDokument(string kodKreskowy)
        {
            string sql = "select ID, NUMER, NAZWA_PELNA_PLATNIKA, NAZWA_PELNA_ODBIORCY, ";
            sql += "COALESCE(ULICA_ODBIORCY,'') as ULICA_ODBIORCY, COALESCE(NRDOMU_ODBIORCY,'') as NRDOMU_ODBIORCY, COALESCE(NRLOKALU_ODBIORCY,'') as NRLOKALU_ODBIORCY, COALESCE(MIEJSCOWOSC_ODBIORCY,'') as MIEJSCOWOSC_ODBIORCY, ";
            sql += "COALESCE(PANSTWO_ODBIORCY,'') as PANSTWO_ODBIORCY, COALESCE(KOD_ODBIORCY,'') as KOD_ODBIORCY, OPERATOR, SYGNATURA, COALESCE(UWAGI,'') as UWAGI ";
            sql += "from GM_FS ";
            sql += "where MAGNUM=" + logowanie.magID + " and SYGNATURA='" + tToSkan.Text + "';";
            FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
            try
            {
                FbDataReader fdk = cdk.ExecuteReader();
                if (fdk.Read())
                {
                    dokId = (int)fdk["ID"];
                    lDokument.Text = (string)fdk["NUMER"];
                    lNabywcaTresc.Text = (string)fdk["NAZWA_PELNA_PLATNIKA"];
                    lOdbiorcaTresc.Text = (string)fdk["NAZWA_PELNA_ODBIORCY"] + Environment.NewLine + (string)fdk["PANSTWO_ODBIORCY"] + "; " + (string)fdk["KOD_ODBIORCY"] + " " + (string)fdk["MIEJSCOWOSC_ODBIORCY"] + Environment.NewLine;
                    lOdbiorcaTresc.Text += (string)fdk["ULICA_ODBIORCY"] + " " + (string)fdk["NRDOMU_ODBIORCY"] + " " + (string)fdk["NRLOKALU_ODBIORCY"];
                    setLog("LOG",lOdbiorcaTresc.Text, kodKreskowy, kodKreskowy, lDokument.Text);

                    //Tu wczytujemy pozycje dokumentu

                    tToSkan.Text = "";
                }
            }
            catch (FbException ex)
            {
                setLog("ERR","Błąd zapytania SQL001: " + ex.Message, kodKreskowy, kodKreskowy, "");
            }
        }

        private void tToSkan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Return))
            {
                if (dokId == 0)
                {
                    //wybór dokumentu
                    SetDokument(tToSkan.Text);
                }
                else
                {
                    //skanowanie towaru
                }
            }
        }

        private void setLog(string typ, string tresc, string kodKreskowy, string listPrzewozowy, string nrDokumentu)
        {
            //dodanie obsługi logowania w bazie
            if (!typ.Equals("LOG"))
            {
                //logów nie wyświetlaj
                MessageBox.Show(typ + " KodKr:" + kodKreskowy + " TRESC:" + tresc + " Magazyn:" + logowanie.magID);
            }
            Console.WriteLine(typ + " KodKr:" + kodKreskowy + " TRESC:" + tresc + " Magazyn:" + logowanie.magID);
            
        }
    }
}
