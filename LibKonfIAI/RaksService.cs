using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LibKonfIAI
{
    class RaksService
    {
        public static string saveNewOrderAsInvoiceToRaks(ConnectionFB polaczenieFB,string orderIdIAI, int magId)
        {
            string komunikatZwrotny = "OK";

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
                        komunikatZwrotny = "1001 Kod: " + response.errors.faultCode + "; Opis:" +  response.errors.faultString;
                        ConnectionFB.setErrOrLogMsg("ERROR", "Błąd wczytania zamówienia " + orderIdIAI + " do biblioteki zapisu nowego zamówienia jako GM_FS do Raks." + System.Environment.NewLine + komunikatZwrotny);
                    }
                    else
                    {
                        if (response.resultsNumberAll == 0)
                        {
                            komunikatZwrotny = "1003 INFO Nie pobrano zamówienia " + orderIdIAI + " ze sklepu IAI - zamówienie nie istnieje!";
                            ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek wczytania zamówienia " + orderIdIAI + " do biblioteki zapisu nowego zamówienia jako GM_FS do Raks." + System.Environment.NewLine + komunikatZwrotny);
                        }
                        else
                        {
                            //mamy zamówienie można działać dalej
                        }
                    }
                }
                catch (Exception exr)
                {
                    komunikatZwrotny = "1002 Błąd-Wyjątek wczytania zamówienia z IAI; Opis:" + exr.Message;
                    ConnectionFB.setErrOrLogMsg("ERROR", "Błąd-wyjątek wczytania zamówienia " + orderIdIAI + " do biblioteki zapisu nowego zamówienia jako GM_FS do Raks." + System.Environment.NewLine + komunikatZwrotny);
                    throw;
                }

            }
            
            return komunikatZwrotny;
        }
    }
}
