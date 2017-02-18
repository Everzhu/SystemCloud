using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Admin.Controllers
{
    public class ConfigController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public static List<Entity.tbConfig> GetConfig()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.TableRoot<Admin.Entity.tbConfig>()
                          select p).ToList();
                return tb;
            }
        }
    }
}