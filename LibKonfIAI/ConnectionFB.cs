using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LibKonfIAI
{
    public class ConnectionFB
    {
        public FbConnection conn;
        private string logUser, logMagNaz, logIP, logHost;
        private int logMagID;
        private string IAIdomena, IAIklucz, IAIlogin;

        public ConnectionFB(string appUser, bool CzyRaks3000=false)
        {
            logUser = appUser;
            setConnection(CzyRaks3000);
            getIAISettingsFromRegistry();
        }
        private void setConnection(bool czyRaks3000)
        {
            conn = new FbConnection(getConnectionString(czyRaks3000));
            setErrOrLogMsg("LOG", "Ustawiono parametry połaczenia. " + DateTime.Now);

            try
            {
                conn.Open();
                if (conn.State > 0)
                {
                    setErrOrLogMsg("LOG", "Nawiązano połaczenie. " + conn.Database + " Status=" + conn.State + " " + DateTime.Now);

                }
                else
                {
                    setErrOrLogMsg("ERROR", "Nie połączono! Status=" + conn.State + " " + DateTime.Now);
                    
                }
            }
            catch (Exception ex)
            {
                setErrOrLogMsg("ERROR", "Błąd(Wyjątek) przy łączeniu do bazy FB: " + ex.Message + " " + DateTime.Now);
            }
        }

        public FbConnection getConnection()
        {
            return conn;
        }

        public void setConnectionOFF()
        {
            conn.Close();
        }
        private String getConnectionString(bool Raks3000)
        {
            String[] setloc = new String[6];
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            if (rejestr == null)
            {
                setErrOrLogMsg("ERROR", "Brak konfiguracji połączenia z bazą FB");
            }

            if (((String)rejestr.GetValue("User")).Length > 0)
            {
                setloc[0] = "User=" + (String)rejestr.GetValue("User") + ";";
            }
            else
            {
                setloc[0] = "User=SYSDBA;";
            };
            if (((String)rejestr.GetValue("Pass")).Length > 0)
            {
                setloc[1] = "Password=" + (String)rejestr.GetValue("Pass") + ";";
            }
            else
            {
                setloc[1] = "Password=masterkey;";
            };
            if (((String)rejestr.GetValue("Path")).Length > 0)
            {
                if (Raks3000)
                {
                    string r3 = (String)rejestr.GetValue("Path");
                    r3 = r3.Substring(0, r3.LastIndexOf("F000"));
                    setloc[2] = "Database=" + r3 + "Raks3000.fdb;";
                }
                else
                {
                    setloc[2] = "Database=" + (String)rejestr.GetValue("Path") + ";";
                }
            }
            else
            {
                if (Raks3000)
                {
                    setloc[2] = "Database=/usr/raks/Data/Raks3000.fdb;";
                }
                else
                {
                    setloc[2] = "Database=C:\\Program Files\\Raks\\Data\\F00001.fdb;";
                    setloc[2] = "Database=/usr/raks/Data/F00001.fdb;";
                }
            };
            if (((String)rejestr.GetValue("Serwer")).Length > 0)
            {
                setloc[3] = "DataSource=" + (String)rejestr.GetValue("Serwer") + ";";
            }
            else
            {
                setloc[3] = "DataSource=127.0.0.1;";
                setloc[3] = "DataSource=10.0.0.100;";
            };
            if (((String)rejestr.GetValue("Port")).Length > 0)
            {
                setloc[4] = "Port=" + (String)rejestr.GetValue("Port") + ";";
            }
            else
            {
                setloc[4] = "Port=3050;";
            };
            setloc[5] = "ServerType=0";
            

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

        private void getIAISettingsFromRegistry()
        {
            //wpis do rejestru ustawień połączenia
            try
            {
                RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
                if (rejestr != null)
                {
                    IAIdomena = (String)rejestr.GetValue("www1");
                    IAIklucz = (String)rejestr.GetValue("www2");
                    IAIlogin = (String)rejestr.GetValue("www3");
                }
            }
            catch (Exception ee)
            {
                setErrOrLogMsg("ERROR","Błąd odczytu z rejestru: " + ee.Message);
                throw;
            }
        }

        public string GetLoggedUser()
        {
            return logUser;
        }

        public static void setErrOrLogMsg(string typ,string msg, string appName="PakeratorDLL")
        {
            SaveLogMsgInToLogEvent(appName, typ, msg);
        }

        private static void SaveLogMsgInToLogEvent(string appName, string type, string msg)
        {
            EventLog appLog = new System.Diagnostics.EventLog();
            try
            {
                appLog.Source = CreateEventSource(appName);
                appLog.WriteEntry(msg);
                appLog.Log = type;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Nie można zapisać do Windows events następującego komunikatu: " + msg + System.Environment.NewLine + "Zgłaszany błąd: " + ex.Message);
            }
        }

        private static string CreateEventSource(string currentAppName)
        {
            string eventSource = currentAppName;
            bool sourceExists;
            try
            {
                // searching the source throws a security exception ONLY if not exists!
                sourceExists = EventLog.SourceExists(eventSource);
                if (!sourceExists)
                {   // no exception until yet means the user as admin privilege
                    EventLog.CreateEventSource(eventSource, "Application");
                }
            }
            catch (SecurityException errSE)
            {
                eventSource = "Application";
                writeLogToLocalFile("ERROR", "Błąd zapisu EventLog Windows; " + errSE.Message);
            }

            return eventSource;
        }

        private static void writeLogToLocalFile(string type, string msg)
        {
            StreamWriter writer = null;

            
            try
            {
                if (Directory.Exists("c:\\imex"))
                {
                    writer = new StreamWriter(@"C:\imex\Pakerator.log", true);
                }
                else
                {
                    writer = new StreamWriter(Environment.GetEnvironmentVariable("temp") + "\\Pakerator.log", true);
                }
                writer.WriteLine(DateTime.Now.ToString() + ";" + type + "; Komunikat: " + msg);
            }
            catch (Exception)
            {
                writer = new StreamWriter(Environment.GetEnvironmentVariable("temp") + "\\Pakerator.log", true);
                writer.WriteLine(DateTime.Now.ToString() + ";" + type + "; Komunikat: " + msg);
                //throw;
            }
            finally
            {
                writer.Close();
            }
        }
    }
}
