using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sms.Controllers
{
    public class SmsTempletController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [NonAction]
        public static string GetTempletByType(Code.EnumHelper.SmsTempletType type)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var templet= (from p in db.Table<Entity.tbSmsTemplet>() where p.SmsTempletType == type select p).FirstOrDefault();
                return templet?.Templet;
            }
        }
    }
}