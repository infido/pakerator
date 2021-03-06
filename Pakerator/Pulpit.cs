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
using System.Net;
using System.ServiceModel;
using Pakerator.ApiProductsSocksServiceGet;
using Microsoft.Win32;
using Pakerator.ApiGetOrdersNotFinishedGet;
using System.Runtime.CompilerServices;

namespace Pakerator
{
    public partial class Pulpit : Form
    {
        public ConnectionDB polaczenie;
        private FbDataAdapter fda;
        private DataSet fds;
        private DataView fDataView;
        //private Login logowanie;
        int dokId = 0;
        int odbiorca = 0;
        int platnik = 0;
        private bool jestSkonczone = false;
        public int magID, magID2;
        public string magKod, magKod2;
        public string magNazwa, magNazwa2;
        Dictionary<int, string> listMagazyny;
        private bool czyBezObliczaniaStanow;
        private bool czyLogowanieByloNiepoprawneImamZamknacApp = false;
        private int curIDUsr;
        private string magazyny;

        public Pulpit()
        {
            InitializeComponent();
            Text = "Pakerator " + Application.ProductVersion;
            polaczenie = new ConnectionDB();
            
            //logowanie = new Login(polaczenie);
            //logowanie.ShowDialog();

            int tryLogin = 3;
            while (tryLogin > 0)
            {
                Autentykacja logToSys = new Autentykacja(polaczenie);
                if (logToSys.GetAutoryzationResult().Equals(AutoryzationType.Uzytkownik))
                {
                    logToSys.SetTimestampLastLogin();
                    polaczenie.setCurrUser(logToSys.GetCurrentUserLogin());
                    curIDUsr = logToSys.GetCurrentUserID();
                    magazyny = logToSys.GetMagazyny();
                    tryLogin = -1;
                    break;
                }
                else if (logToSys.GetAutoryzationResult().Equals(AutoryzationType.Administartor))
                {
                    logToSys.SetTimestampLastLogin();
                    polaczenie.setCurrUser(logToSys.GetCurrentUserLogin());
                    curIDUsr = logToSys.GetCurrentUserID();
                    magazyny = logToSys.GetMagazyny();
                    tryLogin = -1;
                    break;
                }
                tryLogin--;
            }

            if (tryLogin == -1)
            {
                setDictonary(magazyny);
                setCMagazynFromReg();
                setSetingsOfStores();

                if (magazyny.Contains("CENTR"))
                {
                    kontrolaTowarowNaDokSprzedazyToolStripMenuItemGorneMenu.Enabled = true;
                    zamówieniaNaPortaluWwwToolStripMenuItem.Enabled = true;
                    raportRozchodówZZamówieńWwwToolStripMenuItem.Enabled = true;
                }
                else
                {
                    kontrolaTowarowNaDokSprzedazyToolStripMenuItemGorneMenu.Enabled = false;
                    zamówieniaNaPortaluWwwToolStripMenuItem.Enabled = false;
                    raportRozchodówZZamówieńWwwToolStripMenuItem.Enabled = false;
                }
                setLog("ENTRY", "999 Logowanie do systemu Wersja:" + Application.ProductVersion + "; user: " + polaczenie.getCurrentUser() + "; ustawiono kontekst: " + magNazwa, "", "", "", 0, "");
                chkTableLOGSKAN();
            }
            else
            {
                MessageBox.Show("Nieudane logowanie do programu! Program zostanie zamkniety ", "Bład logowania");
                polaczenie.setConnectionOFF();
                czyLogowanieByloNiepoprawneImamZamknacApp = true;
            }
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
            
        }

        private void SetDokument(string kodKreskowy)
        {
            bool czyToJestListPrzewozowy = true; 
            string sql = "";
            int corectDocID = 0; 
            bool toJestMMP = true;

            if (cSkanFZ.Checked==false && kodKreskowy.Length > 0)
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

            if (cSkanFZ.Checked==true)
            {
                sql = "select ID, KOD, NUMER, NAZWA_DOKUMENTU as NAZWA_PELNA_PLATNIKA, NAZWA_PELNA_DOSTAWCY as NAZWA_PELNA_ODBIORCY,  ";
                sql += "COALESCE(ULICA_DOSTAWCY,'') as ULICA_ODBIORCY, COALESCE(NRDOMU_DOSTAWCY,'') as NRDOMU_ODBIORCY, COALESCE(NRLOKALU_DOSTAWCY,'') as NRLOKALU_ODBIORCY, COALESCE(MIEJSCOWOSC_DOSTAWCY,'') as MIEJSCOWOSC_ODBIORCY, ";
                sql += "COALESCE(PANSTWO_DOSTAWCY,'') as PANSTWO_ODBIORCY, COALESCE(KOD_DOSTAWCY,'') as KOD_ODBIORCY, OPERATOR, NR_FAKTURY_DOSTAWCY as SYGNATURA, COALESCE(UWAGI,'') as UWAGI, ID_DOSTAWCY as ID_ODBIORCY, 0 as ID_PLATNIKA  ";
                sql += "FROM GM_FZ ";
                sql += "where MAGNUM=" + magID + " and (NUMER='" + tToSkan.Text + "' OR NR_FAKTURY_DOSTAWCY='" + tToSkan.Text + "' OR  SYGNATURA='" + tToSkan.Text + "');";
            }
            else if (czyToJestListPrzewozowy)
            {
                sql = "select ID, KOD, NUMER, NAZWA_PELNA_PLATNIKA, NAZWA_PELNA_ODBIORCY, ";
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
                sql = "select GM_MM.ID, GM_MM.NUMER, GM_MM.KOD,";
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
                    ltypdok.Text = (string)fdk["KOD"];
                    lDokument.Text = (string)fdk["NUMER"];
                    lListPrzewozowy.Text = kodKreskowy;
                    platnik = (int)fdk["ID_PLATNIKA"];
                    lNabywcaTresc.Text = (string)fdk["NAZWA_PELNA_PLATNIKA"];
                    odbiorca = (int)fdk["ID_ODBIORCY"];
                    lOdbiorcaTresc.Text = (string)fdk["NAZWA_PELNA_ODBIORCY"] + Environment.NewLine + (string)fdk["PANSTWO_ODBIORCY"] + "; " + (string)fdk["KOD_ODBIORCY"] + " " + (string)fdk["MIEJSCOWOSC_ODBIORCY"] + Environment.NewLine;
                    lOdbiorcaTresc.Text += (string)fdk["ULICA_ODBIORCY"] + " " + (string)fdk["NRDOMU_ODBIORCY"] + " " + (string)fdk["NRLOKALU_ODBIORCY"];

                    //Tu wczytujemy pozycje dokumentu
                    if (cSkanFZ.Checked==true)
                    {
                        //Dal faktury zakupowej
                        sql = "select GM_FZPOZ.ID, GM_FZPOZ.LP, GM_TOWARY.TYP, GM_TOWARY.SKROT, COALESCE(GM_TOWARY.SKROT2,'') as SKROT2, COALESCE(GM_TOWARY.KOD_KRESKOWY,'') as KOD_KRESKOWY, GM_TOWARY.NAZWA, GM_FZPOZ.ILOSC_PO as ILOSC, 0 as SKANOWANE, COALESCE(GM_FZPOZ.ZNACZNIKI,'') as ZNACZNIKI, GM_FZPOZ.ID_TOWARU ";
                        sql += ", -1 STAN_" + magKod + " ";
                        if (magID2 != 0 && magID != magID2)
                            sql += " , -1 STAN_" + magKod2 + " ";
                        sql += " , -1 W_WYDANIU_" + magKod + " ";
                        sql += "from GM_FZPOZ ";
                        sql += "join GM_TOWARY ON GM_FZPOZ.ID_TOWARU=GM_TOWARY.ID ";
                        sql += "where GM_FZPOZ.ID_GLOWKI=" + dokId;
                    }
                    else if (czyToJestListPrzewozowy)
                    {
                        //Dla faktury
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

                    setLog("LOG", "006 Ustawienie dokumentu i odbiorcy, " + dataGridViewPozycje.Rows.Count + " pozycji" , kodKreskowy, kodKreskowy, lDokument.Text, ltypdok.Text);

                    dataGridViewPozycje.Columns["ID"].Visible = false;
                    dataGridViewPozycje.Columns["ID_TOWARU"].Visible = false;

                    if (ltypdok.Text.Contains("MMR") || ltypdok.Text.Contains("MMP"))
                    {
                        dataGridViewPozycje.Columns["STAN_" + magKod].Visible = false;
                        if (magID2 != 0 && magID != magID2)
                            dataGridViewPozycje.Columns["STAN_" + magKod2].Visible = false;
                        dataGridViewPozycje.Columns["W_WYDANIU_" + magKod].Visible = false;
                        kolorowanieRekordow(true);
                    }
                    else
                    {
                        kolorowanieRekordow();
                    }
                    
                    string tab = "";
                    if (cSkanFZ.Checked)
                        tab = "GM_FZ";
                    else if (czyToJestListPrzewozowy)
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

                    cdk = new FbCommand("UPDATE " + tab + " SET ZNACZNIKI=SUBSTRING('" + polaczenie.getCurrentUser() + " " + DateTime.Now.ToShortDateString() + " " +
                            DateTime.Now.ToShortTimeString() + ";' || COALESCE(ZNACZNIKI,'') FROM 1 FOR 249) where ID=" + dokId, polaczenie.getConnection());

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
                            
                            if (cSkanFZ.Checked)
                                setLog("ERROR", "0111 Dokument faktury zakupowej: " + lDokument.Text + " jest zablokowany przez innego użytkownika programu! Nie ustawiono informacji o rozpoczeciu skanowania!", tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                            else if (tab.Equals("GM_FS"))
                              setLog("ERROR", "0112 Dokument faktury/paragonu: " + lDokument.Text + " jest zablokowany przez innego użytkownika programu! Nie ustawiono informacji o rozpoczeciu skanowania!", tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                            else
                                setLog("ERROR", "012 Dokument przesunięcia międzymagazynowego: " + lDokument.Text + " jest zablokowany przez innego użytkownika programu! Nie ustawiono informacji o rozpoczeciu skanowania!", tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                        }
                        else
                        {
                            setLog("ERROR", "008 Bład zapytania przy aktualizacji znacznika na nagłówku dokumentu sprzedażowego: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                            throw;
                        }
                    }
                    odswieżPrzypisanieKodówKreskowychToolStripMenuItem.Enabled = true;
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
                    czyBezObliczaniaStanow = false;
                    zapiszHistoria("001 Wybór dokumentu >> List przewozowy: " + tToSkan.Text); 
                    SetDokument(tToSkan.Text);
                    tToSkan.Text = "";
                }
                else
                {
                    //skanowanie towaru
                    bool znalazlem = false;
                    czyBezObliczaniaStanow = true;
                    tToSkan.Text = tToSkan.Text.Trim();

                    //textHistoria.Text = "Towar: " + tToSkan.Text + Environment.NewLine + textHistoria.Text;
                    zapiszHistoria("002 Skanowanie towaru: " + tToSkan.Text);
                    foreach (DataGridViewRow row in dataGridViewPozycje.Rows)
                    {
                        if (row.Cells["KOD_KRESKOWY"].Value.Equals(tToSkan.Text) &&
                            (Convert.ToDecimal( row.Cells["SKANOWANE"].Value) < Convert.ToDecimal( row.Cells["ILOSC"].Value))
                            )
                        {

                            //zapis do bazy
                            try
                            {
                                row.Cells["SKANOWANE"].Value = (int)row.Cells["SKANOWANE"].Value + 1;
                            }
                            catch (Exception ein)
                            {
                                MessageBox.Show("Błąd zwiekszania wartości pola SKANOWANIE dla: " + row.Cells["SKROT"].Value + System.Environment.NewLine + ein.Message);
                                throw;
                            }
                            
                            //updatsw
                            FbCommand cdk = null;
                            try
                            {
                                if (ltypdok.Text.StartsWith("FS")  || ltypdok.Text.StartsWith("FH") || ltypdok.Text.StartsWith("PA"))
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
                                else if (ltypdok.Text.StartsWith("FZ") || ltypdok.Text.StartsWith("DI"))
                                {
                                    cdk = new FbCommand("UPDATE GM_FZPOZ SET ZNACZNIKI=" + row.Cells["SKANOWANE"].Value + " where ID=" + row.Cells["ID"].Value, polaczenie.getConnection());
                                }else
                                    MessageBox.Show("Próba skanowania dokumentu, z nieobsługiwanym typem numeracji: " + ltypdok.Text + System.Environment.NewLine + "Zgłoś problem do administartora systemu.", "Nieobsługiwany typ dokumentu");
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show("Błąd formułowania zapytania ustawiającego wartość pozycji po skanowaniu: " + row.Cells["SKROT"].Value + System.Environment.NewLine + exc.Message,"Błąd w programoie");
                                throw;
                            }
                            try
                            {
                                cdk.ExecuteNonQuery();
                                setLog("LOG", "011 Skanowanie towaru znajdującego sie na dokumencie. Ilość po skanowaniu " + (int)row.Cells["SKANOWANE"].Value, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, (int)row.Cells["ID_TOWARU"].Value, ltypdok.Text);
                            }
                            catch (FbException ex)
                            {
                                MessageBox.Show("012 Bład zapytania: " + ex.Message);
                                setLog("ERROR", "012 Bład zapytania: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                                throw;
                            }
                            znalazlem = true;
                            break;
                        }
                    }

                    if (znalazlem)
                    {
                        zapiszHistoria("003 Znalaleziono pozycję w loop, jest przed kolorowaniem : " + tToSkan.Text);
                        kolorowanieRekordow(czyBezObliczaniaStanow);
                        zapiszHistoria("004 Znalaleziono pozycję w loop, jest przed sprawdzeneim czy skończony dokument : " + tToSkan.Text);
                        sprawdzenieCzySkonczone();
                    }
                    else
                    {
                        zapiszHistoria("005 Nieznalaleziono pozycji w loop, jest przed ogłoszeniem : " + tToSkan.Text);
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

        public static void putLog(ConnectionDB conn ,string usrName, string typ, string tresc, string kodKreskowy, string listPrzewozowy, string nrDokumentu, int idTowaru, string typDok, int dokId, string magNazwa, int magID, string nabywcaTresc, int odbiorca, int platnik)
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
#if DEBUG
                MessageBox.Show("Bład zapisu do bazy log (LOGSKAN) " + System.Environment.NewLine + ex.Message);
#endif
                StreamWriter writer = new StreamWriter(Environment.GetEnvironmentVariable("temp") + "\\Pakerator_" + DateTime.Now.ToShortDateString() + ".log", true);
                try
                {
                    writer.WriteLine(DateTime.Now.ToString() + "; User:" + usrName + "; Kod kreskowy: " + kodKreskowy + "; ststus połączenia: " + conn.getConnectioState() + " Bład zapytania: " + ex.Message);
                    if (conn.getConnectioState() <= 0)
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
                sql += " ('" + polaczenie.getCurrentUser() + "','" + tToSkan.Text + "','" + lListPrzewozowy.Text + "',";
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
                writer.WriteLine(DateTime.Now.ToString() + "; User:" + polaczenie.getCurrentUser() + "; Kod kreskowy: " + tToSkan.Text + "; List przewozowy: " + lListPrzewozowy.Text + "; Dokument: " + lDokument.Text + "; ststus połączenia: " + polaczenie.getConnectioState() + " Bład zapytania: " + tekst);
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
            cSkanFZ.Checked = false;
            odswieżPrzypisanieKodówKreskowychToolStripMenuItem.Enabled = false;

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

        private void kolorowanieRekordow(bool trybMM=false)
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

                try
                {
                    if (trybMM == false)
                    {
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
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd wyliczania stanów przy kolorowaniu rekordów: " + ex.Message,"Błąd..." );
                    throw;
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
            sql += " left join GM_WZ on GM_WZ.ID_FS = GM_FS.ID ";
            sql += " left join GM_WZPOZ on GM_WZPOZ.ID_GLOWKI=GM_WZ.ID ";
            sql += " where GM_FS.MAGAZYNOWY=0 AND GM_FS.FISKALNY=0 ";
            sql += " AND GM_WZPOZ.ILOSC_PO is null ";
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
                try
                {
                    if (row.Cells["TYP"].Value.ToString().Equals("Towar") && jestSkonczone &&
                                Int32.Parse(row.Cells["SKANOWANE"].Value.ToString()) < Int32.Parse(row.Cells["ILOSC"].Value.ToString()))
                    {
                        jestSkonczone = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd przy sprawdzaniu czy dokument skończony: " + ex.Message);
                    throw;
                }
               
            }

            if (jestSkonczone)
            {
                //textHistoria.Text = "Dokument zakończony poprawnie: " + lDokument.Text + Environment.NewLine + textHistoria.Text;
                zapiszHistoria("003 Dokument zakończony poprawnie: " + lDokument.Text);
                FbCommand cdk = null;
                FbCommand controlCdk = null;
                if (ltypdok.Text.StartsWith("FZ") || ltypdok.Text.StartsWith("DI"))
                {
                    controlCdk = new FbCommand(" SELECT ZNACZNIKI from GM_FZ where ID=" + dokId + " WITH LOCK;", polaczenie.getConnection());
                    cdk = new FbCommand("UPDATE GM_FZ SET ZNACZNIKI='Przyjął:" + DateTime.Now.ToShortDateString() + " " +
                          DateTime.Now.ToShortTimeString() + "; ' || ZNACZNIKI where ID=" + dokId, polaczenie.getConnection());
                }
                else if (ltypdok.Text.StartsWith("FS") || ltypdok.Text.StartsWith("PA"))
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

                setLog("LOG", "003 Zakończenie skanowania pozycji dla dokumentu" , tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
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
            Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "LOG", "Ustawienie kontekstu magazynu 1 na " + cMagazyn.Text, "", "", "", 0,"", 0, magNazwa, magID, "", 0, 0);
        }

        private void setCMagazynFromReg()
        {
            try
            {
                RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
                if (rejestr.GetValue("Mag1Settings") != null)
                {
                    cMagazyn.SelectedValue = magID = (int)rejestr.GetValue("Mag1Settings");
                }

                if (rejestr.GetValue("Mag2Settings") != null)
                {
                    cMagazyn2.SelectedValue = magID2 = (int)rejestr.GetValue("Mag2Settings");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("NIeznany błąd przy ustawianiu wartości domyslnej Magazynu " + ex.Message,"Nieznany błąd");
            }
        }

        private void cMagazyn2_Leave(object sender, EventArgs e)
        {
            setSetingsOfStores();
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator", true);
            rejestr.SetValue("Mag2Settings", cMagazyn2.SelectedValue);
            Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "LOG", "Ustawienie kontekstu magazynu 2 na " + cMagazyn2.Text, "", "", "", 0, "", 0, magNazwa, magID, "", 0, 0);
        }

        private void menu2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SessionIAI.GetPopertySettingsForAIA())
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
                                kol[stock.stock_id] = Convert.ToDouble(size.quantity);
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

                Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "REPORT", "702 Wykonanie raportu Raport stanu na magazynach IAI, wyświetlono rekordów " + fdsr.Tables["TAB"].Rows.Count, "", lListPrzewozowy.Text, lDokument.Text, 0, ltypdok.Text, dokId, magNazwa, magID, lNabywcaTresc.Text, odbiorca, platnik);
                Raport rt = new Raport(fdsr);
                rt.Show();
            }
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
            
        }

        private void tToSkan_TextChanged(object sender, EventArgs e)
        {

        }

        private void listaZamówieńZWwwToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "new", " - Nowe", magNazwa);
            ov.Pokaz();
        }

        private void kontrolaTowarowNaDokSprzedazyToolStripMenuItemGorneMenu_Click(object sender, EventArgs e)
        {
            RaportKontrolaIndeksow rap = new RaportKontrolaIndeksow(polaczenie, magID, magID2);
        }

        private void listaZamówieńZWwwRealizowaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "on_order", " - Realizowane", magNazwa);
            ov.Pokaz();
        }

        private void listaZamówieńZWwwPakowaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "packed", " - Pakowane", magNazwa);
            ov.Pokaz();
        }

        private void listaZamówieńZWwwGotoweToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "ready", " - Gotowe", magNazwa);
            ov.Pokaz();
        }

        private void listaZamówieńZWwwZwrotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "returned", " - Zwrot", magNazwa);
            ov.Pokaz();
        }

        private void listaZamówieńZWwwKlientAnulowałToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "canceled", " - Klient anulował", magNazwa);
            ov.Pokaz();
        }

        private void listaZamówieńZWwwReklamacjeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "complainted", " - Reklamacje", magNazwa);
            ov.Pokaz();
        }

        private void listaZamówieńZWwwOczekująceNaWpłatęToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "payment_waiting", " - Oczekuje na włatę", magNazwa);
            ov.Pokaz();
        }

        private void listaZamówieńZWwwOczekująceNaDostawęToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "delivery_waiting", " - Oczekujące na dostawę", magNazwa);
            ov.Pokaz();
        }

        private void listaZamówieńZWwwWstrzymaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "suspended", " - Wstrzymane", magNazwa);
            ov.Pokaz();
        }

        private void listaZamówieńZWwwRealizowaneWProgramieFKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "finished_ext", " - Realizowane w programie F/K", magNazwa);
            ov.Pokaz();
        }

        private void roboczaListaZamówieńZWwwToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "", " - Wybrane statusy", magNazwa);
            ov.Pokaz();
        }

        private void odswieżPrzypisanieKodówKreskowychToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewPozycje.Rows)
            {
                FbCommand cdk = new FbCommand("SELECT KOD_KRESKOWY FROM GM_TOWARY WHERE SKROT='" +  row.Cells["SKROT"].Value.ToString() + "';" , polaczenie.getConnection());
                try
                {
                    var wy = cdk.ExecuteScalar();
                    if (wy!=null)
                    {
                        row.Cells["KOD_KRESKOWY"].Value = wy.ToString();
                    }
                }
                catch (FbException ex)
                {
                    MessageBox.Show("0112 Błąd zapytania o nowy kod kreskowy przuy skanowaniu dokumentu");
                    setLog("ERROR", "0112 Błąd zapytania o nowy kod kreskowy przuy skanowaniu dokumentu: " + ex.Message, tToSkan.Text, lListPrzewozowy.Text, lDokument.Text, ltypdok.Text);
                }
            }
        }

        private void dlaOstatnich7DniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wyczyśćToolStripMenuItem.PerformClick();
            DokumentyDlaMagazynu dm = new DokumentyDlaMagazynu(polaczenie, magID, magKod, magID2, magKod2,true);
            tToSkan.Text = dm.getKodDokumentuFromUser();
            dm.Dispose();
            if (tToSkan.Text.Length > 0)
                SendKeys.SendWait("{ENTER}");
        }

        private void wszystkieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wyczyśćToolStripMenuItem.PerformClick();
            DokumentyDlaMagazynu dm = new DokumentyDlaMagazynu(polaczenie, magID, magKod, magID2, magKod2,false);
            tToSkan.Text = dm.getKodDokumentuFromUser();
            dm.Dispose();
            if (tToSkan.Text.Length > 0)
                SendKeys.SendWait("{ENTER}");
        }

        private void listaZamówieńZWwwNieobsłużonePakowaneRealizowaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrdersView ov = new OrdersView(magID, magID2, polaczenie, polaczenie.getCurrentUser(), "pnr", " - Nieobsłużone, Realizowane, Pakowane", magNazwa);
            ov.Pokaz();
        }

        private void raportRozchodówZZamówieńWwwToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void getRaportRozchodowWgZamowienZwww(int dniWstecz, int odGodziny)
        {
            if (SessionIAI.GetPopertySettingsForAIA())
            {

                var binding = new BasicHttpBinding();
                var address = new EndpointAddress("http://" + SessionIAI.GetIAIDomainForCurrentSession() + "/api/?gate=orders/getOrdersNotFinished/106/soap");
                var client = new ApiGetOrdersNotFinishedGet.ApiOrdersPortTypeClient(binding, address);
                binding.MaxReceivedMessageSize = 2000000; //2MB

                var request = new ApiGetOrdersNotFinishedGet.requestType();

                request.authenticate = new ApiGetOrdersNotFinishedGet.authenticateType();
                request.authenticate.userLogin = SessionIAI.GetIAILoginForCurrentSession();
                request.authenticate.authenticateKey = SessionIAI.GetIAIKeyForCurrentSession();

                request.@params = new ApiGetOrdersNotFinishedGet.paramsType();


                request.@params.ordersStatuses = new string[3];
                request.@params.ordersStatuses[0] = "on_order";
                request.@params.ordersStatuses[1] = "new";
                request.@params.ordersStatuses[2] = "packed";


                request.@params.ordersRange = new ApiGetOrdersNotFinishedGet.ordersRangeType();
                request.@params.ordersRange.ordersDateRange = new ApiGetOrdersNotFinishedGet.ordersDateRangeType();
                request.@params.ordersRange.ordersDateRange.ordersDateType = ApiGetOrdersNotFinishedGet.ordersDateTypeType.add;
                request.@params.ordersRange.ordersDateRange.ordersDateTypeSpecified = true;
                request.@params.ordersRange.ordersDateRange.ordersDatesTypes = new ApiGetOrdersNotFinishedGet.ordersDatesTypeType[1];
                request.@params.ordersRange.ordersDateRange.ordersDatesTypes[0] = ApiGetOrdersNotFinishedGet.ordersDatesTypeType.add;

                request.@params.ordersRange.ordersDateRange.ordersDateBegin = DateTime.Now.AddDays(-dniWstecz).ToString("yyyy-MM-dd " + odGodziny.ToString("00") + ":00:00");
                request.@params.ordersRange.ordersDateRange.ordersDateEnd = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");

                request.@params.resultsLimit = 300;
                request.@params.resultsLimitSpecified = true;

                DataSet fdsr = new DataSet();
                fdsr.Tables.Add("TAB");
                fdsr.Tables["TAB"].Columns.Add("SKROT", typeof(String));
                fdsr.Tables["TAB"].Columns.Add("NAZWA", typeof(String));
                fdsr.Tables["TAB"].Columns.Add("STAN", typeof(Double));
                fdsr.Tables["TAB"].Columns.Add("ROZCHOD", typeof(Double));
                fdsr.Tables["TAB"].Columns.Add("W_WYDANIU", typeof(Double)); //To co zarejestrowane
                fdsr.Tables["TAB"].Columns.Add("BILANS", typeof(Double));

                Dictionary<string, int> rozchody = new Dictionary<string, int>();
                Dictionary<string, int> w_wydaniu = new Dictionary<string, int>();
                Dictionary<string, Double> stan = new Dictionary<string, Double>();
                Dictionary<string, string> nazwa = new Dictionary<string, string>();

                try
                {
                    ApiGetOrdersNotFinishedGet.responseType response = client.getOrdersNotFinished(request);

                    if (response.errors.faultCode != 0)
                    {
                        MessageBox.Show("1023 Błąd pobrania danych o zamówienia z IAI: " + response.errors.faultString);
                    }
                    else
                    {
                        foreach (ApiGetOrdersNotFinishedGet.ResultType www in response.Results)
                        {
                            foreach (ApiGetOrdersNotFinishedGet.productResultType pozwww in www.orderDetails.productsResults)
                            {
                                if (www.orderDetails.apiFlag == apiFlagType.registered_pos)
                                {
                                    if (w_wydaniu.ContainsKey(pozwww.productSizeCodeExternal))
                                        w_wydaniu[pozwww.productSizeCodeExternal] += (int)pozwww.productQuantity;
                                    else
                                    {
                                        w_wydaniu.Add(pozwww.productSizeCodeExternal, (int)pozwww.productQuantity);
                                        rozchody.Add(pozwww.productSizeCodeExternal, 0);
                                        nazwa.Add(pozwww.productSizeCodeExternal, pozwww.productName);
                                        stan.Add(pozwww.productSizeCodeExternal, getStanRaksZMagazynu(pozwww.productSizeCodeExternal));
                                    }
                                }
                                else
                                {
                                    if (rozchody.ContainsKey(pozwww.productSizeCodeExternal))
                                        rozchody[pozwww.productSizeCodeExternal] += (int)pozwww.productQuantity;
                                    else
                                    {
                                        rozchody.Add(pozwww.productSizeCodeExternal, (int)pozwww.productQuantity);
                                        w_wydaniu.Add(pozwww.productSizeCodeExternal, 0);
                                        nazwa.Add(pozwww.productSizeCodeExternal, pozwww.productName);
                                        stan.Add(pozwww.productSizeCodeExternal, getStanRaksZMagazynu(pozwww.productSizeCodeExternal));
                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("1024 Nieznany błąd pobrania danych o zamówienia z IAI: " + ex.Message);
                    throw;
                }


                foreach (var key in rozchody.Keys)
                {
                    fdsr.Tables["TAB"].Rows.Add(key, nazwa[key], (Double)stan[key], (Double)rozchody[key], (Double)w_wydaniu[key], (Double)stan[key] - (Double)rozchody[key] - (Double)w_wydaniu[key]);
                }

                Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "REPORT", "705 Wykonanie raportu Raport rozchodów na magazynach wg IAI, wyświetlono rekordów " + fdsr.Tables["TAB"].Rows.Count, "", lListPrzewozowy.Text, lDokument.Text, 0, ltypdok.Text, dokId, magNazwa, magID, lNabywcaTresc.Text, odbiorca, platnik);
                Raport rt = new Raport(fdsr);
                rt.Show();
            }
        }

        private Double getStanRaksZMagazynu(string indexExternal)
        {
            string sql = "SELECT sum(ILOSC) from GM_MAGAZYN join GM_TOWARY on ID_TOWAR=GM_TOWARY.ID_TOWARU ";
            sql += " where ";
            sql += " ( GM_TOWARY.SKROT='" + indexExternal + "' OR GM_TOWARY.SKROT2='" + indexExternal + "' )";
            if (magID != magID2)
                sql += " and (GM_MAGAZYN.MAGNUM = " + magID + " or GM_MAGAZYN.MAGNUM = " + magID2 + ");";
            else
                sql += " and GM_MAGAZYN.MAGNUM = " + magID + " ;";

            FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());

            try
            {
                var im = cdk.ExecuteScalar();
                if (im != DBNull.Value)
                    return Convert.ToDouble(im);
                else
                    return 0;
            }
            catch (FbException ex)
            {
                Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "1034 Bład zapytania o stany magazynu przy przeliczaniu raportu rozchodów dla towaru: " + indexExternal + System.Environment.NewLine + ex.Message, "", "", "", 0, "", 0, "", magID, "", 0, 0);
                MessageBox.Show("1034 Bład zapytania o stany magazynu przy przeliczaniu raportu rozchodów dla towaru: " + indexExternal + System.Environment.NewLine + ex.Message);
                return -2;
            }
        }

        private void stanDzisiajPo1400ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(0, 14);
        }

        private void stanDzisiajPo900ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(0, 9);
        }

        private void odWczorajPo900ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(1, 9);
        }

        private void stanDzisiajPo1200ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(0, 12);
        }

        private void stanDzisiajPo1600ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(0, 16);
        }

        private void odWczorajPo1600ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(1, 16);
        }

        private void odPrzedwczorajPo1600ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(2, 16);
        }

        private void odPrzedwczorajPo900ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(2, 16);
        }

        private void ostatanie7DniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(7, 0);
        }

        private void Pulpit_Load(object sender, EventArgs e)
        {
            if (czyLogowanieByloNiepoprawneImamZamknacApp)
                Application.Exit();
        }

        private void konfiguracjaPołaczeniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            polaczenie.showForTest(polaczenie.getCurrentUser(), magNazwa, magID, getIpAdress(), Dns.GetHostName());
        }

        private void zmianaHasłaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Autentykacja at = new Autentykacja(polaczenie, curIDUsr);
            if (at.SetNewPassByUser() == AutoryzationType.PassChanged)
            {
                MessageBox.Show("Zmiana przeprowadzona prawidłowo", "Zmiana hasła");
            }
            else
            {
                MessageBox.Show("Zmianę hasła anulowano!", "Zmiana hasła");
            }
        }

        private void od3DniTemuPo1600ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(3, 16);
        }

        private void od4DniTemuPo1600ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(4, 16);
        }

        private void od5DniTemuPo1600ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(5, 16);
        }

        private void od6DniTemuPo1600ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(6, 16);
        }

        private void ostatnie14DniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(14, 0);
        }

        private void ostatnie21DniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRaportRozchodowWgZamowienZwww(21, 0);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void setDictonary(string listaMagazynow)
        {
            listMagazyny = new Dictionary<int, string>();
            FbCommand cdk = new FbCommand("SELECT ID, NUMER || ' ' || NAZWA AS NAZWA, NUMER FROM GM_MAGAZYNY WHERE ARCHIWALNY=0", polaczenie.getConnection());
            try
            {
                FbDataReader fdk = cdk.ExecuteReader();
                while (fdk.Read())
                {
                    string curMagazyn = (string)fdk["NUMER"];
                    if (listaMagazynow.Contains(curMagazyn))
                        listMagazyny.Add((int)fdk["ID"], (string)fdk["NAZWA"]);
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("Błąd wczytywania listy magazynów: " + ex.Message);
            }

            try
            {
                cMagazyn.DataSource = new BindingSource(listMagazyny, null);
                cMagazyn.DisplayMember = "Value";
                cMagazyn.ValueMember = "Key";
            }
            catch (Exception em1)
            {
                MessageBox.Show("Nieznany błąd ustawienienia domyślnego magazynu 1. Możliwe, że nastąpiła zmiana uprawnień do magazynów. Jeżeli komunikat będzie się powtarzał skontaktuj się z Administratorem. " + em1.Message,"Nieznany błąd");
                //throw;
            }

            try
            {
                cMagazyn2.DataSource = new BindingSource(listMagazyny, null);
                cMagazyn2.DisplayMember = "Value";
                cMagazyn2.ValueMember = "Key";
            }
            catch (Exception em2)
            {
                MessageBox.Show("Nieznany błąd ustawienienia domyślnego magazynu 2. Możliwe, że nastąpiła zmiana uprawnień do magazynów. Jeżeli komunikat będzie się powtarzał skontaktuj się z Administratorem. " + em2.Message, "Nieznany błąd");
                //throw;
            }
        }

        private void setSetingsOfStores()
        {
            try
            {
                magID = ((KeyValuePair<int, string>)cMagazyn.SelectedItem).Key;
                magNazwa = ((KeyValuePair<int, string>)cMagazyn.SelectedItem).Value;
                magKod = ((KeyValuePair<int, string>)cMagazyn.SelectedItem).Value.Substring(0, (((KeyValuePair<int, string>)cMagazyn.SelectedItem).Value.IndexOf(" ")));

                magID2 = ((KeyValuePair<int, string>)cMagazyn2.SelectedItem).Key;
                magNazwa2 = ((KeyValuePair<int, string>)cMagazyn2.SelectedItem).Value;
                magKod2 = ((KeyValuePair<int, string>)cMagazyn2.SelectedItem).Value.Substring(0, (((KeyValuePair<int, string>)cMagazyn2.SelectedItem).Value.IndexOf(" ")));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd przy zapamiętywaniu bieżących wybranych magazynów..." + ex.Message,"Nieznany błąd");
            }

            lKontekstPracyMagazyn.Text = "Praca z dokumentami w: " + magNazwa + "   Użytkownik:" + polaczenie.getCurrentUser();
        }
    }
}
