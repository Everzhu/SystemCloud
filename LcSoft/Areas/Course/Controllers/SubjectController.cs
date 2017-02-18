using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Course.Controllers
{
    public class SubjectController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Subject.List();
                var tb = from p in db.Table<Course.Entity.tbSubject>()
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.SubjectName.Contains(vm.SearchText));
                }


                vm.SubjectList = (from p in tb
                                  orderby p.No, p.SubjectName
                                  select p).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.Subject.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbSubject>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                var courseList = (from p in db.Table<Course.Entity.tbCourse>()
                                    .Include(d => d.tbSubject)
                                  where ids.Contains(p.tbSubject.Id)
                                  select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;

                    foreach (var course in courseList.Where(d => d.tbSubject.Id == a.Id))
                    {
                        course.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了科目");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.Subject.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Course.Entity.tbSubject>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.SubjectEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Subject.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (db.Table<Course.Entity.tbSubject>().Where(d=>d.SubjectName == vm.SubjectEdit.SubjectName && d.Id != vm.SubjectEdit.Id).Any())
                    {
                        error.AddError("该科目已存在!");
                    }
                    else
                    {
                        if (vm.SubjectEdit.Id == 0)
                        {
                            var tb = new Course.Entity.tbSubject();
                            tb.No = vm.SubjectEdit.No == null ? db.Table<Course.Entity.tbSubject>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SubjectEdit.No;
                            tb.SubjectName = vm.SubjectEdit.SubjectName;
                            tb.SubjectNameEn = vm.SubjectEdit.SubjectNameEn;
                            tb.RequirePoint = vm.SubjectEdit.RequirePoint;
                            tb.ElectivePoint = vm.SubjectEdit.ElectivePoint;
                            db.Set<Course.Entity.tbSubject>().Add(tb);

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了科目");
                            }
                        }
                        else
                        {
                            var tb = (from p in db.Table<Course.Entity.tbSubject>()
                                      where p.Id == vm.SubjectEdit.Id
                                      select p).FirstOrDefault();

                            if (tb != null)
                            {
                                tb.No = vm.SubjectEdit.No == null ? db.Table<Course.Entity.tbSubject>().Select(d => d.No).DefaultIfEmpty(0).Max() + 1 : (int)vm.SubjectEdit.No;
                                tb.SubjectName = vm.SubjectEdit.SubjectName;
                                tb.SubjectNameEn = vm.SubjectEdit.SubjectNameEn;
                                tb.RequirePoint = vm.SubjectEdit.RequirePoint;
                                tb.ElectivePoint = vm.SubjectEdit.ElectivePoint;

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了科目");
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
                var list = (from p in db.Table<Course.Entity.tbSubject>()
                            orderby p.No, p.SubjectName
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.SubjectName,
                                Value = p.Id.ToString()
                            }).ToList();
                return list;
            }
        }

        public static Dto.Subject.Info SelectInfo(int id)
        {
            var tb = new Dto.Subject.Info();
            using (var db = new XkSystem.Models.DbContext())
            {
                tb = (from p in db.Table<Course.Entity.tbSubject>()
                      where p.Id == id
                      select new Dto.Subject.Info
                      {
                          SubjectName = p.SubjectName,
                          Id = p.Id
                      }).FirstOrDefault();
            }
            return tb;
        }
    }
}