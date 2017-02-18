using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamTeacherController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamTeacher.List();

                var tb = from p in db.Table<Exam.Entity.tbExamTeacher>()
                         where  p.tbExamRoom.Id==vm.ExamRoomId
                         && p.tbTeacher.IsDeleted==false
                         select p;

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    tb = tb.Where(d => d.tbTeacher.TeacherCode.Contains(vm.SearchText) || d.tbTeacher.TeacherName.Contains(vm.SearchText));
                }

                vm.ExamTeacherList = (from p in tb
                               orderby p.tbTeacher.TeacherCode
                               select new Dto.ExamTeacher.List
                               {
                                   Id = p.Id,
                                   TeacherCode=p.tbTeacher.TeacherCode,
                                   TeacherName=p.tbTeacher.TeacherName,
                                   IsPrimary=p.IsPrimary
                               }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamTeacher.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText, ExamRoomId = vm.ExamRoomId, examCourseId = vm.ExamCourseId, scheduleId = vm.ScheduleId }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPrimary(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Exam.Entity.tbExamTeacher>().Find(id);
                if (tb != null)
                {
                    tb.IsPrimary = !tb.IsPrimary;
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
                var tb = (from p in db.Table<Exam.Entity.tbExamTeacher>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除监考教师");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamTeacher.Edit();
                vm.TeacherList = Teacher.Controllers.TeacherController.SelectList();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamTeacher>()
                              where p.Id == id && p.tbTeacher.IsDeleted==false
                              select new  Dto.ExamTeacher.Edit
                              {
                                  Id = p.Id,
                                  TeacherId=p.tbTeacher.Id,
                                  IsPrimary=p.IsPrimary,
                                  ExamRoomId=p.tbExamRoom.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamTeacherEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamTeacher.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamTeacherEdit.Id == 0)
                    {
                        var tb = new Exam.Entity.tbExamTeacher();
                        tb.tbExamRoom = db.Set<Exam.Entity.tbExamRoom>().Find(vm.ExamTeacherEdit.ExamRoomId);
                        tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.ExamTeacherEdit.TeacherId);
                        tb.IsPrimary = vm.ExamTeacherEdit.IsPrimary;
                        db.Set<Exam.Entity.tbExamTeacher>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加监考教师");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamTeacher>()
                                  where p.Id == vm.ExamTeacherEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.tbTeacher = db.Set<Teacher.Entity.tbTeacher>().Find(vm.ExamTeacherEdit.TeacherId);
                            tb.IsPrimary = vm.ExamTeacherEdit.IsPrimary;
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改监考教师");
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

                var classTeacherList = (from p in db.Table<Exam.Entity.tbExamTeacher>()
                                        where p.tbExamRoom.Id == examRoomId
                                        select p.tbTeacher.Id).ToList();

                var check = (from p in db.Table<Exam.Entity.tbExamTeacher>()
                             where p.tbExamRoom.Id==examRoomId
                                && p.tbTeacher.IsDeleted == false
                                && ids.Contains(p.tbTeacher.Id)
                             select new
                             {
                                 p.tbTeacher.TeacherCode,
                                 p.tbTeacher.TeacherName,
                                 p.tbExamRoom.ExamRoomName
                             }).ToList();

                if (check.Count > 0)
                {
                    error.AddError(string.Join("\r\n", check.Select(d => d.TeacherCode + "(" + d.TeacherName + ")已在" + d.ExamRoomName).ToList()));
                }
                else
                {
                    var TeacherList = (from p in db.Table<Teacher.Entity.tbTeacher>()
                                       where ids.Contains(p.Id) && classTeacherList.Contains(p.Id) == false
                                       select p).ToList();

                    foreach (var Teacher in TeacherList)
                    {
                        var tb = new Exam.Entity.tbExamTeacher();
                        tb.tbExamRoom = db.Set<Exam.Entity.tbExamRoom>().Find(examRoomId);
                        tb.tbTeacher = Teacher;
                        tb.IsPrimary = false;
                        db.Set<Exam.Entity.tbExamTeacher>().Add(tb);
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加监考教师");
                    }
                }
                return Code.MvcHelper.Post(error);
            }
        }
    }
}