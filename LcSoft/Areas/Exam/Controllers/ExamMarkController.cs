using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamMarkController : Controller
    {
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamMark.List();
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectPublishList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }
                if (Code.Common.UserType == Code.EnumHelper.SysUserType.Administrator)
                {
                    vm.CourseList = Areas.Exam.Controllers.ExamCourseController.GetExamCourseList(vm.ExamId);
                }
                else
                {
                    vm.CourseList = Areas.Exam.Controllers.ExamCourseController.GetExamCourseListByInput(vm.ExamId);
                }
                if (vm.CourseId == 0 && vm.CourseList.Count > 0)
                {
                    vm.CourseId = vm.CourseList.FirstOrDefault().Value.ConvertToInt();
                }
                else
                {
                    if (vm.CourseList.Where(d => d.Value == vm.CourseId.ToString()).Count() == 0)
                    {
                        if (vm.CourseList.Count > 0)
                        {
                            vm.CourseId = vm.CourseList.FirstOrDefault().Value.ConvertToInt();
                        }
                        else
                        {
                            vm.CourseId = 0;
                        }
                    }
                }
                if (Code.Common.UserType == Code.EnumHelper.SysUserType.Administrator)
                {
                    vm.OrgList = Areas.Exam.Controllers.ExamCourseController.SelectAllOrgList(vm.ExamId, vm.CourseId);
                }
                else
                {
                    vm.OrgList = Areas.Exam.Controllers.ExamCourseController.SelectOrgList(vm.ExamId, vm.CourseId, Code.Common.UserId);
                }
                if (vm.OrgId == 0 && vm.OrgList.Count > 0)
                {
                    vm.OrgId = vm.OrgList.FirstOrDefault().Value.ConvertToInt();
                }
                else
                {
                    if (vm.OrgList.Where(d => d.Value == vm.OrgId.ToString()).Count() == 0)
                    {
                        if (vm.OrgList.Count > 0)
                        {
                            vm.OrgId = vm.OrgList.FirstOrDefault().Value.ConvertToInt();
                        }
                        else
                        {
                            vm.OrgId = 0;
                        }
                    }
                }
                vm.ExamStatusList = Areas.Exam.Controllers.ExamStatusController.SelectList();
                //增加校验
                vm.ExamCourseList = ExamCourseController.SelectDtoExamCourse(vm.ExamId);
                var levelGroupId = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                    where p.tbExam.Id == vm.ExamId && p.tbCourse.Id == vm.CourseId
                                    && p.tbExamLevelGroup.IsDeleted == false
                                    select p.tbExamLevelGroup.Id).FirstOrDefault();
                vm.ExamLevelList = Areas.Exam.Controllers.ExamLevelController.SelectList(levelGroupId);

                var tb = from p in db.Table<Exam.Entity.tbExamMark>()
                         where (p.tbExamCourse.tbCourse.Id == vm.CourseId || vm.CourseId == 0)
                          && p.tbExamCourse.IsDeleted == false
                          && (p.tbExamCourse.tbExam.Id == vm.ExamId || vm.ExamId == 0)
                         select p;

                var examCourseId = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                    where p.tbCourse.Id == vm.CourseId
                                    && p.tbExam.Id == vm.ExamId
                                    select p.Id).FirstOrDefault();

                var orgtype = (from p in db.Table<Course.Entity.tbOrg>()
                               where p.Id == vm.OrgId
                               && (p.tbClass.IsDeleted == false || p.tbClass == null)
                               && p.tbCourse.IsDeleted == false
                               select new
                               {
                                   p.IsClass,
                                   Id = p.tbClass != null ? p.tbClass.Id : 0,
                                   p.tbCourse.CourseName,
                                   p.tbCourse.IsLevel
                               }).FirstOrDefault();
                if (orgtype != null)
                {
                    if (orgtype.IsClass == false)//走班模式
                    {
                        var orgStudentList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                              where p.tbOrg.Id == vm.OrgId
                                              orderby p.No, p.tbStudent.StudentCode
                                              select new
                                              {
                                                  p.No,
                                                  StudentId = p.tbStudent.Id,
                                                  p.tbStudent.StudentCode,
                                                  p.tbStudent.StudentName,
                                                  p.tbOrg.tbCourse.CourseName,
                                                  p.tbOrg.tbCourse.IsLevel,
                                              }).ToList();
                        vm.ExamMarkList = (from p in orgStudentList
                                           select new Dto.ExamMark.List
                                           {
                                               Id = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.Id).FirstOrDefault(),
                                               ExamCourseId = examCourseId,
                                               StudentId = p.StudentId,
                                               Islevel=p.IsLevel,
                                               No = p.No,
                                               StudentCode = p.StudentCode,
                                               StudentName = p.StudentName,
                                               AppraiseMark = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.AppraiseMark).FirstOrDefault(),
                                               SegmentMark = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.SegmentMark).FirstOrDefault(),
                                               TotalMark = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.TotalMark).FirstOrDefault(),
                                               ExamLevelId = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.tbExamLevel).FirstOrDefault() != null ? tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.tbExamLevel.Id).FirstOrDefault() : (int?)null,
                                               ExamStatusId = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.tbExamStatus != null ? d.tbExamStatus.Id : 0).FirstOrDefault(),
                                               ExamStatusName = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.tbExamStatus != null ? d.tbExamStatus.ExamStatusName : string.Empty).FirstOrDefault(),
                                           }).ToList();
                    }
                    else//行政班模式
                    {
                        var orgStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
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
                                                  orgtype.IsLevel,
                                              }).ToList();
                        vm.ExamMarkList = (from p in orgStudentList
                                           select new Dto.ExamMark.List
                                           {
                                               Id = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.Id).FirstOrDefault(),
                                               ExamCourseId = examCourseId,
                                               StudentId = p.StudentId,
                                               Islevel=p.IsLevel,
                                               No = p.No,
                                               StudentCode = p.StudentCode,
                                               StudentName = p.StudentName,
                                               AppraiseMark = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.AppraiseMark).FirstOrDefault(),
                                               SegmentMark = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.SegmentMark).FirstOrDefault(),
                                               TotalMark = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.TotalMark).FirstOrDefault(),
                                               ExamLevelId = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.tbExamLevel).FirstOrDefault() != null ? tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.tbExamLevel.Id).FirstOrDefault() : (int?)null,
                                               ExamStatusId = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.tbExamStatus != null ? d.tbExamStatus.Id : 0).FirstOrDefault(),
                                               ExamStatusName = tb.Where(d => d.tbStudent.Id == p.StudentId).Select(d => d.tbExamStatus != null ? d.tbExamStatus.ExamStatusName : string.Empty).FirstOrDefault(),
                                           }).ToList();
                    }
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamMark.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("List", new { ExamId = vm.ExamId, CourseId = vm.CourseId, OrgId = vm.OrgId, searchText = vm.SearchText }));
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

        public ActionResult Edit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamMark.Edit();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamMark>()
                              where p.Id == id
                              select p).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamMarkEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.ExamMark.Edit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamMarkEdit.Id == 0)
                    {
                        var tb = new Exam.Entity.tbExamMark();
                        db.Set<Exam.Entity.tbExamMark>().Add(tb);
                        if (db.SaveChanges() > 0)
                        {
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试成绩");
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamMark>()
                                  where p.Id == vm.ExamMarkEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Models.ExamMark.List vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var arrystr = new string[] { };
                var txtId = Request["txtId"] != null ? Request["txtId"].Split(',') : arrystr;
                var txtStudentId = Request["txtStudentId"] != null ? Request["txtStudentId"].Split(',') : arrystr;
                var txtExamCourseId = Request["txtExamCourseId"] != null ? Request["txtExamCourseId"].Split(',') : arrystr;
                var drpExamStatusId = Request["ExamStatusId"] != null ? Request["ExamStatusId"].Split(',') : arrystr;
                //var txtAppraiseMark = Request["txtAppraiseMark"] != null ? Request["txtAppraiseMark"].Split(',') : arrystr;
                var txtTotalMark = Request["txtTotalMark"] != null ? Request["txtTotalMark"].Split(',') : null;
                //var txtSegmentMark = Request["txtSegmentMark"] != null ? Request["txtSegmentMark"].Split(',') : arrystr;
                var drpExamLevelId = Request["ExamLevelId"] != null ? Request["ExamLevelId"].Split(',') : arrystr;

                //验证
                var examCourseId = txtExamCourseId.Count() > decimal.Zero ? txtExamCourseId[0].ConvertToInt() : 0;
                var fullMark = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                where p.Id == examCourseId
                                select new
                                {
                                    p.FullAppraiseMark,
                                    p.FullSegmentMark,
                                    p.FullTotalMark,
                                    ExamId = p.tbExam.Id,
                                    CourseId = p.tbCourse.Id
                                }).FirstOrDefault();
                if (fullMark == null) return Code.MvcHelper.Post(null, Url.Action("List"), "暂无数据!");
                for (var i = 0; i < txtStudentId.Count(); i++)
                {
                    var totalMark = txtTotalMark!=null? txtTotalMark[i].ConvertToDecimal():decimal.MinusOne;

                    if (totalMark > fullMark.FullTotalMark)
                    {
                        var strmes = string.Format("考试成绩超过满分值({0})", fullMark.FullTotalMark);
                        return Code.MvcHelper.Post(null, null, strmes, false);
                    }
                }

                var list = (from p in db.Table<Exam.Entity.tbExamMark>()
                            where p.tbExamCourse.Id == examCourseId
                                && txtStudentId.Contains(p.tbStudent.Id.ToString())
                            select p).ToList();
                foreach (var a in list.Where(d => txtId.Contains(d.Id.ToString()) == false))
                {
                    a.IsDeleted = true;
                }

                for (var i = 0; i < txtStudentId.Count(); i++)
                {
                    if (string.IsNullOrEmpty(txtStudentId[i]))
                    {
                        //输入内容为空,判断是否存在Id
                        if (string.IsNullOrEmpty(txtId[i]) == false)
                        {
                            //如果是有id的，那就是数据库中记录的，应该做删除
                            var tb = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("删除考试成绩");
                            tb.IsDeleted = true;
                        }
                    }
                    else
                    {
                        //输入内容不为空，判断是否存在id并执行对应的操作
                        if (string.IsNullOrEmpty(txtId[i]) == false && txtId[i].ConvertToInt() != 0)
                        {
                            //如果有id的，执行更新操作
                            var tb = list.Where(d => d.Id == txtId[i].ConvertToInt()).FirstOrDefault();
                            var totalMark = txtTotalMark != null ? txtTotalMark[i].ConvertToDecimalWithNull():null;
                            //var segmentMark = txtSegmentMark[i].ConvertToDecimalWithNull();
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("修改考试成绩");
                            tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(txtStudentId[i].ConvertToInt());
                            tb.tbExamCourse = db.Set<Exam.Entity.tbExamCourse>().Find(txtExamCourseId[i].ConvertToInt());
                            tb.tbExamStatus = db.Set<Exam.Entity.tbExamStatus>().Find(drpExamStatusId[i].ConvertToInt());
                            //tb.tbExamLevel = db.Set<Exam.Entity.tbExamLevel>().Find(drpExamLevelId[i].ConvertToInt());
                            //tb.AppraiseMark = txtAppraiseMark[i].ConvertToDecimalWithNull();
                            tb.TotalMark = totalMark;
                            //tb.SegmentMark = segmentMark;
                        }
                        else
                        {
                            //没有id的，执行插入操作
                            var tb = new Exam.Entity.tbExamMark();
                            var totalMark = txtTotalMark!=null? txtTotalMark[i].ConvertToDecimalWithNull():null;
                            //var segmentMark = txtSegmentMark[i].ConvertToDecimalWithNull();
                            tb.tbStudent = db.Set<Student.Entity.tbStudent>().Find(txtStudentId[i].ConvertToInt());
                            tb.tbExamCourse = db.Set<Exam.Entity.tbExamCourse>().Find(txtExamCourseId[i].ConvertToInt());
                            tb.tbExamStatus = db.Set<Exam.Entity.tbExamStatus>().Find(drpExamStatusId[i].ConvertToInt());
                            //tb.tbExamLevel = db.Set<Exam.Entity.tbExamLevel>().Find(drpExamLevelId[i].ConvertToInt());
                            //tb.AppraiseMark = txtAppraiseMark[i].ConvertToDecimalWithNull();
                            tb.TotalMark = totalMark;
                            //tb.SegmentMark = segmentMark;
                            db.Set<Exam.Entity.tbExamMark>().Add(tb);
                            XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试成绩");
                        }
                    }
                }
                db.SaveChanges();
                BuildRank(fullMark.ExamId, fullMark.CourseId);
                return Code.MvcHelper.Post(null, string.Empty, "提交成功!");
            }
        }

        [NonAction]
        public static void BuildRank(int examId, int courseId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == examId
                            select new
                            {
                                p.tbYear.tbYearParent.tbYearParent.Id,
                                YearId = p.tbYear.Id,
                            }).FirstOrDefault();
                var gradeList = (from p in db.Table<Basis.Entity.tbGrade>()
                                 orderby p.No, p.GradeName
                                 select p).ToList();
                var orgGroupList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbYear.Id == year.YearId
                                        && p.tbCourse.Id == courseId
                                        && p.tbGrade.IsDeleted == false
                                    select new
                                    {
                                        OrgId = p.Id,
                                        OrgName = p.OrgName,
                                        GradeId = p.tbGrade.Id,
                                        p.IsClass
                                    }).Distinct().ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbClass.IsDeleted == false
                                        && p.tbClass.tbYear.Id == year.Id
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbClass.tbGrade.IsDeleted == false
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassId = p.tbClass.Id,
                                        GradeId = p.tbClass.tbGrade.Id
                                    }).Distinct().ToList();

                var orgStudent = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                  where p.tbOrg.IsDeleted == false
                                      && p.tbOrg.tbYear.Id == year.YearId
                                      && p.tbStudent.IsDeleted == false
                                      && p.tbOrg.tbGrade.IsDeleted == false
                                  select new
                                  {
                                      StudentId = p.tbStudent.Id,
                                      OrgId = p.tbOrg.Id,
                                      GradeId = p.tbOrg.tbGrade.Id
                                  }).Distinct().ToList();

                var examMark = (from p in db.Table<Exam.Entity.tbExamMark>()
                                where p.tbExamCourse.IsDeleted == false
                                    && p.tbExamCourse.tbExam.Id == examId
                                    && p.tbExamCourse.tbCourse.Id == courseId
                                    && p.tbStudent.IsDeleted == false
                                select new
                                {
                                    p,
                                    StudnetId = p.tbStudent.Id
                                }).Distinct().ToList();

                #region 年级排名
                foreach (var grade in gradeList)
                {
                    var grdeExamMrkList = (from t in examMark
                                           join p in classStudent.Where(d => d.GradeId == grade.Id)
                                           on t.StudnetId equals p.StudentId
                                           select new
                                           {
                                               t,
                                               p.ClassId
                                           }).ToList();
                    var rank = 0;
                    decimal? mark = null;
                    var count = 1;
                    var exam = grdeExamMrkList;

                    foreach (var t in exam.OrderByDescending(d => d.t.p.TotalMark))
                    {
                        if (t.t.p.TotalMark == null)
                        {
                            t.t.p.TotalGradeRank = null;
                            continue;
                        }
                        if (mark != t.t.p.TotalMark)
                        {
                            mark = t.t.p.TotalMark;
                            rank = rank + count;
                            count = 1;
                        }
                        else
                        {
                            count = count + 1;
                        }
                        t.t.p.TotalGradeRank = rank;
                    }

                    rank = 0;
                    mark = null;
                    count = 1;
                    foreach (var t in exam.OrderByDescending(d => d.t.p.SegmentMark))
                    {
                        if (t.t.p.SegmentMark == null)
                        {
                            t.t.p.SegmentGradeRank = null;
                            continue;
                        }
                        if (mark != t.t.p.SegmentMark)
                        {
                            mark = t.t.p.SegmentMark;
                            rank = rank + count;
                            count = 1;
                        }
                        else
                        {
                            count = count + 1;
                        }
                        t.t.p.SegmentGradeRank = rank;
                    }
                }
                #endregion

                #region 班级排名
                foreach (var grade in orgGroupList)
                {
                    if (grade.IsClass)//行政班模式
                    {
                        #region 行政班
                        var classExamMrkList = (from t in examMark
                                                join p in classStudent
                                                on t.StudnetId equals p.StudentId
                                                select new
                                                {
                                                    t,
                                                    p.ClassId
                                                }).ToList();

                        var classIds = classStudent.Select(d => d.ClassId).Distinct().ToList();
                        foreach (var classId in classIds)
                        {
                            var examClass = (from t in classExamMrkList
                                             where t.ClassId == classId
                                             select t).ToList();
                            var rank = 0;
                            decimal? mark = null;
                            var count = 1;
                            foreach (var t in examClass.OrderByDescending(d => d.t.p.TotalMark))
                            {
                                if (t.t.p.TotalMark == null)
                                {
                                    t.t.p.TotalClassRank = null;
                                    continue;
                                }
                                if (mark != t.t.p.TotalMark)
                                {
                                    mark = t.t.p.TotalMark;
                                    rank = rank + count;
                                    count = 1;
                                }
                                else
                                {
                                    count = count + 1;
                                }

                                t.t.p.TotalClassRank = rank;
                            }

                            rank = 0;
                            mark = null;
                            count = 1;
                            foreach (var t in examClass.OrderByDescending(d => d.t.p.SegmentMark))
                            {
                                if (t.t.p.SegmentMark == null)
                                {
                                    t.t.p.SegmentClassRank = null;
                                    continue;
                                }
                                if (mark != t.t.p.SegmentMark)
                                {
                                    mark = t.t.p.SegmentMark;
                                    rank = rank + count;
                                    count = 1;
                                }
                                else
                                {
                                    count = count + 1;
                                }

                                t.t.p.SegmentClassRank = rank;
                            }
                        }
                        #endregion 
                    }
                    else
                    {
                        #region 教学班
                        var orgExamMrkList = (from t in examMark
                                              join p in orgStudent
                                              on t.StudnetId equals p.StudentId
                                              select new
                                              {
                                                  t,
                                                  p.OrgId
                                              }).ToList();

                        var OrgIds = orgStudent.Select(d => d.OrgId).Distinct().ToList();
                        foreach (var orgId in OrgIds)
                        {
                            var examOrg = (from t in orgExamMrkList
                                           where t.OrgId == orgId
                                           select t).ToList();
                            var rank = 0;
                            decimal? mark = null;
                            var count = 1;
                            foreach (var t in examOrg.OrderByDescending(d => d.t.p.TotalMark))
                            {
                                if (t.t.p.TotalMark == null)
                                {
                                    t.t.p.TotalClassRank = null;
                                    continue;
                                }
                                if (mark != t.t.p.TotalMark)
                                {
                                    mark = t.t.p.TotalMark;
                                    rank = rank + count;
                                    count = 1;
                                }
                                else
                                {
                                    count = count + 1;
                                }

                                t.t.p.TotalClassRank = rank;
                            }

                            rank = 0;
                            mark = null;
                            count = 1;
                            foreach (var t in examOrg.OrderByDescending(d => d.t.p.SegmentMark))
                            {
                                if (t.t.p.SegmentMark == null)
                                {
                                    t.t.p.TotalClassRank = null;
                                    continue;
                                }
                                if (mark != t.t.p.SegmentMark)
                                {
                                    mark = t.t.p.SegmentMark;
                                    rank = rank + count;
                                    count = 1;
                                }
                                else
                                {
                                    count = count + 1;
                                }

                                t.t.p.SegmentClassRank = rank;
                            }
                        }
                        #endregion 
                    }

                }
                #endregion

                db.SaveChanges();
            }
        }

        #region 导入

        public ActionResult Import()
        {
            //todo:考试成绩和过程分应该可以导入排名？
            var vm = new Models.ExamMark.Import();
            vm.NameTypeList = new List<SelectListItem>() {
                new SelectListItem() { Value="1",Text="学号"},
                new SelectListItem() { Value="2",Text="身份证号"},
                new SelectListItem() { Value="3",Text="CMIS帐号"},
                new SelectListItem() { Value="4",Text="教育ID号"}
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(Models.ExamMark.Import vm)
        {
            if (ModelState.IsValid)
            {
                vm.NameTypeList = new List<SelectListItem>() {
                new SelectListItem() { Value="1",Text="学号"},
                new SelectListItem() { Value="2",Text="身份证号"},
                new SelectListItem() { Value="3",Text="CMIS帐号"},
                new SelectListItem() { Value="4",Text="教育ID号"}
            };
                var file = Request.Files[nameof(vm.UploadFile)];
                var fileSave = System.IO.Path.GetTempFileName();
                file.SaveAs(fileSave);

                using (var db = new XkSystem.Models.DbContext())
                {
                    if (Code.Common.GetFileType(file.FileName) != Code.FileType.Excel)
                    {
                        ModelState.AddModelError("", "上传的文件不是正确的EXCLE文件!");
                        return View(vm);
                    }
                    else
                    {
                        var dt = Code.NpoiHelper.ExcelToDataTable(fileSave, System.IO.Path.GetExtension(file.FileName), string.Empty);
                        if (dt == null)
                        {
                            ModelState.AddModelError("", "无法读取上传的文件，请检查文件格式是否正确!");
                            return View(vm);
                        }
                        else
                        {
                            var tbList = new List<string>() { "唯一标识", "姓名", "课程", "状态", "过程分", "考试成绩", "综合成绩", "等级" };

                            var Text = string.Empty;
                            foreach (var a in tbList)
                            {
                                if (!dt.Columns.Contains(a.ToString()))
                                {
                                    Text += a + ",";
                                }
                            }

                            if (!string.IsNullOrEmpty(Text))
                            {
                                ModelState.AddModelError("", "上传的EXCEL内容与预期不一致!错误详细:" + Text);
                                return View(vm);
                            }

                            //将DataTable转为List
                            foreach (System.Data.DataRow dr in dt.Rows)
                            {
                                var dto = new Dto.ExamMark.Import();
                                dto.NameTypeNo = dr["唯一标识"].ConvertToString();
                                dto.StudentName = dr["姓名"].ConvertToString();
                                dto.CourseName = dr["课程"].ConvertToString();
                                dto.ExamStatus = dr["状态"].ConvertToString();
                                dto.AppraiseMark = dr["过程分"].ConvertToDecimalWithNull();
                                dto.TotalMark = dr["考试成绩"].ConvertToDecimalWithNull();
                                dto.SegmentMark = dr["综合成绩"].ConvertToDecimalWithNull();
                                dto.LevelName = dr["等级"].ConvertToString();
                                vm.ImportList.Add(dto);
                            }

                            vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.NameTypeNo + d.StudentName));

                            if (vm.ImportList.Count == 0)
                            {
                                ModelState.AddModelError("", "未读取到任何有效数据!");
                                return View(vm);
                            }

                            var exam = (from p in db.Table<Exam.Entity.tbExam>()
                                        where p.Id == vm.ExamId
                                        select new
                                        {
                                            YearId = p.tbYear.Id,
                                        }).FirstOrDefault();

                            var examCourselist = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                                    .Include(d => d.tbCourse)
                                                    .Include(d => d.tbExamLevelGroup)
                                                  where p.tbExam.Id == vm.ExamId
                                                    && p.tbCourse.IsDeleted == false
                                                    && p.tbCourse.tbSubject.IsDeleted == false
                                                  select p).ToList();

                            var examStatusList = (from p in db.Table<Exam.Entity.tbExamStatus>()
                                                  orderby p.No
                                                  select p).ToList();

                            var examLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                                    .Include(d => d.tbExamLevelGroup)
                                                 where p.tbExamLevelGroup.IsDeleted == false
                                                 orderby p.No
                                                 select p).ToList();
                            //任课老师
                            var examPowerList = (from p in db.Table<Exam.Entity.tbExamPower>().Include(d => d.tbExamCourse)
                                              .Include(d => d.tbExamCourse.tbCourse)
                                              .Include(d => d.tbTeacher.tbSysUser)
                                                 where p.tbExamCourse.IsDeleted == false
                                                 && p.tbTeacher.IsDeleted == false
                                                 && p.tbExamCourse.tbExam.Id == vm.ExamId
                                                 && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                                 && p.IsOrgTeacher == true
                                                 select p).ToList();

                            var studentList = (from p in db.Table<Student.Entity.tbStudent>().Include(d => d.tbSysUser)
                                               select p).ToList();

                            //验证数据格式是否正确
                            foreach (var import in vm.ImportList)
                            {
                                if (vm.NameTypeId == 1)
                                {
                                    if (string.IsNullOrEmpty(import.NameTypeNo))
                                    {
                                        import.Error = import.Error + "学生学号不能为空!;";
                                    }

                                    if (string.IsNullOrEmpty(import.StudentName))
                                    {
                                        import.Error = import.Error + "学生姓名不能为空!;";
                                    }

                                    if (studentList.Where(d => d.StudentCode == import.NameTypeNo && d.StudentName == import.StudentName).Count() == 0)
                                    {
                                        import.Error = import.Error + "学生姓名和学生学号不匹配!;";
                                    }
                                }
                                else if (vm.NameTypeId == 2)
                                {
                                    if (string.IsNullOrEmpty(import.NameTypeNo))
                                    {
                                        import.Error = import.Error + "身份证号不能为空!;";
                                    }

                                    if (string.IsNullOrEmpty(import.StudentName))
                                    {
                                        import.Error = import.Error + "学生姓名不能为空!;";
                                    }

                                    if (studentList.Where(d => d.tbSysUser.IdentityNumber == import.NameTypeNo && d.StudentName == import.StudentName).Count() == 0)
                                    {
                                        import.Error = import.Error + "身份证号和学生姓名不匹配!;";
                                    }
                                }
                                else if (vm.NameTypeId == 3)
                                {
                                    if (string.IsNullOrEmpty(import.NameTypeNo))
                                    {
                                        import.Error = import.Error + "CMIS号不能为空!;";
                                    }

                                    if (string.IsNullOrEmpty(import.StudentName))
                                    {
                                        import.Error = import.Error + "学生姓名不能为空!;";
                                    }

                                    if (studentList.Where(d => d.CMIS == import.NameTypeNo && d.StudentName == import.StudentName).Count() == 0)
                                    {
                                        import.Error = import.Error + "CMIS号和学生姓名不匹配!;";
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(import.NameTypeNo))
                                    {
                                        import.Error = import.Error + "教育编号不能为空!;";
                                    }

                                    if (string.IsNullOrEmpty(import.StudentName))
                                    {
                                        import.Error = import.Error + "学生姓名不能为空!;";
                                    }

                                    if (studentList.Where(d => d.EduNo == import.NameTypeNo && d.StudentName == import.StudentName).Count() == 0)
                                    {
                                        import.Error = import.Error + "教育编号和学生姓名不匹配!;";
                                    }
                                }
                                if (examCourselist.Where(d => d.tbCourse.CourseName == import.CourseName).Any() == false)
                                {
                                    import.Error = import.Error + "考试课程不存在!;";
                                }
                                else
                                {
                                    if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator)
                                    {
                                        if (examPowerList.Count() > 0)//任课老师
                                        {
                                            var inputCourse = examPowerList.Where(d => d.tbExamCourse.tbCourse.CourseName == import.CourseName).FirstOrDefault();
                                            if (inputCourse == null)
                                            {
                                                import.Error = import.Error + string.Format("任课老师无权限导入该({0})课程;", import.CourseName);
                                            }
                                        }
                                    }
                                    var examCourse = examCourselist.Where(d => d.tbCourse.CourseName == import.CourseName).FirstOrDefault();
                                    if (string.IsNullOrEmpty(import.LevelName) == false && examLevelList.Where(d => d.tbExamLevelGroup.Id == examCourse.tbExamLevelGroup.Id && d.ExamLevelName == import.LevelName).Any() == false)
                                    {
                                        import.Error = import.Error + "等级不存在!;";
                                    }

                                    if (import.AppraiseMark > examCourse.FullAppraiseMark)
                                    {
                                        import.Error = import.Error + string.Format("过程分超过满分值({0};)", examCourse.FullAppraiseMark);
                                    }

                                    if (import.TotalMark > examCourse.FullTotalMark)
                                    {
                                        import.Error = import.Error + string.Format("考试成绩超过满分值({0});", examCourse.FullTotalMark);
                                    }

                                    if (import.SegmentMark > examCourse.FullSegmentMark)
                                    {
                                        import.Error = import.Error + string.Format("综合成绩分超过满分值({0});", examCourse.FullSegmentMark);
                                    }
                                }

                                if (string.IsNullOrEmpty(import.ExamStatus) == false && examStatusList.Where(d => d.ExamStatusName == import.ExamStatus).Any() == false)
                                {
                                    import.Error = import.Error + "考生状态不存在!;";
                                }
                            }

                            if (vm.ImportList.Where(d => string.IsNullOrEmpty(d.Error) == false).Count() > 0)
                            {
                                vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                return View(vm);
                            }

                            var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                                .Include(d => d.tbStudent)
                                                .Include(d => d.tbStudent.tbSysUser)
                                                .Include(d => d.tbExamCourse)
                                                where (p.tbExamCourse.tbCourse.Id == vm.CourseId || vm.CourseId == 0)
                                                 && p.tbExamCourse.IsDeleted == false
                                                 && (p.tbExamCourse.tbExam.Id == vm.ExamId || vm.ExamId == 0)
                                                select p).ToList();

                            var list = new List<Exam.Entity.tbExamMark>();
                            foreach (var import in vm.ImportList)
                            {
                                var levelGroupId = examCourselist.Where(d => d.tbCourse.CourseName == import.CourseName).Select(d => d.tbExamLevelGroup.Id).FirstOrDefault();
                                var examCourseId = examCourselist.Where(d => d.tbCourse.CourseName == import.CourseName).Select(d => d.Id).FirstOrDefault();

                                if (vm.NameTypeId == 1)
                                {
                                    var tb = (from p in examMarkList
                                              where p.tbStudent.StudentCode == import.NameTypeNo
                                               && p.tbExamCourse.Id == examCourseId
                                              select p).FirstOrDefault();
                                    if (tb == null)
                                    {
                                        var tf = new Exam.Entity.tbExamMark();
                                        tf.tbStudent = studentList.Where(d => d.StudentCode == import.NameTypeNo).FirstOrDefault();
                                        tf.tbExamCourse = examCourselist.Where(d => d.Id == examCourseId).FirstOrDefault();
                                        tf.tbExamStatus = examStatusList.Where(d => d.ExamStatusName == import.ExamStatus).FirstOrDefault();
                                        tf.tbExamLevel = examLevelList.Where(d => d.ExamLevelName == import.LevelName && d.tbExamLevelGroup.Id == levelGroupId).FirstOrDefault();
                                        tf.AppraiseMark = import.AppraiseMark;
                                        tf.TotalMark = import.TotalMark;
                                        tf.SegmentMark = import.SegmentMark;

                                        list.Add(tf);
                                    }
                                    else
                                    {
                                        tb.tbExamStatus = examStatusList.Where(d => d.ExamStatusName == import.ExamStatus).FirstOrDefault();
                                        tb.tbExamLevel = examLevelList.Where(d => d.ExamLevelName == import.LevelName && d.tbExamLevelGroup.Id == levelGroupId).FirstOrDefault();
                                        tb.AppraiseMark = import.AppraiseMark;
                                        tb.TotalMark = import.TotalMark;
                                        tb.SegmentMark = import.SegmentMark;
                                    }
                                }
                                else if (vm.NameTypeId == 2)
                                {
                                    var tb = (from p in examMarkList
                                              where p.tbExamCourse.Id == examCourseId
                                               && p.tbStudent.tbSysUser.IdentityNumber == import.NameTypeNo
                                              select p).FirstOrDefault();
                                    if (tb == null)
                                    {
                                        var tf = new Exam.Entity.tbExamMark();
                                        tf.tbStudent = studentList.Where(d => d.tbSysUser.IdentityNumber == import.NameTypeNo).FirstOrDefault();
                                        tf.tbExamCourse = examCourselist.Where(d => d.Id == examCourseId).FirstOrDefault();
                                        tf.tbExamStatus = examStatusList.Where(d => d.ExamStatusName == import.ExamStatus).FirstOrDefault();
                                        tf.tbExamLevel = examLevelList.Where(d => d.ExamLevelName == import.LevelName && d.tbExamLevelGroup.Id == levelGroupId).FirstOrDefault();
                                        tf.AppraiseMark = import.AppraiseMark;
                                        tf.TotalMark = import.TotalMark;
                                        tf.SegmentMark = import.SegmentMark;

                                        list.Add(tf);
                                    }
                                    else
                                    {
                                        tb.tbExamStatus = examStatusList.Where(d => d.ExamStatusName == import.ExamStatus).FirstOrDefault();
                                        tb.tbExamLevel = examLevelList.Where(d => d.ExamLevelName == import.LevelName && d.tbExamLevelGroup.Id == levelGroupId).FirstOrDefault();
                                        tb.AppraiseMark = import.AppraiseMark;
                                        tb.TotalMark = import.TotalMark;
                                        tb.SegmentMark = import.SegmentMark;
                                    }
                                }
                                else if (vm.NameTypeId == 2)
                                {
                                    var tb = (from p in examMarkList
                                              where p.tbStudent.CMIS == import.NameTypeNo
                                               && p.tbExamCourse.Id == examCourseId
                                              select p).FirstOrDefault();
                                    if (tb == null)
                                    {
                                        var tf = new Exam.Entity.tbExamMark();
                                        tf.tbStudent = studentList.Where(d => d.CMIS == import.NameTypeNo).FirstOrDefault();
                                        tf.tbExamCourse = examCourselist.Where(d => d.Id == examCourseId).FirstOrDefault();
                                        tf.tbExamStatus = examStatusList.Where(d => d.ExamStatusName == import.ExamStatus).FirstOrDefault();
                                        tf.tbExamLevel = examLevelList.Where(d => d.ExamLevelName == import.LevelName && d.tbExamLevelGroup.Id == levelGroupId).FirstOrDefault();
                                        tf.AppraiseMark = import.AppraiseMark;
                                        tf.TotalMark = import.TotalMark;
                                        tf.SegmentMark = import.SegmentMark;

                                        list.Add(tf);
                                    }
                                    else
                                    {
                                        tb.tbExamStatus = examStatusList.Where(d => d.ExamStatusName == import.ExamStatus).FirstOrDefault();
                                        tb.tbExamLevel = examLevelList.Where(d => d.ExamLevelName == import.LevelName && d.tbExamLevelGroup.Id == levelGroupId).FirstOrDefault();
                                        tb.AppraiseMark = import.AppraiseMark;
                                        tb.TotalMark = import.TotalMark;
                                        tb.SegmentMark = import.SegmentMark;
                                    }
                                }
                                else
                                {
                                    var tb = (from p in examMarkList
                                              where p.tbStudent.EduNo == import.NameTypeNo
                                               && p.tbExamCourse.Id == examCourseId
                                              select p).FirstOrDefault();
                                    if (tb == null)
                                    {
                                        var tf = new Exam.Entity.tbExamMark();
                                        tf.tbStudent = studentList.Where(d => d.EduNo == import.NameTypeNo).FirstOrDefault();
                                        tf.tbExamCourse = examCourselist.Where(d => d.Id == examCourseId).FirstOrDefault();
                                        tf.tbExamStatus = examStatusList.Where(d => d.ExamStatusName == import.ExamStatus).FirstOrDefault();
                                        tf.tbExamLevel = examLevelList.Where(d => d.ExamLevelName == import.LevelName && d.tbExamLevelGroup.Id == levelGroupId).FirstOrDefault();
                                        tf.AppraiseMark = import.AppraiseMark;
                                        tf.TotalMark = import.TotalMark;
                                        tf.SegmentMark = import.SegmentMark;

                                        list.Add(tf);
                                    }
                                    else
                                    {
                                        tb.tbExamStatus = examStatusList.Where(d => d.ExamStatusName == import.ExamStatus).FirstOrDefault();
                                        tb.tbExamLevel = examLevelList.Where(d => d.ExamLevelName == import.LevelName && d.tbExamLevelGroup.Id == levelGroupId).FirstOrDefault();
                                        tb.AppraiseMark = import.AppraiseMark;
                                        tb.TotalMark = import.TotalMark;
                                        tb.SegmentMark = import.SegmentMark;
                                    }
                                }
                            }

                            db.Set<Exam.Entity.tbExamMark>().AddRange(list);

                            if (db.SaveChanges() > decimal.Zero)
                            {
                                if (vm.IsAutoRank)
                                {
                                    //todo:应该是对导入的数据重新生成排名，而不应该是全部重新生成!
                                    foreach (var course in examCourselist)
                                    {
                                        BuildRank(vm.ExamId, course.tbCourse.Id);
                                    }
                                }

                                vm.Status = true;
                                vm.ImportList.RemoveAll(d => string.IsNullOrEmpty(d.Error));
                                XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("导入考试成绩");
                            }
                            else
                            {
                                ModelState.AddModelError("", "导入失败，未有任何数据保存!");
                                return View(vm);
                            }
                        }
                    }
                }
            }

            return View(vm);
        }

        public ActionResult ImportTemplate()
        {
            var file = Server.MapPath("~/Areas/Exam/Views/ExamMark/ExamMarkTemplate.xlsx");
            return File(file, Code.Common.DownloadType, System.IO.Path.GetFileName(file));
        }

        public ActionResult Export(int examId, int CourseId, int OrgId, string SearchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();
                var org = (from p in db.Table<Course.Entity.tbOrg>()
                           where p.Id == OrgId
                           select new
                           {
                               p.tbYear.tbYearParent.tbYearParent.Id,
                               YearId = p.tbYear.Id,
                               p.IsClass,
                               ClassId = p.tbClass != null ? p.tbClass.Id : 0
                           }).FirstOrDefault();
                //班级学生
                var lstStudent = new List<Dto.ExamMark.List>();
                if (org == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                if (org.IsClass)
                {
                    lstStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  where p.tbClass.Id == org.ClassId
                                  && p.tbClass.tbYear.Id == org.Id
                                  && p.tbStudent.IsDeleted == false
                                  orderby p.No, p.tbStudent.StudentCode
                                  select new Dto.ExamMark.List
                                  {
                                      Id = p.tbStudent.Id,
                                      StudentCode = p.tbStudent.StudentCode,
                                      StudentName = p.tbStudent.StudentName
                                  }).ToList();
                }
                else
                {
                    lstStudent = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                  where p.tbOrg.Id == OrgId
                                  orderby p.No, p.tbStudent.StudentCode
                                  select new Dto.ExamMark.List
                                  {
                                      Id = p.tbStudent.Id,
                                      StudentCode = p.tbStudent.StudentCode,
                                      StudentName = p.tbStudent.StudentName
                                  }).ToList();
                }

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where (SearchText == null || p.tbExamCourse.tbCourse.CourseName.Contains(SearchText))
                           && (p.tbExamCourse.tbCourse.Id == CourseId || CourseId == 0)
                           && p.tbExamCourse.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == examId || examId == 0)
                          select new
                          {
                              p.tbStudent.Id,
                              p.tbStudent.StudentCode,
                              p.tbStudent.StudentName,
                              p.tbExamLevel.ExamLevelName,
                              p.tbExamStatus.ExamStatusName,
                              p.AppraiseMark,
                              p.TotalMark,
                              p.SegmentMark,
                              p.tbExamCourse.tbCourse.CourseName
                          }).ToList();

                var tb = (from p in lstStudent
                          join q in tf on p.Id equals q.Id into list
                          from temp in list.DefaultIfEmpty()
                          select new
                          {
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              ExamLevelName = temp != null ? temp.ExamLevelName : string.Empty,
                              ExamStatusName = temp != null ? temp.ExamStatusName : string.Empty,
                              AppraiseMark = temp != null ? temp.AppraiseMark : null,
                              TotalMark = temp != null ? temp.TotalMark : null,
                              SegmentMark = temp != null ? temp.SegmentMark : null,
                              CourseName = temp != null ? temp.CourseName : db.Table<Course.Entity.tbCourse>().Where(d => d.Id == CourseId).Select(d => d.CourseName).FirstOrDefault()
                          }).ToList();

                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("唯一标识"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("课程"),
                        new System.Data.DataColumn("状态"),
                        new System.Data.DataColumn("过程分"),
                        new System.Data.DataColumn("考试成绩"),
                        new System.Data.DataColumn("综合成绩"),
                        new System.Data.DataColumn("等级")
                    });
                foreach (var a in tb)
                {
                    var dr = dt.NewRow();
                    dr["唯一标识"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["课程"] = a.CourseName;
                    dr["状态"] = a.ExamStatusName;
                    dr["过程分"] = a.AppraiseMark;
                    dr["考试成绩"] = a.TotalMark;
                    dr["综合成绩"] = a.SegmentMark;
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
                    return View();
                }
            }
        }
        #endregion

        public ActionResult TagList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamMark.TagList();
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectPublishList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                if (vm.ExamId != 0)
                {
                    vm.ExamCourseList = Areas.Exam.Controllers.ExamCourseController.GetExamCourseList(vm.ExamId);
                    if (vm.ExamCourseId != 0 && vm.ExamCourseList.Where(d => d.Value == vm.ExamCourseId.ToString()).Any() == false)
                    {
                        vm.ExamCourseId = 0;
                    }

                    vm.ExamTagList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                      where p.tbExamCourse.tbExam.Id == vm.ExamId
                                       && (p.tbExamCourse.Id == vm.ExamCourseId || vm.ExamCourseId == 0)
                                       && p.tbExamCourse.IsDeleted == false
                                       && p.tbExamCourse.tbCourse.IsDeleted == false
                                       && (p.tbExamStatus != null && p.tbExamStatus.ExamStatusName.Contains("正常") == false)
                                      select new Dto.ExamMark.TagList
                                      {
                                          Id = p.Id,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName,
                                          CourseName = p.tbExamCourse.tbCourse.CourseName,
                                          ExamStatusName = p.tbExamStatus.ExamStatusName
                                      }).ToList();
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TagList(Models.ExamMark.TagList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("TagList", new { ExamId = vm.ExamId, ExamCourseId = vm.ExamCourseId, searchText = vm.SearchText }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TagDelete(List<int> ids)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tb = (from p in db.Table<Exam.Entity.tbExamMark>()
                            .Include(d => d.tbExamStatus)
                          where ids.Contains(p.Id)
                          select p).ToList();

                foreach (var a in tb)
                {
                    a.tbExamStatus = null;
                }

                if (db.SaveChanges() > 0)
                {
                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("回复成绩类型!");
                }

                return Code.MvcHelper.Post();
            }
        }

        public ActionResult TagEdit(int id = 0)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamMark.TagEdit();
                vm.ExamCourseList = Areas.Exam.Controllers.ExamCourseController.GetExamCourseList(vm.ExamId);
                vm.ExamStatusList = Exam.Controllers.ExamStatusController.SelectList();

                if (id != 0)
                {
                    var tb = (from p in db.Table<Exam.Entity.tbExamMark>()
                              where p.Id == id
                              select new Dto.ExamMark.TagEdit
                              {
                                  Id = p.Id,
                                  StudentCode = p.tbStudent.StudentCode,
                                  StudentName = p.tbStudent.StudentName,
                                  ExamCourseId = p.tbExamCourse.Id,
                                  ExamStatusId = p.tbExamStatus.Id
                              }).FirstOrDefault();
                    if (tb != null)
                    {
                        vm.ExamTagEdit = tb;
                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TagEdit(Models.ExamMark.TagEdit vm)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var error = new List<string>();
                if (error.Count == decimal.Zero)
                {
                    if (vm.ExamTagEdit.Id == 0)
                    {
                        if (string.IsNullOrEmpty(vm.ExamTagEdit.StudentCode) == false)
                        {
                            var student = (from p in db.Table<Student.Entity.tbStudent>()
                                           where p.StudentCode == vm.ExamTagEdit.StudentCode
                                            && p.IsDeleted == false
                                           select p).FirstOrDefault();
                            if (student != null)
                            {
                                var mark = (from p in db.Table<Exam.Entity.tbExamMark>()
                                            where p.tbStudent.StudentCode == vm.ExamTagEdit.StudentCode
                                                && p.tbExamCourse.Id == vm.ExamTagEdit.ExamCourseId
                                                && p.tbStudent.IsDeleted == false
                                            select p).FirstOrDefault();
                                if (mark == null)
                                {
                                    var tb = new Exam.Entity.tbExamMark();
                                    tb.tbStudent = student;
                                    tb.tbExamCourse = db.Set<Exam.Entity.tbExamCourse>().Find(vm.ExamTagEdit.ExamCourseId);
                                    tb.tbExamStatus = db.Set<Exam.Entity.tbExamStatus>().Find(vm.ExamTagEdit.ExamStatusId);
                                    db.Set<Exam.Entity.tbExamMark>().Add(tb);
                                }
                                else
                                {
                                    mark.tbExamStatus = db.Set<Exam.Entity.tbExamStatus>().Find(vm.ExamTagEdit.ExamStatusId);
                                }

                                if (db.SaveChanges() > 0)
                                {
                                    XkSystem.Areas.Sys.Controllers.SysUserLogController.Insert("添加考试异常人员!");
                                }
                            }
                            else
                            {
                                error.AddError("未查询到该学号所对应的学生信息!");
                            }
                        }
                    }
                    else
                    {
                        var tb = (from p in db.Table<Exam.Entity.tbExamMark>()
                                  where p.Id == vm.ExamTagEdit.Id
                                  select p).FirstOrDefault();
                        if (tb != null)
                        {
                            tb.tbExamCourse = db.Set<Exam.Entity.tbExamCourse>().Find(vm.ExamTagEdit.ExamCourseId);
                            tb.tbExamStatus = db.Set<Exam.Entity.tbExamStatus>().Find(vm.ExamTagEdit.ExamStatusId);
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