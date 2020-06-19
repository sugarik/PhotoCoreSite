using Newtonsoft.Json;
using PhotoCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestLab
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_TestBlurFace_Click(object sender, EventArgs e)
        {
            List<RestrictedFaceArea> hiddenRects = new List<RestrictedFaceArea>();
            hiddenRects.Add(new RestrictedFaceArea() { x = 70, y = 50, w = 100, h = 150, a = 12 });
            //hiddenRects.Add(new BlurMaskData(250, 330, 350, 480, 12));

            BlurFace process = new BlurFace("TestBlurFace.jpg", 3, 100, hiddenRects, 90);
            //BlurFace process = new BlurFace("11HxUraY8RghlvTXtwTzr8Do2pL2K0160", hiddenRects, 90);
            byte[] result = process.ResultArray;
        }


        private void btn_SearchFaces_Click(object sender, EventArgs e)
        {
            byte[] photoArray = System.IO.File.ReadAllBytes("KR_2018_04_29_19_57_19_53314.JPG");
            SearchFaces searchFaces = new SearchFaces(photoArray, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PhotoCore.BillingBGPB billingBGPB = new BillingBGPB();
            billingBGPB.Register(Guid.NewGuid().ToString().Replace("-", ""), "testovaya pokupka", 19635, "Testing", "Testing123", "finish.html", "error.html", "mpi-test.bgpb.by.crt", "mpi.test.key", "Bgpb2019");

            string mdOrder = null;

            int beg = billingBGPB.Answer.IndexOf("mdOrder=");
            if (beg > -1)
            {
                beg = beg + 8;
                int end = billingBGPB.Answer.IndexOf("\"", beg);
                if (end > -1)
                {
                    mdOrder = billingBGPB.Answer.Substring(beg, end - beg);
                    billingBGPB.Status("Testing", "Testing123", mdOrder);
                }
            }
        }


        // POST /payment/rest/register.do?amount=1964&currency=933&language=en&orderNumber=a2023b42ac9044b88968ccfa7d6c4cd9&returnUrl=finish.html&userName=Testing&password=Testing123&failUrl=error.html&description=testovaya pokupka HTTP/1.1
        // GET /payment/rest/getOrderStatus.do?userName=Testing&password=Testing123&orderId=2dc52a7f-247f-47bd-a9c1-b93bc981a494&language=en HTTP/1.1
        // {"depositAmount":0,"currency":"933","authCode":2,"params":{},"ErrorCode":"0","ErrorMessage":"Success","OrderStatus":0,"OrderNumber":"a2023b42ac9044b88968ccfa7d6c4cd9","Amount":1964}


        #region ОПЕРАЦИИ С ТАБЛИЦЕЙ CONTENT
        private void btn_CreateContent_Click(object sender, EventArgs e)
        {
            MsSqlDbExplorer dbExplorer = new MsSqlDbExplorer();
            var result = dbExplorer.CreateTable(MsSqlDbExplorer.Table.Content);
        }

        private void btn_InsertContent_Click(object sender, EventArgs e)
        {
            List<MsSqlDbExplorer.ContentData> data = new List<MsSqlDbExplorer.ContentData>();
            for (int i = 0; i < 10; i++) //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            {
                MsSqlDbExplorer.ContentData cd = new MsSqlDbExplorer.ContentData()
                {
                    amazon_id = Guid.NewGuid(),
                    location_id = 12345,
                    partner_id = 32000,
                    order_id = Guid.NewGuid(),
                    shooting_datetime = DateTime.Now.AddMinutes(-10),
                    ready_datetime = DateTime.Now,
                    expiration_date = DateTime.Now.AddDays(10),
                    locked = false,
                    x = 0.1F,
                    y = 0.2F,
                    w = 0.3F,
                    h = 0.4F,
                    a = 12,
                    //width = 6000, heigth = 4000, size = 2000000,
                    face_cloud_id = Guid.NewGuid().ToString(),
                    thumbnail_cloud_id = Guid.NewGuid().ToString(),
                    photo_cloud_id = Guid.NewGuid().ToString(),
                    face_local_path = "face_local_path_face_local_path_face_local_path_face_local_path_face_local_path",
                    thumbnail_local_path = "thumbnail_local_path_thumbnail_local_path_thumbnail_local_path_thumbnail_local_path_thumbnail_local_path",
                    photo_local_path = "photo_local_path_photo_local_path_photo_local_path_photo_local_path_photo_local_path",
                };
                data.Add(cd);
            }

            MsSqlDbExplorer dbExplorer = new MsSqlDbExplorer();

            DateTime dt1 = DateTime.Now;
            var lost = dbExplorer.InsertIntoContent(data);
            DateTime dt2 = DateTime.Now;
        }

        private void btn_TotalRecordsContent_Click(object sender, EventArgs e)
        {
            int result = new MsSqlDbExplorer().SelectCountFrom(MsSqlDbExplorer.Table.Content);
        }

        private void btn_SelectContent_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=mssql.by1880.hb.by;Initial Catalog=photocore;Persist Security Info=True;User ID=photocoreuser;Password=!Q@WAZSX3e4rdcfv;Pooling=True";
            SqlConnection connection = new SqlConnection(connectionString);
            int counter = 0;
            try
            { // https://www.sqlservertutorial.net/sql-server-basics/sql-server-drop-table/
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT amazon_id FROM Content;";
                var reader = cmd.ExecuteReader();

                List<Guid> guids = new List<Guid>();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        Guid amazon_id = (Guid)reader.GetValue(0);
                        if (guids.Count < 3) guids.Add(amazon_id); ///////////////////////////////////////////////////////////////////////////////////////
                        counter++;
                    }
                }
                reader.Close();

                List<string> dateMasks = new List<string>();
                dateMasks.Add("20200322");
                dateMasks.Add("20200323");
                dateMasks.Add("20200324");
                List<Guid>[] facesGuids = new List<Guid>[guids.Count];
                for (int i = 0; i < guids.Count; i++) facesGuids[i] = guids;

                MsSqlDbExplorer dbExplorer = new MsSqlDbExplorer();

                DateTime dt1 = DateTime.Now;
                var result = dbExplorer.SelectFromContent(facesGuids, "12345", dateMasks);
                DateTime dt2 = DateTime.Now;
            }
            catch (Exception exc)
            {
                // Invalid object name 'Content'.
            }
        }

        private void btn_DeleteContent_Click(object sender, EventArgs e)
        {
            var result = new MsSqlDbExplorer().DropTable(MsSqlDbExplorer.Table.Content);
        }

        #endregion ОПЕРАЦИИ С ТАБЛИЦЕЙ CONTENT

        #region ОПЕРАЦИИ С ТАБЛИЦЕЙ ORDERS

        private void btn_CreateOrders_Click(object sender, EventArgs e)
        {
            MsSqlDbExplorer dbExplorer = new MsSqlDbExplorer();
            var result = dbExplorer.CreateTable(MsSqlDbExplorer.Table.Orders);
        }

        private void btn_InsertOrders_Click_1(object sender, EventArgs e)
        {
            List<string> contentIdList = new List<string>();
            contentIdList.Add(Guid.NewGuid().ToString());
            contentIdList.Add(Guid.NewGuid().ToString());
            contentIdList.Add(Guid.NewGuid().ToString());

            MsSqlDbExplorer dbExplorer = new MsSqlDbExplorer();
            var result = dbExplorer.Orders_AddNewRecord
                (
                    "lebyzhiy_1234567890",
                    DateTime.Now,
                    contentIdList,
                    111,
                    931,
                    "Тестовый заказ",
                    "user id in system",
                    "user session data with ip and browser"
                );
        }

        private void btn_UpdateOrdersAddress_Click(object sender, EventArgs e)
        {
            MsSqlDbExplorer dbExplorer = new MsSqlDbExplorer();
            var result = dbExplorer.Orders_UpdateAddressAndOrderId
                (
                    "lebyzhiy_1234567890",
                    DateTime.Now,
                    "vitaliy.sugak@gmail.com",
                    "vitali.suhak@gmail.com",
                    "+375297776177",
                    7,
                    Guid.NewGuid().ToString()
                );
        }

        private void btn_UpdateOrdersPaymentNotSucc_Click(object sender, EventArgs e)
        {
            MsSqlDbExplorer dbExplorer = new MsSqlDbExplorer();
            var result = dbExplorer.Orders_UpdatePaymentResult
                (
                    "lebyzhiy_1234567890",
                    DateTime.Now,
                    3,
                    "Просто норма",
                    1,
                    "123456 ** 7890",
                    "022022",
                    "VITALI SUHAK",
                    "123456",
                    "2",
                    "192.168.1.1"
                );
        }

        private void btn_UpdateOrdersDelivery_Click(object sender, EventArgs e)
        {
            MsSqlDbExplorer dbExplorer = new MsSqlDbExplorer();
            var result = dbExplorer.Orders_UpdateDeliveryResult
                (
                    "lebyzhiy_1234567890",
                    DateTime.Now,
                    true
                );
        }

        #endregion ОПЕРАЦИИ С ТАБЛИЦЕЙ ORDERS

        #region ОПЕРАЦИИ С БАЗОЙ
        private void btn_CreateAllTables_Click(object sender, EventArgs e)
        {
            MsSqlDbExplorer dbExplorer = new MsSqlDbExplorer();

            int result_Orders = dbExplorer.CreateTable(MsSqlDbExplorer.Table.Orders);

            int result_Content = dbExplorer.CreateTable(MsSqlDbExplorer.Table.Content);
            int result_SearchFaces = dbExplorer.CreateTable(MsSqlDbExplorer.Table.SearchFaces);
            int result_SearchPhotos = dbExplorer.CreateTable(MsSqlDbExplorer.Table.SearchPhotos);
            int result_OrderReservations = dbExplorer.CreateTable(MsSqlDbExplorer.Table.OrderReservations);
            int result_OrderPayments = dbExplorer.CreateTable(MsSqlDbExplorer.Table.OrderPayments);
            dbExplorer = null;
        }

        private void btn_DropAllTables_Click(object sender, EventArgs e)
        {
            MsSqlDbExplorer dbExplorer = new MsSqlDbExplorer();

            int result_Orders = dbExplorer.DropTable(MsSqlDbExplorer.Table.Orders);

            int result_Content = dbExplorer.DropTable(MsSqlDbExplorer.Table.Content);
            int result_SearchFaces = dbExplorer.DropTable(MsSqlDbExplorer.Table.SearchFaces);
            int result_SearchPhotos = dbExplorer.DropTable(MsSqlDbExplorer.Table.SearchPhotos);
            int result_OrderReservations = dbExplorer.DropTable(MsSqlDbExplorer.Table.OrderReservations);
            int result_OrderPayments = dbExplorer.DropTable(MsSqlDbExplorer.Table.OrderPayments);
            dbExplorer = null;
        }

        private void btn_TableRowCount_Click(object sender, EventArgs e)
        {
            MsSqlDbExplorer dbExplorer = new MsSqlDbExplorer();
            int result_Content = dbExplorer.SelectCountFrom(MsSqlDbExplorer.Table.Content);
            int result_SearchFaces = dbExplorer.SelectCountFrom(MsSqlDbExplorer.Table.SearchFaces);
            int result_SearchPhotos = dbExplorer.SelectCountFrom(MsSqlDbExplorer.Table.SearchPhotos);
            int result_OrderReservations = dbExplorer.SelectCountFrom(MsSqlDbExplorer.Table.OrderReservations);
            int result_OrderPayments = dbExplorer.SelectCountFrom(MsSqlDbExplorer.Table.OrderPayments);
            dbExplorer = null;
        }








        #endregion ОПЕРАЦИИ С БАЗОЙ

        private void btn_DeleteOrders_Click(object sender, EventArgs e)
        {
            var result = new MsSqlDbExplorer().DropTable(MsSqlDbExplorer.Table.Orders);
        }

        private void btn_UpdateOrdersPaymentSucc_Click(object sender, EventArgs e)
        {

        }

        private void btn_OrderReservation_Click(object sender, EventArgs e)
        {
            List<string> orders = new List<string>();
            orders.Add(Guid.NewGuid().ToString());
            orders.Add(Guid.NewGuid().ToString());
            orders.Add(Guid.NewGuid().ToString());
            orders.Add(Guid.NewGuid().ToString());
            orders.Add(Guid.NewGuid().ToString());
            OrderReservationRequestFrontEnd orderRequest = new OrderReservationRequestFrontEnd() { photoProcessId = orders, requestId = Guid.NewGuid().ToString(), sessionId = Guid.NewGuid().ToString() };

            OrderReservation orderReservation = new OrderReservation(orderRequest, "SESSION INFO", new ProjectConfigData());
        }

        private void btn_Register_Click(object sender, EventArgs e)
        {
            MsSqlDbExplorer msSqlDbExplorer = new MsSqlDbExplorer();


            // 2. Зарезервировать в банке заказ

            BillingBGPB billingBGPB = new BillingBGPB();
            billingBGPB.Register("Lebjazhiy_12345", "Тестовая покупка", 101, new ProjectConfigData());

            var registerResponse = JsonConvert.DeserializeObject<OrderPayment.RegisterResult>(billingBGPB.Answer);


            string mdOrder = null;

            int beg = billingBGPB.Answer.IndexOf("mdOrder=");
            if (beg > -1)
            {
                beg = beg + 8;
                int end = billingBGPB.Answer.IndexOf("\"", beg);
                if (end > -1) mdOrder = billingBGPB.Answer.Substring(beg, end - beg);
            }
            //response.orderId = request.orderNumber;
            //if (mdOrder != null)
            //{
            //    // обновляем базу данными куда доставлять и банковскими данными

            //    if (msSqlDbExplorer.Orders_UpdateAddressAndOrderId(request.orderNumber, DateTime.Now, request.mainEmail, request.backupEmail, request.phone, request.messangers, mdOrder))
            //    {
            //        response.errorCode = 0;
            //        response.formUrl = configData.billReturnUrl;




            //    }
            //}
            //else response.errorCode = 1;
        }
    }
}

