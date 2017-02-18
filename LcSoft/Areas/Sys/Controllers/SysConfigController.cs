using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Sys.Controllers
{
    public class SysConfigController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysConfig.List();
                var tb = from p in db.Table<Sys.Entity.tbSysConfig>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.Title.Contains(vm.SearchText));
                }

                vm.SysConfigList = (from p in tb
                                   orderby p.No
                                   select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.SysConfig.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Sys.Entity.tbSysConfig>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除用户类型");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.SysConfig.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Sys.Entity.tbSysConfig>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.SysConfigEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.SysConfig.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.SysConfigEdit.Id == 0)
                    {
                        var tb = new Sys.Entity.tbSysConfig();
                        tb.Title = vm.SysConfigEdit.Title;
                        tb.Value = vm.SysConfigEdit.Value;
                        db.Set<Sys.Entity.tbSysConfig>().Add(tb);

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加系统参数。");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Sys.Entity.tbSysConfig>()
                                  where p.Id == vm.SysConfigEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.Title = vm.SysConfigEdit.Title;
                            tb.Value = vm.SysConfigEdit.Value;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改系统参数。");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
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
                var tb = (from p in db.Table<Sys.Entity.tbSysConfig>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.Title,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}