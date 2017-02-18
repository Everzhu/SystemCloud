using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictDegreeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictDegree.List();
                var tb = from p in db.Table<Dict.Entity.tbDictDegree>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.DegreeName.Contains(vm.SearchText));
                }

                vm.DataList = (from p in tb
                               orderby p.No
                               select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DictDegree.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictDegree>()
                          where ids.Contains(p.Id)
                          select p).ToList();
                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了学位");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictDegree.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Dict.Entity.tbDictDegree>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.DataEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DictDegree.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Dict.Entity.tbDictDegree>().Where(d => d.DegreeName == vm.DataEdit.DegreeName && d.Id != vm.DataEdit.Id).Any())
                    {
                        error.AddError("该学位已存在!");
                    }
                    else
                    {
                        if (vm.DataEdit.Id == 0)
                        {
                            var tb = new Dict.Entity.tbDictDegree();
                            tb.No = vm.DataEdit.No == null ? db.Table<Dict.Entity.tbDictDegree>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No;
                            tb.DegreeName = vm.DataEdit.DegreeName;
                            db.Set<Dict.Entity.tbDictDegree>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了学位");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Dict.Entity.tbDictDegree>()
                                      where p.Id == vm.DataEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.DataEdit.No == null ? db.Table<Dict.Entity.tbDictDegree>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.DataEdit.No;
                                tb.DegreeName = vm.DataEdit.DegreeName;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了学位");
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
                return (from p in db.Table<Dict.Entity.tbDictDegree>()
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = p.DegreeName,
                            Value = p.Id.ToString()
                        }).ToList();
            }
        }

    }
}