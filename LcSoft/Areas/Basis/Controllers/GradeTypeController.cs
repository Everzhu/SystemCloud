using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Controllers
{
    public class GradeTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.GradeType.List();
                var tb = from p in db.Table<Basis.Entity.tbGradeType>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.GradeTypeName.Contains(vm.SearchText));
                }

                vm.GradeTypeList = (from p in tb
                                     orderby p.No
                                     select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.GradeType.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Basis.Entity.tbGradeType>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了年级类型");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            var vm = new Models.GradeType.Edit();

            if (id > 0)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    vm.GradeTypeEdit = (from p in db.Table<Basis.Entity.tbGradeType>()
                                         where p.Id == id
                                         select p).FirstOrDefault();
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.GradeType.Edit vm)
        {
            var error = new List<string>();
            if (error.Count == decimal.Zero)
            {
                using (var db = new XkSystem.Models.DbContext())
                {
                    if (vm.GradeTypeEdit.Id > 0)
                    {
                        var tb = db.Set<Basis.Entity.tbGradeType>().Find(vm.GradeTypeEdit.Id);
                        tb.No = vm.GradeTypeEdit.No > 0 ? vm.GradeTypeEdit.No : db.Table<Basis.Entity.tbGradeType>().Where(d => d.Id == vm.GradeTypeEdit.Id).OrderByDescending(d => d.No).FirstOrDefault().No + 1;
                        tb.GradeTypeName = vm.GradeTypeEdit.GradeTypeName;
                    }
                    else
                    {
                        var tb = new Basis.Entity.tbGradeType()
                        {
                            No = vm.GradeTypeEdit.No,
                            GradeTypeName = vm.GradeTypeEdit.GradeTypeName
                        };
                        db.Set<Basis.Entity.tbGradeType>().Add(tb);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加/修改了年级类型");
                    }
                }
            }

            return Code.MvcHelper.Post(error);
        }
    }
}