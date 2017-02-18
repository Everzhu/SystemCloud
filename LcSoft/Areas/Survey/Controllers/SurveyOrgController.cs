using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace XkSystem.Areas.Survey.Controllers
{
    public class SurveyOrgController : Controller
    {
        public ActionResult List()
        {
            return View();
        }
    }
}