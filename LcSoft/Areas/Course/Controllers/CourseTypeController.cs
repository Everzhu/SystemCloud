using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Course.Controllers
{
    public class CourseTypeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.CourseType.List();
                var tb = from p in db.Table<Course.Entity.tbCourseType>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.CourseTypeName.Contains(vm.SearchText));
                }

                vm.CourseTypeList = (from p in tb
                                     orderby p.No, p.CourseTypeName
                                     select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.CourseType.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbCourseType>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.Table<Entity.tbCourse>().Where(d => ids.Contains(d.tbCourseType.Id)).Count() > 0)
                {
                    return Code.MvcHelper.Post(new List<string>() { "系统中有关联的数据，不能直接删除此数据！" });
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了课程类型");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.CourseType.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Course.Entity.tbCourseType>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.CourseTypeEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.CourseType.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Course.Entity.tbCourseType>().Where(d=>d.CourseTypeName == vm.CourseTypeEdit.CourseTypeName && d.Id != vm.CourseTypeEdit.Id).Any())
                    {
                        error.AddError("该课程类型已存在!");
                    }
                    else
                    {
                        if (vm.CourseTypeEdit.Id == 0)
                        {
                            var tb = new Course.Entity.tbCourseType();
                            tb.No = vm.CourseTypeEdit.No == null ? db.Table<Course.Entity.tbCourseType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.CourseTypeEdit.No;
                            tb.CourseTypeName = vm.CourseTypeEdit.CourseTypeName;
                            db.Set<Course.Entity.tbCourseType>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了课程类型");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Course.Entity.tbCourseType>()
                                      where p.Id == vm.CourseTypeEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.CourseTypeEdit.No == null ? db.Table<Course.Entity.tbCourseType>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.CourseTypeEdit.No;
                                tb.CourseTypeName = vm.CourseTypeEdit.CourseTypeName;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了课程类型");
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
                var list = (from p in db.Table<Course.Entity.tbCourseType>()
                            orderby p.No, p.CourseTypeName
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.CourseTypeName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }
    }
}