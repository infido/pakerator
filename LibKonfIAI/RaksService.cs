using FirebirdSql.Data.FirebirdClient;
using LibKonfIAI.ApiOrdersServiceGet;
using LibKonfIAI.ApiPaymentsServicesGet;
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
        public static string saveNewOrderAsInvoiceToRaks(ConnectionFB polaczenieFB, ConnectionFB polaczenieR3, string orderIdIAI, int magId)
        {
            string komunikatZwrotny = "";
            string symDoku = "";

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
                //request.@params.ordersSerialNumbers[0] = 26590;

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
                            bool indeksyNaPozycjachSaOK = true;

                            //mamy zamówienie można działać dalej
                            if (indeksyNaPozycjachSaOK)
                            {
                                int fsid = 0;
                                string rekGIUD="";
                                string rekOpis = "";

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

                                string typDok = "";
                                string sql = "INSERT INTO GM_FS (ID,MAGNUM,ROK,MIESIAC,TYP_DOK_MAGAZYNOWEGO,KOD,NR,NUMER,SPOSOB_LICZENIA,ID_WALUTY,KURS,NAZWA_DOKUMENTU, ";
                                sql += " ID_PLATNIKA, ID_ODBIORCY,NAZWA_SKROCONA_PLATNIKA,NAZWA_PELNA_PLATNIKA,NIP_PLATNIKA,NAZWA_SKROCONA_ODBIORCY,NAZWA_PELNA_ODBIORCY,NIP_ODBIORCY,KOD_KRESKOWY_PLATNIKA, ";
                                sql += " WARTOSC_ZAKUPU_KAUCJ,WAL_WARTOSC_KAUCJ,PLN_WARTOSC_KAUCJ,OPERATOR,ZMIENIL,SYGNATURA,ZNACZNIKI,MAGAZYNOWY,";
                                sql += " GUID,ID_SPOSOBU_PLATNOSCI,NAZWA_SPOSOBU_PLATNOSCI,DATA_PLATNOSCI, ";
                                sql += " DOSTAWA_ULICA,DOSTAWA_KOD_POCZTOWY,DOSTAWA_MIEJSCOWOSC,DOSTAWA_PANSTWO,UWAGI, RODZAJ_CENY";
                                sql += ") values (" + fsid + ", ";
                                sql += magId + ", ";  //MAGNUM

                                sql += DateTime.Now.Year.ToString() + ", "; //ROK
                                sql += DateTime.Now.Month.ToString() + ", "; //MIESIAC
                                NumerDokumentu nrDoku = new NumerDokumentu();

                                if (response.Results[0].orderDetails.clientRequestInvoice.Equals("n"))
                                {
                                    typDok = "PA";
                                    //sprzesaż na paragon

                                    sql += "'" + typDok + "', "; //TYP_DOK_MAGAZYNOWEGO
                                    if (response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceType.ToString().Equals("selff_added"))
                                    {
                                        nrDoku = GetKodNumeracji(polaczenieFB, magId, "PAP");
                                        rekOpis = "PAP ";
                                    }
                                    else if (response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceType.ToString().Equals("auctions"))
                                    {
                                        nrDoku = GetKodNumeracji(polaczenieFB, magId, "PAA");
                                        rekOpis = "PAA ";
                                    }
                                    else
                                    {
                                        nrDoku = GetKodNumeracji(polaczenieFB, magId, "PAI");
                                        rekOpis = "PAI ";
                                    }
                                }
                                else
                                {
                                    //sptrzedaż na fakturę
                                    typDok = "FS";

                                    sql += "'" + typDok + "', "; //TYP_DOK_MAGAZYNOWEGO
                                    if (response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceType.ToString().Equals("selff_added"))
                                    {
                                        nrDoku = GetKodNumeracji(polaczenieFB, magId, "FAP");
                                        rekOpis = "FAP ";
                                    }
                                    else if (response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceType.ToString().Equals("auctions"))
                                    {
                                        nrDoku = GetKodNumeracji(polaczenieFB, magId, "FSA");
                                        rekOpis = "FSA ";
                                    }
                                    else
                                    {
                                        nrDoku = GetKodNumeracji(polaczenieFB, magId, "FSI");
                                        rekOpis = "FSI ";
                                    }
                                }

                                sql += "'" + nrDoku.nazwaKodu + "', "; //KOD definiowany w kodach dokumentów przez administartora Raks
                                sql += nrDoku.nrKolejny + ", "; //NR 
                                symDoku = nrDoku.wyliczonySymbolDlaDok;
                                sql += "'" + nrDoku.wyliczonySymbolDlaDok + "', "; //NUMER - symbol dokumentu
                                rekOpis += nrDoku.wyliczonySymbolDlaDok;

                                sql += "1, "; //SPOSOB_LICZENIA 1-brutto, 0-netto
                                sql += "0, "; //ID_WALUTY  
                                sql += "1, "; //KURS 
                                if (response.Results[0].orderDetails.clientRequestInvoice.Equals("n"))
                                {
                                    sql += "'Paragon' ,"; //NAZWA_DOKUMENTU  >>  To też trzeba wyciągnąć z numeracji
                                    sql += GetSprzedazDetalicznaCustomerID(polaczenieFB).ToString() + ", "; //ID_PLATNIKA
                                    sql += GetSprzedazDetalicznaCustomerID(polaczenieFB).ToString() + ", "; //ID_ODBIORCY
                                    sql += "'Sprzedaż detaliczna'" + ", "; //NAZWA_SKROCONA_PLATNIKA
                                    sql += "'Sprzedaż detaliczna'" + ", "; //NAZWA_PELNA_PLATNIKA
                                    sql += "null, "; //NIP_PLATNIKA
                                    sql += "'Sprzedaż detaliczna'" + ", "; //NAZWA_SKROCONA_ODBIORCY 
                                    sql += "'Sprzedaż detaliczna', "; //NAZWA_PELNA_ODBIORCY 
                                    sql += "null, "; //NIP_ODBIORCY
                                }
                                else
                                {
                                    sql += "'Faktura VAT' ,"; //NAZWA_DOKUMENTU  >>  To też trzeba wyciągnąć z numeracji
                                    string idkh = GetCustomerIDbyNIP(polaczenieFB, response.Results[0].clientResult.clientBillingAddress.clientNip.Replace("-",""), response.Results[0].clientResult.clientBillingAddress.clientFirm, response.Results[0].clientResult.clientBillingAddress.clientPhone1).ToString();
                                    sql += idkh + ", "; //ID_PLATNIKA
                                    sql += idkh + ", "; //ID_ODBIORCY
                                    sql += "'" + response.Results[0].clientResult.clientBillingAddress.clientPhone1 + "'" + ", "; //NAZWA_SKROCONA_PLATNIKA
                                    sql += "'" + response.Results[0].clientResult.clientBillingAddress.clientFirm + "'" + ", "; //NAZWA_PELNA_PLATNIKA
                                    sql += "'" + response.Results[0].clientResult.clientBillingAddress.clientNip.Replace("-","") + "'" + ", "; //NIP_PLATNIKA  
                                    if (response.Results[0].clientResult.clientDeliveryAddress.clientDeliveryAddressFirm.ToString().Length > 0)
                                    {
                                        sql += "'" + response.Results[0].clientResult.clientDeliveryAddress.clientDeliveryAddressPhone1 + "'" + ", "; //NAZWA_SKROCONA_ODBIORCY 
                                        sql += "'" + response.Results[0].clientResult.clientDeliveryAddress.clientDeliveryAddressFirm + "', "; //NAZWA_PELNA_ODBIORCY 
                                    }
                                    else
                                    {
                                        sql += "'" + response.Results[0].clientResult.clientBillingAddress.clientPhone1 + "'" + ", "; //NAZWA_SKROCONA_ODBIORCY
                                        sql += "'" + response.Results[0].clientResult.clientBillingAddress.clientFirm + "'" + ", "; //NAZWA_PELNA_ODBIORCY
                                    }
                                    sql += "'" + response.Results[0].clientResult.clientBillingAddress.clientNip.Replace("-", "") + "'" + ", "; //NIP_ODBIORCY    
                                }
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
                                        sql += "'" + response.Results[0].orderDetails.prepaids[0].payformName + ",IAI-p, " + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "', "; //ZNACZNIK
                                    }
                                    else
                                    {
                                        sql += "'IAI hurt z panelu," + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "' ,"; //ZNACZNIK
                                    }
                                }
                                else if (response.Results[0].orderType.ToString().Equals("t"))
                                {
                                    if (response.Results[0].orderDetails.prepaids.Length > 0)
                                    {
                                        sql += "'" + response.Results[0].orderDetails.prepaids[0].payformName + ",IAI-t, " + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "', "; //ZNACZNIK
                                    }
                                    else
                                    {
                                        sql += "'IAI hurt ze sklepu," + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "' ,"; //ZNACZNIK
                                    }
                                }
                                else if (response.Results[0].orderType.ToString().Equals("n"))
                                {
                                    if (response.Results[0].orderDetails.prepaids.Length > 0)
                                    {
                                        sql += "'" + response.Results[0].orderDetails.prepaids[0].payformName + ",IAI-n, " + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "', "; //ZNACZNIK
                                    }
                                    else
                                    {
                                        sql += "'IAI detal ze sklepu," + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "' ,"; //ZNACZNIK
                                    }
                                }
                                else if (response.Results[0].orderType.ToString().Equals("r"))
                                {
                                    if (response.Results[0].orderDetails.prepaids.Length > 0)
                                    {
                                        sql += "'" + response.Results[0].orderDetails.prepaids[0].payformName + ",IAI-r, " + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "', "; //ZNACZNIK
                                    }
                                    else
                                    {
                                        sql += "'IAI det.pan.," + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "' ,"; //ZNACZNIK
                                    }
                                }
                                else
                                {
                                    if (response.Results[0].orderDetails.prepaids.Length > 0)
                                    {
                                        sql += "'" + response.Results[0].orderDetails.prepaids[0].payformName + ",IAI, " + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "', "; //ZNACZNIK
                                    }
                                    else
                                    {
                                        sql += "'IAI," + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "' ,"; //ZNACZNIK
                                    }
                                }

                                sql += "0, "; //MAGAZYNOWY 
                                rekGIUD = Guid.NewGuid().ToString();
                                sql += "'" + rekGIUD + "' ,"; //GUID

                                //To działa tylko na ID
                                int idTypuPlatnosci = 0;
                                if (response.Results[0].orderDetails.payments.orderPaymentType.Equals("prepaid"))
                                {
                                    string nazwaUzytychPlatnosci="";
                                    bool rozneRodzajePlatnosci = false;
                                    foreach (prepaidType pre in response.Results[0].orderDetails.prepaids)
                                    {
                                        if (!pre.paymentStatus.Equals("c"))
                                        {
                                            if (nazwaUzytychPlatnosci.Length==0)
                                                nazwaUzytychPlatnosci = pre.payformName;
                                            else
                                            {
                                                if (!nazwaUzytychPlatnosci.Contains(pre.payformName))
                                                    rozneRodzajePlatnosci = true;
                                            }
                                        }
                                    }
                                    if (rozneRodzajePlatnosci)
                                    {
                                        sql += GetIDtypuPlatnosci(polaczenieFB, "Płatność złożona") + ", "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Płatność złożona' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }
                                    else
                                    {
                                        idTypuPlatnosci = GetIDtypuPlatnosci(polaczenieFB, nazwaUzytychPlatnosci);
                                        sql += idTypuPlatnosci + ", "; //ID_SPOSOBU_PLATNOSCI 
                                        if (idTypuPlatnosci == 2)
                                            sql += "'Płatność złożona' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                        else
                                            sql += "'" + nazwaUzytychPlatnosci + "' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }
                                }
                                else if (response.Results[0].orderDetails.payments.orderPaymentType.Equals("tradecredit"))
                                {
                                    if (response.Results[0].orderDetails.payments.orderPaymentDays<=3)
                                    {
                                        sql += GetIDtypuPlatnosci(polaczenieFB, "Przelew 3 dni") + ", "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Przelew 3 dni' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }else if (response.Results[0].orderDetails.payments.orderPaymentDays <= 7)
                                    {
                                        sql += GetIDtypuPlatnosci(polaczenieFB, "Przelew 7 dni") + ", "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Przelew 7 dni' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }else if (response.Results[0].orderDetails.payments.orderPaymentDays <= 14)
                                    {
                                        sql += GetIDtypuPlatnosci(polaczenieFB, "Przelew 14 dni") + ", "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Przelew 14 dni' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }else 
                                    {
                                        sql += GetIDtypuPlatnosci(polaczenieFB, "Przelew 21 dni") + ", "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Przelew 21 dni' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }
                                }
                                else if (response.Results[0].orderDetails.payments.orderPaymentType.Equals("cash_on_delivery"))
                                {
                                    string nazwaKuriera = response.Results[0].orderDetails.dispatch.courierName;
                                    idTypuPlatnosci = GetIDtypuPlatnosci(polaczenieFB, "Pobranie " + nazwaKuriera);
                                    if (idTypuPlatnosci != 2)
                                    {
                                        sql += idTypuPlatnosci + ", "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Pobranie" + nazwaKuriera + "' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }
                                    else
                                    {
                                        sql += GetIDtypuPlatnosci(polaczenieFB, "Pobranie") + ", "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Pobranie' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }
                                }else
                                {
                                    sql += GetIDtypuPlatnosci(polaczenieFB, "Płatność złożona") + ", "; //ID_SPOSOBU_PLATNOSCI 
                                    sql += "'Płatność złożona' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                }
                                

                                if (response.Results[0].orderDetails.prepaids.Length > 0)
                                {
                                    sql += "'" + response.Results[0].orderDetails.prepaids[0].paymentAddDate.Substring(0, 10) + "',";  //DATA_PLATNOSCI
                                }
                                else
                                    sql += "'NOW',"; //DATA_PLATNOSCI

                                if (response.Results[0].orderDetails.dispatch.courierName.ToString().Length > 39)
                                    sql += "'" + response.Results[0].orderDetails.dispatch.courierName.ToString().Substring(0, 39) + "' ,"; //DOSTAWA_ULICA >> Kurier
                                else
                                    sql += "'" + response.Results[0].orderDetails.dispatch.courierName.ToString() + "' ,"; //DOSTAWA_ULICA >> Kurier

                                sql += "'" + response.Results[0].clientResult.clientDeliveryAddress.clientDeliveryAddressZipCode + "' ,"; //DOSTAWA_KOD_POCZTOWY
                                sql += "'" + response.Results[0].clientResult.clientDeliveryAddress.clientDeliveryAddressCity + "', "; //DOSTAWA_MIEJSCOWOSC 
                                sql += "'" + response.Results[0].clientResult.clientDeliveryAddress.clientDeliveryAddressCountry + "', "; //DOSTAWA_PANSTWO (Nazwa dostawcy przesyłki)
                                if (response.Results[0].orderDetails.prepaids.Length > 0)
                                {
                                    sql += "'Płatności: " + System.Environment.NewLine;
                                    foreach (prepaidType item in response.Results[0].orderDetails.prepaids)
                                    {
                                        sql += item.payformName + "; " + item.paymentAddDate + "; ";
                                        sql += " Status: " + item.paymentStatus;
                                        sql += "; kwota: " + item.paymentValue.ToString("C") + "; " + GetIdentyfikatorPlatonsciZIAI(item.paymentNumber) + System.Environment.NewLine;
                                    }
                                    sql += System.Environment.NewLine;
                                    sql += "Nazwa kuriera: " + response.Results[0].orderDetails.dispatch.courierName.ToString() + System.Environment.NewLine;
                                    sql += "Koszt wysyłki: " + response.Results[0].orderDetails.payments.orderBaseCurrency.orderDeliveryCost.ToString("C") + System.Environment.NewLine;
                                    sql += "Koszt ubezpieczenia: " + response.Results[0].orderDetails.payments.orderBaseCurrency.orderInsuranceCost.ToString("C");
                                    sql += System.Environment.NewLine + response.Results[0].orderDetails.clientNoteToOrder + "',"; //UWAGI
                                }
                                else
                                {
                                    sql += "'Koszt wysyłki: " + response.Results[0].orderDetails.payments.orderBaseCurrency.orderDeliveryCost.ToString("C") + System.Environment.NewLine;
                                    sql += "Nazwa kuriera: " + response.Results[0].orderDetails.dispatch.courierName.ToString() + System.Environment.NewLine;
                                    sql += "Koszt ubezpieczenia: " + response.Results[0].orderDetails.payments.orderBaseCurrency.orderInsuranceCost.ToString("C");
                                    sql += System.Environment.NewLine + response.Results[0].orderDetails.clientNoteToOrder + "',"; //UWAGI
                                }
                                sql += GetCenaDetalID(polaczenieFB); //RODZAJ_CENY
                                sql += ");";

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
                                if (response.Results[0].orderDetails.clientNoteToOrder.ToString().Length > 0)
                                    SetKomunikatDlaFS(polaczenieFB, rekGIUD, rekOpis, response.Results[0].orderDetails.clientNoteToOrder.ToString());

                                int lp = 1;
                                decimal net = 0;
                                decimal brt = 0;
                                foreach (ApiOrdersServiceGet.productResultType fspoz in response.Results[0].orderDetails.productsResults)
                                {
                                    string sqlKod = "SELECT ID_TOWARU from GM_TOWARY ";
                                    sqlKod += " where ( GM_TOWARY.SKROT='" + fspoz.productSizeCodeExternal + "' OR GM_TOWARY.SKROT2='" + fspoz.productSizeCodeExternal + "' )";
                                    FbCommand cdkKod = new FbCommand(sqlKod, polaczenieFB.getConnection());

                                    int kodTowar = 0;
                                    try
                                    {
                                        var wynik = cdkKod.ExecuteScalar();
                                        if (wynik != null)
                                            kodTowar = Convert.ToInt32(wynik);
                                    }
                                    catch (Exception tex)
                                    {
                                        komunikatZwrotny = "1010 błąd ustalenia ID_TOWARU dla zapis pozycji paragonu/faktury w Raks: dla orderIdIAI: " + orderIdIAI + " pozycja:" + fspoz.productSizeCodeExternal;
                                        ConnectionFB.setErrOrLogMsg("ERROR", "Błąd zapisania do RaksSQL: " + tex.Message + System.Environment.NewLine + komunikatZwrotny);

                                    }

                                    int pozid = 0;
                                    FbCommand gen_id_fspos = new FbCommand("SELECT GEN_ID(GM_FSPOZ_GEN,1) from rdb$database", polaczenieFB.getConnection());
                                    try
                                    {
                                        pozid = Convert.ToInt32(gen_id_fspos.ExecuteScalar());
                                    }
                                    catch (FbException exgen)
                                    {
                                        komunikatZwrotny += "1011 Bład pobrania id pozycji z generatora paragonu/faktury w Raks: dla orderIdIAI: " + orderIdIAI;
                                        ConnectionFB.setErrOrLogMsg("ERROR", "Błąd w zamówieniu do zapisania w RaksSQL: " + exgen.Message + System.Environment.NewLine + komunikatZwrotny);
                                        throw;
                                    }

                                    sql = "INSERT INTO GM_FSPOZ (ID,ID_GLOWKI,LP,ID_TOWARU,ILOSC,ILOSC_ALT,";
                                    sql += "ID_STAWKI,STAWKA,RODZAJ_CENY,";
                                    sql += "CENA_KATALOGOWA,CENA_SPRZEDAZY,";
                                    sql += "CENA_SP_PLN_BRUTTO,CENA_SP_WAL_BRUTTO,CENA_SP_PLN_BRUTTO_ALT,CENA_SP_WAL_BRUTTO_ALT,";
                                    sql += "CENA_SP_PLN_NETTO,CENA_SP_WAL_NETTO,CENA_SP_PLN_NETTO_ALT,CENA_SP_WAL_NETTO_ALT, RABAT,";
                                    sql += "SKROT_ORYGINALNY,NAZWA_ORYGINALNA,SKROT_ALTERNATYWNY,NAZWA_ALTERNATYWNA,GUID,DATA_WYDANIA)";
                                    sql += " values ";
                                    sql += " (" + pozid + "," + fsid + "," + lp + "," + kodTowar + "," + fspoz.productQuantity + "," + fspoz.productQuantity + ",";

                                    int idSta = GetIDstawkiVat(polaczenieFB, kodTowar);
                                    sql += idSta + ","; //ID_STAWKI VAT
                                    sql += GetValStawkiVat(polaczenieR3, idSta) + ","; //STAWKA VAT
                                    sql += GetCenaDetalID(polaczenieFB) + ","; //RODZAJ_CENY

                                    float cennaKatalogowaNetto;
                                    if (fspoz.productPanelPriceNet == 0)
                                    {
                                        cennaKatalogowaNetto = GetCenaKatalogowaNettoDETAL(polaczenieFB, kodTowar, fspoz.productOrderPriceNet);
                                        sql += cennaKatalogowaNetto.ToString().Replace(",", ".") + ","; // CENA_KATALOGOWA netto z cennika Raks
                                        sql += cennaKatalogowaNetto.ToString().Replace(",", ".") + ","; // CENA_SPRZEDAZY netto z cennika Raks (ta sama katalogowa)
                                    }
                                    else
                                    {
                                        cennaKatalogowaNetto = GetCenaKatalogowaNettoDETAL(polaczenieFB, kodTowar, fspoz.productPanelPriceNet);
                                        sql += cennaKatalogowaNetto.ToString().Replace(",", ".") + ","; // CENA_KATALOGOWA netto z cennika Raks
                                        sql += cennaKatalogowaNetto.ToString().Replace(",", ".") + ","; // CENA_SPRZEDAZY netto z cennika Raks (ta sama katalogowa)
                                    }

                                    brt += (decimal)fspoz.productOrderPrice;
                                    sql += fspoz.productOrderPrice.ToString().Replace(",", ".") + ","; //CENA_SP_PLN_BRUTTO
                                    sql += fspoz.productOrderPrice.ToString().Replace(",", ".") + ","; //CENA_SP_WAL_BRUTTO
                                    sql += fspoz.productOrderPrice.ToString().Replace(",", ".") + ","; //CENA_SP_PLN_BRUTTO_ALT
                                    sql += fspoz.productOrderPrice.ToString().Replace(",", ".") + ","; //CENA_SP_WAL_BRUTTO_ALT

                                    net += (decimal)fspoz.productOrderPriceNet;
                                    sql += fspoz.productOrderPriceNet.ToString().Replace(",", ".") + ","; //CENA_SP_PLN_NETTO
                                    sql += fspoz.productOrderPriceNet.ToString().Replace(",", ".") + ","; //CENA_SP_WAL_NETTO
                                    sql += fspoz.productOrderPriceNet.ToString().Replace(",", ".") + ","; //CENA_SP_PLN_NETTO_ALT
                                    sql += fspoz.productOrderPriceNet.ToString().Replace(",", ".") + ","; //CENA_SP_WAL_NETTO_ALT

                                    if (cennaKatalogowaNetto!= fspoz.productOrderPriceNet)
                                    {
                                        sql += Math.Round(Convert.ToDecimal((cennaKatalogowaNetto - fspoz.productOrderPriceNet) / (cennaKatalogowaNetto / 100)), 2).ToString().Replace(",", ".") + ", "; //RABAT
                                    }
                                    else
                                        sql +=  "0,"; //brak RABAT-u

                                    sql += "'" + fspoz.productSizeCodeExternal.Replace("'", "_") + "',"; //SKROT_ORYGINALNY
                                    sql += "'" + fspoz.productName.Replace("'", "_") + "',"; //NAZWA_ORYGINALNA
                                    sql += "'" + fspoz.productSizeCodeExternal.Replace("'", "_") + "',"; //SKROT_ALTERNATYWNY
                                    sql += "'" + fspoz.productName.Replace("'", "_") + "',"; //NAZWA_ALTERNATYWNA

                                    sql += "'" + Guid.NewGuid() + "',";
                                    sql += "'TODAY');"; //DATA_WYDANIA

                                    FbCommand new_fspoz = new FbCommand(sql, polaczenieFB.getConnection());
                                    try
                                    {
                                        new_fspoz.ExecuteScalar();
                                        komunikatZwrotny = komunikatZwrotny.Equals("OK") ? "OK" : komunikatZwrotny + "; OK";
                                        ConnectionFB.setErrOrLogMsg("INFO", "Dodano pozycje " + fspoz.productSizeCodeExternal + " paragonu dla " + orderIdIAI + " w RaksSQL; " + System.Environment.NewLine + komunikatZwrotny);
                                    }
                                    catch (FbException exgen)
                                    {
                                        komunikatZwrotny = "1013 zapisu pozycji paragonu/faktury w Raks: dla orderIdIAI: " + orderIdIAI + "; index:" + fspoz.productSizeCodeExternal;
                                        ConnectionFB.setErrOrLogMsg("ERROR", "Błąd zapisania do RaksSQL: " + exgen.Message + System.Environment.NewLine + komunikatZwrotny);
                                        throw;
                                    }

                                    sql = " UPDATE GM_FS SET ";
                                    sql += "WAL_WARTOSC_NETTO=" + net.ToString().Replace(",", ".") + ", ";
                                    sql += "PLN_WARTOSC_NETTO=" + net.ToString().Replace(",", ".") + ", ";
                                    sql += "WAL_WARTOSC_BRUTTO=" + brt.ToString().Replace(",", ".") + ", ";
                                    sql += "PLN_WARTOSC_BRUTTO=" + brt.ToString().Replace(",", ".") + ", ";
                                    sql += "WARTOSC_ROZRACHUNKU=" + brt.ToString().Replace(",", ".") + ", ";
                                    sql += "WAL_KWOTA_VAT=" + (brt - net).ToString().Replace(",", ".") + ", ";
                                    sql += "PLN_KWOTA_VAT=" + (brt - net).ToString().Replace(",", ".") + " ";
                                    sql += " where ID=" + fsid + ";";
                                    FbCommand sum_fs = new FbCommand(sql, polaczenieFB.getConnection());
                                    try
                                    {
                                        sum_fs.ExecuteScalar();
                                        komunikatZwrotny = komunikatZwrotny.Equals("OK") ? "OK" : komunikatZwrotny + "; OK";
                                        ConnectionFB.setErrOrLogMsg("INFO", "Zaktualizowano wartości nagłówka paragonu/faktury dla " + orderIdIAI + " w RaksSQL; " + System.Environment.NewLine + komunikatZwrotny);
                                    }
                                    catch (FbException exgen)
                                    {
                                        komunikatZwrotny = "1014 uzupełnienia podsumownaia wartości paragonu/faktury w Raks: dla orderIdIAI: " + orderIdIAI;
                                        ConnectionFB.setErrOrLogMsg("ERROR", "Błąd zapisania do RaksSQL: " + exgen.Message + System.Environment.NewLine + komunikatZwrotny);
                                        throw;
                                    }

                                    lp++;
                                }
                            }
                            else
                            {
                                komunikatZwrotny += "1004 Błąd zamówienie " + orderIdIAI + " ma pozycje, które nie mają indeksów w RaksSQL ";
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
            
            return komunikatZwrotny + ">>" + symDoku;
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
            catch (FbException exgen2)
            {
                cid = 1;
                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService) przy pobieraniu id kontrahenta dla sprzedaży detalicznej." + System.Environment.NewLine + "Kod 1005; błąd pobrania ID sprzedaży detalicznej" + System.Environment.NewLine + exgen2.Message);
            };
            return cid;
        }

        private static int GetCenaDetalID(ConnectionFB polaczenieFB)
        {
            int cid = 0;

            FbCommand sp = new FbCommand("SELECT ID from GM_CENY where NAZWA='DETAL';", polaczenieFB.getConnection());
            try
            {
                FbDataReader fdk = sp.ExecuteReader();
                if (fdk.Read())
                {
                    cid = (int)fdk["ID"];
                }
                else
                    cid = 1;

                fdk.Close();
            }
            catch (FbException exgen2)
            {
                cid = 1;
                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService) przy pobieraniu id ceny DETAL." + System.Environment.NewLine + "Kod 1005; błąd pobrania ID sprzedaży detalicznej" + System.Environment.NewLine + exgen2.Message);
            };
            return cid;
        }

        private static float GetCenaKatalogowaNettoDETAL(ConnectionFB polaczenieFB, int idTowaru, float cenaZeSklepu)
        {
            int idCenaDetal = GetCenaDetalID(polaczenieFB);

            float cena = 0;

            FbCommand cen = new FbCommand("SELECT CENA from GM_CENNIK where ID_TOWARU=" + idTowaru + " and IDCENY=" + idCenaDetal + ";", polaczenieFB.getConnection());
            try
            {
                FbDataReader fdk = cen.ExecuteReader();
                if (fdk.Read())
                {
                    cena = (float)Convert.ToDecimal(fdk["CENA"]);
                }
                else
                    cena = cenaZeSklepu;

                fdk.Close();
            }
            catch (FbException exgen2)
            {
                cena = cenaZeSklepu;
                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService) przy wyliczaniu ceny DETAL." + System.Environment.NewLine + "Kod 1055; błąd pobrania wartości ceny DETAL ID_TOWARU: " + idTowaru + System.Environment.NewLine + exgen2.Message);
            };
            return cena;
        }

        private static int GetCustomerIDbyNIP(ConnectionFB polaczenieFB, string nip, string nazwaPelna, string nazwaSkrot)
        {
            int cid = 0;

            FbCommand sp = new FbCommand("SELECT ID from R3_CONTACTS where TAXID='" + nip + "';", polaczenieFB.getConnection());
            try
            {
                FbDataReader fdk = sp.ExecuteReader();
                if (fdk.Read())
                {
                    cid = (int)fdk["ID"];
                }
                else
                {
                    //no to trzeba założyć nowego kontrahenta
                    FbCommand gen_id_kh = new FbCommand("SELECT GEN_ID(R3_CONTACTS_ID_GEN,1) from rdb$database", polaczenieFB.getConnection());
                    try
                    {
                        cid = Convert.ToInt32(gen_id_kh.ExecuteScalar());

                        string sql = "select rdb$set_context('USER_SESSION', 'user_name','WWW') from rdb$database; ";
                        FbCommand setusr = new FbCommand(sql, polaczenieFB.getConnection());
                        var re = setusr.ExecuteScalar();

                        sql = " INSERT into R3_CONTACTS (ID,SHORT_NAME,TAXID,FULL_NAME,C_DATE,M_DATE,C_IDENT,M_IDENT,GUID,PURCHASER) ";
                        sql += " values (" + cid + ",'" + nazwaSkrot + "','" + nip + "','" + nazwaPelna + "', current_timestamp,current_timestamp,'WWW','WWW','" + Guid.NewGuid() + "',1);";
                        FbCommand newkh = new FbCommand(sql, polaczenieFB.getConnection());
                        var re2 = newkh.ExecuteScalar();
                    }
                    catch (FbException exgen)
                    {
                        ConnectionFB.setErrOrLogMsg("ERROR", "Błąd pobrania id przy tworzeniu nowego kontrahenta w zamówieniu do zapisania w RaksSQL: " + exgen.Message);
                        throw;
                    }
                }
                fdk.Close();
            }
            catch (FbException exgen2)
            {
                cid = 1;
                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService) przy pobieraniu id kontrahenta dla sprzedaży detalicznej." + System.Environment.NewLine + "Kod 1005; błąd pobrania ID sprzedaży detalicznej" + System.Environment.NewLine + exgen2.Message);
            };
            return cid;
        }
        private static NumerDokumentu GetKodNumeracji(ConnectionFB polaczenieFB,int magID, string kodNumeracji)
        {
            string sql = "SELECT GM_MAGAZYNY_KODY_DOK_POW.ID_KODU, R3_KODY_DOK.KOD, NUM_STARTOWY, R3_KODY_DOK.NUM_MASKA, GM_MAGAZYNY.NUMER SYMMAG ";
            sql += " FROM GM_MAGAZYNY_KODY_DOK_POW ";
            sql += " join R3_KODY_DOK on GM_MAGAZYNY_KODY_DOK_POW.ID_KODU = R3_KODY_DOK.ID ";
            sql += " join GM_MAGAZYNY on GM_MAGAZYNY_KODY_DOK_POW.ID_MAG=GM_MAGAZYNY.ID ";
            sql += " where GM_MAGAZYNY_KODY_DOK_POW.ID_MAG=" + magID + " AND R3_KODY_DOK.KOD='" + kodNumeracji + "'; ";

            NumerDokumentu nrd = new NumerDokumentu(kodNumeracji);

            FbCommand sp = new FbCommand(sql, polaczenieFB.getConnection());
            try
            {
                FbDataReader fdk = sp.ExecuteReader();
                if (fdk.Read())
                {
                    nrd.idkodu = (int)fdk["ID_KODU"];
                    nrd.nazwaKodu = (string)fdk["KOD"];
                    nrd.nrStartowy = (int)fdk["NUM_STARTOWY"];
                    nrd.maska = (string)fdk["NUM_MASKA"];
                    nrd.symMag = (string)fdk["SYMMAG"];
                    nrd.wyliczonySymbolDlaDok = wymienMaskeNaWartosci(nrd.maska, nrd.symMag);

                    FbCommand nrDokSp = new FbCommand("SELECT FIRST 1 NR FROM GM_FS where KOD='" + nrd.nazwaKodu + "' and ROK=" + DateTime.Now.ToString("yyyy") + " order by NR DESC;", polaczenieFB.getConnection());
                    try
                    {
                        var nrDo = nrDokSp.ExecuteScalar();
                        if (nrDo == null)
                            nrd.nrKolejny = nrd.nrStartowy;
                        else
                            nrd.nrKolejny = Convert.ToInt32(nrDo) + 1;

                        nrd.wyliczonySymbolDlaDok = wymienNaNrKolejny(nrd.wyliczonySymbolDlaDok, nrd.nrKolejny);
                        nrd.err = "OK";
                    }
                    catch (FbException exnum)
                    {
                        nrd.err = "Kod 1008; wyliczenia symbolu dokumentu sprzedaży";
                        ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService.GetKodNumeracji) " + System.Environment.NewLine + nrd.err + System.Environment.NewLine + exnum.Message);
                    };

                }
                else
                {
                    //standardowa numeracja PA i FS
                }
                fdk.Close();
            }
            catch (FbException exgen)
            {
                nrd.err = "Kod 1007; błąd pobrania kodu dokumentu sprzedaży";
                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService.GetKodNumeracji) " + System.Environment.NewLine + nrd.err + System.Environment.NewLine + exgen.Message);
            };
            return nrd;
        }

        private static string wymienMaskeNaWartosci(string maska, string magSYM)
        {
            if (maska.Contains("$$$$"))
                maska = maska.Replace("$$$$", DateTime.Now.ToString("yyyy"));

            if (maska.Contains("$$"))
                maska = maska.Replace("$$", DateTime.Now.ToString("yy"));

            if (maska.Contains("@@"))
                maska = maska.Replace("@@", DateTime.Now.ToString("MM"));

            if (maska.Contains("%%"))
                maska = maska.Replace("%%", DateTime.Now.ToString("dd"));

            if (maska.Contains("&&&&&&"))
                maska = maska.Replace("&&&&&&", magSYM.Substring(0,6));
            else if (maska.Contains("&&&&&"))
                maska = maska.Replace("&&&&&", magSYM.Substring(0, 5));
            else if (maska.Contains("&&&&"))
                maska = maska.Replace("&&&&", magSYM.Substring(0, 4));
            else if (maska.Contains("&&&"))
                maska = maska.Replace("&&&", magSYM.Substring(0, 3));
            else if (maska.Contains("&&"))
                maska = maska.Replace("&&", magSYM.Substring(0, 2));
            else if (maska.Contains("&"))
                maska = maska.Replace("&", magSYM.Substring(0, 1));

            return maska;
        }

        private static string wymienNaNrKolejny(string maska, int nrKolejny)
        {
            if (maska.Contains("#########"))
                maska = maska.Replace("#########", nrKolejny.ToString("000000000") );
            else if (maska.Contains("########"))
                maska = maska.Replace("########", nrKolejny.ToString("00000000"));
            else if (maska.Contains("#######"))
                maska = maska.Replace("#######", nrKolejny.ToString("0000000"));
            else if (maska.Contains("######"))
                maska = maska.Replace("######", nrKolejny.ToString("000000"));
            else if (maska.Contains("#####"))
                maska = maska.Replace("#####", nrKolejny.ToString("00000"));
            else if (maska.Contains("####"))
                maska = maska.Replace("####", nrKolejny.ToString("0000"));
            else if (maska.Contains("###"))
                maska = maska.Replace("###", nrKolejny.ToString("000"));
            else if (maska.Contains("##"))
                maska = maska.Replace("##", nrKolejny.ToString("00"));

            return maska;
        }

        private static int GetIDtypuPlatnosci(ConnectionFB polaczenieFB, string nazwaPlatnosci)
        {
            FbCommand sposobPlat = new FbCommand("SELECT ID from GM_SPOSOBY_ZAP where NAZWA='" + nazwaPlatnosci + "';", polaczenieFB.getConnection());
            try
            {
                var sqlPl = sposobPlat.ExecuteScalar();
                if (sqlPl == null)
                    return 2;
                else
                    return Convert.ToInt32(sqlPl);

            }
            catch (FbException expl)
            {
                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService.GetIDtypuPlatnosci) " + System.Environment.NewLine + " Kod 1009; Bład przy ustalaniu kodu płatności" + System.Environment.NewLine + expl.Message);
                return 2;
            };
        }

        private static int GetIDstawkiVat(ConnectionFB polaczenieFB, int idTowaru)
        {
            FbCommand sVat = new FbCommand("SELECT STAWKAVAT from GM_TOWARY where ID_TOWARU=" + idTowaru + ";", polaczenieFB.getConnection());
            try
            {
                var sqls = sVat.ExecuteScalar();
                if (sqls == null)
                    return 12;
                else
                    return Convert.ToInt32(sqls);

            }
            catch (FbException expl)
            {
                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService.GetIDstawkiVat) " + System.Environment.NewLine + " Kod 1012; Bład przy ustalaniu kodu stawki VAT z towaru ID_TOWARU:" + idTowaru + System.Environment.NewLine + expl.Message);
                return 12;
            };
        }

        private static decimal GetValStawkiVat(ConnectionFB polaczenieRaks300, int idStawkiVAT)
        {
            FbCommand sVat = new FbCommand("SELECT WARTOSC from STAWKI where ID=" + idStawkiVAT + ";", polaczenieRaks300.getConnection());
            try
            {
                var sqls = sVat.ExecuteScalar();
                if (sqls == null)
                    return 23;
                else
                    return Convert.ToDecimal(sqls);

            }
            catch (FbException expl)
            {
                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService.GetValStawkiVat) " + System.Environment.NewLine + " Kod 1013; Bład przy ustalaniu wartości stawki VAT z stawki o ID:" + idStawkiVAT + System.Environment.NewLine + expl.Message);
                return 23;
            };
        }

        private static void SetKomunikatDlaFS(ConnectionFB polaczenieFB, string guid, string sym, string uwagi)
        {
            FbCommand gen_id_msg = new FbCommand("SELECT GEN_ID(R3_ALARMY_ID_GEN,1) from rdb$database", polaczenieFB.getConnection());
            try
            {
                var sqlPl = gen_id_msg.ExecuteScalar();
                int locId = 0;
                if (sqlPl != null)
                    locId = Convert.ToInt32(sqlPl);

                string sql = "INSERT INTO R3_ALARMY (ID, RECORD_GUID, TRESC, AKTYWNY, PRYWATNY, DATA_OD, DATA_DO, ID_PRIOR, C_DATE, M_DATE, C_IDENT, M_IDENT, DATA_AKTYW, DATA_DEZAKTYW, URUCHOM_EDYCJA, URUCHOM_WYBOR, UWAGI, AM_TYPE, RECORD_DESC)";
                sql += " VALUES (";
                sql += " " + locId + ", ";
                sql += " '" + guid + "', ";
                sql += " 'Uwagi zamawiającego do zamówienia', ";
                sql += " 1, ";
                sql += " 0, ";
                sql += " 'NOW', ";
                sql += " null, ";
                sql += " 0, ";
                sql += " current_timestamp, ";
                sql += " current_timestamp, ";
                sql += " 'WWW', ";
                sql += " 'WWW', ";
                sql += " current_timestamp, ";
                sql += " null, ";
                sql += " 1, ";
                sql += " 1, ";
                sql += " '" + uwagi + "',";
                sql += " 28, ";
                sql += " '" + sym + "'";
                sql += " );";

                FbCommand ins_msg = new FbCommand(sql, polaczenieFB.getConnection());
                ins_msg.ExecuteScalar();

            }
            catch (FbException expl)
            {
                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService.SetKomunikatDlaFS) " + System.Environment.NewLine + " Kod 1063; Bład przy ustawianiu wartości komunikatu dla dok sprzedażowego:" + sym + System.Environment.NewLine + expl.Message);
            };
        }

        private static string GetIdentyfikatorPlatonsciZIAI(string idPlatnosci)
        {
            var binding = new BasicHttpBinding();
            var address = new EndpointAddress("http://" + DataSessionIAI.GetIAIDomainForCurrentSession() + "/api/?gate=payments/get/107/soap");
            var client = new ApiPaymentsServicesGet.ApiPaymentsPortTypeClient(binding, address);

            var request = new ApiPaymentsServicesGet.getRequestType();

            request.authenticate = new ApiPaymentsServicesGet.authenticateType();
            request.authenticate.userLogin = DataSessionIAI.GetIAILoginForCurrentSession();
            request.authenticate.authenticateKey = DataSessionIAI.GetIAIKeyForCurrentSession();

            request.@params = new ApiPaymentsServicesGet.getParamsType();
            request.@params.paymentNumber = idPlatnosci;
            request.@params.sourceType = sourceTypeType.order;

            ApiPaymentsServicesGet.getResponseType response = client.get(request);
            try
            {
                if (response.errors.faultCode != 0)
                {
                    string komunikatZwrotny = "1016 Kod: " + response.errors.faultCode + "; Opis:" + response.errors.faultString + " ";
                    ConnectionFB.setErrOrLogMsg("ERROR", "Błąd zwrócony przez API do wczytania identyfikatora płatnosci " + idPlatnosci + " do biblioteki zapisu nowego zamówienia jako GM_FS do Raks." + System.Environment.NewLine + komunikatZwrotny);
                    return " Identyfikator: błąd: " + response.errors.faultString;
                }
                else
                {
                    return " Identyfikator: " +  response.result.externalPaymentId;
                }
            }
            catch (Exception ex)
            {
                ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek (RaksService.GetIdentyfikatorPlatonsciZIAI) " + System.Environment.NewLine + " Kod 1015; Bład przy ustalaniu identyfikatora płatności o ID:" + idPlatnosci + System.Environment.NewLine + ex.Message);
                return "Identyfikator: błąd...";
                throw;
            }
        }
    }

    public class NumerDokumentu
    {
        public string typ="PA"; //PA lub FS
        public string nazwaKodu="PA"; //PAA, PAP, PAI... FSA, FAP, FSI...
        public int idkodu;
        public int nrStartowy=1;
        public int nrKolejny;
        public string maska;
        public string symMag;
        public string err;
        public string wyliczonySymbolDlaDok;

        public NumerDokumentu(string typDok)
        {
            typ = typDok;
        }
        public NumerDokumentu() { }
    }
}
