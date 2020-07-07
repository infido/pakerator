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
                                if (response.Results[0].orderDetails.clientRequestInvoice.Equals("n"))
                                {
                                    typDok = "PA";
                                    //sprzesaż na paragon
                                    string sql = "INSERT INTO GM_FS (ID,MAGNUM,ROK,MIESIAC,TYP_DOK_MAGAZYNOWEGO,KOD,NR,NUMER,SPOSOB_LICZENIA,ID_WALUTY,KURS,NAZWA_DOKUMENTU, ";
                                    sql += " ID_PLATNIKA, ID_ODBIORCY,NAZWA_SKROCONA_PLATNIKA,NAZWA_PELNA_PLATNIKA,NAZWA_SKROCONA_ODBIORCY,NAZWA_PELNA_ODBIORCY,KOD_KRESKOWY_PLATNIKA, ";
                                    sql += " WARTOSC_ZAKUPU_KAUCJ,WAL_WARTOSC_KAUCJ,PLN_WARTOSC_KAUCJ,OPERATOR,ZMIENIL,SYGNATURA,ZNACZNIKI,MAGAZYNOWY,";
                                    sql += " GUID,ID_SPOSOBU_PLATNOSCI,NAZWA_SPOSOBU_PLATNOSCI,DATA_PLATNOSCI, ";
                                    sql += " DOSTAWA_ULICA,DOSTAWA_KOD_POCZTOWY,DOSTAWA_MIEJSCOWOSC,DOSTAWA_PANSTWO,UWAGI";
                                    sql += ") values (" + fsid + ", ";
                                    sql += magId + ", ";  //MAGNUM

                                    sql += DateTime.Now.Year.ToString() + ", "; //ROK
                                    sql += DateTime.Now.Month.ToString() + ", "; //MIESIAC

                                    NumerDokumentu nrDoku;
                                    sql += "'" + typDok + "', "; //TYP_DOK_MAGAZYNOWEGO

                                    if (response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceType.ToString().Equals("selff_added"))
                                        nrDoku = GetKodNumeracji(polaczenieFB, magId, "PAP");
                                    else if (response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceType.ToString().Equals("auctions"))
                                        nrDoku = GetKodNumeracji(polaczenieFB, magId, "PAA");
                                    else
                                        nrDoku = GetKodNumeracji(polaczenieFB, magId, "PAI");

                                    sql += "'" + nrDoku.nazwaKodu + "', "; //KOD definiowany w kodach dokumentów przez administartora Raks
                                    sql += nrDoku.nrKolejny + ", "; //NR 
                                    symDoku = nrDoku.wyliczonySymbolDlaDok;
                                    sql += "'" + nrDoku.wyliczonySymbolDlaDok + "', "; //NUMER - symbol dokumentu


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
                                            sql += "'" + response.Results[0].orderDetails.prepaids[0].payformName + ",IAI-p, " + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "', "; //ZNACZNIK
                                        }
                                        else
                                        {
                                            sql += "'IAI hurt z panelu," + response.Results[0].orderDetails.orderSourceResults.orderSourceDetails.orderSourceName + "' ,"; //ZNACZNIK
                                        }
                                    }else if (response.Results[0].orderType.ToString().Equals("t"))
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
                                    else if(response.Results[0].orderType.ToString().Equals("n"))
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
                                    else if(response.Results[0].orderType.ToString().Equals("r"))
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
                                    sql += "'" + Guid.NewGuid() + "' ,"; //GUID

                                    //To działa tylko na ID
                                    if (response.Results[0].orderDetails.payments.orderPaymentType.Equals("prepaid"))
                                    {
                                        sql += GetIDtypuPlatnosci(polaczenieFB, "Zapłacono przelewem") + ", "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Zapłacono przelewem' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }else if (response.Results[0].orderDetails.payments.orderPaymentType.Equals("tradecredit"))
                                    {
                                        sql += GetIDtypuPlatnosci(polaczenieFB, "Przelew") + ", "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Przelew' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }
                                    else
                                    {
                                        sql += GetIDtypuPlatnosci(polaczenieFB, "Pobranie") + ", "; //ID_SPOSOBU_PLATNOSCI 
                                        sql += "'Pobranie' ,"; //NAZWA_SPOSOBU_PLATNOSCI
                                    }

                                    if (response.Results[0].orderDetails.prepaids.Length > 0)
                                    {
                                        sql += "'" + response.Results[0].orderDetails.prepaids[0].paymentAddDate.Substring(0,10) + "',";  //DATA_PLATNOSCI
                                    }
                                    else
                                        sql += "'NOW',"; //DATA_PLATNOSCI

                                    if (response.Results[0].orderDetails.dispatch.courierName.ToString().Length>39)
                                        sql += "'" + response.Results[0].orderDetails.dispatch.courierName.ToString().Substring(0,39) + "' ,"; //DOSTAWA_ULICA >> Kurier
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
                                            sql += "; kwota: " + item.paymentValue.ToString("C") + "; "  + GetIdentyfikatorPlatonsciZIAI(item.paymentNumber) +  System.Environment.NewLine;
                                        }
                                        sql += System.Environment.NewLine + System.Environment.NewLine;
                                        sql += "Koszt wysyłki: " + response.Results[0].orderDetails.payments.orderBaseCurrency.orderDeliveryCost.ToString("C") + System.Environment.NewLine;
                                        sql += "Koszt ubezpieczenia: " + response.Results[0].orderDetails.payments.orderBaseCurrency.orderInsuranceCost.ToString("C") + "');"; //UWAGI
                                    }
                                    else
                                    {
                                        sql += "'Koszt wysyłki: " + response.Results[0].orderDetails.payments.orderBaseCurrency.orderDeliveryCost.ToString("C") + System.Environment.NewLine;
                                        sql += "Koszt ubezpieczenia: " + response.Results[0].orderDetails.payments.orderBaseCurrency.orderInsuranceCost.ToString("C") + "');"; //UWAGI
                                    }

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
                                        sql += "ID_STAWKI,STAWKA,";
                                        sql += "CENA_KATALOGOWA,CENA_SPRZEDAZY,";
                                        sql += "CENA_SP_PLN_BRUTTO,CENA_SP_WAL_BRUTTO,CENA_SP_PLN_BRUTTO_ALT,CENA_SP_WAL_BRUTTO_ALT,";
                                        sql += "CENA_SP_PLN_NETTO,CENA_SP_WAL_NETTO,CENA_SP_PLN_NETTO_ALT,CENA_SP_WAL_NETTO_ALT, RABAT,";
                                        sql += "SKROT_ORYGINALNY,NAZWA_ORYGINALNA,SKROT_ALTERNATYWNY,NAZWA_ALTERNATYWNA,GUID,DATA_WYDANIA)";
                                        sql += " values ";
                                        sql += " (" + pozid + "," + fsid + "," + lp + "," + kodTowar + "," + fspoz.productQuantity + "," + fspoz.productQuantity + ",";

                                        int idSta = GetIDstawkiVat(polaczenieFB, kodTowar);
                                        sql += idSta + ","; //ID_STAWKI VAT
                                        sql += GetValStawkiVat(polaczenieR3, idSta) + ","; //STAWKA VAT

                                        if (fspoz.productPanelPriceNet == 0)
                                        {
                                            sql += fspoz.productOrderPriceNet.ToString().Replace(",", ".") + ","; // CENA_KATALOGOWA netto z cennika Raks
                                            sql += fspoz.productOrderPriceNet.ToString().Replace(",", ".") + ","; // CENA_SPRZEDAZY netto z cennika Raks (ta sama katalogowa)
                                        }
                                        else
                                        {
                                            sql += fspoz.productPanelPriceNet.ToString().Replace(",", ".") + ","; // CENA_KATALOGOWA netto z cennika Raks
                                            sql += fspoz.productPanelPriceNet.ToString().Replace(",", ".") + ","; // CENA_SPRZEDAZY netto z cennika Raks (ta sama katalogowa)
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

                                        sql += "0,"; //RABAT do wyliczenia

                                        sql += "'" + fspoz.productSizeCodeExternal + "',"; //SKROT_ORYGINALNY
                                        sql += "'" + fspoz.productName + "',"; //NAZWA_ORYGINALNA
                                        sql += "'" + fspoz.productSizeCodeExternal + "',"; //SKROT_ALTERNATYWNY
                                        sql += "'" + fspoz.productName + "',"; //NAZWA_ALTERNATYWNA

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
                                        sql += "WAL_WARTOSC_NETTO=" + net.ToString().Replace(",",".") + ", ";
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
                                    //sptrzedaż na fakturę
                                    typDok = "FS";
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
    }
}
