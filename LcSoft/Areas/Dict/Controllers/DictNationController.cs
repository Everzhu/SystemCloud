using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictNationController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictNation.List();
                var tb = from p in db.Table<Dict.Entity.tbDictNation>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.NationName.Contains(vm.SearchText));
                }

                vm.NationList = (from p in tb
                                 orderby p.No
                                 select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DictNation.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictNation>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了民族");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictNation.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Dict.Entity.tbDictNation>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.NationEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DictNation.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Dict.Entity.tbDictNation>().Where(d => d.NationName == vm.NationEdit.NationName && d.Id != vm.NationEdit.Id).Any())
                    {
                        error.AddError("该民族已存在!");
                    }
                    else
                    {
                        if (vm.NationEdit.Id == 0)
                        {
                            var tb = new Dict.Entity.tbDictNation();
                            tb.No = vm.NationEdit.No == null ? db.Table<Dict.Entity.tbDictNation>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.NationEdit.No;
                            tb.NationName = vm.NationEdit.NationName;
                            db.Set<Dict.Entity.tbDictNation>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增了民族");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Dict.Entity.tbDictNation>()
                                      where p.Id == vm.NationEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.NationEdit.No == null ? db.Table<Dict.Entity.tbDictNation>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.NationEdit.No;
                                tb.NationName = vm.NationEdit.NationName;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了民族");
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
                var tb = (from p in db.Table<Dict.Entity.tbDictNation>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.NationName,
                              Value = p.Id.ToString()
                          }).ToList();

                return tb;
            }
        }
    }
}