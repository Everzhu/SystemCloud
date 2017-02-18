using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Teacher.Controllers
{
    public class TeacherSubjectController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeacherSubject.List();
                vm.SubjectList = Course.Controllers.SubjectController.SelectList();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();

                var tb = from p in db.Table<Teacher.Entity.tbTeacherSubject>()
                         where p.tbTeacher.IsDeleted == false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText));
                }

                if (vm.SubjectId != 0)
                {
                    tb = tb.Where(d => d.tbSubject.Id == vm.SubjectId);
                }

                vm.TeacherSubjectList = (from p in tb
                                         orderby p.tbSubject.No, p.tbSubject.SubjectName, p.tbTeacher.TeacherName
                                         select new Dto.TeacherSubject.List
                                         {
                                             Id = p.Id,
                                             GradeId = p.tbGrade.Id,
                                             GradeName = p.tbGrade.GradeName,
                                             SubjectId = p.tbSubject.Id,
                                             SubjectName = p.tbSubject.SubjectName,
                                             TeacherId = p.tbTeacher.Id,
                                             TeacherCode = p.tbTeacher.TeacherCode,
                                             TeacherName = p.tbTeacher.TeacherName
                                         }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.TeacherSubject.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { subjectId = vm.SubjectId, searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<string> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                foreach (var id in ids)
                {
                    var userSubject = id.Split(',');
                    var teacherId = Convert.ToInt32(userSubject[0]);
                    var subjectId = userSubject[1];
                    var tb = (from p in db.Table<Teacher.Entity.tbTeacherSubject>()
                              where p.tbTeacher.Id == teacherId
                              select p).Count();

                    var tbSubject = (from p in db.Table<Teacher.Entity.tbTeacherSubject>()
                                     where p.tbTeacher.Id == teacherId && p.tbSubject.Id.ToString() == subjectId
                                     select p).ToList();
                    foreach (var a in tbSubject)
                    {
                        a.IsDeleted = true;
                    }
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除科组长");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int teacherId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.TeacherSubject.Edit();
                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                vm.SubjectList = Course.Controllers.SubjectController.SelectList();
                vm.TeacherList = Teacher.Controllers.TeacherController.SelectList();

                //获取选中科目年级
                var SelectedSubjectList = (from p in db.Table<Teacher.Entity.tbTeacherSubject>()
                                           where p.tbTeacher.Id == teacherId
                                           select new
                                           {
                                               value = p.tbGrade.Id + "|" + p.tbSubject.Id
                                           }).ToList();

                foreach (var v in SelectedSubjectList)
                {
                    vm.SelectedSubjectList.Add(v.value);
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.TeacherSubject.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.TeacherId == null || vm.TeacherId <= decimal.Zero)
                    {
                        error.AddError("请选择科研组长!");
                        return Code.MvcHelper.Post(error);
                    }

                    var subjectGrade = string.Empty;
                    foreach (var key in Request.Form.AllKeys)
                    {
                        if (key.StartsWith("CboxSubjectGrade"))
                        {
                            var radiolc = Request[key];
                            if (radiolc != null)
                            {
                                subjectGrade += radiolc + ",";
                            }
                        }                        
                        else
                        {
                            continue;
                        }
                    }
                    //删除之前的数据
                    var deleteTb = (from p in db.Table<Teacher.Entity.tbTeacherSubject>()
                                    where p.tbTeacher.Id == vm.TeacherId
                                    select p).ToList();
                    foreach (var a in deleteTb)
                    {
                        a.IsDeleted = true;
                    }

                    foreach (var gs in subjectGrade.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var tb = new Teacher.Entity.tbTeacherSubject();
                        tb.tbSubject = db.Set<Course.Entity.tbSubject>().Find(Convert.ToInt32(gs.Split('|')[1]));
                        tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.TeacherId);
                        tb.tbGrade = db.Set<Basis.Entity.tbGrade>().Find(Convert.ToInt32(gs.Split('|')[0]));
                        db.Set<Teacher.Entity.tbTeacherSubject>().Add(tb);
                    }

                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改科组长");
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetSubjectByTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacherSubject>()
                          where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                            && p.tbGrade.IsDeleted == false
                            && p.tbSubject.IsDeleted == false
                            group p by new { p.tbSubject.Id, p.tbSubject.No, p.tbSubject.SubjectName } into g
                          orderby g.Key.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = g.Key.SubjectName,
                              Value = g.Key.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetSubjectGradeByTeacher(int subjectId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacherSubject>()
                          where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                            && p.tbGrade.IsDeleted == false
                            && p.tbSubject.Id == subjectId
                          group p by new { p.tbGrade.Id, p.tbGrade.No, p.tbGrade.GradeName } into g
                          orderby g.Key.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = g.Key.GradeName,
                              Value = g.Key.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static bool IsSubjectTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Teacher.Entity.tbTeacherSubject>()
                          where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                            && p.tbGrade.IsDeleted == false
                            && p.tbSubject.IsDeleted == false
                          select decimal.One).Any();
                return tb;
            }
        }
    }
}