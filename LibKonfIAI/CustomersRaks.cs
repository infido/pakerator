using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibKonfIAI
{
    class CustomersRaks
    {
        private int magID, magID2;
        private string usrNam;
        //private ConnectionFB polaczenieFB;

        public int GetOrCreateRaksCustomerID(ConnectionFB polaczenieFB, string kodKontrah, string pelnaNazwa, string nip, string kodEU, string kodWal, string ulica, string nrBudynku, string miejscowosc, string kraj, string kodPoczta, string poczta, string email)
        {
            int cid = 0;

            FbCommand sp = new FbCommand("SELECT ID from R3_CONTACTS where SHORT_NAME='" + kodKontrah + "';", polaczenieFB.getConnection());
            try
            {
                FbDataReader fdk = sp.ExecuteReader();
                if (fdk.Read())
                {
                    cid = (int)fdk["ID"];
                }
                fdk.Close();
            }
            catch (FbException exgen)
            {
                //logg.setUstawienieLoga(Logg.RodzajLogowania.Error, Logg.MediumLoga.File, "Bład odnajdywania lub zakładania kontrahenta w RaksSQL: " + exgen.Message, true);
                //kh_id = 1;
            };

            if (cid == 0)
            {
                FbCommand gen_id_kh = new FbCommand("SELECT GEN_ID(R3_CONTACTS_ID_GEN,1) from rdb$database", polaczenieFB.getConnection());
                try
                {
                    cid = Convert.ToInt32(gen_id_kh.ExecuteScalar());
                }
                catch (FbException exgen)
                {
                    //logg.setUstawienieLoga(Logg.RodzajLogowania.Error, Logg.MediumLoga.File, "Bład pobrania generatora kontrahenta w Raks: " + exgen.Message, true);
                    throw;
                }

                string sql = "insert into R3_CONTACTS (ID,SHORT_NAME, FULL_NAME, TAXID, EU_CODE, CURRENCY_CODE, STREET, BUILDING_NUMBER, PLACE, COUNTRY, ZIPCODE, POCZTA, C_IDENT, M_IDENT, C_DATE, PURCHASER, GUID) ";
                sql += "values (" + cid + ", ";
                sql += "'" + kodKontrah + "', ";
                sql += "'" + (pelnaNazwa.Length > 199 ? pelnaNazwa.Substring(0, 199) : pelnaNazwa) + "', ";
                sql += "'" + (nip.Length > 24 ? nip.Substring(0, 24) : nip) + "', ";
                sql += "'" + (kodEU.Length > 2 ? kodEU.Substring(0, 2) : kodEU) + "', ";
                sql += "'" + (kodWal.Length > 3 ? kodWal.Substring(0, 3) : kodWal) + "', ";
                sql += "'" + (ulica.Length > 39 ? ulica.Substring(0, 39) : ulica) + "', ";
                sql += "'" + (nrBudynku.Length > 9 ? nrBudynku.Substring(0, 9) : nrBudynku) + "', ";
                sql += "'" + (miejscowosc.Length > 39 ? miejscowosc.Substring(0, 39) : miejscowosc) + "', ";
                sql += "'" + (kraj.Length > 39 ? kraj.Substring(0, 39) : kraj) + "', ";
                sql += "'" + (kodPoczta.Length > 9 ? kodPoczta.Substring(0, 9) : kodPoczta) + "', ";
                sql += "'" + (poczta.Length > 39 ? poczta.Substring(0, 39) : poczta) + "', ";
                sql += "'IAI', ";
                sql += "'IAI', ";
                sql += "'" + DateTime.Now.ToString("dd.MM.yyyy, HH.mm.ss.fff") + "', ";
                sql += "1, ";
                sql += "'" + Guid.NewGuid() + "') ";

                FbCommand new_kh = new FbCommand(sql, polaczenieFB.getConnection());
                try
                {
                    new_kh.ExecuteScalar();
                    //logg.setUstawienieLoga(Logg.RodzajLogowania.Info, Logg.MediumLoga.File, "Dodano nowego kontrahenta w RaksSQL: " + kodPresta + " ," + pelnaNazwa, true);
                }
                catch (FbException exgen)
                {
                    //logg.setUstawienieLoga(Logg.RodzajLogowania.Error, Logg.MediumLoga.File, "Bład pobrania generatora kontrahenta w Raks: " + exgen.Message, true);
                    throw;
                }
            }
            else
            {
                //logg.setUstawienieLoga(Logg.RodzajLogowania.Info, Logg.MediumLoga.File, "Znaleziono kontrahenta w RaksSQL: " + kodPresta + " ," + pelnaNazwa, true);
            }

            return cid;
        }

        public int GetSprzedazDetalicznaCustomerID(ConnectionFB polaczenieFB)
        {
            int cid = 0;

            FbCommand sp = new FbCommand("SELECT ID from R3_CONTACTS where SHORT_NAME='Sprzedaż detaliczna';", polaczenieFB.getConnection());
            try
            {
                FbDataReader fdk = sp.ExecuteReader();
                if (fdk.Read())
                {
                    cid = (int)fdk["ID"];
                }
                fdk.Close();
            }
            catch (FbException exgen)
            {
                //logg.setUstawienieLoga(Logg.RodzajLogowania.Error, Logg.MediumLoga.File, "Bład odnajdywania lub zakładania kontrahenta w RaksSQL: " + exgen.Message, true);
                //kh_id = 1;
            };
            return cid;
        }
    }
}
