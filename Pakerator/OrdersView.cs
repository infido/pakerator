using FirebirdSql.Data.FirebirdClient;
using Pakerator.ApiGetOrdersNotFinishedGet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibKonfIAI;
using Microsoft.Win32;

namespace Pakerator
{
    public partial class OrdersView : Form
    {
        public ConnectionDB polaczenie;
        //private FbDataAdapter fda;
        //private DataSet fds;
        //private DataView fDataView;
        int magID, magID2;
        string usrNam;
        string filtrStatus;
        List<Order> orders;
        List<OrderItem> orderItems;

        private void bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        public OrdersView(int magazyn1, int magazyn2, ConnectionDB conn, string userName, string statusZamowien, string tytulOkna)
        {
            InitializeComponent();
            magID = magazyn1;
            magID2 = magazyn2;
            polaczenie = conn;
            usrNam = userName;
            filtrStatus = statusZamowien;
            Text += " " + tytulOkna;

            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator");
            if (rejestr.GetValue("OrderToGloveSettings") != null)
            {
                if (((int)rejestr.GetValue("OrderToGloveSettings"))==1)
                    cSaveStastusToIAI.Checked = true;
                else



                    cSaveStastusToIAI.Checked = false;
            }
        }

        private void bRefresh_Click(object sender, EventArgs e)
        {
            lkomunikat.Visible = false;
            string kodForTry = "";

            if (SessionIAI.GetPopertySettingsForAIA())
            {

                orders = new List<Order>();

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

                if (filtrStatus.Length > 0)
                {
                    request.@params.ordersStatuses = new string[1];
                    request.@params.ordersStatuses[0] = filtrStatus;
                }
                else
                {
                    request.@params.ordersStatuses = new string[7];
                    request.@params.ordersStatuses[0] = "on_order";
                    request.@params.ordersStatuses[1] = "new";
                    request.@params.ordersStatuses[2] = "packed";
                    request.@params.ordersStatuses[3] = "ready";
                    request.@params.ordersStatuses[4] = "payment_waiting";
                    request.@params.ordersStatuses[5] = "delivery_waiting";
                    request.@params.ordersStatuses[6] = "suspended";
                }

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
                request.@params.ordersRange.ordersDateRange.ordersDateBegin = DateTime.Now.AddDays(-((Double)nDniWstecz.Value)).ToString("yyyy-MM-dd 00:00:00");
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

                request.@params.ordersBy = new orderByType[1];
                request.@params.ordersBy[0] = new orderByType();
                request.@params.ordersBy[0].elementName = "order_time";
                request.@params.ordersBy[0].sortDirection = "ASC";

                //request.@params.searchingOperatorTypeMatch = ApiGetOrdersNotFinishedGet.operatorTypeMatch.no_assignment;
                //request.@params.searchingOperatorTypeMatchSpecified =  true;
                //request.@params.ordersDelayed = ApiGetOrdersNotFinishedGet.ordersDelayedType.y;
                //request.@params.ordersDelayedSpecified = true;

                //request.@params.clientRequestInvoice = "clientRequestInvoice";

                try
                {
                    ApiGetOrdersNotFinishedGet.responseType response = client.getOrdersNotFinished(request);

                    if (response.errors.faultCode != 0)
                    {
                        lkomunikat.Text = response.errors.faultString;
                        lkomunikat.Visible = true;
                    }
                    else
                    {
                        lkomunikat.Visible = true;
                        lkomunikat.Text = "Pobrano informacje ze sklepu www o " + response.resultsNumberAll + " zamówieniach";
                        foreach (ApiGetOrdersNotFinishedGet.ResultType www in response.Results)
                        {
                            Order nag = new Order();
                            #region 037 try na mapowanie nagłówków
                            try
                            {
                                    nag.OrderId = www.orderId;
                                    nag.OrderStatus = www.orderDetails.orderStatus;
                                    nag.OrderAddDate = www.orderDetails.orderAddDate;
                                    nag.OrderPaymentType = www.orderDetails.payments.orderPaymentType;
                                    nag.OrderConfirmation = www.orderDetails.orderConfirmation;
                                    nag.CourierName = www.orderDetails.dispatch.courierName;
                                    nag.DeliveryDate = www.orderDetails.dispatch.deliveryDate;
                                    nag.OrderBridgeNote = www.orderBridgeNote;
                                    nag.OrderSerialNumber = www.orderSerialNumber;
                                    nag.ClientFirm = www.clientResult.clientBillingAddress.clientFirm;
                                    nag.ClientFirstName = www.clientResult.clientBillingAddress.clientFirstName;
                                    nag.ClientLastName = www.clientResult.clientBillingAddress.clientLastName;
                                    nag.ClientNip = www.clientResult.clientBillingAddress.clientNip;
                                    nag.ClientCountryName = www.clientResult.clientBillingAddress.clientCountryName;
                                    nag.ClientCity = www.clientResult.clientBillingAddress.clientCity;
                                    nag.ClientZipCode = www.clientResult.clientBillingAddress.clientZipCode;
                                    nag.ClientStreet = www.clientResult.clientBillingAddress.clientStreet;
                                    nag.ClientPhone1 = www.clientResult.clientBillingAddress.clientPhone1;
                                    nag.ClientPhone2 = www.clientResult.clientBillingAddress.clientPhone2;
                                    nag.ClientEmail = www.clientResult.clientAccount.clientEmail;
                                    nag.ClientId = www.clientResult.clientAccount.clientId;
                                    nag.ClientLogin = www.clientResult.clientAccount.clientLogin;
                                nag.ClientNoteToOrder = www.orderDetails.clientNoteToOrder;
                                nag.ApiFlag = www.orderDetails.apiFlag;
                            }
                            catch (Exception exn)
                            {
                                Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "037 Bład w mapowaniu nagłówków zamówień z www do obiektów dla odrerId: " + www.orderId + System.Environment.NewLine + exn.Message, "", "", "", 0, "", 0, "", magID, www.clientResult.clientBillingAddress.clientFirstName + " " + www.clientResult.clientBillingAddress.clientLastName + " " + www.clientResult.clientBillingAddress.clientFirm, 0, 0);
                                MessageBox.Show("037 Bład w mapowaniu nagłówków zamówień z www do obiektów dla odrerId: " + www.orderId + System.Environment.NewLine + exn.Message);
                                throw;
                            }
                            #endregion

                            orderItems = new List<OrderItem>();
                            foreach (ApiGetOrdersNotFinishedGet.productResultType pozwww in www.orderDetails.productsResults)
                            {
                                OrderItem poz = new OrderItem();
                                #region 038 try na mapowanie pozycji
                                try
                                {
                                    poz.BasketPosition = pozwww.basketPosition;
                                    poz.ProductId = pozwww.productId;
                                    poz.ProductCode = pozwww.productCode;
                                    poz.ProductName = pozwww.productName;
                                    poz.VersionName = pozwww.versionName;
                                    poz.ProductQuantity = pozwww.productQuantity;
                                    poz.ProductOrderPriceNetBaseCurrency = pozwww.productOrderPriceNet;
                                    poz.ProductOrderPriceBaseCurrency = pozwww.productOrderPrice;
                                    poz.RemarksToProduct = pozwww.remarksToProduct;
                                    poz.SizeId = pozwww.sizeId;
                                    poz.StockId = pozwww.stockId;
                                    poz.ProductSizeCodeExternal = pozwww.productSizeCodeExternal;
                                    poz.SizePanelName = pozwww.sizePanelName;
                                }
                                catch (Exception exp)
                                {
                                    Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "038 Bład w mapowaniu pozycji zamówień z www do obiektów dla odrerId: " + www.orderId + "; Index external:" + poz.ProductSizeCodeExternal + "; ProductId:" + poz.ProductId + System.Environment.NewLine + exp.Message, "", "", "", 0, "", 0, "", magID, www.clientResult.clientBillingAddress.clientFirstName + " " + www.clientResult.clientBillingAddress.clientLastName + " " + www.clientResult.clientBillingAddress.clientFirm, 0, 0);
                                    MessageBox.Show("038 Bład w mapowaniu pozycji zamówień z www do obiektów dla odrerId: " + www.orderId + "; Index external:" + poz.ProductSizeCodeExternal + "; ProductId:" + poz.ProductId + System.Environment.NewLine + exp.Message);
                                    throw;
                                }
                                #endregion

                                kodForTry = "Order id: " + nag.OrderId + "; Index external:"+ poz.ProductSizeCodeExternal + "; ProductId:" + poz.ProductId;

                                string sqlTest = "SELECT ARCHIWALNY from GM_TOWARY ";
                                sqlTest += " where ( GM_TOWARY.SKROT='" + pozwww.productSizeCodeExternal + "' OR GM_TOWARY.SKROT2='" + pozwww.productSizeCodeExternal + "' )";
                                FbCommand cdkTest = new FbCommand(sqlTest, polaczenie.getConnection());

                                #region 033 try czy jest kartoteka
                                try
                                {
                                    var wynik = cdkTest.ExecuteScalar();
                                    if (wynik == null)
                                        poz.Status = "BRAK";
                                    else if (Convert.ToInt32(wynik) == 1)
                                        poz.Status = "ARCHIWALNY";
                                }
                                catch (Exception tex)
                                {
                                    Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "033 Bład zapytania o indeks w Raks magazynu przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar:" + pozwww.productSizeCodeExternal + System.Environment.NewLine + tex.Message, "", "", "", 0, "", 0, "", magID, nag.ClientFirstName + " " + nag.ClientLastName + " " + nag.ClientFirm, 0, 0);
                                    MessageBox.Show("033 Bład zapytania o indeks w Raks magazynu przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar: " + pozwww.productSizeCodeExternal + System.Environment.NewLine + tex.Message);
                                }
                                #endregion

                                #region 039 try na sprawdzenie czy BRAK i poźniejsze wyliczenie stanów magazynowych
                                try
                                {
                                    if (poz.Status == null || !poz.Status.Equals("BRAK"))
                                    {

                                        string sql = "SELECT sum(ILOSC) from GM_MAGAZYN join GM_TOWARY on ID_TOWAR=GM_TOWARY.ID_TOWARU ";
                                        sql += " where ";
                                        sql += " ( GM_TOWARY.SKROT='" + pozwww.productSizeCodeExternal + "' OR GM_TOWARY.SKROT2='" + pozwww.productSizeCodeExternal + "' )";
                                        sql += " and GM_MAGAZYN.MAGNUM=" + magID + ";";
                                        FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
                                        #region 032 try stan magazynu
                                        try
                                        {
                                            var mg = cdk.ExecuteScalar();
                                            if (mg == DBNull.Value)
                                                poz.Magazyn = 0;
                                            else
                                                poz.Magazyn = Convert.ToSingle(mg);

                                        }
                                        catch (FbException ex)
                                        {
                                            Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "032 Bład zapytania o stany magazynu przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar:" + pozwww.productSizeCodeExternal + System.Environment.NewLine + ex.Message, "", "", "", 0, "", 0, "", magID, nag.ClientFirstName + " " + nag.ClientLastName + " " + nag.ClientFirm, 0, 0);
                                            poz.Magazyn = -2;
                                            MessageBox.Show("032 Bład zapytania o stany magazynu przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar: " + pozwww.productSizeCodeExternal + System.Environment.NewLine + ex.Message);
                                        }
                                        #endregion

                                        if (magID != magID2)
                                        {
                                            sql = "SELECT sum(ILOSC) from GM_MAGAZYN join GM_TOWARY on ID_TOWAR=GM_TOWARY.ID_TOWARU ";
                                            sql += " where ";
                                            sql += " ( GM_TOWARY.SKROT='" + pozwww.productSizeCodeExternal + "' OR GM_TOWARY.SKROT2='" + pozwww.productSizeCodeExternal + "' )";
                                            sql += " and GM_MAGAZYN.MAGNUM=" + magID2 + ";";
                                            cdk = new FbCommand(sql, polaczenie.getConnection());
                                            #region 043 try stan magazynu
                                            try
                                            {
                                                var mg = cdk.ExecuteScalar();
                                                if (mg == DBNull.Value)
                                                    poz.Magazyn2 = 0;
                                                else
                                                    poz.Magazyn2 = Convert.ToSingle(mg);

                                            }
                                            catch (FbException ex)
                                            {
                                                Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "043 Bład zapytania o stany magazynu2 przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar:" + pozwww.productSizeCodeExternal + System.Environment.NewLine + ex.Message, "", "", "", 0, "", 0, "", magID, nag.ClientFirstName + " " + nag.ClientLastName + " " + nag.ClientFirm, 0, 0);
                                                poz.Magazyn = -2;
                                                MessageBox.Show("043 Bład zapytania o stany magazynu2 przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar: " + pozwww.productSizeCodeExternal + System.Environment.NewLine + ex.Message);
                                            }
                                            #endregion
                                        }

                                        sql = "SELECT sum(ILOSC) from GM_MAGAZYN join GM_TOWARY on ID_TOWAR=GM_TOWARY.ID_TOWARU ";
                                        sql += " where ";
                                        sql += " ( GM_TOWARY.SKROT='" + pozwww.productSizeCodeExternal + "' OR GM_TOWARY.SKROT2='" + pozwww.productSizeCodeExternal + "' )";
                                        if (magID!=magID2)
                                            sql += " and GM_MAGAZYN.MAGNUM<>" + magID + "and GM_MAGAZYN.MAGNUM<>" + magID2 + ";";
                                        else
                                            sql += " and GM_MAGAZYN.MAGNUM<>" + magID + ";";

                                        cdk = new FbCommand(sql, polaczenie.getConnection());

                                        #region 034 try stan dla pozostałych magazynów
                                        try
                                        {
                                            var im = cdk.ExecuteScalar();
                                            if (im != DBNull.Value)
                                                poz.InneMagazyny = Convert.ToSingle(im);
                                            else
                                                poz.InneMagazyny = 0;
                                        }
                                        catch (FbException ex)
                                        {
                                            Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "034 Bład zapytania o stany magazynu przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar:" + pozwww.productSizeCodeExternal + System.Environment.NewLine + ex.Message, "", "", "", 0, "", 0, "", magID, nag.ClientFirstName + " " + nag.ClientLastName + " " + nag.ClientFirm, 0, 0);
                                            poz.InneMagazyny = -2;
                                            MessageBox.Show("034 Bład zapytania o stany magazynu przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar:" + pozwww.productSizeCodeExternal + System.Environment.NewLine + ex.Message);
                                        }
                                        #endregion

                                        #region 040 try na sprawdzenie statusu pozycji zamówienia
                                        try
                                        {
                                            if (poz.Status!=null && poz.Status.Equals("ARCHIWALNY"))
                                                poz.Status += " ";

                                            if ((poz.Magazyn - poz.ProductQuantity) >= 0)
                                                poz.Status += "OK";
                                            else if (((poz.Magazyn + poz.Magazyn2) - poz.ProductQuantity) >= 0)
                                                poz.Status += "NA_MAGAZYNIE2";
                                            else if (((poz.Magazyn + poz.Magazyn2 + poz.InneMagazyny) - poz.ProductQuantity) >= 0)
                                                poz.Status += "DO_PRZESUNIĘCIA";
                                            else
                                                poz.Status += "NA_ZAMÓWIENIE";
                                        }
                                        catch (Exception exck)
                                        {
                                            MessageBox.Show("040 Błąd sprawdzenia statusu pozycji dla zamówienia:" + www.orderId + " dla pozycji " + pozwww.productSizeCodeExternal + System.Environment.NewLine + exck.Message);
                                            throw;
                                        }
                                        #endregion
                                    }
                                }
                                catch (Exception exchb)
                                {
                                    MessageBox.Show("039 Błąd sprawdenia statusu nagłówka czy BRAK dla zamówienia:" + www.orderId + " dla pozycji " + pozwww.productSizeCodeExternal + System.Environment.NewLine + exchb.Message);
                                    throw;
                                }
                                #endregion
                                orderItems.Add(poz);

                                #region 041 try na sprawdzenei statusu nagłówka
                                try
                                {
                                    if (nag.StatusStanowRaks == null || nag.StatusStanowRaks.Length == 0)
                                        nag.StatusStanowRaks = poz.Status;
                                    else if (poz.Status.Equals("BRAK"))
                                        nag.StatusStanowRaks = poz.Status;
                                    else if (nag.StatusStanowRaks.Equals("OK") && poz.Status.Equals("NA_MAGAZYNIE2"))
                                        nag.StatusStanowRaks = poz.Status;
                                    else if ((nag.StatusStanowRaks.Equals("OK") || nag.StatusStanowRaks.Equals("NA_MAGAZYNIE2")) && poz.Status.Equals("DO_PRZESUNIĘCIA"))
                                        nag.StatusStanowRaks = poz.Status;
                                    else if ((nag.StatusStanowRaks.Equals("OK") || nag.StatusStanowRaks.Equals("NA_MAGAZYNIE2") || nag.StatusStanowRaks.Equals("DO_PRZESUNIĘCIA")) && poz.Status.Equals("NA_ZAMÓWIENIE"))
                                        nag.StatusStanowRaks = poz.Status;
                                    else if ((nag.StatusStanowRaks.Equals("OK") || nag.StatusStanowRaks.Equals("NA_MAGAZYNIE2") || nag.StatusStanowRaks.Equals("DO_PRZESUNIĘCIA") || nag.StatusStanowRaks.Equals("NA_ZAMÓWIENIE")) && poz.Status.StartsWith("ARCHI"))
                                        nag.StatusStanowRaks = poz.Status;
                                }
                                catch (Exception exckn)
                                {
                                    MessageBox.Show("041 Błąd sprawdenia statusu nagłówka dla zamówienia:" + www.orderId + " dla pozycji " + pozwww.productSizeCodeExternal + System.Environment.NewLine + exckn.Message);
                                    throw;
                                }
                                #endregion
                            }

                            nag.ItemsOfOrder = orderItems;
                            orders.Add(nag);
                        }

                        dataGridView1Naglowki.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                        dataGridView1Naglowki.AutoGenerateColumns = true;
                        dataGridView1Naglowki.DataSource = orders;

                        Pulpit.putLog(polaczenie, usrNam, "REPORT", "703 Widok zamówień z pozycjami, pobrano i wyświetlono:" + orders.Count + " zamówień", "", "", "", 0, "", 0, "", magID, "", 0, 0);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("036 Błąd: " + kodForTry + " Esc.msg:" + ex.Message.ToString());
                    Pulpit.putLog(polaczenie, usrNam, "ERROR", "036 Błąd: " + kodForTry + " Esc.msg:" + ex.Message.ToString(), "", "", "", 0, "", 0, "", magID, "", 0, 0);
                    throw;
                }
                setKolorowanieNAG();
            }
        }

        private void dataGridView1Naglowki_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView2Pozycje.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Order cur = orders.Find(x => x.OrderId == dataGridView1Naglowki.CurrentRow.Cells["orderId"].Value.ToString());
            if (cur != null)
            {
                dataGridView2Pozycje.DataSource = cur.ItemsOfOrder;
                setKolorowaniePOZ();
            }

            if (dataGridView1Naglowki.CurrentRow.Cells["StatusStanowRaks"].Value.Equals("BRAK") ||
                dataGridView1Naglowki.CurrentRow.Cells["StatusStanowRaks"].Value.ToString().StartsWith("ARCHI"))
            {
                bZapiszPozDoSchowkaRaks.Visible = false;
            }
            else
            {
                bZapiszPozDoSchowkaRaks.Visible = true;
                bZapiszPozDoSchowkaRaks.Text = "Zapisz pozycje zamówienia " + dataGridView1Naglowki.CurrentRow.Cells["orderSerialNumber"].Value.ToString()  + " do schowka w RaksSQL";
            }
        }

        public void Pokaz()
        {
            Show();
        }

        private void setKolorowanieNAG()
        {
            foreach (DataGridViewRow row in dataGridView1Naglowki.Rows)
            {
                if (row.Cells["orderStatus"].Value.ToString().Equals("new"))
                {
                    row.DefaultCellStyle.ForeColor = Color.Blue;
                }
                row.DefaultCellStyle.SelectionBackColor = Color.WhiteSmoke;

                if (row.Cells["StatusStanowRaks"].Value.Equals("OK"))
                {
                    row.DefaultCellStyle.BackColor = Color.GreenYellow;
                    row.DefaultCellStyle.SelectionForeColor = Color.Green;
                }
                else if (row.Cells["StatusStanowRaks"].Value.Equals("NA_MAGAZYNIE2"))
                {
                    row.DefaultCellStyle.BackColor = Color.DarkGreen;
                    row.DefaultCellStyle.ForeColor = Color.GreenYellow;
                    row.DefaultCellStyle.SelectionForeColor = Color.DarkGreen;
                }
                else if (row.Cells["StatusStanowRaks"].Value.Equals("DO_PRZESUNIĘCIA"))
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                    row.DefaultCellStyle.SelectionForeColor = Color.YellowGreen;
                }
                else if (row.Cells["StatusStanowRaks"].Value.Equals("NA_ZAMÓWIENIE"))
                {
                    row.DefaultCellStyle.BackColor = Color.Orange;
                    row.DefaultCellStyle.SelectionForeColor = Color.Orange;
                }
                else if (row.Cells["StatusStanowRaks"].Value.ToString().StartsWith("ARCHI"))
                {
                    row.DefaultCellStyle.BackColor = Color.Gray;
                    row.DefaultCellStyle.SelectionForeColor = Color.Gray;
                }
                else if (row.Cells["StatusStanowRaks"].Value.Equals("BRAK"))
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.SelectionForeColor = Color.Red;
                }
            }
        }

        private string setSQLInsertSchowek(string indeks, string schoNaz, string user, string ilosc, string cena, string cenaNet)
        {
            FbCommand gen_id_schowek = new FbCommand("SELECT GEN_ID(GM_SCHOWEK_POZYCJI_GEN,1) from rdb$database", polaczenie.getConnection());
            int idscho = 0;

            #region pobranie nowego id z bazy
            try
            {
                idscho = Convert.ToInt32(gen_id_schowek.ExecuteScalar());
            }
            catch (FbException exgen)
            {
                MessageBox.Show("Błąd pobierania nowego pozycji schowka w RaksSQL ID z bazy. Operacje przerwano! " + exgen.Message);
                return "";
                throw;
            }
            #endregion

            StringBuilder myStringBuilder = new StringBuilder("INSERT INTO GM_SCHOWEK_POZYCJI (");
            myStringBuilder.Append("ID, ");
            myStringBuilder.Append("OPERATOR, ");
            myStringBuilder.Append("IDENTYFIKATOR, ");
            myStringBuilder.Append("PUBLICZNA, ");
            myStringBuilder.Append("ID_TOWARU, ");
            myStringBuilder.Append("ILOSC, ");
            myStringBuilder.Append("CENA_SP_PLN_NETTO_PO_RAB, ");
            myStringBuilder.Append("CENA_SP_PLN_BRUTTO_PO_RAB, ");
            myStringBuilder.Append("CENA_SP_PLN_NETTO_PRZED_RAB, ");
            myStringBuilder.Append("CENA_SP_PLN_BRUTTO_PRZED_RAB, ");
            myStringBuilder.Append("CENA_SP_WAL_NETTO_PRZED_RAB, ");
            myStringBuilder.Append("CENA_SP_WAL_BRUTTO_PRZED_RAB, ");
            myStringBuilder.Append("CENA_SP_WAL_NETTO_PO_RAB, ");
            myStringBuilder.Append("CENA_SP_WAL_BRUTTO_PO_RAB, ");
            myStringBuilder.Append("CENA_KATALOGOWA_NETTO, ");
            myStringBuilder.Append("CENA_KATALOGOWA_BRUTTO, ");
            myStringBuilder.Append("ZNACZNIKI, ");
            myStringBuilder.Append("UWAGI");

            myStringBuilder.Append(") VALUES ( ");

            myStringBuilder.Append(idscho.ToString() + ",");  // ID
            myStringBuilder.Append("'" + user + "', ");  // OPERATOR
            myStringBuilder.Append("'" + schoNaz + "', ");  //IDENTYFIKATOR
            myStringBuilder.Append("1, ");  //1-PUBLICZNA, 0-PRYWATNA

            myStringBuilder.Append(indeks + ", ");  //ID_TOWARU

            myStringBuilder.Append(ilosc.ToString().Replace(",", ".") + ", ");  //ILOSC
            myStringBuilder.Append(cenaNet.ToString().Replace(",", ".") + ", ");  //CENA_SP_PLN_NETTO_PO_RAB
            myStringBuilder.Append(cena.ToString().Replace(",", ".") + ", ");  //CENA_SP_PLN_BRUTTO_PO_RAB
            myStringBuilder.Append(cenaNet.ToString().Replace(",", ".") + ", ");  //CENA_SP_PLN_NETTO_PRZED_RAB
            myStringBuilder.Append(cena.ToString().Replace(",", ".") + ", ");  //CENA_SP_PLN_BRUTTO_PRZED_RAB
            myStringBuilder.Append(cenaNet.ToString().Replace(",", ".") + ", ");  //CENA_SP_WAL_NETTO_PRZED_RAB
            myStringBuilder.Append(cena.ToString().Replace(",", ".") + ", ");  //CCENA_SP_WAL_BRUTTO_PRZED_RAB
            myStringBuilder.Append(cenaNet.ToString().Replace(",", ".") + ", ");  //CENA_SP_WAL_NETTO_PO_RAB
            myStringBuilder.Append(cena.ToString().Replace(",", ".") + ", ");  //CCENA_SP_WAL_BRUTTO_PO_RAB

            myStringBuilder.Append(cenaNet.ToString().Replace(",", ".") + ", ");  //CENA_KATALOGOWA_NETTO
            myStringBuilder.Append(cena.ToString().Replace(",", ".") + ", ");  //CENA_KATALOGOWA_BRUTTO
            myStringBuilder.Append("NULL, ");  //ZNACZNIKI
            myStringBuilder.Append("NULL");  //UWAGI
            myStringBuilder.Append(");");
            return myStringBuilder.ToString();
        }

        private void bZapiszPozDoSchowkaRaks_Click(object sender, EventArgs e)
        {
            //dane nagłówka schowka
            string identf = "IAI " + dataGridView1Naglowki.CurrentRow.Cells["orderSerialNumber"].Value.ToString() + " " + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString() + " " + DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00");

            //zapis pozycji
            int count = 0;
            foreach (DataGridViewRow row in dataGridView2Pozycje.Rows)
            {
                string idtow = "0";
                string sql;

                #region obliczanie ID towaru z Indeksu
                sql = "SELECT ID_TOWARU from GM_TOWARY where SKROT='" + row.Cells["productSizeCodeExternal"].Value.ToString() + "';";

                FbCommand gen_id_towaru = new FbCommand(sql, polaczenie.getConnection());
                try
                {
                    if (gen_id_towaru.ExecuteScalar() != null)
                    {
                        idtow = (gen_id_towaru.ExecuteScalar()).ToString();
                    }
                    else
                    {
                        MessageBox.Show("Nie odnaleziono w RaksSQL towaru i indeksie: " + row.Cells["productSizeCodeExternal"].Value.ToString() + System.Environment.NewLine + "Zapis doschowka przerwano!" );
                        break;
                    }
                }
                catch (FbException exgen)
                {
                    MessageBox.Show("Błąd pobierania nowego ID_TOWARU na podstawie skrótu" + row.Cells["productSizeCodeExternal"].Value.ToString() + " . Operacje przerwano! " + exgen.Message);
                    throw;
                }

                #endregion

                sql = setSQLInsertSchowek(idtow, identf, usrNam, row.Cells["productQuantity"].Value.ToString(), row.Cells["productOrderPriceBaseCurrency"].Value.ToString(), row.Cells["productOrderPriceNetBaseCurrency"].Value.ToString());
                FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
                try
                {
                    cdk.ExecuteScalar();
                }
                catch (FbException ex)
                {
                    MessageBox.Show("Błąd zapisu danych do schowka z okna z zamówień ze sklepu www: " + ex.Message);
                }
                count++;
            }
            MessageBox.Show("Zapisano w schowku RaksSQL " + count + " rekord(ów)");

            string res = OrdersIAI.setIAIapiFlag(dataGridView1Naglowki.CurrentRow.Cells["orderId"].Value.ToString());
            MessageBox.Show(res,"Wynik operacji zmiany statusu dla " + dataGridView1Naglowki.CurrentRow.Cells["orderSerialNumber"].Value.ToString());
            Pulpit.putLog(polaczenie, usrNam, "API", res, "", "", "", 0, "", 0, "", magID, "", 0, 0);
        }

        private void cSaveStastusToIAI_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey rejestr = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Infido\\Pakerator", true);
            if (cSaveStastusToIAI.Checked)
                rejestr.SetValue("OrderToGloveSettings", 1);
            else
                rejestr.SetValue("OrderToGloveSettings", 0);
        }

        private void bAddCompanyToRaks_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Funkcjonalność jeszcze niedostępna w tej wersji...");
        }

        private void setKolorowaniePOZ()
        {
            foreach (DataGridViewRow row in dataGridView2Pozycje.Rows)
            {
                row.DefaultCellStyle.SelectionBackColor = Color.WhiteSmoke;

                if (row.Cells["status"].Value.Equals("OK"))
                {
                    row.DefaultCellStyle.BackColor = Color.GreenYellow;
                    row.DefaultCellStyle.SelectionForeColor = Color.Green;
                }
                else if (row.Cells["status"].Value.Equals("NA_MAGAZYNIE2"))
                {
                    row.DefaultCellStyle.BackColor = Color.DarkGreen;
                    row.DefaultCellStyle.ForeColor = Color.GreenYellow;
                    row.DefaultCellStyle.SelectionForeColor = Color.DarkGreen;
                }
                else if (row.Cells["status"].Value.Equals("DO_PRZESUNIĘCIA"))
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                    row.DefaultCellStyle.SelectionBackColor = Color.YellowGreen;
                }
                else if (row.Cells["status"].Value.Equals("NA_ZAMÓWIENIE"))
                {
                    row.DefaultCellStyle.BackColor = Color.Orange;
                    row.DefaultCellStyle.SelectionBackColor = Color.Orange;
                }
                else if (row.Cells["status"].Value.ToString().StartsWith("ARCHI"))
                {
                    row.DefaultCellStyle.BackColor = Color.Gray;
                    row.DefaultCellStyle.SelectionBackColor = Color.Gray;
                }
                else if (row.Cells["status"].Value.Equals("BRAK"))
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                    row.DefaultCellStyle.SelectionBackColor = Color.Red;
                }
            }
        }
    }

    public class Order
    {
        private string orderId;
        private string orderStatus;
        private string orderBridgeNote;
        private int orderSerialNumber;
        private int clientId;
        private string clientLogin;
        private string clientFirm;
        private string clientFirstName;
        private string clientLastName;
        private string clientNip;
        private string clientPhone1;
        private string clientPhone2;
        private string clientCountryName;
        private string clientCity;
        private string clientStreet;
        private string clientZipCode;
        private string clientEmail;
        private string orderAddDate;
        private string orderPaymentType;
        private string orderConfirmation;
        private string courierName;
        private string deliveryDate;
        private string statusStanowRaks;
        private string clientNoteToOrder;
        private apiFlagType apiFlag;

        private List<OrderItem> itemsOfOrder;

        public Order()
        {

        }
        public int OrderSerialNumber { get => orderSerialNumber; set => orderSerialNumber = value; }

        public string OrderId { get => orderId; set => orderId = value; }
        public string OrderStatus { get => orderStatus; set => orderStatus = value; }
        public apiFlagType ApiFlag { get => apiFlag; set => apiFlag = value; }
        public string OrderAddDate { get => orderAddDate; set => orderAddDate = value; }
        public string OrderPaymentType { get => orderPaymentType; set => orderPaymentType = value; }
        public string OrderConfirmation { get => orderConfirmation; set => orderConfirmation = value; }
        public string CourierName { get => courierName; set => courierName = value; }
        public string DeliveryDate { get => deliveryDate; set => deliveryDate = value; }
        public string ClientPhone1 { get => clientPhone1; set => clientPhone1 = value; }
        public string ClientNoteToOrder { get => clientNoteToOrder; set => clientNoteToOrder = value; }
        public string ClientCity { get => clientCity; set => clientCity = value; }
        public string ClientCountryName { get => clientCountryName; set => clientCountryName = value; }
        public string ClientFirm { get => clientFirm; set => clientFirm = value; }
        public string ClientFirstName { get => clientFirstName; set => clientFirstName = value; }
        public string ClientLastName { get => clientLastName; set => clientLastName = value; }
        public string ClientNip { get => clientNip; set => clientNip = value; }
        public string ClientStreet { get => clientStreet; set => clientStreet = value; }
        public string ClientZipCode { get => clientZipCode; set => clientZipCode = value; }
        public string ClientEmail { get => clientEmail; set => clientEmail = value; }
        public string ClientPhone2 { get => clientPhone2; set => clientPhone2 = value; }
        public int ClientId { get => clientId; set => clientId = value; }
        public string ClientLogin { get => clientLogin; set => clientLogin = value; }
        public string OrderBridgeNote { get => orderBridgeNote; set => orderBridgeNote = value; }
        public List<OrderItem> ItemsOfOrder { get => itemsOfOrder; set => itemsOfOrder = value; }
        public string StatusStanowRaks { get => statusStanowRaks; set => statusStanowRaks = value; }
        
    }

    public class OrderItem
    {
        private int basketPosition;
        private int productId;
        private string productName;
        private string productCode;
        private string versionName;
        private string sizeId;
        private int stockId;
        private float productQuantity;
        private float productOrderPriceNetBaseCurrency;
        private float productOrderPriceBaseCurrency;
        private string remarksToProduct;
        private float magazyn;
        private float magazyn2;
        private float inneMagazyny;
        private string status;
        private string sizePanelName;
        private string productSizeCodeExternal;

        public int BasketPosition { get => basketPosition; set => basketPosition = value; }
        public string ProductSizeCodeExternal { get => productSizeCodeExternal; set => productSizeCodeExternal = value; }
        public float ProductQuantity { get => productQuantity; set => productQuantity = value; }
        public float ProductOrderPriceNetBaseCurrency { get => productOrderPriceNetBaseCurrency; set => productOrderPriceNetBaseCurrency = value; }
        public float ProductOrderPriceBaseCurrency { get => productOrderPriceBaseCurrency; set => productOrderPriceBaseCurrency = value; }
        public float Magazyn { get => magazyn; set => magazyn = value; }
        public float Magazyn2 { get => magazyn2; set => magazyn2 = value; }
        public float InneMagazyny { get => inneMagazyny; set => inneMagazyny = value; }
        public string Status { get => status; set => status = value; }
        public string RemarksToProduct { get => remarksToProduct; set => remarksToProduct = value; }
        public int StockId { get => stockId; set => stockId = value; }
        public string ProductName { get => productName; set => productName = value; }
        public string VersionName { get => versionName; set => versionName = value; }
        public int ProductId { get => productId; set => productId = value; }
        public string SizePanelName { get => sizePanelName; set => sizePanelName = value; }
        public string ProductCode { get => productCode; set => productCode = value; }
        public string SizeId { get => sizeId; set => sizeId = value; }

    }
}
