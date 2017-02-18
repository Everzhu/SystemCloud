using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class ClassTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassType.List();
                var tb = from p in db.Table<Basis.Entity.tbClassType>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.ClassTypeName.Contains(vm.SearchText));
                }

                vm.ClassTypeList = (from p in tb
                                    orderby p.No, p.ClassTypeName
                                    select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ClassType.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbClassType>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了班级类型");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ClassType.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Basis.Entity.tbClassType>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ClassTypeEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ClassType.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Basis.Entity.tbClassType>().Where(d=>d.ClassTypeName == vm.ClassTypeEdit.ClassTypeName && d.Id != vm.ClassTypeEdit.Id).Any())
                    {
                        error.AddError("该班级类型已存在!");
                    }
                    else
                    {
                        if (vm.ClassTypeEdit.Id == 0)
                        {
                            var tb = new Basis.Entity.tbClassType();
                            tb.No = vm.ClassTypeEdit.No == null ? db.Table<Basis.Entity.tbClassType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ClassTypeEdit.No;
                            tb.ClassTypeName = vm.ClassTypeEdit.ClassTypeName;
                            db.Set<Basis.Entity.tbClassType>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了班级类型");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Basis.Entity.tbClassType>()
                                      where p.Id == vm.ClassTypeEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.ClassTypeEdit.No == null ? db.Table<Basis.Entity.tbClassType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.ClassTypeEdit.No;
                                tb.ClassTypeName = vm.ClassTypeEdit.ClassTypeName;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了班级类型");
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
                var list = (from p in db.Table<Basis.Entity.tbClassType>()
                            orderby p.No, p.ClassTypeName
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.ClassTypeName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }
    }
}