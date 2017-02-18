using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictHealthController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictHealth.List();
                var tb = from p in db.Table<Dict.Entity.tbDictHealth>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.HealthName.Contains(vm.SearchText));
                }

                vm.HealthList = (from p in tb
                                 orderby p.No
                                 select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DictHealth.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictHealth>()
                          where ids.Contains(p.Id)
                          select p).ToList();
                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了健康状况");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictHealth.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Dict.Entity.tbDictHealth>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.HealthEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DictHealth.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Dict.Entity.tbDictHealth>().Where(d => d.HealthName == vm.HealthEdit.HealthName && d.Id != vm.HealthEdit.Id).Any())
                    {
                        error.AddError("该健康状况已存在!");
                    }
                    else
                    {
                        if (vm.HealthEdit.Id == 0)
                        {
                            var tb = new Dict.Entity.tbDictHealth();
                            tb.No = vm.HealthEdit.No == null ? db.Table<Dict.Entity.tbDictHealth>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.HealthEdit.No;
                            tb.HealthName = vm.HealthEdit.HealthName;
                            db.Set<Dict.Entity.tbDictHealth>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了健康状况");
                            }

                        }
                        else
                        {
                            var tb = (from p in db.Table<Dict.Entity.tbDictHealth>()
                                      where p.Id == vm.HealthEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.HealthEdit.No == null ? db.Table<Dict.Entity.tbDictHealth>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.HealthEdit.No;
                                tb.HealthName = vm.HealthEdit.HealthName;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了健康状况");
                                }
                            }
                            else
                            {
                                error.AddError(Resources.LocalizedText.MsgNotFound);
                            }
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictHealth>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.HealthName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}