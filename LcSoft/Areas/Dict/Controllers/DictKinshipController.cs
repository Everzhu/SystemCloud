using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Dict.Controllers
{
    public class DictKinshipController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictKinship.List();
                var tb = from p in db.Table<Dict.Entity.tbDictKinship>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.KinshipName.Contains(vm.SearchText));
                }

                vm.KinshipList = (from p in tb
                                  orderby p.No
                                  select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.DictKinship.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Dict.Entity.tbDictKinship>()
                          where ids.Contains(p.Id)
                          select p).ToList();
                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了亲属关系");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.DictKinship.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Dict.Entity.tbDictKinship>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.KinshipEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DictKinship.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Dict.Entity.tbDictKinship>().Where(d => d.KinshipName == vm.KinshipEdit.KinshipName && d.Id != vm.KinshipEdit.Id).Any())
                    {
                        error.AddError("该亲属关系已存在!");
                    }
                    else
                    {
                        if (vm.KinshipEdit.Id == 0)
                        {
                            var tb = new Dict.Entity.tbDictKinship();
                            tb.No = vm.KinshipEdit.No == null ? db.Table<Dict.Entity.tbDictKinship>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.KinshipEdit.No;
                            tb.KinshipName = vm.KinshipEdit.KinshipName;
                            db.Set<Dict.Entity.tbDictKinship>().Add(tb);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("新增了亲属关系");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Dict.Entity.tbDictKinship>()
                                      where p.Id == vm.KinshipEdit.Id
                                      select p).FirstOrDefault();
                            if (tb != null)
                            {
                                tb.No = vm.KinshipEdit.No == null ? db.Table<Dict.Entity.tbDictKinship>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.KinshipEdit.No;
                                tb.KinshipName = vm.KinshipEdit.KinshipName;
                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了亲属关系");
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
                return (from p in db.Table<Dict.Entity.tbDictKinship>()
                        orderby p.No
                        select new System.Web.Mvc.SelectListItem
                        {
                            Text = p.KinshipName,
                            Value = p.Id.ToString()
                        }).ToList();
            }
        }
    }
}