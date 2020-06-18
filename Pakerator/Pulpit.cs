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
using System.ServiceModel;
using Pakerator.ApiProductsSocksServiceGet;
using Microsoft.Win32;

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
        public int magID, magID2;
        public string magKod, magKod2;
        public string magNazwa, magNazwa2;
        Dictionary<int, string> listMagazyny;

        public Pulpit()
        {
            InitializeComponent();
            Text = "Pakerator " + Application.ProductVersion;
            polaczenie = new ConnectionDB();
            logowanie = new Login(polaczenie);

            logowanie.ShowDialog();

            setDictonary();
            setSetingsOfStores();
            setLog("ENTRY", "999 Logowanie do systemu Wersja:" + Application.ProductVersion + "; user: " + logowanie.userName + "; ustawiono kontekst: " + magNazwa, "", "", "", 0, "");
            chkTableLOGSKAN();
            setCMagazynFromReg();
        }

        #region sprawdzanie czy jest założona tabela w bazie
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
        #endregion

        private void Pulpit_FormClosed(object sender, FormClosedEventArgs e)
        {
            polaczenie.Close();
        }

        private void konfiguracjaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polaczenie.showForTest(logowanie.userName, magNazwa, magID, getIpAdress(), Dns.GetHostName());
        }

        private void SetDokument(string kodKreskowy)
        {
            bool czyToJestListPrzewozowy = true; 
            string sql = "";
            int corectDocID = 0; 
            bool toJestMMP = true;

            if (kodKreskowy.Length > 0)
            {
                if (kodKreskowy.Contains("/P/") || kodKreskowy.Contains("/R/") || kodKreskowy.Contains("MM"))
                {
                    czyToJestListPrzewozowy = false;
                }
                else
                {
                    czyToJestListPrzewozowy = true;
                }
            }

            if (czyToJestListPrzewozowy)
            {
                sql = "select ID, NUMER, NAZWA_PELNA_PLATNIKA, NAZWA_PELNA_ODBIORCY, ";
                sql += "COALESCE(ULICA_ODBIORCY,'') as ULICA_ODBIORCY, COALESCE(NRDOMU_ODBIORCY,'') as NRDOMU_ODBIORCY, COALESCE(NRLOKALU_ODBIORCY,'') as NRLOKALU_ODBIORCY, COALESCE(MIEJSCOWOSC_ODBIORCY,'') as MIEJSCOWOSC_ODBIORCY, ";
                sql += "COALESCE(PANSTWO_ODBIORCY,'') as PANSTWO_ODBIORCY, COALESCE(KOD_ODBIORCY,'') as KOD_ODBIORCY, OPERATOR, SYGNATURA, COALESCE(UWAGI,'') as UWAGI, ID_ODBIORCY, ID_PLATNIKA ";
                sql += "from GM_FS ";
                sql += "where MAGNUM=" + magID + " and SYGNATURA='" + tToSkan.Text + "';";
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
                        if (magnum == magID)
                        {
                            //Dokument z bieżącego magazynu
                            corectDocID = (int)fdktmp["ID"];
                            
                            //Typ dokumentu jest brany z rekordu
                            tmpkod = (string)fdktmp["KOD"];
                            toJestMMP = tmpkod.Equals("MMP") ? true : false;
                        }
                        else if (magnum2 == magID)
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
                    setLog("ERROR", "005 Błąd przy wysukiwaniu dokumentu przesunięcia MM: " + ee.Message, kodKreskowy, kodKreskowy, "", tmpkod);
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
            
            try
            {
                FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
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
                        sql += ", -1 STAN_" + magKod + " ";
                        if (magID2 != 0 && magID != magID2)
                            sql += " , -1 STAN_" + magKod2 + " ";
                        sql += " , -1 W_WYDANIU_" + magKod + " ";
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
                        sql += ", -1 STAN_" + magKod + " ";
                        if (magID2 != 0 && magID != magID2)
                            sql += " , -1 STAN_" + magKod2 + " ";
                        sql += " , -1 W_WYDANIU_" + magKod + " ";
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
                        sql += ", -1 STAN_" + magKod + " ";
                        if (magID2 != 0 && magID != magID2)
                            sql += " , -1 STAN_" + magKod2 + " ";
                        sql += " , -1 W_WYDANIU_" + magKod + " ";
                        sql += " from GM_MMRPOZ";
                        sql += " join GM_TOWARY ON GM_MMRPOZ.ID_TOWARU=GM_TOWARY.ID ";
                        sql += " where GM_MMRPOZ.ID_GLOWKI=" + dokId;
                    }
                    setLog("LOG", "006 Ustawienie dokumentu i odbiorcy", kodKreskowy, kodKreskowy, lDokument.Text, ltypdok.Text);

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
                        setLog("ERROR", "007 Nieudane zapytanie o pozycjie dokumentu dla listy: " + ex.Message, kodKreskowy, kodKreskowy, lDokument.Text, ltypdok.Text);
                        throw;
                    }
                    fDataView.Table = fds.Tables["POZ"];
                    dataGridViewPozycje.DataSource = fDataView;

                    dataGridViewPozycje.Columns["ID"].Visible = false;
                    dataGridViewPozycje.Columns["ID_TOWARU"].Visible = false;
                    kolorowanieRekordow();

                    string tab = "";
                    if (czyToJestListPrzewozowy)
                    {
                        tab = "GM_FS";
                    }
                    else if (toJestMMP)
                    {
                        //aktualizauje nagłówka MMP
                        tab = "GM_MM";
                    }
                    else
                    {
                        //aktualizauje nagłówka MMR
                        tab = "GM_MM";
                    }

                    cdk = new FbCommand("UPDATE " + tab + " SET ZNACZNIKI=COALESCE('" + logowanie.userName + " " + DateTime.Now.ToShortDateString() + " " +
                            DateTime.Now.ToShortTimeString() + ";',ZNACZNIKI)where ID=" + dokId, polaczenie.getConnection());

                    try
                    {
                        cdk.ExecuteNonQuery();
                    }
                    catch (FbException ex)
                    {
                        if (ex.ErrorCode == 335544345)
                        {
                            //blokowanie dalszych operacji - zablokowany dokument w Raks
                            tToSkan.Enabled = false;
                            lBlokadaDokwRaks.Visible = true;
                            
                            if (tab.Equals("GM_FS"))
                              setLog("ERROR", "011 Dokument faktury: " + lDokument.Text + " jest zablokowany przez innego użytkownika programu! Nie ustawiono informacji o rozpoczeciu skanowania!", tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                            else
                                setLog("ERROR", "012 Dokument przesunięcia międzymagazynowego: " + lDokument.Text + " jest zablokowany przez innego użytkownika programu! Nie ustawiono informacji o rozpoczeciu skanowania!", tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                        }
                        else
                        {
                            setLog("ERROR", "008 Bład zapytania: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                            throw;
                        }
                    }
                    
                }
                else
                {
                    setLog("WARNING", "009 Nie znaleziono dokumentu dla listu przewozowego, ani MM", kodKreskowy, kodKreskowy, "", "");
                }
            }
            catch (FbException ex)
            {
                setLog("ERR","010 Błąd zapytania SQL001: " + ex.Message, kodKreskowy, kodKreskowy, "","");
            }
        }

        private void tToSkan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Return))
            {
                if (dokId == 0)
                {
                    //wybór dokumentu
                    //textHistoria.Text = "List przewozowy: " + tToSkan.Text + Environment.NewLine + textHistoria.Text;
                    zapiszHistoria("001 Wybór dokumentu >> List przewozowy: " + tToSkan.Text); 
                    SetDokument(tToSkan.Text);
                    tToSkan.Text = "";
                }
                else
                {
                    //skanowanie towaru
                    bool znalazlem = false;

                    //textHistoria.Text = "Towar: " + tToSkan.Text + Environment.NewLine + textHistoria.Text;
                    zapiszHistoria("002 Skanowanie towaru: " + tToSkan.Text);
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
                                setLog("LOG", "011 Skanowanie towaru znajdującego sie na dokumencie. Ilość po skanowaniu " + (int)row.Cells["SKANOWANE"].Value, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, (int)row.Cells["ID_TOWARU"].Value, ltypdok.Text);
                            }
                            catch (FbException ex)
                            {
                                setLog("ERROR", "012 Bład zapytania: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
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
                        setLog("MESSAGE", "013 Nie znaleziono kodu kreskowego na dokumencie", tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);

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

        public static void putLog(ConnectionDB conn ,string usrName, string typ, string tresc, string kodKreskowy, string listPrzewozowy, string nrDokumentu, int idTowaru, string typDok, int dokId, string magNazwa, int magID, string nabywcaTresc, string odbiorca, string platnik)
        {
            string sql = "INSERT INTO LOGSKAN ";
            sql += "(pracownik, kodkreskowy, list_przewozowy , dokument_fs_id, dokument_mm_id, dokument_zo_id ,towar_id, ";
            sql += " komunikat, operacja";
            sql += ", magazyn_nazwa, magazyn_id, kontrahent, ip, host, odbiorca, platnik ) ";
            sql += " values ";
            sql += " ('" + usrName + "','" + kodKreskowy + "','',";
            if (typDok.Equals("FS"))
            {
                sql += dokId + ",0,0,"; //zera mm_id i zo_id 
            }
            else if (typDok.Contains("MM"))
            {
                sql += "0," + dokId + ",0,"; //zera mm_id i zo_id
            }
            else
            {
                sql += "0,0,0,"; //zera mm_id i zo_id
            }
            sql += idTowaru + ",'" + tresc + "','" + typ + "','" + magNazwa + "'," + magID + ",'" + nabywcaTresc + "','" + getIpAdress() + "','" + Dns.GetHostName() + "'," + odbiorca + "," + platnik + ");";


            FbCommand cdk = new FbCommand(sql, conn.getConnection());
            try
            {
                cdk.ExecuteNonQuery();
            }
            catch (FbException ex)
            {
                //zapiszDoLOG(ex.Message);
                //throw;
            }
        }

        private void setLog(string typ, string tresc, string kodKreskowy, string listPrzewozowy, string nrDokumentu, int idTowaru, string typDok )
        {
            zapiszHistoriaTryb("Typ:" + typ + "; Treść:" + tresc + "; Kod kreskowy:" + kodKreskowy + "; List przewozowy:" + listPrzewozowy + "; Nr Dok.:" + nrDokumentu + "; Id towaru:" + idTowaru + "; Typ Dok." + typDok, false);   
            
            //TODO: użytkownika
            //TODO: adres IP z jakiego jest to robione

            string sql = "";

            //dodanie obsługi logowania w bazie
            if (typ.Equals("ENTRY"))
            {
                //Info o logowaniu, bez komunikatu --- TRYB Silent
            }
            else if (!typ.Equals("LOG"))
            {
                //logów nie wyświetlaj
                MessageBox.Show(typ + " KodKr:" + kodKreskowy + " TRESC:" + tresc + " Magazyn:" + magID);
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
                sql += idTowaru + ",'" + tresc + "','" + typ + "','" + magNazwa + "'," + magID + ",'" + lNabywcaTresc.Text + "','" + getIpAdress() + "','" + Dns.GetHostName() + "'," + odbiorca + "," + platnik + ");";


            FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
            try
            {
                cdk.ExecuteNonQuery();
            }
            catch (FbException ex)
            {
                zapiszDoLOG(ex.Message);
                throw;
            }

            //Console.WriteLine(typ + " KodKr:" + kodKreskowy + " TRESC:" + tresc + " Magazyn:" + logowanie.magID);

            zapiszHistoria(tresc + " ,kod: " + kodKreskowy + "; Dok.: " + lDokument.Text);
        }

        private void zapiszHistoria(string linia)
        {
            zapiszHistoriaTryb(linia, true);
        }

        private void zapiszHistoriaTryb(string linia,bool wyswietlWHistorii)
        {
            if (wyswietlWHistorii)
            {
                textHistoria.Text = linia + Environment.NewLine + textHistoria.Text;
            }
            StreamWriter writer = new StreamWriter(Environment.GetEnvironmentVariable("temp") + "\\Pakerator_historia_" + DateTime.Now.ToShortDateString() + ".log", true);
            try
            {
                writer.WriteLine(DateTime.Now.ToString() + "; " + linia);
            }
            finally
            {
                writer.Close();
            }
            
        }

        private void zapiszDoLOG(string tekst)
        {
            StreamWriter writer = new StreamWriter(Environment.GetEnvironmentVariable("temp") + "\\Pakerator_" + DateTime.Now.ToShortDateString() + ".log", true);
            try
            {
                writer.WriteLine(DateTime.Now.ToString() + "; User:" + logowanie.userName + "; Kod kreskowy: " + tToSkan.Text + "; List przewozowy: " + lListPrzewozowy.Text + "; Dokument: " + lDokument.Text + "; ststus połączenia: " + polaczenie.getConnectioState() + " Bład zapytania: " + tekst);
                if (polaczenie.getConnectioState() <= 0)
                {
                    writer.WriteLine("Proba zakniecia systemu z braku polaczenia do bazy danych");
                    MessageBox.Show("Błąd połączenia, program zostanie zamknięty, prosze uruchomić go ponownie.");
                    Application.Exit();
                }

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
        }

        public static string getIpAdress()
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
            tToSkan.Enabled = true;
            lBlokadaDokwRaks.Visible = false;
            bSetStatusAgain.Visible = false;

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
            Int32 magA = 0;
            Int32 magB = 0;

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

                magA = sprawdzenieStanuMagazynu(magID, row.Cells["SKROT"].Value.ToString());
                row.Cells["STAN_" + magKod].Value = magA;

                row.Cells["W_WYDANIU_" + magKod].Value = getIloscWWydaniu(magID, row.Cells["SKROT"].Value.ToString()) - Convert.ToInt32(row.Cells["ILOSC"].Value);

                if ((magA - Convert.ToInt32(row.Cells["W_WYDANIU_" + magKod].Value)) <= Convert.ToInt32(row.Cells["ILOSC"].Value))
                {
                    row.Cells["W_WYDANIU_" + magKod].Style.BackColor = Color.DeepPink;
                    row.Cells["W_WYDANIU_" + magKod].Style.ForeColor = Color.Yellow;
                    row.Cells["STAN_" + magKod].Style.BackColor = Color.DeepPink;
                    row.Cells["STAN_" + magKod].Style.ForeColor = Color.Yellow;
                }

                if (magA <= Convert.ToInt32(row.Cells["ILOSC"].Value))
                {
                    row.Cells["STAN_" + magKod].Style.BackColor = Color.DarkRed;
                    row.Cells["STAN_" + magKod].Style.ForeColor = Color.Yellow;
                }

                if (magID2 != 0 && magID != magID2)
                {
                    magB = sprawdzenieStanuMagazynu(magID2, row.Cells["SKROT"].Value.ToString());
                    row.Cells["STAN_" + magKod2].Value = magB;
                }
            }
        }

        private Int32 sprawdzenieStanuMagazynu(int idMag, string kodTowaru)
        {
            string sql = "SELECT sum(ILOSC) from GM_MAGAZYN join GM_TOWARY on ID_TOWAR=GM_TOWARY.ID_TOWARU ";
            sql += " where GM_TOWARY.SKROT='" + kodTowaru + "' and GM_MAGAZYN.MAGNUM=" + idMag + ";" ;


            FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
            try
            {
                if (cdk.ExecuteScalar() != DBNull.Value)
                    return Convert.ToInt32(cdk.ExecuteScalar());
                else
                    return 0;
            }
            catch (FbException ex)
            {
                setLog("ERROR", "014 Bład zapytania: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                zapiszDoLOG("Błąd sprawdzenia stanu magazynowego: " + ex.Message);
                return -2;
            }
        }

        private Int32 getIloscWWydaniu(int idMag, string kodTowaru)
        {
            string sql = "SELECT sum(ILOSC) from GM_FSPOZ  ";
            sql += " join GM_FS on GM_FSPOZ.ID_GLOWKI=GM_FS.ID join GM_TOWARY on GM_FSPOZ.ID_TOWARU=GM_TOWARY.ID_TOWARU ";
            sql += " where GM_FS.MAGAZYNOWY=0 AND GM_FS.FISKALNY=0 ";
            sql += " AND SKROT='" + kodTowaru + "' and GM_FS.MAGNUM=" + idMag + ";";


            FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
            try
            {
                if (cdk.ExecuteScalar() != DBNull.Value)
                    return Convert.ToInt32(cdk.ExecuteScalar());
                else
                    return 0;
            }
            catch (FbException ex)
            {
                setLog("ERROR", "015 Bład zapytania: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                zapiszDoLOG("Błąd sprawdzenia stanu magazynowego: " + ex.Message);
                return 0;
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
                //textHistoria.Text = "Dokument zakończony poprawnie: " + lDokument.Text + Environment.NewLine + textHistoria.Text;
                zapiszHistoria("003 Dokument zakończony poprawnie: " + lDokument.Text);
                FbCommand cdk = null;
                FbCommand controlCdk = null;
                if (ltypdok.Text.Equals("FS"))
                {
                    controlCdk = new FbCommand(" SELECT ZNACZNIKI from GM_FS where ID=" + dokId + " WITH LOCK;", polaczenie.getConnection());
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
                     if (ex.ErrorCode == 335544345)
                      {
                        tToSkan.Enabled = false;
                        lBlokadaDokwRaks.Visible = true;
                        bSetStatusAgain.Visible = true;
                        setLog("ERROR", "004a Bład zapytania kontrolnego przed aktualizacją nagłówka dokumentu: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                        return;
                      }
                      else
                       {
                        setLog("ERROR", "004 Bład zapytania: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                        throw;
                       }
                     }


                SoundPlayer player = new SoundPlayer();
                player.SoundLocation = Application.StartupPath + "\\fanfare_x.wav";
                player.Load();
                player.Play();

                MessageBox.Show("Dokument jest skończony!", "Potwierdzenie", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                zapiszHistoriaTryb("Do skanowania" + tToSkan.Text + "; List przewozowy " + lListPrzewozowy.Text + "; Dokument " + lDokument.Text +"; Typ dokumentu  " + ltypdok.Text, false);
                wyczyśćToolStripMenuItem.PerformClick();
            }
        }

        private void historiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Historia hist = new Historia(polaczenie);
            hist.Show();
        }

        private void Pulpit_Shown(object sender, EventArgs e)
        {
            zapiszHistoriaTryb("Pokazano pulpit...", false);
        }

        private void Pulpit_Leave(object sender, EventArgs e)
        {
            zapiszHistoriaTryb("Opuszczono pulpit...", false);
        }

        private void Pulpit_Activated(object sender, EventArgs e)
        {
            zapiszHistoriaTryb("Aktywacja pulpitu...", false);
        }

        private void bSetStatusAgain_Click(object sender, EventArgs e)
        {
            sprawdzenieCzySkonczone();
        }

        private void cMagazyn_SelectedValueChanged(object sender, EventArgs e)
        {
        }

        private void cMagazyn_Leave(object sender, EventArgs e)
        {
            setSetingsOfStores();
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator", true);
            rejestr.SetValue("Mag1Settings", cMagazyn.SelectedValue);
        }

        private void setCMagazynFromReg()
        {
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            if (rejestr.GetValue("Mag1Settings")!=null)
            {
                cMagazyn.SelectedValue = (int)rejestr.GetValue("Mag1Settings");
            }

            if (rejestr.GetValue("Mag2Settings") != null)
            {
                cMagazyn2.SelectedValue = (int)rejestr.GetValue("Mag2Settings");
            }
        }

        private void cMagazyn2_Leave(object sender, EventArgs e)
        {
            setSetingsOfStores();
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator", true);
            rejestr.SetValue("Mag2Settings", cMagazyn2.SelectedValue);
        }

        private void menu2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataSet fdsr = new DataSet();
            fdsr.Tables.Add("TAB");
            fdsr.Tables["TAB"].Columns.Add("SKROT", typeof(String));
            fdsr.Tables["TAB"].Columns.Add("SKROT2", typeof(String));
            fdsr.Tables["TAB"].Columns.Add("KOD_KRESKOWY", typeof(String));
            fdsr.Tables["TAB"].Columns.Add("NAZWA", typeof(String));
            fdsr.Tables["TAB"].Columns.Add("M0", typeof(Double));
            fdsr.Tables["TAB"].Columns.Add("M1", typeof(Double));
            fdsr.Tables["TAB"].Columns.Add("M2", typeof(Double));
            fdsr.Tables["TAB"].Columns.Add("M3", typeof(Double));
            fdsr.Tables["TAB"].Columns.Add("M4", typeof(Double));
            fdsr.Tables["TAB"].Columns.Add("M5", typeof(Double));
            fdsr.Tables["TAB"].Columns.Add("M6", typeof(Double));
            fdsr.Tables["TAB"].Columns.Add("M7", typeof(Double));
            fdsr.Tables["TAB"].Columns.Add("M8", typeof(Double));
            fdsr.Tables["TAB"].Columns.Add("M9", typeof(Double));
            fdsr.Tables["TAB"].Columns.Add("M10", typeof(Double));
            fdsr.Tables["TAB"].Columns.Add("M11", typeof(Double));
            //fdsr.Tables["TAB"].Columns.Add("MAG", typeof(String));
            //fdsr.Tables["TAB"].Columns.Add("ILOSC", typeof(Double));

            var binding = new BasicHttpBinding();
            var address = new EndpointAddress("http://" + SessionIAI.GetIAIDomainForCurrentSession() + "/api/?gate=productsstocks/get/106/soap");
            var client = new ApiProductsSocksServiceGet.ApiProductsStocksPortTypeClient(binding, address);

            var request = new ApiProductsSocksServiceGet.getRequestType();
            request.authenticate = new ApiProductsSocksServiceGet.authenticateType();
            request.authenticate.system_key = SessionIAI.GetIAIKeyForCurrentSession();
            request.authenticate.system_login = SessionIAI.GetIAILoginForCurrentSession();

            request.@params = new getParamsType();
            request.@params.products = new sizeIdentType[1];
            request.@params.products[0] = new sizeIdentType();
            request.@params.products[0].identType = identsType.index;

            foreach (DataGridViewRow row in dataGridViewPozycje.Rows)
            {
                request.@params.products[0].identValue = row.Cells["SKROT"].Value.ToString();

                ApiProductsSocksServiceGet.getResponseType response = client.get(request);

                Double[] kol = new Double[12];
                string[] mags = new string[12];
                foreach (ApiProductsSocksServiceGet.getStockType stock in response.results[0].quantities.stocks)
                {
                    //if (!fdsr.Tables["TAB"].Columns.Contains("M"+stock.stock_id))
                    //{
                    //    fdsr.Tables["TAB"].Columns.Add("M" + stock.stock_id, typeof(Double));
                    //}
                    
                    mags[stock.stock_id] = "M" + stock.stock_id;

                    try
                    {
                        foreach (ApiProductsSocksServiceGet.getSizeType size in stock.sizes)
                        {
                            kol[stock.stock_id] = Convert.ToDouble( size.quantity);
                        }
                    }
                    catch
                    {

                    }

                }

                //textHistoria.Text += "Skrót:" + row.Cells["SKROT"].Value.ToString() + "; magazyn: M" + stock.stock_id + "; ilość: " + size.quantity + System.Environment.NewLine;
                    fdsr.Tables["TAB"].Rows.Add(row.Cells["SKROT"].Value.ToString(), row.Cells["SKROT2"].Value.ToString(), row.Cells["KOD_KRESKOWY"].Value.ToString(),
                    row.Cells["NAZWA"].Value.ToString(), kol[0], kol[1], kol[2], kol[3], kol[4], kol[5], kol[6], kol[7], kol[8], kol[9], kol[10], kol[11]);
            }

            Raport rt = new Raport(fdsr);
            rt.Show();
        }

        private void pobranieInfoOTowarachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var binding = new BasicHttpBinding();
            var address = new EndpointAddress("http://" + SessionIAI.GetIAIDomainForCurrentSession() + "/api/?gate=products/get/106/soap");
            var client = new ApiProductsServiceGet.ApiProductsPortTypeClient(binding, address);

            var request = new ApiProductsServiceGet.requestType();
            request.authenticate = new ApiProductsServiceGet.authenticateType();
            request.authenticate.userLogin = SessionIAI.GetIAILoginForCurrentSession();
            request.authenticate.authenticateKey = SessionIAI.GetIAIKeyForCurrentSession();

            request.@params = new ApiProductsServiceGet.paramsType();
            request.@params.returnProducts = "active";
            request.@params.returnElements = new string[1];
            request.@params.returnElements[0] = "returnElements";
            request.@params.productIsAvailable = "productIsAvailable";
            request.@params.productIsVisible = "productIsVisible";
            request.@params.productIndexes = new ApiProductsServiceGet.productIndexItemType[1];
            request.@params.productIndexes[0] = new ApiProductsServiceGet.productIndexItemType();
            request.@params.productIndexes[0].productIndex = "LM0037";

            ApiProductsServiceGet.responseType response = client.get(request);

            textHistoria.Text += response.results.ToString();

        }

        private void kontrolaTowarowNaDokSprzedazyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RaportKontrolaIndeksow rap = new RaportKontrolaIndeksow(polaczenie, magID, magID2);
        }

        private void tToSkan_TextChanged(object sender, EventArgs e)
        {

        }

        private void listaZamówieńZWwwToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, logowanie.userName);
            ov.Pokaz();
        }

        private void label3_Click(object sender, EventArgs e)
        {

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

            cMagazyn2.DataSource = new BindingSource(listMagazyny, null);
            cMagazyn2.DisplayMember = "Value";
            cMagazyn2.ValueMember = "Key";
        }

        private void setSetingsOfStores()
        {
            magID = ((KeyValuePair<int, string>)cMagazyn.SelectedItem).Key;
            magNazwa = ((KeyValuePair<int, string>)cMagazyn.SelectedItem).Value;
            magKod = ((KeyValuePair<int, string>)cMagazyn.SelectedItem).Value.Substring(0, (((KeyValuePair<int, string>)cMagazyn.SelectedItem).Value.IndexOf(" ")));

            if (magID != ((KeyValuePair<int, string>)cMagazyn2.SelectedItem).Key)
            {
                magID2 = ((KeyValuePair<int, string>)cMagazyn2.SelectedItem).Key;
                magNazwa2 = ((KeyValuePair<int, string>)cMagazyn2.SelectedItem).Value;
                magKod2 = ((KeyValuePair<int, string>)cMagazyn2.SelectedItem).Value.Substring(0, (((KeyValuePair<int, string>)cMagazyn2.SelectedItem).Value.IndexOf(" ")));

            }
            else
            {
                magID2 = 0;
                magNazwa2 = "";
                magKod2 = "";
            }

            lKontekstPracyMagazyn.Text = "Praca z dokumentami w: " + magNazwa + "   Użytkownik:" + logowanie.userName;
        }
    }
}
