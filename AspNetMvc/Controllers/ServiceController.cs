using AspNetMvc.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Hosting;
using SVK.BGPB;
using System.Net;
using System.Text;
using log4net;
using System.Text.RegularExpressions;

using PhotoCore;

// спрятать в файл параметры:
// 1) string faceDir
// 2) int[] additionalRotateAngles = null, int photoJpegQuality = 95, double faceClippingRatio = 1.2
// 3) string awsAccessKeyId, string awsSecretAccessKey, string awsEndpoint, string awsCollectionId, float awsFaceMatchThreshold = 90
// 4) string connectionString (или отдельно логин и пароль)
// 5) SimilarityLevel


// логирование:
// https://stackoverflow.com/questions/53658910/log4net-in-my-net-website-on-debugging-always-shows-iserrorenabled-value-as-fal
// https://weblogs.asp.net/jhallal/configure-log4net-logging-framework-for-mvc
namespace AspNetMvc.Controllers
{
    public class ServiceController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("ServiceController");

        string faceDir = HostingEnvironment.ApplicationPhysicalPath + "faces\\";

        bool isTestMode = true;

        public ActionResult Search()
        {
            ViewBag.requestId = Guid.NewGuid();
            return View();
        }

        [HttpPost]
        public JsonResult SearchFaces(SearchFacesRequestFrontEnd request)
        {
            string sessionInfo = GetSessionData("SearchFaces");
            SearchFaces searchFaces = new SearchFaces(request, sessionInfo, faceDir);
            return Json(searchFaces.Response);
        }

        [HttpPost]
        public JsonResult SearchPhotos(SearchPhotosRequestFrontEnd request)
        {
            /// 1. добавить бы сюда словесное описание результата. Например, могут быть ошибки из-за отсутствия лица или дат            
            string sessionInfo = GetSessionData("SearchPhotos");

            if (isTestMode) // тестовый режим
            {
                var response = new SearchPhotosResponseBackEnd();
                if (request != null)
                {
                    if (request.requestId == null) response.requestId = Guid.NewGuid().ToString();
                    else response.requestId = request.requestId;
                    response.photos = new List<Photo>();
                    Photo photo1 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1AnX2NyJ7lfnIkJaFUhhZDHzhMpGHLXTM" };
                    response.photos.Add(photo1);
                    Photo photo2 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1bJKklazVpQ-Cc8KMWMaKVQvRSeRz_pNR" };
                    response.photos.Add(photo2);
                    Photo photo3 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1ZABIBuDyFjL4oX_8bnDc8rql-MA0Jh3Z" };
                    response.photos.Add(photo3);
                    Photo photo4 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1PxD1th02jJadoIkXA-L122wrVJlZten6" };
                    response.photos.Add(photo4);
                    Photo photo5 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1mXCHtEUMCeRz4Gzg55u4ew9vLKcw0lFw" };
                    response.photos.Add(photo5);
                    Photo photo6 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1tfod3zdLnqofGJ-Rl9BeGnvhzfLcSUee" };
                    response.photos.Add(photo6);
                    Photo photo7 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1AnX2NyJ7lfnIkJaFUhhZDHzhMpGHLXTM" };
                    response.photos.Add(photo7);
                    Photo photo8 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1bJKklazVpQ-Cc8KMWMaKVQvRSeRz_pNR" };
                    response.photos.Add(photo8);
                    Photo photo9 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1ZABIBuDyFjL4oX_8bnDc8rql-MA0Jh3Z" };
                    response.photos.Add(photo9);
                    Photo photo10 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1PxD1th02jJadoIkXA-L122wrVJlZten6" };
                    response.photos.Add(photo10);
                    Photo photo11 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1mXCHtEUMCeRz4Gzg55u4ew9vLKcw0lFw" };
                    response.photos.Add(photo11);
                    Photo photo12 = new Photo() { photoProcessId = Guid.NewGuid().ToString(), dateTime = DateTime.Now.ToString(), thumbnailLink = "https://drive.google.com/uc?export=view&id=1tfod3zdLnqofGJ-Rl9BeGnvhzfLcSUee" };
                    response.photos.Add(photo12);
                    PriceCalculator calc = new PriceCalculator(response.photos.Count);
                    response.prices = calc.PriceValues;
                    response.discounts = calc.DiscountValues;

                    if (response.photos != null && response.photos.Count == 0) response.photos = null;
                    return Json(response);
                }
                else return Json(response);
            }
            else // рабочий режим
            {
                SearchPhotos searchPhotos = new SearchPhotos(request, sessionInfo, faceDir, new ProjectConfigData());
                return Json(searchPhotos.Response);
            }
        }

        [HttpPost]
        public JsonResult OrderReservation(OrderReservationRequestFrontEnd request)
        {
            string sessionInfo = GetSessionData("OrderReservation");
            OrderReservation orderReservation = new OrderReservation(request, sessionInfo, new ProjectConfigData());
            return Json(orderReservation.Response);
        }

        [HttpPost]
        public JsonResult OrderPayment(OrderPaymentRequestFrontEnd request)
        {
            string sessionInfo = GetSessionData("OrderPayment");
            OrderPayment orderPayment = new OrderPayment(request, sessionInfo, new ProjectConfigData());
            return Json(orderPayment.Response);
        }

        [HttpPost]
        public JsonResult BillingReport(OrderPaymentRequestFrontEnd request)
        {
            // {merchant-url}?mdOrder={mdOrder}&orderNumber={orderNumber}&operation={operation}&status={status}
            // mdOrder       ANS36     Номер заказа в платежной системе
            // orderNumber   AN..32    Номер(идентификатор) заказа в системе магазина, уникален для каждого магазина в пределах системы
            // operation     N..9      Тип операции:
            //                          approved  - операция холдирования суммы (только для двухстадийной системы оплаты);
            //                          deposited - операция завершения;
            //                          reversed  - операция отмены;
            //                          refunded  - операция возврата;
            // status        N1        Индикатор успешности операции, указанной в параметре operation(1 - операция прошла успешно, 0 - операция завершилась ошибкой)

            // Пример URL: https://myshop.ru/callback/?mdOrder=1234567890-098776-234-522&orderNumber=0987&operation=deposited&status=0

            //PhotoCore.OrderPayment orderPayment = new OrderPayment(request, sessionInfo, new ProjectConfigData());
            return Json(null);
        }




        /// <summary> Формирование данных по сессии </summary>
        /// <returns> Строки с данными по сессии </returns>
        string GetSessionData(string method)
        {
            string result = "";
            try
            {
                StringBuilder sb = new StringBuilder();
                if (method != null) sb.Append(method); else sb.Append("NULL");
                sb.Append("\n");
                sb.Append(String.Format("{0:yyyy}/{0:MM}/{0:dd} {0:HH}:{0:mm}:{0:ss}.{0:fff}\n", DateTime.Now));
                if (Request.ServerVariables["REMOTE_ADDR"] != null) sb.Append(Request.ServerVariables["REMOTE_ADDR"]); else sb.Append("NULL");
                sb.Append("\n");
                if (Request.ServerVariables["REMOTE_PORT"] != null) sb.Append(Request.ServerVariables["REMOTE_PORT"]); else sb.Append("NULL");
                sb.Append("\n");
                if (Request.ServerVariables["AUTH_USER"] != null) sb.Append(Request.ServerVariables["AUTH_USER"]); else sb.Append("NULL");
                sb.Append("\n");
                if (Request.ServerVariables["HTTP_CONTENT_LENGTH"] != null) sb.Append(Request.ServerVariables["HTTP_CONTENT_LENGTH"]); else sb.Append("NULL");
                sb.Append("\n");
                if (Request.ServerVariables["HTTP_USER_AGENT"] != null) sb.Append(Request.ServerVariables["HTTP_USER_AGENT"]); else sb.Append("NULL");
                result = sb.ToString();
            }
            catch (Exception exc) { }
            return result;
        }
    }
}