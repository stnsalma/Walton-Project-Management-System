using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProjectManagement
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{opt}",
                                defaults: new { controller = "Home", action = "login", id = UrlParameter.Optional, opt = UrlParameter.Optional }


            );
            //routes.MapRoute(
            //    name: "ModelColorDailySales",
            //    url: "Common/ModelColorWiseDailySales/{id}/{date}",
            //    defaults: new { controller = "Common", action = "ModelColorWiseDailySales", id = UrlParameter.Optional, date = UrlParameter.Optional });
       }
    }
}
