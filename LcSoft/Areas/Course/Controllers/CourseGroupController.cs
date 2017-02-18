using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Course.Controllers
{
    public class CourseGroupController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.CourseGroup.List();

                var tb = db.Table<Course.Entity.tbCourseGroup>();
                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.CourseGroupName.Contains(vm.SearchText));
                }

                vm.CourseGroupList = (from p in tb
                                           orderby p.No
                                           select p).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.CourseGroup.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbCourseGroup>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }
                if (db.Table<Entity.tbCourse>().Where(d => ids.Contains(d.tbCourseGroup.Id)).Count() > 0)
                {
                    return Code.MvcHelper.Post(new List<string>() { "系统中有关联的数据，不能直接删除此数据！" });
                }
                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了课程分组");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.CourseGroup.Edit();

                if (id != 0)
                {
                    var tb = db.Table<Course.Entity.tbCourseGroup>().Where(d => d.Id == id).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.CourseGroupEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.CourseGroup.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Course.Entity.tbCourseGroup>().Where(d=>d.CourseGroupName == vm.CourseGroupEdit.CourseGroupName && d.Id != vm.CourseGroupEdit.Id).Any())
                    {
                        error.AddError("该课程分组已存在!");
                    }
                    else
                    {
                        if (vm.CourseGroupEdit.Id == 0)
                        {
                            var tb = new Course.Entity.tbCourseGroup();
                            tb.No = vm.CourseGroupEdit.No == null ? db.Table<Course.Entity.tbCourseGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.CourseGroupEdit.No;
                            tb.CourseGroupName = vm.CourseGroupEdit.CourseGroupName;
                            db.Set<Course.Entity.tbCourseGroup>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了课程分组");
                            }
                        }
                        else
                        {
                            var tb = db.Set<Course.Entity.tbCourseGroup>().Find(vm.CourseGroupEdit.Id);
                            tb.No = vm.CourseGroupEdit.No == null ? db.Table<Course.Entity.tbCourseGroup>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.CourseGroupEdit.No;
                            tb.CourseGroupName = vm.CourseGroupEdit.CourseGroupName;

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了课程分组");
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
                var tb = (from p in db.Table<Course.Entity.tbCourseGroup>()
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.CourseGroupName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
    }
}