using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AspNetMvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            #region подключаем логгер
            ILoggerRepository repository = log4net.LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(repository, new FileInfo(Server.MapPath("log4net.config"))); // подключаем файл с конфигурацией логгера
            #endregion подключаем логгер

        }
    }
}
