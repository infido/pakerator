using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LibKonfIAI
{
    public class RaksService 
    {
        public static string saveNewOrderAsInvoiceToRaks(ConnectionFB polaczenieFB,string orderIdIAI, int magId)
        {
            string komunikatZwrotny = "";

            if (DataSessionIAI.GetPopertySettingsForAIA())
            {
                var binding = new BasicHttpBinding();
                var address = new EndpointAddress("http://" + DataSessionIAI.GetIAIDomainForCurrentSession() + "/api/?gate=orders/get/107/soap");
                var client = new ApiOrdersServiceGet.ApiOrdersPortTypeClient(binding, address);

                var request = new ApiOrdersServiceGet.requestType();

                request.authenticate = new ApiOrdersServiceGet.authenticateType();
                request.authenticate.userLogin = DataSessionIAI.GetIAILoginForCurrentSession();
                request.authenticate.authenticateKey = DataSessionIAI.GetIAIKeyForCurrentSession();

                request.@params = new ApiOrdersServiceGet.paramsType();

                request.@params.ordersIds = new string[1];
                request.@params.ordersIds[0] = orderIdIAI;
                //request.@params.ordersSerialNumbers = new int[1];
                //request.@params.ordersSerialNumbers[0] = 1;

                try
                {
                    ApiOrdersServiceGet.responseType response = client.get(request);

                    if (response.errors.faultCode != 0)
                    {
                        komunikatZwrotny += "1001 Kod: " + response.errors.faultCode + "; Opis:" +  response.errors.faultString + " ";
                        ConnectionFB.setErrOrLogMsg("ERROR", "Błąd wczytania zamówienia " + orderIdIAI + " do biblioteki zapisu nowego zamówienia jako GM_FS do Raks." + System.Environment.NewLine + komunikatZwrotny);
                    }
                    else
                    {
                        if (response.resultsNumberAll == 0)
                        {
                            komunikatZwrotny += "1003 INFO Nie pobrano zamówienia " + orderIdIAI + " ze sklepu IAI - zamówienie nie istnieje! ";
                            ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek wczytania zamówienia " + orderIdIAI + " do biblioteki zapisu nowego zamówienia jako GM_FS do Raks." + System.Environment.NewLine + komunikatZwrotny);
                        }
                        else
                        {
                            //Sprawdzić jeszcze raz czy mamy wszystkie indexy z pozycji i czy nie są puste
                            bool indeksyNaPozycjachSaOK = true;
//TODO: dorobić sprawdzenie pozycji  

                            //mamy zamówienie można działać dalej
                            if (indeksyNaPozycjachSaOK)
                            {
                                int fsid = 0;
                                FbCommand gen_id_fs = new FbCommand("SELECT GEN_ID(GM_FS_GEN,1) from rdb$database", polaczenieFB.getConnection());
                                try
                                {
                                    fsid = Convert.ToInt32(gen_id_fs.ExecuteScalar());
                                }
                                catch (FbException exgen)
                                {
                                    komunikatZwrotny += "1006 Bład pobrania id z generatora paragonu/faktury w Raks: dla orderIdIAI: " + orderIdIAI;
                                    ConnectionFB.setErrOrLogMsg("ERROR", "Błąd w zamówieniu do zapisania w RaksSQL: " + exgen.Message + System.Environment.NewLine + komunikatZwrotny);
                                    throw;
                                }

                                if (response.Results[0].orderDetails.clientRequestInvoice.Equals("n"))
                                {
                                    //sprzesaż na paragon
                                    string sql = "INSERT INTO GM_FS (ID,MAGNUM,TYP_DOK_MAGAZYNOWEGO,KOD,ROK,MIESIAC,NR,NUMER,SPOSOB_LICZENIA,ID_WALUTY,KURS,NAZWA_DOKUMENTU, ";
                                    sql += " ID_PLATNIKA, ID_ODBIORCY,NAZWA_SKROCONA_PLATNIKA,NAZWA_PELNA_PLATNIKA,NAZWA_SKROCONA_ODBIORCY,NAZWA_PELNA_ODBIORCY,KOD_KRESKOWY_PLATNIKA, ";
                                    sql += " WARTOSC_ZAKUPU_KAUCJ,WAL_WARTOSC_KAUCJ,PLN_WARTOSC_KAUCJ,OPERATOR,ZMIENIL,SYGNATURA,ZNACZNIKI,MAGAZYNOWY,";
                                    sql += " GUID,ID_SPOSOBU_PLATNOSCI,NAZWA_SPOSOBU_PLATNOSCI,DOSTAWA_ULICA,DOSTAWA_KOD_POCZTOWY,DOSTAWA_MIEJSCOWOSC,DOSTAWA_PANSTWO";
                                    sql += ") values (" + fsid + ", ";
                                    sql += magId + ", ";  //MAGNUM
                                    sql += "'PA', "; //TYP_DOK_MAGAZYNOWEGO
                                    sql += "'PA', "; //KOD definiowany przez użytkownika w Raks
                                    sql += DateTime.Now.Year.ToString() + ", "; //ROK
                                    sql += DateTime.Now.Month.ToString() + ", "; //MIESIAC

                                    //Wyliczanie nr z numeracji do zrobienia
                                    sql += fsid + ", "; //NR 
                                    sql += "'101/PA/M8" + fsid + "', "; //NUMER - symbol dokumentu
                                    sql += "1, "; //SPOSOB_LICZENIA 1-brutto, 0-netto
                                    sql += "0, "; //ID_WALUTY  
                                    sql += "1, "; //KURS 
                                    sql += "'Paragon' ,"; //NAZWA_DOKUMENTU  >>  To też trzeba wyciągnąć z numeracji
                                    sql += GetSprzedazDetalicznaCustomerID(polaczenieFB).ToString() + ", "; //ID_PLATNIKA
                                    sql += GetSprzedazDetalicznaCustomerID(polaczenieFB).ToString() + ", "; //ID_ODBIORCY
                                    sql += "'Sprzedaż detaliczna'" + ", "; //NAZWA_SKROCONA_PLATNIKA
                                    sql += "'Sprzedaż detaliczna'" + ", "; //NAZWA_PELNA_PLATNIKA
                                    sql += "'Sprzedaż detaliczna'" + ", "; //NAZWA_SKROCONA_ODBIORCY 
                                    sql += "'Sprzedaż detaliczna', "; //NAZWA_PELNA_ODBIORCY 
                                    sql += "'" + response.Results[0].clientResult.clientDeliveryAddress.clientDeliveryAddressPhone1 + "', "; //KOD_KRESKOWY_PLATNIKA
                                    sql += "0, "; //WARTOSC_ZAKUPU_KAUCJI 
                                    sql += "0, "; //WAL_WARTOSC_KAUJI 
                                    sql += "0, "; //PLN_WARTOSC_KAUCJ 
                                    sql += "'" + polaczenieFB.GetLoggedUser() + "' ,"; //OPERATOR
                                    sql += "'" + polaczenieFB.GetLoggedUser() + "' ,"; //ZMIENIL
                                    sql += "'" + response.Results[0].orderSerialNumber + "' ,"; //SYGNATURA
                                    
                                    if (response.Results[0].orderType.ToString().Equals("p"))
                                    {
                                        if (response.Results[0].orderDetails.prepaids.Length > 0)
                                        {
                                            sql += "'" + response.Results[0].orderDetails.prepaids[0].payformId + ",IAI hurt z panelu,' ,"; //ZNACZNIK
                                        }
                                        else
                                        {
                                            sql += "'IAI hurt z panelu,' ,"; //ZNACZNIK
                                        }
                                    }else if (response.Results[0].orderType.ToString().Equals("t"))
                                    {
                                        if (response.Results[0].orderDetails.prepaids.Length > 0)
                                        {
                                            sql += "'" + response.Results[0].orderDetails.prepaids[0].payformId + ",IAI hurt ze sklepu,' ,"; //ZNACZNIK
                                        }
                                        else
                                        {
                                            sql += "'IAI hurt ze sklepu,' ,"; //ZNACZNIK
                                        }
                                    }
                                    else if(response.Results[0].orderType.ToString().Equals("n"))
                                    {
                                        if (response.Results[0].orderDetails.prepaids.Length > 0)
                                        {
                                            sql += "'" + response.Results[0].orderDetails.prepaids[0].payformId + ",IAI detal ze sklepu,' ,"; //ZNACZNIK
                                        }
                                        else
                                        {
                                            sql += "'IAI detal ze sklepu,' ,"; //ZNACZNIK
                                        }
                                    }
                                    else if(response.Results[0].orderType.ToString().Equals("r"))
                                    {
                                        if (response.Results[0].orderDetails.prepaids.Length > 0)
                                        {
                                            sql += "'" + response.Results[0].orderDetails.prepaids[0].payformId + ",IAI detal z panelu,' ,"; //ZNACZNIK
                                        }
                                        else
                                        {
                                            sql += "'IAI detal z panelu,' ,"; //ZNACZNIK
                                        }
                                    }
                                    else
                                    {
                                        if (response.Results[0].orderDetails.prepaids.Length > 0)
                                        {
                                            sql += "'" + response.Results[0].orderDetails.prepaids[0].payformId + ",IAI,' ,"; //ZNACZNIK
                                        }
                                        else
                                        {
                                            sql += "'IAI,' ,"; //ZNACZNIK
                                        }
                                    }

                                        sql += "0, "; //MAGAZYNOWY 
                                    sql += "'" + Guid.NewGuid() + "' ,"; //GUID

                                    //To działa tylko na ID
                                    if (response.Results[0].orderDetails.payments.orderPaymentType.Equals("prepaid"))
                                    {
                                        sql += "0, "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Zapłacono przelewem' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }else if (response.Results[0].orderDetails.payments.orderPaymentType.Equals("tradecredit"))
                                    {
                                        sql += "0, "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Przelew' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }
                                    else
                                    {
                                        sql += "0, "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Pobranie' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }

                                    sql += "'" + response.Results[0].orderDetails.dispatch.courierName + "' ,"; //DOSTAWA_ULICA >> Kurier
                                    sql += "'" + response.Results[0].clientResult.clientDeliveryAddress.clientDeliveryAddressZipCode + "' ,"; //DOSTAWA_KOD_POCZTOWY
                                    sql += "'" + response.Results[0].clientResult.clientDeliveryAddress.clientDeliveryAddressCity + "', "; //DOSTAWA_MIEJSCOWOSC 
                                    sql += "'" + response.Results[0].clientResult.clientDeliveryAddress.clientDeliveryAddressCountry + "'); "; //DOSTAWA_PANSTWO (Nazwa dostawcy przesyłki)


                                    FbCommand new_fs = new FbCommand(sql, polaczenieFB.getConnection());
                                    try
                                    {
                                        new_fs.ExecuteScalar();
                                        komunikatZwrotny = "OK";
                                        ConnectionFB.setErrOrLogMsg("INFO", "Dodano nagłówkek paragonu dla " + orderIdIAI + " w RaksSQL; " + System.Environment.NewLine + komunikatZwrotny);
                                    }
                                    catch (FbException exgen)
                                    {
                                        komunikatZwrotny = "1007 zapisu nagłówka paragonu/faktury w Raks: dla orderIdIAI: " + orderIdIAI;
                                        ConnectionFB.setErrOrLogMsg("ERROR", "Błąd zapisania do RaksSQL: " + exgen.Message + System.Environment.NewLine + komunikatZwrotny);
                                        throw;
                                    }
                                }
                                else
                                {
                                    //sptrzedaż na fakturę
                                }
                            }
                            else
                            {
                                komunikatZwrotny += "1004 Błąd zamówienie " + orderIdIAI + " ma pozycje, które nie mają indeksów w RaksSQL " ;
                                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd w zamówieniu do zapisania w RaksSQL; " + System.Environment.NewLine + komunikatZwrotny);
                            }
                        }
                    }
                }
                catch (Exception exr)
                {
                    komunikatZwrotny += "1002 Błąd-Wyjątek wczytania zamówienia z IAI; Opis:" + exr.Message + " ";
                    ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek wczytania zamówienia " + orderIdIAI + " do biblioteki zapisu nowego zamówienia jako GM_FS do Raks." + System.Environment.NewLine + komunikatZwrotny);
                    throw;
                }

            }
            
            return komunikatZwrotny;
        }

        private static int GetSprzedazDetalicznaCustomerID(ConnectionFB polaczenieFB)
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
                cid = 1;
                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService) przy pobieraniu id kontrahenta dla sprzedaży detalicznej." + System.Environment.NewLine + "Kod 1005; błąd pobrania ID sprzedaży detalicznej");
            };
            return cid;
        }
    }
}
