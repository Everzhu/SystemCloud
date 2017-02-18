using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Data;

namespace XkSystem.Areas.Course.Controllers
{
    public class OrgTeacherController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgTeacher.List();

                var org = (from p in db.Table<Course.Entity.tbOrg>()
                           where p.Id == vm.OrgId
                           select p).FirstOrDefault();
                if (org != null)
                {
                    var tb = from p in db.Table<Course.Entity.tbOrgTeacher>()
                             where p.tbOrg.Id == vm.OrgId
                             select p;

                    if (string.IsNullOrEmpty(vm.SearchText) == false)
                    {
                        tb = tb.Where(d => d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText));
                    }

                    vm.OrgTeacherList = (from p in tb
                                         orderby p.tbTeacher.TeacherName
                                         select new Dto.OrgTeacher.List
                                         {
                                             Id = p.Id,
                                             OrgName = p.tbOrg.OrgName,
                                             TeacherName = p.tbTeacher.TeacherName,
                                         }).ToList();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.OrgTeacher.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除了任课老师");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgTeacher.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                              where p.Id == id
                              select new Dto.OrgTeacher.Edit
                              {
                                  Id = p.Id,
                                  OrgId = p.tbOrg.Id,
                                  TeacherId = p.tbTeacher.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.OrgTeacherEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.OrgTeacher.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.OrgTeacherEdit.Id == 0)
                    {
                        var tb = new Course.Entity.tbOrgTeacher();
                        tb.tbOrg = db.Set<Course.Entity.tbOrg>().Find(vm.OrgId);
                        db.Set<Course.Entity.tbOrgTeacher>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了任课老师");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                  where p.Id == vm.OrgTeacherEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改了任课老师");
                            }
                        }
                        else
                        {
                            error.AddError(Resources.LocalizedText.MsgNotFound);
                        }
                    }
                }

                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(List<int> ids, int orgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.OrgTeacher.Edit();

                var classTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                        where p.tbOrg.Id == orgId
                                        select p.tbTeacher.Id).ToList();
                var TeacherList = (from p in db.Table<Teacher.Entity.tbTeacher>()
                                   where ids.Contains(p.Id) && classTeacherList.Contains(p.Id) == false
                                   select p).ToList();
                foreach (var Teacher in TeacherList)
                {
                    var tb = new Course.Entity.tbOrgTeacher();
                    tb.tbOrg = db.Set<Course.Entity.tbOrg>().Find(orgId);
                    tb.tbTeacher = Teacher;
                    db.Set<Course.Entity.tbOrgTeacher>().Add(tb);
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加了任课老师");
                }

                return Json(new { Status = decimal.One, Message = "操作成功！" });
            }
        }

        public static List<System.Web.Mvc.SelectListItem> SelectOrgList(int teacherId, int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                          where (p.tbTeacher.Id == teacherId || teacherId == 0)
                          && p.tbOrg.tbYear.Id == yearId
                          orderby p.tbOrg.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbOrg.OrgName,
                              Value = p.tbOrg.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static Areas.Teacher.Dto.Teacher.Info GetTeacherByOrgId(int orgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                          where p.tbOrg.Id == orgId
                          orderby p.No
                          select new Areas.Teacher.Dto.Teacher.Info
                          {
                              Id = p.tbTeacher.Id,
                              TeacherCode = p.tbTeacher.TeacherCode,
                              TeacherName = p.tbTeacher.TeacherName
                          }).FirstOrDefault();
                return tb;
            }
        }

        [NonAction]
        public static List<Areas.Course.Dto.Org.Edit> SelectOrgListByTeacher(int teacherId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from t in db.Table<Course.Entity.tbOrgTeacher>()
                          where t.tbTeacher.Id == teacherId
                          orderby t.tbOrg.No
                          select new Areas.Course.Dto.Org.Edit
                          {
                              Id = t.tbOrg.Id,
                              OrgName = t.tbOrg.OrgName
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public bool IsOrgTeacher(int yearId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                          where p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                            && p.tbOrg.IsDeleted == false
                            && p.tbOrg.tbYear.Id == yearId
                          select decimal.One).Any();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetOrgListByOrgTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                          where p.tbOrg.IsDeleted == false
                             && p.tbOrg.tbYear.IsDefault
                             && p.tbOrg.tbCourse.IsDeleted == false
                             && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
                             && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                          orderby p.tbOrg.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbOrg.OrgName,
                              Value = p.tbOrg.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetOrgListByOrgTeacher(int yearId, int teacherId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.Table<Course.Entity.tbOrgTeacher>()
                         where p.tbOrg.IsDeleted == false
                            && p.tbOrg.tbYear.Id == yearId
                            && p.tbOrg.tbCourse.IsDeleted == false
                            && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
                         select p;

                if (teacherId == 0)
                {
                    tb = tb.Where(d => d.tbTeacher.tbSysUser.Id == Code.Common.UserId);
                }
                else
                {
                    tb = tb.Where(d => d.tbTeacher.Id == teacherId);
                }

                var list = (from p in tb
                            orderby p.tbOrg.No
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = p.tbOrg.OrgName,
                                Value = p.tbOrg.Id.ToString()
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetOrgListByCourse(int yearId, int subjectId = 0, int courseId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.Table<Course.Entity.tbOrgTeacher>()
                         where p.tbOrg.IsDeleted == false
                            && p.tbOrg.tbYear.Id == yearId
                            && p.tbOrg.tbCourse.IsDeleted == false
                            && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
                         select p;

                if (subjectId != 0)
                {
                    tb = tb.Where(d => d.tbOrg.tbCourse.tbSubject.Id == subjectId);
                }

                if (courseId != 0)
                {
                    tb = tb.Where(d => d.tbOrg.tbCourse.Id == courseId);
                }

                var list = (from p in tb
                            group p by new { p.tbOrg.Id, p.tbOrg.No, p.tbOrg.OrgName } into g
                            orderby g.Key.No
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = g.Key.OrgName,
                                Value = g.Key.Id.ToString()
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetOrgGradeListByCourse(int yearId,int gradeId,int subjectId = 0,int courseId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.Table<Course.Entity.tbOrgTeacher>()
                         where p.tbOrg.IsDeleted == false
                            && p.tbOrg.tbYear.Id == yearId
                            && p.tbOrg.tbCourse.IsDeleted == false
                            && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
                            && p.tbOrg.tbGrade.Id==gradeId
                         select p;

                if (subjectId != 0)
                {
                    tb = tb.Where(d => d.tbOrg.tbCourse.tbSubject.Id == subjectId);
                }

                if (courseId != 0)
                {
                    tb = tb.Where(d => d.tbOrg.tbCourse.Id == courseId);
                }

                var list = (from p in tb
                            group p by new { p.tbOrg.Id, p.tbOrg.No, p.tbOrg.OrgName } into g
                            orderby g.Key.No
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = g.Key.OrgName,
                                Value = g.Key.Id.ToString()
                            }).ToList();
                return list;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetOrgListBySubjectCourse(int yearId,int subjectId = 0, int courseId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = from p in db.Table<Course.Entity.tbOrgTeacher>()
                         where p.tbOrg.IsDeleted == false
                            && p.tbOrg.tbYear.Id == yearId
                            && p.tbOrg.tbCourse.IsDeleted == false
                            && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
                         select p;

                if (subjectId != 0)
                {
                    tb = tb.Where(d => d.tbOrg.tbCourse.tbSubject.Id == subjectId);
                }

                if (courseId != 0)
                {
                    tb = tb.Where(d => d.tbOrg.tbCourse.Id == courseId);
                }

                var list = (from p in tb
                            group p by new { p.tbOrg.Id, p.tbOrg.No, p.tbOrg.OrgName } into g
                            orderby g.Key.No
                            select new System.Web.Mvc.SelectListItem
                            {
                                Text = g.Key.OrgName,
                                Value = g.Key.Id.ToString()
                            }).ToList();
                return list;
            }
        }
    }
}