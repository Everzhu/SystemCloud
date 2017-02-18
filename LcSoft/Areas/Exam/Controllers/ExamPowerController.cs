using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamPowerController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPower.List();
                vm.ExamName = db.Table<Exam.Entity.tbExam>().FirstOrDefault(d => d.Id == vm.ExamId).ExamName;
                vm.SubjectList = Course.Controllers.SubjectController.SelectList();
                vm.CourseTypeList = Course.Controllers.CourseTypeController.SelectList();
                var tb = from p in db.Table<Exam.Entity.tbExamCourse>()
                         where p.tbExam.Id == vm.ExamId
                         select p;

                var examPowerList = (from p in db.Table<Exam.Entity.tbExamPower>()
                                        where p.tbExamCourse.IsDeleted==false
                                        && p.tbExamCourse.tbExam.Id==vm.ExamId
                                        select new
                                        {
                                            p.tbTeacher.TeacherName,
                                            ExamCourseId = p.tbExamCourse.Id
                                        }).ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbCourse.CourseName.Contains(vm.SearchText));
                }

                if (vm.SubjectId != 0)
                {
                    tb = tb.Where(d => d.tbCourse.tbSubject.Id == vm.SubjectId);
                }

                if (vm.CourseTypeId != 0)
                {
                    tb = tb.Where(d => d.tbCourse.tbCourseType.Id == vm.CourseTypeId);
                }

                var tf = (from p in tb
                                       orderby p.tbCourse.tbSubject.No,p.tbCourse.CourseName
                                       select new
                                       {
                                           Id = p.Id,
                                           SubjectName=p.tbCourse.tbSubject.SubjectName,
                                           CourseName = p.tbCourse.CourseName,
                                           FromDate = p.FromDate,
                                           ToDate = p.ToDate
                                       }).ToList();

                var tg = (from p in tf
                          select new
                          {
                              Id = p.Id,
                              SubjectName = p.SubjectName,
                              CourseName = p.CourseName,
                              FromDate = p.FromDate.ConvertDateTimeToString(),
                              ToDate = p.ToDate.ConvertDateTimeToString(),
                          }).ToList();

                vm.ExamPowerList = (from p in tg
                                    select new Dto.ExamPower.List
                                    {
                                        Id = p.Id,
                                        SubjectName = p.SubjectName,
                                        CourseName = p.CourseName,
                                        InputDate =p.FromDate+ "-" +p.ToDate,
                                        TeacherName = examPowerList.Where(d => d.ExamCourseId == p.Id).FirstOrDefault() == null ? "设置" : string.Join(",", examPowerList.Where(d => d.ExamCourseId == p.Id).Select(d => d.TeacherName).ToArray())
                                    }).ToList();

                return View(vm);
            }
        }

        public ActionResult PowerTeacherList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPower.List();
                
                var tb = from p in db.Table<Exam.Entity.tbExamPower>()
                         where p.tbExamCourse.Id == vm.ExamCourseId && p.tbTeacher.IsDeleted==false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => (d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText)));
                }
                vm.ExamPowerList = (from p in tb
                                    orderby p.tbTeacher.TeacherCode
                                    select new Dto.ExamPower.List
                                    {
                                        Id = p.Id,
                                        TeacherCode = p.tbTeacher.TeacherCode,
                                        TeacherName = p.tbTeacher.TeacherName,
                                        IsOrgTeacher=p.IsOrgTeacher
                                    }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamPower.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText,examId=vm.ExamId, SubjectId = vm.SubjectId, CourseTypeId = vm.CourseTypeId}));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PowerTeacherList(Models.ExamPower.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("PowerTeacherList", new { searchText = vm.SearchText, examId = vm.ExamId,examCourseId=vm.ExamCourseId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetTeacher(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Exam.Entity.tbExamPower>().Find(id);
                if (tb != null)
                {
                    tb.IsOrgTeacher = !tb.IsOrgTeacher;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamPower>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除录入老师");
                }

                return Code.MvcHelper.Post();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(List<int> ids, int examCourseId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var classTeacherList = (from p in db.Table<Exam.Entity.tbExamPower>()
                                        where p.tbExamCourse.Id == examCourseId
                                        select p.tbTeacher.Id).ToList();

                var check = (from p in db.Table<Exam.Entity.tbExamPower>()
                             where p.tbExamCourse.Id == examCourseId
                                && ids.Contains(p.tbTeacher.Id)
                             select new
                             {
                                 p.tbTeacher.TeacherCode,
                                 p.tbTeacher.TeacherName,
                                 p.tbExamCourse.tbCourse.CourseName
                             }).ToList();

                if (check.Count > 0)
                {
                    error.AddError(string.Join("\r\n", check.Select(d => d.TeacherCode + "(" + d.TeacherName + ")已在" + d.CourseName).ToList()));
                }
                else
                {
                    var TeacherList = (from p in db.Table<Teacher.Entity.tbTeacher>()
                                       where ids.Contains(p.Id) && classTeacherList.Contains(p.Id) == false
                                       select p).ToList();

                    foreach (var Teacher in TeacherList)
                    {
                        var tb = new Exam.Entity.tbExamPower();
                        tb.tbExamCourse = db.Set<Exam.Entity.tbExamCourse>().Find(examCourseId);
                        tb.tbTeacher = Teacher;
                        tb.IsOrgTeacher = false;
                        db.Set<Exam.Entity.tbExamPower>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加录入人员");
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InserOrgTeacher(int examId,int examCourseId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.List();
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var year = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                where p.Id == examCourseId && p.tbCourse.IsDeleted == false
                                && p.tbExam.Id == examId
                                select new
                                {
                                    YearId = p.tbExam.tbYear.Id,
                                    CouseId = p.tbCourse.Id
                                }).FirstOrDefault();

                    var teacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                       where p.tbOrg.IsDeleted == false && p.tbTeacher.IsDeleted == false
                                       && p.tbOrg.tbCourse.Id == year.CouseId
                                       && p.tbOrg.tbYear.Id == year.YearId
                                       select p.tbTeacher).Distinct().ToList();

                    foreach (var Teacher in teacherList)
                    {
                        var check = (from p in db.Table<Exam.Entity.tbExamPower>()
                                     where p.tbExamCourse.Id == examCourseId
                                        && p.tbTeacher.Id==Teacher.Id
                                     select p).FirstOrDefault();
                        if (check == null)
                        {
                            var tb = new Exam.Entity.tbExamPower();
                            tb.tbExamCourse = db.Set<Exam.Entity.tbExamCourse>().Find(examCourseId);
                            tb.tbTeacher = Teacher;
                            tb.IsOrgTeacher = true;
                            db.Set<Exam.Entity.tbExamPower>().Add(tb);
                        }
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("批量添加录入人员");
                    }
                    db.SaveChanges();
                }
                return Code.MvcHelper.Post(null, string.Empty, "添加成功!");
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPower.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                              where p.Id == id
                              select new Dto.ExamPower.Edit
                              {
                                  Id = p.Id,
                                  FromDate=p.FromDate,
                                  ToDate=p.ToDate
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamPowerEdit = tb;
                        vm.FromDate = tb.FromDate.ConvertDateTimeToString(Code.Common.StringToDateTime);
                        vm.ToDate = tb.ToDate.ConvertDateTimeToString(Code.Common.StringToDateTime);
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamPower.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamPowerEdit.Id ==0)
                    {
                        //var tb = new Exam.Entity.tbExamCourse();
                        //tb.FromDate = vm.ExamPowerEdit.FromDate;
                        //tb.ToDate = vm.ExamPowerEdit.ToDate;

                        //db.Set<Exam.Entity.tbExamCourse>().Add(tb);
                        //if (db.SaveChanges() > 0)
                        //{
                        //    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加课程录入时间");
                        //}
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                  where p.Id == vm.ExamPowerEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.FromDate =vm.FromDate.ConvertToDateTime();
                            tb.ToDate = vm.ToDate.ConvertToDateTime();
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改课程录入时间");
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
        public ActionResult Save(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPower.List();
                var arrystr = new string[] { };
                var ids= Request["txtId"] != null ? Request["txtId"].Split(',') : arrystr;
                var fromDate = Request["FromDate"] != null ? Request["FromDate"] : string.Empty;
                var toDate = Request["ToDate"] != null ? Request["ToDate"] : string.Empty;
                if(string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
                    return Code.MvcHelper.Post(null, Url.Action("List",new { examId= examId }), "请输入批量设置开始时间!");
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    foreach (var id in ids)
                    {
                        var examCourseId = id.ConvertToInt();
                        var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                  where p.Id ==examCourseId
                                  select p).FirstOrDefault();
                        if(tb!=null)
                        {
                            tb.FromDate = fromDate.ConvertToDateTime();
                            tb.ToDate = toDate.ConvertToDateTime();
                        }

                    }
                    db.SaveChanges();

                }
                return Code.MvcHelper.Post(null, string.Empty, "保存成功!");
            }
        }
    }
}