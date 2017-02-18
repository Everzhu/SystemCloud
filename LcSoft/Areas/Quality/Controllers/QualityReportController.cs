using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Quality.Controllers
{
    public class QualityReportController : Controller
    {
        public ActionResult Grade()
        {
            return View();
        }

        public ActionResult Class()
        {
            return View();
        }

        public ActionResult Student()
        {
            return View();
        }

        #region 学生综合素质报告单
        public ActionResult StudentReport()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Student)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityReport.StudentReport();
                //根据当前登录用户Id获取学生信息
                var student = (from p in db.Table<Basis.Entity.tbClassStudent>()
                               .Include(d => d.tbClass)
                               .Include(d => d.tbStudent)
                               .Include(d => d.tbClass.tbYear)
                               where p.IsDeleted == false
                               && p.tbClass.IsDeleted == false
                               && p.tbStudent.IsDeleted == false
                               && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                               select p).FirstOrDefault();
                if (student != null)
                {
                    //根据班级Id获取班主任信息
                    var teacher = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                               .Include(d => d.tbClass)
                               .Include(d => d.tbTeacher)
                                   where p.IsDeleted == false
                                   && p.tbClass.IsDeleted == false
                                   && p.tbTeacher.IsDeleted == false
                                   && p.tbClass.Id == student.tbClass.Id
                                   select p).FirstOrDefault();
                    vm.ClassName = student.tbClass.ClassName;
                    vm.StudentCode = student.tbStudent.StudentCode;
                    vm.StudentName = student.tbStudent.StudentName;
                    if (teacher != null)
                    {
                        vm.ClassTeacher = teacher.tbTeacher.TeacherName;
                    }

                    //根据学生Id获取年级
                    vm.GradeYearList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  .Include(d => d.tbClass.tbGrade)
                                  .Include(d => d.tbClass.tbYear)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        orderby p.tbClass.tbYear.No descending
                                        select p).ToList();
                    //获取学年ID
                    var yearIdList = vm.GradeYearList.Select(d => d.tbClass.tbYear.Id).ToList();
                    //获取学期
                    vm.YearTermList = (from p in db.Table<Basis.Entity.tbYear>()
                                        .Include(d => d.tbYearParent)
                                       where p.IsDeleted == false
                                       && yearIdList.Contains(p.tbYearParent.Id)
                                       select p).ToList();
                    var yearTermIdList = vm.YearTermList.Select(d => d.Id).ToList();
                    vm.YearSectionList = (from p in db.Table<Basis.Entity.tbYear>()
                                          .Include(d => d.tbYearParent)
                                          where p.IsDeleted == false
                                          && yearTermIdList.Contains(p.tbYearParent.Id)
                                          select p).ToList();
                    //获取年级
                    vm.GradeList = (from p in vm.GradeYearList
                                    orderby p.tbClass.tbGrade.No descending
                                    select new System.Web.Mvc.SelectListItem
                                    {
                                        Text = p.tbClass.tbGrade.GradeName,
                                        Value = p.tbClass.tbGrade.Id.ToString(),
                                    }).ToList();

                    //默认第一个年级Id去查询后面数据
                    if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                    {
                        vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                    }

                    //根据当前登录用户Id和年级Id获取学生信息
                    var gradeStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                        .Include(d => d.tbStudent)
                              .Include(d => d.tbClass.tbGrade)
                              .Include(d => d.tbClass.tbYear)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        && p.tbClass.tbGrade.Id == vm.GradeId
                                        select p).FirstOrDefault();

                    //根据状态获取学期
                    if (gradeStudent != null)
                    {
                        decimal maxTotal = 0;
                        //获取学期
                        var yearTermList = (from p in db.Table<Basis.Entity.tbYear>()
                                            where p.tbYearParent.Id == gradeStudent.tbClass.tbYear.Id
                                            && p.IsDeleted == false
                                            orderby p.No
                                            select p).ToList();

                        //获取学段
                        if (vm.YearTermId == 0)
                        {
                            var yearSection = vm.YearSectionList.Where(d => d.IsDefault == true).FirstOrDefault();
                            if (yearSection != null)
                            {
                                vm.YearTermId = yearSection.tbYearParent.Id;
                            }
                        }
                        var yearSectionList = (from p in db.Table<Basis.Entity.tbYear>()
                                              .Include(d => d.tbYearParent)
                                               where p.tbYearParent.Id == vm.YearTermId
                                               && p.IsDeleted == false
                                               orderby p.No
                                               select p).ToList();
                        var yearSectionIdList = yearSectionList.Select(d => d.Id).ToList();

                        #region 成绩等级
                        //获取课程分组
                        vm.CourseGroupItemList = (from p in db.Table<Course.Entity.tbCourseGroup>()
                                                  where p.IsDeleted == false
                                                  orderby p.No
                                                  select new System.Web.Mvc.SelectListItem
                                                  {
                                                      Text = p.CourseGroupName,
                                                      Value = p.Id.ToString(),
                                                  }).ToList();
                        var courseGroupIdList = vm.CourseGroupItemList.Select(d => d.Value).ToList();

                        //获取领域
                        vm.CourseDomainItemList = (from p in db.Table<Course.Entity.tbCourseDomain>()
                                                   where p.IsDeleted == false
                                                   orderby p.No
                                                   select p).ToList();
                        var courseDomainItemIdList = vm.CourseDomainItemList.Select(d => d.Id).ToList();

                        //获取教学班课程、分组、领域信息
                        vm.CourseDomainList = (from p in db.Table<Course.Entity.tbOrg>()
                                               where p.IsDeleted == false
                                               && courseDomainItemIdList.Contains(p.tbCourse.tbCourseDomain.Id)
                                               && (vm.CourseGroupItemId <= 0 ? true : p.tbCourse.tbCourseGroup.Id == vm.CourseGroupItemId)
                                               && p.tbGrade.Id == vm.GradeId
                                               orderby p.tbCourse.tbCourseDomain.No
                                               select new Dto.QualityReport.StudentReport
                                               {
                                                   CourseId = p.tbCourse.Id,
                                                   CourseName = p.tbCourse.CourseName,
                                                   CourseGroupId = p.tbCourse.tbCourseGroup.Id,
                                                   CourseGroupName = p.tbCourse.tbCourseGroup.CourseGroupName,
                                                   CourseDomainName = p.tbCourse.tbCourseDomain.CourseDomainName,
                                                   CourseDomainNo = p.tbCourse.tbCourseDomain.No,
                                                   CourseDomainId = p.tbCourse.tbCourseDomain.Id,
                                               }).Distinct().ToList();

                        var courseIdList = vm.CourseDomainList.Select(d => d.CourseId).ToList();
                        var courseDomainIdList = vm.CourseDomainList.Select(d => d.CourseDomainId).Distinct().ToList();
                        var courseDomainNameList = vm.CourseDomainList.Select(d => d.CourseDomainName).Distinct().ToList();

                        //考试
                        vm.ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                       where p.IsPublish == true
                                       && p.IsDeleted == false
                                       && yearSectionIdList.Contains(p.tbYear.Id)
                                       select p).ToList();
                        var examIdList = vm.ExamList.Select(d => d.Id).ToList();

                        if (vm.ExamList.Count() > 0)
                        {
                            #region 获取个人积点
                            //获取个人积点
                            vm.ExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                               where p.tbExamCourse.IsDeleted == false
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbExamCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                                && p.tbExamCourse.tbExam.IsDeleted == false
                                                && examIdList.Contains(p.tbExamCourse.tbExam.Id)
                                                && courseIdList.Contains(p.tbExamCourse.tbCourse.Id)
                                                && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                               select new Dto.QualityReport.StudentReport
                                               {
                                                   CourseGroupId = p.tbExamCourse.tbCourse.tbCourseGroup.Id,
                                                   ExamId = p.tbExamCourse.tbExam.Id,
                                                   TotalMark = p.TotalMark,
                                                   CourseId = p.tbExamCourse.tbCourse.Id,
                                                   LevelName = p.tbExamLevel != null ? p.tbExamLevel.ExamLevelName : string.Empty,
                                                   LevelScore = p.tbExamLevel != null ? p.tbExamLevel.ExamLevelValue : 0,
                                                   CourseName = p.tbExamCourse.tbCourse.CourseName,
                                                   CourseDomainId = p.tbExamCourse.tbCourse.tbCourseDomain.Id,
                                               }).ToList();

                            //获取领域学生积点总分
                            var studentExamMarkList = (from p in vm.ExamMarkList
                                                       group p by new { p.CourseDomainId, p.ExamId } into g
                                                       select new
                                                       {
                                                           ExamId = g.Key.ExamId,
                                                           CourseDomainId = g.Key.CourseDomainId,
                                                           StudentScore = g.Sum(d => d.LevelScore),
                                                       }).ToList();
                            #endregion

                            #region 获取班级学生积点
                            //获取班级学生
                            var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                            .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass.tbGrade)
                                  .Include(d => d.tbClass.tbYear)
                                                    where p.IsDeleted == false
                                                    && p.tbClass.IsDeleted == false
                                                    && p.tbStudent.IsDeleted == false
                                                    && p.tbClass.Id == gradeStudent.tbClass.Id
                                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                                    select p).ToList();
                            //获取领域班级学生积点总分
                            var studentIdList = classStudentList.Select(d => d.tbStudent.Id).ToList();
                            var classExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                                     where p.tbExamCourse.IsDeleted == false
                                                      && p.tbStudent.IsDeleted == false
                                                      && p.tbExamCourse.IsDeleted == false
                                                      && p.tbExamCourse.tbCourse.IsDeleted == false
                                                      && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                                      && p.tbExamCourse.tbExam.IsDeleted == false
                                                      && examIdList.Contains(p.tbExamCourse.tbExam.Id)
                                                      && courseIdList.Contains(p.tbExamCourse.tbCourse.Id)
                                                      && studentIdList.Contains(p.tbStudent.Id)
                                                     group p by new { p.tbStudent.Id, CourseDomainId = p.tbExamCourse.tbCourse.tbCourseDomain.Id, ExamId = p.tbExamCourse.tbExam.Id } into g
                                                     select new
                                                     {
                                                         ExamId = g.Key.ExamId,
                                                         StudentId = g.Key.Id,
                                                         CourseDomainId = g.Key.CourseDomainId,
                                                         ClassScore = g.Sum(d => d.tbExamLevel.ExamLevelValue),
                                                     }).ToList();
                            //获取领域班级积点平均分
                            var classSumList = (from p in classExamMarkList
                                                group p by new { p.StudentId, p.CourseDomainId, p.ExamId } into g
                                                select new
                                                {
                                                    ExamId = g.Key.ExamId,
                                                    CourseDomainId = g.Key.CourseDomainId,
                                                    ClassScore = g.Sum(d => d.ClassScore),
                                                }).ToList();
                            //获取领域积点平均分
                            var classAvgList = (from p in classSumList
                                                group p by new { p.CourseDomainId, p.ExamId } into g
                                                select new
                                                {
                                                    ExamId = g.Key.ExamId,
                                                    CourseDomainId = g.Key.CourseDomainId,
                                                    ClassAvg = g.Average(d => d.ClassScore),
                                                }).ToList();

                            //获取每个学生积点总分
                            var classStudentMarkList = (from p in classExamMarkList
                                                        group p by new { p.StudentId, p.ExamId } into g
                                                        select new
                                                        {
                                                            ExamId = g.Key.ExamId,
                                                            StudentId = g.Key.StudentId,
                                                            ClassScore = g.Sum(d => d.ClassScore),
                                                        }).ToList();
                            //获取班级积点平均分
                            vm.ClassLevelAvgList = (from p in classStudentMarkList
                                                    join t in classStudentList on p.StudentId equals t.tbStudent.Id
                                                    group p by new { t.tbClass.Id, p.ExamId } into g
                                                    select new Dto.QualityReport.StudentReport
                                                    {
                                                        ExamId = g.Key.ExamId,
                                                        ClassLevelAvg = g.Average(d => d.ClassScore),
                                                    }).ToList();
                            #endregion

                            #region 绑定雷达图数据
                            maxTotal = 0;
                            foreach (var exam in vm.ExamList)
                            {
                                var demainList = new List<object>();
                                var sList = new List<object>();
                                var cList = new List<object>();
                                var gList = new List<object>();
                                var tList = new List<object>();
                                var i = 0;
                                //maxTotal = studentExamMarkList.Where(d=>d.ExamId==exam.Id).Sum(d => d.StudentScore);
                                //sList.Add(maxTotal.ToString("0.00"));
                                foreach (var courseDomainItem in vm.CourseDomainItemList)
                                {
                                    var studentTotal = studentExamMarkList.Where(d => d.CourseDomainId == courseDomainItem.Id && d.ExamId == exam.Id).FirstOrDefault();

                                    var classTotal = classAvgList.Where(d => d.CourseDomainId == courseDomainItem.Id && d.ExamId == exam.Id).FirstOrDefault();
                                    if (i == 0)
                                    {
                                        if (studentExamMarkList.Where(d => d.CourseDomainId == courseDomainItem.Id && d.ExamId == exam.Id).Sum(d => d.StudentScore) > maxTotal)
                                        {
                                            maxTotal = studentExamMarkList.Where(d => d.CourseDomainId == courseDomainItem.Id && d.ExamId == exam.Id).Sum(d => d.StudentScore);
                                        }
                                        sList.Add(studentExamMarkList.Where(d => d.ExamId == exam.Id).Sum(d => d.StudentScore).ToString("0.00"));
                                        if (studentTotal != null)
                                        {
                                            sList.Add(studentTotal.StudentScore.ToString("0.00"));
                                        }
                                        else
                                        {
                                            sList.Add(0);
                                        }
                                        if (classAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.ClassAvg) > maxTotal)
                                        {
                                            maxTotal = classAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.ClassAvg);
                                        }
                                        cList.Add(classAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.ClassAvg).ToString("0.00"));
                                        if (classTotal != null)
                                        {
                                            cList.Add(classTotal.ClassAvg.ToString("0.00"));
                                        }
                                        else
                                        {
                                            cList.Add(0);
                                        }

                                    }
                                    else
                                    {
                                        if (studentTotal != null)
                                        {
                                            if (studentTotal.StudentScore > maxTotal)
                                            {
                                                maxTotal = studentTotal.StudentScore;
                                            }
                                            sList.Add(studentTotal.StudentScore.ToString("0.00"));
                                        }
                                        else
                                        {
                                            sList.Add("0.00");
                                        }

                                        if (classTotal != null)
                                        {
                                            if (classTotal.ClassAvg > maxTotal)
                                            {
                                                maxTotal = classTotal.ClassAvg;
                                            }
                                            cList.Add(classTotal.ClassAvg.ToString("0.00"));
                                        }
                                        else
                                        {
                                            cList.Add(0);
                                        }
                                    }
                                    i++;
                                }
                                demainList.Add(new { name = "个人", value = sList });
                                demainList.Add(new { name = "班级均分", value = cList });
                                var courseDemainAvg = new Dto.QualityReport.StudentReport();
                                courseDemainAvg.ReportName = Code.Common.ToJSONString(demainList);
                                courseDemainAvg.ExamId = exam.Id;
                                vm.CourseDemainAvgList.Add(courseDemainAvg);

                            }
                            var courseDemainList = new List<object>();
                            //courseDemainList.Add(new { name = "总评", max = (maxTotal > 0 ? maxTotal : 5) });
                            courseDemainList.Add(new { name = "总评", max = 100 });
                            foreach (var courseDemainName in courseDomainNameList)
                            {
                                //courseDemainList.Add(new { name = courseDemainName, max = (maxTotal > 0 ? maxTotal : 5) });
                                courseDemainList.Add(new { name = courseDemainName, max = 100 });
                            }
                            vm.CourseDemainNames = Code.Common.ToJSONString(courseDemainList);
                            #endregion
                        }
                        #endregion

                        #region 荣誉
                        //获取荣誉
                        vm.HonorList = (from p in db.Table<Student.Entity.tbStudentHonor>()
                                                .Include(d => d.tbstudentHonorLevel)
                                                .Include(d => d.tbStudentHonorType)
                                        where p.IsDeleted == false
                                        && p.tbstudentHonorLevel.IsDeleted == false
                                        && p.tbStudentHonorType.IsDeleted == false
                                        && p.tbYear.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && yearSectionIdList.Contains(p.tbYear.Id)
                                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select new Dto.QualityReport.StudentReport
                                        {
                                            HonorName = p.HonorName,
                                            StudentHonorLevelName = p.tbstudentHonorLevel.StudentHonorLevelName,
                                            StudentHonorTypeName = p.tbStudentHonorType.StudentHonorTypeName,
                                            InputDate = p.InputDate,
                                        }).ToList();
                        #endregion

                        #region 获取评价
                        //获取评价
                        vm.QualityList = (from p in db.Table<Quality.Entity.tbQuality>()
                                          where p.IsDeleted == false
                                          && yearSectionIdList.Contains(p.tbYear.Id)
                                          && p.IsOpen == true
                                          orderby p.No
                                          select p).ToList();
                        var qualityIdList = vm.QualityList.Select(d => d.Id).ToList();
                        //获取评价分组
                        var qualityItemGroupList = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                                                    where p.IsDeleted == false
                                                    && p.tbQuality.IsDeleted == false
                                                    && qualityIdList.Contains(p.tbQuality.Id)
                                                    select p).ToList();

                        var qualityItemGroupIdList = qualityItemGroupList.Select(d => d.Id).ToList();

                        //获取评价项
                        var qualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                               where p.IsDeleted == false
                                               && p.tbQualityItemGroup.IsDeleted == false
                                               && qualityItemGroupIdList.Contains(p.tbQualityItemGroup.Id)
                                               select p).ToList();

                        var qualityItemIdList = qualityItemList.Select(d => d.Id).ToList();

                        //获取评价选项
                        var qualityOtionList = (from p in db.Table<Quality.Entity.tbQualityOption>()
                                                where p.IsDeleted == false
                                                && p.tbQualityItem.IsDeleted == false
                                                && qualityItemIdList.Contains(p.tbQualityItem.Id)
                                                select p).ToList();

                        var qualityOtionIdList = qualityOtionList.Select(d => d.Id).ToList();

                        vm.QualityDataList = (from p in db.Table<Quality.Entity.tbQualityData>()
                                              where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                               && qualityItemIdList.Contains(p.tbQualityItem.Id)
                                                && qualityOtionIdList.Contains(p.tbQualityOption.Id)
                                                && (p.tbQualityItem.QualityItemType == Code.EnumHelper.QualityItemType.Radio ||
                                                p.tbQualityItem.QualityItemType == Code.EnumHelper.QualityItemType.CheckBox)
                                              group p by new { QualityId = p.tbQualityItem.tbQualityItemGroup.tbQuality.Id, p.tbQualityItem.Id, p.tbQualityItem.QualityItemName } into g
                                              select new Dto.QualityReport.StudentReport
                                              {
                                                  QualityId = g.Key.QualityId,
                                                  QualityItemName = g.Key.QualityItemName,
                                                  OptionAvg = g.Average(d => d.tbQualityOption.OptionValue),
                                              }).ToList();
                        #endregion

                        #region 获取考勤统计
                        //获取学生学期所在教学班信息
                        vm.OrgList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                      where p.IsDeleted == false
                                      && p.tbStudent.IsDeleted == false
                                      && p.tbOrg.IsDeleted == false
                                      && p.tbOrg.tbYear.IsDeleted == false
                                      && yearSectionIdList.Contains(p.tbOrg.tbYear.Id)
                                      && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbOrg.OrgName,
                                          Value = p.tbOrg.Id.ToString(),
                                      }).ToList();

                        vm.OrgList.AddRange((from p in db.Table<Course.Entity.tbOrg>()
                                             where p.IsDeleted == false
                                             && p.tbClass.IsDeleted == false
                                             && p.tbYear.IsDeleted == false
                                             && yearSectionIdList.Contains(p.tbYear.Id)
                                             && p.tbClass.Id == student.tbClass.Id
                                             orderby p.No
                                             select new System.Web.Mvc.SelectListItem
                                             {
                                                 Text = p.OrgName,
                                                 Value = p.Id.ToString(),
                                             }).ToList());
                        vm.OrgList = vm.OrgList.Distinct().ToList();

                        var orgIdList = vm.OrgList.Select(d => d.Value).ToList();

                        //获取考勤类型信息
                        vm.AttendanceTypeList = (from p in db.Table<Attendance.Entity.tbAttendanceType>()
                                                 where p.IsDeleted == false
                                                 orderby p.No
                                                 select p).ToList();

                        //获取考勤结果
                        vm.AttendanceList = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                  .Include(d => d.tbOrg)
                                  .Include(d => d.tbAttendanceType)
                                             where p.IsDeleted == false
                                             && p.tbOrg.IsDeleted == false
                                             && p.tbAttendanceType.IsDeleted == false
                                             && orgIdList.Contains(p.tbOrg.Id.ToString())
                                             select p).ToList();
                        #endregion

                        #region 获取评语
                        //获取我的评语
                        vm.SelfComment = (from p in db.Table<Quality.Entity.tbQualitySelf>()
                                    .Include(d => d.tbStudent)
                                          where p.IsDeleted == false
                                      && p.tbYear.IsDeleted == false
                                      && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                      && p.tbYear.Id == vm.YearTermId
                                          select new Dto.QualityReport.StudentReport
                                          {
                                              Comment = p.Content,
                                              InputDate = p.InputDate,
                                          }).FirstOrDefault();

                        //获取学期期待
                        vm.PlanComment = (from p in db.Table<Quality.Entity.tbQualityPlan>()
                                    .Include(d => d.tbStudent)
                                          where p.IsDeleted == false
                                      && p.tbYear.IsDeleted == false
                                      && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                      && p.tbYear.Id == vm.YearTermId
                                          select new Dto.QualityReport.StudentReport
                                          {
                                              Comment = p.Content,
                                              InputDate = p.InputDate,
                                          }).FirstOrDefault();

                        //获取学期总结
                        vm.SummaryComment = (from p in db.Table<Quality.Entity.tbQualitySummary>()
                                    .Include(d => d.tbStudent)
                                             where p.IsDeleted == false
                                         && p.tbYear.IsDeleted == false
                                         && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                         && p.tbYear.Id == vm.YearTermId
                                             select new Dto.QualityReport.StudentReport
                                             {
                                                 Comment = p.Content,
                                                 InputDate = p.InputDate,
                                             }).FirstOrDefault();

                        //获取班主任评语
                        vm.ClassComment = (from p in db.Table<Quality.Entity.tbQualityComment>()
                                    .Include(d => d.tbSysUser)
                                           where p.IsDeleted == false
                                       && p.tbYear.IsDeleted == false
                                       && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                       && p.tbYear.Id == vm.YearTermId
                                           select new Dto.QualityReport.StudentReport
                                           {
                                               Comment = p.Comment,
                                               InputDate = p.InputDate,
                                               UserName = "班主任：" + p.tbSysUser.UserName,
                                           }).FirstOrDefault();
                        //获取任课教师评语
                        vm.OrgTeacherComment = (from p in db.Table<Quality.Entity.tbQualityRemark>()
                                            .Include(d => d.tbSysUser)
                                                where p.IsDeleted == false
                                                && p.tbOrg.IsDeleted == false
                                            && p.tbOrg.tbYear.IsDeleted == false
                                            && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                            && p.tbOrg.tbYear.tbYearParent.Id == vm.YearTermId
                                                select new Dto.QualityReport.StudentReport
                                                {
                                                    Comment = p.Remark,
                                                    InputDate = p.InputDate,
                                                    UserName = p.tbOrg.OrgName + "：" + p.tbSysUser.UserName,
                                                }).ToList();
                        #endregion


                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentReport(Models.QualityReport.StudentReport vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("StudentReport", new
            {
                gradeId = vm.GradeId,
                yearTermId = vm.YearTermId,
                courseGroupItemId = vm.CourseGroupItemId,
            }));
        }
        #endregion

        #region 班级汇总
        public ActionResult ClassReport()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityReport.ClassReport();
                //获取学段信息
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                //获取学年信息
                var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == vm.YearId
                              select p.tbYearParent.tbYearParent.Id).FirstOrDefault();

                //获取考试信息
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.IsDeleted == false
                                && p.tbYear.Id == vm.YearId
                                select p).ToList();

                vm.ExamList = (from p in examList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                if (vm.ExamList.Count > 0 && vm.ExamId == 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取教师所在行政班信息
                vm.ClassList = Areas.Basis.Controllers.ClassTeacherController.GetClassByClassTeacher();
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取学生所在行政班信息
                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                          .Include(d => d.tbClass)
                          .Include(d => d.tbClass.tbGrade)
                          .Include(d => d.tbStudent)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbClass.tbYear.Id == yearId
                                        //&& ((vm.ClassId == null || vm.ClassId == 0) ? true : p.tbClass.Id == vm.ClassId)
                                        && p.tbClass.Id == vm.ClassId
                                        select p).ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    classStudentList = classStudentList.Where(d => d.tbStudent.StudentName.Contains(vm.SearchText)).ToList();
                }

                var studentIdList = classStudentList.Select(d => d.tbStudent.Id).ToList();

                //考试
                var exam = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.IsPublish == true
                            && p.IsDeleted == false
                            && p.Id == vm.ExamId
                            select new
                            {
                                ExamName = p.ExamName,
                                ExamId = p.Id,
                            }).Distinct().FirstOrDefault();

                if (exam != null)
                {
                    //获取成绩信息
                    var studentExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                               .Include(d => d.tbExamLevel)
                                               where p.tbExamCourse.IsDeleted == false
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbExamCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                                && p.tbExamCourse.tbExam.IsDeleted == false
                                                && p.tbExamCourse.tbExam.Id == exam.ExamId
                                                && studentIdList.Contains(p.tbStudent.Id)
                                               group p by p.tbStudent.Id into g
                                               select new
                                               {
                                                   StudentId = g.Key,
                                                   TotalLevelScore = g.Sum(d => (d.tbExamLevel != null ? d.tbExamLevel.ExamLevelValue : 0)),
                                               }).ToList();
                    //获取学生等级信息
                    vm.ClassLevelList = (from p in classStudentList
                                         join t in studentExamMarkList on p.tbStudent.Id equals t.StudentId
                                         select new Dto.QualityReport.ClassReport
                                         {
                                             GradeName = p.tbClass.tbGrade.GradeName,
                                             ClassName = p.tbClass.ClassName,
                                             StudentCode = p.tbStudent.StudentCode,
                                             StudentName = p.tbStudent.StudentName,
                                             LevleValue = t.TotalLevelScore,
                                         }).ToList();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassReport(Models.QualityReport.ClassReport vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassReport", new { yearId = vm.YearId, examId = vm.ExamId, searchText = vm.SearchText, classId = vm.ClassId, }));
        }

        public ActionResult ClassExport()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var vm = new Models.QualityReport.ClassExport();
                //获取学年信息
                var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == vm.YearId
                              select p.tbYearParent.tbYearParent.Id).FirstOrDefault();

                //获取班级信息
                var tb = from p in db.Table<Basis.Entity.tbClass>()
                         where p.tbYear.Id == yearId
                         && p.IsDeleted == false
                         select p;

                //获取教师所在行政班信息
                vm.ClassList = Areas.Basis.Controllers.ClassTeacherController.GetClassByClassTeacher();
                if (vm.ClassId == 0 && vm.ClassList.Count > 0)
                {
                    vm.ClassId = vm.ClassList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取学生所在班信息
                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                          .Include(d => d.tbClass)
                          .Include(d => d.tbClass.tbGrade)
                          .Include(d => d.tbStudent)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbClass.tbYear.Id == yearId
                                        //&& ((vm.ClassId == null || vm.ClassId == 0) ? true : p.tbClass.Id == vm.ClassId)
                                        && p.tbClass.Id == vm.ClassId
                                        select p).ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    classStudentList = classStudentList.Where(d => d.tbStudent.StudentName.Contains(vm.SearchText)).ToList();
                }

                var studentIdList = classStudentList.Select(d => d.tbStudent.Id).ToList();

                //考试
                var exam = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.IsPublish == true
                            && p.IsDeleted == false
                            && p.tbYear.Id == vm.YearId
                            && p.Id == vm.ExamId
                            select new
                            {
                                ExamName = p.ExamName,
                                ExamId = p.Id,
                            }).Distinct().FirstOrDefault();

                if (exam != null)
                {
                    //获取成绩信息
                    var studentExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                               .Include(d => d.tbExamLevel)
                                               where p.tbExamCourse.IsDeleted == false
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbExamCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                                && p.tbExamCourse.tbExam.IsDeleted == false
                                                && p.tbExamCourse.tbExam.Id == exam.ExamId
                                                && studentIdList.Contains(p.tbStudent.Id)
                                               group p by p.tbStudent.Id into g
                                               select new
                                               {
                                                   StudentId = g.Key,
                                                   TotalLevelScore = g.Sum(d => (d.tbExamLevel != null ? d.tbExamLevel.ExamLevelValue : 0)),
                                               }).ToList();
                    //获取学生等级信息
                    vm.ClassLevelList = (from p in classStudentList
                                         join t in studentExamMarkList on p.tbStudent.Id equals t.StudentId
                                         select new Dto.QualityReport.ClassExport
                                         {
                                             GradeName = p.tbClass.tbGrade.GradeName,
                                             ClassName = p.tbClass.ClassName,
                                             StudentCode = p.tbStudent.StudentCode,
                                             StudentName = p.tbStudent.StudentName,
                                             LevleValue = t.TotalLevelScore,
                                         }).ToList();
                }
                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("年级"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("学号"),
                        new System.Data.DataColumn("姓名"),
                        new System.Data.DataColumn("积点"),
                        new System.Data.DataColumn("星级")
                    });
                foreach (var a in vm.ClassLevelList)
                {
                    var dr = dt.NewRow();
                    dr["年级"] = a.GradeName;
                    dr["班级"] = a.ClassName;
                    dr["学号"] = a.StudentCode;
                    dr["姓名"] = a.StudentName;
                    dr["积点"] = a.LevleValue;
                    if (a.LevleValue >= 95)
                    {
                        dr["星级"] = "★★★★★";
                    }
                    else if (90 <= a.LevleValue && a.LevleValue < 95)
                    {
                        dr["星级"] = "★★★★☆";
                    }
                    else if (80 <= a.LevleValue && a.LevleValue < 90)
                    {
                        dr["星级"] = "★★★☆☆";
                    }
                    else if (70 <= a.LevleValue && a.LevleValue < 80)
                    {
                        dr["星级"] = "★★☆☆☆";
                    }
                    else if (60 <= a.LevleValue && a.LevleValue < 70)
                    {
                        dr["星级"] = "★☆☆☆☆";
                    }
                    else if (a.LevleValue < 60)
                    {
                        dr["星级"] = "☆☆☆☆☆";
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
        #endregion

        #region 年级汇总
        public ActionResult GradeReport()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityReport.GradeReport();
                //获取学段信息
                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Section);
                if (vm.YearList.Count > 0 && vm.YearId == 0)
                {
                    vm.YearId = vm.YearList.OrderByDescending(d => d.Selected).FirstOrDefault().Value.ConvertToInt();
                }
                //获取学年信息
                var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == vm.YearId
                              select p.tbYearParent.tbYearParent.Id).FirstOrDefault();

                //获取考试信息
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.IsDeleted == false
                                && p.tbYear.Id == vm.YearId
                                select p).ToList();

                vm.ExamList = (from p in examList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                if (vm.ExamList.Count > 0 && vm.ExamId == 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }
                //获取年级信息
                var tb = from p in db.Table<Basis.Entity.tbGrade>()
                         where p.IsDeleted == false
                         select p;
                //获取教师所在年级信息
                vm.GradeList = Teacher.Controllers.TeacherGradeController.GetGradeByTeacher();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //获取行政班信息
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 .Include(d => d.tbGrade)
                                 where p.tbYear.Id == yearId
                                 && p.IsDeleted == false
                                 //&& ((vm.GradeId == null || vm.GradeId == 0) ? true : p.tbGrade.Id == vm.GradeId)
                                 && p.tbGrade.Id == vm.GradeId
                                 select p).ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    classList = classList.Where(d => d.ClassName.Contains(vm.SearchText)).ToList();
                }

                vm.ClassList = (from p in classList
                                select new Dto.QualityReport.GradeReport
                                {
                                    ClassId = p.Id,
                                    GradeName = p.tbGrade.GradeName,
                                    ClassName = p.ClassName,
                                }).ToList();

                var classIdList = vm.ClassList.Select(d => d.ClassId).ToList();

                //获取学生所在行政班信息
                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                          .Include(d => d.tbClass)
                          .Include(d => d.tbClass.tbGrade)
                          .Include(d => d.tbStudent)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && classIdList.Contains(p.tbClass.Id)
                                        && p.tbClass.tbYear.Id == yearId
                                        select p).ToList();

                var studentIdList = classStudentList.Select(d => d.tbStudent.Id).ToList();

                //考试
                var exam = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.IsPublish == true
                            && p.IsDeleted == false
                            && p.tbYear.Id == vm.YearId
                            && p.Id == vm.ExamId
                            select new
                            {
                                ExamName = p.ExamName,
                                ExamId = p.Id,
                            }).Distinct().FirstOrDefault();

                if (exam != null)
                {
                    //获取考试成绩信息
                    var studentExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                               .Include(d => d.tbExamLevel)
                                               where p.tbExamCourse.IsDeleted == false
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbExamCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                                && p.tbExamCourse.tbExam.IsDeleted == false
                                                && p.tbExamCourse.tbExam.Id == exam.ExamId
                                                && studentIdList.Contains(p.tbStudent.Id)
                                               group p by p.tbStudent.Id into g
                                               select new
                                               {
                                                   StudentId = g.Key,
                                                   TotalLevelScore = g.Sum(d => (d.tbExamLevel != null ? d.tbExamLevel.ExamLevelValue : 0)),
                                               }).ToList();

                    vm.ClassExamMarkList = (from p in classStudentList
                                            join t in studentExamMarkList on p.tbStudent.Id equals t.StudentId
                                            select new Dto.QualityReport.GradeReport
                                            {
                                                GradeName = p.tbClass.tbGrade.GradeName,
                                                ClassName = p.tbClass.ClassName,
                                                ClassId = p.tbClass.Id,
                                                LevleValue = t.TotalLevelScore,
                                            }).ToList();
                }
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GradeReport(Models.QualityReport.GradeReport vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("GradeReport", new
            {
                searchText = vm.SearchText,
                gradeId = vm.GradeId,
                yearId = vm.YearId,
                examId = vm.ExamId,
            }));
        }

        public ActionResult GradeExport()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var file = System.IO.Path.GetTempFileName();

                var vm = new Models.QualityReport.GradeExport();
                //获取学年信息
                var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == vm.YearId
                              select p.tbYearParent.tbYearParent.Id).FirstOrDefault();
                //获取年级信息
                var tb = from p in db.Table<Basis.Entity.tbGrade>()
                         where p.IsDeleted == false
                         select p;

                /*
                vm.GradeList = (from p in tb
                                orderby p.No ascending
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.GradeName,
                                    Value = p.Id.ToString(),
                                }).ToList();
                                */
                //获取教师所在年级信息
                vm.GradeList = Teacher.Controllers.TeacherGradeController.GetGradeByTeacher();
                if (vm.GradeId == 0 && vm.ClassList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                //{
                //    vm.GradeId = tb.FirstOrDefault().Id;
                //}

                //获取行政班信息
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 .Include(d => d.tbGrade)
                                 where p.tbYear.Id == yearId
                                 && p.IsDeleted == false
                                 //&& ((vm.GradeId == null || vm.GradeId == 0) ? true : p.tbGrade.Id == vm.GradeId)
                                 && p.tbGrade.Id == vm.GradeId
                                 select p).ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    classList = classList.Where(d => d.ClassName.Contains(vm.SearchText)).ToList();
                }

                vm.ClassList = (from p in classList
                                select new Dto.QualityReport.GradeExport
                                {
                                    ClassId = p.Id,
                                    GradeName = p.tbGrade.GradeName,
                                    ClassName = p.ClassName,
                                }).ToList();

                var classIdList = vm.ClassList.Select(d => d.ClassId).ToList();

                //获取学生所在行政班信息
                var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                          .Include(d => d.tbClass)
                          .Include(d => d.tbClass.tbGrade)
                          .Include(d => d.tbStudent)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && classIdList.Contains(p.tbClass.Id)
                                        && p.tbClass.tbYear.Id == yearId
                                        select p).ToList();

                var studentIdList = classStudentList.Select(d => d.tbStudent.Id).ToList();

                //考试
                var exam = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.IsPublish == true
                            && p.IsDeleted == false
                            && p.tbYear.Id == vm.YearId
                            && p.Id == vm.ExamId
                            select new
                            {
                                ExamName = p.ExamName,
                                ExamId = p.Id,
                            }).Distinct().FirstOrDefault();

                if (exam != null)
                {
                    //获取考试成绩信息
                    var studentExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                               .Include(d => d.tbExamLevel)
                                               where p.tbExamCourse.IsDeleted == false
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbExamCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                                && p.tbExamCourse.tbExam.IsDeleted == false
                                                && p.tbExamCourse.tbExam.Id == exam.ExamId
                                                && studentIdList.Contains(p.tbStudent.Id)
                                               group p by p.tbStudent.Id into g
                                               select new
                                               {
                                                   StudentId = g.Key,
                                                   TotalLevelScore = g.Sum(d => (d.tbExamLevel != null ? d.tbExamLevel.ExamLevelValue : 0)),
                                               }).ToList();

                    vm.ClassExamMarkList = (from p in classStudentList
                                            join t in studentExamMarkList on p.tbStudent.Id equals t.StudentId
                                            select new Dto.QualityReport.GradeExport
                                            {
                                                GradeName = p.tbClass.tbGrade.GradeName,
                                                ClassName = p.tbClass.ClassName,
                                                ClassId = p.tbClass.Id,
                                                LevleValue = t.TotalLevelScore,
                                            }).ToList();
                }
                var dt = new System.Data.DataTable();
                dt.Columns.AddRange(new System.Data.DataColumn[]
                    {
                        new System.Data.DataColumn("年级"),
                        new System.Data.DataColumn("班级"),
                        new System.Data.DataColumn("五星"),
                        new System.Data.DataColumn("四星"),
                        new System.Data.DataColumn("三星"),
                        new System.Data.DataColumn("二星"),
                        new System.Data.DataColumn("一星")
                    });
                foreach (var cla in vm.ClassList)
                {
                    var dr = dt.NewRow();
                    dr["年级"] = cla.GradeName;
                    dr["班级"] = cla.ClassName;
                    if (vm.ClassExamMarkList.Where(d => d.ClassId == cla.ClassId && d.LevleValue >= 95).Count() > 0)
                    {
                        dr["五星"] = vm.ClassExamMarkList.Where(d => d.ClassId == cla.ClassId && d.LevleValue >= 95).Count();
                    }
                    else
                    {
                        dr["五星"] = "0";
                    }
                    if (vm.ClassExamMarkList.Where(d => d.ClassId == cla.ClassId && (90 <= d.LevleValue && d.LevleValue < 95)).Count() > 0)
                    {
                        dr["四星"] = vm.ClassExamMarkList.Where(d => d.ClassId == cla.ClassId && (90 <= d.LevleValue && d.LevleValue < 95)).Count();
                    }
                    else
                    {
                        dr["四星"] = "0";
                    }
                    if (vm.ClassExamMarkList.Where(d => d.ClassId == cla.ClassId && (80 <= d.LevleValue && d.LevleValue < 90)).Count() > 0)
                    {
                        dr["三星"] = vm.ClassExamMarkList.Where(d => d.ClassId == cla.ClassId && (80 <= d.LevleValue && d.LevleValue < 90)).Count();
                    }
                    else
                    {
                        dr["三星"] = "0";
                    }
                    if (vm.ClassExamMarkList.Where(d => d.ClassId == cla.ClassId && (70 <= d.LevleValue && d.LevleValue < 80)).Count() > 0)
                    {
                        dr["二星"] = vm.ClassExamMarkList.Where(d => d.ClassId == cla.ClassId && (70 <= d.LevleValue && d.LevleValue < 80)).Count();
                    }
                    else
                    {
                        dr["二星"] = "0";
                    }
                    if (vm.ClassExamMarkList.Where(d => d.ClassId == cla.ClassId && (60 <= d.LevleValue && d.LevleValue < 70)).Count() > 0)
                    {
                        dr["一星"] = vm.ClassExamMarkList.Where(d => d.ClassId == cla.ClassId && (60 <= d.LevleValue && d.LevleValue < 70)).Count();
                    }
                    else
                    {
                        dr["一星"] = "0";
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
        #endregion

        #region 班主任综合素养
        public ActionResult ClassList()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Administrator && Code.Common.UserType != Code.EnumHelper.SysUserType.Teacher)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityReport.ClassList();

                vm.YearList = Basis.Controllers.YearController.SelectList(Code.EnumHelper.YearType.Term);
                //获取当前激活学段
                var section = (from p in db.Table<Basis.Entity.tbYear>()
                               .Include(d => d.tbYearParent)
                               where p.IsDeleted == false
                               && p.IsDefault == true
                               select p).FirstOrDefault();
                if (vm.YearList.Count > 0 && vm.YearId == 0 && section != null && section.YearType == Code.EnumHelper.YearType.Section)
                {
                    vm.YearId = vm.YearList.Where(d => d.Value == section.tbYearParent.Id.ToString()).FirstOrDefault().Value.ConvertToInt();
                }
                var yearId = (from p in db.Table<Basis.Entity.tbYear>()
                              where p.Id == vm.YearId
                              select p.tbYearParent.Id).FirstOrDefault();

                //获取教师行政班信息
                vm.ClassItemList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                    where p.IsDeleted == false
                                    && p.tbClass.IsDeleted == false
                                    && p.tbTeacher.IsDeleted == false
                                    && p.tbTeacher.tbSysUser.Id == Code.Common.UserId
                                     && p.tbClass.tbYear.Id == yearId
                                    select new System.Web.Mvc.SelectListItem
                                    {
                                        Text = p.tbClass.ClassName,
                                        Value = p.tbClass.Id.ToString(),
                                    }).Distinct().ToList();

                //默认取第一个评教Id去查询后面数据
                if (vm.ClassId == 0 && vm.ClassItemList.Count > 0)
                {
                    vm.ClassId = vm.ClassItemList.FirstOrDefault().Value.ConvertToInt();
                }

                var classList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                         .Include(d => d.tbStudent)
                         .Include(d => d.tbClass)
                                 where p.IsDeleted == false
                                && p.tbClass.IsDeleted == false
                                //&& p.tbClass.tbYear.IsDefault == true
                                && p.tbStudent.IsDeleted == false
                                && p.tbClass.tbYear.Id == yearId
                                && p.tbClass.Id == vm.ClassId
                                 select p).ToList();

                if (string.IsNullOrEmpty(vm.SearchText) == false)
                {
                    classList = classList.Where(d => d.tbStudent.StudentName.Contains(vm.SearchText)).ToList();
                }
                vm.ClassReportList = (from p in classList
                                      select new Dto.QualityReport.ClassList
                                      {
                                          StudentId = p.tbStudent.Id,
                                          StudentCode = p.tbStudent.StudentCode,
                                          StudentName = p.tbStudent.StudentName,
                                      }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassList(Models.QualityReport.ClassList vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassList", new
            {
                searchText = vm.SearchText,
                yearId = vm.YearId,
                classId = vm.ClassId,
            }));
        }

        public ActionResult ClassStudent()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityReport.ClassStudent();
                //根据当前登录用户Id获取学生信息
                var student = (from p in db.Table<Basis.Entity.tbClassStudent>()
                               .Include(d => d.tbClass)
                               .Include(d => d.tbStudent)
                               where p.IsDeleted == false
                               && p.tbClass.IsDeleted == false
                               && p.tbStudent.IsDeleted == false
                               && p.tbStudent.Id == vm.StudentId
                               select p).FirstOrDefault();
                if (student != null)
                {
                    //根据班级Id获取班主任信息
                    var teacher = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                               .Include(d => d.tbClass)
                               .Include(d => d.tbTeacher)
                                   where p.IsDeleted == false
                                   && p.tbClass.IsDeleted == false
                                   && p.tbTeacher.IsDeleted == false
                                   && p.tbClass.Id == student.tbClass.Id
                                   select p).FirstOrDefault();
                    vm.ClassName = student.tbClass.ClassName;
                    vm.StudentCode = student.tbStudent.StudentCode;
                    vm.StudentName = student.tbStudent.StudentName;
                    if (teacher != null)
                    {
                        vm.ClassTeacher = teacher.tbTeacher.TeacherName;
                    }

                    //根据学生Id获取年级
                    vm.GradeYearList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  .Include(d => d.tbClass.tbGrade)
                                  .Include(d => d.tbClass.tbYear)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbStudent.Id == vm.StudentId
                                        orderby p.tbClass.tbYear.No descending
                                        select p).ToList();
                    //获取学年ID
                    var yearIdList = vm.GradeYearList.Select(d => d.tbClass.tbYear.Id).ToList();
                    //获取学期
                    vm.YearTermList = (from p in db.Table<Basis.Entity.tbYear>()
                                        .Include(d => d.tbYearParent)
                                       where p.IsDeleted == false
                                       && yearIdList.Contains(p.tbYearParent.Id)
                                       select p).ToList();

                    var yearTermIdList = vm.YearTermList.Select(d => d.Id).ToList();
                    vm.YearSectionList = (from p in db.Table<Basis.Entity.tbYear>()
                                          .Include(d => d.tbYearParent)
                                          where p.IsDeleted == false
                                          && yearTermIdList.Contains(p.tbYearParent.Id)
                                          select p).ToList();
                    //获取年级
                    vm.GradeList = (from p in vm.GradeYearList
                                    orderby p.tbClass.tbGrade.No descending
                                    select new System.Web.Mvc.SelectListItem
                                    {
                                        Text = p.tbClass.tbGrade.GradeName,
                                        Value = p.tbClass.tbGrade.Id.ToString(),
                                    }).ToList();

                    //默认默认第一个年级Id去查询后面数据
                    if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                    {
                        vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                    }

                    //根据当前登录用户Id和年级Id获取学生信息
                    var gradeStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                        .Include(d => d.tbStudent)
                              .Include(d => d.tbClass.tbGrade)
                              .Include(d => d.tbClass.tbYear)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbStudent.Id == vm.StudentId
                                        && p.tbClass.tbGrade.Id == vm.GradeId
                                        select p).FirstOrDefault();

                    //根据状态获取学期
                    if (gradeStudent != null)
                    {
                        decimal maxTotal = 0;
                        //获取学期
                        var yearTermList = (from p in db.Table<Basis.Entity.tbYear>()
                                            where p.tbYearParent.Id == gradeStudent.tbClass.tbYear.Id
                                            && p.IsDeleted == false
                                            orderby p.No
                                            select p).ToList();

                        //获取学段
                        if (vm.YearTermId == 0)
                        {
                            var yearSection = vm.YearSectionList.Where(d => d.IsDefault == true).FirstOrDefault();
                            if (yearSection != null)
                            {
                                vm.YearTermId = yearSection.tbYearParent.Id;
                            }
                        }
                        var yearSectionList = (from p in db.Table<Basis.Entity.tbYear>()
                                              .Include(d => d.tbYearParent)
                                               where p.tbYearParent.Id == vm.YearTermId
                                               && p.IsDeleted == false
                                               orderby p.No
                                               select p).ToList();
                        var yearSectionIdList = yearSectionList.Select(d => d.Id).ToList();

                        #region 成绩等级
                        //获取课程分组
                        vm.CourseGroupItemList = (from p in db.Table<Course.Entity.tbCourseGroup>()
                                                  where p.IsDeleted == false
                                                  orderby p.No
                                                  select new System.Web.Mvc.SelectListItem
                                                  {
                                                      Text = p.CourseGroupName,
                                                      Value = p.Id.ToString(),
                                                  }).ToList();
                        var courseGroupIdList = vm.CourseGroupItemList.Select(d => d.Value).ToList();

                        //获取领域
                        vm.CourseDomainItemList = (from p in db.Table<Course.Entity.tbCourseDomain>()
                                                   where p.IsDeleted == false
                                                   orderby p.No
                                                   select p).ToList();
                        var courseDomainItemIdList = vm.CourseDomainItemList.Select(d => d.Id).ToList();

                        //获取教学班课程、分组、领域信息
                        vm.CourseDomainList = (from p in db.Table<Course.Entity.tbOrg>()
                                               where p.IsDeleted == false
                                               && courseDomainItemIdList.Contains(p.tbCourse.tbCourseDomain.Id)
                                               && (vm.CourseGroupItemId <= 0 ? true : p.tbCourse.tbCourseGroup.Id == vm.CourseGroupItemId)
                                               && p.tbGrade.Id == vm.GradeId
                                               orderby p.tbCourse.tbCourseDomain.No
                                               select new Dto.QualityReport.StudentReport
                                               {
                                                   CourseId = p.tbCourse.Id,
                                                   CourseName = p.tbCourse.CourseName,
                                                   CourseGroupId = p.tbCourse.tbCourseGroup.Id,
                                                   CourseGroupName = p.tbCourse.tbCourseGroup.CourseGroupName,
                                                   CourseDomainName = p.tbCourse.tbCourseDomain.CourseDomainName,
                                                   CourseDomainNo = p.tbCourse.tbCourseDomain.No,
                                                   CourseDomainId = p.tbCourse.tbCourseDomain.Id,
                                               }).Distinct().ToList();

                        var courseIdList = vm.CourseDomainList.Select(d => d.CourseId).ToList();
                        var courseDemainIdList = vm.CourseDomainList.Select(d => d.CourseDomainId).Distinct().ToList();
                        var courseDemainNameList = vm.CourseDomainList.Select(d => d.CourseDomainName).Distinct().ToList();

                        //考试
                        vm.ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                       where p.IsPublish == true
                                       && p.IsDeleted == false
                                       && yearSectionIdList.Contains(p.tbYear.Id)
                                       select p).ToList();
                        var examIdList = vm.ExamList.Select(d => d.Id).ToList();

                        if (vm.ExamList.Count() > 0)
                        {
                            #region 获取个人积点
                            //获取个人积点
                            vm.ExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                               where p.tbExamCourse.IsDeleted == false
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbExamCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                                && p.tbExamCourse.tbExam.IsDeleted == false
                                                && examIdList.Contains(p.tbExamCourse.tbExam.Id)
                                                && courseIdList.Contains(p.tbExamCourse.tbCourse.Id)
                                                && p.tbStudent.Id == vm.StudentId
                                               orderby p.tbExamCourse.tbCourse.tbCourseDomain.No
                                               select new Dto.QualityReport.StudentReport
                                               {
                                                   CourseGroupId = p.tbExamCourse.tbCourse.tbCourseGroup.Id,
                                                   ExamId = p.tbExamCourse.tbExam.Id,
                                                   TotalMark = p.TotalMark,
                                                   CourseId = p.tbExamCourse.tbCourse.Id,
                                                   LevelName = p.tbExamLevel != null ? p.tbExamLevel.ExamLevelName : string.Empty,
                                                   LevelScore = p.tbExamLevel != null ? p.tbExamLevel.ExamLevelValue : 0,
                                                   CourseName = p.tbExamCourse.tbCourse.CourseName,
                                                   CourseDomainId = p.tbExamCourse.tbCourse.tbCourseDomain.Id,
                                               }).ToList();

                            //获取领域学生积点总分
                            var studentExamMarkList = (from p in vm.ExamMarkList
                                                       group p by new { p.CourseDomainId, p.ExamId } into g
                                                       select new
                                                       {
                                                           ExamId = g.Key.ExamId,
                                                           CourseDomainId = g.Key.CourseDomainId,
                                                           StudentScore = g.Sum(d => d.LevelScore),
                                                       }).ToList();
                            #endregion

                            #region 获取班级学生积点
                            //获取班级学生
                            var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                            .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass.tbGrade)
                                  .Include(d => d.tbClass.tbYear)
                                                    where p.IsDeleted == false
                                                    && p.tbClass.IsDeleted == false
                                                    && p.tbStudent.IsDeleted == false
                                                    && p.tbClass.Id == gradeStudent.tbClass.Id
                                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                                    select p).ToList();
                            //获取领域班级学生积点总分
                            var studentIdList = classStudentList.Select(d => d.tbStudent.Id).ToList();
                            var classExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                                     where p.tbExamCourse.IsDeleted == false
                                                      && p.tbStudent.IsDeleted == false
                                                      && p.tbExamCourse.IsDeleted == false
                                                      && p.tbExamCourse.tbCourse.IsDeleted == false
                                                      && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                                      && p.tbExamCourse.tbExam.IsDeleted == false
                                                      && examIdList.Contains(p.tbExamCourse.tbExam.Id)
                                                      && courseIdList.Contains(p.tbExamCourse.tbCourse.Id)
                                                      && studentIdList.Contains(p.tbStudent.Id)
                                                     group p by new { p.tbStudent.Id, CourseDomainId = p.tbExamCourse.tbCourse.tbCourseDomain.Id, ExamId = p.tbExamCourse.tbExam.Id } into g
                                                     select new
                                                     {
                                                         ExamId = g.Key.ExamId,
                                                         StudentId = g.Key.Id,
                                                         CourseDomainId = g.Key.CourseDomainId,
                                                         ClassScore = g.Sum(d => d.tbExamLevel.ExamLevelValue),
                                                     }).ToList();
                            //获取领域班级积点平均分
                            var classSumList = (from p in classExamMarkList
                                                group p by new { p.StudentId, p.CourseDomainId, p.ExamId } into g
                                                select new
                                                {
                                                    ExamId = g.Key.ExamId,
                                                    CourseDomainId = g.Key.CourseDomainId,
                                                    ClassScore = g.Sum(d => d.ClassScore),
                                                }).ToList();
                            //获取领域积点平均分
                            var classAvgList = (from p in classSumList
                                                group p by new { p.CourseDomainId, p.ExamId } into g
                                                select new
                                                {
                                                    ExamId = g.Key.ExamId,
                                                    CourseDomainId = g.Key.CourseDomainId,
                                                    ClassAvg = g.Average(d => d.ClassScore),
                                                }).ToList();

                            //获取每个学生积点总分
                            var classStudentMarkList = (from p in classExamMarkList
                                                        group p by new { p.StudentId, p.ExamId } into g
                                                        select new
                                                        {
                                                            ExamId = g.Key.ExamId,
                                                            StudentId = g.Key.StudentId,
                                                            ClassScore = g.Sum(d => d.ClassScore),
                                                        }).ToList();
                            //获取班级积点平均分
                            vm.ClassLevelAvgList = (from p in classStudentMarkList
                                                    join t in classStudentList on p.StudentId equals t.tbStudent.Id
                                                    group p by new { t.tbClass.Id, p.ExamId } into g
                                                    select new Dto.QualityReport.StudentReport
                                                    {
                                                        ExamId = g.Key.ExamId,
                                                        ClassLevelAvg = g.Average(d => d.ClassScore),
                                                    }).ToList();
                            #endregion

                            #region 获取年级学生积点
                            //获取年级学生
                            var gradeStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                            .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass.tbGrade)
                                  .Include(d => d.tbClass.tbYear)
                                                    where p.IsDeleted == false
                                                    && p.tbClass.IsDeleted == false
                                                    && p.tbStudent.IsDeleted == false
                                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                                    select p).ToList();
                            //获取年级学生积点
                            studentIdList = gradeStudentList.Select(d => d.tbStudent.Id).ToList();
                            var gradeExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                                     where p.tbExamCourse.IsDeleted == false
                                                      && p.tbStudent.IsDeleted == false
                                                      && p.tbExamCourse.IsDeleted == false
                                                      && p.tbExamCourse.tbCourse.IsDeleted == false
                                                      && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                                      && p.tbExamCourse.tbExam.IsDeleted == false
                                                      && examIdList.Contains(p.tbExamCourse.tbExam.Id)
                                                      && courseIdList.Contains(p.tbExamCourse.tbCourse.Id)
                                                      && studentIdList.Contains(p.tbStudent.Id)
                                                     group p by new { p.tbStudent.Id, CourseDomainId = p.tbExamCourse.tbCourse.tbCourseDomain.Id, ExamId = p.tbExamCourse.tbExam.Id } into g
                                                     select new
                                                     {
                                                         ExamId = g.Key.ExamId,
                                                         StudentId = g.Key.Id,
                                                         CourseDomainId = g.Key.CourseDomainId,
                                                         GradeScore = g.Sum(d => (d.tbExamLevel != null ? d.tbExamLevel.ExamLevelValue : 0)),
                                                     }).ToList();

                            //获取领域年级积点总分
                            var gradeSumList = (from p in gradeExamMarkList
                                                group p by new { p.StudentId, p.CourseDomainId, p.ExamId } into g
                                                select new
                                                {
                                                    ExamId = g.Key.ExamId,
                                                    CourseDomainId = g.Key.CourseDomainId,
                                                    GradeScore = g.Sum(d => d.GradeScore),
                                                }).ToList();

                            //获取领域年级积点平均分
                            var gradeAvgList = (from p in gradeSumList
                                                group p by new { p.CourseDomainId, p.ExamId } into g
                                                select new
                                                {
                                                    ExamId = g.Key.ExamId,
                                                    CourseDomainId = g.Key.CourseDomainId,
                                                    GradeAvg = g.Average(d => d.GradeScore),
                                                }).ToList();

                            //获取学生积点总分
                            var gradeStudentExamMarkList = (from p in gradeExamMarkList
                                                            group p by new { p.StudentId, p.ExamId } into g
                                                            select new
                                                            {
                                                                ExamId = g.Key.ExamId,
                                                                StudentId = g.Key.StudentId,
                                                                GradeScore = g.Sum(d => d.GradeScore),
                                                            }).ToList();

                            //获取年级积点平均分
                            vm.GradeLevelAvgList = (from p in gradeStudentExamMarkList
                                                    join t in gradeStudentList on p.StudentId equals t.tbStudent.Id
                                                    group p by new { t.tbClass.tbGrade.Id, p.ExamId } into g
                                                    select new Dto.QualityReport.StudentReport
                                                    {
                                                        ExamId = g.Key.ExamId,
                                                        GradeLevelAvg = g.Average(d => d.GradeScore),
                                                    }).ToList();
                            #endregion

                            #region 绑定雷达图数据
                            maxTotal = 0;
                            foreach (var exam in vm.ExamList)
                            {
                                var demainList = new List<object>();
                                var sList = new List<object>();
                                var cList = new List<object>();
                                var gList = new List<object>();
                                var tList = new List<object>();
                                var i = 0;
                                foreach (var courseDomainId in courseDemainIdList)
                                {
                                    var studentTotal = studentExamMarkList.Where(d => d.CourseDomainId == courseDomainId && d.ExamId == exam.Id).FirstOrDefault();
                                    var classTotal = classAvgList.Where(d => d.CourseDomainId == courseDomainId && d.ExamId == exam.Id).FirstOrDefault();
                                    var gradeTotal = gradeAvgList.Where(d => d.CourseDomainId == courseDomainId && d.ExamId == exam.Id).FirstOrDefault();
                                    if (i == 0)
                                    {
                                        if (studentExamMarkList.Where(d => d.ExamId == exam.Id).Sum(d => d.StudentScore) > maxTotal)
                                        {
                                            maxTotal = studentExamMarkList.Where(d => d.ExamId == exam.Id).Sum(d => d.StudentScore);
                                        }
                                        sList.Add(studentExamMarkList.Where(d => d.ExamId == exam.Id).Sum(d => d.StudentScore).ToString("0.00"));
                                        if (studentTotal != null)
                                        {
                                            sList.Add(studentTotal.StudentScore.ToString("0.00"));
                                        }
                                        else
                                        {
                                            sList.Add("0.00");
                                        }
                                        if (classAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.ClassAvg) > maxTotal)
                                        {
                                            maxTotal = classAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.ClassAvg);
                                        }
                                        cList.Add(classAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.ClassAvg).ToString("0.00"));
                                        if (classTotal != null)
                                        {
                                            cList.Add(classTotal.ClassAvg.ToString("0.00"));
                                        }
                                        else
                                        {
                                            cList.Add("0.00");
                                        }
                                        if (gradeAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.GradeAvg) > maxTotal)
                                        {
                                            maxTotal = gradeAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.GradeAvg);
                                        }
                                        gList.Add(gradeAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.GradeAvg).ToString("0.00"));
                                        if (gradeTotal != null)
                                        {
                                            gList.Add(gradeTotal.GradeAvg.ToString("0.00"));
                                        }
                                        else
                                        {
                                            gList.Add("0.00");
                                        }
                                    }
                                    else
                                    {
                                        if (studentTotal != null)
                                        {
                                            if (studentTotal.StudentScore > maxTotal)
                                            {
                                                maxTotal = studentTotal.StudentScore;
                                            }
                                            sList.Add(studentTotal.StudentScore.ToString("0.00"));
                                        }
                                        else
                                        {
                                            sList.Add("0.00");
                                        }

                                        if (classTotal != null)
                                        {
                                            if (classTotal.ClassAvg > maxTotal)
                                            {
                                                maxTotal = classTotal.ClassAvg;
                                            }
                                            cList.Add(classTotal.ClassAvg.ToString("0.00"));
                                        }
                                        else
                                        {
                                            cList.Add("0.00");
                                        }

                                        if (gradeTotal != null)
                                        {
                                            if (gradeTotal.GradeAvg > maxTotal)
                                            {
                                                maxTotal = gradeTotal.GradeAvg;
                                            }
                                            gList.Add(gradeTotal.GradeAvg.ToString("0.00"));
                                        }
                                        else
                                        {
                                            gList.Add("0.00");
                                        }
                                    }
                                    i++;
                                }
                                demainList.Add(new { name = "个人", value = sList });
                                demainList.Add(new { name = "班级均分", value = cList });
                                demainList.Add(new { name = "年级均分", value = gList });
                                var courseDemainAvg = new Dto.QualityReport.StudentReport();
                                courseDemainAvg.ReportName = Code.Common.ToJSONString(demainList);
                                courseDemainAvg.ExamId = exam.Id;
                                vm.CourseDemainAvgList.Add(courseDemainAvg);

                            }
                            var courseDemainList = new List<object>();
                            //courseDemainList.Add(new { name = "总评", max = (maxTotal > 0 ? maxTotal : 5) });
                            courseDemainList.Add(new { name = "总评", max = 100 });
                            foreach (var courseDomainName in courseDemainNameList)
                            {
                                //courseDemainList.Add(new { name = courseDomainName, max = (maxTotal > 0 ? maxTotal : 5) });
                                courseDemainList.Add(new { name = courseDomainName, max = 100 });
                            }
                            vm.CourseDemainNames = Code.Common.ToJSONString(courseDemainList);
                            #endregion
                        }
                        #endregion

                        #region 荣誉
                        //获取荣誉
                        vm.HonorList = (from p in db.Table<Student.Entity.tbStudentHonor>()
                                                .Include(d => d.tbstudentHonorLevel)
                                                .Include(d => d.tbStudentHonorType)
                                        where p.IsDeleted == false
                                        && p.tbstudentHonorLevel.IsDeleted == false
                                        && p.tbStudentHonorType.IsDeleted == false
                                        && p.tbYear.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && yearSectionIdList.Contains(p.tbYear.Id)
                                         && p.tbStudent.Id == vm.StudentId
                                        select new Dto.QualityReport.StudentReport
                                        {
                                            HonorName = p.HonorName,
                                            StudentHonorLevelName = p.tbstudentHonorLevel.StudentHonorLevelName,
                                            StudentHonorTypeName = p.tbStudentHonorType.StudentHonorTypeName,
                                            InputDate = p.InputDate,
                                        }).ToList();
                        #endregion

                        #region 获取评价
                        //获取评价
                        vm.QualityList = (from p in db.Table<Quality.Entity.tbQuality>()
                                          where p.IsDeleted == false
                                          && yearSectionIdList.Contains(p.tbYear.Id)
                                          && p.IsOpen == true
                                          orderby p.No
                                          select p).ToList();
                        var qualityIdList = vm.QualityList.Select(d => d.Id).ToList();
                        //获取评价分组
                        var qualityItemGroupList = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                                                    where p.IsDeleted == false
                                                    && p.tbQuality.IsDeleted == false
                                                    && qualityIdList.Contains(p.tbQuality.Id)
                                                    select p).ToList();

                        var qualityItemGroupIdList = qualityItemGroupList.Select(d => d.Id).ToList();

                        //获取评价项
                        var qualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                               where p.IsDeleted == false
                                               && p.tbQualityItemGroup.IsDeleted == false
                                               && qualityItemGroupIdList.Contains(p.tbQualityItemGroup.Id)
                                               select p).ToList();

                        var qualityItemIdList = qualityItemList.Select(d => d.Id).ToList();

                        //获取评价选项
                        var qualityOtionList = (from p in db.Table<Quality.Entity.tbQualityOption>()
                                                where p.IsDeleted == false
                                                && p.tbQualityItem.IsDeleted == false
                                                && qualityItemIdList.Contains(p.tbQualityItem.Id)
                                                select p).ToList();

                        var qualityOtionIdList = qualityOtionList.Select(d => d.Id).ToList();

                        vm.QualityDataList = (from p in db.Table<Quality.Entity.tbQualityData>()
                                              where p.tbStudent.Id == vm.StudentId
                                              && qualityItemIdList.Contains(p.tbQualityItem.Id)
                                               && qualityOtionIdList.Contains(p.tbQualityOption.Id)
                                               && (p.tbQualityItem.QualityItemType == Code.EnumHelper.QualityItemType.Radio ||
                                               p.tbQualityItem.QualityItemType == Code.EnumHelper.QualityItemType.CheckBox)
                                              group p by new { QualityId = p.tbQualityItem.tbQualityItemGroup.tbQuality.Id, p.tbQualityItem.Id, p.tbQualityItem.QualityItemName } into g
                                              select new Dto.QualityReport.StudentReport
                                              {
                                                  QualityId = g.Key.QualityId,
                                                  QualityItemName = g.Key.QualityItemName,
                                                  OptionAvg = g.Average(d => d.tbQualityOption.OptionValue),
                                              }).ToList();
                        #endregion

                        #region 获取考勤统计
                        //获取学生学期所在教学班信息
                        vm.OrgList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                      where p.IsDeleted == false
                                      && p.tbStudent.IsDeleted == false
                                      && p.tbOrg.IsDeleted == false
                                      && p.tbOrg.tbYear.IsDeleted == false
                                      && yearSectionIdList.Contains(p.tbOrg.tbYear.Id)
                                      && p.tbStudent.Id == vm.StudentId
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbOrg.OrgName,
                                          Value = p.tbOrg.Id.ToString(),
                                      }).ToList();

                        vm.OrgList.AddRange((from p in db.Table<Course.Entity.tbOrg>()
                                             where p.IsDeleted == false
                                             && p.tbClass.IsDeleted == false
                                             && p.tbYear.IsDeleted == false
                                             && yearSectionIdList.Contains(p.tbYear.Id)
                                             && p.tbClass.Id == student.tbClass.Id
                                             orderby p.No
                                             select new System.Web.Mvc.SelectListItem
                                             {
                                                 Text = p.OrgName,
                                                 Value = p.Id.ToString(),
                                             }).ToList());
                        vm.OrgList = vm.OrgList.Distinct().ToList();

                        var orgIdList = vm.OrgList.Select(d => d.Value).ToList();

                        //获取考勤类型信息
                        vm.AttendanceTypeList = (from p in db.Table<Attendance.Entity.tbAttendanceType>()
                                                 where p.IsDeleted == false
                                                 orderby p.No
                                                 select p).ToList();

                        //获取考勤结果
                        vm.AttendanceList = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                  .Include(d => d.tbOrg)
                                  .Include(d => d.tbAttendanceType)
                                             where p.IsDeleted == false
                                             && p.tbOrg.IsDeleted == false
                                             && p.tbAttendanceType.IsDeleted == false
                                             && orgIdList.Contains(p.tbOrg.Id.ToString())
                                             select p).ToList();
                        #endregion

                        #region 获取评语
                        //获取我的评语
                        vm.SelfComment = (from p in db.Table<Quality.Entity.tbQualitySelf>()
                                    .Include(d => d.tbStudent)
                                          where p.IsDeleted == false
                                      && p.tbYear.IsDeleted == false
                                      && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                      && p.tbYear.Id == vm.YearTermId
                                          select new Dto.QualityReport.StudentReport
                                          {
                                              Comment = p.Content,
                                              InputDate = p.InputDate,
                                          }).FirstOrDefault();

                        //获取学期期待
                        vm.PlanComment = (from p in db.Table<Quality.Entity.tbQualityPlan>()
                                    .Include(d => d.tbStudent)
                                          where p.IsDeleted == false
                                      && p.tbYear.IsDeleted == false
                                      && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                      && p.tbYear.Id == vm.YearTermId
                                          select new Dto.QualityReport.StudentReport
                                          {
                                              Comment = p.Content,
                                              InputDate = p.InputDate,
                                          }).FirstOrDefault();

                        //获取学期总结
                        vm.SummaryComment = (from p in db.Table<Quality.Entity.tbQualitySummary>()
                                    .Include(d => d.tbStudent)
                                             where p.IsDeleted == false
                                         && p.tbYear.IsDeleted == false
                                         && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                         && p.tbYear.Id == vm.YearTermId
                                             select new Dto.QualityReport.StudentReport
                                             {
                                                 Comment = p.Content,
                                                 InputDate = p.InputDate,
                                             }).FirstOrDefault();

                        //获取班主任评语
                        vm.ClassComment = (from p in db.Table<Quality.Entity.tbQualityComment>()
                                    .Include(d => d.tbSysUser)
                                           where p.IsDeleted == false
                                       && p.tbYear.IsDeleted == false
                                       && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                       && p.tbYear.Id == vm.YearTermId
                                           select new Dto.QualityReport.StudentReport
                                           {
                                               Comment = p.Comment,
                                               InputDate = p.InputDate,
                                               UserName = "班主任：" + p.tbSysUser.UserName,
                                           }).FirstOrDefault();
                        //获取任课教师评语
                        vm.OrgTeacherComment = (from p in db.Table<Quality.Entity.tbQualityRemark>()
                                            .Include(d => d.tbSysUser)
                                                where p.IsDeleted == false
                                                && p.tbOrg.IsDeleted == false
                                            && p.tbOrg.tbYear.IsDeleted == false
                                            && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                            && p.tbOrg.tbYear.tbYearParent.Id == vm.YearTermId
                                                select new Dto.QualityReport.StudentReport
                                                {
                                                    Comment = p.Remark,
                                                    InputDate = p.InputDate,
                                                    UserName = p.tbOrg.OrgName + "：" + p.tbSysUser.UserName,
                                                }).ToList();
                        #endregion

                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassStudent(Models.QualityReport.ClassStudent vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassStudent", new
            {
                yearId = vm.YearId,
                gradeId = vm.GradeId,
                classId = vm.ClassId,
                studentId = vm.StudentId,
                yearTermId = vm.YearTermId,
                courseGroupItemId = vm.CourseGroupItemId,
            }));
        }
        #endregion

        #region 孩子综合素质报告单
        public ActionResult ChildReport()
        {
            if (Code.Common.UserType != Code.EnumHelper.SysUserType.Family)
            {
                return Content(Code.Common.Redirect(Url.Action("Index", "SysIndex", new { area = "Sys" }), "当前身份类别无法访问该功能!"));
            }

            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.QualityReport.ChildReport();
                //根据当前登录用户Id获取学生信息
                var student = (from p in db.Table<Basis.Entity.tbClassStudent>()
                               .Include(d => d.tbClass)
                               .Include(d => d.tbStudent)
                               .Include(d => d.tbClass.tbYear)
                               where p.IsDeleted == false
                               && p.tbClass.IsDeleted == false
                               && p.tbStudent.IsDeleted == false
                               && (p.tbStudent.tbSysUser.Id == Code.Common.UserId || p.tbStudent.tbSysUserFamily.Id == Code.Common.UserId)
                               select p).FirstOrDefault();
                if (student != null)
                {
                    //根据班级Id获取班主任信息
                    var teacher = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                               .Include(d => d.tbClass)
                               .Include(d => d.tbTeacher)
                                   where p.IsDeleted == false
                                   && p.tbClass.IsDeleted == false
                                   && p.tbTeacher.IsDeleted == false
                                   && p.tbClass.Id == student.tbClass.Id
                                   select p).FirstOrDefault();
                    vm.ClassName = student.tbClass.ClassName;
                    vm.StudentCode = student.tbStudent.StudentCode;
                    vm.StudentName = student.tbStudent.StudentName;
                    if (teacher != null)
                    {
                        vm.ClassTeacher = teacher.tbTeacher.TeacherName;
                    }

                    //根据学生Id获取年级
                    vm.GradeYearList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  .Include(d => d.tbClass.tbGrade)
                                  .Include(d => d.tbClass.tbYear)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                         && (p.tbStudent.tbSysUser.Id == Code.Common.UserId || p.tbStudent.tbSysUserFamily.Id == Code.Common.UserId)
                                        orderby p.tbClass.tbYear.No descending
                                        select p).ToList();
                    //获取学年ID
                    var yearIdList = vm.GradeYearList.Select(d => d.tbClass.tbYear.Id).ToList();
                    //获取学期
                    vm.YearTermList = (from p in db.Table<Basis.Entity.tbYear>()
                                        .Include(d => d.tbYearParent)
                                       where p.IsDeleted == false
                                       && yearIdList.Contains(p.tbYearParent.Id)
                                       select p).ToList();
                    var yearTermIdList = vm.YearTermList.Select(d => d.Id).ToList();
                    vm.YearSectionList = (from p in db.Table<Basis.Entity.tbYear>()
                                          .Include(d => d.tbYearParent)
                                          where p.IsDeleted == false
                                          && yearTermIdList.Contains(p.tbYearParent.Id)
                                          select p).ToList();
                    //获取年级
                    vm.GradeList = (from p in vm.GradeYearList
                                    orderby p.tbClass.tbGrade.No descending
                                    select new System.Web.Mvc.SelectListItem
                                    {
                                        Text = p.tbClass.tbGrade.GradeName,
                                        Value = p.tbClass.tbGrade.Id.ToString(),
                                    }).ToList();

                    //默认默认第一个年级Id去查询后面数据
                    if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                    {
                        vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                    }

                    //根据当前登录用户Id和年级Id获取学生信息
                    var gradeStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                         .Include(d => d.tbStudent)
                               .Include(d => d.tbClass.tbGrade)
                               .Include(d => d.tbClass.tbYear)
                                        where p.IsDeleted == false
                                        && p.tbClass.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && (p.tbStudent.tbSysUser.Id == Code.Common.UserId || p.tbStudent.tbSysUserFamily.Id == Code.Common.UserId)
                                        && p.tbClass.tbGrade.Id == vm.GradeId
                                        select p).FirstOrDefault();

                    //根据状态获取学期
                    if (gradeStudent != null)
                    {
                        decimal maxTotal = 0;
                        //获取学期
                        var yearTermList = (from p in db.Table<Basis.Entity.tbYear>()
                                            where p.tbYearParent.Id == gradeStudent.tbClass.tbYear.Id
                                            && p.IsDeleted == false
                                            orderby p.No
                                            select p).ToList();

                        //获取学段
                        if (vm.YearTermId == 0)
                        {
                            var yearSection = vm.YearSectionList.Where(d => d.IsDefault == true).FirstOrDefault();
                            if (yearSection != null)
                            {
                                vm.YearTermId = yearSection.tbYearParent.Id;
                            }
                        }
                        var yearSectionList = (from p in db.Table<Basis.Entity.tbYear>()
                                              .Include(d => d.tbYearParent)
                                               where p.tbYearParent.Id == vm.YearTermId
                                               && p.IsDeleted == false
                                               orderby p.No
                                               select p).ToList();
                        var yearSectionIdList = yearSectionList.Select(d => d.Id).ToList();

                        #region 成绩等级
                        //获取课程分组
                        vm.CourseGroupItemList = (from p in db.Table<Course.Entity.tbCourseGroup>()
                                                  where p.IsDeleted == false
                                                  orderby p.No
                                                  select new System.Web.Mvc.SelectListItem
                                                  {
                                                      Text = p.CourseGroupName,
                                                      Value = p.Id.ToString(),
                                                  }).ToList();
                        var courseGroupIdList = vm.CourseGroupItemList.Select(d => d.Value).ToList();

                        //获取领域
                        vm.CourseDomainItemList = (from p in db.Table<Course.Entity.tbCourseDomain>()
                                                   where p.IsDeleted == false
                                                   orderby p.No
                                                   select p).ToList();
                        var courseDomainItemIdList = vm.CourseDomainItemList.Select(d => d.Id).ToList();

                        //获取教学班课程、分组、领域信息
                        vm.CourseDomainList = (from p in db.Table<Course.Entity.tbOrg>()
                                               where p.IsDeleted == false
                                               && courseDomainItemIdList.Contains(p.tbCourse.tbCourseDomain.Id)
                                               && (vm.CourseGroupItemId <= 0 ? true : p.tbCourse.tbCourseGroup.Id == vm.CourseGroupItemId)
                                               && p.tbGrade.Id == vm.GradeId
                                               orderby p.tbCourse.tbCourseDomain.No
                                               select new Dto.QualityReport.StudentReport
                                               {
                                                   CourseId = p.tbCourse.Id,
                                                   CourseName = p.tbCourse.CourseName,
                                                   CourseGroupId = p.tbCourse.tbCourseGroup.Id,
                                                   CourseGroupName = p.tbCourse.tbCourseGroup.CourseGroupName,
                                                   CourseDomainName = p.tbCourse.tbCourseDomain.CourseDomainName,
                                                   CourseDomainNo = p.tbCourse.tbCourseDomain.No,
                                                   CourseDomainId = p.tbCourse.tbCourseDomain.Id,
                                               }).Distinct().ToList();

                        var courseIdList = vm.CourseDomainList.Select(d => d.CourseId).ToList();
                        var courseDomainIdList = vm.CourseDomainList.Select(d => d.CourseDomainId).Distinct().ToList();
                        var courseDomainNameList = vm.CourseDomainList.Select(d => d.CourseDomainName).Distinct().ToList();

                        //考试
                        vm.ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                       where p.IsPublish == true
                                       && p.IsDeleted == false
                                       && yearSectionIdList.Contains(p.tbYear.Id)
                                       select p).ToList();
                        var examIdList = vm.ExamList.Select(d => d.Id).ToList();

                        if (vm.ExamList.Count() > 0)
                        {
                            #region 获取个人积点
                            //获取个人积点
                            vm.ExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                               where p.tbExamCourse.IsDeleted == false
                                                && p.tbStudent.IsDeleted == false
                                                && p.tbExamCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.IsDeleted == false
                                                && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                                && p.tbExamCourse.tbExam.IsDeleted == false
                                                && examIdList.Contains(p.tbExamCourse.tbExam.Id)
                                                && courseIdList.Contains(p.tbExamCourse.tbCourse.Id)
                                                && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                               select new Dto.QualityReport.ChildReport
                                               {
                                                   CourseGroupId = p.tbExamCourse.tbCourse.tbCourseGroup.Id,
                                                   ExamId = p.tbExamCourse.tbExam.Id,
                                                   TotalMark = p.TotalMark,
                                                   CourseId = p.tbExamCourse.tbCourse.Id,
                                                   LevelName = p.tbExamLevel != null ? p.tbExamLevel.ExamLevelName : string.Empty,
                                                   LevelScore = p.tbExamLevel != null ? p.tbExamLevel.ExamLevelValue : 0,
                                                   CourseName = p.tbExamCourse.tbCourse.CourseName,
                                                   CourseDomainId = p.tbExamCourse.tbCourse.tbCourseDomain.Id,
                                               }).ToList();

                            //获取领域学生积点总分
                            var studentExamMarkList = (from p in vm.ExamMarkList
                                                       group p by new { p.CourseDomainId, p.ExamId } into g
                                                       select new
                                                       {
                                                           ExamId = g.Key.ExamId,
                                                           CourseDomainId = g.Key.CourseDomainId,
                                                           StudentScore = g.Sum(d => d.LevelScore),
                                                       }).ToList();
                            #endregion

                            #region 获取班级学生积点
                            //获取班级学生
                            var classStudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                            .Include(d => d.tbStudent)
                                  .Include(d => d.tbClass.tbGrade)
                                  .Include(d => d.tbClass.tbYear)
                                                    where p.IsDeleted == false
                                                    && p.tbClass.IsDeleted == false
                                                    && p.tbStudent.IsDeleted == false
                                                    && p.tbClass.Id == gradeStudent.tbClass.Id
                                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                                    select p).ToList();
                            //获取领域班级学生积点总分
                            var studentIdList = classStudentList.Select(d => d.tbStudent.Id).ToList();
                            var classExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                                     where p.tbExamCourse.IsDeleted == false
                                                      && p.tbStudent.IsDeleted == false
                                                      && p.tbExamCourse.IsDeleted == false
                                                      && p.tbExamCourse.tbCourse.IsDeleted == false
                                                      && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                                      && p.tbExamCourse.tbExam.IsDeleted == false
                                                      && examIdList.Contains(p.tbExamCourse.tbExam.Id)
                                                      && courseIdList.Contains(p.tbExamCourse.tbCourse.Id)
                                                      && studentIdList.Contains(p.tbStudent.Id)
                                                     group p by new { p.tbStudent.Id, CourseDomainId = p.tbExamCourse.tbCourse.tbCourseDomain.Id, ExamId = p.tbExamCourse.tbExam.Id } into g
                                                     select new
                                                     {
                                                         ExamId = g.Key.ExamId,
                                                         StudentId = g.Key.Id,
                                                         CourseDomainId = g.Key.CourseDomainId,
                                                         ClassScore = g.Sum(d => d.tbExamLevel.ExamLevelValue),
                                                     }).ToList();
                            //获取领域班级积点平均分
                            var classSumList = (from p in classExamMarkList
                                                group p by new { p.StudentId, p.CourseDomainId, p.ExamId } into g
                                                select new
                                                {
                                                    ExamId = g.Key.ExamId,
                                                    CourseDomainId = g.Key.CourseDomainId,
                                                    ClassScore = g.Sum(d => d.ClassScore),
                                                }).ToList();
                            //获取领域积点平均分
                            var classAvgList = (from p in classSumList
                                                group p by new { p.CourseDomainId, p.ExamId } into g
                                                select new
                                                {
                                                    ExamId = g.Key.ExamId,
                                                    CourseDomainId = g.Key.CourseDomainId,
                                                    ClassAvg = g.Average(d => d.ClassScore),
                                                }).ToList();

                            //获取每个学生积点总分
                            var classStudentMarkList = (from p in classExamMarkList
                                                        group p by new { p.StudentId, p.ExamId } into g
                                                        select new
                                                        {
                                                            ExamId = g.Key.ExamId,
                                                            StudentId = g.Key.StudentId,
                                                            ClassScore = g.Sum(d => d.ClassScore),
                                                        }).ToList();
                            //获取班级积点平均分
                            vm.ClassLevelAvgList = (from p in classStudentMarkList
                                                    join t in classStudentList on p.StudentId equals t.tbStudent.Id
                                                    group p by new { t.tbClass.Id, p.ExamId } into g
                                                    select new Dto.QualityReport.ChildReport
                                                    {
                                                        ExamId = g.Key.ExamId,
                                                        ClassLevelAvg = g.Average(d => d.ClassScore),
                                                    }).ToList();
                            #endregion

                            #region 绑定雷达图数据
                            maxTotal = 0;
                            foreach (var exam in vm.ExamList)
                            {
                                var demainList = new List<object>();
                                var sList = new List<object>();
                                var cList = new List<object>();
                                var gList = new List<object>();
                                var tList = new List<object>();
                                var i = 0;
                                //maxTotal = studentExamMarkList.Where(d=>d.ExamId==exam.Id).Sum(d => d.StudentScore);
                                //sList.Add(maxTotal.ToString("0.00"));
                                foreach (var courseDomainItem in vm.CourseDomainItemList)
                                {
                                    var studentTotal = studentExamMarkList.Where(d => d.CourseDomainId == courseDomainItem.Id && d.ExamId == exam.Id).FirstOrDefault();

                                    var classTotal = classAvgList.Where(d => d.CourseDomainId == courseDomainItem.Id && d.ExamId == exam.Id).FirstOrDefault();
                                    if (i == 0)
                                    {
                                        if (studentExamMarkList.Where(d => d.CourseDomainId == courseDomainItem.Id && d.ExamId == exam.Id).Sum(d => d.StudentScore) > maxTotal)
                                        {
                                            maxTotal = studentExamMarkList.Where(d => d.CourseDomainId == courseDomainItem.Id && d.ExamId == exam.Id).Sum(d => d.StudentScore);
                                        }
                                        sList.Add(studentExamMarkList.Where(d => d.ExamId == exam.Id).Sum(d => d.StudentScore).ToString("0.00"));
                                        if (studentTotal != null)
                                        {
                                            sList.Add(studentTotal.StudentScore.ToString("0.00"));
                                        }
                                        else
                                        {
                                            sList.Add(0);
                                        }
                                        if (classAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.ClassAvg) > maxTotal)
                                        {
                                            maxTotal = classAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.ClassAvg);
                                        }
                                        cList.Add(classAvgList.Where(d => d.ExamId == exam.Id).Sum(d => d.ClassAvg).ToString("0.00"));
                                        if (classTotal != null)
                                        {
                                            cList.Add(classTotal.ClassAvg.ToString("0.00"));
                                        }
                                        else
                                        {
                                            cList.Add(0);
                                        }

                                    }
                                    else
                                    {
                                        if (studentTotal != null)
                                        {
                                            if (studentTotal.StudentScore > maxTotal)
                                            {
                                                maxTotal = studentTotal.StudentScore;
                                            }
                                            sList.Add(studentTotal.StudentScore.ToString("0.00"));
                                        }
                                        else
                                        {
                                            sList.Add("0.00");
                                        }

                                        if (classTotal != null)
                                        {
                                            if (classTotal.ClassAvg > maxTotal)
                                            {
                                                maxTotal = classTotal.ClassAvg;
                                            }
                                            cList.Add(classTotal.ClassAvg.ToString("0.00"));
                                        }
                                        else
                                        {
                                            cList.Add(0);
                                        }
                                    }
                                    i++;
                                }
                                demainList.Add(new { name = "个人", value = sList });
                                demainList.Add(new { name = "班级均分", value = cList });
                                var courseDemainAvg = new Dto.QualityReport.ChildReport();
                                courseDemainAvg.ReportName = Code.Common.ToJSONString(demainList);
                                courseDemainAvg.ExamId = exam.Id;
                                vm.CourseDemainAvgList.Add(courseDemainAvg);

                            }
                            var courseDemainList = new List<object>();
                            //courseDemainList.Add(new { name = "总评", max = (maxTotal > 0 ? maxTotal : 5) });
                            courseDemainList.Add(new { name = "总评", max = 100 });
                            foreach (var courseDemainName in courseDomainNameList)
                            {
                                //courseDemainList.Add(new { name = courseDemainName, max = (maxTotal > 0 ? maxTotal : 5) });
                                courseDemainList.Add(new { name = courseDemainName, max = 100 });
                            }
                            vm.CourseDemainNames = Code.Common.ToJSONString(courseDemainList);
                            #endregion
                        }
                        #endregion

                        #region 荣誉
                        //获取荣誉
                        vm.HonorList = (from p in db.Table<Student.Entity.tbStudentHonor>()
                                                .Include(d => d.tbstudentHonorLevel)
                                                .Include(d => d.tbStudentHonorType)
                                        where p.IsDeleted == false
                                        && p.tbstudentHonorLevel.IsDeleted == false
                                        && p.tbStudentHonorType.IsDeleted == false
                                        && p.tbYear.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && yearSectionIdList.Contains(p.tbYear.Id)
                                        && p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                        select new Dto.QualityReport.ChildReport
                                        {
                                            HonorName = p.HonorName,
                                            StudentHonorLevelName = p.tbstudentHonorLevel.StudentHonorLevelName,
                                            StudentHonorTypeName = p.tbStudentHonorType.StudentHonorTypeName,
                                            InputDate = p.InputDate,
                                        }).ToList();
                        #endregion

                        #region 获取评价
                        //获取评价
                        vm.QualityList = (from p in db.Table<Quality.Entity.tbQuality>()
                                          where p.IsDeleted == false
                                          && yearSectionIdList.Contains(p.tbYear.Id)
                                          && p.IsOpen == true
                                          orderby p.No
                                          select p).ToList();
                        var qualityIdList = vm.QualityList.Select(d => d.Id).ToList();
                        //获取评价分组
                        var qualityItemGroupList = (from p in db.Table<Quality.Entity.tbQualityItemGroup>()
                                                    where p.IsDeleted == false
                                                    && p.tbQuality.IsDeleted == false
                                                    && qualityIdList.Contains(p.tbQuality.Id)
                                                    select p).ToList();

                        var qualityItemGroupIdList = qualityItemGroupList.Select(d => d.Id).ToList();

                        //获取评价项
                        var qualityItemList = (from p in db.Table<Quality.Entity.tbQualityItem>()
                                               where p.IsDeleted == false
                                               && p.tbQualityItemGroup.IsDeleted == false
                                               && qualityItemGroupIdList.Contains(p.tbQualityItemGroup.Id)
                                               select p).ToList();

                        var qualityItemIdList = qualityItemList.Select(d => d.Id).ToList();

                        //获取评价选项
                        var qualityOtionList = (from p in db.Table<Quality.Entity.tbQualityOption>()
                                                where p.IsDeleted == false
                                                && p.tbQualityItem.IsDeleted == false
                                                && qualityItemIdList.Contains(p.tbQualityItem.Id)
                                                select p).ToList();

                        var qualityOtionIdList = qualityOtionList.Select(d => d.Id).ToList();

                        vm.QualityDataList = (from p in db.Table<Quality.Entity.tbQualityData>()
                                              where p.tbStudent.tbSysUser.Id == Code.Common.UserId
                                               && qualityItemIdList.Contains(p.tbQualityItem.Id)
                                                && qualityOtionIdList.Contains(p.tbQualityOption.Id)
                                                && (p.tbQualityItem.QualityItemType == Code.EnumHelper.QualityItemType.Radio ||
                                                p.tbQualityItem.QualityItemType == Code.EnumHelper.QualityItemType.CheckBox)
                                              group p by new { QualityId = p.tbQualityItem.tbQualityItemGroup.tbQuality.Id, p.tbQualityItem.Id, p.tbQualityItem.QualityItemName } into g
                                              select new Dto.QualityReport.ChildReport
                                              {
                                                  QualityId = g.Key.QualityId,
                                                  QualityItemName = g.Key.QualityItemName,
                                                  OptionAvg = g.Average(d => d.tbQualityOption.OptionValue),
                                              }).ToList();
                        #endregion

                        #region 获取考勤统计
                        //获取学生学期所在教学班信息
                        vm.OrgList = (from p in db.Table<Course.Entity.tbOrgStudent>()
                                      where p.IsDeleted == false
                                      && p.tbStudent.IsDeleted == false
                                      && p.tbOrg.IsDeleted == false
                                      && p.tbOrg.tbYear.IsDeleted == false
                                      && yearSectionIdList.Contains(p.tbOrg.tbYear.Id)
                                      && (p.tbStudent.tbSysUser.Id == Code.Common.UserId || p.tbStudent.tbSysUserFamily.Id == Code.Common.UserId)
                                      orderby p.No
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Text = p.tbOrg.OrgName,
                                          Value = p.tbOrg.Id.ToString(),
                                      }).ToList();

                        vm.OrgList.AddRange((from p in db.Table<Course.Entity.tbOrg>()
                                             where p.IsDeleted == false
                                             && p.tbClass.IsDeleted == false
                                             && p.tbYear.IsDeleted == false
                                             && yearSectionIdList.Contains(p.tbYear.Id)
                                             && p.tbClass.Id == student.tbClass.Id
                                             orderby p.No
                                             select new System.Web.Mvc.SelectListItem
                                             {
                                                 Text = p.OrgName,
                                                 Value = p.Id.ToString(),
                                             }).ToList());
                        vm.OrgList = vm.OrgList.Distinct().ToList();

                        var orgIdList = vm.OrgList.Select(d => d.Value).ToList();

                        //获取考勤类型信息
                        vm.AttendanceTypeList = (from p in db.Table<Attendance.Entity.tbAttendanceType>()
                                                 where p.IsDeleted == false
                                                 orderby p.No
                                                 select p).ToList();

                        //获取考勤结果
                        vm.AttendanceList = (from p in db.Table<Attendance.Entity.tbAttendance>()
                                  .Include(d => d.tbOrg)
                                  .Include(d => d.tbAttendanceType)
                                             where p.IsDeleted == false
                                             && p.tbOrg.IsDeleted == false
                                             && p.tbAttendanceType.IsDeleted == false
                                             && orgIdList.Contains(p.tbOrg.Id.ToString())
                                             select p).ToList();
                        #endregion

                        #region 获取评语
                        //获取我的评语
                        vm.SelfComment = (from p in db.Table<Quality.Entity.tbQualitySelf>()
                                    .Include(d => d.tbStudent)
                                          where p.IsDeleted == false
                                      && p.tbYear.IsDeleted == false
                                      && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                      && p.tbYear.Id == vm.YearTermId
                                          select new Dto.QualityReport.ChildReport
                                          {
                                              Comment = p.Content,
                                              InputDate = p.InputDate,
                                          }).FirstOrDefault();

                        //获取学期期待
                        vm.PlanComment = (from p in db.Table<Quality.Entity.tbQualityPlan>()
                                    .Include(d => d.tbStudent)
                                          where p.IsDeleted == false
                                      && p.tbYear.IsDeleted == false
                                      && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                      && p.tbYear.Id == vm.YearTermId
                                          select new Dto.QualityReport.ChildReport
                                          {
                                              Comment = p.Content,
                                              InputDate = p.InputDate,
                                          }).FirstOrDefault();

                        //获取学期总结
                        vm.SummaryComment = (from p in db.Table<Quality.Entity.tbQualitySummary>()
                                    .Include(d => d.tbStudent)
                                             where p.IsDeleted == false
                                         && p.tbYear.IsDeleted == false
                                         && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                         && p.tbYear.Id == vm.YearTermId
                                             select new Dto.QualityReport.ChildReport
                                             {
                                                 Comment = p.Content,
                                                 InputDate = p.InputDate,
                                             }).FirstOrDefault();

                        //获取班主任评语
                        vm.ClassComment = (from p in db.Table<Quality.Entity.tbQualityComment>()
                                    .Include(d => d.tbSysUser)
                                           where p.IsDeleted == false
                                       && p.tbYear.IsDeleted == false
                                       && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                       && p.tbYear.Id == vm.YearTermId
                                           select new Dto.QualityReport.ChildReport
                                           {
                                               Comment = p.Comment,
                                               InputDate = p.InputDate,
                                               UserName = "班主任：" + p.tbSysUser.UserName,
                                           }).FirstOrDefault();
                        //获取任课教师评语
                        vm.OrgTeacherComment = (from p in db.Table<Quality.Entity.tbQualityRemark>()
                                            .Include(d => d.tbSysUser)
                                                where p.IsDeleted == false
                                                && p.tbOrg.IsDeleted == false
                                            && p.tbOrg.tbYear.IsDeleted == false
                                            && p.tbStudent.Id == gradeStudent.tbStudent.Id
                                            && p.tbOrg.tbYear.tbYearParent.Id == vm.YearTermId
                                                select new Dto.QualityReport.ChildReport
                                                {
                                                    Comment = p.Remark,
                                                    InputDate = p.InputDate,
                                                    UserName = p.tbOrg.OrgName + "：" + p.tbSysUser.UserName,
                                                }).ToList();
                        #endregion


                    }
                }

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChildReport(Models.QualityReport.ChildReport vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ChildReport", new
            {
                gradeId = vm.GradeId,
                yearTermId = vm.YearTermId,
                courseGroupItemId = vm.CourseGroupItemId,
            }));
        }
        #endregion
    }
}