using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamStudentController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamStudent.List();

                var tb = from p in db.Table<Exam.Entity.tbExamStudent>()
                         where  p.tbExamRoom.Id==vm.ExamRoomId
                         && p.tbStudent.IsDeleted==false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                vm.ExamStudentList = (from p in tb
                               orderby p.tbStudent.StudentCode
                               select new Dto.ExamStudent.List
                               {
                                   Id = p.Id,
                                   StudentCode=p.tbStudent.StudentCode,
                                   StudentName=p.tbStudent.StudentName,
                               }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamStudent.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, ExamRoomId = vm.ExamRoomId, examCourseId = vm.ExamCourseId, scheduleId = vm.ScheduleId}));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamStudent>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除考场学生");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamStudent.Edit();
                vm.StudentList =Student.Controllers.StudentController.SelectList();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamStudent>()
                              where p.Id == id && p.tbStudent.IsDeleted==false
                              select new  Dto.ExamStudent.Edit
                              {
                                  Id = p.Id,
                                  StudentId=p.tbStudent.Id,
                                  ExamRoomId=p.tbExamRoom.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamStudentEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamStudent.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamStudentEdit.Id == 0)
                    {
                        var tb = new Exam.Entity.tbExamStudent();
                        tb.tbExamRoom = db.Set<Exam.Entity.tbExamRoom>().Find(vm.ExamStudentEdit.ExamRoomId);
                        tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.ExamStudentEdit.StudentId);
                        db.Set<Exam.Entity.tbExamStudent>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考场学生");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamStudent>()
                                  where p.Id == vm.ExamStudentEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(vm.ExamStudentEdit.StudentId);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考场学生");
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
        public ActionResult Insert(List<int> ids, int examRoomId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();

                var classStudentList = (from p in db.Table<Exam.Entity.tbExamStudent>()
                                        where p.tbExamRoom.Id == examRoomId
                                        select p.tbStudent.Id).ToList();

                var check = (from p in db.Table<Exam.Entity.tbExamStudent>()
                             where p.tbExamRoom.Id==examRoomId
                                && ids.Contains(p.tbStudent.Id)
                             select new
                             {
                                 p.tbStudent.StudentCode,
                                 p.tbStudent.StudentName,
                                 p.tbExamRoom.ExamRoomName
                             }).ToList();

                if (check.Count > 0)
                {
                    error.AddError(string.Join("\r\n", check.Select(d => d.StudentCode + "(" + d.StudentName + ")已在" + d.ExamRoomName).ToList()));
                }
                else
                {
                    var StudentList = (from p in db.Table<Student.Entity.tbStudent>()
                                       where ids.Contains(p.Id) && classStudentList.Contains(p.Id) == false
                                       select p).ToList();

                    foreach (var Student in StudentList)
                    {
                        var tb = new Exam.Entity.tbExamStudent();
                        tb.tbExamRoom = db.Set<Exam.Entity.tbExamRoom>().Find(examRoomId);
                        tb.tbStudent = Student;
                        db.Set<Exam.Entity.tbExamStudent>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考场学生");
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }
    }
}