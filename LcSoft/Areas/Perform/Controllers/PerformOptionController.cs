using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Perform.Controllers
{
    public class PerformOptionController : Controller
    {
        public ActionResult Delete(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbPerformOption>()
                          where p.Id == id
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除评价选项");
                }

                return Json("ok", JsonRequestBehavior.AllowGet);
            }
        }

        [NonAction]
        public static List<Dto.PerformOption.Edit> SelectOptionList(int performItemId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.Table<Entity.tbPerformOption>()
                         where p.tbPerformItem.Id == performItemId
                         && p.tbPerformItem.IsDeleted == false
                         select p;

                var tbList = (from p in tb
                              orderby p.No
                              select new Dto.PerformOption.Edit
                              {
                                  Id = p.Id,
                                  PerformItemId = p.tbPerformItem.Id,
                                  PerformItemName = p.tbPerformItem.PerformItemName,
                                  OptionName = p.OptionName,
                                  OptionValue = p.OptionValue,
                                  No = p.No
                              }).ToList();
                return tbList;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList(int? performItemId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Entity.tbPerformOption>()
                          where p.tbPerformItem.Id == performItemId
                          && p.tbPerformItem.IsDeleted == false
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.OptionName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

    }
}