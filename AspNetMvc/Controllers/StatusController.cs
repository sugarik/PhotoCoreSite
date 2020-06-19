using AspNetMvc.Classes;
using AspNetMvc.Models;
using SVK.BGPB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetMvc.Controllers
{
    public class StatusController : Controller
    {
        // GET: Status
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //public JsonResult RegisterDo(TestModel model) // RegisterDo
        public JsonResult RegisterDo(OrderRequestFromClient request) // RegisterDo
        {
            RegisterDo registerDo = new RegisterDo(Guid.NewGuid().ToString().Replace("-", ""), "testovaya pokupka", 19.635f, "Testing", "Testing123", "finish.html", "error.html");
           // string k = model.Name;
            Answer ans = new Answer();
            return Json(ans);
        }

        [HttpGet]
        public JsonResult GetOrderStatusDo(TestModel model)
        {
            Answer ans = new Answer();
            return Json(ans, JsonRequestBehavior.AllowGet);
        }
    }
}