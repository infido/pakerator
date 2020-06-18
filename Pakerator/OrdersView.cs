using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pakerator
{
    public partial class OrdersView : Form
    {
        public ConnectionDB polaczenie;
        private FbDataAdapter fda;
        private DataSet fds;
        private DataView fDataView;
        int magID, magID2;
        string usrNam;

        private void bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public OrdersView(int magazyn1, int magazyn2, ConnectionDB conn, string userName)
        {
            InitializeComponent();
            magID = magazyn1;
            magID2 = magazyn2;
            polaczenie = conn;
            usrNam = userName;
            //Pulpit.setL
        }

        private void bRefresh_Click(object sender, EventArgs e)
        {
            lkomunikat.Visible = false;
            
            var binding = new BasicHttpBinding();
            var address = new EndpointAddress("http://" + SessionIAI.GetIAIDomainForCurrentSession() + "/api/?gate=orders/getOrdersNotFinished/106/soap");
            var client = new ApiGetOrdersNotFinishedGet.ApiOrdersPortTypeClient(binding, address);
            binding.MaxReceivedMessageSize = 2000000; //2MB

            var request = new ApiGetOrdersNotFinishedGet.requestType();

            request.authenticate = new ApiGetOrdersNotFinishedGet.authenticateType();
            request.authenticate.userLogin = SessionIAI.GetIAILoginForCurrentSession();
            request.authenticate.authenticateKey = SessionIAI.GetIAIKeyForCurrentSession();

            request.@params = new ApiGetOrdersNotFinishedGet.paramsType();
            
            //request.@params.orderPrepaidStatus = "orderPrepaidStatus";

            request.@params.ordersStatuses = new string[1];
            request.@params.ordersStatuses[0] = "on_order";

            //request.@params.couriersName = new string[1];
            //request.@params.couriersName[0] = "couriersName";

            //request.@params.orderPaymentType = "orderPaymentType";
            //request.@params.ordersIds = new string[1];
            //request.@params.ordersIds[0] = "ordersIds";
            //request.@params.ordersSerialNumbers = new int[1];
            //request.@params.ordersSerialNumbers[0] = 1;

            //request.@params.clients = new ApiGetOrdersNotFinishedGet.clientType[1];
            //request.@params.clients[0] = new ApiGetOrdersNotFinishedGet.clientType();
            //request.@params.clients[0].clientLogin = "clientLogin";
            //request.@params.clients[0].clientFirstName = "clientFirstName";
            //request.@params.clients[0].clientLastName = "clientLastName";
            //request.@params.clients[0].clientCity = "clientCity";
            //request.@params.clients[0].clientEmail = "clientEmail";
            //request.@params.clients[0].clientHasTaxNumber = "clientHasTaxNumber";
            //request.@params.clients[0].clientSearchingMode = "clientSearchingMode";
            //request.@params.clients[0].clientFirm = "clientFirm";
            //request.@params.clients[0].clientCountryId = "clientCountryId";
            //request.@params.clients[0].clientCountryName = "clientCountryName";

            request.@params.ordersRange = new ApiGetOrdersNotFinishedGet.ordersRangeType();
            request.@params.ordersRange.ordersDateRange = new ApiGetOrdersNotFinishedGet.ordersDateRangeType();
            request.@params.ordersRange.ordersDateRange.ordersDateType = ApiGetOrdersNotFinishedGet.ordersDateTypeType.add;
            request.@params.ordersRange.ordersDateRange.ordersDateTypeSpecified = true;
            request.@params.ordersRange.ordersDateRange.ordersDatesTypes = new ApiGetOrdersNotFinishedGet.ordersDatesTypeType[1];
            request.@params.ordersRange.ordersDateRange.ordersDatesTypes[0] = ApiGetOrdersNotFinishedGet.ordersDatesTypeType.add;
            request.@params.ordersRange.ordersDateRange.ordersDateBegin = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd 00:00:00");
            request.@params.ordersRange.ordersDateRange.ordersDateEnd = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");

            //request.@params.orderSource = new ApiGetOrdersNotFinishedGet.orderSourceType();
            //request.@params.orderSource.shopsMask = 4;
            //request.@params.orderSource.shopsMaskSpecified = true;
            //request.@params.orderSource.shopsIds = new int[1];
            //request.@params.orderSource.shopsIds[0] = 5;

            //request.@params.orderSource.auctionsParams = new ApiGetOrdersNotFinishedGet.auctionsParamsType();
            //request.@params.orderSource.auctionsParams.auctionsServicesNames = new string[1];
            //request.@params.orderSource.auctionsParams.auctionsServicesNames[0] = "auctionsServicesNames";
            //request.@params.orderSource.auctionsParams.auctionsItemsIds = new int[1];
            //request.@params.orderSource.auctionsParams.auctionsItemsIds[0] = 6;
            //request.@params.orderSource.auctionsParams.auctionsAccounts = new ApiGetOrdersNotFinishedGet.auctionAccountType[1];
            //request.@params.orderSource.auctionsParams.auctionsAccounts[0] = new ApiGetOrdersNotFinishedGet.auctionAccountType();
            //request.@params.orderSource.auctionsParams.auctionsAccounts[0].auctionsAccountId = 7;
            //request.@params.orderSource.auctionsParams.auctionsAccounts[0].auctionsAccountIdSpecified = true;
            //request.@params.orderSource.auctionsParams.auctionsAccounts[0].auctionsAccountLogin = "auctionsAccountLogin";
            //request.@params.orderSource.auctionsParams.auctionsClients = new ApiGetOrdersNotFinishedGet.auctionClientType[1];
            //request.@params.orderSource.auctionsParams.auctionsClients[0] = new ApiGetOrdersNotFinishedGet.auctionClientType();
            //request.@params.orderSource.auctionsParams.auctionsClients[0].auctionClientId = "auctionClientId";
            //request.@params.orderSource.auctionsParams.auctionsClients[0].auctionClientLogin = "auctionClientLogin";

            //request.@params.products = new ApiGetOrdersNotFinishedGet.productType[1];
            //request.@params.products[0] = new ApiGetOrdersNotFinishedGet.productType();
            //request.@params.products[0].productId = 8;
            //request.@params.products[0].productIdSpecified = true;
            //request.@params.products[0].productName = "productName";
            //request.@params.products[0].sizeId = "sizeId";
            //request.@params.products[0].sizePanelName = "sizePanelName";

            //request.@params.resultsPage = 9;
            //request.@params.resultsPageSpecified = true;
            request.@params.resultsLimit = 300;
            request.@params.resultsLimitSpecified = true;

            //request.@params.packages = new ApiGetOrdersNotFinishedGet.packagesType();
            //request.@params.packages.packagesNumbers = new string[1];
            //request.@params.packages.packagesNumbers[0] = "packagesNumbers";
            //request.@params.packages.orderHasPackageNumbers = "orderHasPackageNumbers";

            //request.@params.stocks = new ApiGetOrdersNotFinishedGet.stockType[1];
            //request.@params.stocks[0] = new ApiGetOrdersNotFinishedGet.stockType();
            //request.@params.stocks[0].stockId = 11;
            //request.@params.stocks[0].stockIdSpecified = true;

            //request.@params.campaign = new ApiGetOrdersNotFinishedGet.campaignType();
            //request.@params.campaign.campaignId = 12;
            //request.@params.campaign.campaignIdSpecified = true;
            //request.@params.campaign.discountCodes = new string[1];
            //request.@params.campaign.discountCodes[0] = "discountCodes";

            //request.@params.loyaltyPointsMode = ApiGetOrdersNotFinishedGet.loyaltyPointsModeType.all;
            //request.@params.loyaltyPointsModeSpecified = true;
            //request.@params.orderOperatorLogin = "orderOperatorLogin";
            //request.@params.orderPackingPersonLogin = "orderPackingPersonLogin";

            //request.@params.ordersBy = new ApiGetOrdersNotFinishedGet.orderByType[1];
            //request.@params.ordersBy[0] = new ApiGetOrdersNotFinishedGet.orderByType();
            //request.@params.ordersBy[0].elementName = "elementName";
            //request.@params.ordersBy[0].sortDirection = "sortDirection";

            //request.@params.searchingOperatorTypeMatch = ApiGetOrdersNotFinishedGet.operatorTypeMatch.no_assignment;
            //request.@params.searchingOperatorTypeMatchSpecified =  true;
            //request.@params.ordersDelayed = ApiGetOrdersNotFinishedGet.ordersDelayedType.y;
            //request.@params.ordersDelayedSpecified = true;

            //request.@params.clientRequestInvoice = "clientRequestInvoice";

            try
            {
                ApiGetOrdersNotFinishedGet.responseType response = client.getOrdersNotFinished(request);

                if (response.errors.faultCodeSpecified)
                {
                    lkomunikat.Text = response.errors.faultString;
                    lkomunikat.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message.ToString());
                Pulpit.putLog(polaczenie, usrNam, "ERROR", ex.Message.ToString(), "", "", "", 0, "", 0, "", magID, "", 0, 0);
                throw;
            }
        }

        public void Pokaz()
        {
            Show();
        }
    }
}
