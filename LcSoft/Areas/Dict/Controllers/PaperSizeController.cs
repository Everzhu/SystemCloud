using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class PaperSizeController : Controller
    {
        public ActionResult List()
        {
            return View(Service.PaperSize.List());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.PaperSize.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                error.AddError(Service.PaperSize.Delete(ids));
            }

            return Code.MvcHelper.Post(error);
        }

        public ActionResult Edit(int id = 0)
        {
            return View(Service.PaperSize.Edit(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.PaperSize.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                if (vm.PaperSizeEdit.Id == 0)
                {
                    error.AddError(Service.PaperSize.Insert(vm));
                }
                else
                {
                    error.AddError(Service.PaperSize.Update(vm));
                }
            }

            return Code.MvcHelper.Post(error);
        }
    }
}