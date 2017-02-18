using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Course.Controllers
{
    public class CourseDomainController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.CourseDomain.List();
                var tb = db.Table<Course.Entity.tbCourseDomain>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.CourseDomainName.Contains(vm.SearchText));
                }

                vm.CourseDomainList = (from p in tb
                                       orderby p.No
                                       select p).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.CourseDomain.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbCourseDomain>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.Table<Entity.tbCourse>().Where(d => ids.Contains(d.tbCourseDomain.Id)).Count() > 0)
                {
                    return Code.MvcHelper.Post(new List<string>() { "系统中有关联的数据，不能直接删除此数据！" });
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了领域");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.CourseDomain.Edit();

                if (id != 0)
                {
                    var tb = db.Table<Course.Entity.tbCourseDomain>().Where(d => d.Id == id).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.CourseDomainEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.CourseDomain.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Course.Entity.tbCourseDomain>().Where(d => d.CourseDomainName == vm.CourseDomainEdit.CourseDomainName && d.Id != vm.CourseDomainEdit.Id).Any())
                    {
                        error.AddError("该领域分组已存在!");
                    }
                    else
                    {
                        if (vm.CourseDomainEdit.Id == 0)
                        {
                            var tb = new Course.Entity.tbCourseDomain();
                            tb.No = vm.CourseDomainEdit.No == null ? db.Table<Course.Entity.tbCourseDomain>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.CourseDomainEdit.No;
                            tb.CourseDomainName = vm.CourseDomainEdit.CourseDomainName;
                            db.Set<Course.Entity.tbCourseDomain>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了领域分组");
                            }
                        }
                        else
                        {
                            var tb = db.Set<Course.Entity.tbCourseDomain>().Find(vm.CourseDomainEdit.Id);
                            tb.No = vm.CourseDomainEdit.No == null ? db.Table<Course.Entity.tbCourseDomain>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.CourseDomainEdit.No;
                            tb.CourseDomainName = vm.CourseDomainEdit.CourseDomainName;

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了领域分组");
                            }
                        }
                    }
                }
                return Code.MvcHelper.Post();
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbCourseDomain>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.CourseDomainName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}