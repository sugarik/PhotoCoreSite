using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotoCore
{
    public class OrderPayment
    {
        //{"errorCode":"0","orderId":"cfe0546d-48ef-4078-92c2-c4688c1454fd","formUrl":"https://mpi-test.bgpb.by/payment/merchants/Testing/payment_en.html?mdOrder=cfe0546d-48ef-4078-92c2-c4688c1454fd"}
        //{"errorCode":"1","errorMessage":"Order number is duplicated, order with given order number is processed already"}

        public class RegisterResult
        {
            /// <summary>
            /// Только при успешной регистрации
            /// </summary>
            public string orderId { get; set; }
            /// <summary>
            /// Только при успешной регистрации
            /// </summary>
            public string formUrl { get; set; }
            /// <summary>
            /// Будет всегда
            /// </summary>
            public Int16 errorCode { get; set; }
            /// <summary>
            /// Только при ошибке
            /// </summary>
            public string errorMessage { get; set; }
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SearchFaces));

        OrderPaymentResponseBackEnd response = null;
        public OrderPaymentResponseBackEnd Response { get { return response; } }

        public OrderPayment(OrderPaymentRequestFrontEnd request, string sessionInfo, ProjectConfigData configData)
        {
            response = new OrderPaymentResponseBackEnd();
            if (String.IsNullOrEmpty(request.requestId)) response.requestId = Guid.NewGuid().ToString();
            else response.requestId = request.requestId;

            try
            {
                if (!String.IsNullOrEmpty(request.orderNumber) && !String.IsNullOrEmpty(request.mainEmail))
                {
                    string pattern = @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$";
                    Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                    bool mainMatched = r.Match(request.mainEmail).Success; // true в случае подтверждения совпадения

                    bool backupMatched = false;
                    if (!String.IsNullOrEmpty(request.backupEmail))
                        backupMatched = r.Match(request.backupEmail).Success; // true в случае подтверждения совпадения

                    if (mainMatched || backupMatched)
                    {
                        // 1. Вычитать данные по заказу и извлечь его параметры
                        int amount;
                        Int16 currency;
                        string description;

                        MsSqlDbExplorer msSqlDbExplorer = new MsSqlDbExplorer();

                        // извлекаем данные по заказу для формирования запроса в банк
                        if(msSqlDbExplorer.Orders_GetReservedOrderInfo(request.orderNumber, out amount, out currency, out description) && amount > 0 && currency > 0 && String.IsNullOrEmpty(description))
                        {
                            // 2. Зарезервировать в банке заказ

                            BillingBGPB billingBGPB = new BillingBGPB();
                            billingBGPB.Register(request.orderNumber, description, amount, new ProjectConfigData());

                            var registerResponse = JsonConvert.DeserializeObject<RegisterResult>(billingBGPB.Answer);

                            if (registerResponse.errorCode == 0) // заказ на оплату принят
                            {
                                if (msSqlDbExplorer.Orders_UpdateAddressAndOrderId(request.orderNumber, DateTime.Now, request.mainEmail, request.backupEmail, request.phone, request.messangers, registerResponse.orderId))
                                {
                                    response.errorCode = 0;
                                    response.formUrl = registerResponse.formUrl;
                                    response.orderId = registerResponse.orderId;
                                }
                                else
                                {
                                    response.errorCode = 7;
                                }
                            }
                            else
                            {
                                if (msSqlDbExplorer.Orders_UpdateAddressAndOrderId(request.orderNumber, DateTime.Now, request.mainEmail, request.backupEmail, request.phone, request.messangers, registerResponse.orderId))
                                {
                                    response.errorCode = (byte)registerResponse.errorCode; ///////////////////////////////////////////////////////
                                    response.formUrl = registerResponse.formUrl;
                                    response.orderId = registerResponse.orderId;
                                }
                                else
                                {
                                    response.errorCode = 7;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception exc)
            {

            }

        }
    }
}
