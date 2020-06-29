using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LibKonfIAI
{
    public class OrdersIAI
    {
        int magID, magID2;
        private string usrNam;

        public static string setIAIapiFlag(string orderId)
        {
            string komunikatZwrotny = "OK";
            if (DataSessionIAI.GetPopertySettingsForAIA())
            {
                var binding = new BasicHttpBinding();
                var address = new EndpointAddress("http://" + DataSessionIAI.GetIAIDomainForCurrentSession() + "/api/?gate=orders/update/107/soap");
                var client = new ApiOrdersPortTypeUpdate.ApiOrdersPortTypeClient(binding, address);

                var request = new ApiOrdersPortTypeUpdate.requestType();

                request.authenticate = new ApiOrdersPortTypeUpdate.authenticateType();
                request.authenticate.userLogin = DataSessionIAI.GetIAILoginForCurrentSession();
                request.authenticate.authenticateKey = DataSessionIAI.GetIAIKeyForCurrentSession();

                request.@params = new ApiOrdersPortTypeUpdate.paramsType();

                request.@params.orders = new ApiOrdersPortTypeUpdate.orderType[1];
                request.@params.orders[0] = new ApiOrdersPortTypeUpdate.orderType();
                request.@params.orders[0].orderId = orderId;
                //request.@params.orders[0].orderSerialNumber = 1;
                //request.@params.orders[0].orderSerialNumberSpecified = true;
                //request.@params.orders[0].orderStatus = "orderStatus";

                //ustawienie flagi "registered_pos"
                request.@params.orders[0].apiFlag = ApiOrdersPortTypeUpdate.apiFlagType.registered_pos;
                request.@params.orders[0].apiFlagSpecified = true;

                //request.@params.orders[0].apiNoteToOrder = "apiNoteToOrder";

                try
                {
                    ApiOrdersPortTypeUpdate.responseType response = client.update(request);

                    if (response.errors.faultCode != 0)
                    {
                        komunikatZwrotny += response.errors.faultString;
                    }
                    else
                    {
                        komunikatZwrotny += "Wywołano poprawnie zmianę statusu zamówienia " + response.results.ordersResults[0].orderSerialNumber + " i otrzymano odpowiedź kod: " + response.results.ordersResults[0].faultCode + ";" + response.results.ordersResults[0].faultString;
                    }

                }
                catch (Exception apiEx)
                {
                    komunikatZwrotny += " Błąd! " + apiEx.Message;
                    throw;
                }
            }
            return komunikatZwrotny;
        }

    }


}
