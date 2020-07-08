using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LibKonfIAI
{
    public static class OrdersIAI
    {
        //int magID, magID2;
        //private string usrNam;

        public static string setIAIapiFlag(string orderId)
        {
            string komunikatZwrotny = "OK";
            if (DataSessionIAI.GetPopertySettingsForAIA())
            {
                var binding = new BasicHttpBinding();
                var address = new EndpointAddress("http://" + DataSessionIAI.GetIAIDomainForCurrentSession() + "/api/?gate=orders/update/107/soap");
                var client = new ApiOrdersServiceUpdate.ApiOrdersPortTypeClient(binding, address);

                var request = new ApiOrdersServiceUpdate.requestType();

                request.authenticate = new ApiOrdersServiceUpdate.authenticateType();
                request.authenticate.userLogin = DataSessionIAI.GetIAILoginForCurrentSession();
                request.authenticate.authenticateKey = DataSessionIAI.GetIAIKeyForCurrentSession();

                request.@params = new ApiOrdersServiceUpdate.paramsType();

                request.@params.orders = new ApiOrdersServiceUpdate.orderType[1];
                request.@params.orders[0] = new ApiOrdersServiceUpdate.orderType();
                request.@params.orders[0].orderId = orderId;
                //request.@params.orders[0].orderSerialNumber = 1;
                //request.@params.orders[0].orderSerialNumberSpecified = true;
                //request.@params.orders[0].orderStatus = "orderStatus";

                //ustawienie flagi "registered_pos"
                request.@params.orders[0].apiFlag = ApiOrdersServiceUpdate.apiFlagType.registered_pos;
                request.@params.orders[0].apiFlagSpecified = true;

                //request.@params.orders[0].apiNoteToOrder = "apiNoteToOrder";

                try
                {
                    ApiOrdersServiceUpdate.responseType response = client.update(request);

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

        public static string setIAIapiStatus(string orderId, string statusZamowieniaWgRaks)
        {
            string komunikatZwrotny = "OK";
            if (DataSessionIAI.GetPopertySettingsForAIA())
            {
                var binding = new BasicHttpBinding();
                var address = new EndpointAddress("http://" + DataSessionIAI.GetIAIDomainForCurrentSession() + "/api/?gate=orders/update/107/soap");
                var client = new ApiOrdersServiceUpdate.ApiOrdersPortTypeClient(binding, address);

                var request = new ApiOrdersServiceUpdate.requestType();

                request.authenticate = new ApiOrdersServiceUpdate.authenticateType();
                request.authenticate.userLogin = DataSessionIAI.GetIAILoginForCurrentSession();
                request.authenticate.authenticateKey = DataSessionIAI.GetIAIKeyForCurrentSession();

                request.@params = new ApiOrdersServiceUpdate.paramsType();

                request.@params.orders = new ApiOrdersServiceUpdate.orderType[1];
                request.@params.orders[0] = new ApiOrdersServiceUpdate.orderType();
                request.@params.orders[0].orderId = orderId;
                //request.@params.orders[0].orderSerialNumber = 1;
                //request.@params.orders[0].orderSerialNumberSpecified = true;
                
                if (statusZamowieniaWgRaks.Equals("OK") || statusZamowieniaWgRaks.Equals("NA_MAGAZYNIE2"))
                    request.@params.orders[0].orderStatus = "packed";
                else if (statusZamowieniaWgRaks.Equals("DO_PRZESUNIĘCIA") || statusZamowieniaWgRaks.Equals("NA_ZAMÓWIENIE"))
                    request.@params.orders[0].orderStatus = "on_order";

                //ustawienie flagi "registered_pos"
                request.@params.orders[0].apiFlag = ApiOrdersServiceUpdate.apiFlagType.registered_pos;
                request.@params.orders[0].apiFlagSpecified = true;

                //request.@params.orders[0].apiNoteToOrder = "apiNoteToOrder";

                try
                {
                    ApiOrdersServiceUpdate.responseType response = client.update(request);

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
