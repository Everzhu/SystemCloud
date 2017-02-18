using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamReportController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.CourseList = Areas.Exam.Controllers.ExamCourseController.SelectExamCourseList(vm.ExamId);
                if (vm.CourseId == 0 && vm.CourseList.Count > 0)
                {
                    vm.CourseId = vm.CourseList.FirstOrDefault().Value.ConvertToInt();
                }

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && (p.tbCourse.Id == vm.CourseId || vm.CourseId == 0)
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();

                vm.SubjectList = (from p in SubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                //分数项目
                vm.OptionList = new List<string>() { "过程分", "考试成绩 ", "班名", "级名", "综合成绩", "班名", "级名", "等级" };

                var tg = from p in db.Table<Exam.Entity.tbExamMark>()
                         where ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                          && p.tbExamCourse.IsDeleted == false
                          && (p.tbExamCourse.tbCourse.Id == vm.CourseId || vm.CourseId == 0)
                          && p.tbExamCourse.tbExam.Id == vm.ExamId
                         select p;

                var tf = (from t in tg
                          orderby t.tbStudent.StudentCode descending
                          select new Dto.ExamReport.List
                          {
                              AppraiseMark = t.AppraiseMark,
                              SegmentMark = t.SegmentMark,
                              TotalMark = t.TotalMark,
                              TotalClassRank = t.TotalClassRank,
                              TotalGradeRank = t.TotalGradeRank,
                              SegmentClassRank = t.SegmentClassRank,
                              SegmentGradeRank = t.SegmentGradeRank,
                              ExamLevelName = t.tbExamLevel.ExamLevelName,
                              ExamStatusName = t.tbExamStatus.ExamStatusName,
                              StudentCode = t.tbStudent.StudentCode,
                              StudentName = t.tbStudent.StudentName,
                              StudentId = t.tbStudent.Id,
                              SubjectId = t.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();
                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == vm.ExamId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                //学生班级
                var classStudentList = from p in db.Table<Basis.Entity.tbClassStudent>()
                                       where p.tbStudent.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbClass.tbGrade.IsDeleted == false
                                        && p.tbClass.tbClassType.IsDeleted == false
                                        && p.tbClass.tbYear.Id == yearId
                                        && ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                                       select p;

                var classStudent = (from p in classStudentList
                                    select new Dto.ExamReport.List
                                    {
                                        ClassId = p.tbClass.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName
                                    }).ToList();

                var tb = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              AppraiseMark = p.AppraiseMark,
                              SegmentMark = p.SegmentMark,
                              TotalMark = p.TotalMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              SegmentClassRank = p.SegmentClassRank,
                              SegmentGradeRank = p.SegmentGradeRank,
                              ExamLevelName = p.ExamLevelName,
                              ClassName = t.ClassName,
                              t.ClassId
                          }).ToList();

                vm.ClassStudentList = (from p in tb
                                       group p by new
                                       {
                                           p.ClassId,
                                           p.ClassName,
                                           p.StudentId,
                                           p.StudentCode,
                                           p.StudentName
                                       } into g
                                       orderby g.Key.ClassName, g.Key.StudentCode
                                       select new Dto.ExamReport.List
                                       {
                                           ClassId = g.Key.ClassId,
                                           ClassName = g.Key.ClassName,
                                           StudentId = g.Key.StudentId,
                                           StudentCode = g.Key.StudentCode,
                                           StudentName = g.Key.StudentName
                                       }).ToList();

                vm.ExamMarkList = (from p in tb
                                   select new Dto.ExamReport.List
                                   {
                                       StudentId = p.StudentId,
                                       SubjectId = p.SubjectId,
                                       StudentCode = p.StudentCode,
                                       StudentName = p.StudentName,
                                       AppraiseMark = p.AppraiseMark,
                                       SegmentMark = p.SegmentMark,
                                       TotalMark = p.TotalMark,
                                       TotalClassRank = p.TotalClassRank,
                                       TotalGradeRank = p.TotalGradeRank,
                                       SegmentClassRank = p.SegmentClassRank,
                                       SegmentGradeRank = p.SegmentGradeRank,
                                       ExamLevelName = p.ExamLevelName,
                                       ClassName = p.ClassName
                                   }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                ExamId = vm.ExamId,
                CourseId = vm.CourseId,
                searchText = vm.SearchText
            }));
        }

        public ActionResult ClassList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == vm.ExamId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                var teacher = (from p in db.Table<Teacher.Entity.tbTeacher>()
                               where p.tbSysUser.Id == Code.Common.UserId
                               select new
                               {
                                   p.Id,
                                   p.TeacherName
                               }).FirstOrDefault();
                if (teacher == null) return View(vm);

                vm.ClassList = Areas.Basis.Controllers.ClassController.SelectClassList(yearId, teacher.Id);

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();

                vm.SubjectList = (from p in SubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                //分数项目
                vm.OptionList = new List<string>() { "过程分", "考试成绩 ", "班名", "级名", "综合成绩", "班名", "级名", "等级" };

                var classIds = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                where p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && p.tbTeacher.Id == teacher.Id
                                orderby p.tbClass.No
                                select p.tbClass.Id).ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                          && p.tbExamCourse.IsDeleted == false
                          && (p.tbExamCourse.tbExam.Id == vm.ExamId || vm.ExamId == 0)
                          select new
                          {
                              p.AppraiseMark,
                              p.SegmentMark,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              p.SegmentClassRank,
                              p.SegmentGradeRank,
                              p.tbExamLevel.ExamLevelName,
                              p.tbExamStatus.ExamStatusName,
                              p.tbStudent.StudentCode,
                              p.tbStudent.StudentName,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                //学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && (p.tbClass.Id == vm.ClassId || vm.ClassId == 0)
                                    && classIds.Contains(p.tbClass.Id)
                                    && ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var tb = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              AppraiseMark = p.AppraiseMark,
                              SegmentMark = p.SegmentMark,
                              TotalMark = p.TotalMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              SegmentClassRank = p.SegmentClassRank,
                              SegmentGradeRank = p.SegmentGradeRank,
                              ExamLevelName = p.ExamLevelName,
                              ClassName = t.ClassName
                          }).ToList();

                vm.ExamMarkList = (from p in tb
                                   select new Dto.ExamReport.List
                                   {
                                       StudentId = p.StudentId,
                                       SubjectId = p.SubjectId,
                                       StudentCode = p.StudentCode,
                                       StudentName = p.StudentName,
                                       AppraiseMark = p.AppraiseMark,
                                       SegmentMark = p.SegmentMark,
                                       TotalMark = p.TotalMark,
                                       TotalClassRank = p.TotalClassRank,
                                       TotalGradeRank = p.TotalGradeRank,
                                       SegmentClassRank = p.SegmentClassRank,
                                       SegmentGradeRank = p.SegmentGradeRank,
                                       ExamLevelName = p.ExamLevelName,
                                       ClassTeacherName = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.tbSysUser.Id == Code.Common.UserId).Select(d => d.TeacherName).FirstOrDefault(),
                                       ClassName = p.ClassName
                                   }).ToList();

                vm.ClassStudentList = (from p in vm.ExamMarkList
                                       select new Dto.ExamReport.List
                                       {
                                           StudentId = p.StudentId,
                                           StudentCode = p.StudentCode,
                                           StudentName = p.StudentName,
                                           ClassTeacherName = p.ClassTeacherName,
                                           ClassName = p.ClassName
                                       }).Distinct().ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassList(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassList", new { ExamId = vm.ExamId, ClassId = vm.ClassId, searchText = vm.SearchText }));
        }


        public ActionResult OrgList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                YearId = p.tbYear.Id,
                            }).FirstOrDefault();
                var teacher = (from p in db.Table<Teacher.Entity.tbTeacher>()
                               where p.tbSysUser.Id == Code.Common.UserId
                               select new
                               {
                                   p.Id,
                                   p.TeacherName
                               }).FirstOrDefault();
                if (teacher == null) return View(vm);

                vm.OrgList = Areas.Course.Controllers.OrgTeacherController.SelectOrgList(teacher.Id, year.YearId);
                if (vm.OrgId == 0 && vm.OrgList.Count > 0)
                {
                    vm.OrgId = vm.OrgList.FirstOrDefault().Value.ConvertToInt();
                }

                var subjectId = (from p in db.Table<Course.Entity.tbOrg>()
                                 where p.Id == vm.OrgId
                                 select p.tbCourse.tbSubject.Id).FirstOrDefault();

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && p.tbCourse.tbSubject.Id == subjectId
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();

                vm.SubjectList = (from p in SubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                //分数项目
                vm.OptionList = new List<string>() { "过程分", "考试成绩 ", "班名", "级名", "综合成绩", "班名", "级名", "等级" };

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                          && p.tbExamCourse.IsDeleted == false
                          && (p.tbExamCourse.tbExam.Id == vm.ExamId || vm.ExamId == 0)
                          && p.tbExamCourse.tbCourse.tbSubject.Id == subjectId
                          select new
                          {
                              p.AppraiseMark,
                              p.SegmentMark,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              p.SegmentClassRank,
                              p.SegmentGradeRank,
                              p.tbExamLevel.ExamLevelName,
                              p.tbExamStatus.ExamStatusName,
                              p.tbStudent.StudentCode,
                              p.tbStudent.StudentName,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                //学生班级
                var orgtype = (from p in db.Table<Course.Entity.tbOrg>()
                               where p.Id == vm.OrgId
                               && (p.tbClass.IsDeleted == false || p.tbClass == null)
                               && p.tbCourse.IsDeleted == false
                               select new
                               {
                                   p.IsClass,
                                   Id = p.tbClass != null ? p.tbClass.Id : 0,
                                   p.tbCourse.CourseName,
                                   p.OrgName
                               }).FirstOrDefault();
                if (orgtype == null) return View(vm);

                if (orgtype.IsClass == false)//走读班模式
                {
                    var orgStudent = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                      where p.tbStudent.IsDeleted == false
                                      && p.tbOrg.IsDeleted == false
                                      && (p.tbOrg.Id == vm.OrgId || vm.OrgId == 0)
                                      && ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                                      select new
                                      {
                                          OrgId = p.tbOrg.Id,
                                          p.tbOrg.OrgName,
                                          StudentId = p.tbStudent.Id,
                                      }).ToList();
                    var tb = (from p in tf
                              join t in orgStudent
                              on new { p.StudentId } equals new { t.StudentId }
                              select new
                              {
                                  StudentId = p.StudentId,
                                  SubjectId = p.SubjectId,
                                  StudentCode = p.StudentCode,
                                  StudentName = p.StudentName,
                                  AppraiseMark = p.AppraiseMark,
                                  SegmentMark = p.SegmentMark,
                                  TotalMark = p.TotalMark,
                                  TotalClassRank = p.TotalClassRank,
                                  TotalGradeRank = p.TotalGradeRank,
                                  SegmentClassRank = p.SegmentClassRank,
                                  SegmentGradeRank = p.SegmentGradeRank,
                                  ExamLevelName = p.ExamLevelName,
                                  OrgName = t.OrgName
                              }).ToList();
                    vm.ExamMarkList = (from p in tb
                                       select new Dto.ExamReport.List
                                       {
                                           StudentId = p.StudentId,
                                           SubjectId = p.SubjectId,
                                           StudentCode = p.StudentCode,
                                           StudentName = p.StudentName,
                                           AppraiseMark = p.AppraiseMark,
                                           SegmentMark = p.SegmentMark,
                                           TotalMark = p.TotalMark,
                                           TotalClassRank = p.TotalClassRank,
                                           TotalGradeRank = p.TotalGradeRank,
                                           SegmentClassRank = p.SegmentClassRank,
                                           SegmentGradeRank = p.SegmentGradeRank,
                                           ExamLevelName = p.ExamLevelName,
                                           OrgTeacherName = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.tbSysUser.Id == Code.Common.UserId).Select(d => d.TeacherName).FirstOrDefault(),
                                           ClassName = p.OrgName
                                       }).ToList();

                    vm.ClassStudentList = (from p in vm.ExamMarkList
                                           select new Dto.ExamReport.List
                                           {
                                               StudentId = p.StudentId,
                                               StudentCode = p.StudentCode,
                                               StudentName = p.StudentName,
                                               OrgTeacherName = p.OrgTeacherName,
                                               ClassName = p.ClassName
                                           }).Distinct().ToList();
                }
                else //行政班模式
                {
                    var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                            where p.tbClass.Id == orgtype.Id
                                            && p.tbStudent.IsDeleted == false
                                            orderby p.No, p.tbStudent.StudentCode
                                            select new
                                            {
                                                p.No,
                                                StudentId = p.tbStudent.Id,
                                                p.tbStudent.StudentCode,
                                                p.tbStudent.StudentName,
                                                CourseName = orgtype.CourseName,
                                                OrgName = orgtype.OrgName
                                            }).ToList();
                    var tb = (from p in tf
                              join t in classStudentList
                              on new { p.StudentId } equals new { t.StudentId }
                              select new
                              {
                                  StudentId = p.StudentId,
                                  SubjectId = p.SubjectId,
                                  StudentCode = p.StudentCode,
                                  StudentName = p.StudentName,
                                  AppraiseMark = p.AppraiseMark,
                                  SegmentMark = p.SegmentMark,
                                  TotalMark = p.TotalMark,
                                  TotalClassRank = p.TotalClassRank,
                                  TotalGradeRank = p.TotalGradeRank,
                                  SegmentClassRank = p.SegmentClassRank,
                                  SegmentGradeRank = p.SegmentGradeRank,
                                  ExamLevelName = p.ExamLevelName,
                                  OrgName = t.OrgName
                              }).ToList();
                    vm.ExamMarkList = (from p in tb
                                       select new Dto.ExamReport.List
                                       {
                                           StudentId = p.StudentId,
                                           SubjectId = p.SubjectId,
                                           StudentCode = p.StudentCode,
                                           StudentName = p.StudentName,
                                           AppraiseMark = p.AppraiseMark,
                                           SegmentMark = p.SegmentMark,
                                           TotalMark = p.TotalMark,
                                           TotalClassRank = p.TotalClassRank,
                                           TotalGradeRank = p.TotalGradeRank,
                                           SegmentClassRank = p.SegmentClassRank,
                                           SegmentGradeRank = p.SegmentGradeRank,
                                           ExamLevelName = p.ExamLevelName,
                                           OrgTeacherName = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.tbSysUser.Id == Code.Common.UserId).Select(d => d.TeacherName).FirstOrDefault(),
                                           ClassName = p.OrgName
                                       }).ToList();

                    vm.ClassStudentList = (from p in vm.ExamMarkList
                                           select new Dto.ExamReport.List
                                           {
                                               StudentId = p.StudentId,
                                               StudentCode = p.StudentCode,
                                               StudentName = p.StudentName,
                                               OrgTeacherName = p.OrgTeacherName,
                                               ClassName = p.ClassName
                                           }).Distinct().ToList();

                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrgList(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("OrgList", new { ExamId = vm.ExamId, OrgId = vm.OrgId, searchText = vm.SearchText }));
        }

        public ActionResult StudentList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == vm.ExamId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.tbSysUser.Id == Code.Common.UserId
                               select new
                               {
                                   p.Id,
                                   p.StudentCode,
                                   p.StudentName
                               }).FirstOrDefault();

                if (student == null) return View(vm);
                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false
                                    && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();

                vm.SubjectList = (from p in SubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                //分数项目
                vm.OptionList = new List<string>() { "过程分", "考试成绩 ", "班名", "级名", "综合成绩", "班名", "级名", "等级" };


                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbStudent.Id == student.Id
                          && p.tbExamCourse.IsDeleted == false
                          && (p.tbExamCourse.tbExam.Id == vm.ExamId || vm.ExamId == 0)
                          select new
                          {
                              p.AppraiseMark,
                              p.SegmentMark,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              p.SegmentClassRank,
                              p.SegmentGradeRank,
                              p.tbExamLevel.ExamLevelName,
                              p.tbExamStatus.ExamStatusName,
                              p.tbStudent.StudentCode,
                              p.tbStudent.StudentName,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                //学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && (p.tbStudent.Id == student.Id)
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var tb = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              AppraiseMark = p.AppraiseMark,
                              SegmentMark = p.SegmentMark,
                              TotalMark = p.TotalMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              SegmentClassRank = p.SegmentClassRank,
                              SegmentGradeRank = p.SegmentGradeRank,
                              ExamLevelName = p.ExamLevelName,
                              ClassName = t.ClassName
                          }).ToList();

                vm.ExamMarkList = (from p in tb
                                   select new Dto.ExamReport.List
                                   {
                                       StudentId = p.StudentId,
                                       SubjectId = p.SubjectId,
                                       StudentCode = p.StudentCode,
                                       StudentName = p.StudentName,
                                       AppraiseMark = p.AppraiseMark,
                                       SegmentMark = p.SegmentMark,
                                       TotalMark = p.TotalMark,
                                       TotalClassRank = p.TotalClassRank,
                                       TotalGradeRank = p.TotalGradeRank,
                                       SegmentClassRank = p.SegmentClassRank,
                                       SegmentGradeRank = p.SegmentGradeRank,
                                       ExamLevelName = p.ExamLevelName,
                                       ClassName = p.ClassName
                                   }).ToList();

                vm.ClassStudentList = (from p in vm.ExamMarkList
                                       select new Dto.ExamReport.List
                                       {
                                           StudentId = p.StudentId,
                                           StudentCode = p.StudentCode,
                                           StudentName = p.StudentName,
                                           ClassName = p.ClassName
                                       }).Distinct().ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentList(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentList", new { ExamId = vm.ExamId }));
        }

        #region 西乡出国报表
        public ActionResult StudentMarkList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }
                vm.ClassList = Areas.Basis.Controllers.ClassController.SelectClassList(0, vm.GradeId);
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                var subjectlist = new List<string>() { "语文", "数学", "英语", "物理", "化学", "历史", "地理", "生物", "政治", "体育", "音乐", "美术" };
                var sectionSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                          where subjectlist.Contains(p.SubjectName)
                                          orderby p.No, p.SubjectName
                                          select new Dto.ExamReport.SubjectList
                                          {
                                              SubjectId = p.Id,
                                              SubjectName = p.SubjectName,
                                              SubjectNameEn = p.SubjectNameEn
                                          }).ToList();

                vm.SectionSubjectList = sectionSubjectList;

                //表头项目
                vm.GradeTypeList = new List<SelectListItem>()
                {
                    new System.Web.Mvc.SelectListItem { Text = "高一", Value = "Grade10" },
                    new System.Web.Mvc.SelectListItem { Text = "高二", Value = "Grade11" },
                    new System.Web.Mvc.SelectListItem { Text = "高三", Value = "Grade12" }
                 };
                vm.OptionList = new List<string>() { "1st term", "2nd term" };

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && p.tbExamCourse.tbExam.tbYear.IsDeleted == false
                           && p.tbExamCourse.tbExamSection.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                          select new
                          {
                              p.TotalMark,
                              p.tbStudent.StudentCode,
                              p.tbStudent.StudentName,
                              StudentId = p.tbStudent.Id,
                              GradeId = p.tbExamCourse.tbExamSection.tbGrade.Id,
                              GradeName = p.tbExamCourse.tbExamSection.tbGrade.GradeName,
                              p.tbExamCourse.tbExamSection.ExamSectionName,
                              p.tbExamCourse.tbExamSection.ExamSectionNameEn,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              SubjectName = p.tbExamCourse.tbCourse.tbSubject.SubjectName
                          }).ToList();

                //学生班级
                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                        where p.tbStudent.IsDeleted == false
                                        && (p.tbClass.Id == vm.ClassId || vm.ClassId == 0)
                                        && (p.tbClass.tbGrade.Id == vm.GradeId || vm.GradeId == 0)
                                        && ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                                        select new
                                        {
                                            StudentId = p.tbStudent.Id,
                                            StudentCode = p.tbStudent.StudentCode,
                                            StudentName = p.tbStudent.StudentName,
                                            StudentNameEn = p.tbStudent.StudentNameEn,
                                            Birthday = p.tbStudent.Birthday,
                                            Birthday_Eng = p.tbStudent.Birthday,
                                            EntranceDate = p.tbStudent.EntranceDate,
                                            EntranceDateEn = p.tbStudent.EntranceDate,
                                            Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                        }).ToList();

                var classStudent = (from t in classStudentList
                                    select new Dto.ExamReport.List
                                    {
                                        StudentId = t.StudentId,
                                        StudentCode = t.StudentCode,
                                        Birthday = "出生年月:" + Convert.ToDateTime(t.Birthday).ToString("MM/yyyy"),
                                        BirthdayEn = "Birthday Date:" + this.NumberTranEng(Convert.ToDateTime(t.Birthday).Month) + " " + Convert.ToDateTime(t.Birthday).Year,
                                        EntranceDate = "入学时间:" + Convert.ToDateTime(t.EntranceDate).ToString("MM/yyyy"),
                                        EntranceDateEn = "Entrance Date:" + this.NumberTranEng(Convert.ToDateTime(t.EntranceDateEn).Month) + " " + Convert.ToDateTime(t.EntranceDateEn).Year,
                                        GradeDate = "毕业时间:" + "07" + "/" + Convert.ToDateTime(Convert.ToDateTime(t.EntranceDate).Year + 3 + "-07-10").Year.ToString(),
                                        GradeDateEn = "Time of graduation:" + this.NumberTranEng(7) + " " + (Convert.ToDateTime(t.EntranceDate).Year + 3).ToString(),
                                        IssueDate = DateTime.Now.ToString("yyyy年MM月dd日"),
                                        SexName = "性别:" + t.Sex,
                                        SexNameEn = t.Sex != null ? t.Sex == "男" ? "Sex:Male" : "Sex:Female" : "Sex:" + null,
                                        StudentName = "姓名:" + t.StudentName,
                                        StudentNameEn = "Name:" + t.StudentNameEn,
                                    }).ToList();

                vm.ClassStudentList = classStudent;

                var tb = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              TotalMark = p.TotalMark,
                              p.ExamSectionName,
                              p.ExamSectionNameEn,
                              GradeName = p.GradeName,
                          }).ToList();

                vm.ExamMarkList = (from p in tb
                                   select new Dto.ExamReport.List
                                   {
                                       StudentId = p.StudentId,
                                       SubjectId = p.SubjectId,
                                       StudentCode = p.StudentCode,
                                       TotalMark = p.TotalMark,
                                       GradeName = p.GradeName,
                                       ExamSectionName = p.ExamSectionName ?? "",
                                       ExamSectionNameEn = p.ExamSectionNameEn ?? ""
                                   }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentMarkList(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentMarkList", new
            {
                GradeId = vm.GradeId,
                ClassId = vm.ClassId,
                searchText = vm.SearchText
            }));
        }
        #endregion

        #region 北师大南山分校出国报表
        public ActionResult StudentSeniorMarkList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectHighList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }
                vm.ClassList = Areas.Basis.Controllers.ClassController.SelectClassList(0, vm.GradeId);
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                var subjectlist = new List<string>() { "语文", "数学", "英语", "物理", "化学", "历史", "地理", "生物", "政治", "体育", "音乐", "美术" };
                var sectionSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                          where subjectlist.Contains(p.SubjectName)
                                          orderby p.No, p.SubjectName
                                          select new Dto.ExamReport.SubjectList
                                          {
                                              SubjectId = p.Id,
                                              SubjectName = p.SubjectName,
                                              SubjectNameEn = p.SubjectNameEn
                                          }).ToList();

                vm.SectionSubjectList = sectionSubjectList;

                //表头项目
                vm.GradeTypeList = new List<SelectListItem>()
                {
                    new System.Web.Mvc.SelectListItem { Text = "高一年级", Value = "Grade One" },
                    new System.Web.Mvc.SelectListItem { Text = "高二年级", Value = "Grade Two" },
                    new System.Web.Mvc.SelectListItem { Text = "高三年级", Value = "Grade Three" }
                 };
                vm.OptionEnChList = new List<SelectListItem>()
                {
                    new System.Web.Mvc.SelectListItem { Text = "第一学期", Value = "The 1st term" },
                    new System.Web.Mvc.SelectListItem { Text = "第二学期", Value = "The 2nd term" }
                 };

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && p.tbExamCourse.tbExam.tbYear.IsDeleted == false
                           && p.tbExamCourse.tbExamSection.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                          select new
                          {
                              p.TotalMark,
                              p.tbStudent.StudentCode,
                              p.tbStudent.StudentName,
                              StudentId = p.tbStudent.Id,
                              GradeId = p.tbExamCourse.tbExamSection.tbGrade.Id,
                              GradeName = p.tbExamCourse.tbExamSection.tbGrade.GradeName,
                              p.tbExamCourse.tbExamSection.ExamSectionName,
                              p.tbExamCourse.tbExamSection.ExamSectionNameEn,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              SubjectName = p.tbExamCourse.tbCourse.tbSubject.SubjectName
                          }).ToList();

                //学生班级
                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                        where p.tbStudent.IsDeleted == false
                                        && (p.tbClass.Id == vm.ClassId || vm.ClassId == 0)
                                        && (p.tbClass.tbGrade.Id == vm.GradeId || vm.GradeId == 0)
                                        && ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                                        select new
                                        {
                                            StudentId = p.tbStudent.Id,
                                            StudentCode = p.tbStudent.StudentCode,
                                            StudentName = p.tbStudent.StudentName,
                                            StudentNameEn = p.tbStudent.StudentNameEn,
                                            Birthday = p.tbStudent.Birthday,
                                            Birthday_Eng = p.tbStudent.Birthday,
                                            EntranceDate = p.tbStudent.EntranceDate,
                                            EntranceDateEn = p.tbStudent.EntranceDate,
                                            Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                        }).ToList();

                var classStudent = (from t in classStudentList
                                    select new Dto.ExamReport.List
                                    {
                                        StudentId = t.StudentId,
                                        StudentCode = t.StudentCode,
                                        Birthday = "出生日期(Birth Date):" + Convert.ToDateTime(t.Birthday).Year + "." + Convert.ToDateTime(t.Birthday).Month.ToString().PadLeft(2, '0') + "." + Convert.ToDateTime(t.Birthday).Day.ToString().PadLeft(2, '0'),
                                        EntranceDate = "入学日期(Enrollment Date):" + Convert.ToDateTime(t.EntranceDate).Year + ".09" + ".01",
                                        SexName = "性别(Sex):" + t.Sex + (t.Sex != null ? t.Sex == "男" ? "(Male)" : "(Female)" : null),
                                        StudentName = "姓名(Name):" + t.StudentName + (t.StudentNameEn),
                                    }).ToList();

                vm.ClassStudentList = classStudent;

                var tb = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              TotalMark = p.TotalMark,
                              p.ExamSectionName,
                              p.ExamSectionNameEn,
                              GradeName = p.GradeName,
                          }).ToList();

                vm.ExamMarkList = (from p in tb
                                   select new Dto.ExamReport.List
                                   {
                                       StudentId = p.StudentId,
                                       SubjectId = p.SubjectId,
                                       StudentCode = p.StudentCode,
                                       TotalMark = p.TotalMark,
                                       GradeName = p.GradeName,
                                       ExamSectionName = p.ExamSectionName ?? "",
                                       ExamSectionNameEn = p.ExamSectionNameEn ?? ""
                                   }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentSeniorMarkList(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentSeniorMarkList", new
            {
                GradeId = vm.GradeId,
                ClassId = vm.ClassId,
                searchText = vm.SearchText
            }));
        }

        public ActionResult StudentJeniorMarkList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectJuniorList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }
                vm.ClassList = Areas.Basis.Controllers.ClassController.SelectClassList(0, vm.GradeId);
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                var subjectlist = new List<string>() { "语文", "数学", "英语", "物理", "化学", "历史", "地理", "生物", "政治", "体育", "音乐", "美术" };
                var sectionSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                          where subjectlist.Contains(p.SubjectName)
                                          orderby p.No, p.SubjectName
                                          select new Dto.ExamReport.SubjectList
                                          {
                                              SubjectId = p.Id,
                                              SubjectName = p.SubjectName,
                                              SubjectNameEn = p.SubjectNameEn
                                          }).ToList();

                vm.SectionSubjectList = sectionSubjectList;

                //表头项目
                vm.GradeTypeList = new List<SelectListItem>()
                {
                    new System.Web.Mvc.SelectListItem { Text = "初一年级", Value = "Grade One" },
                    new System.Web.Mvc.SelectListItem { Text = "初二年级", Value = "Grade Two" },
                    new System.Web.Mvc.SelectListItem { Text = "初三年级", Value = "Grade Three" }
                 };
                vm.OptionEnChList = new List<SelectListItem>()
                {
                    new System.Web.Mvc.SelectListItem { Text = "第一学期", Value = "The 1st term" },
                    new System.Web.Mvc.SelectListItem { Text = "第二学期", Value = "The 2nd term" }
                 };

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && p.tbExamCourse.tbExam.tbYear.IsDeleted == false
                           && p.tbExamCourse.tbExamSection.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                          select new
                          {
                              p.TotalMark,
                              p.tbStudent.StudentCode,
                              p.tbStudent.StudentName,
                              StudentId = p.tbStudent.Id,
                              GradeId = p.tbExamCourse.tbExamSection.tbGrade.Id,
                              GradeName = p.tbExamCourse.tbExamSection.tbGrade.GradeName,
                              p.tbExamCourse.tbExamSection.ExamSectionName,
                              p.tbExamCourse.tbExamSection.ExamSectionNameEn,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              SubjectName = p.tbExamCourse.tbCourse.tbSubject.SubjectName
                          }).ToList();

                //学生班级
                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                        where p.tbStudent.IsDeleted == false
                                        && (p.tbClass.Id == vm.ClassId || vm.ClassId == 0)
                                        && (p.tbClass.tbGrade.Id == vm.GradeId || vm.GradeId == 0)
                                        && ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                                        select new
                                        {
                                            StudentId = p.tbStudent.Id,
                                            StudentCode = p.tbStudent.StudentCode,
                                            StudentName = p.tbStudent.StudentName,
                                            StudentNameEn = p.tbStudent.StudentNameEn,
                                            Birthday = p.tbStudent.Birthday,
                                            Birthday_Eng = p.tbStudent.Birthday,
                                            EntranceDate = p.tbStudent.EntranceDate,
                                            EntranceDateEn = p.tbStudent.EntranceDate,
                                            Sex = p.tbStudent.tbSysUser.tbSex.SexName,
                                        }).ToList();

                var classStudent = (from t in classStudentList
                                    select new Dto.ExamReport.List
                                    {
                                        StudentId = t.StudentId,
                                        StudentCode = t.StudentCode,
                                        Birthday = "出生日期(Birth Date):" + Convert.ToDateTime(t.Birthday).Year + "." + Convert.ToDateTime(t.Birthday).Month.ToString().PadLeft(2, '0') + "." + Convert.ToDateTime(t.Birthday).Day.ToString().PadLeft(2, '0'),
                                        EntranceDate = "入学日期(Enrollment Date):" + Convert.ToDateTime(t.EntranceDate).Year + ".09" + ".01",
                                        SexName = "性别(Sex):" + t.Sex + (t.Sex != null ? t.Sex == "男" ? "(Male)" : "(Female)" : null),
                                        StudentName = "姓名(Name):" + t.StudentName + (t.StudentNameEn),
                                    }).ToList();

                vm.ClassStudentList = classStudent;

                var tb = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              TotalMark = p.TotalMark,
                              p.ExamSectionName,
                              p.ExamSectionNameEn,
                              GradeName = p.GradeName,
                          }).ToList();

                vm.ExamMarkList = (from p in tb
                                   select new Dto.ExamReport.List
                                   {
                                       StudentId = p.StudentId,
                                       SubjectId = p.SubjectId,
                                       StudentCode = p.StudentCode,
                                       TotalMark = p.TotalMark,
                                       GradeName = p.GradeName,
                                       ExamSectionName = p.ExamSectionName ?? "",
                                       ExamSectionNameEn = p.ExamSectionNameEn ?? ""
                                   }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentJeniorMarkList(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentJeniorMarkList", new
            {
                GradeId = vm.GradeId,
                ClassId = vm.ClassId,
                searchText = vm.SearchText
            }));
        }
        #endregion

        #region 查学生总成绩单
        public ActionResult StudentTotalList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();

                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where (p.StudentCode == vm.SearchText
                               || p.StudentName == vm.SearchText
                               || (p.tbSysUser.Id == Code.Common.UserId && Code.Common.UserType == Code.EnumHelper.SysUserType.Student)
                               || (p.tbSysUserFamily.Id == Code.Common.UserId && Code.Common.UserType == Code.EnumHelper.SysUserType.Family))
                               select new
                               {
                                   p.Id,
                                   p.StudentCode,
                                   p.StudentName,
                                   p.EntranceDate
                               }).FirstOrDefault();
                var studentcode = "学号:" + (student != null ? student.StudentCode : string.Empty);
                var studentName = "姓名:" + (student != null ? student.StudentName : string.Empty);
                var entranceDate = "入学时间:" + (student != null ? (student.EntranceDate != null ? Convert.ToDateTime(student.EntranceDate.ToString()).ToString(Code.Common.StringToDate) : string.Empty) : string.Empty);
                vm.OptionList = new List<string>() { studentcode, studentName, entranceDate };

                if (student == null)
                {
                    vm.GradeOptionList = new List<string>() { "获得总学分:", "总学分积点:", "平均学分积点:" };
                    return View(vm);
                }

                vm.CommentList = Perform.Controllers.PerformCommentController.GetAllComment(student.Id);

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbStudent.Id == student.Id
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                          orderby p.tbExamCourse.tbCourse.tbCourseDomain.No, p.tbExamCourse.tbCourse.tbSubject.No,
                           p.tbExamCourse.tbCourse.No
                          select new
                          {
                              p.SegmentMark,
                              p.tbExamCourse.tbCourse.tbCourseDomain.CourseDomainName,
                              p.tbExamCourse.tbCourse.tbSubject.SubjectName,
                              p.tbExamCourse.tbCourse.CourseName,
                              Point= p.tbExamLevel != null ? p.tbExamCourse.tbCourse.Point:decimal.Zero,
                              p.tbExamCourse.tbCourse.tbCourseType.CourseTypeName,
                              p.tbExamLevel.ExamLevelName,
                              ExamLevelValue = p.tbExamLevel != null ? p.tbExamLevel.ExamLevelValue : (decimal?)null,
                              StudyPoint = p.tbExamCourse.tbCourse.Point * (p.tbExamLevel != null ? p.tbExamLevel.ExamLevelValue : (decimal?)null),
                              p.tbExamCourse.tbExamSection.ExamSectionName,
                              p.tbTeacher.TeacherName
                          }).ToList();

                vm.ExamMarkList = (from p in tf
                                   select new Dto.ExamReport.List
                                   {
                                       CourseDomainName = p.CourseDomainName,
                                       SubjectName = p.SubjectName,
                                       CourseName = p.CourseName,
                                       SegmentMark = p.SegmentMark,
                                       Point = p.Point,
                                       CourseTypeName = p.CourseTypeName,
                                       ExamLevelName = p.ExamLevelName,
                                       ExamLevelValue = p.ExamLevelValue,
                                       StudyPoint = p.StudyPoint,
                                       ExamSectionName = p.ExamSectionName,
                                       TeacherName = p.TeacherName
                                   }).ToList();

                var totalPoint = vm.ExamMarkList.Sum(d => d.Point);
                var totalStudyPoint = vm.ExamMarkList.Sum(d => d.StudyPoint ?? 0);
                var totalAvgPoint = totalPoint > decimal.Zero ? decimal.Round(totalStudyPoint / totalPoint, 2, MidpointRounding.AwayFromZero) : decimal.Zero;
                vm.GradeOptionList = new List<string>() { "获得总学分:" + totalPoint, "总学分积点:" + decimal.Round(totalStudyPoint, 2, MidpointRounding.AwayFromZero), "平均学分积点:" + totalAvgPoint };

                return View(vm);
            }
        }

        public ActionResult SearchMarlList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();

                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where (p.StudentCode == vm.SearchText
                               || p.StudentName == vm.SearchText
                               || (p.tbSysUser.Id == Code.Common.UserId && Code.Common.UserType == Code.EnumHelper.SysUserType.Student)
                               || (p.tbSysUserFamily.Id == Code.Common.UserId && Code.Common.UserType == Code.EnumHelper.SysUserType.Family))

                               select new
                               {
                                   p.Id,
                                   p.StudentCode,
                                   p.StudentName,
                               }).FirstOrDefault();
                var studentcode = "学号:" + (student != null ? student.StudentCode : string.Empty);
                var studentName = "姓名:" + (student != null ? student.StudentName : string.Empty);
                vm.OptionList = new List<string>() { studentcode, studentName };

                if (student == null) return View(vm);

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbStudent.Id == student.Id
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                          orderby p.tbExamCourse.tbCourse.tbSubject.No,
                          p.tbExamCourse.tbCourse.No
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.tbExamCourse.tbExam.ExamName,
                              ExamNo = p.tbExamCourse.tbExam.No,
                              p.tbExamStatus.ExamStatusName,
                              p.TotalMark,
                              p.AppraiseMark,
                              p.SegmentMark,
                              p.tbExamCourse.tbCourse.tbSubject.SubjectName,
                              tbSubject = p.tbExamCourse.tbCourse.tbSubject,
                              tbExam = p.tbExamCourse.tbExam,
                              p.tbExamCourse.tbCourse.CourseName,
                              Point = p.tbExamLevel != null ? p.tbExamCourse.tbCourse.Point : decimal.Zero,
                              p.tbExamLevel.ExamLevelName,
                              ExamLevelValue = p.tbExamLevel != null ? p.tbExamLevel.ExamLevelValue : (decimal?)null,
                              ExamLevelGroupId = p.tbExamLevel != null ? p.tbExamLevel.tbExamLevelGroup.Id : 0,
                              StudyPoint = p.tbExamCourse.tbCourse.Point * (p.tbExamLevel != null ? p.tbExamLevel.ExamLevelValue : (decimal?)null),
                              p.tbExamCourse.tbExamSection.ExamSectionName,
                              p.tbTeacher.TeacherName,
                              p.TotalGradeRank,
                          }).ToList();

                //获取科目
                var subjectList = tf.Select(d => d.tbSubject).Distinct().ToList();
                vm.SubjectName = Code.Common.ToJSONString(subjectList.Select(d => d.SubjectName).ToList());
                //获取考试
                var exams = tf.Select(d => d.tbExam).Distinct().ToList();
                vm.ExamName = Code.Common.ToJSONString(exams.Select(d => d.ExamName).ToList());

                var totalGradeList = new List<object>();
                foreach (var subject in subjectList)
                {
                    var scoreGradeList = new List<object>();
                    foreach (var exam in exams)
                    {
                        var examScore = tf.Where(d => d.tbSubject.Id == subject.Id && d.tbExam.Id == exam.Id).FirstOrDefault();
                        if (examScore != null)
                        {
                            scoreGradeList.Add(examScore.TotalGradeRank);
                        }
                    }
                    totalGradeList.Add(new { name = subject.SubjectName, type = "line", data = scoreGradeList });
                }
                vm.ReportScoreGrade = Code.Common.ToJSONString((totalGradeList));

                var examLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                     select new
                                     {
                                         p.No,
                                         p.ExamLevelName,
                                         LevelGroupId = p.tbExamLevelGroup.Id,
                                         p.Rate
                                     }).ToList();

                var examList = (from p in tf
                                select new
                                {
                                    p.ExamName,
                                    p.ExamId,
                                    p.ExamNo
                                }).Distinct().OrderByDescending(d => d.ExamNo).ToList();
                vm.ExamList = (from p in examList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString()
                               }).Distinct().ToList();

                vm.ExamMarkList = (from p in tf
                                   select new Dto.ExamReport.List
                                   {
                                       ExamId = p.ExamId,
                                       SubjectName = p.SubjectName,
                                       CourseName = p.CourseName,
                                       ExamStatusName = p.ExamStatusName,
                                       SegmentMark = p.SegmentMark,
                                       TotalMark = p.TotalMark,
                                       Point = p.Point,
                                       AppraiseMark = p.AppraiseMark,
                                       ExamLevelName = p.ExamLevelName,
                                       ExamLevelRemark = string.Join("<br/>", examLevelList.Where(d => d.LevelGroupId == p.ExamLevelGroupId).Select(d => d.ExamLevelName + "(" + d.Rate + "%)").ToList()),
                                       ExamSectionName = p.ExamSectionName,
                                       StudyPoint = p.StudyPoint,
                                       TeacherName = p.TeacherName
                                   }).ToList();
                return View(vm);
            }
        }

        public ActionResult SearchMarlReport(string subjectName, string examName, string reportScoreGrade)
        {
            var vm = new Models.ExamReport.List();
            vm.SubjectName = subjectName;
            vm.ExamName = examName;
            vm.ReportScoreGrade = reportScoreGrade;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentTotalList(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentTotalList", new { searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchMarlList(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SearchMarlList", new { searchText = vm.SearchText }));
        }
        #endregion

        public string NumberTranEng(int month)
        {
            var flag = string.Empty;
            switch (month)
            {
                case 1:
                    flag = "January";
                    break;
                case 2:
                    flag = "February";
                    break;
                case 3:
                    flag = "March";
                    break;
                case 4:
                    flag = "April";
                    break;
                case 5:
                    flag = "May";
                    break;
                case 6:
                    flag = "June";
                    break;
                case 7:
                    flag = "July";
                    break;
                case 8:
                    flag = "August";
                    break;
                case 9:
                    flag = "September";
                    break;
                case 10:
                    flag = "October";
                    break;
                case 11:
                    flag = "November";
                    break;
                case 12:
                    flag = "December";
                    break;
                default:
                    break;
            }
            return flag;
        }
        #region 导出
        public ActionResult Export(int examId, int? CourseId, string SearchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == examId
                                    && (p.tbCourse.Id == CourseId || CourseId == 0)
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   select new System.Web.Mvc.SelectListItem
                                   {
                                       Text = p.tbCourse.tbSubject.SubjectName,
                                       Value = p.tbCourse.tbSubject.Id.ToString()
                                   }).Distinct().ToList();

                //分数项目
                var OptionList = new List<string>() { "过程分", "考试成绩 ", "考试成绩班名", "考试成绩级名", "综合成绩", "综合成绩班名", "综合成绩级名", "等级" };

                //班级学生
                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where (p.tbStudent.StudentCode.Contains(SearchText) || p.tbStudent.StudentName.Contains(SearchText) || SearchText == null)
                          && p.tbExamCourse.IsDeleted == false
                          && (p.tbExamCourse.tbCourse.Id == CourseId || CourseId == 0)
                           && (p.tbExamCourse.tbExam.Id == examId || examId == 0)
                          select new
                          {
                              p.AppraiseMark,
                              p.SegmentMark,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              p.SegmentClassRank,
                              p.SegmentGradeRank,
                              p.tbExamLevel.ExamLevelName,
                              p.tbExamStatus.ExamStatusName,
                              p.tbStudent.StudentCode,
                              p.tbStudent.StudentName,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();
                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == examId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                //学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && (p.tbStudent.StudentCode.Contains(SearchText) || p.tbStudent.StudentName.Contains(SearchText) || SearchText == null)
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              AppraiseMark = p.AppraiseMark,
                              SegmentMark = p.SegmentMark,
                              TotalMark = p.TotalMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              SegmentClassRank = p.SegmentClassRank,
                              SegmentGradeRank = p.SegmentGradeRank,
                              ExamLevelName = p.ExamLevelName,
                              ClassName = t.ClassName
                          }).ToList();

                var tb = (from p in tg
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              AppraiseMark = p.AppraiseMark,
                              SegmentMark = p.SegmentMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              SegmentClassRank = p.SegmentClassRank,
                              SegmentGradeRank = p.SegmentGradeRank,
                              TotalMark = p.TotalMark,
                              ExamLevelName = p.ExamLevelName,
                              ClassName = p.ClassName
                          }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("班级"),
                    });
                foreach (var subject in SubjectList)
                {
                    for (var i = 0; i < OptionList.Count; i++)
                    {
                        dt.Columns.AddRange(new System.Data.DataColumn[]
                        {
                            new System.Data.DataColumn(subject.Text+OptionList[i])
                        });
                    }
                }
                foreach (var a in tb)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["班级"] = a.ClassName;
                    foreach (var subject in SubjectList)
                    {
                        var mark = tb.Where(d => d.SubjectId == subject.Value.ConvertToInt() && d.StudentId == a.StudentId).Select(d => d).FirstOrDefault();
                        if (mark != null)
                        {
                            for (var i = 0; i < OptionList.Count; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        dr[subject.Text + OptionList[i]] = a.AppraiseMark;
                                        break;
                                    case 1:
                                        dr[subject.Text + OptionList[i]] = a.TotalMark;
                                        break;
                                    case 2:
                                        dr[subject.Text + OptionList[i]] = a.TotalClassRank;
                                        break;
                                    case 3:
                                        dr[subject.Text + OptionList[i]] = a.TotalGradeRank;
                                        break;
                                    case 4:
                                        dr[subject.Text + OptionList[i]] = a.SegmentMark;
                                        break;
                                    case 5:
                                        dr[subject.Text + OptionList[i]] = a.SegmentClassRank;
                                        break;
                                    case 6:
                                        dr[subject.Text + OptionList[i]] = a.SegmentGradeRank;
                                        break;
                                    case 7:
                                        dr[subject.Text + OptionList[i]] = a.ExamLevelName;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            for (var i = 0; i < OptionList.Count; i++)
                            {
                                dr[subject.Text + OptionList[i]] = string.Empty;
                            }
                        }
                    }
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);

                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return View();
                }
            }
        }

        public ActionResult ExportCLass(int examId, int ClassId, string SearchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var teacher = (from p in db.Table<Teacher.Entity.tbTeacher>()
                               where p.tbSysUser.Id == Code.Common.UserId
                               select new
                               {
                                   p.Id,
                                   p.TeacherName
                               }).FirstOrDefault();
                if (teacher == null)
                {
                    return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                }
                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == examId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   select new System.Web.Mvc.SelectListItem
                                   {
                                       Text = p.tbCourse.tbSubject.SubjectName,
                                       Value = p.tbCourse.tbSubject.Id.ToString()
                                   }).Distinct().ToList();

                //分数项目
                var OptionList = new List<string>() { "过程分", "考试成绩 ", "考试成绩班名", "考试成绩级名", "综合成绩", "综合成绩班名", "综合成绩级名", "等级" };

                //班级学生
                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where (p.tbStudent.StudentCode.Contains(SearchText) || p.tbStudent.StudentName.Contains(SearchText) || SearchText == null)
                          && p.tbExamCourse.IsDeleted == false
                          && (p.tbExamCourse.tbExam.Id == examId || examId == 0)
                          select new
                          {
                              p.AppraiseMark,
                              p.SegmentMark,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              p.SegmentClassRank,
                              p.SegmentGradeRank,
                              p.tbExamLevel.ExamLevelName,
                              p.tbExamStatus.ExamStatusName,
                              p.tbStudent.StudentCode,
                              p.tbStudent.StudentName,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();
                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == examId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();

                var classIds = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                where p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && p.tbTeacher.Id == teacher.Id
                                orderby p.tbClass.No
                                select p.tbClass.Id).ToList();
                //学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && (p.tbClass.Id == ClassId || ClassId == 0)
                                    && classIds.Contains(p.tbClass.Id)
                                    && (p.tbStudent.StudentCode.Contains(SearchText) || p.tbStudent.StudentName.Contains(SearchText) || SearchText == null)
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              AppraiseMark = p.AppraiseMark,
                              SegmentMark = p.SegmentMark,
                              TotalMark = p.TotalMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              SegmentClassRank = p.SegmentClassRank,
                              SegmentGradeRank = p.SegmentGradeRank,
                              ExamLevelName = p.ExamLevelName,
                              ClassName = t.ClassName
                          }).ToList();

                var tb = (from p in tg
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              AppraiseMark = p.AppraiseMark,
                              SegmentMark = p.SegmentMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              SegmentClassRank = p.SegmentClassRank,
                              SegmentGradeRank = p.SegmentGradeRank,
                              TotalMark = p.TotalMark,
                              ExamLevelName = p.ExamLevelName,
                              ClassTeacherName = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.tbSysUser.Id == Code.Common.UserId).Select(d => d.TeacherName).FirstOrDefault(),
                              ClassName = p.ClassName
                          }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("班主任"),
                    });
                foreach (var subject in SubjectList)
                {
                    for (var i = 0; i < OptionList.Count; i++)
                    {
                        dt.Columns.AddRange(new System.Data.DataColumn[]
                        {
                            new System.Data.DataColumn(subject.Text+OptionList[i])
                        });
                    }
                }
                foreach (var a in tb)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["班级"] = a.ClassName;
                    dr["班主任"] = a.ClassTeacherName;
                    foreach (var subject in SubjectList)
                    {
                        var mark = tb.Where(d => d.SubjectId == subject.Value.ConvertToInt() && d.StudentId == a.StudentId).Select(d => d).FirstOrDefault();
                        if (mark != null)
                        {
                            for (var i = 0; i < OptionList.Count; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        dr[subject.Text + OptionList[i]] = a.AppraiseMark;
                                        break;
                                    case 1:
                                        dr[subject.Text + OptionList[i]] = a.TotalMark;
                                        break;
                                    case 2:
                                        dr[subject.Text + OptionList[i]] = a.TotalClassRank;
                                        break;
                                    case 3:
                                        dr[subject.Text + OptionList[i]] = a.TotalGradeRank;
                                        break;
                                    case 4:
                                        dr[subject.Text + OptionList[i]] = a.SegmentMark;
                                        break;
                                    case 5:
                                        dr[subject.Text + OptionList[i]] = a.SegmentClassRank;
                                        break;
                                    case 6:
                                        dr[subject.Text + OptionList[i]] = a.SegmentGradeRank;
                                        break;
                                    case 7:
                                        dr[subject.Text + OptionList[i]] = a.ExamLevelName;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            for (var i = 0; i < OptionList.Count; i++)
                            {
                                dr[subject.Text + OptionList[i]] = string.Empty;
                            }
                        }
                    }
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);

                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return View();
                }
            }
        }

        public ActionResult ExportOrg(int examId, int OrgId, string SearchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var teacher = (from p in db.Table<Teacher.Entity.tbTeacher>()
                               where p.tbSysUser.Id == Code.Common.UserId
                               select new
                               {
                                   p.Id,
                                   p.TeacherName
                               }).FirstOrDefault();
                if (teacher == null)
                {
                    return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                }

                var subjectId = (from p in db.Table<Course.Entity.tbOrg>()
                                 where p.Id == OrgId
                                 select p.tbCourse.tbSubject.Id).FirstOrDefault();

                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && p.tbCourse.tbSubject.Id == subjectId
                                       orderby p.tbCourse.tbSubject.No
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id
                                       }).Distinct().ToList();

                var SubjectList = (from p in examSubjectList
                                   select new System.Web.Mvc.SelectListItem
                                   {
                                       Text = p.SubjectName,
                                       Value = p.SubjectId.ToString()
                                   }).ToList();


                //分数项目
                var OptionList = new List<string>() { "过程分", "考试成绩 ", "考试成绩班名", "考试成绩级名", "综合成绩", "综合成绩班名", "综合成绩级名", "等级" };

                //班级学生
                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where (p.tbStudent.StudentCode.Contains(SearchText) || p.tbStudent.StudentName.Contains(SearchText) || SearchText == null)
                          && p.tbExamCourse.IsDeleted == false
                          && (p.tbExamCourse.tbExam.Id == examId || examId == 0)
                          && p.tbExamCourse.tbCourse.tbSubject.Id == subjectId
                          select new
                          {
                              p.AppraiseMark,
                              p.SegmentMark,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              p.SegmentClassRank,
                              p.SegmentGradeRank,
                              p.tbExamLevel.ExamLevelName,
                              p.tbExamStatus.ExamStatusName,
                              p.tbStudent.StudentCode,
                              p.tbStudent.StudentName,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();
                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == examId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                YearId = p.tbYear.tbYearParent.tbYearParent.Id,
                            }).FirstOrDefault();


                //学生班级
                var orgtype = (from p in db.Table<Course.Entity.tbOrg>()
                               where p.Id == OrgId
                               && (p.tbClass.IsDeleted == false || p.tbClass == null)
                               && p.tbCourse.IsDeleted == false
                               select new
                               {
                                   p.IsClass,
                                   Id = p.tbClass != null ? p.tbClass.Id : 0,
                                   p.tbCourse.CourseName,
                                   p.OrgName
                               }).FirstOrDefault();
                var lst = new List<Dto.ExamReport.List>();
                if (orgtype.IsClass == false)//走读班模式
                {
                    var orgStudent = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                      where p.tbStudent.IsDeleted == false
                                      && p.tbOrg.IsDeleted == false
                                      && p.tbOrg.Id == OrgId
                                      && ((p.tbStudent.StudentCode.Contains(SearchText) || p.tbStudent.StudentName.Contains(SearchText)) || SearchText == null)
                                      select new
                                      {
                                          OrgId = p.tbOrg.Id,
                                          p.tbOrg.OrgName,
                                          StudentId = p.tbStudent.Id,
                                      }).ToList();
                    var tb = (from p in tf
                              join t in orgStudent
                              on new { p.StudentId } equals new { t.StudentId }
                              select new
                              {
                                  StudentId = p.StudentId,
                                  SubjectId = p.SubjectId,
                                  StudentCode = p.StudentCode,
                                  StudentName = p.StudentName,
                                  AppraiseMark = p.AppraiseMark,
                                  SegmentMark = p.SegmentMark,
                                  TotalMark = p.TotalMark,
                                  TotalClassRank = p.TotalClassRank,
                                  TotalGradeRank = p.TotalGradeRank,
                                  SegmentClassRank = p.SegmentClassRank,
                                  SegmentGradeRank = p.SegmentGradeRank,
                                  ExamLevelName = p.ExamLevelName,
                                  OrgName = t.OrgName
                              }).ToList();
                    var ExamMarkList = (from p in tb
                                        select new Dto.ExamReport.List
                                        {
                                            StudentId = p.StudentId,
                                            SubjectId = p.SubjectId,
                                            StudentCode = p.StudentCode,
                                            StudentName = p.StudentName,
                                            AppraiseMark = p.AppraiseMark,
                                            SegmentMark = p.SegmentMark,
                                            TotalMark = p.TotalMark,
                                            TotalClassRank = p.TotalClassRank,
                                            TotalGradeRank = p.TotalGradeRank,
                                            SegmentClassRank = p.SegmentClassRank,
                                            SegmentGradeRank = p.SegmentGradeRank,
                                            ExamLevelName = p.ExamLevelName,
                                            OrgTeacherName = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.tbSysUser.Id == Code.Common.UserId).Select(d => d.TeacherName).FirstOrDefault(),
                                            ClassName = p.OrgName
                                        }).ToList();

                    lst = (from p in ExamMarkList
                           select new Dto.ExamReport.List
                           {
                               StudentId = p.StudentId,
                               StudentCode = p.StudentCode,
                               StudentName = p.StudentName,
                               OrgTeacherName = p.OrgTeacherName,
                               ClassName = p.ClassName
                           }).Distinct().ToList();
                }
                else //行政班模式
                {
                    var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                            where p.tbClass.Id == orgtype.Id
                                            && p.tbStudent.IsDeleted == false
                                            orderby p.No, p.tbStudent.StudentCode
                                            select new
                                            {
                                                p.No,
                                                StudentId = p.tbStudent.Id,
                                                p.tbStudent.StudentCode,
                                                p.tbStudent.StudentName,
                                                CourseName = orgtype.CourseName,
                                                OrgName = orgtype.OrgName
                                            }).ToList();
                    var tb = (from p in tf
                              join t in classStudentList
                              on new { p.StudentId } equals new { t.StudentId }
                              select new
                              {
                                  StudentId = p.StudentId,
                                  SubjectId = p.SubjectId,
                                  StudentCode = p.StudentCode,
                                  StudentName = p.StudentName,
                                  AppraiseMark = p.AppraiseMark,
                                  SegmentMark = p.SegmentMark,
                                  TotalMark = p.TotalMark,
                                  TotalClassRank = p.TotalClassRank,
                                  TotalGradeRank = p.TotalGradeRank,
                                  SegmentClassRank = p.SegmentClassRank,
                                  SegmentGradeRank = p.SegmentGradeRank,
                                  ExamLevelName = p.ExamLevelName,
                                  OrgName = t.OrgName
                              }).ToList();
                    var ExamMarkList = (from p in tb
                                        select new Dto.ExamReport.List
                                        {
                                            StudentId = p.StudentId,
                                            SubjectId = p.SubjectId,
                                            StudentCode = p.StudentCode,
                                            StudentName = p.StudentName,
                                            AppraiseMark = p.AppraiseMark,
                                            SegmentMark = p.SegmentMark,
                                            TotalMark = p.TotalMark,
                                            TotalClassRank = p.TotalClassRank,
                                            TotalGradeRank = p.TotalGradeRank,
                                            SegmentClassRank = p.SegmentClassRank,
                                            SegmentGradeRank = p.SegmentGradeRank,
                                            ExamLevelName = p.ExamLevelName,
                                            OrgTeacherName = db.Table<Teacher.Entity.tbTeacher>().Where(d => d.tbSysUser.Id == Code.Common.UserId).Select(d => d.TeacherName).FirstOrDefault(),
                                            ClassName = p.OrgName
                                        }).ToList();

                    lst = (from p in ExamMarkList
                           select new Dto.ExamReport.List
                           {
                               StudentId = p.StudentId,
                               StudentCode = p.StudentCode,
                               StudentName = p.StudentName,
                               OrgTeacherName = p.OrgTeacherName,
                               ClassName = p.ClassName
                           }).Distinct().ToList();

                }

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("任课老师"),
                    });
                foreach (var subject in SubjectList)
                {
                    for (var i = 0; i < OptionList.Count; i++)
                    {
                        dt.Columns.AddRange(new System.Data.DataColumn[]
                        {
                            new System.Data.DataColumn(subject.Text+OptionList[i])
                        });
                    }
                }
                foreach (var a in lst)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["班级"] = a.ClassName;
                    dr["任课老师"] = a.OrgTeacherName;
                    foreach (var subject in SubjectList)
                    {
                        var mark = lst.Where(d => d.SubjectId == subject.Value.ConvertToInt() && d.StudentId == a.StudentId).Select(d => d).FirstOrDefault();
                        if (mark != null)
                        {
                            for (var i = 0; i < OptionList.Count; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        dr[subject.Text + OptionList[i]] = a.AppraiseMark;
                                        break;
                                    case 1:
                                        dr[subject.Text + OptionList[i]] = a.TotalMark;
                                        break;
                                    case 2:
                                        dr[subject.Text + OptionList[i]] = a.TotalClassRank;
                                        break;
                                    case 3:
                                        dr[subject.Text + OptionList[i]] = a.TotalGradeRank;
                                        break;
                                    case 4:
                                        dr[subject.Text + OptionList[i]] = a.SegmentMark;
                                        break;
                                    case 5:
                                        dr[subject.Text + OptionList[i]] = a.SegmentClassRank;
                                        break;
                                    case 6:
                                        dr[subject.Text + OptionList[i]] = a.SegmentGradeRank;
                                        break;
                                    case 7:
                                        dr[subject.Text + OptionList[i]] = a.ExamLevelName;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            for (var i = 0; i < OptionList.Count; i++)
                            {
                                dr[subject.Text + OptionList[i]] = string.Empty;
                            }
                        }
                    }
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);

                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return View();
                }
            }
        }

        public ActionResult ExportStudent(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where p.tbSysUser.Id == Code.Common.UserId
                               select new
                               {
                                   p.Id,
                                   p.StudentCode,
                                   p.StudentName
                               }).FirstOrDefault();
                if (student == null)
                {
                    return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                }
                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == examId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   select new System.Web.Mvc.SelectListItem
                                   {
                                       Text = p.tbCourse.tbSubject.SubjectName,
                                       Value = p.tbCourse.tbSubject.Id.ToString()
                                   }).Distinct().ToList();

                //分数项目
                var OptionList = new List<string>() { "过程分", "考试成绩 ", "考试成绩班名", "考试成绩级名", "综合成绩", "综合成绩班名", "综合成绩级名", "等级" };

                //班级学生
                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbStudent.Id == student.Id
                          && p.tbExamCourse.IsDeleted == false
                          && (p.tbExamCourse.tbExam.Id == examId || examId == 0)
                          select new
                          {
                              p.AppraiseMark,
                              p.SegmentMark,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              p.SegmentClassRank,
                              p.SegmentGradeRank,
                              p.tbExamLevel.ExamLevelName,
                              p.tbExamStatus.ExamStatusName,
                              p.tbStudent.StudentCode,
                              p.tbStudent.StudentName,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();
                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == examId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();

                //学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && p.tbStudent.Id == student.Id
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              AppraiseMark = p.AppraiseMark,
                              SegmentMark = p.SegmentMark,
                              TotalMark = p.TotalMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              SegmentClassRank = p.SegmentClassRank,
                              SegmentGradeRank = p.SegmentGradeRank,
                              ExamLevelName = p.ExamLevelName,
                              ClassName = t.ClassName
                          }).ToList();

                var tb = (from p in tg
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              AppraiseMark = p.AppraiseMark,
                              SegmentMark = p.SegmentMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              SegmentClassRank = p.SegmentClassRank,
                              SegmentGradeRank = p.SegmentGradeRank,
                              TotalMark = p.TotalMark,
                              ExamLevelName = p.ExamLevelName,
                              ClassName = p.ClassName
                          }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("班级"),
                    });
                foreach (var subject in SubjectList)
                {
                    for (var i = 0; i < OptionList.Count; i++)
                    {
                        dt.Columns.AddRange(new System.Data.DataColumn[]
                        {
                            new System.Data.DataColumn(subject.Text+OptionList[i])
                        });
                    }
                }
                foreach (var a in tb)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["班级"] = a.ClassName;
                    foreach (var subject in SubjectList)
                    {
                        var mark = tb.Where(d => d.SubjectId == subject.Value.ConvertToInt() && d.StudentId == a.StudentId).Select(d => d).FirstOrDefault();
                        if (mark != null)
                        {
                            for (var i = 0; i < OptionList.Count; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        dr[subject.Text + OptionList[i]] = a.AppraiseMark;
                                        break;
                                    case 1:
                                        dr[subject.Text + OptionList[i]] = a.TotalMark;
                                        break;
                                    case 2:
                                        dr[subject.Text + OptionList[i]] = a.TotalClassRank;
                                        break;
                                    case 3:
                                        dr[subject.Text + OptionList[i]] = a.TotalGradeRank;
                                        break;
                                    case 4:
                                        dr[subject.Text + OptionList[i]] = a.SegmentMark;
                                        break;
                                    case 5:
                                        dr[subject.Text + OptionList[i]] = a.SegmentClassRank;
                                        break;
                                    case 6:
                                        dr[subject.Text + OptionList[i]] = a.SegmentGradeRank;
                                        break;
                                    case 7:
                                        dr[subject.Text + OptionList[i]] = a.ExamLevelName;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            for (var i = 0; i < OptionList.Count; i++)
                            {
                                dr[subject.Text + OptionList[i]] = string.Empty;
                            }
                        }
                    }
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);

                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return Content("<script>alert('导出有错!');</script>");
                }
            }
        }
        #endregion

        public ActionResult Report()
        {
            return View();
        }

        public ActionResult ReportOrgTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.ReportOrgTeacher();
                vm.ExamList = Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                var exam = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            select new
                            {
                                YearId = p.tbYear.Id,
                            }).FirstOrDefault();
                if (exam != null)
                {
                    vm.OrgList = Course.Controllers.OrgTeacherController.GetOrgListByOrgTeacher();
                    if (vm.OrgId == 0 && vm.OrgList.Count > 0)
                    {
                        vm.OrgId = vm.OrgList.FirstOrDefault().Value.ConvertToInt();
                    }

                    var org = (from p in db.Table<Course.Entity.tbOrg>()
                               where p.Id == vm.OrgId
                               select new
                               {
                                   OrgId = p.Id,
                                   p.OrgName,
                                   CourseId = p.tbCourse.Id
                               }).FirstOrDefault();
                    if (org != null)
                    {
                        vm.ExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                           join q in Course.Controllers.OrgStudentController.GetOrgStudent(db, org.OrgId) on p.tbStudent.Id equals q.Id
                                           where p.tbExamCourse.tbCourse.Id == org.CourseId
                                             && p.tbExamCourse.tbExam.Id == vm.ExamId
                                             && p.tbExamCourse.IsDeleted == false
                                           orderby p.tbStudent.StudentCode
                                           select new Dto.ExamMark.List
                                           {
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               ExamStatusName = p.tbExamStatus.ExamStatusName,
                                               AppraiseMark = p.AppraiseMark,
                                               TotalMark = p.TotalMark,
                                               TotalGradeRank = p.TotalGradeRank,
                                               TotalClassRank = p.TotalClassRank,
                                               SegmentMark = p.SegmentMark,
                                               SegmentGradeRank = p.SegmentGradeRank,
                                               SegmentClassRank = p.SegmentClassRank,
                                               ExamLevelName = p.tbExamLevel.ExamLevelName,
                                               ClassName = org.OrgName
                                           }).ToList();
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportOrgTeacher(Models.ExamReport.ReportOrgTeacher vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ReportOrgTeacher", new { examId = vm.ExamId, orgId = vm.OrgId }));
        }

        public ActionResult ReportSubjectTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.ReportSubjectTeacher();
                vm.ExamList = Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.SubjectList = Teacher.Controllers.TeacherSubjectController.GetSubjectByTeacher();
                if (vm.SubjectId == 0 && vm.SubjectList.Count > 0)
                {
                    vm.SubjectId = vm.SubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.GradeList = Teacher.Controllers.TeacherSubjectController.GetSubjectGradeByTeacher(vm.SubjectId ?? 0);
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.SubjectId != 0)
                {
                    vm.CourseList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                     where p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SubjectId
                                       && p.tbExam.Id == vm.ExamId
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Text = p.tbCourse.CourseName,
                                         Value = p.tbCourse.Id.ToString()
                                     }).ToList();

                    var exam = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == vm.ExamId
                                select new
                                {
                                    YearId = p.tbYear.Id,
                                }).FirstOrDefault();
                    if (exam != null)
                    {
                        var examMark = from p in db.Table<Exam.Entity.tbExamMark>()
                                       where p.tbStudent.IsDeleted == false
                                        && p.tbExamCourse.IsDeleted == false
                                        && p.tbExamCourse.tbExam.Id == vm.ExamId
                                        && p.tbExamCourse.tbCourse.IsDeleted == false
                                        && p.tbExamCourse.tbCourse.tbSubject.Id == vm.SubjectId
                                       select p;
                        if (vm.CourseId != 0)
                        {
                            examMark = examMark.Where(d => d.tbExamCourse.tbCourse.Id == vm.CourseId);
                        }

                        vm.OrgList = Course.Controllers.OrgTeacherController.GetOrgGradeListByCourse(exam.YearId, vm.GradeId, vm.SubjectId ?? 0, vm.CourseId ?? 0);

                        if (vm.OrgId != 0)
                        {
                            var org = (from p in db.Table<Course.Entity.tbOrg>()
                                       where p.Id == vm.OrgId
                                       select new
                                       {
                                           OrgId = p.Id,
                                           CourseId = p.tbCourse.Id
                                       }).FirstOrDefault();
                            if (org != null)
                            {
                                examMark = from p in examMark
                                           join q in Course.Controllers.OrgStudentController.GetOrgGradeStudent(db, org.OrgId, vm.GradeId) on p.tbStudent.Id equals q.Id
                                           where p.tbExamCourse.tbCourse.Id == org.CourseId
                                             && p.tbExamCourse.tbExam.Id == vm.ExamId
                                             && p.tbExamCourse.IsDeleted == false
                                           orderby p.tbStudent.StudentCode
                                           select p;
                            }
                        }

                        vm.ExamMarkList = (from p in examMark
                                           where (p.tbExamCourse.tbCourse.Id == vm.CourseId || vm.CourseId == 0)
                                             && (p.tbExamCourse.tbCourse.tbSubject.Id == vm.SubjectId || vm.SubjectId == 0)
                                             && p.tbExamCourse.tbExam.Id == vm.ExamId
                                             && p.tbExamCourse.IsDeleted == false
                                           orderby p.tbStudent.StudentCode
                                           select new Dto.ExamMark.List
                                           {
                                               StudentCode = p.tbStudent.StudentCode,
                                               StudentName = p.tbStudent.StudentName,
                                               ExamStatusName = p.tbExamStatus.ExamStatusName,
                                               AppraiseMark = p.AppraiseMark,
                                               TotalMark = p.TotalMark,
                                               TotalGradeRank = p.TotalGradeRank,
                                               TotalClassRank = p.TotalClassRank,
                                               SegmentMark = p.SegmentMark,
                                               SegmentGradeRank = p.SegmentGradeRank,
                                               SegmentClassRank = p.SegmentClassRank,
                                               ExamLevelName = p.tbExamLevel.ExamLevelName
                                           }).ToList();
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportSubjectTeacher(Models.ExamReport.ReportSubjectTeacher vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ReportSubjectTeacher", new { examId = vm.ExamId, subjectId = vm.SubjectId, courseId = vm.CourseId, orgId = vm.OrgId }));
        }

        public ActionResult ReportClassTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.ReportClassTeacher();
                vm.ExamList = Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.ClassList = Basis.Controllers.ClassTeacherController.GetClassByClassTeacher();
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                var examMark = from p in db.Table<Exam.Entity.tbExamMark>()
                               join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals q.tbStudent.Id
                               where p.tbExamCourse.tbExam.Id == vm.ExamId
                                   && p.tbExamCourse.IsDeleted == false
                                   && p.tbStudent.IsDeleted == false
                                   && q.tbClass.Id == vm.ClassId
                               select new { p, q.tbClass.ClassName };

                vm.CourseList = (from p in examMark
                                 group p by new { p.p.tbExamCourse.tbCourse.CourseName, p.p.tbExamCourse.tbCourse.Id, p.p.tbExamCourse.No, SubjectNo = p.p.tbExamCourse.tbCourse.tbSubject.No } into g
                                 orderby g.Key.SubjectNo, g.Key.No
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Text = g.Key.CourseName,
                                     Value = g.Key.Id.ToString()
                                 }).ToList();
                if (vm.CourseId == 0 && vm.CourseList.Count > 0)
                {
                    vm.CourseId = vm.CourseList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.ExamMarkList = (from p in examMark
                                   where p.p.tbExamCourse.tbCourse.Id == vm.CourseId
                                     && p.p.tbExamCourse.tbExam.Id == vm.ExamId
                                     && p.p.tbExamCourse.IsDeleted == false
                                   orderby p.p.tbStudent.StudentCode
                                   select new Dto.ExamMark.List
                                   {
                                       StudentCode = p.p.tbStudent.StudentCode,
                                       StudentName = p.p.tbStudent.StudentName,
                                       ExamStatusName = p.p.tbExamStatus.ExamStatusName,
                                       AppraiseMark = p.p.AppraiseMark,
                                       TotalMark = p.p.TotalMark,
                                       TotalGradeRank = p.p.TotalGradeRank,
                                       TotalClassRank = p.p.TotalClassRank,
                                       SegmentMark = p.p.SegmentMark,
                                       SegmentGradeRank = p.p.SegmentGradeRank,
                                       SegmentClassRank = p.p.SegmentClassRank,
                                       ExamLevelName = p.p.tbExamLevel.ExamLevelName,
                                       ClassName = p.ClassName
                                   }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportClassTeacher(Models.ExamReport.ReportClassTeacher vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ReportClassTeacher", new { examId = vm.ExamId, classId = vm.ClassId, courseId = vm.CourseId }));
        }

        public ActionResult ReportGradeTeacher()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.ReportGradeTeacher();
                vm.ExamList = Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.GradeList = Teacher.Controllers.TeacherGradeController.GetGradeByTeacher();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.ClassList = Basis.Controllers.ClassController.GetClassList(0, vm.GradeId);
                if (vm.ClassId != -1)
                {
                    if (vm.ClassList.Where(d => d.Value == vm.ClassId.ToString()).Any() == false)
                    {
                        vm.ClassId = 0;
                    }
                    if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                    {
                        vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                    }
                }

                var examMark = from p in db.Table<Exam.Entity.tbExamMark>()
                               join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals q.tbStudent.Id
                               where p.tbExamCourse.tbExam.Id == vm.ExamId
                                   && p.tbExamCourse.IsDeleted == false
                                   && p.tbStudent.IsDeleted == false
                                   && q.tbClass.IsDeleted == false
                                   && q.tbClass.tbGrade.Id == vm.GradeId
                                   && (q.tbClass.Id == vm.ClassId || vm.ClassId == -1)
                               select p;

                vm.CourseList = (from p in examMark
                                 group p by new { p.tbExamCourse.tbCourse.CourseName, p.tbExamCourse.tbCourse.Id, p.tbExamCourse.No, SubjectNo = p.tbExamCourse.tbCourse.tbSubject.No } into g
                                 orderby g.Key.SubjectNo, g.Key.No
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Text = g.Key.CourseName,
                                     Value = g.Key.Id.ToString()
                                 }).ToList();
                if (vm.CourseId == 0 && vm.CourseList.Count > 0)
                {
                    vm.CourseId = vm.CourseList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.ExamMarkList = (from p in examMark
                                   where p.tbExamCourse.tbCourse.Id == vm.CourseId
                                     && p.tbExamCourse.tbExam.Id == vm.ExamId
                                     && p.tbExamCourse.IsDeleted == false
                                   orderby p.tbStudent.StudentCode
                                   select new Dto.ExamMark.List
                                   {
                                       StudentCode = p.tbStudent.StudentCode,
                                       StudentName = p.tbStudent.StudentName,
                                       ExamStatusName = p.tbExamStatus.ExamStatusName,
                                       AppraiseMark = p.AppraiseMark,
                                       TotalMark = p.TotalMark,
                                       TotalGradeRank = p.TotalGradeRank,
                                       TotalClassRank = p.TotalClassRank,
                                       SegmentMark = p.SegmentMark,
                                       SegmentGradeRank = p.SegmentGradeRank,
                                       SegmentClassRank = p.SegmentClassRank,
                                       ExamLevelName = p.tbExamLevel.ExamLevelName
                                   }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportGradeTeacher(Models.ExamReport.ReportGradeTeacher vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ReportGradeTeacher", new { examId = vm.ExamId, gradeId = vm.GradeId, classId = vm.ClassId ?? -1, courseId = vm.CourseId }));
        }

        public ActionResult ExportReportClassTeacher(int examId, int classId, int courseId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var examMark = from p in db.Table<Exam.Entity.tbExamMark>()
                               join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals q.tbStudent.Id
                               where p.tbExamCourse.tbExam.Id == examId
                                   && p.tbExamCourse.IsDeleted == false
                                   && p.tbStudent.IsDeleted == false
                                   && q.tbClass.Id == classId
                               select new { p, q.tbClass.ClassName };

                var tb = (from p in examMark
                          where p.p.tbExamCourse.tbCourse.Id == courseId
                            && p.p.tbExamCourse.tbExam.Id == examId
                            && p.p.tbExamCourse.IsDeleted == false
                          orderby p.p.tbStudent.StudentCode
                          select new Dto.ExamMark.List
                          {
                              StudentCode = p.p.tbStudent.StudentCode,
                              StudentName = p.p.tbStudent.StudentName,
                              ExamStatusName = p.p.tbExamStatus.ExamStatusName,
                              AppraiseMark = p.p.AppraiseMark,
                              TotalMark = p.p.TotalMark,
                              TotalGradeRank = p.p.TotalGradeRank,
                              TotalClassRank = p.p.TotalClassRank,
                              SegmentMark = p.p.SegmentMark,
                              SegmentGradeRank = p.p.SegmentGradeRank,
                              SegmentClassRank = p.p.SegmentClassRank,
                              ExamLevelName = p.p.tbExamLevel.ExamLevelName,
                              ClassName = p.ClassName
                          }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("过程分"),
                        new System.Data.DataColumn("考试成绩"),
                        new System.Data.DataColumn("考试成绩级名"),
                        new System.Data.DataColumn("综合成绩"),
                        new System.Data.DataColumn("综合成绩级名"),
                        new System.Data.DataColumn("等级")
                    });

                foreach (var a in tb)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["班级"] = a.ClassName;
                    dr["过程分"] = a.AppraiseMark;
                    dr["考试成绩"] = a.TotalMark;
                    dr["考试成绩级名"] = a.TotalGradeRank;
                    dr["综合成绩"] = a.SegmentMark;
                    dr["综合成绩级名"] = a.SegmentGradeRank;
                    dr["等级"] = a.ExamLevelName;
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);
                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return Content("<script>alert('导出有错!');</script>");
                }
            }
        }

        public ActionResult ExportReportOrgTeacher(int examId, int orgId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var course = (from p in db.Table<Course.Entity.tbOrg>()
                              where p.Id == orgId
                              select new
                              {
                                  p.tbCourse.Id,
                                  p.OrgName
                              }).FirstOrDefault();

                var tb = (from p in db.Table<Exam.Entity.tbExamMark>()
                          join q in Course.Controllers.OrgStudentController.GetOrgStudent(db, orgId) on p.tbStudent.Id equals q.Id
                          where p.tbExamCourse.tbCourse.Id == course.Id
                            && p.tbExamCourse.tbExam.Id == examId
                            && p.tbExamCourse.IsDeleted == false
                          orderby p.tbStudent.StudentCode
                          select new Dto.ExamMark.List
                          {
                              StudentCode = p.tbStudent.StudentCode,
                              StudentName = p.tbStudent.StudentName,
                              ExamStatusName = p.tbExamStatus.ExamStatusName,
                              AppraiseMark = p.AppraiseMark,
                              TotalMark = p.TotalMark,
                              TotalGradeRank = p.TotalGradeRank,
                              TotalClassRank = p.TotalClassRank,
                              SegmentMark = p.SegmentMark,
                              SegmentGradeRank = p.SegmentGradeRank,
                              SegmentClassRank = p.SegmentClassRank,
                              ExamLevelName = p.tbExamLevel.ExamLevelName,
                              ClassName = course.OrgName
                          }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("过程分"),
                        new System.Data.DataColumn("考试成绩"),
                        new System.Data.DataColumn("考试成绩级名"),
                        new System.Data.DataColumn("综合成绩"),
                        new System.Data.DataColumn("综合成绩级名"),
                        new System.Data.DataColumn("等级")
                    });

                foreach (var a in tb)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["班级"] = a.ClassName;
                    dr["过程分"] = a.AppraiseMark;
                    dr["考试成绩"] = a.TotalMark;
                    dr["考试成绩级名"] = a.TotalGradeRank;
                    dr["综合成绩"] = a.SegmentMark;
                    dr["综合成绩级名"] = a.SegmentGradeRank;
                    dr["等级"] = a.ExamLevelName;
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);
                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return Content("<script>alert('导出有错!');</script>");
                }
            }
        }

        public ActionResult ExportReportSubjectTeacher(int examId, int orgId, int subjectId, int courseId, int gradeId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var examMark = from p in db.Table<Exam.Entity.tbExamMark>()
                               where p.tbStudent.IsDeleted == false
                                && p.tbExamCourse.IsDeleted == false
                                && p.tbExamCourse.tbExam.Id == examId
                                && p.tbExamCourse.tbCourse.IsDeleted == false
                                && p.tbExamCourse.tbCourse.tbSubject.Id == subjectId
                               select p;
                if (courseId != 0)
                {
                    examMark = examMark.Where(d => d.tbExamCourse.tbCourse.Id == courseId);
                }

                if (orgId != 0)
                {
                    var org = (from p in db.Table<Course.Entity.tbOrg>()
                               where p.Id == orgId
                               select new
                               {
                                   OrgId = p.Id,
                                   CourseId = p.tbCourse.Id
                               }).FirstOrDefault();
                    if (org != null)
                    {
                        examMark = from p in examMark
                                   join q in Course.Controllers.OrgStudentController.GetOrgGradeStudent(db, org.OrgId, gradeId) on p.tbStudent.Id equals q.Id
                                   where p.tbExamCourse.tbCourse.Id == org.CourseId
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && p.tbExamCourse.IsDeleted == false
                                   orderby p.tbStudent.StudentCode
                                   select p;
                    }
                }

                var tb = (from p in examMark
                          where (p.tbExamCourse.tbCourse.Id == courseId || courseId == 0)
                            && (p.tbExamCourse.tbCourse.tbSubject.Id == subjectId || subjectId == 0)
                            && p.tbExamCourse.tbExam.Id == examId
                            && p.tbExamCourse.IsDeleted == false
                          orderby p.tbStudent.StudentCode
                          select new Dto.ExamMark.List
                          {
                              StudentCode = p.tbStudent.StudentCode,
                              StudentName = p.tbStudent.StudentName,
                              ExamStatusName = p.tbExamStatus.ExamStatusName,
                              AppraiseMark = p.AppraiseMark,
                              TotalMark = p.TotalMark,
                              TotalGradeRank = p.TotalGradeRank,
                              TotalClassRank = p.TotalClassRank,
                              SegmentMark = p.SegmentMark,
                              SegmentGradeRank = p.SegmentGradeRank,
                              SegmentClassRank = p.SegmentClassRank,
                              ExamLevelName = p.tbExamLevel.ExamLevelName
                          }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("考试成绩"),
                        new System.Data.DataColumn("过程分"),
                        new System.Data.DataColumn("考试成绩级名"),
                        new System.Data.DataColumn("综合成绩"),
                        new System.Data.DataColumn("综合成绩级名"),
                        new System.Data.DataColumn("等级")
                    });

                foreach (var a in tb)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["班级"] = a.ClassName;
                    dr["过程分"] = a.AppraiseMark;
                    dr["考试成绩"] = a.TotalMark;
                    dr["考试成绩级名"] = a.TotalGradeRank;
                    dr["综合成绩"] = a.SegmentMark;
                    dr["综合成绩级名"] = a.SegmentGradeRank;
                    dr["等级"] = a.ExamLevelName;
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);
                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return Content("<script>alert('导出有错!');</script>");
                }
            }
        }

        public ActionResult ExportReportGradeTeacher(int examId, int courseId, int gradeId, int classId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var examMark = from p in db.Table<Exam.Entity.tbExamMark>()
                               join q in db.Table<Basis.Entity.tbClassStudent>() on p.tbStudent.Id equals q.tbStudent.Id
                               where p.tbExamCourse.tbExam.Id == examId
                                   && p.tbExamCourse.IsDeleted == false
                                   && p.tbStudent.IsDeleted == false
                                   && p.tbExamCourse.tbCourse.Id == courseId
                                   && q.tbClass.IsDeleted == false
                                   && q.tbClass.tbGrade.Id == gradeId
                                   && (q.tbClass.Id == classId || classId == -1)
                               select new { p, q.tbClass.ClassName };

                var tb = (from p in examMark
                          orderby p.p.tbStudent.StudentCode
                          select new Dto.ExamMark.List
                          {
                              StudentCode = p.p.tbStudent.StudentCode,
                              StudentName = p.p.tbStudent.StudentName,
                              ExamStatusName = p.p.tbExamStatus.ExamStatusName,
                              AppraiseMark = p.p.AppraiseMark,
                              TotalMark = p.p.TotalMark,
                              TotalGradeRank = p.p.TotalGradeRank,
                              TotalClassRank = p.p.TotalClassRank,
                              SegmentMark = p.p.SegmentMark,
                              SegmentGradeRank = p.p.SegmentGradeRank,
                              SegmentClassRank = p.p.SegmentClassRank,
                              ExamLevelName = p.p.tbExamLevel.ExamLevelName,
                              ClassName = p.ClassName
                          }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("过程分"),
                        new System.Data.DataColumn("考试成绩"),
                        new System.Data.DataColumn("考试成绩级名"),
                        new System.Data.DataColumn("综合成绩"),
                        new System.Data.DataColumn("综合成绩级名"),
                        new System.Data.DataColumn("等级")
                    });

                foreach (var a in tb)
                {
                    var dr = dt.NewRow();
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["班级"] = a.ClassName;
                    dr["过程分"] = a.AppraiseMark;
                    dr["考试成绩"] = a.TotalMark;
                    dr["考试成绩级名"] = a.TotalGradeRank;
                    dr["综合成绩"] = a.SegmentMark;
                    dr["综合成绩级名"] = a.SegmentGradeRank;
                    dr["等级"] = a.ExamLevelName;
                    dt.Rows.Add(dr);
                }

                Code.NpoiHelper.DataTableToExcel(file, dt);
                if (string.IsNullOrEmpty(file) == false)
                {
                    return File(file, Code.Common.DownloadType, Code.Common.ExportByExcel);
                }
                else
                {
                    return Content("<script>alert('导出有错!');</script>");
                }
            }
        }

        #region 学生学分
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentPointList(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentPointList", new { searchText = vm.SearchText }));
        }

        public ActionResult StudentPointList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();
                //表头项
                vm.OptionList = new List<string>() { "必修", "选修" };
                var student = (from p in db.Table<Student.Entity.tbStudent>()
                               where (p.StudentCode == vm.SearchText || p.StudentName == vm.SearchText || (p.tbSysUser.Id == Code.Common.UserId && Code.Common.UserType == Code.EnumHelper.SysUserType.Student))
                               select new
                               {
                                   p.Id,
                                   p.StudentCode,
                                   p.StudentName
                               }).FirstOrDefault();
                vm.GradeOptionList = new List<string>() {student!=null?student.StudentCode:string.Empty,
                                                         student != null ? student.StudentName : string.Empty
                                                         };
                if (student == null) return View(vm);

                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   orderby p.No, p.SubjectName
                                   select new
                                   {
                                       SubjectId = p.Id,
                                       p.SubjectName,
                                       p.RequirePoint,
                                       p.ElectivePoint
                                   }).ToList();

                vm.SubjectList = (from p in subjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbStudent.Id == student.Id
                          && p.tbExamCourse.IsDeleted == false
                          && p.tbExamCourse.tbCourse.IsDeleted == false
                          && p.tbExamCourse.tbCourse.tbCourseType.IsDeleted == false
                          && p.tbExamCourse.tbExam.IsDeleted == false
                          select new
                          {
                              Point = p.tbExamLevel.ExamLevelValue > decimal.Zero ? p.tbExamCourse.tbCourse.Point : decimal.Zero,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.tbCourse.tbCourseType.CourseTypeName,
                              ExamId = p.tbExamCourse.tbExam.Id
                          }).ToList();

                var EmTotalPointList = (from p in tf
                                        where "必修".Contains(p.CourseTypeName)
                                        group p by new
                                        {
                                            p.SubjectId
                                        } into g
                                        select new
                                        {
                                            g.Key.SubjectId,
                                            TotalPoint = g.Sum(d => d.Point)
                                        }).ToList();

                var XmTotalPointList = (from p in tf
                                        where "选修".Contains(p.CourseTypeName)
                                        group p by new
                                        {
                                            p.SubjectId
                                        } into g
                                        select new
                                        {
                                            g.Key.SubjectId,
                                            TotalPoint = g.Sum(d => d.Point)
                                        }).ToList();

                vm.ExamMarkList = (from p in subjectList
                                   select new Dto.ExamReport.List
                                   {
                                       SubjectId = p.SubjectId,
                                       SubjectName = p.SubjectName,
                                       RequirePoint = p.RequirePoint,
                                       ElectivePoint = p.ElectivePoint,
                                       EmPoint = EmTotalPointList.Where(c => c.SubjectId == p.SubjectId).Select(c => c.TotalPoint).FirstOrDefault(),
                                       XmPoint = XmTotalPointList.Where(c => c.SubjectId == p.SubjectId).Select(c => c.TotalPoint).FirstOrDefault()
                                   }).ToList();

                return View(vm);
            }
        }

        public ActionResult ClassStudentPointList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();
                vm.OptionList = new List<string>() { "必修", "选修" };

                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Year);
                if (vm.YearId == 0 && vm.YearList.Count > 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }

                vm.GradeList = Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.ClassList = Basis.Controllers.ClassController.SelectList(vm.YearId ?? 0, vm.GradeId);

                var tb = db.Table<Basis.Entity.tbClassStudent>();

                if (vm.GradeId > 0)
                {
                    tb = tb.Where(d => d.tbClass.tbGrade.Id == vm.GradeId);
                }

                if (vm.YearId > 0)
                {
                    tb = tb.Where(d => d.tbClass.tbYear.Id == vm.YearId);
                }

                if (vm.ClassId != 0)
                {
                    tb = tb.Where(d => d.tbClass.Id == vm.ClassId);
                }

                if (!string.IsNullOrEmpty(vm.SearchText))
                {
                    tb = tb.Where(d => d.tbStudent.StudentCode.Contains(vm.SearchText) || d.tbStudent.StudentName.Contains(vm.SearchText));
                }

                var student = (from p in tb
                               orderby p.tbStudent.StudentCode
                               select new
                               {
                                   p.tbStudent.Id,
                                   p.tbStudent.StudentCode,
                                   p.tbStudent.StudentName
                               }).Distinct().ToList();

                vm.ClassStudentList = (from p in student
                                       select new Dto.ExamReport.List
                                       {
                                           StudentId = p.Id,
                                           StudentCode = p.StudentCode,
                                           StudentName = p.StudentName
                                       }).ToList();
                var StudentIds = student.Select(d => d.Id).Distinct().ToList();

                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   orderby p.No, p.SubjectName
                                   select new
                                   {
                                       SubjectId = p.Id,
                                       p.SubjectName,
                                       p.RequirePoint,
                                       p.ElectivePoint
                                   }).ToList();

                vm.SubjectList = (from p in subjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                vm.SubjectList.Add(new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where StudentIds.Contains(p.tbStudent.Id)
                          && p.tbExamCourse.IsDeleted == false
                          && p.tbExamCourse.tbCourse.IsDeleted == false
                          && p.tbExamCourse.tbCourse.tbCourseType.IsDeleted == false
                          && p.tbExamCourse.tbExam.IsDeleted == false
                          select new
                          {
                              Point = p.tbExamLevel.ExamLevelValue > decimal.Zero ? p.tbExamCourse.tbCourse.Point : decimal.Zero,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.tbCourse.tbCourseType.CourseTypeName,
                              StudentId = p.tbStudent.Id,
                              ExamId = p.tbExamCourse.tbExam.Id
                          }).ToList();

                var EmTotalPointList = (from p in tf
                                        where "必修".Contains(p.CourseTypeName)
                                        group p by new
                                        {
                                            p.SubjectId,
                                            p.StudentId
                                        } into g
                                        select new
                                        {
                                            g.Key.SubjectId,
                                            g.Key.StudentId,
                                            TotalPoint = g.Sum(d => d.Point)
                                        }).ToList();

                var XmTotalPointList = (from p in tf
                                        where "选修".Contains(p.CourseTypeName)
                                        group p by new
                                        {
                                            p.SubjectId,
                                            p.StudentId
                                        } into g
                                        select new
                                        {
                                            g.Key.SubjectId,
                                            g.Key.StudentId,
                                            TotalPoint = g.Sum(d => d.Point)
                                        }).ToList();

                var tg = (from p in tf
                          select new Dto.ExamReport.List
                          {
                              SubjectId = p.SubjectId,
                              StudentId = p.StudentId,
                              EmPoint = EmTotalPointList.Where(c => c.SubjectId == p.SubjectId && c.StudentId == p.StudentId).Select(c => c.TotalPoint).FirstOrDefault(),
                              XmPoint = XmTotalPointList.Where(c => c.SubjectId == p.SubjectId && c.StudentId == p.StudentId).Select(c => c.TotalPoint).FirstOrDefault()
                          }).ToList();

                var TotalPointList = (from p in tg
                                      group p by new
                                      {
                                          p.StudentId
                                      } into g
                                      select new
                                      {
                                          g.Key.StudentId,
                                          TotalEmPoint = g.Sum(d => d.EmPoint),
                                          TotalXmPoint = g.Sum(d => d.XmPoint)
                                      }).ToList();
                vm.ExamMarkList = (from p in tg
                                   select new Dto.ExamReport.List
                                   {
                                       SubjectId = p.SubjectId,
                                       StudentId = p.StudentId,
                                       EmPoint = p.EmPoint,
                                       XmPoint = p.XmPoint,
                                       TotalEmPoint = TotalPointList.Where(d => d.StudentId == p.StudentId).Select(d => d.TotalEmPoint).FirstOrDefault(),
                                       TotalXmPoint = TotalPointList.Where(d => d.StudentId == p.StudentId).Select(d => d.TotalXmPoint).FirstOrDefault(),
                                   }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassStudentPointList(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassStudentPointList", new { searchText = vm.SearchText, yearId = vm.YearId, gradeId = vm.GradeId, classId = vm.ClassId }));
        }
        #endregion

        #region 北大
        public ActionResult TermTotalMarkReportBD()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();
                vm.ExamList = Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == vm.ExamId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                       orderby p.tbCourse.tbSubject.No
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id
                                       }).Distinct().ToList();

                vm.SubjectList = (from p in examSubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                if (vm.SubjectId == 0 && vm.SubjectList.Count > 0)
                {
                    vm.SubjectId = vm.SubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.SubjectId != 0)
                {
                    vm.CourseList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                     where p.tbCourse.IsDeleted == false
                                       && p.tbCourse.tbSubject.Id == vm.SubjectId
                                       && p.tbExam.Id == vm.ExamId
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Text = p.tbCourse.CourseName,
                                         Value = p.tbCourse.Id.ToString()
                                     }).ToList();

                    var exam = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == vm.ExamId
                                select new
                                {
                                    YearId = p.tbYear.Id,
                                }).FirstOrDefault();
                    if (exam != null)
                    {
                        var AppraiseMark = from p in db.Table<Exam.Entity.tbExamAppraiseMark>()
                                           where p.tbStudent.IsDeleted == false
                                           && p.tbCourse.IsDeleted == false
                                           && p.tbYear.IsDeleted == false
                                           select p;

                        var examMark = from p in db.Table<Exam.Entity.tbExamMark>()
                                       where p.tbStudent.IsDeleted == false
                                        && p.tbExamCourse.IsDeleted == false
                                        && p.tbExamCourse.tbExam.Id == vm.ExamId
                                        && p.tbExamCourse.tbCourse.IsDeleted == false
                                        && p.tbExamCourse.tbCourse.tbSubject.Id == vm.SubjectId
                                       select p;
                        if (vm.CourseId != 0)
                        {
                            examMark = examMark.Where(d => d.tbExamCourse.tbCourse.Id == vm.CourseId);
                        }

                        vm.OrgList = Course.Controllers.OrgTeacherController.GetOrgListBySubjectCourse(exam.YearId, vm.SubjectId ?? 0, vm.CourseId ?? 0);

                        if (vm.OrgId != 0)
                        {
                            var org = (from p in db.Table<Course.Entity.tbOrg>()
                                       where p.Id == vm.OrgId
                                       select new
                                       {
                                           OrgId = p.Id,
                                           CourseId = p.tbCourse.Id
                                       }).FirstOrDefault();
                            if (org != null)
                            {
                                examMark = from p in examMark
                                           join q in Course.Controllers.OrgStudentController.GetOrgGradeStudent(db, org.OrgId) on p.tbStudent.Id equals q.Id
                                           where p.tbExamCourse.tbCourse.Id == org.CourseId
                                             && p.tbExamCourse.tbExam.Id == vm.ExamId
                                             && p.tbExamCourse.IsDeleted == false
                                           orderby p.tbStudent.StudentCode
                                           select p;

                                var tv = (from p in examMark
                                          join o in AppraiseMark
                                          on p.tbStudent.Id equals o.tbStudent.Id
                                          select new
                                          {
                                              p.tbStudent.Id,
                                              p.tbStudent.StudentCode,
                                              p.tbStudent.StudentName,
                                              //期中分
                                              //TermCenterValue = p.Mark1,
                                              //期末
                                              //TermEndValue = p.Mark2,
                                              //过程分
                                              ProcessValue = o.Mark2,
                                              //考勤分
                                              AttendanceValue = o.Mark1,
                                              OrgId = org.OrgId,
                                              LevelName = p.tbExamLevel.ExamLevelName,
                                              SegmentMark = p.SegmentMark,
                                              StudyPoint = p.tbExamCourse.tbCourse.Point * p.tbExamLevel.ExamLevelValue,
                                          }).ToList();
                            }
                        }

                    }
                }

                return View(vm);
            }
        }
        #endregion
    }
}