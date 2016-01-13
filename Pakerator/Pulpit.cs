using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using System.Media;

namespace Pakerator
{
    public partial class Pulpit : Form
    {
        public ConnectionDB polaczenie;
        private FbDataAdapter fda;
        private DataSet fds;
        private DataView fDataView;
        private Login logowanie;
        int dokId = 0;
        private bool jestSkonczone = false;
        
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
                    lListPrzewozowy.Text = kodKreskowy;
                    lNabywcaTresc.Text = (string)fdk["NAZWA_PELNA_PLATNIKA"];
                    lOdbiorcaTresc.Text = (string)fdk["NAZWA_PELNA_ODBIORCY"] + Environment.NewLine + (string)fdk["PANSTWO_ODBIORCY"] + "; " + (string)fdk["KOD_ODBIORCY"] + " " + (string)fdk["MIEJSCOWOSC_ODBIORCY"] + Environment.NewLine;
                    lOdbiorcaTresc.Text += (string)fdk["ULICA_ODBIORCY"] + " " + (string)fdk["NRDOMU_ODBIORCY"] + " " + (string)fdk["NRLOKALU_ODBIORCY"];
                    setLog("LOG", lOdbiorcaTresc.Text, kodKreskowy, kodKreskowy, lDokument.Text);

                    //Tu wczytujemy pozycje dokumentu

                    sql = "select GM_FSPOZ.ID, GM_FSPOZ.LP, GM_TOWARY.TYP, GM_TOWARY.SKROT, COALESCE(GM_TOWARY.SKROT2,'') as SKROT2, COALESCE(GM_TOWARY.KOD_KRESKOWY,'') as KOD_KRESKOWY, GM_TOWARY.NAZWA, GM_FSPOZ.ILOSC, 0 as SKANOWANE, COALESCE(GM_FSPOZ.ZNACZNIKI,'') as ZNACZNIKI ";
                    sql += "from GM_FSPOZ ";
                    sql += "join GM_TOWARY ON GM_FSPOZ.ID_TOWARU=GM_TOWARY.ID ";
                    sql += "where GM_FSPOZ.ID_GLOWKI=" + dokId;

                    fda = new FbDataAdapter(sql, polaczenie.getConnection().ConnectionString);
                    fds = new DataSet();
                    fDataView = new DataView();
                    fds.Tables.Add("POZ");
                    try
                    {
                        fda.Fill(fds.Tables["POZ"]);
                    }
                    catch (Exception ex)
                    {
                        setLog("ERROR", "Nieudane zapytanie o pozycjie dokumentu dla listy: " + ex.Message, kodKreskowy, kodKreskowy, lDokument.Text);
                        throw;
                    }
                    fDataView.Table = fds.Tables["POZ"];
                    dataGridViewPozycje.DataSource = fDataView;

                    dataGridViewPozycje.Columns["ID"].Visible = false;
                    kolorowanieRekordow();

                    cdk = new FbCommand("UPDATE GM_FS SET ZNACZNIKI='Pakuje:" + logowanie.userName + " " + DateTime.Now.ToShortDateString() + " " +
                    DateTime.Now.ToShortTimeString() + "; ' || ZNACZNIKI where ID=" + dokId, polaczenie.getConnection());
                    try
                    {
                        cdk.ExecuteNonQuery();
                    }
                    catch (FbException ex)
                    {
                        setLog("ERROR", "Bład zapytania: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text);
                        throw;
                    }
                    
                }
                else
                {
                    setLog("WARNING", "Nie znaleziono dokumentu dla listu przewozowego", kodKreskowy, kodKreskowy, "");
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
                    textHistoria.Text = "List przewozowy: " + tToSkan.Text + Environment.NewLine + textHistoria.Text;
                    SetDokument(tToSkan.Text);
                    tToSkan.Text = "";
                }
                else
                {
                    //skanowanie towaru
                    textHistoria.Text = "Towar: " + tToSkan.Text + Environment.NewLine + textHistoria.Text;
                    foreach (DataGridViewRow row in dataGridViewPozycje.Rows)
                    {
                        if (row.Cells["KOD_KRESKOWY"].Value.Equals(tToSkan.Text))
                        {
                            //zapis do bazy
                            row.Cells["SKANOWANE"].Value = (int)row.Cells["SKANOWANE"].Value + 1;

                            //updatsw
                            FbCommand cdk = new FbCommand("UPDATE GM_FSPOZ SET ZNACZNIKI=" + row.Cells["SKANOWANE"].Value + " where ID=" + row.Cells["ID"].Value, polaczenie.getConnection());
                            try
                            {
                                cdk.ExecuteNonQuery();
                            }
                            catch (FbException ex)
                            {
                                setLog("ERROR","Bład zapytania: " + ex.Message, tToSkan.Text,lListPrzewozowy.Text,lDokument.Text);
                                throw;
                            }
                            break;
                        }
                    }
                    dataGridViewPozycje.Refresh();
                    tToSkan.Text = "";
                    kolorowanieRekordow();
                    sprawdzenieCzySkonczone();
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

        private void wyczyśćToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dokId = 0;
            lDokument.Text = "";
            lNabywcaTresc.Text = "";
            lOdbiorcaTresc.Text = "";
            lListPrzewozowy.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //System.Media.SystemSounds.Hand.Play();
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = Application.StartupPath + "\\fanfare_x.wav";
            player.Load();
            player.Play();

        }

        private void kolorowanieRekordow()
        {
            foreach (DataGridViewRow row in dataGridViewPozycje.Rows)
            {
                if (!row.Cells["TYP"].Value.Equals("Towar"))
                {
                    row.DefaultCellStyle.BackColor = Color.Gray;
                }
                else if (row.Cells["SKANOWANE"].Value.ToString().Equals(row.Cells["ILOSC"].Value.ToString()))
                {
                    row.DefaultCellStyle.BackColor = Color.Green;
                }
                else if (Int32.Parse(row.Cells["SKANOWANE"].Value.ToString()) != 0 &&
                    Int32.Parse(row.Cells["SKANOWANE"].Value.ToString()) < Int32.Parse(row.Cells["ILOSC"].Value.ToString()))
                {
                    row.DefaultCellStyle.BackColor = Color.Azure;
                }
                else if (Int32.Parse(row.Cells["SKANOWANE"].Value.ToString()) > Int32.Parse(row.Cells["ILOSC"].Value.ToString()))
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }

        private void sprawdzenieCzySkonczone()
        {
            jestSkonczone = true;
            foreach (DataGridViewRow row in dataGridViewPozycje.Rows)
            {
                if (row.Cells["TYP"].Value.ToString().Equals("Towar") && jestSkonczone &&
                    Int32.Parse(row.Cells["SKANOWANE"].Value.ToString()) < Int32.Parse(row.Cells["ILOSC"].Value.ToString()))
                {
                    jestSkonczone = false;
                }
               
            }

            if (jestSkonczone)
            {
                FbCommand cdk = new FbCommand("UPDATE GM_FS SET ZNACZNIKI='Zapakował:" + DateTime.Now.ToShortDateString() + " " +
                    DateTime.Now.ToShortTimeString() + "; ' || ZNACZNIKI where ID=" + dokId, polaczenie.getConnection());
                try
                {
                    cdk.ExecuteNonQuery();
                }
                catch (FbException ex)
                {
                    setLog("ERROR", "Bład zapytania: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text);
                    throw;
                }

                SoundPlayer player = new SoundPlayer();
                player.SoundLocation = Application.StartupPath + "\\fanfare_x.wav";
                player.Load();
                player.Play();

                MessageBox.Show("Dokument jest skończony!", "Potwierdzenie", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }
    }
}
