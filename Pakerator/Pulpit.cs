﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using System.Media;
using System.IO;

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
            chkTableLOGSKAN();
        }

        private void chkTableLOGSKAN()
        {
            string sql = "select 1 from rdb$relations where rdb$relation_name = 'LOGSKAN';";
                        FbCommand cdk = new FbCommand(sql , polaczenie.getConnection());
            try
            {
                if (cdk.ExecuteScalar() == null)
                {
                    //nie ma tabeli to dodajemy
                    cdk = new FbCommand("CREATE SEQUENCE LOGSKAN_ID;", polaczenie.getConnection());
                    cdk.ExecuteScalar();

                    sql = " create table LOGSKAN (";
                    sql += " id int primary key, ";
                    sql += " pracownik varchar(30), ";
                    sql += " kodkreskowy varchar(100), ";
                    sql += " dokument_nr varchar(30), ";
                    sql += " dokument_id int, ";
                    sql += " towar_id int, ";
                    sql += " komunikat varchar(800), ";
                    sql += " operacja varchar(30), ";
                    sql += " magazyn_nazwa varchar(150), ";
                    sql += " magazyn_id int, ";
                    sql += " kontrahent varchar(255), ";
                    sql += " odbiorca int, ";
                    sql += " platnik int, ";
                    sql += " utworzono timestamp default current_timestamp); ";
                    cdk = new FbCommand(sql, polaczenie.getConnection());
                    cdk.ExecuteNonQuery();

                    sql = " create INDEX logskan_ndx1 on LOGSKAN (dokument_id);";
                    cdk = new FbCommand(sql, polaczenie.getConnection());
                    cdk.ExecuteNonQuery();

                    sql = " create INDEX logskan_ndx2 on LOGSKAN (id,utworzono);";
                    cdk = new FbCommand(sql, polaczenie.getConnection());
                    cdk.ExecuteNonQuery();

                    sql = " create INDEX logskan_ndx3 on LOGSKAN (kodkreskowy);";
                    cdk = new FbCommand(sql, polaczenie.getConnection());
                    cdk.ExecuteNonQuery();

                    sql = "CREATE TRIGGER trgLOGSKAN_BI for LOGSKAN active before insert position 0 as begin if ((new.id is null) or (new.id = 0)) ";
                    sql += "then begin new.id = NEXT VALUE FOR LOGSKAN_ID; end end;";
                    cdk = new FbCommand(sql, polaczenie.getConnection());
                    cdk.ExecuteNonQuery();

                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("Bład przy zakładaniu tabeli LOGSKAN: " + ex.Message);
                throw;
            }
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
                    bool znalazlem = false;

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
                            znalazlem = true;
                            break;
                        }
                    }

                    if (znalazlem)
                    {
                        kolorowanieRekordow();
                        sprawdzenieCzySkonczone();
                    }
                    else
                    {
                        
                        SoundPlayer player = new SoundPlayer();
                        player.SoundLocation = Application.StartupPath + "\\klaxon_ahooga.wav";
                        player.Load();
                        player.Play();

                        setLog("MESSAGE", "Nie znaleziono kodu kreskowego na dokumencie", tToSkan.Text, lListPrzewozowy.Text, lDokument.Text);

                    }
                    dataGridViewPozycje.Refresh();
                    tToSkan.Text = "";
                }
            }
        }

        private void setLog(string typ, string tresc, string kodKreskowy, string listPrzewozowy, string nrDokumentu)
        {
            //TODO: użytkownika
            //TODO: adres IP z jakiego jest to robione
            
            //dodanie obsługi logowania w bazie
            if (!typ.Equals("LOG"))
            {
                //logów nie wyświetlaj
                MessageBox.Show(typ + " KodKr:" + kodKreskowy + " TRESC:" + tresc + " Magazyn:" + logowanie.magID);
            } else if (!typ.Equals("INFO"))
            {
                //Zwykły wpis do loga
            }
            else if (!typ.Equals("MESSAGE"))
            {
                //Bład skanowania, np brak kodu na dokumencie
            }

            //string sql = "INSERT INTO LOGSKAN ";
            //sql += "(DATA,TIME,DOKID) ";
            //sql += " values ";
            //sql += " (" + DateTime.Now.ToShortDateString() + "," + DateTime.Now.ToShortTimeString() + "," + dokId + ")";
            //FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
            //try
            //{
            //    cdk.ExecuteNonQuery();
            //}
            //catch (FbException ex)
            //{
            //    StreamWriter writer = new StreamWriter(Environment.GetEnvironmentVariable("temp") + "\\Pakerator_" + DateTime.Now.ToShortDateString() + ".log", true);
            //    try
            //    {
            //        writer.WriteLine(DateTime.Now.ToString() + "; User:" + logowanie.userName + "; Kod kreskowy: " + tToSkan.Text + "; List przewozowy: " + lListPrzewozowy.Text + "; Dokument: " + lDokument.Text+ "; Bład zapytania: " + ex.Message);

            //    }
            //    catch (Exception exf)
            //    {
            //        //wyłączone na chwilę MessageBox.Show(exf.Message);
            //        throw;
            //    }
            //    finally
            //    {
            //        writer.Close();
            //    }
            //    throw;
            //}

            Console.WriteLine(typ + " KodKr:" + kodKreskowy + " TRESC:" + tresc + " Magazyn:" + logowanie.magID);

            textHistoria.Text = tresc + " ,kod: " + kodKreskowy + "; Dok.: " + lDokument.Text + Environment.NewLine + textHistoria.Text;
            
        }

        private void wyczyśćToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dokId = 0;
            lDokument.Text = "";
            lNabywcaTresc.Text = "";
            lOdbiorcaTresc.Text = "";
            lListPrzewozowy.Text = "";
            dataGridViewPozycje.DataSource = null;
            dataGridViewPozycje.Rows.Clear();
            dataGridViewPozycje.Columns.Clear();
            dataGridViewPozycje.Refresh();
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
                textHistoria.Text = "Dokument zakończony poprawnie: " + lDokument.Text + Environment.NewLine + textHistoria.Text;
                
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
                wyczyśćToolStripMenuItem.PerformClick();
            }
        }
    }
}
