using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamCourseController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.List();
                vm.ExamName = db.Table<Exam.Entity.tbExam>().FirstOrDefault(d => d.Id == vm.ExamId).ExamName;
                vm.SubjectList = Course.Controllers.SubjectController.SelectList();
                vm.CourseList = Course.Controllers.CourseController.SelectList();
                vm.CourseTypeList = Course.Controllers.CourseTypeController.SelectList();
                vm.ExamLevelGroupList = Exam.Controllers.ExamLevelGroupController.SelectList();

                var tb = from p in db.Table<Exam.Entity.tbExamCourse>()
                         where p.tbExam.Id == vm.ExamId
                         select p;

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
                          orderby p.tbCourse.tbSubject.No, p.tbCourse.CourseName
                          select new
                          {
                              Id = p.Id,
                              CourseName = p.tbCourse.CourseName,
                              ExamLevelGroupName = p.tbExamLevelGroup.ExamLevelGroupName,
                              ExamName = p.tbExam.ExamName,
                              FullSegmentMark = p.FullSegmentMark,
                              FullTotalMark = p.FullTotalMark,
                              TotalRate = p.TotalRate,
                              AppraiseRate = p.AppraiseRate,
                              FullAppraiseMark = p.FullAppraiseMark,
                              SubjectName = p.tbCourse.tbSubject.SubjectName,
                              StudyTime = p.tbExamSection.tbGrade.GradeName + p.tbExamSection.ExamSectionName,
                              FromDate = p.FromDate != null ? p.FromDate.ToString() : string.Empty,
                              ToDate = p.ToDate != null ? p.ToDate.ToString() : string.Empty,
                              p.Identified
                          }).ToList();

                var courseRateList = (from p in db.Table<Entity.tbExamCourseRate>()
                                  where  p.tbExamCourse.IsDeleted==false
                                      && p.tbExamCourse1.IsDeleted==false
                                      && p.tbExamCourse.tbExam.Id==vm.ExamId
                                  select new {p.tbExamCourse.Id, courseRateId = p.Id }).ToList();

                vm.ExamCourseList = (from p in tf
                                     select new Dto.ExamCourse.List
                                     {
                                         Id = p.Id,
                                         CourseName = p.CourseName,
                                         ExamLevelGroupName = p.ExamLevelGroupName,
                                         ExamName = p.ExamName,
                                         FullSegmentMark = p.FullSegmentMark,
                                         FullTotalMark = p.FullTotalMark,
                                         TotalRate = p.TotalRate,
                                         AppraiseRate = p.AppraiseRate,
                                         FullAppraiseMark = p.FullAppraiseMark,
                                         SubjectName = p.SubjectName,
                                         StudyTime = p.StudyTime,
                                         Identified=p.Identified,
                                         SetName= (from t in courseRateList where t.Id ==p.Id select t.courseRateId).Count() > decimal.Zero ? "已设置(" + (from t in courseRateList where t.Id == p.Id select t.courseRateId).Count() + "个课程)" : "设置",
                                         InputDate = (!string.IsNullOrEmpty(p.FromDate) ? p.FromDate.ConvertToDateTime().ToString(Code.Common.StringToDateTime) : p.FromDate)
                                                    + "-" + (!string.IsNullOrEmpty(p.ToDate) ? p.ToDate.ConvertToDateTime().ToString(Code.Common.StringToDateTime) : p.FromDate)
                                     }).ToList();
                return View(vm);
            }
        }

        public ActionResult SearchCourse(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.SearchCourse();
                vm.SubjectList = Course.Controllers.SubjectController.SelectList();
                vm.FieldList = new List<SelectListItem>()
                {
                    new System.Web.Mvc.SelectListItem { Text = "教学班", Value = "1" },
                    new System.Web.Mvc.SelectListItem { Text = "全部科目", Value = "2" },
                 };

                if (vm.FieldId == 0 && vm.FieldList.Count > 0)
                {
                    vm.FieldId = vm.FieldList.FirstOrDefault().Value.ConvertToInt();
                }

                var exam = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            select new
                            {
                                YearId = p.tbYear.Id,
                            }).FirstOrDefault();
                if (exam != null)
                {
                    vm.YearId = exam.YearId;
                }

                if (vm.FieldId == decimal.One)//查教学班
                {
                    var tb = from p in db.Table<Course.Entity.tbOrg>()
                             where p.tbYear.Id == vm.YearId
                                 && p.tbCourse.IsDeleted == false
                                 && p.tbCourse.tbSubject.IsDeleted == false
                                 && (p.tbCourse.tbSubject.Id == vm.SubjectId || vm.SubjectId == 0)
                             select p;
                    if (string.IsNullOrEmpty(vm.SearchText) == false)
                    {
                        tb = tb.Where(d => d.tbCourse.CourseName.Contains(vm.SearchText));
                    }

                    vm.SubjectCourseList = (from p in tb
                                            orderby p.tbCourse.CourseName
                                            select new Areas.Course.Dto.Course.List
                                            {
                                                Id = p.tbCourse.Id,
                                                CourseName = p.tbCourse.CourseName,
                                                SubjectName = p.tbCourse.tbSubject.SubjectName
                                            }).Distinct().ToList();
                }
                else
                {
                    var tb = from p in db.Table<Course.Entity.tbCourse>()
                             where p.tbSubject.IsDeleted == false
                                 && (p.tbSubject.Id == vm.SubjectId || vm.SubjectId == 0)
                             select p;
                    if (string.IsNullOrEmpty(vm.SearchText) == false)
                    {
                        tb = tb.Where(d => d.CourseName.Contains(vm.SearchText));
                    }

                    vm.SubjectCourseList = (from p in tb
                                            orderby p.CourseName
                                            select new Areas.Course.Dto.Course.List
                                            {
                                                Id = p.Id,
                                                CourseName = p.CourseName,
                                                SubjectName = p.tbSubject.SubjectName
                                            }).Distinct().ToList();
                }

                return View(vm);
            }
        }

        [HttpPost]
        public ActionResult Insert(List<int> ids, int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var examCourseList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                      where p.tbExam.Id == examId
                                      && p.tbCourse.IsDeleted == false
                                      select p.tbCourse.Id).ToList();
                var courseList = (from p in db.Table<Course.Entity.tbCourse>()
                                  where ids.Contains(p.Id) && examCourseList.Contains(p.Id) == false
                                  select p).ToList();
                var levelGroup = db.Table<Exam.Entity.tbExamLevelGroup>().OrderBy(d => d.No).FirstOrDefault();
                var list = new List<Exam.Entity.tbExamCourse>();
                foreach (var course in courseList)
                {
                    var tb = new Exam.Entity.tbExamCourse();
                    tb.tbExam = db.Set<Exam.Entity.tbExam>().Find(examId);
                    tb.tbCourse = course;
                    tb.tbExamLevelGroup = levelGroup;
                    list.Add(tb);
                }

                db.Set<Exam.Entity.tbExamCourse>().AddRange(list);

                if (db.SaveChanges() > 0)
                {
                    var examCourseIds= (from p in db.Table<Exam.Entity.tbExamCourse>()
                                          where p.tbExam.Id == examId
                                          && p.tbCourse.IsDeleted == false
                                          select p.Id).ToList();
                    //添加任课老师
                    foreach (var examCourseId in examCourseIds)
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
                                            && p.tbTeacher.Id == Teacher.Id
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
                    }
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试课程");
                }

                db.SaveChanges();

                return Code.MvcHelper.Post(null, string.Empty, "操作成功");
            }
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchCourse(Models.ExamCourse.SearchCourse vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SearchCourse", new
            {
                examId = vm.ExamId,
                FieldId = vm.FieldId,
                SubjectId = vm.SubjectId,
                searchText = vm.SearchText
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCourse(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.SearchCourse();
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    foreach (var courseId in ids)
                    {
                        var tc = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                  where p.tbCourse.Id == courseId && p.tbExam.Id == vm.ExamId
                                  select p).FirstOrDefault();
                        if (tc == null)
                        {
                            var tb = new Exam.Entity.tbExamCourse();
                            tb.tbCourse = db.Set<Course.Entity.tbCourse>().Find(courseId);
                            tb.tbExam = db.Set<Exam.Entity.tbExam>().Find(vm.ExamId);
                            tb.FullSegmentMark = 100;
                            tb.FullTotalMark = 100;
                            tb.TotalRate = 100;
                            tb.AppraiseRate = 100;
                            db.Set<Exam.Entity.tbExamCourse>().Add(tb);
                        }
                    }
                    if (db.SaveChanges() > 0)
                    {
                        XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试课程");
                    }

                }

                return Code.MvcHelper.Post(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamCourse.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { examId = vm.ExamId, SubjectId = vm.SubjectId, CourseTypeId = vm.CourseTypeId, searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.IsDeleted = true;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除考试课程");
                }

                return Code.MvcHelper.Post();
            }
        }

        #region 统计转换
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuildExamAppraiseMark(List<int> ids, int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.List();
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var exam = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == examId
                                select new
                                {
                                    p.tbYear,
                                }).FirstOrDefault();
                    var examCourseList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                            .Include(d => d.tbCourse)
                                          where ids.Contains(p.Id)
                                          select p).ToList();
                    var examApprasieMark = (from p in db.Table<Exam.Entity.tbExamAppraiseMark>()
                                            join q in db.Table<Exam.Entity.tbExamCourse>() on p.tbCourse.Id equals q.tbCourse.Id
                                            where p.tbYear.Id == exam.tbYear.Id
                                            && ids.Contains(q.Id)
                                            select p).Include(d => d.tbCourse).Include(d => d.tbStudent).ToList();
                    //var studentAppraiseValue = (from p in db.Table<Perform.Entity.tbPerformData>()
                    //                            join q in db.Table<Exam.Entity.tbExamCourse>() on p.tbCourse.Id equals q.tbCourse.Id
                    //                            where ids.Contains(q.Id)
                    //                            && p.tbPerformItem.IsDeleted == false
                    //                            && p.tbPerformItem.tbPerformGroup.IsDeleted == false
                    //                            && p.tbPerformItem.tbPerformGroup.tbPerform.IsDeleted == false
                    //                            && p.tbPerformItem.tbPerformGroup.tbPerform.tbYear.Id == exam.tbYear.Id
                    //                            select new
                    //                            {
                    //                                CourseId = p.tbCourse.Id,
                    //                                StudentId = p.tbStudent.Id,
                    //                                p.Score
                    //                            }).ToList();
                    //按照配置的比例生成评价总分
                    var studentAppraiseValue = (from p in db.Table<Perform.Entity.tbPerformTotal>()
                                                join q in db.Table<Exam.Entity.tbExamCourse>() on p.tbCourse.Id equals q.tbCourse.Id
                                                where ids.Contains(q.Id)
                                                && p.tbPerform.IsDeleted == false
                                                && p.tbPerform.tbYear.Id == exam.tbYear.Id
                                                select new
                                                {
                                                    CourseId = p.tbCourse.Id,
                                                    StudentId = p.tbStudent.Id,
                                                    Score=p.TotalScore
                                                }).ToList();

                    var studentAttendanceValue = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                                  join q in db.Table<Exam.Entity.tbExamCourse>() on p.tbOrg.tbCourse.Id equals q.tbCourse.Id
                                                  where p.tbAttendanceType.IsDeleted == false
                                                  && p.tbOrg.tbYear.Id == exam.tbYear.Id
                                                  && p.tbOrg.IsDeleted == false
                                                  && ids.Contains(q.Id)
                                                  select new
                                                  {
                                                      CourseId = p.tbOrg.tbCourse.Id,
                                                      StudentId = p.tbStudent.Id,
                                                      p.tbAttendanceType.AttendanceValue
                                                  }).ToList();
                    var orgList = (from p in db.Table<Course.Entity.tbOrg>()
                                   join q in db.Table<Exam.Entity.tbExamCourse>() on p.tbCourse.Id equals q.tbCourse.Id
                                   where p.tbYear.Id == exam.tbYear.Id
                                   && ids.Contains(q.Id)
                                   select new
                                   {
                                       p.Id,
                                       p.IsClass,
                                       ClassId = p.tbClass != null ? p.tbClass.Id : 0,
                                       CourseId = p.tbCourse.Id
                                   }).ToList();

                    foreach (var examCourse in examCourseList)
                    {
                        var orgIds = orgList.Where(d => d.CourseId == examCourse.tbCourse.Id && d.IsClass == false).Select(d => d.Id).ToList();
                        var classIds = orgList.Where(d => d.CourseId == examCourse.tbCourse.Id && d.IsClass).Select(d => d.ClassId).ToList();
                        var orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                                .Include(d => d.tbOrg)
                                                .Include(d => d.tbStudent)
                                              where orgIds.Contains(p.tbOrg.Id)
                                                && p.tbStudent.IsDeleted == false
                                              select p).ToList();
                        var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                    .Include(d => d.tbClass)
                                                    .Include(d => d.tbStudent)
                                                where classIds.Contains(p.tbClass.Id)
                                                    && p.tbStudent.IsDeleted == false
                                                select p).ToList();


                        var list = new List<Exam.Entity.tbExamAppraiseMark>();
                        foreach (var s in orgList)
                        {
                            var isClass = s.IsClass;
                            if (isClass)//行政班
                            {
                                foreach (var t in classStudentList.Where(d => d.tbClass.Id == s.ClassId))
                                {
                                    var a = studentAttendanceValue.Where(d => d.CourseId == examCourse.tbCourse.Id && d.StudentId == t.tbStudent.Id).Sum(d => d.AttendanceValue);
                                    var tb = examApprasieMark.Where(d => d.tbCourse.Id == examCourse.tbCourse.Id && d.tbStudent.Id == t.tbStudent.Id).FirstOrDefault();
                                    if (tb == null)
                                    {
                                        var tf = new Exam.Entity.tbExamAppraiseMark();
                                        tf.tbStudent = t.tbStudent;
                                        tf.tbCourse = examCourse.tbCourse;
                                        tf.tbYear = exam.tbYear;
                                        tf.Mark1 = a;//考勤分
                                        tf.Mark2 = studentAppraiseValue.Where(d => d.CourseId == examCourse.tbCourse.Id && d.StudentId == t.tbStudent.Id).Sum(d => d.Score);//过程分
                                        list.Add(tf);
                                    }
                                    else
                                    {
                                        tb.Mark1 = a;//考勤分
                                        tb.Mark2 = studentAppraiseValue.Where(d => d.CourseId == examCourse.tbCourse.Id && d.StudentId == t.tbStudent.Id).Sum(d => d.Score);//过程分
                                    }

                                }
                            }
                            else
                            {
                                foreach (var t in orgStudentList.Where(d => d.tbOrg.Id == s.Id))
                                {
                                    var a = studentAttendanceValue.Where(d => d.CourseId == examCourse.tbCourse.Id && d.StudentId == t.tbStudent.Id).Sum(d => d.AttendanceValue);
                                    var tb = examApprasieMark.Where(d => d.tbCourse.Id == examCourse.tbCourse.Id && d.tbStudent.Id == t.tbStudent.Id).Select(d => d).FirstOrDefault();
                                    if (tb == null)
                                    {
                                        var tf = new Exam.Entity.tbExamAppraiseMark();
                                        tf.tbStudent = t.tbStudent;
                                        tf.tbCourse = examCourse.tbCourse;
                                        tf.tbYear = exam.tbYear;
                                        tf.Mark1 = a;//考勤分
                                        tf.Mark2 = studentAppraiseValue.Where(d => d.CourseId == examCourse.tbCourse.Id && d.StudentId == t.tbStudent.Id).Sum(d => d.Score);//过程分
                                        list.Add(tf);
                                    }
                                    else
                                    {
                                        tb.Mark1 = a;//考勤分
                                        tb.Mark2 = studentAppraiseValue.Where(d => d.CourseId == examCourse.tbCourse.Id && d.StudentId == t.tbStudent.Id).Sum(d => d.Score);//过程分
                                    }
                                }
                            }
                        }

                        db.Set<Exam.Entity.tbExamAppraiseMark>().AddRange(list);
                    }

                    db.SaveChanges();
                }

                return Code.MvcHelper.Post(null, null, "生成成功!");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuildAppraiseMark(List<int> ids, int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.List();
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var year = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == examId
                                select new
                                {
                                    YearId = p.tbYear.Id,
                                }).FirstOrDefault();

                    foreach (var examCourseId in ids)
                    {
                        var courseId = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                        where p.Id == examCourseId
                                        select p.tbCourse.Id).FirstOrDefault();

                        var examApprasieMark = (from p in db.Table<Exam.Entity.tbExamAppraiseMark>()
                                                where p.tbYear.Id == year.YearId
                                                && p.tbCourse.Id == courseId
                                                && p.tbStudent.IsDeleted == false
                                                select new
                                                {
                                                    StudentId = p.tbStudent.Id,
                                                    p.Mark1,
                                                    p.Mark2
                                                }).ToList();

                        var examMark = (from p in db.Table<Exam.Entity.tbExamMark>()
                                        where p.tbExamCourse.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbExamCourse.tbExam.Id == examId
                                        && p.tbExamCourse.tbCourse.Id == courseId
                                        select new
                                        {
                                            p,
                                            StudentId = p.tbStudent.Id
                                        }).ToList();

                        foreach (var t in examMark)
                        {
                            var aa = examApprasieMark.Where(d => d.StudentId == t.StudentId).FirstOrDefault();
                            if (aa != null)
                            {
                                t.p.AppraiseMark = aa.Mark1 + aa.Mark2;
                                if (t.p.AppraiseMark < decimal.Zero)
                                {
                                    t.p.AppraiseMark = decimal.Zero;
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                }
                return Code.MvcHelper.Post(null, string.Empty, "生成成功!");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuildSegmentMark(List<int> ids, int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.List();
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var examMark = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                    && p.tbStudent.IsDeleted == false
                                    && ids.Contains(p.tbExamCourse.Id)
                                    select new
                                    {
                                        p,
                                        StudentId = p.tbStudent.Id,
                                        p.tbExamCourse.TotalRate,
                                        p.tbExamCourse.AppraiseRate,
                                        p.tbExamCourse.FullSegmentMark
                                    }).ToList();

                    foreach (var t in examMark)
                    {
                        if (t.p.AppraiseMark == null && t.p.TotalMark == null)
                        {
                            t.p.SegmentMark = null;
                        }
                        else
                        {
                            var SegmentMark = (t.p.AppraiseMark ?? 0) * t.AppraiseRate / 100 + (t.p.TotalMark ?? 0) * t.TotalRate / 100;
                            t.p.SegmentMark = SegmentMark > t.FullSegmentMark ? t.FullSegmentMark : SegmentMark;
                        }
                    }

                    db.SaveChanges();
                }

                return Code.MvcHelper.Post(null, string.Empty, "生成成功!");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuildLevel(List<int> ids, int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.List();
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var year = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == examId
                                select new
                                {
                                    YearId = p.tbYear.Id,
                                }).FirstOrDefault();

                    foreach (var examCourseId in ids)
                    {
                        var course = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                      where p.Id == examCourseId && p.tbExamLevelGroup.IsDeleted == false
                                      select new
                                      {
                                          p.tbCourse.Id,
                                          LevelGroupId = p.tbExamLevelGroup.Id
                                      }).FirstOrDefault();

                        var examLevelGroup = (from p in db.Table<Exam.Entity.tbExamLevelGroup>()
                                              where p.Id == course.LevelGroupId
                                              select p).FirstOrDefault();

                        var examLevel = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                         where p.IsDeleted == false && p.tbExamLevelGroup.Id == course.LevelGroupId
                                         select p).ToList();

                        var examMark = (from p in db.Table<Exam.Entity.tbExamMark>()
                                            .Include(d => d.tbStudent)
                                            .Include(d => d.tbExamCourse)
                                        where p.tbExamCourse.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbExamCourse.tbExam.Id == examId
                                        && p.tbExamCourse.tbCourse.Id == course.Id
                                        select p).ToList();


                        var rank = decimal.Zero;
                        var mark = (decimal?)null;
                        var count = decimal.One;
                        foreach (var e in examMark.OrderByDescending(d => d.SegmentMark))
                        {
                            if (examLevelGroup.IsGenerate)//百分比
                            {
                                if (mark != e.SegmentMark)
                                {
                                    mark = e.SegmentMark;
                                    rank = rank + count;
                                    count = decimal.One;
                                }
                                else
                                {
                                    count = count + decimal.One;
                                }

                                Exam.Entity.tbExamLevel level = null;
                                if ((double)e.SegmentMark >= (double)e.tbExamCourse.FullSegmentMark * 0.6)
                                {
                                    level = (from p0 in examLevel.Where(p => p.Rate != decimal.Zero)
                                             let v0 = (int)decimal.Ceiling(examMark.Count * p0.Rate / 100)
                                             let v1 = examMark[v0 - 1].SegmentMark
                                             where p0.ExamLevelName != "D" && v1 <= mark
                                             select p0).FirstOrDefault();
                                    if (level != null)
                                    {
                                        e.tbExamLevel = level;
                                    }
                                }
                                else
                                {
                                    level = examLevel.Where(p => p.ExamLevelName == "D").FirstOrDefault();
                                    if (level != null)
                                    {
                                        e.tbExamLevel = level;
                                    }
                                }
                            }
                            else //分数段
                            {
                                var tt = (from p in examLevel
                                          where p.MaxScore >= e.SegmentMark && p.MinScore <= e.SegmentMark
                                          select p).FirstOrDefault();
                                e.tbExamLevel = tt;
                            }
                        }
                    }

                    db.SaveChanges();
                }

                return Code.MvcHelper.Post(null, string.Empty, "生成成功!");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuildRank(List<int> ids, int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.List();
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    foreach (var examCourseId in ids)
                    {
                        var courseId = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                        where p.Id == examCourseId
                                        select p.tbCourse.Id).FirstOrDefault();
                        Areas.Exam.Controllers.ExamMarkController.BuildRank(examId, courseId);
                    }
                    db.SaveChanges();

                }
                return Code.MvcHelper.Post(null, string.Empty, "生成成功!");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BuildTeacher(List<int> ids, int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.List();
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    var year = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == examId
                                select new
                                {
                                    p.tbYear.tbYearParent.tbYearParent.Id,
                                    YearId = p.tbYear.Id
                                }).FirstOrDefault();

                    foreach (var examCourseId in ids)
                    {
                        var course = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                      where p.Id == examCourseId
                                      select new
                                      {
                                          p.tbCourse.Id
                                      }).FirstOrDefault();

                        var orgList = (from p in db.Table<Course.Entity.tbOrg>()
                                      where p.tbCourse.IsDeleted==false && p.tbCourse.Id == course.Id
                                      && p.tbYear.Id==year.YearId
                                      select new
                                      {
                                          OrgId=p.Id,
                                          ClassId= p.tbClass != null ? p.tbClass.Id : 0,
                                          p.IsClass
                                      }).ToList();

                        var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                     .Include(d => d.tbOrg.tbClass)
                                              where p.tbOrg.IsDeleted == false
                                              && p.tbOrg.tbCourse.Id == course.Id
                                              && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
                                              && p.tbOrg.tbYear.Id == year.YearId
                                              && p.tbTeacher.IsDeleted == false
                                              select new
                                              {
                                                  OrgId = p.tbOrg.Id,
                                                  p.tbTeacher
                                              }).ToList();

                        var tb = (from p in db.Table<Exam.Entity.tbExamMark>()
                                            .Include(d => d.tbStudent)
                                            .Include(d => d.tbExamCourse)
                                        where p.tbExamCourse.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbExamCourse.tbExam.Id == examId
                                        && p.tbExamCourse.tbCourse.Id == course.Id
                                        select new
                                        {
                                            p,
                                            p.tbStudent.Id
                                        }).ToList();

                        var orgStudentList = new List<Dto.ExamCourse.List>();
                        foreach (var o in orgList)
                        {
                            if(o.IsClass)//行政班模式
                            {
                                orgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                                      where p.tbClass.Id ==o.ClassId
                                                      && p.tbStudent.IsDeleted == false
                                                      && p.tbClass.tbYear.Id==year.Id
                                                      orderby p.No, p.tbStudent.StudentCode
                                                      select new Dto.ExamCourse.List
                                                      {
                                                         StudentId= p.tbStudent.Id,
                                                         OrgId=o.OrgId
                                                      }).Distinct().ToList();

                                var teacher = (from p in orgTeacherList
                                               join q in orgStudentList on p.OrgId equals q.OrgId
                                               select new
                                               {
                                                   p.tbTeacher,
                                                   q.StudentId
                                               }).ToList();
                                foreach (var t in tb)
                                {
                                    var s = teacher.Where(d => d.StudentId == t.Id).FirstOrDefault();
                                    if (s != null)
                                    {
                                        t.p.tbTeacher =s.tbTeacher;
                                    }
                                }
                            }
                            else
                            {
                                orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                                  where p.tbOrg.Id == o.OrgId
                                                  orderby p.No, p.tbStudent.StudentCode
                                                  select new Dto.ExamCourse.List
                                                  {
                                                      StudentId = p.tbStudent.Id,
                                                      OrgId = o.OrgId
                                                  }).Distinct().ToList();

                                var teacher = (from p in orgTeacherList
                                               join q in orgStudentList on p.OrgId equals q.OrgId
                                               select new
                                               {
                                                   p.tbTeacher,
                                                   q.StudentId
                                               }).ToList();

                                foreach (var t in tb)
                                {
                                    var s = teacher.Where(d => d.StudentId == t.Id).FirstOrDefault();
                                    if (s != null)
                                    {
                                        t.p.tbTeacher = s.tbTeacher;
                                    }
                                }
                            }
                        }
                    }

                    db.SaveChanges();
                }

                return Code.MvcHelper.Post(null, string.Empty, "生成成功!");
            }
        }

        #endregion

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.Edit();
                vm.ExamSectionList = Exam.Controllers.ExamSectionController.SelectList();

                vm.ExamLevelGroupList = Exam.Controllers.ExamLevelGroupController.SelectList();
                vm.TeacherList = Teacher.Controllers.TeacherController.SelectList();
                vm.CourseList = Course.Controllers.CourseController.SelectList();
                var examId = vm.ExamId;
                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                              where p.Id == id
                              && (p.tbExamLevelGroup.IsDeleted == false || p.tbExamLevelGroup == null)
                              select new Dto.ExamCourse.Edit
                              {
                                  Id = p.Id,
                                  CourseId = p.tbCourse.Id,
                                  ExamLevelGroupId = p.tbExamLevelGroup != null ? p.tbExamLevelGroup.Id : 0,
                                  ExamId = p.tbExam.Id,
                                  FullAppraiseMark = p.FullAppraiseMark,
                                  AppraiseRate = p.AppraiseRate,
                                  FullSegmentMark = p.FullSegmentMark,
                                  FullTotalMark = p.FullTotalMark,
                                  TotalRate = p.TotalRate,
                                  ExamSectionId = p.tbExamSection.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamCourseEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamCourse.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamCourseEdit.Id == 0)
                    {
                        var tb = new Exam.Entity.tbExamCourse();
                        tb.tbCourse = db.Set<Course.Entity.tbCourse>().Find(vm.ExamCourseEdit.CourseId);
                        tb.tbExam = db.Set<Exam.Entity.tbExam>().Find(vm.ExamId);
                        tb.tbExamLevelGroup = db.Set<Exam.Entity.tbExamLevelGroup>().Find(vm.ExamCourseEdit.ExamLevelGroupId);
                        tb.FullAppraiseMark = vm.ExamCourseEdit.FullAppraiseMark;
                        tb.AppraiseRate = vm.ExamCourseEdit.AppraiseRate;
                        tb.TotalRate = vm.ExamCourseEdit.TotalRate;
                        tb.FullSegmentMark = vm.ExamCourseEdit.FullSegmentMark;
                        tb.FullTotalMark = vm.ExamCourseEdit.FullTotalMark;
                        tb.tbExamSection = db.Set<Exam.Entity.tbExamSection>().Find(vm.ExamCourseEdit.ExamSectionId);
                        tb.Identified = vm.ExamCourseEdit.Identified;
                        db.Set<Exam.Entity.tbExamCourse>().Add(tb);

                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试课程");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                  where p.Id == vm.ExamCourseEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            //如果修改课程验证是否已添加该课程
                            var tm = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                      where p.Id != vm.ExamCourseEdit.Id && p.tbCourse.Id == vm.ExamCourseEdit.CourseId
                                      && p.tbExam.Id == vm.ExamId
                                      select p).FirstOrDefault();
                            if (tm == null)
                            {
                                tb.tbExamLevelGroup = db.Set<Exam.Entity.tbExamLevelGroup>().Find(vm.ExamCourseEdit.ExamLevelGroupId);
                                tb.tbCourse = db.Set<Course.Entity.tbCourse>().Find(vm.ExamCourseEdit.CourseId);
                                tb.FullAppraiseMark = vm.ExamCourseEdit.FullAppraiseMark;
                                tb.AppraiseRate = vm.ExamCourseEdit.AppraiseRate;
                                tb.TotalRate = vm.ExamCourseEdit.TotalRate;
                                tb.FullSegmentMark = vm.ExamCourseEdit.FullSegmentMark;
                                tb.FullTotalMark = vm.ExamCourseEdit.FullTotalMark;
                                tb.Identified = vm.ExamCourseEdit.Identified;
                                tb.tbExamSection = db.Set<Exam.Entity.tbExamSection>().Find(vm.ExamCourseEdit.ExamSectionId);
                            }
                            else
                            {
                                error.AddError("此考试课程已存在!");
                            }

                            if (db.SaveChanges() > 0)
                            {
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考试课程");
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

        public ActionResult GetExamCourseJson(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                          where p.tbExam.Id == examId
                            && p.tbCourse.IsDeleted == false
                            && p.tbCourse.tbSubject.IsDeleted == false
                          orderby p.tbCourse.tbSubject.No, p.tbCourse.CourseCode
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbCourse.CourseName,
                              Value = p.Id.ToString()
                          }).ToList();
                return Json(tb, JsonRequestBehavior.AllowGet);
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectCourseList(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                          where p.tbExam.Id == examId
                            && p.tbCourse.IsDeleted == false
                          orderby p.tbCourse.No, p.tbCourse.CourseName
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbCourse.CourseName,
                              Value = p.tbCourse.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectExamCourseList(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                          where p.tbExam.Id == examId
                          && p.tbCourse.IsDeleted == false
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbCourse.CourseName,
                              Value = p.tbCourse.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetExamCourseList(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                          where p.tbExam.Id == examId
                          && p.tbCourse.IsDeleted == false
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbCourse.CourseName,
                              Value = p.tbCourse.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetExamCourseListByInput(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                          join q in db.Table<Exam.Entity.tbExamPower>() on p.Id equals q.tbExamCourse.Id
                          where p.tbExam.Id == examId
                          && p.tbCourse.IsDeleted == false
                          && q.tbTeacher.tbSysUser.Id == Code.Common.UserId
                          && p.FromDate <= DateTime.Now && DateTime.Now <= p.ToDate
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.tbCourse.CourseName,
                              Value = p.tbCourse.Id.ToString()
                          }).ToList();
                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectOrgList(int examId, int? courseId, int userId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == examId
                            select new
                            {
                                YearId = p.tbYear.Id,
                            }).FirstOrDefault();
                var tb = new List<System.Web.Mvc.SelectListItem>();
                if (year != null)
                {
                    var InputType = (from p in db.Table<Exam.Entity.tbExamPower>()
                                     where p.tbExamCourse.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && p.tbTeacher.tbSysUser.Id == userId
                                     && p.tbExamCourse.tbCourse.Id == courseId
                                     select p.IsOrgTeacher).FirstOrDefault();
                    if (InputType == true)//任课老师
                    {
                        tb = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                              where p.tbOrg.tbCourse.Id == courseId
                                && p.tbOrg.IsDeleted == false
                                && p.tbTeacher.tbSysUser.Id == userId
                                && p.tbOrg.tbYear.Id == year.YearId
                                && (courseId == 0 || p.tbOrg.tbCourse.Id == courseId)
                              orderby p.tbOrg.No
                              select new System.Web.Mvc.SelectListItem
                              {
                                  Text = p.tbOrg.OrgName,
                                  Value = p.tbOrg.Id.ToString()
                              }).ToList();
                    }
                    else//指定人员
                    {
                        var tf = (from p in db.Table<Exam.Entity.tbExamPower>()
                                  where p.tbExamCourse.IsDeleted == false
                                  && p.tbExamCourse.tbExam.Id == examId
                                  && p.tbTeacher.IsDeleted == false
                                  && p.tbTeacher.tbSysUser.Id == userId
                                  && p.tbExamCourse.tbCourse.Id == courseId
                                  select p).FirstOrDefault();
                        if (tf != null)
                        {
                            tb = (from p in db.Table<Course.Entity.tbOrg>()
                                  where p.tbYear.Id == year.YearId
                                    && (courseId == 0 || p.tbCourse.Id == courseId)
                                  orderby p.No
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.OrgName,
                                      Value = p.Id.ToString()
                                  }).ToList();
                        }
                    }
                }

                return tb;
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> SelectAllOrgList(int examId, int? courseId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == examId
                            select new
                            {
                                YearId = p.tbYear.Id,
                            }).FirstOrDefault();
                var tb = new List<System.Web.Mvc.SelectListItem>();
                if (year != null)
                {
                    tb = (from p in db.Table<Course.Entity.tbOrg>()
                          where p.tbYear.Id == year.YearId
                            && (courseId == 0 || p.tbCourse.Id == courseId)
                          orderby p.No
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.OrgName,
                              Value = p.Id.ToString()
                          }).ToList();
                }

                return tb;
            }
        }

        [NonAction]
        public static Dto.ExamCourse.List SelectDtoExamCourse(int examId = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamCourse>()
                          where p.Id == examId
                          && (p.tbExamLevelGroup.IsDeleted == false || p.tbExamLevelGroup == null)
                          select new Dto.ExamCourse.List
                          {
                              Id = p.Id,
                              FullSegmentMark = p.FullSegmentMark,
                              FullTotalMark = p.FullTotalMark,
                              TotalRate = p.TotalRate,
                              AppraiseRate = p.AppraiseRate,
                              FullAppraiseMark = p.FullAppraiseMark
                          }).FirstOrDefault();

                if (tb == null)
                {
                    return null;
                }
                else
                {
                    return tb;
                }
            }
        }

        [NonAction]
        public static List<System.Web.Mvc.SelectListItem> GetExamSubjectList(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tf = (from p in db.Table<Exam.Entity.tbExamCourse>()
                          where p.tbExam.Id == examId
                          && p.tbCourse.IsDeleted == false
                          && p.tbCourse.tbSubject.IsDeleted == false
                          orderby p.tbCourse.tbSubject.No
                          select new
                          {
                              p.tbCourse.tbSubject.SubjectName,
                              p.tbCourse.tbSubject.Id,
                          }).Distinct().ToList();
                var tb = (from p in tf
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.SubjectName,
                              Value = p.Id.ToString()
                          }).ToList();
                return tb;
            }
        }
        #region 模块认定
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetIdentified(int id)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = db.Set<Exam.Entity.tbExamCourse>().Find(id);
                if (tb != null)
                {
                    tb.Identified = !tb.Identified;
                }

                db.SaveChanges();

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult IdentifiedCourse()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.List();
                vm.ExamName = db.Table<Exam.Entity.tbExam>().FirstOrDefault(d => d.Id == vm.ExamId).ExamName;
                vm.SubjectList = GetExamSubjectList(vm.ExamId);
                vm.CourseTypeList = Course.Controllers.CourseTypeController.SelectList();
                var tb = from p in db.Table<Exam.Entity.tbExamCourse>()
                         where p.tbExam.Id == vm.ExamId
                         select p;

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
                          orderby p.tbCourse.tbSubject.No, p.tbCourse.CourseName
                          select new
                          {
                              Id = p.Id,
                              SubjectId=p.tbCourse.tbSubject.Id,
                              SubjectName = p.tbCourse.tbSubject.SubjectName,
                              CourseName = p.tbCourse.CourseName,
                              CourseId=p.tbCourse.Id,
                              p.Identified
                          }).ToList();

                var tg = (from p in tf
                          where p.Identified==false
                          select new
                          {
                              Id = p.Id,
                              p.SubjectId,
                              SubjectName = p.SubjectName,
                              CourseName = p.CourseName,
                          }).ToList();

                vm.ExamCourseList = (from p in tg
                                    select new Dto.ExamCourse.List
                                    {
                                        Id = p.Id,
                                        SubjectId=p.SubjectId,
                                        SubjectName = p.SubjectName,
                                        CourseName = p.CourseName,
                                    }).ToList();

                vm.IdentifiedCourseList = (from p in tf
                                           where p.Identified
                                     select new Dto.ExamCourse.List
                                     {
                                         Id = p.Id,
                                         SubjectId = p.SubjectId,
                                         CourseId=p.CourseId,
                                         CourseName = p.CourseName,
                                     }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IdentifiedCourse(Models.ExamCourse.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("IdentifiedCourse", new
            {
                examId = vm.ExamId,
                SubjectId = vm.SubjectId,
                CourseTypeId=vm.CourseTypeId,
                searchText = vm.SearchText
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveModel(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamCourse.List();
                var lstExamCourse = new List<Dto.ExamCourse.List>();
                var arrystr = new string[] { };
                var txtId = Request["txtId"] != null ? Request["txtId"].Split(',') : arrystr;
                var drpIdentifiedId = Request["IdentifiedId"] != null ? Request["IdentifiedId"].Split(',') : arrystr;
                var cboxlist = new List<int>();
                var CboxIdUnion = Request["CboxId"] != null ? Request["CboxId"].Split(',') : null;
                if (CboxIdUnion == null)
                {
                    return Code.MvcHelper.Post(null, string.Empty, "请选择考试课程!");
                }
                if (CboxIdUnion != null)
                {
                    for (var i = 0; i < CboxIdUnion.Count(); i++)
                    {
                        cboxlist.Add(CboxIdUnion[i].ConvertToInt());
                    }
                }
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    //选中的考试课程
                    for (var i = 0; i < txtId.Count(); i++)
                    {
                        var examCourseId = txtId[i].ConvertToInt();
                        if (cboxlist.Contains(txtId[i].ConvertToInt()))
                        {
                            var model = new Dto.ExamCourse.List();
                            model.ExamCourseId = examCourseId;
                            var courseId = drpIdentifiedId[i].ConvertToInt();
                            var IdentifiedId = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                                where p.tbCourse.Id == courseId && p.tbExam.Id == examId
                                                select p.Id).FirstOrDefault();
                            model.IdentifyExamCourseId = IdentifiedId;
                            lstExamCourse.Add(model);
                        }
                    }
                    var examCourseIds = lstExamCourse.Select(c => c.ExamCourseId).ToList();
                    //课程汇总的考试比例
                    var examCourseRate = (from p in db.Table<Exam.Entity.tbExamCourseRate>()
                                          where p.tbExamCourse.IsDeleted==false &&
                                                p.tbExamCourse.tbExam.Id == examId  
                                                && examCourseIds.Contains(p.tbExamCourse.Id)
                                          select new
                                          {
                                              ID = p.tbExamCourse.Id,
                                              ToAddExamCourseId = p.tbExamCourse1.Id,
                                              p.Rate
                                          }).ToList();
                    var examCourseAll = new List<int>();
                    examCourseAll.AddRange(examCourseIds);
                    examCourseAll.AddRange(lstExamCourse.Where(c => c.IdentifyExamCourseId !=0).Select(c => c.IdentifyExamCourseId));
                    examCourseAll = examCourseAll.Distinct().ToList();

                    var ExamCourseList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                          where examCourseAll.Contains(p.Id)
                                          select p).ToList();

                    //需要进行统计的成绩 
                    var ToAddExamCourseIds = examCourseRate.Select(c => c.ToAddExamCourseId).ToList();
                    var ToAddExamMark = (from p in db.Table<Exam.Entity.tbExamMark>()
                                         where ToAddExamCourseIds.Contains(p.tbExamCourse.Id) &&
                                               p.tbStudent.IsDeleted==false
                                         select new
                                         {
                                             ExamCourseId = p.tbExamCourse.Id,
                                             StudentId = p.tbStudent.Id,
                                             ExamMark = p,
                                         }).ToList();
                    var studentBasis = (from p0 in db.Table<Student.Entity.tbStudent>()
                                        select p0).ToList();

                    var examMarks = (from p in db.Table<Exam.Entity.tbExamMark>()
                                         where examCourseAll.Contains(p.tbExamCourse.Id) &&
                                               p.tbStudent.IsDeleted == false 
                                         select new
                                         {
                                             ExamCourseId = p.tbExamCourse.Id,
                                             StudentId = p.tbStudent.Id,
                                             ExamMark = p,
                                         }).ToList();
                    //删除已认定成绩
                    var oldExamMarks = examMarks.Where(p => p.ExamMark.IsValid == 2);
                    foreach (var em in oldExamMarks)
                    {
                       db.Set<Entity.tbExamMark>().Remove(em.ExamMark);
                    }
                    examMarks = examMarks.Where(p => p.ExamMark.IsValid < 2).ToList();
                    var NewExamMark = new List<Exam.Entity.tbExamMark> (); //新生成的一条汇总成绩，先生成排名和等级再写入数据库

                    foreach (var examCourse in lstExamCourse)
                    {
                        foreach (var em in examMarks.Where(c => c.ExamCourseId == examCourse.ExamCourseId && c.ExamMark.IsValid == decimal.Zero))
                        {
                            var segmentMark = decimal.Zero;
                            var totalMark = decimal.Zero;
                            var appraiseMark = decimal.Zero;
                            //需要按比率进行汇总的成绩中一定是已经包括了本次考试的成绩。
                            var ToAddList = examCourseRate.Where(c => c.ID == em.ExamCourseId).Select(c => new { c.ToAddExamCourseId, c.Rate }).Distinct().ToList();
                            if (ToAddList.Count > 0)
                            {
                                foreach (var t in ToAddList)
                                {
                                    //因为之前的成绩可以存在只有IsValid=1，而没有IsValid=0的情况，因此，此处按IsValid排序取第一条。
                                    var tv = ToAddExamMark.Where(c => c.ExamCourseId == t.ToAddExamCourseId 
                                              && c.StudentId == em.StudentId).OrderBy(c => c.ExamMark.IsValid).FirstOrDefault();
                                    if (tv != null)
                                    {
                                        segmentMark += (tv.ExamMark.SegmentMark == null ? decimal.Zero : tv.ExamMark.SegmentMark.ConvertToDecimal()) * t.Rate / 100;
                                        totalMark += (tv.ExamMark.TotalMark == null ? decimal.Zero : tv.ExamMark.TotalMark.ConvertToDecimal()) * t.Rate / 100;
                                        appraiseMark += (tv.ExamMark.AppraiseMark == null ? decimal.Zero : tv.ExamMark.AppraiseMark.ConvertToDecimal()) * t.Rate / 100;
                                        if (tv.ExamMark.IsValid != decimal.Zero)
                                        {
                                            tv.ExamMark.IsValid = 0;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                segmentMark = em.ExamMark.SegmentMark==null?decimal.Zero: em.ExamMark.SegmentMark.ConvertToDecimal();
                                totalMark = em.ExamMark.TotalMark==null?decimal.Zero: em.ExamMark.TotalMark.ConvertToDecimal();
                                appraiseMark = em.ExamMark.AppraiseMark==null?decimal.Zero: em.ExamMark.AppraiseMark.ConvertToDecimal();
                            }

                            NewExamMark.Add(new Exam.Entity.tbExamMark
                            {
                                tbExamCourse = ExamCourseList.Where(c => c.Id == examCourse.IdentifyExamCourseId).FirstOrDefault(),
                                TotalMark = totalMark,
                                AppraiseMark = appraiseMark,
                                SegmentMark = segmentMark,
                                tbStudent = studentBasis.Where(p => p.Id == em.StudentId).FirstOrDefault(),
                                IsValid = 2,
                            });
                        }
                    }
                    #region 排名
                    var groupList = lstExamCourse.Select(d => d.IdentifyExamCourseId).Distinct().ToList();
                    foreach (var g in groupList)
                    {
                        var course = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                      where p.Id == g && p.tbExamLevelGroup.IsDeleted == false
                                      select new
                                      {
                                          p.tbCourse.Id,
                                          LevelGroupId = p.tbExamLevelGroup.Id
                                      }).FirstOrDefault();

                        var examLevelGroup = (from p in db.Table<Exam.Entity.tbExamLevelGroup>()
                                              where p.Id == course.LevelGroupId
                                              select p).FirstOrDefault();

                        var examLevel = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                         where p.IsDeleted == false && p.tbExamLevelGroup.Id == course.LevelGroupId
                                         select p).ToList();

                        var rank = 0;
                        var mark = (decimal?)null;
                        var count = 1;
                        foreach (var e in NewExamMark.OrderByDescending(d => d.SegmentMark))
                        {
                            if (examLevelGroup.IsGenerate)//百分比
                            {
                                if (mark != e.SegmentMark)
                                {
                                    mark = e.SegmentMark;
                                    rank = rank + count;
                                    count = 1;
                                }
                                else
                                {
                                    count = count + 1;
                                }
                                e.SegmentGradeRank = rank;

                                Exam.Entity.tbExamLevel level = null;
                                if ((double)e.SegmentMark >= (double)e.tbExamCourse.FullSegmentMark * 0.6)
                                {
                                    level = (from p0 in examLevel.Where(p => p.Rate != decimal.Zero)
                                             let v0 = (int)decimal.Ceiling(NewExamMark.Count * p0.Rate / 100)
                                             let v1 = NewExamMark[v0 - 1].SegmentMark
                                             where p0.ExamLevelName != "D" && v1 <= mark
                                             select p0).FirstOrDefault();
                                    if (level != null)
                                    {
                                        e.tbExamLevel = level;
                                    }
                                }
                                else
                                {
                                    level = examLevel.Where(p => p.ExamLevelName == "D").FirstOrDefault();
                                    if (level != null)
                                    {
                                        e.tbExamLevel = level;
                                    }
                                }
                            }
                            else //分数段
                            {
                                if (mark != e.SegmentMark)
                                {
                                    mark = e.SegmentMark;
                                    rank = rank + count;
                                    count = 1;
                                }
                                else
                                {
                                    count = count + 1;
                                }
                                e.SegmentGradeRank = rank;

                                var tt = (from p in examLevel
                                          where p.MaxScore >= e.SegmentMark && p.MinScore <= e.SegmentMark
                                          select p).FirstOrDefault();
                                if (tt != null)
                                {
                                    e.tbExamLevel = tt;
                                }
                            }
                        }
                    }
                    #endregion 

                    foreach (var t in NewExamMark)
                    {
                        var tv = new Exam.Entity.tbExamMark
                        {
                            tbExamCourse = t.tbExamCourse,
                            TotalMark = t.TotalMark,
                            AppraiseMark = t.AppraiseMark,
                            SegmentMark = t.SegmentMark,
                            SegmentGradeRank = t.SegmentGradeRank,
                            tbExamLevel = t.tbExamLevel,
                            tbStudent = t.tbStudent,
                            IsValid = t.IsValid,
                        };
                        db.Set<Exam.Entity.tbExamMark>().Add(tv);
                    }
                }
                db.SaveChanges();
                return Code.MvcHelper.Post(null, string.Empty, "认定成功!");
            }
        }
        #endregion 
    }
}