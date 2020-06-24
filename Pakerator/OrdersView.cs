using FirebirdSql.Data.FirebirdClient;
using Pakerator.ApiGetOrdersNotFinishedGet;
using System;
using System.Collections;
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
        //private FbDataAdapter fda;
        //private DataSet fds;
        //private DataView fDataView;
        int magID, magID2;
        string usrNam;
        List<Order> orders;
        List<OrderItem> orderItems;

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

                request.@params.ordersStatuses = new string[2];
                request.@params.ordersStatuses[0] = "on_order";
                request.@params.ordersStatuses[1] = "new";

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
                        foreach (ApiGetOrdersNotFinishedGet.ResultType www in response.Results)
                        {
                            Order nag = new Order();
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


                            orderItems = new List<OrderItem>();
                            foreach (ApiGetOrdersNotFinishedGet.productResultType pozwww in www.orderDetails.productsResults)
                            {
                                OrderItem poz = new OrderItem();
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

                                string sqlTest = "SELECT ARCHIWALNY from GM_TOWARY ";
                                sqlTest += " where ( GM_TOWARY.SKROT='" + pozwww.productSizeCodeExternal + "' OR GM_TOWARY.SKROT2='" + pozwww.productSizeCodeExternal + "' )";
                                FbCommand cdkTest = new FbCommand(sqlTest, polaczenie.getConnection());
                                try
                                {
                                    if (cdkTest.ExecuteScalar() == DBNull.Value)
                                        poz.Status = "BRAK";
                                    else if ((int)cdkTest.ExecuteScalar() == 1)
                                        poz.Status = "ARCHIWALNY";
                                }
                                catch (Exception tex)
                                {
                                    Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "033 Bład zapytania o indeks w Raks magazynu przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar:" + pozwww.productSizeCodeExternal + System.Environment.NewLine + tex.Message, "", "", "", 0, "", 0, "", magID, nag.ClientFirstName + " " + nag.ClientLastName + " " + nag.ClientFirm, 0, 0);
                                    MessageBox.Show("033 Bład zapytania o indeks w Raks magazynu przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar: " + pozwww.productSizeCodeExternal + System.Environment.NewLine + tex.Message);
                                }

                                if (poz.Status==null || !poz.Status.Equals("BRAK"))
                                {

                                    string sql = "SELECT sum(ILOSC) from GM_MAGAZYN join GM_TOWARY on ID_TOWAR=GM_TOWARY.ID_TOWARU ";
                                    sql += " where ";
                                    sql += " ( GM_TOWARY.SKROT='" + pozwww.productSizeCodeExternal + "' OR GM_TOWARY.SKROT2='" + pozwww.productSizeCodeExternal + "' )";
                                    sql += " and GM_MAGAZYN.MAGNUM=" + magID + ";";
                                    FbCommand cdk = new FbCommand(sql, polaczenie.getConnection());
                                    try
                                    {
                                        if (cdk.ExecuteScalar() != DBNull.Value)
                                            poz.Magazyn = (float)cdk.ExecuteScalar();
                                        else
                                            poz.Magazyn = 0;
                                    }
                                    catch (FbException ex)
                                    {
                                        Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "032 Bład zapytania o stany magazynu przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar:" + pozwww.productSizeCodeExternal + System.Environment.NewLine + ex.Message, "", "", "", 0, "", 0, "", magID, nag.ClientFirstName + " " + nag.ClientLastName + " " + nag.ClientFirm, 0, 0);
                                        poz.Magazyn = -2;
                                    }

                                    sql = "SELECT sum(ILOSC) from GM_MAGAZYN join GM_TOWARY on ID_TOWAR=GM_TOWARY.ID_TOWARU ";
                                    sql += " where ";
                                    sql += " ( GM_TOWARY.SKROT='" + pozwww.productSizeCodeExternal + "' OR GM_TOWARY.SKROT2='" + pozwww.productSizeCodeExternal + "' )";
                                    sql += " and GM_MAGAZYN.MAGNUM<>" + magID + ";";
                                    cdk = new FbCommand(sql, polaczenie.getConnection());
                                    try
                                    {
                                        if (cdk.ExecuteScalar() != DBNull.Value)
                                            poz.InneMagazyny = (float)cdk.ExecuteScalar();
                                        else
                                            poz.InneMagazyny = 0;
                                    }
                                    catch (FbException ex)
                                    {
                                        Pulpit.putLog(polaczenie, polaczenie.getCurrentUser(), "ERROR", "034 Bład zapytania o stany magazynu przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar:" + pozwww.productSizeCodeExternal + System.Environment.NewLine + ex.Message, "", "", "", 0, "", 0, "", magID, nag.ClientFirstName + " " + nag.ClientLastName + " " + nag.ClientFirm, 0, 0);
                                        poz.InneMagazyny = -2;
                                        MessageBox.Show("034 Bład zapytania o stany magazynu przy przeliczaniu rekordów prezentacji zamówień z www odrerId: " + nag.OrderId + ", Towar:" + pozwww.productSizeCodeExternal + System.Environment.NewLine + ex.Message);
                                    }

                                    if ((poz.Magazyn - poz.ProductQuantity) >= 0)
                                        poz.Status += "OK";
                                    else if (((poz.Magazyn + poz.InneMagazyny) - poz.ProductQuantity) >= 0)
                                        poz.Status += "DO_PRZESUNIĘCIA";
                                    else
                                        poz.Status += "NA_ZAMÓWIENIE";
                                }
                                orderItems.Add(poz);
                                if (nag.StatusStanowRaks==null || nag.StatusStanowRaks.Length == 0)
                                    nag.StatusStanowRaks = poz.Status;
                                else if (poz.Status.Equals("BRAK"))
                                    nag.StatusStanowRaks = poz.Status;
                                else if (nag.StatusStanowRaks.Equals("OK") && poz.Status.Equals("DO_PRZESUNIĘCIA"))
                                    nag.StatusStanowRaks = poz.Status;
                                else if ((nag.StatusStanowRaks.Equals("OK") || nag.StatusStanowRaks.Equals("DO_PRZESUNIĘCIA")) && poz.Status.Equals("NA_ZAMÓWIENIE"))
                                    nag.StatusStanowRaks = poz.Status;
                                else if ((nag.StatusStanowRaks.Equals("OK") || nag.StatusStanowRaks.Equals("DO_PRZESUNIĘCIA") || nag.StatusStanowRaks.Equals("NA_ZAMÓWIENIE")) && poz.Status.StartsWith("ARCHI"))
                                    nag.StatusStanowRaks = poz.Status;
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
                    MessageBox.Show("Błąd: " + ex.Message.ToString());
                    Pulpit.putLog(polaczenie, usrNam, "ERROR", ex.Message.ToString(), "", "", "", 0, "", 0, "", magID, "", 0, 0);
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

                if (row.Cells["StatusStanowRaks"].Value.Equals("OK"))
                    row.DefaultCellStyle.BackColor = Color.YellowGreen;
                else if (row.Cells["StatusStanowRaks"].Value.Equals("DO_PRZESUNIĘCIA"))
                    row.DefaultCellStyle.BackColor = Color.YellowGreen;
                else if (row.Cells["StatusStanowRaks"].Value.Equals("NA_ZAMÓWIENIE"))
                    row.DefaultCellStyle.BackColor = Color.Orange;
                else if (row.Cells["StatusStanowRaks"].Value.ToString().StartsWith("ARCHI"))
                    row.DefaultCellStyle.BackColor = Color.DarkOrange;
                else if (row.Cells["StatusStanowRaks"].Value.Equals("BRAK"))
                    row.DefaultCellStyle.BackColor = Color.Red;
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

        private List<OrderItem> itemsOfOrder;

        public Order()
        {

        }
        public int OrderSerialNumber { get => orderSerialNumber; set => orderSerialNumber = value; }

        public string OrderId { get => orderId; set => orderId = value; }
        public string OrderStatus { get => orderStatus; set => orderStatus = value; }
        public string OrderAddDate { get => orderAddDate; set => orderAddDate = value; }
        public string OrderPaymentType { get => orderPaymentType; set => orderPaymentType = value; }
        public string OrderConfirmation { get => orderConfirmation; set => orderConfirmation = value; }
        public string CourierName { get => courierName; set => courierName = value; }
        public string DeliveryDate { get => deliveryDate; set => deliveryDate = value; }
        public string ClientPhone1 { get => clientPhone1; set => clientPhone1 = value; }
        public string ClientPhone2 { get => clientPhone2; set => clientPhone2 = value; }
        public string ClientCountryName { get => clientCountryName; set => clientCountryName = value; }
        public string ClientCity { get => clientCity; set => clientCity = value; }
        public string ClientFirm { get => clientFirm; set => clientFirm = value; }
        public string ClientFirstName { get => clientFirstName; set => clientFirstName = value; }
        public string ClientLastName { get => clientLastName; set => clientLastName = value; }
        public string ClientNip { get => clientNip; set => clientNip = value; }
        public string ClientStreet { get => clientStreet; set => clientStreet = value; }
        public string ClientZipCode { get => clientZipCode; set => clientZipCode = value; }
        public string ClientEmail { get => clientEmail; set => clientEmail = value; }
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
