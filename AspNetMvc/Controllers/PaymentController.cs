using AspNetMvc.Classes;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetMvc.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Local(string id)
        {
            return View();

        }
        [HttpPost]
        public ActionResult Bank(string id)
        {
            return View();
        }







        [HttpPost]
        public ActionResult Payment(OrderRequestFromClient request)
        {
            OrderPaymentRequest order = null;
            if (request.photoProcessId != null && request.photoProcessId.Count > 0 && request.sessionId != null)
            {
                order = new OrderPaymentRequest()
                {
                    //sessionId = request.sessionId,
                    //photoProcessId = request.photoProcessId,

                    //orderId = Guid.NewGuid().ToString(),
                    //userId = User.Identity.GetUserId(),
                    //amount = 15,
                    //currency = 933,
                    //description = "texttext",
                    //failUrl = "payment/fail",
                    //returnUrl = "payment/success"
                };


                // запрос и обработка ответа

                // сохранение в БД

                //success
                return Redirect("https://172.22.147.38/payment/merchants/Testing/payment_ru.html?mdOrder=4f788e8f-2c04-4a82-a40d-c5bf7caadf53");
                //error
                return Redirect("/Payment/Error");
            }
            else
            {
                int id = 2;
                return RedirectToAction("Error", "Payment", new { id = id });
            }


        }
    }
}