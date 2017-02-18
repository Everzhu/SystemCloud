using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Controllers
{
    public class MoralPhotoController : Controller
    {
        public ActionResult List()
        {
            var vm = new Models.MoralPhoto.List();
            using (var db = new XkSystem.Models.DbContext())
            {
                vm.MoralPhotoList= (from p in db.Table<Entity.tbMoralPhoto>()
                          where p.tbMoralData.Id == vm.MoralDataId
                          select p).ToList();
            }
            return View(vm);
        }
    }
}