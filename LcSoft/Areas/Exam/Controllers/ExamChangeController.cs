using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamChangeController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamChange.List();
                var tbStudent = from p in db.Table<Student.Entity.tbStudent>()
                                select p;

                var student = (from p in tbStudent
                               where (p.StudentCode == vm.SearchText || p.StudentName == vm.SearchText)
                               select new
                               {
                                   p.Id,
                                   p.StudentCode,
                                   p.StudentName
                               }).FirstOrDefault();
                if (student != null)
                {
                    vm.StudentId = student.Id;
                    var tb = from p in db.Table<Exam.Entity.tbExamMark>()
                             where p.tbExamCourse.IsDeleted == false && p.tbExamCourse.tbCourse.IsDeleted == false
                              && p.tbStudent.Id == student.Id
                              && p.tbExamCourse.tbExam.IsDeleted == false
                             select p;

                    vm.ExamMarkList = (from p in tb
                                       select new Dto.ExamChange.List
                                       {
                                           Id = p.Id,
                                           CourseId = p.tbExamCourse.tbCourse.Id,
                                           StudentId = p.tbStudent.Id,
                                           StudentCode = p.tbStudent.StudentCode,
                                           StudentName = p.tbStudent.StudentName,
                                           ExamName = p.tbExamCourse.tbExam.ExamName,
                                           CourseName = p.tbExamCourse.tbCourse.CourseName,
                                           AppraiseMark = p.AppraiseMark,
                                           SegmentMark = p.SegmentMark,
                                           TotalMark = p.TotalMark,
                                           ExamLevelName = p.tbExamLevel.ExamLevelName,
                                       }).ToList();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamChange.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除考试成绩");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult Edit(int studentId, int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamChange.Edit();
                if (studentId == 0)
                {
                    return Content("<script>alert('请先输入学号或者学生姓名，点击搜索按钮查询学生成绩，才能进行调整');parent.location.reload();</script>");
                }

                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                vm.ExamStatusList = Areas.Exam.Controllers.ExamStatusController.SelectList();
                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamMark>()
                              where p.Id == id
                              select new Dto.ExamChange.Edit
                              {
                                  Id = p.Id,
                                  ExamId = p.tbExamCourse.tbExam.Id,
                                  ExamCourseId = p.tbExamCourse.Id,
                                  AppraiseMark = p.AppraiseMark,
                                  TotalMark = p.TotalMark,
                                  SegmentMark = p.SegmentMark,
                                  ExamLevelId = p.tbExamLevel.Id,
                                  StudentId = p.tbStudent.Id,
                                  LevelGroupId = p.tbExamCourse.tbExamLevelGroup.Id,
                                  ExamStatusId = p.tbExamStatus.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamCourseList = Areas.Exam.Controllers.ExamCourseController.GetExamCourseList(tb.ExamId);
                        vm.ExamLevelList = Areas.Exam.Controllers.ExamLevelController.SelectList(tb.LevelGroupId ?? 0);
                        vm.ExamChangeEdit = tb;
                    }
                }
                else
                {
                    vm.ExamCourseList = Areas.Exam.Controllers.ExamCourseController.GetExamCourseList(vm.ExamList.FirstOrDefault().Value.ConvertToInt());
                    if (vm.ExamCourseList.Count > 0)
                    {
                        vm.ExamLevelList = Areas.Exam.Controllers.ExamLevelController.SelectList(vm.ExamList.FirstOrDefault().Value.ConvertToInt(), vm.ExamCourseList.FirstOrDefault().Value.ConvertToInt());
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamChange.Edit vm, int studentId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamChangeEdit.Id == 0)
                    {
                        var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                                  where p.tbStudent.Id == studentId 
                                  && p.tbExamCourse.Id == vm.ExamChangeEdit.ExamCourseId
                                  && p.tbExamCourse.tbExam.Id == vm.ExamChangeEdit.ExamId
                                  select p).FirstOrDefault();
                        if (tf == null)
                        {
                            var tb = new Exam.Entity.tbExamMark();
                            tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(studentId);
                            tb.tbExamCourse = db.Table<Exam.Entity.tbExamCourse>().Where(d => d.tbExam.Id == vm.ExamChangeEdit.ExamId && d.Id == vm.ExamChangeEdit.ExamCourseId).FirstOrDefault();
                            tb.AppraiseMark = vm.ExamChangeEdit.AppraiseMark;
                            tb.SegmentMark = vm.ExamChangeEdit.SegmentMark;
                            tb.TotalMark = vm.ExamChangeEdit.TotalMark;
                            tb.tbExamStatus = db.Set<Exam.Entity.tbExamStatus>().Find(vm.ExamChangeEdit.ExamStatusId);
                            tb.tbExamLevel = db.Set<Exam.Entity.tbExamLevel>().Find(vm.ExamChangeEdit.ExamLevelId);
                            db.Set<Exam.Entity.tbExamMark>().Add(tb);
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试成绩");
                        }
                        else
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考试成绩");
                            tf.AppraiseMark = vm.ExamChangeEdit.AppraiseMark;
                            tf.TotalMark = vm.ExamChangeEdit.TotalMark;
                            tf.SegmentMark = vm.ExamChangeEdit.SegmentMark;
                            tf.tbExamLevel = db.Set<Exam.Entity.tbExamLevel>().Find(vm.ExamChangeEdit.ExamLevelId);
                            tf.tbExamStatus = db.Set<Exam.Entity.tbExamStatus>().Find(vm.ExamChangeEdit.ExamStatusId);
                        }

                        db.SaveChanges();
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamMark>()
                                  where p.Id == vm.ExamChangeEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.tbExamCourse = db.Table<Exam.Entity.tbExamCourse>().Where(d => d.tbExam.Id == vm.ExamChangeEdit.ExamId && d.Id == vm.ExamChangeEdit.ExamCourseId).FirstOrDefault();
                            tb.AppraiseMark = vm.ExamChangeEdit.AppraiseMark;
                            tb.TotalMark = vm.ExamChangeEdit.TotalMark;
                            tb.SegmentMark = vm.ExamChangeEdit.SegmentMark;
                            tb.tbExamStatus = db.Set<Exam.Entity.tbExamStatus>().Find(vm.ExamChangeEdit.ExamStatusId);
                            tb.tbExamLevel = db.Set<Exam.Entity.tbExamLevel>().Find(vm.ExamChangeEdit.ExamLevelId);
                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考试成绩");
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
    }
}