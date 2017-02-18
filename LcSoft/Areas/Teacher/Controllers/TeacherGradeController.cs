using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Teacher.Controllers
{
    public class TeacherGradeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeacherGrade.List();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();

                var tb = from p in db.Table<Teacher.Entity.tbTeacherGrade>()
                         where p.tbTeacher.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText));
                }

                vm.TeacherGradeList = (from p in tb
                                       orderby p.tbTeacher.TeacherName
                                       select new Dto.TeacherGrade.List
                                       {
                                           Id = p.Id,
                                           GradeId = p.tbGrade.Id,
                                           TeacherId = p.tbTeacher.Id,
                                           TeacherCode = p.tbTeacher.TeacherCode,
                                           TeacherName = p.tbTeacher.TeacherName
                                       }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.TeacherGrade.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacherGrade>()
                          where ids.Contains(p.tbTeacher.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除年级组长");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int teacherId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeacherGrade.Edit();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                vm.TeacherList = Teacher.Controllers.TeacherController.SelectList();

                vm.TeacherGradeList = (from p in db.Table<Teacher.Entity.tbTeacherGrade>()
                                       where p.tbTeacher.Id == teacherId
                                       select p.tbGrade.Id).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.TeacherGrade.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var gradeIds = new List<int>();
                    if (Request["CboxGrade"] != null)
                    {
                        gradeIds = Request["CboxGrade"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(d => d.ConvertToInt()).ToList();
                    }

                    var TeacherGradeList = (from p in db.Table<Teacher.Entity.tbTeacherGrade>()
                                                .Include(d => d.tbGrade)
                                            where p.tbTeacher.Id == vm.TeacherId
                                            select p).ToList();
                    foreach (var a in TeacherGradeList.Where(d => gradeIds.Contains(d.tbGrade.Id) == false))
                    {
                        a.IsDeleted = true;
                    }

                    foreach (var a in gradeIds.Where(d => TeacherGradeList.Select(q => q.tbGrade.Id).Contains(d) == false))
                    {
                        var tb = new Teacher.Entity.tbTeacherGrade();
                        tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.TeacherId);
                        tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(a);
                        db.Set<Teacher.Entity.tbTeacherGrade>().Add(tb);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改年级组长");
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetGradeByTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacherGrade>()
                          where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                            && p.tbGrade.IsDeleted == false
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbGrade.GradeName,
                              Value = p.tbGrade.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static bool IsGradeTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacherGrade>()
                          where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                            && p.tbGrade.IsDeleted == false
                          select decimal.One).Any();
                return tb;
            }
        }
    }
}