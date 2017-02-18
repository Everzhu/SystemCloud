using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace XkSystem
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //短域名
            routes.MapRoute(
                "ShortLogin",
                "Login",
                new { controller = "SysIndex", action = "Login" }
            ).DataTokens.Add("Area", "Sys");

            /*
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "SysIndex", action = "Login", id = UrlParameter.Optional }
            ).DataTokens.Add("Area", "Sys");
            */

            //if (Code.Common.IsMobile)
            //{
            //    routes.MapRoute(
            //              name: "Default",
            //              url: "{controller}/{action}/{id}",
            //              defaults: new { controller = Code.Common.IndexController, action = "Login", id = UrlParameter.Optional }
            //          ).DataTokens.Add("Area", Code.Common.IndexArea);
            //}
            //else
            //{
            //    routes.MapRoute(
            //              name: "Default",
            //              url: "{controller}/{action}/{id}",
            //              defaults: new { controller = Code.Common.IndexController, action = "Index", id = UrlParameter.Optional }
            //          ).DataTokens.Add("Area", Code.Common.IndexArea);
            //}
            routes.MapRoute(
                  name: "Default",
                  url: "{controller}/{action}/{id}",
                  defaults: new { controller = Code.Common.IndexController, action = Code.Common.IndexAction, id = UrlParameter.Optional }
              ).DataTokens.Add("Area", Code.Common.IndexArea);
        }
    }
}
