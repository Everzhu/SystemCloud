using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Wechat
{
    public class WechatAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Wechat";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Wechat_default",
                "Wechat/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
                );
        }
    }
}