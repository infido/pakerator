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
using System.IO;
using System.Net;

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
        int odbiorca = 0;
        int platnik = 0;
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
                    sql += " list_przewozowy varchar(30), ";
                    sql += " dokument_fs_id int, ";
                    sql += " dokument_mm_id int, ";
                    sql += " dokument_zo_id int, ";
                    sql += " towar_id int, ";
                    sql += " komunikat varchar(800), ";
                    sql += " operacja varchar(30), ";
                    sql += " magazyn_nazwa varchar(255), ";
                    sql += " magazyn_id int, ";
                    sql += " kontrahent varchar(255), ";
                    sql += " ip varchar(60), ";
                    sql += " host varchar(60), ";
                    sql += " odbiorca int, ";
                    sql += " platnik int, ";
                    sql += " utworzono timestamp default current_timestamp); ";
                    cdk = new FbCommand(sql, polaczenie.getConnection());
                    cdk.ExecuteNonQuery();

                    sql = " create INDEX logskan_ndx1fs on LOGSKAN (dokument_fs_id);";
                    cdk = new FbCommand(sql, polaczenie.getConnection());
                    cdk.ExecuteNonQuery();

                    sql = " create INDEX logskan_ndx1mm on LOGSKAN (dokument_mm_id);";
                    cdk = new FbCommand(sql, polaczenie.getConnection());
                    cdk.ExecuteNonQuery();

                    sql = " create INDEX logskan_ndx1zo on LOGSKAN (dokument_zo_id);";
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
            bool czyToJestListPrzewozowy = true; 
            string sql = "";
            int corectDocID = 0; 
            bool toJestMMP = true;

            if (kodKreskowy.Length > 0)
            {
                czyToJestListPrzewozowy = kodKreskowy.Substring(0, 2).Equals("MM") ? false : true;
            }

            if (czyToJestListPrzewozowy)
            {
                sql = "select ID, NUMER, NAZWA_PELNA_PLATNIKA, NAZWA_PELNA_ODBIORCY, ";
                sql += "COALESCE(ULICA_ODBIORCY,'') as ULICA_ODBIORCY, COALESCE(NRDOMU_ODBIORCY,'') as NRDOMU_ODBIORCY, COALESCE(NRLOKALU_ODBIORCY,'') as NRLOKALU_ODBIORCY, COALESCE(MIEJSCOWOSC_ODBIORCY,'') as MIEJSCOWOSC_ODBIORCY, ";
                sql += "COALESCE(PANSTWO_ODBIORCY,'') as PANSTWO_ODBIORCY, COALESCE(KOD_ODBIORCY,'') as KOD_ODBIORCY, OPERATOR, SYGNATURA, COALESCE(UWAGI,'') as UWAGI, ID_ODBIORCY, ID_PLATNIKA ";
                sql += "from GM_FS ";
                sql += "where MAGNUM=" + logowanie.magID + " and SYGNATURA='" + tToSkan.Text + "';";
            }
            else
            {
                string tmpkod = "";
                //ma być niezlależne, jak skanujemy MMR a jesteśmy na przyjeciu to robimy przyjęcie
                sql = "select ID, MAGNUM, ZRODLO_CEL, ID_MM, KOD from GM_MM where NUMER ='" + kodKreskowy + "'";
                FbCommand cdktmp = new FbCommand(sql, polaczenie.getConnection());
                try
                {
                    
                    FbDataReader fdktmp = cdktmp.ExecuteReader();
                    if (fdktmp.Read())
                    {
                        int magnum = (int)fdktmp["MAGNUM"];
                        int magnum2 = (int)fdktmp["ZRODLO_CEL"];
                        if (magnum == logowanie.magID)
                        {
                            //Dokument z bieżącego magazynu
                            corectDocID = (int)fdktmp["ID"];
                            
                            //Typ dokumentu jest brany z rekordu
                            tmpkod = (string)fdktmp["KOD"];
                            toJestMMP = tmpkod.Equals("MMP") ? true : false;
                        }
                        else if (magnum2 == logowanie.magID)
                        {
                            //zeskanowano dokument z przeciwnego magazynu
                            corectDocID = (int)fdktmp["ID_MM"];

                            //Typ dokumentu jest zamieniany na przeciwny bo jest odczyt z przeciwnego dokumentu
                            tmpkod = (string)fdktmp["KOD"];
                            toJestMMP = tmpkod.Equals("MMP") ? false : true;
                        }
                        fdktmp.Close();
                    }
                }
                catch (Exception ee)
                {
                    setLog("ERROR", "Błąd przy wysukiwaniu dokumentu przesunięcia MM: " + ee.Message, kodKreskowy, kodKreskowy, "", tmpkod);
                    throw;
                }
                sql = "select GM_MM.ID, GM_MM.NUMER, ";
                sql += " case GM_MM.KOD ";
                sql += "  when 'MMP' then (MAG2.NUMER || ' ' || MAG2.NAZWA) ";
                sql += "  when 'MMR' then (MAG1.NUMER || ' ' || MAG1.NAZWA) ";
                sql += "  else '' ";
                sql += " end as NAZWA_PELNA_PLATNIKA, ";
                sql += " case GM_MM.KOD ";
                sql += "  when 'MMP' then (MAG1.NUMER || ' ' || MAG1.NAZWA) ";
                sql += "  when 'MMR' then (MAG2.NUMER || ' ' || MAG2.NAZWA) ";
                sql += "  else '' ";
                sql += " end as  NAZWA_PELNA_ODBIORCY, ";
                sql += "'' as ULICA_ODBIORCY, '' as NRDOMU_ODBIORCY, '' as NRLOKALU_ODBIORCY, '' as MIEJSCOWOSC_ODBIORCY, ";
                sql += "'' as PANSTWO_ODBIORCY, '' as KOD_ODBIORCY, GM_MM.OPERATOR, GM_MM.SYGNATURA, COALESCE(GM_MM.UWAGI,'') as UWAGI, 0 as ID_ODBIORCY, 0 as ID_PLATNIKA ";
                sql += "from GM_MM ";
                sql += "join GM_MAGAZYNY as MAG1 on GM_MM.MAGNUM = MAG1.ID ";
                sql += "join GM_MAGAZYNY as MAG2 on GM_MM.ZRODLO_CEL = MAG2.ID ";
                sql += "where GM_MM.ID=" + corectDocID + ";";

            }
            FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
            try
            {
                FbDataReader fdk = cdk.ExecuteReader();
                if (fdk.Read())
                {
                    dokId = (int)fdk["ID"];
                    lDokument.Text = (string)fdk["NUMER"];
                    lListPrzewozowy.Text = kodKreskowy;
                    platnik = (int)fdk["ID_PLATNIKA"];
                    lNabywcaTresc.Text = (string)fdk["NAZWA_PELNA_PLATNIKA"];
                    odbiorca = (int)fdk["ID_ODBIORCY"];
                    lOdbiorcaTresc.Text = (string)fdk["NAZWA_PELNA_ODBIORCY"] + Environment.NewLine + (string)fdk["PANSTWO_ODBIORCY"] + "; " + (string)fdk["KOD_ODBIORCY"] + " " + (string)fdk["MIEJSCOWOSC_ODBIORCY"] + Environment.NewLine;
                    lOdbiorcaTresc.Text += (string)fdk["ULICA_ODBIORCY"] + " " + (string)fdk["NRDOMU_ODBIORCY"] + " " + (string)fdk["NRLOKALU_ODBIORCY"];

                    //Tu wczytujemy pozycje dokumentu
                    if (czyToJestListPrzewozowy)
                    {
                        //Dla faktury
                        ltypdok.Text = "FS";
                        sql = "select GM_FSPOZ.ID, GM_FSPOZ.LP, GM_TOWARY.TYP, GM_TOWARY.SKROT, COALESCE(GM_TOWARY.SKROT2,'') as SKROT2, COALESCE(GM_TOWARY.KOD_KRESKOWY,'') as KOD_KRESKOWY, GM_TOWARY.NAZWA, GM_FSPOZ.ILOSC, 0 as SKANOWANE, COALESCE(GM_FSPOZ.ZNACZNIKI,'') as ZNACZNIKI, GM_FSPOZ.ID_TOWARU ";
                        sql += "from GM_FSPOZ ";
                        sql += "join GM_TOWARY ON GM_FSPOZ.ID_TOWARU=GM_TOWARY.ID ";
                        sql += "where GM_FSPOZ.ID_GLOWKI=" + dokId;
                    }
                    else if (toJestMMP)
                    {
                        //Dla przesunięcia między magazynowego MMP
                        ltypdok.Text = "MMP";
                        sql =  "select GM_MMPPOZ.ID, GM_MMPPOZ.LP, ";
                        sql += "GM_TOWARY.TYP, GM_TOWARY.SKROT, COALESCE(GM_TOWARY.SKROT2,'') as SKROT2, COALESCE(GM_TOWARY.KOD_KRESKOWY,'') as KOD_KRESKOWY, GM_TOWARY.NAZWA, ";
                        sql += "GM_MMPPOZ.ILOSC_PO as ILOSC, 0 as SKANOWANE, COALESCE(GM_MMPPOZ.ZNACZNIKI,'') as ZNACZNIKI, GM_MMPPOZ.ID_TOWARU ";
                        sql += " from GM_MMPPOZ";
                        sql += " join GM_TOWARY ON GM_MMPPOZ.ID_TOWARU=GM_TOWARY.ID ";
                        sql += " where GM_MMPPOZ.ID_GLOWKI=" + dokId;
                    }
                    else if (!toJestMMP)
                    {
                        //Dla przesunięcia między magazynowego MMR
                        ltypdok.Text = "MMR";
                        sql = "select GM_MMRPOZ.ID, GM_MMRPOZ.LP, ";
                        sql += "GM_TOWARY.TYP, GM_TOWARY.SKROT, COALESCE(GM_TOWARY.SKROT2,'') as SKROT2, COALESCE(GM_TOWARY.KOD_KRESKOWY,'') as KOD_KRESKOWY, GM_TOWARY.NAZWA, ";
                        sql += "GM_MMRPOZ.ILOSC_PO as ILOSC, 0 as SKANOWANE, COALESCE(GM_MMRPOZ.ZNACZNIKI,'') as ZNACZNIKI, GM_MMRPOZ.ID_TOWARU ";
                        sql += " from GM_MMRPOZ";
                        sql += " join GM_TOWARY ON GM_MMRPOZ.ID_TOWARU=GM_TOWARY.ID ";
                        sql += " where GM_MMRPOZ.ID_GLOWKI=" + dokId;
                    }
                    setLog("LOG", "Ustawienie dokumentu i odbiorcy", kodKreskowy, kodKreskowy, lDokument.Text, ltypdok.Text);

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
                        setLog("ERROR", "Nieudane zapytanie o pozycjie dokumentu dla listy: " + ex.Message, kodKreskowy, kodKreskowy, lDokument.Text, ltypdok.Text);
                        throw;
                    }
                    fDataView.Table = fds.Tables["POZ"];
                    dataGridViewPozycje.DataSource = fDataView;

                    dataGridViewPozycje.Columns["ID"].Visible = false;
                    dataGridViewPozycje.Columns["ID_TOWARU"].Visible = false;
                    kolorowanieRekordow();

                    if (czyToJestListPrzewozowy)
                    {
                        cdk = new FbCommand("UPDATE GM_FS SET ZNACZNIKI='Pakuje:" + logowanie.userName + " " + DateTime.Now.ToShortDateString() + " " +
                        DateTime.Now.ToShortTimeString() + "; ' || COALESCE(ZNACZNIKI,'') where ID=" + dokId, polaczenie.getConnection());
                    }
                    else if (toJestMMP)
                    {
                        //aktualizauje nagłówka MMP
                        cdk = new FbCommand("UPDATE GM_MM SET ZNACZNIKI='Rozpakowuje:" + logowanie.userName + " " + DateTime.Now.ToShortDateString() + " " +
                        DateTime.Now.ToShortTimeString() + "; ' || COALESCE(ZNACZNIKI,'') where ID=" + dokId, polaczenie.getConnection());
                    }
                    else
                    {
                        //aktualizauje nagłówka MMR
                        cdk = new FbCommand("UPDATE GM_MM SET ZNACZNIKI='Pakuje:" + logowanie.userName + " " + DateTime.Now.ToShortDateString() + " " +
                        DateTime.Now.ToShortTimeString() + "; ' || COALESCE(ZNACZNIKI,'') where ID=" + dokId, polaczenie.getConnection());
                    }

                    try
                    {
                        cdk.ExecuteNonQuery();
                    }
                    catch (FbException ex)
                    {
                        setLog("ERROR", "Bład zapytania: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                        throw;
                    }
                    
                }
                else
                {
                    setLog("WARNING", "Nie znaleziono dokumentu dla listu przewozowego, ani MM", kodKreskowy, kodKreskowy, "", "");
                }
            }
            catch (FbException ex)
            {
                setLog("ERR","Błąd zapytania SQL001: " + ex.Message, kodKreskowy, kodKreskowy, "","");
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
                            FbCommand cdk = null;
                            if (ltypdok.Text.Equals("FS"))
                            {
                                cdk = new FbCommand("UPDATE GM_FSPOZ SET ZNACZNIKI=" + row.Cells["SKANOWANE"].Value + " where ID=" + row.Cells["ID"].Value, polaczenie.getConnection());
                            }
                            else if (ltypdok.Text.Equals("MMP"))
                            {
                                cdk = new FbCommand("UPDATE GM_MMPPOZ SET ZNACZNIKI=" + row.Cells["SKANOWANE"].Value + " where ID=" + row.Cells["ID"].Value, polaczenie.getConnection());
                            }
                            else if (ltypdok.Text.Equals("MMR"))
                            {
                                cdk = new FbCommand("UPDATE GM_MMRPOZ SET ZNACZNIKI=" + row.Cells["SKANOWANE"].Value + " where ID=" + row.Cells["ID"].Value, polaczenie.getConnection());
                            }
                            try
                            {
                                cdk.ExecuteNonQuery();
                                setLog("LOG", "Skanowanie towaru znajdującego sie na dokumencie. Ilość po skanowaniu " + (int)row.Cells["SKANOWANE"].Value, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, (int)row.Cells["ID_TOWARU"].Value, ltypdok.Text);
                            }
                            catch (FbException ex)
                            {
                                setLog("ERROR", "Bład zapytania: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
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

                        //TODO: dodanie obsługi odnalezienia id towaru skanowanego do wpisania w LOG
                        setLog("MESSAGE", "Nie znaleziono kodu kreskowego na dokumencie", tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);

                    }
                    dataGridViewPozycje.Refresh();
                    tToSkan.Text = "";
                }
            }
        }

        private void setLog(string typ, string tresc, string kodKreskowy, string listPrzewozowy, string nrDokumentu, string typDok)
        {
            setLog(typ, tresc, kodKreskowy, listPrzewozowy, nrDokumentu, 0, typDok);
        }

        private void setLog(string typ, string tresc, string kodKreskowy, string listPrzewozowy, string nrDokumentu, int idTowaru, string typDok )
        {
            //TODO: użytkownika
            //TODO: adres IP z jakiego jest to robione

            string sql = "";

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


                sql = "INSERT INTO LOGSKAN ";
                sql += "(pracownik, kodkreskowy, list_przewozowy , dokument_fs_id, dokument_mm_id, dokument_zo_id ,towar_id, komunikat, operacja";
                sql += ", magazyn_nazwa, magazyn_id, kontrahent, ip, host, odbiorca, platnik ) ";
                sql += " values ";
                sql += " ('" + logowanie.userName + "','" + tToSkan.Text + "','" + lListPrzewozowy.Text + "',";
                if (typDok.Equals("FS"))
                {
                    sql += dokId + ",0,0,"; //zera mm_id i zo_id 
                }
                else if (typDok.Contains("MM"))
                {
                    sql +=  "0," + dokId + ",0,"; //zera mm_id i zo_id
                }
                else
                {
                    sql += "0,0,0,"; //zera mm_id i zo_id
                }
                sql += idTowaru + ",'" + tresc + "','" + typ + "','" + logowanie.magNazwa + "'," + logowanie.getIdMagazynAsString() + ",'" + lNabywcaTresc.Text + "','" + getIpAdress() + "','" + Dns.GetHostName() + "'," + odbiorca + "," + platnik + ");";


            FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
            try
            {
                cdk.ExecuteNonQuery();
            }
            catch (FbException ex)
            {
                StreamWriter writer = new StreamWriter(Environment.GetEnvironmentVariable("temp") + "\\Pakerator_" + DateTime.Now.ToShortDateString() + ".log", true);
                try
                {
                    writer.WriteLine(DateTime.Now.ToString() + "; User:" + logowanie.userName + "; Kod kreskowy: " + tToSkan.Text + "; List przewozowy: " + lListPrzewozowy.Text + "; Dokument: " + lDokument.Text + "; Bład zapytania: " + ex.Message);

                }
                catch (Exception exf)
                {
                    MessageBox.Show("Błąd zapisu błedu: " + exf.Message);
                    throw;
                }
                finally
                {
                    writer.Close();
                }
                throw;
            }

            //Console.WriteLine(typ + " KodKr:" + kodKreskowy + " TRESC:" + tresc + " Magazyn:" + logowanie.magID);

            textHistoria.Text = tresc + " ,kod: " + kodKreskowy + "; Dok.: " + lDokument.Text + Environment.NewLine + textHistoria.Text;
            
        }

        private string getIpAdress()
        {
            string wynik = "";
            try
            { // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                foreach (IPAddress localIP in localIPs)
                {
                    if (localIP.Equals("127.0.0.1") || localIP.Equals("localhost"))
                    {
                        //nie rób nic
                    }
                    else
                    {
                        if (wynik.Length != 0)
                        {
                            wynik += "; ";
                        }
                        wynik += localIP.ToString();
                    }
                }
            }
            catch { }
            return wynik;
        }

        private void wyczyśćToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dokId = 0;
            lDokument.Text = "";
            lNabywcaTresc.Text = "";
            lOdbiorcaTresc.Text = "";
            lListPrzewozowy.Text = "";
            ltypdok.Text = "";
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
                FbCommand cdk = null;
                if (ltypdok.Text.Equals("FS"))
                {
                    cdk = new FbCommand("UPDATE GM_FS SET ZNACZNIKI='Zapakował:" + DateTime.Now.ToShortDateString() + " " +
                          DateTime.Now.ToShortTimeString() + "; ' || ZNACZNIKI where ID=" + dokId, polaczenie.getConnection());
                }
                else if (ltypdok.Text.Equals("MMR"))
                {
                    cdk = new FbCommand("UPDATE GM_MM SET ZNACZNIKI='Zapakował:" + DateTime.Now.ToShortDateString() + " " +
                          DateTime.Now.ToShortTimeString() + "; ' || ZNACZNIKI where ID=" + dokId, polaczenie.getConnection());
                }
                else if (ltypdok.Text.Equals("MMP"))
                {
                    cdk = new FbCommand("UPDATE GM_MM SET ZNACZNIKI='Rozpakował:" + DateTime.Now.ToShortDateString() + " " +
                          DateTime.Now.ToShortTimeString() + "; ' || ZNACZNIKI where ID=" + dokId, polaczenie.getConnection());
                }
                try
                {
                    cdk.ExecuteNonQuery();
                }
                catch (FbException ex)
                {
                    setLog("ERROR", "Bład zapytania: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
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

        private void historiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Historia hist = new Historia(polaczenie);
            hist.Show();
        }
    }
}
