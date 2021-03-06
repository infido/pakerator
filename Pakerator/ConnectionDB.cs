﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System.Xml.Linq;
using System.ServiceModel;

namespace Pakerator
{
    public partial class ConnectionDB : Form
    {
        public static FbConnection conn;
        private string logUser, logMagNaz, logIP, logHost;
        private int logMagID;

        public ConnectionDB()
        {
            InitializeComponent();
            //wczytanie wartości do pól
            setConnection(false);
        }

        public void showForTest(string logUser, string logMagNaz, int logMagID, string logIP, string logHost)
        {
            this.logUser = logUser;
            this.logMagNaz = logMagNaz;
            this.logMagID = logMagID;
            this.logIP = logIP;
            this.logHost = logHost;
            Show();
        }

        private void setConnection(Boolean _trybTest)
        {
            conn = new FbConnection(getConnectionString());
            if (_trybTest) outtext.Text += "Ustawiono parametry połaczenia. " + DateTime.Now + System.Environment.NewLine;

            try
            {
                conn.Open();
                if (conn.State > 0)
                {
                    if (_trybTest)
                    {
                        outtext.Text += "Nawiązano połaczenie. " + conn.Database + " Status=" + conn.State + " " + DateTime.Now + System.Environment.NewLine;
                    }
                    else
                    {
                        outtext.Text += "Nawiązano połaczenie! " + conn.Database + " Status=" + conn.State + " " + DateTime.Now + System.Environment.NewLine;
                    }
                } 
                else
                {
                    if (_trybTest)
                    {
                        outtext.Text += "Nie połączono! Status=" + conn.State + " " + DateTime.Now + System.Environment.NewLine;
                    }
                    else
                    {
                        MessageBox.Show("Błąd połączenia z bazą!");
                        Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (_trybTest)
                {
                    outtext.Text += "Błąd: " + ex.Message + " " + DateTime.Now + System.Environment.NewLine;
                }
                else
                {
                    MessageBox.Show(ex.Message);
                    setSettingConnection();
                }
            }
        }
        private void setSettingConnection()
        {
            try
            {
                ShowDialog();
            }
            catch (FbException ex)
            {
                MessageBox.Show("Nieznamy błąd: " + ex.Message.ToString() );
                throw;
            }
        }

        public FbConnection getConnection()
        {
            return conn;
        }

        public void setConnectionOFF()
        {
            conn.Close();
            outtext.Text += "Rozłaczono! Status=" + conn.State + " " + DateTime.Now + System.Environment.NewLine;
        }

        private void Zapisz_Click(object sender, EventArgs e)
        {
            // dodanie obsługi zapisu do rejestru
            setConnectionStringToRegistry();
            // Visible = false;
        }

        private void Testuj_Click(object sender, EventArgs e)
        {
            setConnection(true);
        }

        private String getConnectionString()
        {
            String[] setloc = new String[6];
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            if (rejestr == null)
            {
                setConnectionStringToRegistry();
            }
            getConnectionSettingsFromRegistry();

            if (tUser.Text.Length > 0)
            {
                setloc[0] = "User=" + tUser.Text + ";";
            }
            else
            {
                setloc[0] = "User=SYSDBA;";
            };
            if (tPassword.Text.Length > 0)
            {
                setloc[1] = "Password=" + tPassword.Text + ";";
            }
            else
            {
                setloc[1] = "Password=masterkey;";
            };
            if (tPath.Text.Length > 0)
            {
                setloc[2] = "Database=" + tPath.Text + ";";
            }
            else
            {
                setloc[2] = "Database=C:\\Program Files\\Raks\\Data\\F00001.fdb;";
                setloc[2] = "Database=/usr/raks/Data/F00001.fdb;";
            };
            if (tServer.Text.Length > 0)
            {
                setloc[3] = "DataSource=" + tServer.Text + ";";
            }
            else
            {
                //setloc[3] = "DataSource=10.6.3.9;";
                //setloc[3] = "DataSource=localhost;";
                
                setloc[3] = "DataSource=127.0.0.1;";
                setloc[3] = "DataSource=10.0.0.100;";
            };
            if (tPort.Text.Length > 0)
            {
                setloc[4] = "Port=" + tPort.Text + ";";
            }
            else
            {
                setloc[4] = "Port=3050;";
            };
            if (instalacjaSieciowa.Checked)
            {
                setloc[5] = "ServerType=0";
            }
            else
            {
                //the embedded server
                setloc[5] = "ServerType=1";
            };

            string connectionString =
                setloc[0] +
                setloc[1] +
                setloc[2] +
                setloc[3] +
                setloc[4] +
                "Dialect=3;" +
                //"Charset=NONE;" +
                "Charset=WIN1250;" +
                "Role=;" +
                "Connection lifetime=15;" +
                "Pooling=true;" +
                "MinPoolSize=0;" +
                "MaxPoolSize=50;" +
                "Packet Size=8192;" +
                setloc[5];
            
            return connectionString;
        }

        private void setEnableFields()
        {
            if (instalacjaSieciowa.Checked)
            {
                tServer.Enabled = true;
                tPath.Enabled = true;
                tPort.Enabled = true;
            }
            else
            {
                tServer.Enabled = false;
                tPath.Enabled = false;
                tPort.Enabled = false;
            }
        }
        private void setConnectionStringToRegistry()
        {
            //wpis do rejestru ustawień połączenia
            try
            {
                RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator",true);
                if (rejestr == null)
                {
                    RegistryKey rejestrNew = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Infido\\Pakerator");
                    rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator", true);
                }

                rejestr.SetValue("User", tUser.Text);
                rejestr.SetValue("Pass", tPassword.Text);
                rejestr.SetValue("Path", tPath.Text);
                if (instalacjaSieciowa.Checked)
                {
                    rejestr.SetValue("Net", 1);
                }
                else
                {
                    rejestr.SetValue("Net", 0);
                }
                rejestr.SetValue("Serwer", tServer.Text);
                rejestr.SetValue("Path", tPath.Text);
                rejestr.SetValue("Port", tPort.Text);

                rejestr.SetValue("www1", tDomena.Text);
                rejestr.SetValue("www2", tKlucz.Text);
                rejestr.SetValue("www3", tLogin.Text);
            }
            catch (Exception ee)
            {
                    MessageBox.Show("Błąd zapisu do rejestru: " + ee.Message);
                    //throw;
            }
        }

        private void getConnectionSettingsFromRegistry()
        {
            //wpis do rejestru ustawień połączenia
            try
            {
                RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            if (rejestr == null)
            {
                setConnectionStringToRegistry();
            }

            tUser.Text = (String)rejestr.GetValue("User");
            tPassword.Text = (String)rejestr.GetValue("Pass");
            tPath.Text=(String)rejestr.GetValue("Path");
            //Console.WriteLine(">>" + rejestr.GetValue("Net"));
            if ((int)rejestr.GetValue("Net")==1)
            {
                instalacjaLokalna.Checked = false;
                instalacjaSieciowa.Checked = true;
            }
            else
            {
                instalacjaLokalna.Checked = true;
                instalacjaSieciowa.Checked = false;
            }
            tServer.Text = (String)rejestr.GetValue("Serwer");
            tPath.Text = (String)rejestr.GetValue("Path");
            tPort.Text = (String)rejestr.GetValue("Port");

            tDomena.Text = (String)rejestr.GetValue("www1");
            tKlucz.Text = (String)rejestr.GetValue("www2");
            tLogin.Text = (String)rejestr.GetValue("www3");
            }
            catch (Exception ee)
            {
                MessageBox.Show("Błąd odczytu z rejestru: " + ee.Message);
                //throw;
            }
        }

        private void instalacjaSieciowa_CheckedChanged(object sender, EventArgs e)
        {
            setEnableFields();
        }

        private void instalacjaLokalna_CheckedChanged(object sender, EventArgs e)
        {
            setEnableFields();
        }

        private void bRozlacz_Click(object sender, EventArgs e)
        {
            setConnectionOFF();
        }

        private void bClear_Click(object sender, EventArgs e)
        {
            outtext.Text = "";
        }

        private void bNewFDB_Click(object sender, EventArgs e)
        {
            if (conn.State > 0)
            {
                MessageBox.Show("Należy najpierw zamknąć aktywne połączenei do bazy!");
            }
            else
            {
                try
                {
                    //setConnectionStringToRegistry();
                    //FbEwispergi fdb = new FbEwispergi(getConnectionString());
                    //outtext.Text += fdb.createFDB() + System.Environment.NewLine;
                    MessageBox.Show("Funkcjonalność zablokowana!");
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Błąd: "+ ee.Message);
                    throw;
                }
            }

        }

        private void tPath_DoubleClick(object sender, EventArgs e)
        {
            tPath.Text = "C:\\Program Files\\Raks\\Data\\F00001.fdb";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void zapiszKonfiguracjęDoPlikuToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (saveFileDialog1.ShowDialog()==DialogResult.OK)
            {
                try
                {
                    XDocument xdoc = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XComment("Parametry aplikacji"),
                        new XElement("Konfiguracja",
                            new XElement("tUser", tUser.Text),
                            new XElement("tPassword", tPassword.Text),
                            new XElement("instalacjaLokalna", instalacjaLokalna.Checked),
                            new XElement("instalacjaSieciowa", instalacjaSieciowa.Checked),
                            new XElement("tServer", tServer.Text),
                            new XElement("tPath", tPath.Text),
                            new XElement("tPort", tPort.Text)
                         )
                    );
                    xdoc.Save(saveFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " Zostaną ustawione dane domyślne.");
                }
            }
        }

        private void otwórzKonfiguracjęZPlikuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
            {
                XDocument xDoc = XDocument.Load(openFileDialog1.FileName);
                tUser.Text = (String)xDoc.Root.Element("tUser");
                tPassword.Text = (String)xDoc.Root.Element("tPassword");
                instalacjaLokalna.Checked = Convert.ToBoolean((String)xDoc.Root.Element("instalacjaLokalna"));
                instalacjaSieciowa.Checked = Convert.ToBoolean((String)xDoc.Root.Element("instalacjaSieciowa"));
                tServer.Text = (String)xDoc.Root.Element("tServer");
                tPath.Text = (String)xDoc.Root.Element("tPath");
                tPort.Text = (String)xDoc.Root.Element("tPort");
            }
        }

        public int getConnectioState()
        {
            return (int)conn.State;
        }

        private void bCheckStock_Click(object sender, EventArgs e)
        {
            var binding = new BasicHttpBinding();
            var address = new EndpointAddress("http://" + SessionIAI.GetIAIDomainForCurrentSession() + "/api/?gate=checkserverload/checkServerLoad/106/soap");
            var client = new ServiceReferenceIAI.checkServerLoadPortTypeClient(binding, address);

            var request = new ServiceReferenceIAI.requestType();
            request.authenticate = new ServiceReferenceIAI.authenticateType();
            request.authenticate.system_key = SessionIAI.GetIAIKeyForCurrentSession();
            request.authenticate.system_login = SessionIAI.GetIAILoginForCurrentSession();

            ServiceReferenceIAI.responseType response = client.checkServerLoad(request);

            outtext.Text += "Test statusu serwera IAI Shop. " + DateTime.Now + System.Environment.NewLine;
            outtext.Text += response.serverLoadStatus.ToString() + " " + System.Environment.NewLine;

            setLogFromConn("Test statusu serwera IAI Shop. STATUS=" + response.serverLoadStatus.ToString());
        }

        private void setLogFromConn(string komunikat)
        {
            string sql = "INSERT INTO LOGSKAN ";
            sql += "(operacja, komunikat, pracownik, magazyn_nazwa, magazyn_id, ip, host) ";
            sql += " values ('SOAP-TEST', ";
            sql += "'" + komunikat + "',"; 
            sql += " '" + logUser + "','" + logMagNaz + "'," + logMagID + ",'" + logIP + "','" + logHost + "');";


            FbCommand cdk = new FbCommand(sql, conn);
            try
            {
                cdk.ExecuteNonQuery();
            }
            catch (FbException ex)
            {
                MessageBox.Show("Bład zapisu loga do bazy danych RaksSQL! " + ex.Message.ToString());
                throw;
            }
        }

        private void bGenerujTocken_Click(object sender, EventArgs e)
        {
            tTocken.Text = SessionIAI.GenerateKey(tKlucz.Text);
        }

        private void bSzyfruj_Click(object sender, EventArgs e)
        {
            tKlucz.Text = SessionIAI.HashPassword(tHasloWWW.Text);
        }

        public void setCurrUser(string currUsr)
        {
            logUser = currUsr;
        }

        public string getCurrentUser()
        {
            return logUser;
        }
    }

    public static class SessionIAI
    {
        public static bool GetPopertySettingsForAIA()
        {
            if (SessionIAI.GetIAIDomainForCurrentSession() != null && SessionIAI.GetIAILoginForCurrentSession() != null && SessionIAI.GetIAIKeyForCurrentSession() != null)
                return true;
            else
            {
                MessageBox.Show("Ustawienia konfiguracji połączenia do skepu internetowego IAI wygladają na błędne!");
                return false;
            }
        }

        public static string GetIAIKeyForCurrentSession()
        {
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            if (rejestr != null)
            {
                return SessionIAI.GenerateKey((String)rejestr.GetValue("www2"));
            }
            else
            {
                MessageBox.Show("Bład odczytu parametru klucz połączenia do sklepu IAI w kalsie połącznia.");
                return "";
            }
        }

        public static string GetIAILoginForCurrentSession()
        {
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            if (rejestr != null)
            {
                return (String)rejestr.GetValue("www3");
            }
            else
            {
                MessageBox.Show("Bład odczytu parametru login połączenia do sklepu IAI w kalsie połącznia.");
                return "";
            }
        }

        public static string GetIAIDomainForCurrentSession()
        {
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            if (rejestr != null)
            {
                return (String)rejestr.GetValue("www1");
            }
            else
            {
                MessageBox.Show("Bład odczytu parametru adres połączenia do sklepu IAI w kalsie połącznia.");
                return "";
            }
        }


        /// <summary>
        /// Generates SHA1 session key string
        /// </summary>
        /// <param name="password">SHA1 hashed password
        /// <returns>SHA1 hash string</returns>
        public static string GenerateKey(string hashedPassword)
        {
            System.Security.Cryptography.HashAlgorithm hash = System.Security.Cryptography.SHA1.Create();
            string date = System.String.Format("{0:yyyyMMdd}", System.DateTime.Now);
            string strToHash = date + hashedPassword;
            byte[] keyBytes, hashBytes;
            keyBytes = System.Text.Encoding.UTF8.GetBytes(strToHash);
            hashBytes = hash.ComputeHash(keyBytes);
            string hashedString = string.Empty;
            foreach (byte b in hashBytes)
            {
                hashedString += String.Format("{0:x2}", b);
            }
            return hashedString;
        }
        /// <summary>
        /// Hashes specified password with SHA1 algorithm
        /// </summary>
        /// <param name="password">User password
        /// <returns>SHA1 hash  string</returns>
        public static string HashPassword(string password)
        {
            System.Security.Cryptography.HashAlgorithm hash = System.Security.Cryptography.SHA1.Create();
            byte[] keyBytes, hashBytes;
            keyBytes = System.Text.Encoding.UTF8.GetBytes(password);
            hashBytes = hash.ComputeHash(keyBytes);
            string hashedString = string.Empty;
            foreach (byte b in hashBytes)
            {
                hashedString += String.Format("{0:x2}", b);
            }
            return hashedString;
        }
    }
}
