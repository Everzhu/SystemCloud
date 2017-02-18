using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class PeriodTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.PeriodType.List();
                var tb = from p in db.Table<Basis.Entity.tbPeriodType>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.PeriodTypeName.Contains(vm.SearchText));
                }

                vm.PeriodTypeList = (from p in tb
                                     orderby p.No
                                     select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.PeriodType.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbPeriodType>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了节次类型");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.PeriodType.Edit();

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.PeriodTypeEdit = (from p in db.Table<Basis.Entity.tbPeriodType>()
                                         where p.Id == id
                                         select p).FirstOrDefault();
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.PeriodType.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (vm.PeriodTypeEdit.Id > 0)
                    {
                        var tb = db.Set<Basis.Entity.tbPeriodType>().Find(vm.PeriodTypeEdit.Id);
                        tb.Color = vm.PeriodTypeEdit.Color;
                        tb.No = vm.PeriodTypeEdit.No > 0 ? vm.PeriodTypeEdit.No : db.Table<Basis.Entity.tbPeriodType>().Where(d => d.Id == vm.PeriodTypeEdit.Id).OrderByDescending(d => d.No).FirstOrDefault().No + 1;
                        tb.PeriodTypeName = vm.PeriodTypeEdit.PeriodTypeName;
                    }
                    else
                    {
                        var tb = new Basis.Entity.tbPeriodType()
                        {
                            Color = vm.PeriodTypeEdit.Color,
                            No = vm.PeriodTypeEdit.No,
                            PeriodTypeName = vm.PeriodTypeEdit.PeriodTypeName
                        };
                        db.Set<Basis.Entity.tbPeriodType>().Add(tb);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改了节次类型");
                    }
                }
            }

            return Code.MvcHelper.Post(error);
        }

        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return (from p in db.Table<Entity.tbPeriodType>()
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem()
                        {
                            Value = p.Id.ToString(),
                            Text = p.PeriodTypeName
                        }).ToList();
            }
        }
    }
}