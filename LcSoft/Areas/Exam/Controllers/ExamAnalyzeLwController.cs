using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
//罗外
namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamAnalyzeLwController : Controller
    {
        #region 基本分析按科目统计 按班级
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                vm.OptionList = new List<string>() { "班级", "教师", "实考数", "平均分", "最高分", "最低分", "优秀率%", "良好率%", "及格率%", "均分排名", "优率排名", "良好率排名", "及格率排名" };
                vm.ClumnList = new List<string>() { "均分进退", "优秀进退", "良好进退", "及格进退" };
                var lsExam = new List<int>();
                //本次考试
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }
                lsExam.Add(vm.ExamId);
                //上次考试
                vm.LastExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.LastExamId == 0 && vm.LastExamList.Count > 0)
                {
                    vm.LastExamId = vm.LastExamList.FirstOrDefault().Value.ConvertToInt();
                }
                lsExam.Add(vm.LastExamId);

                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }
                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                  where p.tbCourse.IsDeleted == false
                                  && p.tbGrade.IsDeleted == false
                                  && p.tbGrade.Id == vm.GradeId
                                  select p.tbCourse.Id).Distinct().ToList();
                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == vm.ExamId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && courseIdList.Contains(p.tbCourse.Id)
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(vm.SearchText) || vm.SearchText == null)
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id,
                                           SubjectNo=p.tbCourse.tbSubject.No,
                                           CourseId = p.tbCourse.Id
                                       }).Distinct().ToList();

                var SubjectList = (from p in examSubjectList
                                   orderby p.SubjectNo
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.SubjectId
                                   }).Distinct().ToList();

                vm.SubjectList = (from p in SubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                vm.SubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                YearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();

                if (year == null) return View(vm);

                vm.ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = year.ExamName, Value = vm.ExamId.ToString() });

                var lastExam = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == vm.LastExamId
                                select new
                                {
                                    p.ExamName
                                }).FirstOrDefault();

                vm.ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = lastExam.ExamName, Value = vm.LastExamId.ToString() });


                //任课老师
                var CourseIds = examSubjectList.Select(d => d.CourseId).Distinct().ToList();
                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                     .Include(d => d.tbOrg.tbClass)
                                      where p.tbOrg.IsDeleted == false
                                      && CourseIds.Contains(p.tbOrg.tbCourse.Id)
                                      && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
                                      && p.tbOrg.tbYear.Id == year.Id
                                      && p.tbTeacher.IsDeleted == false
                                      select new Dto.ExamAnalyze.List
                                      {
                                          SubjectId = p.tbOrg.tbCourse.tbSubject.Id,
                                          ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();
                //班主任
                var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                        where p.tbClass.IsDeleted == false
                                        && p.tbClass.tbYear.Id == year.YearId
                                        && p.tbTeacher.IsDeleted == false
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = p.tbClass.Id,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).ToList();

                vm.SubjectTeacherList = orgTeacherList;
                vm.ClassTeacherList = classTeacherList;

                if (vm.chkSubject == null) return View(vm);
                var chksubjectList = vm.chkSubject.Split(',');
                //分数段分组
                var examSegmentGroupList = (from p in db.Table<Exam.Entity.tbExam>().Include(d=>d.tbExamSegmentGroup)
                                    where lsExam.Contains(p.Id)
                                    select new
                                    {
                                        ExamId=p.Id,
                                        SegmentGroupId= p.tbExamSegmentGroup!=null?p.tbExamSegmentGroup.Id:0
                                    }).ToList();
                var SegmentGroupIds = examSegmentGroupList.Select(d => d.SegmentGroupId).Distinct().ToList();
                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject)
                                       where p.tbGrade.Id == vm.GradeId
                                        && (chksubjectList.Contains(p.tbSubject.Id.ToString()) || p.tbSubject == null)
                                        && SegmentGroupIds.Contains(p.tbExamSegmentGroup.Id)
                                       select new
                                       {
                                           SegmentGroupId = p.tbExamSegmentGroup.Id,
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsNormal,
                                           p.IsPass,
                                           p.IsTotal,
                                           SubjectId = p.tbSubject != null ? p.tbSubject.Id : 0
                                       }).ToList();

                //选中的科目
                var selectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                         where chksubjectList.Contains(p.Id.ToString())
                                         orderby p.No, p.SubjectName
                                         select new
                                         {
                                             p.Id,
                                             p.SubjectName,
                                         }).Distinct().ToList();

                vm.selectSubjectList = (from p in selectSubjectList
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.SubjectName,
                                        }).Distinct().ToList();
                if (chksubjectList.Contains("0"))
                {
                    vm.selectSubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });
                }

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var classtudentList = (from p in classStudent
                                       select new
                                       {
                                           p.ClassId,
                                           p.ClassName
                                       }).Distinct().ToList();

                vm.ClassStudentList = (from p in classtudentList
                                       select new Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           ClassName = p.ClassName,
                                       }).Distinct().ToList();
                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.TotalMark != null
                                     && lsExam.Contains(p.tbExamCourse.tbExam.Id)
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        p.tbExamCourse.FullTotalMark
                                    }).ToList();


                #region  本次成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in classStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                             FullTotalMark = p.FullTotalMark
                                         }).ToList();
                //单个班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ExamId,
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ExamId = g.Key.ExamId,
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           StudentCount = g.Count().ToString(),
                                           AvgMark = g.Average(d => d.TotalMark),
                                           MaxMark = g.Max(d => d.TotalMark),
                                           MinMark = g.Min(d => d.TotalMark)
                                       }).ToList();
                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ExamId,
                                                p.ClassId,
                                                p.StudentId
                                            } into g
                                            select new
                                            {
                                                g.Key.ExamId,
                                                g.Key.ClassId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();
                var ty = (from p in totalStudentMarkList
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              StudentCount = g.Count()
                          }).ToList();
                var totalMarkList = (from p in totalStudentMarkList
                                     group p by new { p.ClassId, p.ExamId } into g
                                     select new Exam.Dto.ExamAnalyze.List
                                     {
                                         ExamId = g.Key.ExamId,
                                         ClassId = g.Key.ClassId,
                                         StudentCount = ty.Where(d => d.ExamId == g.Key.ExamId && d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                         AvgMark = g.Average(d => d.StudentTotalMark),
                                         MaxMark = g.Max(d => d.StudentTotalMark),
                                         MinMark = g.Min(d => d.StudentTotalMark)
                                     }).ToList();
                //总年级
                var gradeSubjectMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ExamId,
                                                p.SubjectId
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                ExamId = g.Key.ExamId,
                                                SubjectId = g.Key.SubjectId,
                                                ClassId = 0,
                                                StudentCount = g.Count().ToString(),
                                                AvgMark = g.Average(d => d.TotalMark),
                                                MaxMark = g.Max(d => d.TotalMark),
                                                MinMark = g.Min(d => d.TotalMark)
                                            }).ToList();

                var totalGradeStudentMarkList = (from p in classExamMarkList
                                                 group p by new
                                                 {
                                                     p.ExamId,
                                                     p.StudentId
                                                 } into g
                                                 select new
                                                 {
                                                     g.Key.ExamId,
                                                     ClassId = 0,
                                                     StudentTotalMark = g.Sum(d => d.TotalMark)
                                                 }).ToList();
                var tyGrade = (from p in totalStudentMarkList
                               group p by new
                               {
                                   p.ExamId
                               } into g
                               select new
                               {
                                   g.Key.ExamId,
                                   ClassId = 0,
                                   StudentCount = g.Count()
                               }).ToList();

                var totalGradeMarkList = (from p in totalGradeStudentMarkList
                                          group p by new { p.ClassId, p.ExamId } into g
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = g.Key.ExamId,
                                              ClassId = g.Key.ClassId,
                                              StudentCount = tyGrade.Where(d => d.ExamId == g.Key.ExamId && d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                              AvgMark = decimal.Round(g.Average(d => d.StudentTotalMark).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                              MaxMark = g.Max(d => d.StudentTotalMark),
                                              MinMark = g.Min(d => d.StudentTotalMark)
                                          }).ToList();

                #region 优秀良好及格排名
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var examid in lsExam.Distinct())
                {
                    var segmentGroupId = examSegmentGroupList.Where(d => d.ExamId == examid).Select(d => d.SegmentGroupId).FirstOrDefault();
                    foreach (var o in SegmentMarkList.Where(d=>d.SegmentGroupId==segmentGroupId))
                    {
                        var isGood = o.IsGood;
                        var isPass = o.IsPass;
                        var isNormal = o.IsNormal;
                        var isTotal = o.IsTotal;
                        if (isTotal && o.SubjectId == 0)//总分
                        {
                            if (isGood)
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d=>d.ExamId==examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = decimal.One,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isNormal)//良好人数
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d=>d.ExamId==examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId,
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = 2,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isPass)//及格人数
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d => d.ExamId == examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = 3,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                        }
                        else//各科目
                        {
                            //优秀科目人数
                            if (isGood)
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = decimal.One,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isNormal)//良好人数
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId,
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = 2,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isPass)//及格人数
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = 3,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                        }
                    }
                }
                
                var tk = (from p in lst
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                              p.SubjectId,
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              g.Key.SubjectId,
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum)
                          }).ToList();
                //年级项
                var tkGrade = (from p in tk
                               group p by new
                               {
                                   p.ExamId,
                                   p.SubjectId,
                                   p.Status
                               } into g
                               select new
                               {
                                   g.Key.ExamId,
                                   g.Key.SubjectId,
                                   ClassId = 0,
                                   g.Key.Status,
                                   StudentNum = g.Sum(d => d.StudentNum)
                               }).ToList();

                var exam = (from p in subjectMarkList.Union(totalMarkList)
                            select new
                            {
                                ExamId = p.ExamId,
                                ClassId = p.ClassId,
                                SubjectId = p.SubjectId,
                                StudentCount = p.StudentCount.ToString(),
                                AvgMark = p.AvgMark,
                                MaxMark = p.MaxMark,
                                MinMark = p.MinMark,
                                GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                            }).ToList();

                var examGrade = (from p in gradeSubjectMarkList.Union(totalGradeMarkList)
                                 select new Dto.ExamAnalyze.List
                                 {
                                     ExamId = p.ExamId,
                                     ClassId = p.ClassId,
                                     SubjectId = p.SubjectId,
                                     StudentCount = p.StudentCount.ToString(),
                                     AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                     MaxMark = p.MaxMark,
                                     MinMark = p.MinMark,
                                     GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                     NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                     PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                 }).ToList();

                //科目班级排名
                var lsComm = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in exam)
                {
                    var temp = new Exam.Dto.ExamAnalyze.List
                    {
                        ExamId = o.ExamId,
                        ClassId = o.ClassId,
                        SubjectId = o.SubjectId,
                        AvgRank = 0,
                        GoodRank = 0,
                        NormalRank = 0,
                        PassRank = 0
                    };
                    lsComm.Add(temp);
                }
                foreach (var examId in lsExam)
                {
                    foreach (var subject in vm.selectSubjectList)
                    {
                        var rank = 0;
                        decimal? mark = null;
                        var count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.AvgRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.GoodRate))
                        {
                            if (mark != t.GoodRate)
                            {
                                mark = t.GoodRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.GoodRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.NormalRate))
                        {
                            if (mark != t.NormalRate)
                            {
                                mark = t.NormalRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.NormalRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.PassRate))
                        {
                            if (mark != t.PassRate)
                            {
                                mark = t.PassRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.PassRank = rank;
                            }
                        }
                    }
                }


                #endregion
                vm.ExamGradeAnalyzeList = examGrade;

                vm.ExamAnalyzeList = (from p in exam
                                      select new Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = p.SubjectId,
                                          StudentCount = p.StudentCount,
                                          AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                          MaxMark = p.MaxMark,
                                          MinMark = p.MinMark,
                                          GoodRate = p.GoodRate,
                                          NormalRate = p.NormalRate,
                                          PassRate = p.PassRate,
                                          AvgRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.AvgRank).FirstOrDefault(),
                                          GoodRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.GoodRank).FirstOrDefault(),
                                          NormalRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.NormalRank).FirstOrDefault(),
                                          PassRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.PassRank).FirstOrDefault(),
                                      }).ToList();

                #endregion

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var CheckedAll = Request["CheckedAll"] != null ? Request.Form["CheckedAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("List", new { ExamId = vm.ExamId, lastexamId = vm.LastExamId, GradeId = vm.GradeId, CheckedAll = CheckedAll, chkSubject = chksubjectList, searchText = vm.SearchText }));
        }

        public ActionResult ClassAnalyzeList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                vm.OptionList = new List<string>() { "教师", "科目", "实考数", "平均分", "最高分", "最低分", "优秀率%", "良好率%", "及格率%", "均分排名", "优率排名", "良好率排名", "及格率排名" };
                vm.ClumnList = new List<string>() { "均分进退", "优秀进退", "良好进退", "及格进退" };
                var lsExam = new List<int>();
                //本次考试
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }
                lsExam.Add(vm.ExamId);
                //上次考试
                vm.LastExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.LastExamId == 0 && vm.LastExamList.Count > 0)
                {
                    vm.LastExamId = vm.LastExamList.FirstOrDefault().Value.ConvertToInt();
                }
                lsExam.Add(vm.LastExamId);

                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }
                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == vm.GradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == vm.ExamId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && courseIdList.Contains(p.tbCourse.Id)
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(vm.SearchText) || vm.SearchText == null)
                                       orderby p.tbCourse.tbSubject.No
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id,
                                           CourseId = p.tbCourse.Id
                                       }).Distinct().ToList();
                var SubjectList = (from p in examSubjectList
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.SubjectId
                                   }).Distinct().ToList();

                var SubjectIds = SubjectList.Select(d => d.SubjectId).ToList();

                vm.SubjectList = (from p in SubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                vm.SubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                YearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();

                if (year == null) return View(vm);

                //班级
                var tbClassStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                      where p.tbStudent.IsDeleted == false
                                      && p.tbClass.tbGrade.Id == vm.GradeId
                                      && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                      orderby p.tbClass.No, p.tbClass.ClassName
                                      select new
                                      {
                                          ClassId = p.tbClass.Id,
                                          p.tbClass.ClassName
                                      }).Distinct().ToList(); ;

                vm.ClassList = (from p in tbClassStudent
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Value = p.ClassId.ToString(),
                                    Text = p.ClassName,
                                }).Distinct().ToList();

                vm.ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = year.ExamName, Value = vm.ExamId.ToString() });

                var lastExam = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == vm.LastExamId
                                select new
                                {
                                    p.ExamName
                                }).FirstOrDefault();

                vm.ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = lastExam.ExamName, Value = vm.LastExamId.ToString() });


                //任课老师
                var CourseIds = examSubjectList.Select(d => d.CourseId).Distinct().ToList();
                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                     .Include(d => d.tbOrg.tbClass)
                                      where p.tbOrg.IsDeleted == false
                                      && CourseIds.Contains(p.tbOrg.tbCourse.Id)
                                      && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
                                      && p.tbOrg.tbYear.Id == year.Id
                                      && p.tbTeacher.IsDeleted == false
                                      select new Dto.ExamAnalyze.List
                                      {
                                          SubjectId = p.tbOrg.tbCourse.tbSubject.Id,
                                          ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();
                //班主任
                var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                        where p.tbClass.IsDeleted == false
                                        && p.tbClass.tbYear.Id == year.YearId
                                        && p.tbTeacher.IsDeleted == false
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = p.tbClass.Id,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).ToList();

                if (vm.chkClass == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');

                //分数段分组
                var examSegmentGroupList = (from p in db.Table<Exam.Entity.tbExam>().Include(d => d.tbExamSegmentGroup)
                                            where lsExam.Contains(p.Id)
                                            select new
                                            {
                                                ExamId = p.Id,
                                                SegmentGroupId = p.tbExamSegmentGroup != null ? p.tbExamSegmentGroup.Id : 0
                                            }).ToList();
                var SegmentGroupIds = examSegmentGroupList.Select(d => d.SegmentGroupId).Distinct().ToList();

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject)
                                       where p.tbGrade.Id == vm.GradeId
                                        && (SubjectIds.Contains(p.tbSubject.Id) || p.tbSubject == null)
                                        && SegmentGroupIds.Contains(p.tbExamSegmentGroup.Id)
                                       select new
                                       {
                                           SegmentGroupId=p.tbExamSegmentGroup.Id,
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsNormal,
                                           p.IsPass,
                                           p.IsTotal,
                                           SubjectId = p.tbSubject != null ? p.tbSubject.Id : 0
                                       }).ToList();

                //选中班级
                var tbselctClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                        where chkclassList.Contains(p.Id.ToString())
                                        orderby p.No
                                        select new
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.ClassName,
                                        }).ToList();
                vm.selctClassList = (from p in tbselctClassList
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Value = p.Value,
                                         Text = p.Text,
                                     }).Distinct().ToList();


                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                    && chkclassList.Contains(p.tbClass.Id.ToString())
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                vm.SubjectTeacherList = orgTeacherList;
                vm.ClassTeacherList = classTeacherList;

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.TotalMark != null
                                     && p.tbStudent.IsDeleted == false
                                     && lsExam.Contains(p.tbExamCourse.tbExam.Id)
                                     && SubjectIds.Contains(p.tbExamCourse.tbCourse.tbSubject.Id)
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                                    }).ToList();


                #region  本次成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in classStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName
                                         }).ToList();
                //单个班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ExamId,
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ExamId = g.Key.ExamId,
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           StudentCount = g.Count().ToString(),
                                           AvgMark = g.Average(d => d.TotalMark),
                                           MaxMark = g.Max(d => d.TotalMark),
                                           MinMark = g.Min(d => d.TotalMark)
                                       }).ToList();
                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ExamId,
                                                p.ClassId,
                                                p.StudentId
                                            } into g
                                            select new
                                            {
                                                g.Key.ExamId,
                                                g.Key.ClassId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();

                //总年级平均分
                var gradeSubjectTotalMarkList = (from p in classExamMarkList
                                                 group p by new
                                                 {
                                                     p.ExamId,
                                                     p.SubjectId
                                                 } into g
                                                 select new Exam.Dto.ExamAnalyze.List
                                                 {
                                                     ExamId = g.Key.ExamId,
                                                     ClassId = 0,
                                                     SubjectId = g.Key.SubjectId,
                                                     AvgMark = g.Average(d => d.TotalMark),
                                                 }).ToList();
                var gradeTotalMarkList = (from p in totalStudentMarkList
                                            group p by new
                                            {
                                                p.ExamId
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                ExamId = g.Key.ExamId,
                                                ClassId = 0,
                                                SubjectId = 0,
                                                AvgMark = g.Average(d => d.StudentTotalMark),
                                            }).ToList();

                var ty = (from p in totalStudentMarkList
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              StudentCount = g.Count()
                          }).ToList();
                var totalMarkList = (from p in totalStudentMarkList
                                     group p by new { p.ClassId, p.ExamId } into g
                                     select new Exam.Dto.ExamAnalyze.List
                                     {
                                         ExamId = g.Key.ExamId,
                                         ClassId = g.Key.ClassId,
                                         StudentCount = ty.Where(d => d.ExamId == g.Key.ExamId && d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                         AvgMark = g.Average(d => d.StudentTotalMark),
                                         MaxMark = g.Max(d => d.StudentTotalMark),
                                         MinMark = g.Min(d => d.StudentTotalMark)
                                     }).ToList();

                #region 优秀良好及格排名
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var examid in lsExam.Distinct())
                {
                    var segmentGroupId = examSegmentGroupList.Where(d => d.ExamId == examid).Select(d => d.SegmentGroupId).FirstOrDefault();
                    foreach (var o in SegmentMarkList.Where(d => d.SegmentGroupId == segmentGroupId))
                    {
                        var isGood = o.IsGood;
                        var isPass = o.IsPass;
                        var isNormal = o.IsNormal;
                        var isTotal = o.IsTotal;
                        if (isTotal && o.SubjectId == 0)//总分
                        {
                            if (isGood)
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d=>d.ExamId==examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = decimal.One,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isNormal)//良好人数
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d => d.ExamId == examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId,
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = 2,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isPass)//及格人数
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d => d.ExamId == examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = 3,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                        }
                        else//各科目
                        {
                            //优秀科目人数
                            if (isGood)
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = decimal.One,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isNormal)//良好人数
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId,
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = 2,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isPass)//及格人数
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = 3,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                        }
                    }
                }
                var tk = (from p in lst
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                              p.SubjectId,
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              g.Key.SubjectId,
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum)
                          }).ToList();

                var exam = (from p in subjectMarkList.Union(totalMarkList)
                            select new
                            {
                                ExamId = p.ExamId,
                                ClassId = p.ClassId,
                                SubjectId = p.SubjectId,
                                StudentCount = p.StudentCount.ToString(),
                                AvgMark = p.AvgMark,
                                MaxMark = p.MaxMark,
                                MinMark = p.MinMark,
                                GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                            }).ToList();


                //科目班级排名
                var lsComm = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in exam)
                {
                    var temp = new Exam.Dto.ExamAnalyze.List
                    {
                        ExamId = o.ExamId,
                        ClassId = o.ClassId,
                        SubjectId = o.SubjectId,
                        AvgRank = 0,
                        GoodRank = 0,
                        NormalRank = 0,
                        PassRank = 0
                    };
                    lsComm.Add(temp);
                }
                foreach (var examId in lsExam)
                {
                    foreach (var subject in vm.SubjectList)
                    {
                        var rank = 0;
                        decimal? mark = null;
                        var count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.AvgRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.GoodRate))
                        {
                            if (mark != t.GoodRate)
                            {
                                mark = t.GoodRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.GoodRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.NormalRate))
                        {
                            if (mark != t.NormalRate)
                            {
                                mark = t.NormalRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.NormalRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.PassRate))
                        {
                            if (mark != t.PassRate)
                            {
                                mark = t.PassRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.PassRank = rank;
                            }
                        }
                    }
                }


                #endregion

                vm.ExamGradeAnalyzeList = (from p in gradeSubjectTotalMarkList.Union(gradeTotalMarkList)
                                           select new Dto.ExamAnalyze.List
                                           {
                                               ExamId = p.ExamId,
                                               ClassId = p.ClassId,
                                               SubjectId = p.SubjectId,
                                               AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                           }).ToList();

                vm.ExamAnalyzeList = (from p in exam
                                      select new Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = p.SubjectId,
                                          StudentCount = p.StudentCount,
                                          AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                          MaxMark = p.MaxMark,
                                          MinMark = p.MinMark,
                                          GoodRate = p.GoodRate,
                                          NormalRate = p.NormalRate,
                                          PassRate = p.PassRate,
                                          AvgRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.AvgRank).FirstOrDefault(),
                                          GoodRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.GoodRank).FirstOrDefault(),
                                          NormalRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.NormalRank).FirstOrDefault(),
                                          PassRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.PassRank).FirstOrDefault(),
                                      }).ToList();

                #endregion

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassAnalyzeList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["CheckedAll"] != null ? Request.Form["CheckedAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("ClassAnalyzeList", new { ExamId = vm.ExamId, lastexamId = vm.LastExamId, GradeId = vm.GradeId, CheckedAll = CheckedAll, chkClass = chkclassList, searchText = vm.SearchText }));
        }

        #endregion

        #region 等级分析
        public ActionResult ExamLevelList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                var lsExam = new List<int>();
                var ntExam = new List<int>();
                //本次考试
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }
                lsExam.Add(vm.ExamId);
                //上次考试
                vm.LastExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.LastExamId == 0 && vm.LastExamList.Count > 0)
                {
                    vm.LastExamId = vm.LastExamList.FirstOrDefault().Value.ConvertToInt();
                }
                ntExam.Add(vm.LastExamId);

                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == vm.GradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == vm.ExamId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && courseIdList.Contains(p.tbCourse.Id)
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(vm.SearchText) || vm.SearchText == null)
                                       orderby p.tbCourse.tbSubject.No
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id,
                                           CourseId = p.tbCourse.Id
                                       }).Distinct().ToList();

                var SubjectList = (from p in examSubjectList
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.SubjectId
                                   }).Distinct().ToList();

                vm.SubjectList = (from p in SubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                if (vm.chkSubject == null) return View(vm);
                var chksubjectList = vm.chkSubject.Split(',');

                //选中的科目
                var selectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                         where chksubjectList.Contains(p.Id.ToString())
                                         orderby p.No, p.SubjectName
                                         select new
                                         {
                                             p.Id,
                                             p.SubjectName,
                                         }).Distinct().ToList();

                vm.selectSubjectList = (from p in selectSubjectList
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.SubjectName,
                                        }).Distinct().ToList();

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                YearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();

                if (year == null) return View(vm);

                //班主任
                var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                     .Include(d => d.tbClass)
                                        where p.tbClass.IsDeleted == false
                                        && p.tbClass.tbYear.Id == year.YearId
                                        && p.tbTeacher.IsDeleted == false
                                        select new
                                        {
                                            ClassId = p.tbClass != null ? p.tbClass.Id : 0,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).ToList();

                //任课老师
                var courseIds = examSubjectList.Select(d => d.CourseId).Distinct().ToList();
                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                      .Include(d => d.tbOrg.tbClass)
                                      where p.tbOrg.IsDeleted == false
                                      && courseIds.Contains(p.tbOrg.tbCourse.Id)
                                      && p.tbOrg.tbYear.Id == year.Id
                                      && p.tbTeacher.IsDeleted == false
                                      select new Dto.ExamAnalyze.List
                                      {
                                          SubjectId = p.tbOrg.tbCourse.tbSubject.Id,
                                          ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();
                vm.SubjectTeacherList = orgTeacherList;

                //年级学生班级
                vm.StudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  where p.tbStudent.IsDeleted == false
                                  && p.tbClass.tbGrade.Id == vm.GradeId
                                  && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                  orderby p.tbClass.No, p.tbClass.ClassName
                                  select new Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.tbClass.Id,
                                      ClassName = p.tbClass.ClassName,
                                      StudentId = p.tbStudent.Id
                                  }).ToList();

                var classtudentList = (from p in vm.StudentList
                                       select new
                                       {
                                           p.ClassId,
                                           p.ClassName
                                       }).Distinct().ToList();

                vm.ClassStudentList = (from p in classtudentList
                                       select new Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           ClassName = p.ClassName,
                                           TeacherName = classTeacherList.Where(c => c.ClassId == p.ClassId).Select(d => d.TeacherName).FirstOrDefault()
                                       }).Distinct().ToList();

                vm.ClassOrgStudentList = (from p in classtudentList
                                          select new Dto.ExamAnalyze.List
                                          {
                                              ClassId = p.ClassId,
                                              ClassName = p.ClassName,
                                              TeacherName = orgTeacherList.Where(c => c.ClassId == p.ClassId).Select(d => d.TeacherName).FirstOrDefault()
                                          }).Distinct().ToList();

                #region 本次考试
                var exam = (from p in db.Table<Exam.Entity.tbExam>()
                            .Include(d => d.tbExamLevelGroup)
                            where p.IsDeleted == false
                            && p.Id == vm.ExamId
                            select p).FirstOrDefault();

                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && lsExam.Contains(p.tbExamCourse.tbExam.Id)
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        p.tbExamCourse.FullTotalMark
                                    }).ToList();

                //获取科目满分
                var examSubjectTotal = (from p in examMarkList
                                        group p by new { p.SubjectId, p.FullTotalMark } into g
                                        select new
                                        {
                                            SubjectId = g.Key.SubjectId,
                                            FullTotalMark = g.Key.FullTotalMark,
                                        }).ToList();

                //获取考试总分满分
                var examTotal = examSubjectTotal.Sum(d => d.FullTotalMark);

                var classExamMarkList = (from p in examMarkList
                                         join t in vm.StudentList
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                             FullTotalMark = p.FullTotalMark
                                         }).ToList();

                //获取班级学生总分
                vm.ExamLsStudentList = (from p in classExamMarkList
                                        group p by new { p.ClassId, p.StudentId } into g
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = g.Key.ClassId,
                                            StudentId = g.Key.StudentId,
                                            TotalMark = g.Sum(d => d.TotalMark),
                                        }).ToList();

                //获取总分等级
                if (exam != null && exam.tbExamLevelGroup != null)
                {
                    vm.LsTotalLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                           where p.IsDeleted == false
                                           && p.tbExamLevelGroup.IsTotal == true
                                           && p.tbExamLevelGroup.IsDeleted == false
                                           && p.tbExamLevelGroup.Id == exam.tbExamLevelGroup.Id
                                           orderby p.No
                                           select p).ToList();
                }

                foreach (var a in vm.ClassStudentList)
                {
                    var studentTotalList = vm.ExamLsStudentList.Where(d => d.ClassId == a.ClassId).ToList();

                    foreach (var b in vm.LsTotalLevelList)
                    {
                        var classStudentLevel = new Dto.ExamAnalyze.List();
                        classStudentLevel.ClassId = a.ClassId;
                        classStudentLevel.TotalLevelId = b.Id;
                        if (studentTotalList.Where(d => (b.MinScore * examTotal / 100) <= d.TotalMark && d.TotalMark <= (b.MaxScore * examTotal / 100)).Count() > 0)
                        {
                            classStudentLevel.TotalLevelCount = studentTotalList.Where(d => (b.MinScore * examTotal / 100) <= d.TotalMark && d.TotalMark <= (b.MaxScore * examTotal / 100)).Count();
                        }
                        else
                        {
                            classStudentLevel.TotalLevelCount = 0;
                        }
                        vm.LsClassStudentLevelList.Add(classStudentLevel);
                    }
                }
                #endregion

                #region 对比考试
                exam = (from p in db.Table<Exam.Entity.tbExam>()
                            .Include(d => d.tbExamLevelGroup)
                        where p.IsDeleted == false
                        && p.Id == vm.LastExamId
                        select p).FirstOrDefault();

                var examNtMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                      where p.tbExamCourse.IsDeleted == false
                                       && p.tbStudent.IsDeleted == false
                                       && ntExam.Contains(p.tbExamCourse.tbExam.Id)
                                       && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                      select new
                                      {
                                          p.TotalMark,
                                          ExamId = p.tbExamCourse.tbExam.Id,
                                          StudentId = p.tbStudent.Id,
                                          SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                          p.tbExamCourse.FullTotalMark
                                      }).ToList();

                //获取科目满分
                var examNtSubjectTotal = (from p in examNtMarkList
                                          group p by new { p.SubjectId, p.FullTotalMark } into g
                                          select new
                                          {
                                              SubjectId = g.Key.SubjectId,
                                              FullTotalMark = g.Key.FullTotalMark,
                                          }).ToList();

                //获取考试总分满分
                var examNtTotal = examNtSubjectTotal.Sum(d => d.FullTotalMark);

                var classExamNtMarkList = (from p in examNtMarkList
                                           join t in vm.StudentList
                                           on p.StudentId equals t.StudentId
                                           select new
                                           {
                                               ExamId = p.ExamId,
                                               StudentId = p.StudentId,
                                               SubjectId = p.SubjectId,
                                               TotalMark = p.TotalMark,
                                               ClassId = t.ClassId,
                                               ClassName = t.ClassName,
                                               FullTotalMark = p.FullTotalMark
                                           }).ToList();

                //获取班级学生总分
                vm.ExamNtStudentList = (from p in classExamNtMarkList
                                        group p by new { p.ClassId, p.StudentId } into g
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = g.Key.ClassId,
                                            StudentId = g.Key.StudentId,
                                            TotalMark = g.Sum(d => d.TotalMark),
                                        }).ToList();

                //获取总分等级
                if (exam != null && exam.tbExamLevelGroup != null)
                {
                    vm.NtTotalLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                           where p.IsDeleted == false
                                           && p.tbExamLevelGroup.IsTotal == true
                                           && p.tbExamLevelGroup.IsDeleted == false
                                           && p.tbExamLevelGroup.Id == exam.tbExamLevelGroup.Id
                                           orderby p.No
                                           select p).ToList();
                }

                var LictA = vm.LsTotalLevelList.Select(d => d.Id).ToList();
                var ListB = vm.NtTotalLevelList.Select(d => d.Id).ToList();
                if (vm.LsTotalLevelList.Count() == vm.NtTotalLevelList.Count() && LictA.Count(t => ListB.Contains(t)) == vm.LsTotalLevelList.Count())
                {
                    vm.TotalLevelList = vm.LsTotalLevelList;
                }

                foreach (var a in vm.ClassStudentList)
                {
                    var studentTotalList = vm.ExamNtStudentList.Where(d => d.ClassId == a.ClassId).ToList();

                    foreach (var b in vm.NtTotalLevelList)
                    {
                        var classStudentLevel = new Dto.ExamAnalyze.List();
                        classStudentLevel.ClassId = a.ClassId;
                        classStudentLevel.TotalLevelId = b.Id;
                        if (studentTotalList.Where(d => (b.MinScore * examTotal / 100) <= d.TotalMark && d.TotalMark <= (b.MaxScore * examTotal / 100)).Count() > 0)
                        {
                            classStudentLevel.TotalLevelCount = studentTotalList.Where(d => (b.MinScore * examTotal / 100) <= d.TotalMark && d.TotalMark <= (b.MaxScore * examTotal / 100)).Count();
                        }
                        else
                        {
                            classStudentLevel.TotalLevelCount = 0;
                        }
                        vm.NtClassStudentLevelList.Add(classStudentLevel);
                    }
                }
                #endregion

                #region 科目 本次考试
                #region 本次考试
                //获取等级
                var subjectLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                        .Include(d => d.tbExamLevelGroup)
                                        where p.IsDeleted == false
                                        && p.tbExamLevelGroup.IsDeleted == false
                                        && p.tbExamLevelGroup.IsTotal == false
                                        select p).ToList();

                var subjectMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                       where p.tbExamCourse.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && lsExam.Contains(p.tbExamCourse.tbExam.Id)
                                        && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                       select new
                                       {
                                           p.TotalMark,
                                           ExamId = p.tbExamCourse.tbExam.Id,
                                           StudentId = p.tbStudent.Id,
                                           SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                           LevelGroupId = p.tbExamCourse.tbExamLevelGroup.Id,
                                           p.tbExamCourse.FullTotalMark,
                                           ExamLevelId = p.tbExamLevel != null ? p.tbExamLevel.Id : 0,
                                       }).ToList();

                var subjectTotalLevelList = (from p in subjectMarkList
                                             join t in subjectLevelList on p.LevelGroupId equals t.tbExamLevelGroup.Id
                                             select new
                                             {
                                                 SubjectId = p.SubjectId,
                                                 TotalLevelId = t.Id,
                                                 TotalLavelName = t.ExamLevelName,
                                                 TotalLavelMax = t.MaxScore,
                                                 TotalLavelMin = t.MinScore,
                                             }).Distinct().ToList();
                vm.SubjectTotalLevelList = (from p in subjectTotalLevelList
                                            select new Dto.ExamAnalyze.List
                                            {
                                                SubjectId = p.SubjectId,
                                                TotalLevelId = p.TotalLevelId,
                                                TotalLavelName = p.TotalLavelName,
                                                TotalLavelMax = p.TotalLavelMax,
                                                TotalLavelMin = p.TotalLavelMin,
                                            }).ToList();

                var classSubjectExamMarkList = (from p in subjectMarkList
                                                join t in vm.StudentList
                                                on p.StudentId equals t.StudentId
                                                select new
                                                {
                                                    ExamId = p.ExamId,
                                                    StudentId = p.StudentId,
                                                    SubjectId = p.SubjectId,
                                                    TotalMark = p.TotalMark,
                                                    ClassId = t.ClassId,
                                                    ClassName = t.ClassName,
                                                    LevelGroupId = p.LevelGroupId,
                                                    FullTotalMark = p.FullTotalMark,
                                                    ExamLevelId = p.ExamLevelId,
                                                }).ToList();

                foreach (var selectSubject in vm.selectSubjectList)
                {
                    foreach (var a in vm.ClassOrgStudentList)
                    {
                        var studentTotalList = classSubjectExamMarkList.Where(d => d.ClassId == a.ClassId && d.SubjectId.ToString() == selectSubject.Value).ToList();

                        foreach (var b in vm.SubjectTotalLevelList.Where(d => d.SubjectId == selectSubject.Value.ConvertToInt()).Distinct().ToList())
                        {
                            var classStudentLevel = new Dto.ExamAnalyze.List();
                            classStudentLevel.ClassId = a.ClassId;
                            classStudentLevel.SubjectId = selectSubject.Value.ConvertToInt();
                            classStudentLevel.TotalLevelId = b.TotalLevelId;
                            //if (studentTotalList.Where(d => (b.TotalLavelMin * d.FullTotalMark / 100) <= d.TotalMark && d.TotalMark <= (b.TotalLavelMax * d.FullTotalMark / 100)).Count() > 0)
                            //{
                            //    classStudentLevel.TotalLevelCount = studentTotalList.Where(d => (b.TotalLavelMin * d.FullTotalMark / 100) <= d.TotalMark && d.TotalMark <= (b.TotalLavelMax * d.FullTotalMark / 100)).Count();
                            //}
                            //else
                            //{
                            //    classStudentLevel.TotalLevelCount = 0;
                            //}
                            if (studentTotalList.Where(d => d.ExamLevelId == b.TotalLevelId).ToList().Count() > 0)
                            {
                                classStudentLevel.TotalLevelCount = studentTotalList.Where(d => d.ExamLevelId == b.TotalLevelId).ToList().Count();
                            }
                            else
                            {
                                classStudentLevel.TotalLevelCount = 0;
                            }
                            vm.SClassStudentLevelList.Add(classStudentLevel);
                        }
                    }
                }
                #endregion

                #region 对比考试
                var examNtSubjectMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                             where p.tbExamCourse.IsDeleted == false
                                              && p.tbStudent.IsDeleted == false
                                              && ntExam.Contains(p.tbExamCourse.tbExam.Id)
                                              && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                             select new
                                             {
                                                 p.TotalMark,
                                                 ExamId = p.tbExamCourse.tbExam.Id,
                                                 StudentId = p.tbStudent.Id,
                                                 SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                                 LevelGroupId = p.tbExamCourse.tbExamLevelGroup.Id,
                                                 p.tbExamCourse.FullTotalMark,
                                                 ExamLevelId = p.tbExamLevel != null ? p.tbExamLevel.Id : 0,
                                             }).ToList();

                var ntSubjectExamMarkList = (from p in examNtSubjectMarkList
                                             join t in vm.StudentList
                                             on p.StudentId equals t.StudentId
                                             select new
                                             {
                                                 ExamId = p.ExamId,
                                                 StudentId = p.StudentId,
                                                 SubjectId = p.SubjectId,
                                                 TotalMark = p.TotalMark,
                                                 ClassId = t.ClassId,
                                                 ClassName = t.ClassName,
                                                 LevelGroupId = p.LevelGroupId,
                                                 FullTotalMark = p.FullTotalMark,
                                                 ExamLevelId = p.ExamLevelId,
                                             }).ToList();



                foreach (var selectSubject in vm.selectSubjectList)
                {
                    foreach (var a in vm.ClassOrgStudentList)
                    {
                        var studentTotalList = ntSubjectExamMarkList.Where(d => d.ClassId == a.ClassId && d.SubjectId.ToString() == selectSubject.Value).ToList();
                        foreach (var b in vm.SubjectTotalLevelList.Where(d => d.SubjectId == selectSubject.Value.ConvertToInt()).Distinct().ToList())
                        {
                            var classStudentLevel = new Dto.ExamAnalyze.List();
                            classStudentLevel.ClassId = a.ClassId;
                            classStudentLevel.SubjectId = selectSubject.Value.ConvertToInt();
                            classStudentLevel.TotalLevelId = b.TotalLevelId;
                            //if (studentTotalList.Where(d => (b.TotalLavelMin * d.FullTotalMark / 100) <= d.TotalMark && d.TotalMark <= (b.TotalLavelMax * d.FullTotalMark / 100)).Count() > 0)
                            //{
                            //    classStudentLevel.TotalLevelCount = studentTotalList.Where(d => (b.TotalLavelMin * d.FullTotalMark / 100) <= d.TotalMark && d.TotalMark <= (b.TotalLavelMax * d.FullTotalMark / 100)).Count();
                            //}
                            //else
                            //{
                            //    classStudentLevel.TotalLevelCount = 0;
                            //}
                            if (studentTotalList.Where(d => d.ExamLevelId == b.TotalLevelId).ToList().Count() > 0)
                            {
                                classStudentLevel.TotalLevelCount = studentTotalList.Where(d => d.ExamLevelId == b.TotalLevelId).ToList().Count();
                            }
                            else
                            {
                                classStudentLevel.TotalLevelCount = 0;
                            }
                            vm.SNtClassStudentLevelList.Add(classStudentLevel);
                        }
                    }
                }
                #endregion
                #endregion
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExamLevelList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var CheckedAll = Request["CheckedAll"] != null ? Request.Form["CheckedAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("ExamLevelList", new { ExamId = vm.ExamId, LastExamId = vm.LastExamId, GradeId = vm.GradeId, chkSubject = chksubjectList, searchText = vm.SearchText, CheckedAll = CheckedAll }));
        }
        #endregion

        #region 分数段
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SegmentSubjectList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("SegmentSubjectList", new { ExamId = vm.ExamId, GradeId = vm.GradeId, chkSubject = chksubjectList, searchText = vm.SearchText }));
        }

        public ActionResult SegmentSubjectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                vm.OptionList = new List<string>() { "分数段" };
                vm.ClumnList = new List<string>() { "年级人数", "年级累计" };
                var lsExam = new List<int>();
                //本次考试
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }
                lsExam.Add(vm.ExamId);

                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }
                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == vm.GradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && courseIdList.Contains(p.tbCourse.Id)
                                    && (p.tbCourse.tbSubject.SubjectName.Contains(vm.SearchText) || vm.SearchText == null)
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

                vm.SubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                YearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();

                if (year == null) return View(vm);

                vm.ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = year.ExamName, Value = vm.ExamId.ToString() });

                if (vm.chkSubject == null) return View(vm);
                var chksubjectList = vm.chkSubject.Split(',');

                //分数段分组
                var segmentGroupId = (from p in db.Table<Exam.Entity.tbExam>().Include(d => d.tbExamSegmentGroup)
                                            where lsExam.Contains(p.Id)
                                            select p.tbExamSegmentGroup.Id
                                            ).FirstOrDefault();

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject)
                                       where p.tbGrade.Id == vm.GradeId
                                        &&  p.tbExamSegmentGroup.Id== segmentGroupId
                                        && (chksubjectList.Contains(p.tbSubject.Id.ToString()) || p.tbSubject == null)
                                       orderby p.tbSubject.No, p.No, p.SegmentName
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsNormal,
                                           p.IsPass,
                                           p.IsTotal,
                                           SubjectId = p.tbSubject != null ? p.tbSubject.Id : 0
                                       }).ToList();
                vm.SegmentList = (from p in SegmentMarkList
                                  select new Dto.ExamAnalyze.SegmentList
                                  {
                                      SegmentId = p.SegmentId,
                                      SegmentName = p.SegmentName,
                                      SubjectId = p.SubjectId
                                  }).ToList();

                //选中的科目
                var selectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                         where chksubjectList.Contains(p.Id.ToString())
                                         orderby p.No, p.SubjectName
                                         select new
                                         {
                                             p.Id,
                                             p.SubjectName,
                                         }).Distinct().ToList();

                vm.selectSubjectList = (from p in selectSubjectList
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.SubjectName,
                                        }).Distinct().ToList();
                if (chksubjectList.Contains("0"))
                {
                    vm.selectSubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });
                }

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var classtudentList = (from p in classStudent
                                       select new
                                       {
                                           p.ClassId,
                                           p.ClassName
                                       }).Distinct().ToList();

                vm.ClassStudentList = (from p in classtudentList
                                       select new Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           ClassName = p.ClassName
                                       }).Distinct().ToList();


                //考试成绩
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && lsExam.Contains(p.tbExamCourse.tbExam.Id)
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        p.tbExamCourse.FullTotalMark
                                    }).ToList();


                #region  成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in classStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName
                                         }).ToList();

                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ClassId,
                                                p.StudentId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                SubjectId = 0,
                                                g.Key.StudentId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();

                #region 向下累计数
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    var isTotal = o.IsTotal;
                    if (isTotal && o.SubjectId == 0)//总分
                    {
                        //分数段人数
                        var tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count(),
                                  }).ToList();
                        //累计
                        var accumulateStudent = (from p in totalStudentMarkList
                                                 where p.StudentTotalMark >= o.MaxMark
                                                 group p by new
                                                 {
                                                     p.ClassId
                                                 } into g
                                                 select new
                                                 {
                                                     g.Key.ClassId,
                                                     StudentCount = g.Count(),
                                                 }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      SegmentId = o.SegmentId,
                                      ClassId = p.ClassId,
                                      SubjectId = 0,
                                      StudentNum = p.StudentCount,
                                      TotalStudentCount = accumulateStudent.Where(d => d.ClassId == p.ClassId).Select(d => d.StudentCount).FirstOrDefault(),//班级累计
                                      GradeStudentCount = tm.Sum(d => d.StudentCount),//年级人数，
                                      TotalGradeStudentCount = accumulateStudent.Sum(d => d.StudentCount)//年级累计
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                    else//各科目
                    {
                        //分数段人数
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var accumulateStudent = (from p in classExamMarkList
                                                 where p.SubjectId == o.SubjectId
                                                                && p.TotalMark >= o.MaxMark
                                                 group p by new
                                                 {
                                                     p.ClassId
                                                 } into g
                                                 select new
                                                 {
                                                     g.Key.ClassId,
                                                     StudentCount = g.Count(),
                                                 }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      SegmentId = o.SegmentId,
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                      TotalStudentCount = accumulateStudent.Where(d => d.ClassId == p.ClassId).Select(d => d.StudentCount).FirstOrDefault(),//班级累计
                                      GradeStudentCount = tm.Sum(d => d.StudentCount),//年级人数，
                                      TotalGradeStudentCount = accumulateStudent.Sum(d => d.StudentCount)//年级累计
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }

                #endregion

                vm.ExamAnalyzeList = (from p in lst
                                      select new Dto.ExamAnalyze.List
                                      {
                                          SegmentId = p.SegmentId,
                                          ClassId = p.ClassId,
                                          SubjectId = p.SubjectId,
                                          StudentNum = p.StudentNum,
                                          TotalStudentCount = p.TotalStudentCount,
                                          GradeStudentCount = p.GradeStudentCount,
                                          TotalGradeStudentCount = p.TotalGradeStudentCount
                                      }).ToList();

                #endregion

                return View(vm);
            }
        }
        #endregion

        #region 各科前若干名统计
        public ActionResult ExamTopRankList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                var lsExam = new List<int>();
                //本次考试
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }
                lsExam.Add(vm.ExamId);

                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }
                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == vm.GradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   && courseIdList.Contains(p.tbCourse.Id)
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

                if (vm.chkSubject == null) return View(vm);
                var chksubjectList = vm.chkSubject.Split(',');

                //选中的科目
                var selectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                         where chksubjectList.Contains(p.Id.ToString())
                                         orderby p.No, p.SubjectName
                                         select new
                                         {
                                             p.Id,
                                             p.SubjectName,
                                         }).Distinct().ToList();

                vm.selectSubjectList = (from p in selectSubjectList
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.SubjectName,
                                        }).Distinct().ToList();

                //获取分数等级
                vm.TotalLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                     .Include(d => d.tbExamLevelGroup)
                                     where p.IsDeleted == false
                                     orderby p.No
                                     select p).ToList();

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                YearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();

                if (year == null) return View(vm);

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                     && (p.tbClass.ClassName.Contains(vm.SearchText) || vm.SearchText == null)
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                vm.ClassStudentList = (from p in classStudent
                                       group p by new { p.ClassId, p.ClassName } into g
                                       select new Dto.ExamAnalyze.List
                                       {
                                           ClassId = g.Key.ClassId,
                                           ClassName = g.Key.ClassName,
                                           TotalCount = g.Where(d => d.ClassId == g.Key.ClassId).Count(),
                                       }).Distinct().ToList();

                #region 本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && lsExam.Contains(p.tbExamCourse.tbExam.Id)
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        p.TotalGradeRank,
                                    }).ToList();

                vm.StudentExamMarkList = (from p in examMarkList
                                          join t in classStudent
                                          on p.StudentId equals t.StudentId
                                          select new Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              StudentId = p.StudentId,
                                              SubjectId = p.SubjectId,
                                              TotalMark = p.TotalMark,
                                              ClassId = t.ClassId,
                                              ClassName = t.ClassName,
                                              GradeRank = p.TotalGradeRank,
                                          }).ToList();

                //获取班级学生总分
                vm.TopTotalExamMarkList = (from p in vm.StudentExamMarkList
                                           group p by new { p.ClassId, p.StudentId } into g
                                           select new Dto.ExamAnalyze.List
                                           {
                                               ClassId = g.Key.ClassId,
                                               StudentId = g.Key.StudentId,
                                               TotalMark = g.Sum(d => d.TotalMark),
                                               TotalGradeRank = 0,
                                           }).ToList();

                vm.DownTotalExamMarkList = (from p in vm.StudentExamMarkList
                                            group p by new { p.ClassId, p.StudentId } into g
                                            select new Dto.ExamAnalyze.List
                                            {
                                                ClassId = g.Key.ClassId,
                                                StudentId = g.Key.StudentId,
                                                TotalMark = g.Sum(d => d.TotalMark),
                                                TotalGradeRank = 0,
                                            }).ToList();

                //按总分成绩从高到低
                var gradeRank = decimal.Zero;
                decimal? gradeMark = null;
                var gradeCount = decimal.One;
                foreach (var t in vm.TopTotalExamMarkList.OrderByDescending(d => d.TotalMark))
                {
                    if (gradeMark != t.TotalMark)
                    {
                        gradeMark = t.TotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.TotalGradeRank = gradeRank;
                }

                //按总分从低到高
                gradeRank = decimal.Zero;
                gradeMark = null;
                gradeCount = decimal.One;
                foreach (var t in vm.DownTotalExamMarkList.OrderBy(d => d.TotalMark))
                {
                    if (gradeMark != t.TotalMark)
                    {
                        gradeMark = t.TotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.TotalGradeRank = gradeRank;
                }

                #endregion
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExamTopRankList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var CheckedAll = Request["CheckedAll"] != null ? Request.Form["CheckedAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("ExamTopRankList", new { ExamId = vm.ExamId, GradeId = vm.GradeId, chkSubject = chksubjectList, searchText = vm.SearchText, CheckedAll = CheckedAll }));
        }
        #endregion

        #region 班级前N名
        public ActionResult TotalMarkTopNList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //分数项目
                vm.OptionList = new List<string>() { "人数", "比率 " };

                //科目和班级
                var classList = new List<System.Web.Mvc.SelectListItem>();
                var subjectList = new List<System.Web.Mvc.SelectListItem>();
                var yearId = 0;
                this.getSubjectClassList(vm.ExamId, vm.GradeId, null, out classList, out subjectList, out yearId);
                vm.SubjectList = subjectList;
                vm.ClassList = classList;

                var rank = vm.SearchText.ConvertToDecimal();

                if (vm.chkClass == null || vm.chkSubject == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');
                var chksubjectList = vm.chkSubject.Split(',');

                var selectclassList = new List<System.Web.Mvc.SelectListItem>();
                var selectsubjectList = new List<System.Web.Mvc.SelectListItem>();
                this.getSelectSubjectClassList(chkclassList, chksubjectList, out selectclassList, out selectsubjectList);
                vm.selctClassList = selectclassList;
                vm.selectSubjectList = selectsubjectList;

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && chkclassList.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();


                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == vm.ExamId || vm.ExamId == 0)
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalClassRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              ClassRank = p.TotalClassRank,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in vm.selectSubjectList)
                {
                    //班级人数
                    var subjectId = o.Value.ConvertToInt();
                    var totalStudetCount = (from p in tg
                                            where p.SubjectId == subjectId
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                TotalCount = g.Count()
                                            }).ToList();
                    //前N名人数
                    var tm = (from p in tg
                              where p.SubjectId == subjectId
                              && p.ClassRank > decimal.Zero && p.ClassRank <= rank
                              group p by new
                              {
                                  p.ClassId
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  StudentCount = g.Count(),
                                  TotalCount = totalStudetCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = subjectId,
                                  SubjectName = o.Text,
                                  StudentCount = p.StudentCount > decimal.Zero ? p.StudentCount.ToString() : string.Empty,
                                  Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentCount * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                              }).ToList();
                    lst.AddRange(tb);
                }
                vm.ExamAnalyzeList = lst;

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TotalMarkTopNList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("TotalMarkTopNList", new
            {
                ExamId = vm.ExamId,
                GradeId = vm.GradeId,
                chkSubject = chksubjectList,
                chkClass = chkclassList,
                CheckedAll = CheckedAll,
                chkClassAll = chkClassAll,
                searchText = vm.SearchText
            }));
        }

        public ActionResult TotalMarkTopNDetailList(int examId, int gradeId, int classId, string chkSubject, decimal? rank)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();
                if (rank == null) return View(vm);

                var chksubjectList = chkSubject.Split(',');
                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == examId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();


                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == examId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == gradeId
                                    && p.tbClass.Id == classId && p.tbClass.tbYear.Id == yearId
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentCode,
                                        p.tbStudent.StudentName
                                    }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == vm.ExamId || vm.ExamId == 0)
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalClassRank,
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              t.StudentCode,
                              t.StudentName,
                              SubjectId = p.SubjectId,
                              ClassRank = p.TotalClassRank,
                              p.TotalMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                var lsttotal = new List<string>();
                foreach (var o in SubjectList)
                {
                    //分数段人数
                    var tm = (from p in tg
                              where p.SubjectId == o.SubjectId
                              && p.ClassRank > decimal.Zero && p.ClassRank <= rank
                              select new
                              {
                                  SubjectName = o.SubjectName,
                                  p.StudentCode,
                                  p.StudentName,
                                  p.TotalMark
                              }).ToList();
                    var studentCount = tm.Count();

                    if (studentCount > decimal.Zero)
                    {
                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      StudentCode = p.StudentCode,
                                      StudentName = p.StudentName,
                                      SubjectName = p.SubjectName,
                                      Mark = p.TotalMark.ToString(),
                                      TotalCount = studentCount
                                  }).ToList();
                        lst.AddRange(tb);
                        lsttotal.Add(studentCount.ToString());
                    }
                    else
                    {
                        var model = new Exam.Dto.ExamAnalyze.List()
                        {
                            StudentCode = string.Empty,
                            StudentName = string.Empty,
                            SubjectName = o.SubjectName,
                            Mark = string.Empty,
                            TotalCount = 0
                        };
                        lst.Add(model);
                        lsttotal.Add(studentCount.ToString());
                    }
                }
                vm.ExamAnalyzeList = lst;

                return View(vm);
            }
        }
        #endregion

        #region 班级成绩
        public ActionResult ClassMarkList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamReport.List();
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }

                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }
                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == vm.GradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && courseIdList.Contains(p.tbCourse.Id)
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id,
                                       p.FullTotalMark
                                   }).Distinct().ToList();

                vm.SubjectList = (from p in SubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                vm.SubjectList.Add(new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });

                var examLevelGroup = (from p in db.Table<Exam.Entity.tbExam>()
                                      where p.Id == vm.ExamId
                                      select p.tbExamLevelGroup).FirstOrDefault();

                var examLevelGroupId = 0;
                var isGenerate = false;
                if (examLevelGroup != null)
                {
                    examLevelGroupId = examLevelGroup.Id;
                    isGenerate = examLevelGroup.IsGenerate;
                }
                //获取分数等级
                var TotalLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                     .Include(d => d.tbExamLevelGroup)
                                      where p.tbExamLevelGroup.Id == examLevelGroupId
                                      orderby p.No
                                      select p).ToList();

                //分数项目
                vm.OptionList = new List<string>() { "考试成绩 ", "班名", "级名", "等级" };

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbExam.Id == vm.ExamId
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
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullTotalMark
                          }).ToList();
                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == vm.ExamId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                //学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && ((p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)) || vm.SearchText == null)
                                    select new Dto.ExamReport.List
                                    {
                                        ClassId = p.tbClass.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName
                                    }).ToList();

                var classIdList = classStudent.Select(d => d.ClassId).Distinct().ToList();

                vm.ClassStudentList = classStudent;

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
                              TotalMark = p.TotalMark,
                              FullTotalMark = p.FullTotalMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              ExamLevelName = p.ExamLevelName,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName
                          }).ToList();

                var totalFullMark = SubjectList.Sum(d => d.FullTotalMark);

                var studentMarkList = (from p in tb
                                       group p by new
                                       {
                                           p.StudentId,
                                           StudentCode = p.StudentCode,
                                           StudentName = p.StudentName,
                                           p.ClassId
                                       } into g
                                       select new Dto.ExamReport.List
                                       {
                                           ClassId = g.Key.ClassId,
                                           StudentId = g.Key.StudentId,
                                           StudentCode = g.Key.StudentCode,
                                           StudentName = g.Key.StudentName,
                                           TotalMark = g.Sum(d => d.TotalMark),
                                           TotalClassRank = 0,
                                           TotalGradeRank = 0,
                                           SubjectId = 0,
                                           ExamLevelName = string.Empty,
                                           ClassName = string.Empty
                                       }).ToList();

                var lst = studentMarkList;

                //年级排名和等级
                var rank = 0;
                decimal? mark = null;
                var count = 1;
                foreach (var t in lst.OrderByDescending(d => d.TotalMark))
                {
                    if (isGenerate)//百分比
                    {
                        if (mark != t.TotalMark)
                        {
                            mark = t.TotalMark;
                            rank = rank + count;
                            count = 1;
                        }
                        else
                        {
                            count = count + 1;
                        }
                        Exam.Entity.tbExamLevel level = null;
                        if ((double)t.TotalMark >= (double)totalFullMark * 0.59)
                        {
                            level = (from p0 in TotalLevelList.Where(p => p.Rate != decimal.Zero)
                                     let v0 = (int)decimal.Ceiling(lst.Count * p0.Rate / 100)
                                     let v1 = lst[v0 - 1].TotalMark
                                     where p0.ExamLevelName != "D" && v1 <= mark
                                     select p0).FirstOrDefault();
                            if (level != null)
                            {
                                t.ExamLevelName = level.ExamLevelName;
                            }
                        }
                        else
                        {
                            level = TotalLevelList.Where(p => p.ExamLevelName == "D").FirstOrDefault();
                            if (level != null)
                            {
                                t.ExamLevelName = level.ExamLevelName;
                            }
                        }
                    }
                    else
                    {
                        var tk = (from p in TotalLevelList
                                  where p.MaxScore >= t.TotalMark && p.MinScore <= t.TotalMark
                                  select p).FirstOrDefault();
                        if (tk != null)
                        {
                            t.ExamLevelName = tk.ExamLevelName;
                        }
                    }
                }

                rank = 0;
                mark = null;
                count = 1;
                foreach (var t in lst.OrderByDescending(d => d.TotalMark))
                {
                    //年级排名
                    if (mark != t.TotalMark)
                    {
                        mark = t.TotalMark;
                        rank = rank + count;
                        count = 1;
                    }
                    else
                    {
                        count = count + 1;
                    }
                    var tt = (from p in lst
                              where p.StudentId == t.StudentId
                              select p).FirstOrDefault();
                    if (tt != null)
                    {
                        tt.TotalGradeRank = rank;
                    }
                }

                //班级排名
                foreach (var classId in classIdList)
                {
                    rank = 0;
                    mark = null;
                    count = 1;
                    foreach (var t in lst.Where(d => d.ClassId == classId).OrderByDescending(d => d.TotalMark))
                    {
                        if (mark != t.TotalMark)
                        {
                            mark = t.TotalMark;
                            rank = rank + count;
                            count = 1;
                        }
                        else
                        {
                            count = count + 1;
                        }
                        var tt = (from p in lst.Where(d => d.ClassId == classId)
                                  where p.StudentId == t.StudentId
                                  select p).FirstOrDefault();
                        if (tt != null)
                        {
                            tt.TotalClassRank = rank;
                        }
                    }
                }

                vm.ExamTotalMarkList = lst;

                var tm = (from p in tb
                          select new Dto.ExamReport.List
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              TotalMark = p.TotalMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              ExamLevelName = p.ExamLevelName,
                              ClassName = p.ClassName
                          }).ToList();

                vm.ExamMarkList = (from p in tm.Union(lst)
                                   select new Dto.ExamReport.List
                                   {
                                       StudentId = p.StudentId,
                                       SubjectId = p.SubjectId,
                                       StudentCode = p.StudentCode,
                                       StudentName = p.StudentName,
                                       TotalMark = p.TotalMark,
                                       TotalClassRank = p.TotalClassRank,
                                       TotalGradeRank = p.TotalGradeRank,
                                       ExamLevelName = p.ExamLevelName,
                                       ClassName = p.ClassName
                                   }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassMarkList(Models.ExamReport.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassMarkList", new { ExamId = vm.ExamId, GradeId = vm.GradeId, searchText = vm.SearchText }));
        }
        #endregion

        #region 与上次学生进退步
        public ActionResult AdvanceList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                var lsExam = new List<int>();
                var ntExam = new List<int>();
                //本次考试
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }
                lsExam.Add(vm.ExamId);
                //上次考试
                vm.LastExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.LastExamId == 0 && vm.LastExamList.Count > 0)
                {
                    vm.LastExamId = vm.LastExamList.FirstOrDefault().Value.ConvertToInt();
                }
                ntExam.Add(vm.LastExamId);

                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }
                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == vm.GradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && courseIdList.Contains(p.tbCourse.Id)
                                    && (p.tbCourse.tbSubject.SubjectName.Contains(vm.SearchText) || vm.SearchText == null)
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

                if (vm.chkSubject == null) return View(vm);
                var chksubjectList = vm.chkSubject.Split(',');

                //选中的科目
                var selectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                         where chksubjectList.Contains(p.Id.ToString())
                                         orderby p.No, p.SubjectName
                                         select new
                                         {
                                             p.Id,
                                             p.SubjectName,
                                         }).Distinct().ToList();

                vm.selectSubjectList = (from p in selectSubjectList
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.SubjectName,
                                        }).Distinct().ToList();

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                TopYearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();
                if (year == null) return View(vm);
                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.TopYearId
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                    }).ToList();

                vm.ClassStudentList = (from p in classStudent
                                       select new Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           ClassName = p.ClassName,
                                           StudentId = p.StudentId,
                                           StudentCode = p.StudentCode,
                                           StudentName = p.StudentName,
                                       }).Distinct().ToList();

                #region 本次考试
                var lsExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                      where p.tbExamCourse.IsDeleted == false
                                       && p.tbStudent.IsDeleted == false
                                       && lsExam.Contains(p.tbExamCourse.tbExam.Id)
                                       && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                      select new
                                      {
                                          p.TotalMark,
                                          ExamId = p.tbExamCourse.tbExam.Id,
                                          StudentId = p.tbStudent.Id,
                                          SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                          p.tbExamCourse.FullTotalMark,
                                          p.TotalGradeRank,
                                      }).ToList();

                vm.SClassStudentLevelList = (from p in lsExamMarkList
                                             join t in classStudent
                                             on p.StudentId equals t.StudentId
                                             select new Dto.ExamAnalyze.List
                                             {
                                                 ExamId = p.ExamId,
                                                 StudentId = p.StudentId,
                                                 SubjectId = p.SubjectId,
                                                 TotalMark = p.TotalMark,
                                                 ClassId = t.ClassId,
                                                 ClassName = t.ClassName,
                                                 GradeRank = p.TotalGradeRank,
                                             }).ToList();

                //获取班级学生总分
                var examLsStudentTotalList = (from p in vm.SClassStudentLevelList
                                              group p by new { p.ClassId, p.StudentId } into g
                                              select new Dto.ExamAnalyze.List
                                              {
                                                  ClassId = g.Key.ClassId,
                                                  StudentId = g.Key.StudentId,
                                                  TotalMark = g.Sum(d => d.TotalMark),
                                              }).ToList();

                var gradeRank = decimal.Zero;
                decimal? gradeMark = null;
                var gradeCount = decimal.One;
                foreach (var t in examLsStudentTotalList.OrderByDescending(d => d.TotalMark))
                {
                    if (gradeMark != t.TotalMark)
                    {
                        gradeMark = t.TotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.TotalGradeRank = gradeRank;
                }
                vm.ExamLsStudentList = (from p in examLsStudentTotalList
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = p.ClassId,
                                            StudentId = p.StudentId,
                                            TotalMark = p.TotalMark,
                                            TotalGradeRank = p.TotalGradeRank,
                                        }).ToList();
                #endregion

                #region 上次考试
                var ntExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                      where p.tbExamCourse.IsDeleted == false
                                       && p.tbStudent.IsDeleted == false
                                       && ntExam.Contains(p.tbExamCourse.tbExam.Id)
                                       && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                      select new
                                      {
                                          p.TotalMark,
                                          ExamId = p.tbExamCourse.tbExam.Id,
                                          StudentId = p.tbStudent.Id,
                                          SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                          p.tbExamCourse.FullTotalMark,
                                          p.TotalGradeRank,
                                      }).ToList();

                vm.SNtClassStudentLevelList = (from p in ntExamMarkList
                                               join t in classStudent
                                               on p.StudentId equals t.StudentId
                                               select new Dto.ExamAnalyze.List
                                               {
                                                   ExamId = p.ExamId,
                                                   StudentId = p.StudentId,
                                                   SubjectId = p.SubjectId,
                                                   TotalMark = p.TotalMark,
                                                   ClassId = t.ClassId,
                                                   ClassName = t.ClassName,
                                                   GradeRank = p.TotalGradeRank,
                                               }).ToList();

                //获取班级学生总分
                var examNtStudentTotalList = (from p in vm.SNtClassStudentLevelList
                                              group p by new { p.ClassId, p.StudentId } into g
                                              select new Dto.ExamAnalyze.List
                                              {
                                                  ClassId = g.Key.ClassId,
                                                  StudentId = g.Key.StudentId,
                                                  TotalMark = g.Sum(d => d.TotalMark),
                                              }).ToList();
                gradeRank = decimal.Zero;
                gradeMark = null;
                gradeCount = decimal.One;
                foreach (var t in examNtStudentTotalList.OrderByDescending(d => d.TotalMark))
                {
                    if (gradeMark != t.TotalMark)
                    {
                        gradeMark = t.TotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.TotalGradeRank = gradeRank;
                }

                //获取班级学生总分
                vm.ExamNtStudentList = (from p in examNtStudentTotalList
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = p.ClassId,
                                            StudentId = p.StudentId,
                                            TotalMark = p.TotalMark,
                                            TotalGradeRank = p.TotalGradeRank,
                                        }).ToList();
                #endregion

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdvanceList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var CheckedAll = Request["CheckedAll"] != null ? Request.Form["CheckedAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("AdvanceList", new { ExamId = vm.ExamId, LastExamId = vm.LastExamId, GradeId = vm.GradeId, chkSubject = chksubjectList, searchText = vm.SearchText, CheckedAll = CheckedAll }));
        }
        #endregion

        #region 班级科目成绩分析
        public ActionResult ClassSubjectList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                vm.OptionList = new List<string>() { "班级", "教师", "实考数", "平均分", "最高分", "最低分", "优秀率%", "良好率%", "及格率%", "均分排名", "优率排名", "良好率排名", "及格率排名" };
                vm.ClumnList = new List<string>() { "均分进退", "优秀进退", "良好进退", "及格进退" };
                var lsExam = new List<int>();
                //本次考试
                vm.ExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.ExamId == 0 && vm.ExamList.Count > 0)
                {
                    vm.ExamId = vm.ExamList.FirstOrDefault().Value.ConvertToInt();
                }
                lsExam.Add(vm.ExamId);

                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }
                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == vm.GradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && courseIdList.Contains(p.tbCourse.Id)
                                    && (p.tbCourse.tbSubject.SubjectName.Contains(vm.SearchText) || vm.SearchText == null)
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

                vm.SubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                YearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();

                if (year == null) return View(vm);

                vm.ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = year.ExamName, Value = vm.ExamId.ToString() });

                var lastExam = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == vm.ExamId
                                select new
                                {
                                    p.ExamName
                                }).FirstOrDefault();

                vm.ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = lastExam.ExamName, Value = vm.LastExamId.ToString() });


                //任课老师
                var orgTeacherLisr = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                     .Include(d => d.tbOrg.tbClass)
                                      where p.tbOrg.IsDeleted == false
                                      && p.tbOrg.tbYear.Id == year.Id
                                      && p.tbTeacher.IsDeleted == false
                                      select new
                                      {
                                          OrgId = p.tbOrg.Id,
                                          ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();

                if (vm.chkSubject == null) return View(vm);
                var chksubjectList = vm.chkSubject.Split(',');

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject)
                                       where p.tbGrade.Id == vm.GradeId
                                        && (chksubjectList.Contains(p.tbSubject.Id.ToString()) || p.tbSubject == null)
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsNormal,
                                           p.IsPass,
                                           p.IsTotal,
                                           SubjectId = p.tbSubject != null ? p.tbSubject.Id : 0
                                       }).ToList();

                //选中的科目
                var selectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                         where chksubjectList.Contains(p.Id.ToString())
                                         orderby p.No, p.SubjectName
                                         select new
                                         {
                                             p.Id,
                                             p.SubjectName,
                                         }).Distinct().ToList();

                vm.selectSubjectList = (from p in selectSubjectList
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.SubjectName,
                                        }).Distinct().ToList();
                if (chksubjectList.Contains("0"))
                {
                    vm.selectSubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });
                }

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var classtudentList = (from p in classStudent
                                       select new
                                       {
                                           p.ClassId,
                                           p.ClassName
                                       }).Distinct().ToList();

                vm.ClassStudentList = (from p in classtudentList
                                       select new Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           ClassName = p.ClassName,
                                           TeacherName = orgTeacherLisr.Where(c => c.ClassId == p.ClassId).Select(d => d.TeacherName).FirstOrDefault()
                                       }).Distinct().ToList();
                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                               && lsExam.Contains(p.tbExamCourse.tbExam.Id)
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        p.tbExamCourse.FullTotalMark
                                    }).ToList();


                #region  本次成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in classStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                             FullTotalMark = p.FullTotalMark
                                         }).ToList();
                //单个班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ExamId,
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ExamId = g.Key.ExamId,
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           StudentCount = g.Count().ToString(),
                                           AvgMark = g.Average(d => d.TotalMark),
                                           MaxMark = g.Max(d => d.TotalMark),
                                           MinMark = g.Min(d => d.TotalMark)
                                       }).ToList();
                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ExamId,
                                                p.ClassId,
                                                p.StudentId
                                            } into g
                                            select new
                                            {
                                                g.Key.ExamId,
                                                g.Key.ClassId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();
                var ty = (from p in totalStudentMarkList
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              StudentCount = g.Count()
                          }).ToList();
                var totalMarkList = (from p in totalStudentMarkList
                                     group p by new { p.ClassId, p.ExamId } into g
                                     select new Exam.Dto.ExamAnalyze.List
                                     {
                                         ExamId = g.Key.ExamId,
                                         ClassId = g.Key.ClassId,
                                         StudentCount = ty.Where(d => d.ExamId == g.Key.ExamId && d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                         AvgMark = g.Average(d => d.StudentTotalMark),
                                         MaxMark = g.Max(d => d.StudentTotalMark),
                                         MinMark = g.Min(d => d.StudentTotalMark)
                                     }).ToList();
                //总年级
                var gradeSubjectMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ExamId,
                                                p.SubjectId
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                ExamId = g.Key.ExamId,
                                                SubjectId = g.Key.SubjectId,
                                                ClassId = 0,
                                                StudentCount = g.Count().ToString(),
                                                AvgMark = g.Average(d => d.TotalMark),
                                                MaxMark = g.Max(d => d.TotalMark),
                                                MinMark = g.Min(d => d.TotalMark)
                                            }).ToList();

                var totalGradeStudentMarkList = (from p in classExamMarkList
                                                 group p by new
                                                 {
                                                     p.ExamId,
                                                     p.StudentId
                                                 } into g
                                                 select new
                                                 {
                                                     g.Key.ExamId,
                                                     ClassId = 0,
                                                     StudentTotalMark = g.Sum(d => d.TotalMark)
                                                 }).ToList();
                var tyGrade = (from p in totalStudentMarkList
                               group p by new
                               {
                                   p.ExamId
                               } into g
                               select new
                               {
                                   g.Key.ExamId,
                                   ClassId = 0,
                                   StudentCount = g.Count()
                               }).ToList();

                var totalGradeMarkList = (from p in totalGradeStudentMarkList
                                          group p by new { p.ClassId, p.ExamId } into g
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = g.Key.ExamId,
                                              ClassId = g.Key.ClassId,
                                              StudentCount = tyGrade.Where(d => d.ExamId == g.Key.ExamId && d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                              AvgMark = decimal.Round(g.Average(d => d.StudentTotalMark).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                              MaxMark = g.Max(d => d.StudentTotalMark),
                                              MinMark = g.Min(d => d.StudentTotalMark)
                                          }).ToList();

                #region 优秀良好及格排名
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;
                    var isNormal = o.IsNormal;
                    var isTotal = o.IsTotal;
                    if (isTotal && o.SubjectId == 0)//总分
                    {
                        if (isGood)
                        {
                            //分数段人数
                            var tm = (from p in totalStudentMarkList
                                      where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count()
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = 0,
                                          Status = decimal.One,
                                          StudentNum = p.StudentCount
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            var tm = (from p in totalStudentMarkList
                                      where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId,
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count()
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = 0,
                                          Status = 2,
                                          StudentNum = p.StudentCount
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            var tm = (from p in totalStudentMarkList
                                      where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count()
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = 0,
                                          Status = 3,
                                          StudentNum = p.StudentCount
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                    }
                    else//各科目
                    {
                        //优秀科目人数
                        if (isGood)
                        {
                            //分数段人数
                            var tm = (from p in classExamMarkList
                                      where p.SubjectId == o.SubjectId
                                      && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count(),
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = o.SubjectId,
                                          Status = decimal.One,
                                          StudentNum = p.StudentCount,
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            var tm = (from p in classExamMarkList
                                      where p.SubjectId == o.SubjectId
                                      && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId,
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count(),
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = o.SubjectId,
                                          Status = 2,
                                          StudentNum = p.StudentCount,
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            var tm = (from p in classExamMarkList
                                      where p.SubjectId == o.SubjectId
                                      && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count(),
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = o.SubjectId,
                                          Status = 3,
                                          StudentNum = p.StudentCount,
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                    }
                }

                var tk = (from p in lst
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                              p.SubjectId,
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              g.Key.SubjectId,
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum)
                          }).ToList();
                //年级项
                var tkGrade = (from p in tk
                               group p by new
                               {
                                   p.ExamId,
                                   p.SubjectId,
                                   p.Status
                               } into g
                               select new
                               {
                                   g.Key.ExamId,
                                   g.Key.SubjectId,
                                   ClassId = 0,
                                   g.Key.Status,
                                   StudentNum = g.Sum(d => d.StudentNum)
                               }).ToList();

                var exam = (from p in subjectMarkList.Union(totalMarkList)
                            select new
                            {
                                ExamId = p.ExamId,
                                ClassId = p.ClassId,
                                SubjectId = p.SubjectId,
                                StudentCount = p.StudentCount.ToString(),
                                AvgMark = p.AvgMark,
                                MaxMark = p.MaxMark,
                                MinMark = p.MinMark,
                                GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                            }).ToList();

                var examGrade = (from p in gradeSubjectMarkList.Union(totalGradeMarkList)
                                 select new Dto.ExamAnalyze.List
                                 {
                                     ExamId = p.ExamId,
                                     ClassId = p.ClassId,
                                     SubjectId = p.SubjectId,
                                     StudentCount = p.StudentCount.ToString(),
                                     AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                     MaxMark = p.MaxMark,
                                     MinMark = p.MinMark,
                                     GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                     NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                     PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                 }).ToList();

                //科目班级排名
                var lsComm = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in exam)
                {
                    var temp = new Exam.Dto.ExamAnalyze.List
                    {
                        ExamId = o.ExamId,
                        ClassId = o.ClassId,
                        SubjectId = o.SubjectId,
                        AvgRank = 0,
                        GoodRank = 0,
                        NormalRank = 0,
                        PassRank = 0
                    };
                    lsComm.Add(temp);
                }
                foreach (var examId in lsExam)
                {
                    foreach (var subject in vm.selectSubjectList)
                    {
                        var rank = 0;
                        decimal? mark = null;
                        var count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.AvgRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.GoodRate))
                        {
                            if (mark != t.GoodRate)
                            {
                                mark = t.GoodRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.GoodRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.NormalRate))
                        {
                            if (mark != t.NormalRate)
                            {
                                mark = t.NormalRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.NormalRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.PassRate))
                        {
                            if (mark != t.PassRate)
                            {
                                mark = t.PassRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.PassRank = rank;
                            }
                        }
                    }
                }


                #endregion
                vm.ExamGradeAnalyzeList = examGrade;

                vm.ExamAnalyzeList = (from p in exam
                                      select new Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = p.SubjectId,
                                          StudentCount = p.StudentCount,
                                          AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                          MaxMark = p.MaxMark,
                                          MinMark = p.MinMark,
                                          GoodRate = p.GoodRate,
                                          NormalRate = p.NormalRate,
                                          PassRate = p.PassRate,
                                          AvgRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.AvgRank).FirstOrDefault(),
                                          GoodRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.GoodRank).FirstOrDefault(),
                                          NormalRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.NormalRank).FirstOrDefault(),
                                          PassRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.PassRank).FirstOrDefault(),
                                      }).ToList();

                #endregion

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassSubjectList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var CheckedAll = Request["CheckedAll"] != null ? Request.Form["CheckedAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("ClassSubjectList", new { ExamId = vm.ExamId, GradeId = vm.GradeId, CheckedAll = CheckedAll, chkSubject = chksubjectList, searchText = vm.SearchText }));
        }

        public ActionResult ClassSubjectReport(int ExamId, int gradeId, string chkSubject, int classId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                //var arrystr = string.Empty;
                //var chksubjectList = (chkSubject != null && chkSubject !="") ? chkSubject : arrystr;

                vm.OptionList = new List<string>() { "均分排名", "优率排名", "良好率排名", "及格率排名" };
                var lsExam = new List<int>();
                lsExam.Add(ExamId);

                vm.GradeId = gradeId;
                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && (p.tbCourse.tbSubject.SubjectName.Contains(searchText) || searchText == null)
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

                vm.SubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                YearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();

                vm.ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = year.ExamName, Value = vm.ExamId.ToString() });

                var lastExam = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == vm.ExamId
                                select new
                                {
                                    p.ExamName
                                }).FirstOrDefault();

                vm.ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = lastExam.ExamName, Value = vm.LastExamId.ToString() });


                //任课老师
                var orgTeacherLisr = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                     .Include(d => d.tbOrg.tbClass)
                                      where p.tbOrg.IsDeleted == false
                                      && p.tbOrg.tbYear.Id == year.Id
                                      && p.tbTeacher.IsDeleted == false
                                      select new
                                      {
                                          OrgId = p.tbOrg.Id,
                                          ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();

                if (vm.chkSubject == null || chkSubject == "") return View(vm);
                var chksubjectList = chkSubject.Split(',');

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject)
                                       where p.tbGrade.Id == vm.GradeId
                                        && (chksubjectList.Contains(p.tbSubject.Id.ToString()) || p.tbSubject == null)
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsNormal,
                                           p.IsPass,
                                           p.IsTotal,
                                           SubjectId = p.tbSubject != null ? p.tbSubject.Id : 0
                                       }).ToList();

                //选中的科目
                var selectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                         where chksubjectList.Contains(p.Id.ToString())
                                         orderby p.No, p.SubjectName
                                         select new
                                         {
                                             p.Id,
                                             p.SubjectName,
                                         }).Distinct().ToList();

                vm.selectSubjectList = (from p in selectSubjectList
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.SubjectName,
                                        }).Distinct().ToList();
                if (chksubjectList.Contains("0"))
                {
                    vm.selectSubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });
                }

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var classtudentList = (from p in classStudent
                                       select new
                                       {
                                           p.ClassId,
                                           p.ClassName
                                       }).Distinct().ToList();

                vm.ClassStudentList = (from p in classtudentList
                                       select new Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           ClassName = p.ClassName,
                                           TeacherName = orgTeacherLisr.Where(c => c.ClassId == p.ClassId).Select(d => d.TeacherName).FirstOrDefault()
                                       }).Distinct().ToList();
                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                               && lsExam.Contains(p.tbExamCourse.tbExam.Id)
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        p.tbExamCourse.FullTotalMark
                                    }).ToList();


                #region  本次成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in classStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                             FullTotalMark = p.FullTotalMark
                                         }).ToList();
                //单个班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ExamId,
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ExamId = g.Key.ExamId,
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           StudentCount = g.Count().ToString(),
                                           AvgMark = g.Average(d => d.TotalMark),
                                           MaxMark = g.Max(d => d.TotalMark),
                                           MinMark = g.Min(d => d.TotalMark)
                                       }).ToList();
                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ExamId,
                                                p.ClassId,
                                                p.StudentId
                                            } into g
                                            select new
                                            {
                                                g.Key.ExamId,
                                                g.Key.ClassId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();
                var ty = (from p in totalStudentMarkList
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              StudentCount = g.Count()
                          }).ToList();
                var totalMarkList = (from p in totalStudentMarkList
                                     group p by new { p.ClassId, p.ExamId } into g
                                     select new Exam.Dto.ExamAnalyze.List
                                     {
                                         ExamId = g.Key.ExamId,
                                         ClassId = g.Key.ClassId,
                                         StudentCount = ty.Where(d => d.ExamId == g.Key.ExamId && d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                         AvgMark = g.Average(d => d.StudentTotalMark),
                                         MaxMark = g.Max(d => d.StudentTotalMark),
                                         MinMark = g.Min(d => d.StudentTotalMark)
                                     }).ToList();
                //总年级
                var gradeSubjectMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ExamId,
                                                p.SubjectId
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                ExamId = g.Key.ExamId,
                                                SubjectId = g.Key.SubjectId,
                                                ClassId = 0,
                                                StudentCount = g.Count().ToString(),
                                                AvgMark = g.Average(d => d.TotalMark),
                                                MaxMark = g.Max(d => d.TotalMark),
                                                MinMark = g.Min(d => d.TotalMark)
                                            }).ToList();

                var totalGradeStudentMarkList = (from p in classExamMarkList
                                                 group p by new
                                                 {
                                                     p.ExamId,
                                                     p.StudentId
                                                 } into g
                                                 select new
                                                 {
                                                     g.Key.ExamId,
                                                     ClassId = 0,
                                                     StudentTotalMark = g.Sum(d => d.TotalMark)
                                                 }).ToList();
                var tyGrade = (from p in totalStudentMarkList
                               group p by new
                               {
                                   p.ExamId
                               } into g
                               select new
                               {
                                   g.Key.ExamId,
                                   ClassId = 0,
                                   StudentCount = g.Count()
                               }).ToList();

                var totalGradeMarkList = (from p in totalGradeStudentMarkList
                                          group p by new { p.ClassId, p.ExamId } into g
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = g.Key.ExamId,
                                              ClassId = g.Key.ClassId,
                                              StudentCount = tyGrade.Where(d => d.ExamId == g.Key.ExamId && d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                              AvgMark = decimal.Round(g.Average(d => d.StudentTotalMark).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                              MaxMark = g.Max(d => d.StudentTotalMark),
                                              MinMark = g.Min(d => d.StudentTotalMark)
                                          }).ToList();

                #region 优秀良好及格排名
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;
                    var isNormal = o.IsNormal;
                    var isTotal = o.IsTotal;
                    if (isTotal && o.SubjectId == 0)//总分
                    {
                        if (isGood)
                        {
                            //分数段人数
                            var tm = (from p in totalStudentMarkList
                                      where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count()
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = 0,
                                          Status = decimal.One,
                                          StudentNum = p.StudentCount
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            var tm = (from p in totalStudentMarkList
                                      where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId,
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count()
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = 0,
                                          Status = 2,
                                          StudentNum = p.StudentCount
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            var tm = (from p in totalStudentMarkList
                                      where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count()
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = 0,
                                          Status = 3,
                                          StudentNum = p.StudentCount
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                    }
                    else//各科目
                    {
                        //优秀科目人数
                        if (isGood)
                        {
                            //分数段人数
                            var tm = (from p in classExamMarkList
                                      where p.SubjectId == o.SubjectId
                                      && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count(),
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = o.SubjectId,
                                          Status = decimal.One,
                                          StudentNum = p.StudentCount,
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            var tm = (from p in classExamMarkList
                                      where p.SubjectId == o.SubjectId
                                      && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId,
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count(),
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = o.SubjectId,
                                          Status = 2,
                                          StudentNum = p.StudentCount,
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            var tm = (from p in classExamMarkList
                                      where p.SubjectId == o.SubjectId
                                      && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                      group p by new
                                      {
                                          p.ExamId,
                                          p.ClassId
                                      } into g
                                      select new
                                      {
                                          g.Key.ExamId,
                                          g.Key.ClassId,
                                          StudentCount = g.Count(),
                                      }).ToList();

                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = o.SubjectId,
                                          Status = 3,
                                          StudentNum = p.StudentCount,
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                    }
                }

                var tk = (from p in lst
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                              p.SubjectId,
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              g.Key.SubjectId,
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum)
                          }).ToList();
                //年级项
                var tkGrade = (from p in tk
                               group p by new
                               {
                                   p.ExamId,
                                   p.SubjectId,
                                   p.Status
                               } into g
                               select new
                               {
                                   g.Key.ExamId,
                                   g.Key.SubjectId,
                                   ClassId = 0,
                                   g.Key.Status,
                                   StudentNum = g.Sum(d => d.StudentNum)
                               }).ToList();

                var exam = (from p in subjectMarkList.Union(totalMarkList)

                            select new
                            {
                                ExamId = p.ExamId,
                                ClassId = p.ClassId,
                                SubjectId = p.SubjectId,
                                StudentCount = p.StudentCount.ToString(),
                                AvgMark = p.AvgMark,
                                MaxMark = p.MaxMark,
                                MinMark = p.MinMark,
                                GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                            }).ToList();

                var examGrade = (from p in gradeSubjectMarkList.Union(totalGradeMarkList)
                                 select new Dto.ExamAnalyze.List
                                 {
                                     ExamId = p.ExamId,
                                     ClassId = p.ClassId,
                                     SubjectId = p.SubjectId,
                                     StudentCount = p.StudentCount.ToString(),
                                     AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                     MaxMark = p.MaxMark,
                                     MinMark = p.MinMark,
                                     GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                     NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                     PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                 }).ToList();

                //科目班级排名
                var lsComm = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in exam)
                {
                    var temp = new Exam.Dto.ExamAnalyze.List
                    {
                        ExamId = o.ExamId,
                        ClassId = o.ClassId,
                        SubjectId = o.SubjectId,
                        AvgRank = 0,
                        GoodRank = 0,
                        NormalRank = 0,
                        PassRank = 0
                    };
                    lsComm.Add(temp);
                }
                foreach (var examId in lsExam)
                {
                    foreach (var subject in vm.selectSubjectList)
                    {
                        var rank = 0;
                        decimal? mark = null;
                        var count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.AvgRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.GoodRate))
                        {
                            if (mark != t.GoodRate)
                            {
                                mark = t.GoodRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.GoodRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.NormalRate))
                        {
                            if (mark != t.NormalRate)
                            {
                                mark = t.NormalRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.NormalRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.PassRate))
                        {
                            if (mark != t.PassRate)
                            {
                                mark = t.PassRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == examId && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.PassRank = rank;
                            }
                        }
                    }
                }


                #endregion
                vm.ExamGradeAnalyzeList = examGrade;

                vm.ExamAnalyzeList = (from p in exam
                                      where p.ClassId == classId
                                      select new Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.ExamId,
                                          ClassId = p.ClassId,
                                          SubjectId = p.SubjectId,
                                          StudentCount = p.StudentCount,
                                          AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                          MaxMark = p.MaxMark,
                                          MinMark = p.MinMark,
                                          GoodRate = p.GoodRate,
                                          NormalRate = p.NormalRate,
                                          PassRate = p.PassRate,
                                          AvgRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.AvgRank).FirstOrDefault(),
                                          GoodRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.GoodRank).FirstOrDefault(),
                                          NormalRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.NormalRank).FirstOrDefault(),
                                          PassRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.PassRank).FirstOrDefault(),
                                      }).ToList();

                //获取科目
                //var subjectList = vm.selectSubjectList.Select(d => d.Text).Distinct().ToList();
                vm.chkSelectSubject = Code.Common.ToJSONString(vm.selectSubjectList.Select(d => d.Text).ToList());
                //排名
                vm.RankName = Code.Common.ToJSONString(vm.OptionList);

                var totalGradeList = new List<object>();

                var avgRankList = new List<object>();
                var goodRankList = new List<object>();
                var normalRankList = new List<object>();
                var passRankList = new List<object>();
                foreach (var subject in vm.selectSubjectList)
                {
                    var examScore = vm.ExamAnalyzeList.Where(d => d.SubjectId == subject.Value.ConvertToDecimal()).FirstOrDefault();
                    if (examScore != null)
                    {
                        avgRankList.Add(examScore.AvgRank);
                        goodRankList.Add(examScore.GoodRank);
                        normalRankList.Add(examScore.NormalRank);
                        passRankList.Add(examScore.PassRank);
                    }
                }
                totalGradeList.Add(new { name = vm.OptionList[0], type = "line", data = avgRankList });
                totalGradeList.Add(new { name = vm.OptionList[1], type = "line", data = goodRankList });
                totalGradeList.Add(new { name = vm.OptionList[2], type = "line", data = normalRankList });
                totalGradeList.Add(new { name = vm.OptionList[3], type = "line", data = passRankList });
                vm.ReportScoreGrade = Code.Common.ToJSONString((totalGradeList));
                #endregion

                return View(vm);
            }
        }
        #endregion

        #region  导出
        public ActionResult ExportClass(int examId, int lastexamId, int gradeId, string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计

                var lsExam = new List<int>();
                lsExam.Add(examId);
                lsExam.Add(lastexamId);

                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == gradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && courseIdList.Contains(p.tbCourse.Id)
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(searchText) || searchText == null)
                                       orderby p.tbCourse.tbSubject.No
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id,
                                           CourseId = p.tbCourse.Id
                                       }).Distinct().ToList();
                var SubjectList = (from p in examSubjectList
                                   select new Dto.ExamAnalyze.List()
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.SubjectId
                                   }).Distinct().ToList();

                var SubjectIds = SubjectList.Select(d => d.SubjectId).ToList();

                SubjectList.Insert(0, new Dto.ExamAnalyze.List() { SubjectName = "总分", SubjectId = 0 });

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == examId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                YearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();

                if (year == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var ExamThanList = new List<System.Web.Mvc.SelectListItem>();
                ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = year.ExamName, Value = examId.ToString() });

                var lastExam = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == lastexamId
                                select new
                                {
                                    p.ExamName
                                }).FirstOrDefault();

                ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = lastExam.ExamName, Value = lastexamId.ToString() });


                //任课老师
                var CourseIds = examSubjectList.Select(d => d.CourseId).Distinct().ToList();
                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                     .Include(d => d.tbOrg.tbClass)
                                      where p.tbOrg.IsDeleted == false
                                      && CourseIds.Contains(p.tbOrg.tbCourse.Id)
                                      && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
                                      && p.tbOrg.tbYear.Id == year.Id
                                      && p.tbTeacher.IsDeleted == false
                                      select new Dto.ExamAnalyze.List
                                      {
                                          SubjectId = p.tbOrg.tbCourse.tbSubject.Id,
                                          ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();
                //班主任
                var classTeacher = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                    where p.tbClass.IsDeleted == false
                                    && p.tbClass.tbYear.Id == year.YearId
                                    && p.tbTeacher.IsDeleted == false
                                    select new Dto.ExamAnalyze.List
                                    {
                                        ClassId = p.tbClass.Id,
                                        TeacherName = p.tbTeacher.TeacherName
                                    }).ToList();

                if (string.IsNullOrEmpty(chkClass)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkClassList = chkClass.Split(',');
                //分数段分组
                var examSegmentGroupList = (from p in db.Table<Exam.Entity.tbExam>().Include(d => d.tbExamSegmentGroup)
                                            where lsExam.Contains(p.Id)
                                            select new
                                            {
                                                ExamId = p.Id,
                                                SegmentGroupId = p.tbExamSegmentGroup != null ? p.tbExamSegmentGroup.Id : 0
                                            }).ToList();
                var SegmentGroupIds = examSegmentGroupList.Select(d => d.SegmentGroupId).Distinct().ToList();
                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject)
                                       where p.tbGrade.Id == gradeId
                                        && SegmentGroupIds.Contains(p.tbExamSegmentGroup.Id)
                                        && (SubjectIds.Contains(p.tbSubject.Id) || p.tbSubject == null)
                                       select new
                                       {
                                           SegmentGroupId=p.tbExamSegmentGroup.Id,
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsNormal,
                                           p.IsPass,
                                           p.IsTotal,
                                           SubjectId = p.tbSubject != null ? p.tbSubject.Id : 0
                                       }).ToList();

                //选中班级
                var tbselctClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                        where chkClassList.Contains(p.Id.ToString())
                                        orderby p.No
                                        select new
                                        {
                                            Id = p.Id,
                                            ClassName = p.ClassName,
                                        }).ToList();
                var selctClassList = (from p in tbselctClassList
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Value = p.Id.ToString(),
                                          Text = p.ClassName,
                                      }).Distinct().ToList();


                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == gradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                    && chkClassList.Contains(p.tbClass.Id.ToString())
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.TotalMark != null
                                     && p.tbStudent.IsDeleted == false
                                     && lsExam.Contains(p.tbExamCourse.tbExam.Id)
                                     && SubjectIds.Contains(p.tbExamCourse.tbCourse.tbSubject.Id)
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                                    }).ToList();


                #region  本次成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in classStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName
                                         }).ToList();
                //单个班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ExamId,
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ExamId = g.Key.ExamId,
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           StudentCount = g.Count().ToString(),
                                           AvgMark = g.Average(d => d.TotalMark),
                                           MaxMark = g.Max(d => d.TotalMark),
                                           MinMark = g.Min(d => d.TotalMark)
                                       }).ToList();
                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ExamId,
                                                p.ClassId,
                                                p.StudentId
                                            } into g
                                            select new
                                            {
                                                g.Key.ExamId,
                                                g.Key.ClassId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();
                //总年级平均分
                var gradeSubjectTotalMarkList = (from p in classExamMarkList
                                                 group p by new
                                                 {
                                                     p.ExamId,
                                                     p.SubjectId
                                                 } into g
                                                 select new Exam.Dto.ExamAnalyze.List
                                                 {
                                                     ExamId = g.Key.ExamId,
                                                     ClassId = 0,
                                                     SubjectId = g.Key.SubjectId,
                                                     AvgMark = g.Average(d => d.TotalMark),
                                                 }).ToList();
                var gradeTotalMarkList = (from p in totalStudentMarkList
                                          group p by new
                                          {
                                              p.ExamId
                                          } into g
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = g.Key.ExamId,
                                              ClassId = 0,
                                              SubjectId = 0,
                                              AvgMark = g.Average(d => d.StudentTotalMark),
                                          }).ToList();

                var ty = (from p in totalStudentMarkList
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              StudentCount = g.Count()
                          }).ToList();
                var totalMarkList = (from p in totalStudentMarkList
                                     group p by new { p.ClassId, p.ExamId } into g
                                     select new Exam.Dto.ExamAnalyze.List
                                     {
                                         ExamId = g.Key.ExamId,
                                         ClassId = g.Key.ClassId,
                                         StudentCount = ty.Where(d => d.ExamId == g.Key.ExamId && d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                         AvgMark = g.Average(d => d.StudentTotalMark),
                                         MaxMark = g.Max(d => d.StudentTotalMark),
                                         MinMark = g.Min(d => d.StudentTotalMark)
                                     }).ToList();

                #region 优秀良好及格排名
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var examid in lsExam.Distinct())
                {
                    var segmentGroupId = examSegmentGroupList.Where(d => d.ExamId == examid).Select(d => d.SegmentGroupId).FirstOrDefault();
                    foreach (var o in SegmentMarkList.Where(d => d.SegmentGroupId == segmentGroupId))
                    {
                        var isGood = o.IsGood;
                        var isPass = o.IsPass;
                        var isNormal = o.IsNormal;
                        var isTotal = o.IsTotal;
                        if (isTotal && o.SubjectId == 0)//总分
                        {
                            if (isGood)
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d=>d.ExamId==examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = decimal.One,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isNormal)//良好人数
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d => d.ExamId == examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId,
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = 2,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isPass)//及格人数
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d => d.ExamId == examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = 3,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                        }
                        else//各科目
                        {
                            //优秀科目人数
                            if (isGood)
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = decimal.One,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isNormal)//良好人数
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId,
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = 2,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isPass)//及格人数
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = 3,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                        }
                    }
                }
                var tk = (from p in lst
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                              p.SubjectId,
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              g.Key.SubjectId,
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum)
                          }).ToList();

                var exam = (from p in subjectMarkList.Union(totalMarkList)
                            select new
                            {
                                ExamId = p.ExamId,
                                ClassId = p.ClassId,
                                SubjectId = p.SubjectId,
                                StudentCount = p.StudentCount.ToString(),
                                AvgMark = p.AvgMark,
                                MaxMark = p.MaxMark,
                                MinMark = p.MinMark,
                                GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                            }).ToList();


                //科目班级排名
                var lsComm = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in exam)
                {
                    var temp = new Exam.Dto.ExamAnalyze.List
                    {
                        ExamId = o.ExamId,
                        ClassId = o.ClassId,
                        SubjectId = o.SubjectId,
                        AvgRank = 0,
                        GoodRank = 0,
                        NormalRank = 0,
                        PassRank = 0
                    };
                    lsComm.Add(temp);
                }
                foreach (var ea in lsExam)
                {
                    foreach (var subject in SubjectList)
                    {
                        var rank = 0;
                        decimal? mark = null;
                        var count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == ea && d.SubjectId == subject.SubjectId).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == ea && p.ClassId == t.ClassId && p.SubjectId == subject.SubjectId
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.AvgRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == ea && d.SubjectId == subject.SubjectId).OrderByDescending(d => d.GoodRate))
                        {
                            if (mark != t.GoodRate)
                            {
                                mark = t.GoodRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == ea && p.ClassId == t.ClassId && p.SubjectId == subject.SubjectId
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.GoodRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == ea && d.SubjectId == subject.SubjectId).OrderByDescending(d => d.NormalRate))
                        {
                            if (mark != t.NormalRate)
                            {
                                mark = t.NormalRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == ea && p.ClassId == t.ClassId && p.SubjectId == subject.SubjectId
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.NormalRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == ea && d.SubjectId == subject.SubjectId).OrderByDescending(d => d.PassRate))
                        {
                            if (mark != t.PassRate)
                            {
                                mark = t.PassRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == ea && p.ClassId == t.ClassId && p.SubjectId == subject.SubjectId
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.PassRank = rank;
                            }
                        }
                    }
                }


                #endregion
                var ExamGradeAnalyzeList = (from p in gradeSubjectTotalMarkList.Union(gradeTotalMarkList)
                                           select new Dto.ExamAnalyze.List
                                           {
                                               ExamId = p.ExamId,
                                               ClassId = p.ClassId,
                                               SubjectId = p.SubjectId,
                                               AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                           }).ToList();
                var examMark = (from p in exam
                                select new Dto.ExamAnalyze.List
                                {
                                    ExamId = p.ExamId,
                                    ClassId = p.ClassId,
                                    SubjectId = p.SubjectId,
                                    StudentCount = p.StudentCount,
                                    AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                    MaxMark = p.MaxMark,
                                    MinMark = p.MinMark,
                                    GoodRate = p.GoodRate,
                                    NormalRate = p.NormalRate,
                                    PassRate = p.PassRate,
                                    AvgRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.AvgRank).FirstOrDefault(),
                                    GoodRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.GoodRank).FirstOrDefault(),
                                    NormalRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.NormalRank).FirstOrDefault(),
                                    PassRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.PassRank).FirstOrDefault(),
                                }).ToList();

                #endregion

                #endregion

                #region 导出
                var OptionList = new List<string>() { "教师", "科目", "实考数", "平均分", "最高分", "最低分", "优秀率%", "良好率%", "及格率%", "均分排名", "优率排名", "良好率排名", "及格率排名" };
                var ClumnList = new List<string>() { "均分进退", "优秀进退", "良好进退", "及格进退" };
                var BList = new List<string>() { "上次", "本次", "近期变化" };
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = hssfworkbook.CreateCellStyle();//设置垂直居中格式
                cellstyle.VerticalAlignment = VerticalAlignment.Center;//垂直居中
                cellstyle.Alignment = HorizontalAlignment.Center;//居中
                cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                //缩小字体填充  
                cellstyle.ShrinkToFit = false;

                var sheetName = ExamThanList.Where(d => d.Value == examId.ToString()).Select(d => d.Text).FirstOrDefault() + "基础分析-按班级";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                foreach (var classId in selctClassList)
                {
                    IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                    //表头
                    ICell cell = cellHeader.CreateCell(0);
                    cell.SetCellValue(classId.Text);
                    CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, 0, 19);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    rowStartIndex++;
                    var cuExamName = ExamThanList.Where(d => d.Value == examId.ToString()).Select(d => d.Text).FirstOrDefault();
                    cellHeader = sheet1.CreateRow(rowStartIndex);
                    cell = cellHeader.CreateCell(0);
                    cell.SetCellValue(cuExamName);
                    var cellRangeAddress1 = new CellRangeAddress(rowStartIndex, rowStartIndex, 0, OptionList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress1, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress1);

                    var LastExamName = ExamThanList.Where(d => d.Value == lastexamId.ToString()).Select(d => d.Text).FirstOrDefault();
                    cell = cellHeader.CreateCell(OptionList.Count());
                    cell.SetCellValue(LastExamName);
                    var cellRangeAddress2 = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count(), OptionList.Count() + ClumnList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress2, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress2);

                    cell = cellHeader.CreateCell(OptionList.Count() + ClumnList.Count());
                    cell.SetCellValue("B值进退");
                    var cellRangeAddress3 = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count() + ClumnList.Count(), OptionList.Count() + ClumnList.Count() + BList.Count()-1);
                    setRegionStyle(sheet1, cellRangeAddress3, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress3);

                    //表头第三行
                    IRow cellHeaderR = sheet1.CreateRow(rowStartIndex + 1);
                    var Order = 0;
                    var no = 0;
                    foreach (var option in OptionList)
                    {
                        ICell cell2 = cellHeaderR.CreateCell(Order);
                        cell2.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(Order, 15 * 256);
                        cell2.SetCellValue(option);
                        Order++;
                    }
                    foreach (var option in ClumnList)
                    {
                        ICell cell2 = cellHeaderR.CreateCell(13 + no);
                        cell2.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(13 + no, 15 * 256);
                        cell2.SetCellValue(option);
                        no++;
                    }
                    no = 0;
                    foreach (var option in BList)
                    {
                        ICell cell2 = cellHeaderR.CreateCell(17 + no);
                        cell2.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(17+ no, 15 * 256);
                        cell2.SetCellValue(option);
                        no++;
                    }

                    setBorder(cellRangeAddress, sheet1, hssfworkbook);
                    setBorder(cellRangeAddress1, sheet1, hssfworkbook);
                    setBorder(cellRangeAddress2, sheet1, hssfworkbook);

                    rowStartIndex++;
                    //数据行
                    foreach (var t in SubjectList)
                    {
                        var SubjectId = t.SubjectId;
                        var teacherName = string.Empty;
                        if (SubjectId != 0)
                        {
                            var teacher = string.Join(",", orgTeacherList.Where(d => d.ClassId == classId.Value.ConvertToInt() && d.SubjectId == SubjectId).Select(d => d.TeacherName).Distinct().ToArray());
                            teacherName = teacher;
                        }
                        else
                        {
                            var teacher = string.Join(",", classTeacher.Where(d => d.ClassId == classId.Value.ConvertToInt()).Select(d => d.TeacherName).Distinct().ToArray());
                            teacherName = teacher;
                        }
                        var markGrade =ExamGradeAnalyzeList.Where(d => d.ExamId ==examId && d.SubjectId == SubjectId
                                                && d.ClassId == 0
                                                ).Select(d => d).FirstOrDefault();

                        var markLastGrade =ExamGradeAnalyzeList.Where(d => d.ExamId == lastexamId && d.SubjectId == SubjectId
                                && d.ClassId == 0
                                ).Select(d => d).FirstOrDefault();

                        //B值变化
                        var cuAvg = examMark.Where(d => d.ExamId ==examId && d.SubjectId == SubjectId
                                                                                        && d.ClassId == classId.Value.ConvertToInt()
                                                                                        ).Select(d => d.AvgMark).FirstOrDefault();

                        var cuGradeAvg = markGrade != null ? markGrade.AvgMark : decimal.Zero;

                        var cuRate = cuGradeAvg > decimal.Zero ? Math.Round((cuAvg / cuGradeAvg).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero) : decimal.Zero;

                        var lastAvg = examMark.Where(d => d.ExamId ==lastexamId && d.SubjectId == SubjectId
                                                                                       && d.ClassId == classId.Value.ConvertToInt()
                                                                                       ).Select(d => d.AvgMark).FirstOrDefault();
                        var lastGradeAvg = markLastGrade != null ? markLastGrade.AvgMark : decimal.Zero;

                        var lastRate = lastGradeAvg > decimal.Zero ? Math.Round((lastAvg / lastGradeAvg).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero) : decimal.Zero;
                        var chageRate = cuRate - lastRate;

                        //本次考试
                        var mark = examMark.Where(d => d.ExamId == examId && d.SubjectId == t.SubjectId
                                                                                                && d.ClassId == classId.Value.ConvertToInt()
                                                                                                ).Select(d => d).FirstOrDefault();

                        cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                        cell = cellHeader.CreateCell(0);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(teacherName);

                        cell = cellHeader.CreateCell(1);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(t.SubjectName);

                        cell = cellHeader.CreateCell(2);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.StudentCount : string.Empty);

                        cell = cellHeader.CreateCell(3);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.AvgMark.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(4);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.MaxMark.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(5);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.MinMark.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(6);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.GoodRate.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(7);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.NormalRate.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(8);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.PassRate.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(9);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.AvgRank.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(10);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.GoodRank.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(11);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.NormalRate.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(12);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.PassRank.ToString() : string.Empty);

                        var lastmark = examMark.Where(d => d.ExamId == lastexamId && d.SubjectId == t.SubjectId
                                                                                                 && d.ClassId == classId.Value.ConvertToInt()
                                                                                                ).Select(d => d).FirstOrDefault();

                        var avgRank = ((mark != null ? mark.AvgRank : 0) - (lastmark != null ? lastmark.AvgRank : 0)).ToString();
                        var goodRank = ((mark != null ? mark.GoodRank : 0) - (lastmark != null ? lastmark.GoodRank : 0)).ToString();
                        var normalRank = ((mark != null ? mark.NormalRank : 0) - (lastmark != null ? lastmark.NormalRank : 0)).ToString();
                        var passRank = ((mark != null ? mark.PassRank : 0) - (lastmark != null ? lastmark.PassRank : 0)).ToString();
                        if (mark == null || lastmark == null)
                        {
                            avgRank = string.Empty;
                            goodRank = string.Empty;
                            normalRank = string.Empty;
                            passRank = string.Empty;
                        }

                        cell = cellHeader.CreateCell(13);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(avgRank);

                        cell = cellHeader.CreateCell(14);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(goodRank);

                        cell = cellHeader.CreateCell(15);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(normalRank);

                        cell = cellHeader.CreateCell(16);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(passRank);

                        cell = cellHeader.CreateCell(17);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(lastRate.ToString());

                        cell = cellHeader.CreateCell(18);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(cuRate.ToString());

                        cell = cellHeader.CreateCell(19);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(chageRate.ToString());

                        rowStartIndex++;
                    }

                    rowStartIndex += 5;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("基本分析按班级报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportSubject(int examId, int lastexamId, int gradeId, string chkSubject, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计

                var lsExam = new List<int>();
                lsExam.Add(examId);
                lsExam.Add(lastexamId);

                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == gradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(searchText) || searchText == null)
                                        && courseIdList.Contains(p.tbCourse.Id)
                                       orderby p.tbCourse.tbSubject.No
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id,
                                           SubjectNo=p.tbCourse.tbSubject.No,
                                           CourseId = p.tbCourse.Id
                                       }).Distinct().ToList();
                var SubjectList = (from p in examSubjectList
                                   orderby p.SubjectNo
                                   select new Dto.ExamAnalyze.List()
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.SubjectId
                                   }).Distinct().ToList();


                SubjectList.Insert(0, new Dto.ExamAnalyze.List() { SubjectName = "总分", SubjectId = 0 });

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == examId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                YearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();
                if (year == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var ExamThanList = new List<System.Web.Mvc.SelectListItem>();
                ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = year.ExamName, Value = examId.ToString() });

                var lastExam = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.Id == lastexamId
                                select new
                                {
                                    p.ExamName
                                }).FirstOrDefault();

                ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = lastExam.ExamName, Value = lastexamId.ToString() });


                //任课老师
                var CourseIds = examSubjectList.Select(d => d.CourseId).Distinct().ToList();
                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                     .Include(d => d.tbOrg.tbClass)
                                      where p.tbOrg.IsDeleted == false
                                      && CourseIds.Contains(p.tbOrg.tbCourse.Id)
                                      && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
                                      && p.tbOrg.tbYear.Id == year.Id
                                      && p.tbTeacher.IsDeleted == false
                                      select new Dto.ExamAnalyze.List
                                      {
                                          SubjectId = p.tbOrg.tbCourse.tbSubject.Id,
                                          ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();
                //班主任
                var classTeacher = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                    where p.tbClass.IsDeleted == false
                                    && p.tbClass.tbYear.Id == year.YearId
                                    && p.tbTeacher.IsDeleted == false
                                    select new Dto.ExamAnalyze.List
                                    {
                                        ClassId = p.tbClass.Id,
                                        TeacherName = p.tbTeacher.TeacherName
                                    }).ToList();

                if (string.IsNullOrEmpty(chkSubject)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chksubjectList = chkSubject.Split(',');
                var examSegmentGroupList = (from p in db.Table<Exam.Entity.tbExam>().Include(d => d.tbExamSegmentGroup)
                                            where lsExam.Contains(p.Id)
                                            select new
                                            {
                                                ExamId = p.Id,
                                                SegmentGroupId = p.tbExamSegmentGroup != null ? p.tbExamSegmentGroup.Id : 0
                                            }).ToList();
                var SegmentGroupIds = examSegmentGroupList.Select(d => d.SegmentGroupId).Distinct().ToList();
                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject).Include(d=>d.tbExamSegmentGroup)
                                       where p.tbGrade.Id == gradeId
                                        && (chksubjectList.Contains(p.tbSubject.Id.ToString()) || p.tbSubject == null)
                                        && SegmentGroupIds.Contains(p.tbExamSegmentGroup.Id)
                                       select new
                                       {
                                           SegmentGroupId=p.tbExamSegmentGroup.Id,
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsNormal,
                                           p.IsPass,
                                           p.IsTotal,
                                           SubjectId = p.tbSubject != null ? p.tbSubject.Id : 0
                                       }).ToList();

                //选中的科目
                var selectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                         where chksubjectList.Contains(p.Id.ToString())
                                         orderby p.No, p.SubjectName
                                         select new
                                         {
                                             p.Id,
                                             p.SubjectName,
                                         }).Distinct().ToList();

                var selectSubject = (from p in selectSubjectList
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Value = p.Id.ToString(),
                                         Text = p.SubjectName,
                                     }).Distinct().ToList();
                if (chksubjectList.Contains("0"))
                {
                    selectSubject.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });
                }

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == gradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var classtudentList = (from p in classStudent
                                       select new
                                       {
                                           p.ClassId,
                                           p.ClassName
                                       }).Distinct().ToList();

                var classTeacherList = (from p in classtudentList
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = p.ClassId,
                                            ClassName = p.ClassName,
                                        }).Distinct().ToList();
                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.TotalMark != null
                                     && p.tbStudent.IsDeleted == false
                                     && lsExam.Contains(p.tbExamCourse.tbExam.Id)
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        p.tbExamCourse.FullTotalMark
                                    }).ToList();


                #region  本次成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in classStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                             FullTotalMark = p.FullTotalMark
                                         }).ToList();
                //单个班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ExamId,
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ExamId = g.Key.ExamId,
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           StudentCount = g.Count().ToString(),
                                           AvgMark = g.Average(d => d.TotalMark),
                                           MaxMark = g.Max(d => d.TotalMark),
                                           MinMark = g.Min(d => d.TotalMark)
                                       }).ToList();
                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ExamId,
                                                p.ClassId,
                                                p.StudentId
                                            } into g
                                            select new
                                            {
                                                g.Key.ExamId,
                                                g.Key.ClassId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();
                var ty = (from p in totalStudentMarkList
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              StudentCount = g.Count()
                          }).ToList();
                var totalMarkList = (from p in totalStudentMarkList
                                     group p by new { p.ClassId, p.ExamId } into g
                                     select new Exam.Dto.ExamAnalyze.List
                                     {
                                         ExamId = g.Key.ExamId,
                                         ClassId = g.Key.ClassId,
                                         StudentCount = ty.Where(d => d.ExamId == g.Key.ExamId && d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                         AvgMark = g.Average(d => d.StudentTotalMark),
                                         MaxMark = g.Max(d => d.StudentTotalMark),
                                         MinMark = g.Min(d => d.StudentTotalMark)
                                     }).ToList();
                //总年级
                var gradeSubjectMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ExamId,
                                                p.SubjectId
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                ExamId = g.Key.ExamId,
                                                SubjectId = g.Key.SubjectId,
                                                ClassId = 0,
                                                StudentCount = g.Count().ToString(),
                                                AvgMark = g.Average(d => d.TotalMark),
                                                MaxMark = g.Max(d => d.TotalMark),
                                                MinMark = g.Min(d => d.TotalMark)
                                            }).ToList();

                var totalGradeStudentMarkList = (from p in classExamMarkList
                                                 group p by new
                                                 {
                                                     p.ExamId,
                                                     p.StudentId
                                                 } into g
                                                 select new
                                                 {
                                                     g.Key.ExamId,
                                                     ClassId = 0,
                                                     StudentTotalMark = g.Sum(d => d.TotalMark)
                                                 }).ToList();
                var tyGrade = (from p in totalStudentMarkList
                               group p by new
                               {
                                   p.ExamId
                               } into g
                               select new
                               {
                                   g.Key.ExamId,
                                   ClassId = 0,
                                   StudentCount = g.Count()
                               }).ToList();

                var totalGradeMarkList = (from p in totalGradeStudentMarkList
                                          group p by new { p.ClassId, p.ExamId } into g
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = g.Key.ExamId,
                                              ClassId = g.Key.ClassId,
                                              StudentCount = tyGrade.Where(d => d.ExamId == g.Key.ExamId && d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                              AvgMark = g.Average(d => d.StudentTotalMark),
                                              MaxMark = g.Max(d => d.StudentTotalMark),
                                              MinMark = g.Min(d => d.StudentTotalMark)
                                          }).ToList();

                #region 优秀良好及格排名
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var examid in lsExam.Distinct())
                {
                    var segmentGroupId = examSegmentGroupList.Where(d => d.ExamId == examid).Select(d => d.SegmentGroupId).FirstOrDefault();
                    foreach (var o in SegmentMarkList.Where(d => d.SegmentGroupId == segmentGroupId))
                    {
                        var isGood = o.IsGood;
                        var isPass = o.IsPass;
                        var isNormal = o.IsNormal;
                        var isTotal = o.IsTotal;
                        if (isTotal && o.SubjectId == 0)//总分
                        {
                            if (isGood)
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d=>d.ExamId== examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = decimal.One,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isNormal)//良好人数
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d => d.ExamId == examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId,
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = 2,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isPass)//及格人数
                            {
                                //分数段人数
                                var tm = (from p in totalStudentMarkList.Where(d => d.ExamId == examid)
                                          where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count()
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = 0,
                                              Status = 3,
                                              StudentNum = p.StudentCount
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                        }
                        else//各科目
                        {
                            //优秀科目人数
                            if (isGood)
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = decimal.One,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isNormal)//良好人数
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId,
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = 2,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                            if (isPass)//及格人数
                            {
                                //分数段人数
                                var tm = (from p in classExamMarkList.Where(d => d.ExamId == examid)
                                          where p.SubjectId == o.SubjectId
                                          && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                          group p by new
                                          {
                                              p.ExamId,
                                              p.ClassId
                                          } into g
                                          select new
                                          {
                                              g.Key.ExamId,
                                              g.Key.ClassId,
                                              StudentCount = g.Count(),
                                          }).ToList();

                                var tb = (from p in tm
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              ClassId = p.ClassId,
                                              SubjectId = o.SubjectId,
                                              Status = 3,
                                              StudentNum = p.StudentCount,
                                          }).ToList();
                                lst.AddRange(tb);
                            }
                        }
                    }
                }
                var tk = (from p in lst
                          group p by new
                          {
                              p.ExamId,
                              p.ClassId,
                              p.SubjectId,
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.ExamId,
                              g.Key.ClassId,
                              g.Key.SubjectId,
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum)
                          }).ToList();
                //年级项
                var tkGrade = (from p in tk
                               group p by new
                               {
                                   p.ExamId,
                                   p.SubjectId,
                                   p.Status
                               } into g
                               select new
                               {
                                   g.Key.ExamId,
                                   g.Key.SubjectId,
                                   ClassId = 0,
                                   g.Key.Status,
                                   StudentNum = g.Sum(d => d.StudentNum)
                               }).ToList();

                var exam = (from p in subjectMarkList.Union(totalMarkList)
                            select new
                            {
                                ExamId = p.ExamId,
                                ClassId = p.ClassId,
                                SubjectId = p.SubjectId,
                                StudentCount = p.StudentCount.ToString(),
                                AvgMark = p.AvgMark,
                                MaxMark = p.MaxMark,
                                MinMark = p.MinMark,
                                GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                            }).ToList();

                var examGrade = (from p in gradeSubjectMarkList.Union(totalGradeMarkList)
                                 select new Dto.ExamAnalyze.List
                                 {
                                     ExamId = p.ExamId,
                                     ClassId = p.ClassId,
                                     SubjectId = p.SubjectId,
                                     StudentCount = p.StudentCount.ToString(),
                                     AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                     MaxMark = p.MaxMark,
                                     MinMark = p.MinMark,
                                     GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                     NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                     PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tkGrade.Where(d => d.ExamId == p.ExamId && d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                 }).ToList();

                //科目班级排名
                var lsComm = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in exam)
                {
                    var temp = new Exam.Dto.ExamAnalyze.List
                    {
                        ExamId = o.ExamId,
                        ClassId = o.ClassId,
                        SubjectId = o.SubjectId,
                        AvgRank = 0,
                        GoodRank = 0,
                        NormalRank = 0,
                        PassRank = 0
                    };
                    lsComm.Add(temp);
                }
                foreach (var ea in lsExam.Distinct())
                {
                    foreach (var subject in selectSubject)
                    {
                        var rank = 0;
                        decimal? mark = null;
                        var count = 1;
                        foreach (var t in exam.Where(d => d.ExamId ==ea && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == ea && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.AvgRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == ea && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.GoodRate))
                        {
                            if (mark != t.GoodRate)
                            {
                                mark = t.GoodRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == ea && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.GoodRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == ea && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.NormalRate))
                        {
                            if (mark != t.NormalRate)
                            {
                                mark = t.NormalRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == ea && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.NormalRank = rank;
                            }
                        }
                        rank = 0;
                        mark = null;
                        count = 1;
                        foreach (var t in exam.Where(d => d.ExamId == ea && d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.PassRate))
                        {
                            if (mark != t.PassRate)
                            {
                                mark = t.PassRate;
                                rank = rank + count;
                                count = 1;
                            }
                            else
                            {
                                count = count + 1;
                            }
                            var tt = (from p in lsComm
                                      where p.ExamId == ea && p.ClassId == t.ClassId && p.SubjectId == subject.Value.ConvertToInt()
                                      select p).FirstOrDefault();
                            if (tt != null)
                            {
                                tt.PassRank = rank;
                            }
                        }
                    }
                }

                #endregion
                var ExamGradeAnalyzeList = examGrade;

                var examMark = (from p in exam
                                select new Dto.ExamAnalyze.List
                                {
                                    ExamId = p.ExamId,
                                    ClassId = p.ClassId,
                                    SubjectId = p.SubjectId,
                                    StudentCount = p.StudentCount,
                                    AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                    MaxMark = p.MaxMark,
                                    MinMark = p.MinMark,
                                    GoodRate = p.GoodRate,
                                    NormalRate = p.NormalRate,
                                    PassRate = p.PassRate,
                                    AvgRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.AvgRank).FirstOrDefault(),
                                    GoodRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.GoodRank).FirstOrDefault(),
                                    NormalRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.NormalRank).FirstOrDefault(),
                                    PassRank = lsComm.Where(d => d.ExamId == p.ExamId && d.SubjectId == p.SubjectId && d.ClassId == p.ClassId).Select(d => d.PassRank).FirstOrDefault(),
                                }).ToList();

                #endregion

                #endregion

                #region 导出
                var OptionList = new List<string>() { "班级", "教师", "实考数", "平均分", "最高分", "最低分", "优秀率%", "良好率%", "及格率%", "均分排名", "优率排名", "良好率排名", "及格率排名" };
                var ClumnList = new List<string>() { "均分进退", "优秀进退", "良好进退", "及格进退" };
                var BList = new List<string>() { "上次", "本次", "近期变化"};
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = hssfworkbook.CreateCellStyle();//设置垂直居中格式
                cellstyle.VerticalAlignment = VerticalAlignment.Center;//垂直居中
                cellstyle.Alignment = HorizontalAlignment.Center;//居中
                cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                //缩小字体填充  
                cellstyle.ShrinkToFit = false;

                var sheetName = ExamThanList.Where(d => d.Value == examId.ToString()).Select(d => d.Text).FirstOrDefault() + "基础分析";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                foreach (var subject in selectSubject)
                {
                    IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                    //表头
                    ICell cell = cellHeader.CreateCell(0);
                    cell.SetCellValue(subject.Text);
                    CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, 0, 19);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    rowStartIndex++;
                    var cuExamName = ExamThanList.Where(d => d.Value == examId.ToString()).Select(d => d.Text).FirstOrDefault();
                    cellHeader = sheet1.CreateRow(rowStartIndex);
                    cell = cellHeader.CreateCell(0);
                    cell.SetCellValue(cuExamName);
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, 0, OptionList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    var LastExamName = ExamThanList.Where(d => d.Value == lastexamId.ToString()).Select(d => d.Text).FirstOrDefault();
                    cell = cellHeader.CreateCell(OptionList.Count());
                    cell.SetCellValue(LastExamName);
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count(), OptionList.Count() + ClumnList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    cell = cellHeader.CreateCell(OptionList.Count() + ClumnList.Count());
                    cell.SetCellValue("B值进退");
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count() + ClumnList.Count(), OptionList.Count() + ClumnList.Count() + BList.Count()-1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    //表头第三行
                    IRow cellHeaderR = sheet1.CreateRow(rowStartIndex + 1);
                    var Order = 0;
                    var no = 0;
                    foreach (var option in OptionList)
                    {
                        ICell cell2 = cellHeaderR.CreateCell(Order);
                        cell2.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(Order, 15 * 256);
                        cell2.SetCellValue(option);
                        Order++;
                    }
                    foreach (var option in ClumnList)
                    {
                        ICell cell2 = cellHeaderR.CreateCell(13 + no);
                        cell2.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(Order + no, 15 * 256);
                        cell2.SetCellValue(option);
                        no++;
                    }
                    no = 0;
                    foreach (var option in BList)
                    {
                        ICell cell2 = cellHeaderR.CreateCell(17 + no);
                        cell2.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(17 + no, 15 * 256);
                        cell2.SetCellValue(option);
                        no++;
                    }

                    setBorder(cellRangeAddress, sheet1, hssfworkbook);

                    rowStartIndex++;
                    //数据行
                    var SubjectId = subject.Value.ConvertToInt();
                    var markGrade = examGrade.Where(d => d.ExamId == examId && d.SubjectId == SubjectId
                          && d.ClassId == 0
                          ).Select(d => d).FirstOrDefault();

                    var markLastGrade = examGrade.Where(d => d.ExamId == lastexamId && d.SubjectId == SubjectId
                        && d.ClassId == 0
                        ).Select(d => d).FirstOrDefault();

                    foreach (var t in classTeacherList)
                    {
                        var teacherName = string.Empty;
                        if (SubjectId != 0)
                        {
                            var teacher = string.Join(",", orgTeacherList.Where(d => d.ClassId == t.ClassId && d.SubjectId == SubjectId).Select(d => d.TeacherName).Distinct().ToArray());
                            teacherName = teacher;
                        }
                        else
                        {
                            var teacher = string.Join(",", classTeacher.Where(d => d.ClassId == t.ClassId).Select(d => d.TeacherName).Distinct().ToArray());
                            teacherName = teacher;
                        }

                        //B值变化
                        var cuAvg = examMark.Where(d => d.ExamId == examId && d.SubjectId == SubjectId
                                                                                        && d.ClassId == t.ClassId
                                                                                        ).Select(d => d.AvgMark).FirstOrDefault();

                        var cuGradeAvg = markGrade != null ? markGrade.AvgMark : decimal.Zero;

                        var cuRate = cuGradeAvg > decimal.Zero ? Math.Round((cuAvg / cuGradeAvg).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero) : decimal.Zero;

                        var lastAvg = examMark.Where(d => d.ExamId == lastexamId && d.SubjectId == SubjectId
                                                                                       && d.ClassId == t.ClassId
                                                                                       ).Select(d => d.AvgMark).FirstOrDefault();
                        var lastGradeAvg = markLastGrade != null ? markLastGrade.AvgMark : decimal.Zero;

                        var lastRate = lastGradeAvg > decimal.Zero ? Math.Round((lastAvg / lastGradeAvg).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero) : decimal.Zero;
                        var chageRate = cuRate - lastRate;

                        //本次考试
                        var mark = examMark.Where(d => d.ExamId == examId && d.SubjectId == subject.Value.ConvertToInt()
                                                                                                && d.ClassId == t.ClassId
                                                                                                ).Select(d => d).FirstOrDefault();

                        cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                        cell = cellHeader.CreateCell(0);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(t.ClassName);

                        cell = cellHeader.CreateCell(1);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(teacherName);

                        cell = cellHeader.CreateCell(2);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.StudentCount : string.Empty);

                        cell = cellHeader.CreateCell(3);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.AvgMark.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(4);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.MaxMark.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(5);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.MinMark.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(6);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.GoodRate.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(7);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.NormalRate.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(8);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.PassRate.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(9);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.AvgRank.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(10);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.GoodRank.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(11);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.NormalRank.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(12);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.PassRank.ToString() : string.Empty);

                        var lastmark = examMark.Where(d => d.ExamId == lastexamId && d.SubjectId == subject.Value.ConvertToInt()
                                                                                                 && d.ClassId == t.ClassId
                                                                                                ).Select(d => d).FirstOrDefault();

                        var avgRank =((mark != null ? mark.AvgRank : 0) - (lastmark != null ? lastmark.AvgRank : 0)).ToString();
                        var goodRank =((mark != null ? mark.GoodRank : 0) - (lastmark != null ? lastmark.GoodRank : 0)).ToString();
                        var normalRank =((mark != null ? mark.NormalRank : 0) - (lastmark != null ? lastmark.NormalRank : 0)).ToString();
                        var passRank =((mark != null ? mark.PassRank : 0) - (lastmark != null ? lastmark.PassRank : 0)).ToString();
                        if(mark==null || lastmark==null)
                        {
                            avgRank = string.Empty;
                            goodRank = string.Empty;
                            normalRank = string.Empty;
                            passRank = string.Empty;
                        }
                        cell = cellHeader.CreateCell(13);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(avgRank);

                        cell = cellHeader.CreateCell(14);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(goodRank);

                        cell = cellHeader.CreateCell(15);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(normalRank);

                        cell = cellHeader.CreateCell(16);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(passRank);

                        cell = cellHeader.CreateCell(17);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(lastRate.ToString());

                        cell = cellHeader.CreateCell(18);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(cuRate.ToString());

                        cell = cellHeader.CreateCell(19);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(chageRate.ToString());

                        rowStartIndex++;
                    }
                    //年级行
                    cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                   

                    cell = cellHeader.CreateCell(0);
                    cell.CellStyle = cellstyle;
                    cell.SetCellValue("年级");

                    cell = cellHeader.CreateCell(1);
                    cell.CellStyle = cellstyle;
                    cell.SetCellValue(string.Empty);

                    cell = cellHeader.CreateCell(2);
                    cell.CellStyle = cellstyle;
                    cell.SetCellValue(markGrade != null ? markGrade.StudentCount : string.Empty);

                    cell = cellHeader.CreateCell(3);
                    cell.CellStyle = cellstyle;
                    cell.SetCellValue(markGrade != null ? markGrade.AvgMark.ToString() : string.Empty);

                    cell = cellHeader.CreateCell(4);
                    cell.CellStyle = cellstyle;
                    cell.SetCellValue(markGrade != null ? markGrade.MaxMark.ToString() : string.Empty);

                    cell = cellHeader.CreateCell(5);
                    cell.CellStyle = cellstyle;
                    cell.SetCellValue(markGrade != null ? markGrade.MinMark.ToString() : string.Empty);

                    cell = cellHeader.CreateCell(6);
                    cell.CellStyle = cellstyle;
                    cell.SetCellValue(markGrade != null ? markGrade.GoodRate.ToString() : string.Empty);

                    cell = cellHeader.CreateCell(7);
                    cell.CellStyle = cellstyle;
                    cell.SetCellValue(markGrade != null ? markGrade.NormalRate.ToString() : string.Empty);

                    cell = cellHeader.CreateCell(8);
                    cell.CellStyle = cellstyle;
                    cell.SetCellValue(markGrade != null ? markGrade.PassRate.ToString() : string.Empty);
                    for (var i = 0; i < 11; i++)
                    {
                        cell = cellHeader.CreateCell(9 + i);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(string.Empty);
                    }

                    rowStartIndex += 5;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("基本分析按科目报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportSegmentSubject(int examId, int gradeId, string chkSubject, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == examId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                TopYearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();

                if (year == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var ExamThanList = new List<System.Web.Mvc.SelectListItem>();
                ExamThanList.Add(new System.Web.Mvc.SelectListItem { Text = year.ExamName, Value = examId.ToString() });

                if (string.IsNullOrEmpty(chkSubject)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chksubjectList = chkSubject.Split(',');

                //分数段分组
                var segmentGroupId = (from p in db.Table<Exam.Entity.tbExam>().Include(d => d.tbExamSegmentGroup)
                                      where p.Id==examId
                                      select p.tbExamSegmentGroup.Id
                                            ).FirstOrDefault();
                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject)
                                       where p.tbGrade.Id == gradeId
                                        && p.tbExamSegmentGroup.Id==segmentGroupId
                                        && (chksubjectList.Contains(p.tbSubject.Id.ToString()) || p.tbSubject == null)
                                       orderby p.tbSubject.No, p.No, p.SegmentName
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsNormal,
                                           p.IsPass,
                                           p.IsTotal,
                                           SubjectId = p.tbSubject != null ? p.tbSubject.Id : 0
                                       }).ToList();
                var SegmentList = (from p in SegmentMarkList
                                   select new Dto.ExamAnalyze.SegmentList
                                   {
                                       SegmentId = p.SegmentId,
                                       SegmentName = p.SegmentName,
                                       SubjectId = p.SubjectId
                                   }).Distinct().ToList();

                //选中的科目
                var SubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   where chksubjectList.Contains(p.Id.ToString())
                                   orderby p.No, p.SubjectName
                                   select new
                                   {
                                       p.Id,
                                       p.SubjectName,
                                   }).Distinct().ToList();

                var selectSubjectList = (from p in SubjectList
                                         select new System.Web.Mvc.SelectListItem
                                         {
                                             Value = p.Id.ToString(),
                                             Text = p.SubjectName,
                                         }).Distinct().ToList();
                if (chksubjectList.Contains("0"))
                {
                    selectSubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });
                }

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == gradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.TopYearId
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                var studentList = (from p in classStudent
                                   select new
                                   {
                                       p.ClassId,
                                       p.ClassName
                                   }).Distinct().ToList();

                var ClassStudentList = (from p in studentList
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = p.ClassId,
                                            ClassName = p.ClassName
                                        }).Distinct().ToList();


                //考试成绩
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        p.tbExamCourse.FullTotalMark
                                    }).ToList();


                #region  成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in classStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName
                                         }).ToList();

                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.ClassId,
                                                p.StudentId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                SubjectId = 0,
                                                g.Key.StudentId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();

                #region 向下累计数
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    var isTotal = o.IsTotal;
                    if (isTotal && o.SubjectId == 0)//总分
                    {
                        //分数段人数
                        var tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count(),
                                  }).ToList();
                        //累计
                        var accumulateStudent = (from p in totalStudentMarkList
                                                 where p.StudentTotalMark >= o.MaxMark
                                                 group p by new
                                                 {
                                                     p.ClassId
                                                 } into g
                                                 select new
                                                 {
                                                     g.Key.ClassId,
                                                     StudentCount = g.Count(),
                                                 }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      SegmentId = o.SegmentId,
                                      ClassId = p.ClassId,
                                      SubjectId = 0,
                                      StudentNum = p.StudentCount,
                                      TotalStudentCount = accumulateStudent.Where(d => d.ClassId == p.ClassId).Select(d => d.StudentCount).FirstOrDefault(),//班级累计
                                      GradeStudentCount = tm.Sum(d => d.StudentCount),//年级人数，
                                      TotalGradeStudentCount = accumulateStudent.Sum(d => d.StudentCount)//年级累计
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                    else//各科目
                    {
                        //分数段人数
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var accumulateStudent = (from p in classExamMarkList
                                                 where p.SubjectId == o.SubjectId
                                                 && p.TotalMark >= o.MaxMark
                                                 group p by new
                                                 {
                                                     p.ClassId
                                                 } into g
                                                 select new
                                                 {
                                                     g.Key.ClassId,
                                                     StudentCount = g.Count(),
                                                 }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      SegmentId = o.SegmentId,
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                      TotalStudentCount = accumulateStudent.Where(d => d.ClassId == p.ClassId).Select(d => d.StudentCount).FirstOrDefault(),//班级累计
                                      GradeStudentCount = tm.Sum(d => d.StudentCount),//年级人数，
                                      TotalGradeStudentCount = accumulateStudent.Sum(d => d.StudentCount)//年级累计
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }

                #endregion

                var ExamAnalyzeList = (from p in lst
                                       select new Dto.ExamAnalyze.List
                                       {
                                           SegmentId = p.SegmentId,
                                           ClassId = p.ClassId,
                                           SubjectId = p.SubjectId,
                                           StudentNum = p.StudentNum,
                                           TotalStudentCount = p.TotalStudentCount,
                                           GradeStudentCount = p.GradeStudentCount,
                                           TotalGradeStudentCount = p.TotalGradeStudentCount
                                       }).ToList();

                #endregion

                #endregion

                #region 导出
                var OptionList = new List<string>() { "分数段" };
                var ClumnList = new List<string>() { "年级人数", "年级累计" };
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = hssfworkbook.CreateCellStyle();//设置垂直居中格式
                cellstyle.VerticalAlignment = VerticalAlignment.Center;//垂直居中
                cellstyle.Alignment = HorizontalAlignment.Center;//居中
                cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                //缩小字体填充  
                cellstyle.ShrinkToFit = false;

                var sheetName = ExamThanList.Where(d => d.Value == examId.ToString()).Select(d => d.Text).FirstOrDefault() + "分数段";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                foreach (var subject in selectSubjectList)
                {
                    IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                    //表头
                    ICell cell = cellHeader.CreateCell(0);
                    cell.SetCellValue(subject.Text);
                    CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, 0, OptionList.Count() + ClassStudentList.Count() + ClumnList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    rowStartIndex++;
                    cellHeader = sheet1.CreateRow(rowStartIndex);

                    cell = cellHeader.CreateCell(0);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(OptionList[0]);

                    var No = 0;
                    foreach (var t in ClassStudentList)
                    {
                        cell = cellHeader.CreateCell(No + 1);
                        cell.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(No, 15 * 256);
                        cell.SetCellValue(t.ClassName);
                        No++;
                    }

                    cell = cellHeader.CreateCell(ClassStudentList.Count() + 1);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(ClumnList[0]);

                    cell = cellHeader.CreateCell(ClassStudentList.Count() + 2);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(ClumnList[1]);

                    //数据行
                    foreach (var t in SegmentList.Where(d => d.SubjectId == subject.Value.ConvertToInt()))
                    {
                        cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                        cell = cellHeader.CreateCell(0);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(t.SegmentName);

                        var markTotal = ExamAnalyzeList.Where(d => d.SegmentId == t.SegmentId && d.SubjectId ==subject.Value.ConvertToInt()
                                                                                           ).Select(d => d).FirstOrDefault();
                        var Order = 0;
                        foreach (var c in ClassStudentList)
                        {
                            var mark = ExamAnalyzeList.Where(d => d.SegmentId == t.SegmentId && d.SubjectId ==subject.Value.ConvertToInt()
                                                                                                                           && d.ClassId == c.ClassId
                                                                                                                           ).Select(d => d).FirstOrDefault();
                            if (mark != null)
                            {
                                var st = mark.StudentNum + "/" + mark.TotalStudentCount;
                                cell = cellHeader.CreateCell(Order + 1);
                                cell.CellStyle = cellstyle;
                                cell.SetCellValue(st);
                                Order++;
                            }
                            else
                            {
                                cell = cellHeader.CreateCell(Order + 1);
                                cell.CellStyle = cellstyle;
                                cell.SetCellValue("0/0");
                                Order++;
                            }
                        }
                        if (markTotal != null)
                        {
                            cell = cellHeader.CreateCell(ClassStudentList.Count() + 1);
                            cell.CellStyle = cellstyle;
                            cell.SetCellValue(markTotal.GradeStudentCount.ToString());

                            cell = cellHeader.CreateCell(ClassStudentList.Count() + 2);
                            cell.CellStyle = cellstyle;
                            cell.SetCellValue(markTotal.TotalGradeStudentCount.ToString());
                        }
                        else
                        {
                            cell = cellHeader.CreateCell(ClassStudentList.Count() + 1);
                            cell.CellStyle = cellstyle;
                            cell.SetCellValue(0);

                            cell = cellHeader.CreateCell(ClassStudentList.Count() + 2);
                            cell.CellStyle = cellstyle;
                            cell.SetCellValue(0);
                        }
                        rowStartIndex++;
                    }
                    rowStartIndex += 3;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("分数段报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportLevel(int examId, int lastexamId, int gradeId, string chkSubject, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 成绩统计
                var vm = new Models.ExamAnalyze.List();

                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == gradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && courseIdList.Contains(p.tbCourse.Id)
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(vm.SearchText) || vm.SearchText == null)
                                       orderby p.tbCourse.tbSubject.No
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id,
                                           CourseId = p.tbCourse.Id
                                       }).Distinct().ToList();

                if (string.IsNullOrEmpty(chkSubject)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chksubjectList = vm.chkSubject.Split(',');

                //选中的科目
                var selectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                         where chksubjectList.Contains(p.Id.ToString())
                                         orderby p.No, p.SubjectName
                                         select new
                                         {
                                             p.Id,
                                             p.SubjectName,
                                         }).Distinct().ToList();

                vm.selectSubjectList = (from p in selectSubjectList
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.SubjectName,
                                        }).Distinct().ToList();

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                TopYearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();

                if (year == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                //班主任
                var classTeacherList = (from p in db.Table<Basis.Entity.tbClassTeacher>()
                                     .Include(d => d.tbClass)
                                        where p.tbClass.IsDeleted == false
                                        && p.tbClass.tbYear.Id == year.TopYearId
                                        && p.tbTeacher.IsDeleted == false
                                        select new
                                        {
                                            ClassId = p.tbClass != null ? p.tbClass.Id : 0,
                                            TeacherName = p.tbTeacher.TeacherName
                                        }).ToList();

                //任课老师
                var courseIds = examSubjectList.Select(d => d.CourseId).Distinct().ToList();
                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                      .Include(d => d.tbOrg.tbClass)
                                      where p.tbOrg.IsDeleted == false
                                      && courseIds.Contains(p.tbOrg.tbCourse.Id)
                                      && p.tbOrg.tbYear.Id == year.Id
                                      && p.tbTeacher.IsDeleted == false
                                      select new Dto.ExamAnalyze.List
                                      {
                                          SubjectId = p.tbOrg.tbCourse.tbSubject.Id,
                                          ClassId = p.tbOrg.tbClass != null ? p.tbOrg.tbClass.Id : 0,
                                          TeacherName = p.tbTeacher.TeacherName
                                      }).ToList();


                //年级学生班级
                vm.StudentList = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                  where p.tbStudent.IsDeleted == false
                                  && p.tbClass.tbGrade.Id == gradeId
                                  && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.TopYearId
                                  orderby p.tbClass.No, p.tbClass.ClassName
                                  select new Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.tbClass.Id,
                                      ClassName = p.tbClass.ClassName,
                                      StudentId = p.tbStudent.Id
                                  }).ToList();

                var classtudentList = (from p in vm.StudentList
                                       select new
                                       {
                                           p.ClassId,
                                           p.ClassName
                                       }).Distinct().ToList();

                vm.ClassStudentList = (from p in classtudentList
                                       select new Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           ClassName = p.ClassName,
                                           TeacherName = classTeacherList.Where(c => c.ClassId == p.ClassId).Select(d => d.TeacherName).FirstOrDefault()
                                       }).Distinct().ToList();

                vm.ClassOrgStudentList = (from p in classtudentList
                                          select new Dto.ExamAnalyze.List
                                          {
                                              ClassId = p.ClassId,
                                              ClassName = p.ClassName,
                                          }).Distinct().ToList();

                #region 本次考试
                var exam = (from p in db.Table<Exam.Entity.tbExam>()
                            .Include(d => d.tbExamLevelGroup)
                            where p.IsDeleted == false
                            && p.Id == examId
                            select p).FirstOrDefault();

                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        p.tbExamCourse.FullTotalMark
                                    }).ToList();

                //获取科目满分
                var examSubjectTotal = (from p in examMarkList
                                        group p by new { p.SubjectId, p.FullTotalMark } into g
                                        select new
                                        {
                                            SubjectId = g.Key.SubjectId,
                                            FullTotalMark = g.Key.FullTotalMark,
                                        }).ToList();

                //获取考试总分满分
                var examTotal = examSubjectTotal.Sum(d => d.FullTotalMark);

                var classExamMarkList = (from p in examMarkList
                                         join t in vm.StudentList
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                             FullTotalMark = p.FullTotalMark
                                         }).ToList();

                //获取班级学生总分
                vm.ExamLsStudentList = (from p in classExamMarkList
                                        group p by new { p.ClassId, p.StudentId } into g
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = g.Key.ClassId,
                                            StudentId = g.Key.StudentId,
                                            TotalMark = g.Sum(d => d.TotalMark),
                                        }).ToList();

                //获取总分等级
                if (exam != null && exam.tbExamLevelGroup != null)
                {
                    vm.LsTotalLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                           where p.IsDeleted == false
                                           && p.tbExamLevelGroup.IsTotal == true
                                           && p.tbExamLevelGroup.IsDeleted == false
                                           && p.tbExamLevelGroup.Id == exam.tbExamLevelGroup.Id
                                           orderby p.No
                                           select p).ToList();
                }

                foreach (var a in vm.ClassStudentList)
                {
                    var studentTotalList = vm.ExamLsStudentList.Where(d => d.ClassId == a.ClassId).ToList();

                    foreach (var b in vm.LsTotalLevelList)
                    {
                        var classStudentLevel = new Dto.ExamAnalyze.List();
                        classStudentLevel.ClassId = a.ClassId;
                        classStudentLevel.TotalLevelId = b.Id;
                        if (studentTotalList.Where(d => (b.MinScore * examTotal / 100) <= d.TotalMark && d.TotalMark <= (b.MaxScore * examTotal / 100)).Count() > 0)
                        {
                            classStudentLevel.TotalLevelCount = studentTotalList.Where(d => (b.MinScore * examTotal / 100) <= d.TotalMark && d.TotalMark <= (b.MaxScore * examTotal / 100)).Count();
                        }
                        else
                        {
                            classStudentLevel.TotalLevelCount = 0;
                        }
                        vm.LsClassStudentLevelList.Add(classStudentLevel);
                    }
                }
                #endregion

                #region 对比考试
                exam = (from p in db.Table<Exam.Entity.tbExam>()
                            .Include(d => d.tbExamLevelGroup)
                        where p.IsDeleted == false
                        && p.Id == lastexamId
                        select p).FirstOrDefault();

                var examNtMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                      where p.tbExamCourse.IsDeleted == false
                                       && p.tbStudent.IsDeleted == false
                                       && p.tbExamCourse.tbExam.Id == lastexamId
                                       && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                      select new
                                      {
                                          p.TotalMark,
                                          ExamId = p.tbExamCourse.tbExam.Id,
                                          StudentId = p.tbStudent.Id,
                                          SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                          p.tbExamCourse.FullTotalMark
                                      }).ToList();

                //获取科目满分
                var examNtSubjectTotal = (from p in examNtMarkList
                                          group p by new { p.SubjectId, p.FullTotalMark } into g
                                          select new
                                          {
                                              SubjectId = g.Key.SubjectId,
                                              FullTotalMark = g.Key.FullTotalMark,
                                          }).ToList();

                //获取考试总分满分
                var examNtTotal = examNtSubjectTotal.Sum(d => d.FullTotalMark);

                var classExamNtMarkList = (from p in examNtMarkList
                                           join t in vm.StudentList
                                           on p.StudentId equals t.StudentId
                                           select new
                                           {
                                               ExamId = p.ExamId,
                                               StudentId = p.StudentId,
                                               SubjectId = p.SubjectId,
                                               TotalMark = p.TotalMark,
                                               ClassId = t.ClassId,
                                               ClassName = t.ClassName,
                                               FullTotalMark = p.FullTotalMark
                                           }).ToList();

                //获取班级学生总分
                vm.ExamNtStudentList = (from p in classExamNtMarkList
                                        group p by new { p.ClassId, p.StudentId } into g
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = g.Key.ClassId,
                                            StudentId = g.Key.StudentId,
                                            TotalMark = g.Sum(d => d.TotalMark),
                                        }).ToList();

                //获取总分等级
                if (exam != null && exam.tbExamLevelGroup != null)
                {
                    vm.NtTotalLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                           where p.IsDeleted == false
                                           && p.tbExamLevelGroup.IsTotal == true
                                           && p.tbExamLevelGroup.IsDeleted == false
                                           && p.tbExamLevelGroup.Id == exam.tbExamLevelGroup.Id
                                           orderby p.No
                                           select p).ToList();
                }

                var LictA = vm.LsTotalLevelList.Select(d => d.Id).ToList();
                var ListB = vm.NtTotalLevelList.Select(d => d.Id).ToList();
                if (vm.LsTotalLevelList.Count() == vm.NtTotalLevelList.Count() && LictA.Count(t => ListB.Contains(t)) == vm.LsTotalLevelList.Count())
                {
                    vm.TotalLevelList = vm.LsTotalLevelList;
                }

                foreach (var a in vm.ClassStudentList)
                {
                    var studentTotalList = vm.ExamNtStudentList.Where(d => d.ClassId == a.ClassId).ToList();

                    foreach (var b in vm.NtTotalLevelList)
                    {
                        var classStudentLevel = new Dto.ExamAnalyze.List();
                        classStudentLevel.ClassId = a.ClassId;
                        classStudentLevel.TotalLevelId = b.Id;
                        if (studentTotalList.Where(d => (b.MinScore * examTotal / 100) <= d.TotalMark && d.TotalMark <= (b.MaxScore * examTotal / 100)).Count() > 0)
                        {
                            classStudentLevel.TotalLevelCount = studentTotalList.Where(d => (b.MinScore * examTotal / 100) <= d.TotalMark && d.TotalMark <= (b.MaxScore * examTotal / 100)).Count();
                        }
                        else
                        {
                            classStudentLevel.TotalLevelCount = 0;
                        }
                        vm.NtClassStudentLevelList.Add(classStudentLevel);
                    }
                }
                #endregion

                #region 科目 本次考试
                #region 本次考试
                //获取等级
                var subjectLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                        .Include(d => d.tbExamLevelGroup)
                                        where p.IsDeleted == false
                                        && p.tbExamLevelGroup.IsDeleted == false
                                        && p.tbExamLevelGroup.IsTotal == false
                                        select p).ToList();

                var subjectMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                       where p.tbExamCourse.IsDeleted == false
                                        && p.tbStudent.IsDeleted == false
                                        && p.tbExamCourse.tbExam.Id == examId
                                        && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                       select new
                                       {
                                           p.TotalMark,
                                           ExamId = p.tbExamCourse.tbExam.Id,
                                           StudentId = p.tbStudent.Id,
                                           SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                           LevelGroupId = p.tbExamCourse.tbExamLevelGroup.Id,
                                           p.tbExamCourse.FullTotalMark,
                                           ExamLevelId = p.tbExamLevel != null ? p.tbExamLevel.Id : 0,
                                       }).ToList();

                var subjectTotalLevelList = (from p in subjectMarkList
                                             join t in subjectLevelList on p.LevelGroupId equals t.tbExamLevelGroup.Id
                                             //where t.IsTotal == false
                                             select new
                                             {
                                                 SubjectId = p.SubjectId,
                                                 TotalLevelId = t.Id,
                                                 TotalLavelName = t.ExamLevelName,
                                                 TotalLavelMax = t.MaxScore,
                                                 TotalLavelMin = t.MinScore,
                                             }).Distinct().ToList();
                vm.SubjectTotalLevelList = (from p in subjectTotalLevelList
                                            select new Dto.ExamAnalyze.List
                                            {
                                                SubjectId = p.SubjectId,
                                                TotalLevelId = p.TotalLevelId,
                                                TotalLavelName = p.TotalLavelName,
                                                TotalLavelMax = p.TotalLavelMax,
                                                TotalLavelMin = p.TotalLavelMin,
                                            }).ToList();

                var classSubjectExamMarkList = (from p in subjectMarkList
                                                join t in vm.StudentList
                                                on p.StudentId equals t.StudentId
                                                select new
                                                {
                                                    ExamId = p.ExamId,
                                                    StudentId = p.StudentId,
                                                    SubjectId = p.SubjectId,
                                                    TotalMark = p.TotalMark,
                                                    ClassId = t.ClassId,
                                                    ClassName = t.ClassName,
                                                    LevelGroupId = p.LevelGroupId,
                                                    FullTotalMark = p.FullTotalMark,
                                                    ExamLevelId = p.ExamLevelId,
                                                }).ToList();

                foreach (var selectSubject in vm.selectSubjectList)
                {
                    foreach (var a in vm.ClassOrgStudentList)
                    {
                        var studentTotalList = classSubjectExamMarkList.Where(d => d.ClassId == a.ClassId && d.SubjectId.ToString() == selectSubject.Value).ToList();

                        foreach (var b in vm.SubjectTotalLevelList.Where(d => d.SubjectId == selectSubject.Value.ConvertToInt()).Distinct().ToList())
                        {
                            var classStudentLevel = new Dto.ExamAnalyze.List();
                            classStudentLevel.ClassId = a.ClassId;
                            classStudentLevel.SubjectId = selectSubject.Value.ConvertToInt();
                            classStudentLevel.TotalLevelId = b.TotalLevelId;
                            //if (studentTotalList.Where(d => (b.TotalLavelMin * d.FullTotalMark / 100) <= d.TotalMark && d.TotalMark <= (b.TotalLavelMax * d.FullTotalMark / 100)).Count() > 0)
                            //{
                            //    classStudentLevel.TotalLevelCount = studentTotalList.Where(d => (b.TotalLavelMin * d.FullTotalMark / 100) <= d.TotalMark && d.TotalMark <= (b.TotalLavelMax * d.FullTotalMark / 100)).Count();
                            //}
                            //else
                            //{
                            //    classStudentLevel.TotalLevelCount = 0;
                            //}
                            if (studentTotalList.Where(d => d.ExamLevelId == b.TotalLevelId).ToList().Count() > 0)
                            {
                                classStudentLevel.TotalLevelCount = studentTotalList.Where(d => d.ExamLevelId == b.TotalLevelId).ToList().Count();
                            }
                            else
                            {
                                classStudentLevel.TotalLevelCount = 0;
                            }
                            vm.SClassStudentLevelList.Add(classStudentLevel);
                        }
                    }
                }
                #endregion

                #region 对比考试
                var examNtSubjectMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                             where p.tbExamCourse.IsDeleted == false
                                              && p.tbStudent.IsDeleted == false
                                              && p.tbExamCourse.tbExam.Id == lastexamId
                                              && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                             select new
                                             {
                                                 p.TotalMark,
                                                 ExamId = p.tbExamCourse.tbExam.Id,
                                                 StudentId = p.tbStudent.Id,
                                                 SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                                 LevelGroupId = p.tbExamCourse.tbExamLevelGroup.Id,
                                                 p.tbExamCourse.FullTotalMark,
                                                 ExamLevelId = p.tbExamLevel != null ? p.tbExamLevel.Id : 0,
                                             }).ToList();

                var ntSubjectExamMarkList = (from p in examNtSubjectMarkList
                                             join t in vm.StudentList
                                             on p.StudentId equals t.StudentId
                                             select new
                                             {
                                                 ExamId = p.ExamId,
                                                 StudentId = p.StudentId,
                                                 SubjectId = p.SubjectId,
                                                 TotalMark = p.TotalMark,
                                                 ClassId = t.ClassId,
                                                 ClassName = t.ClassName,
                                                 LevelGroupId = p.LevelGroupId,
                                                 FullTotalMark = p.FullTotalMark,
                                                 ExamLevelId = p.ExamLevelId,
                                             }).ToList();



                foreach (var selectSubject in vm.selectSubjectList)
                {
                    foreach (var a in vm.ClassOrgStudentList)
                    {
                        var studentTotalList = ntSubjectExamMarkList.Where(d => d.ClassId == a.ClassId && d.SubjectId.ToString() == selectSubject.Value).ToList();
                        foreach (var b in vm.SubjectTotalLevelList.Where(d => d.SubjectId == selectSubject.Value.ConvertToInt()).Distinct().ToList())
                        {
                            var classStudentLevel = new Dto.ExamAnalyze.List();
                            classStudentLevel.ClassId = a.ClassId;
                            classStudentLevel.SubjectId = selectSubject.Value.ConvertToInt();
                            classStudentLevel.TotalLevelId = b.TotalLevelId;
                            //if (studentTotalList.Where(d => (b.TotalLavelMin * d.FullTotalMark / 100) <= d.TotalMark && d.TotalMark <= (b.TotalLavelMax * d.FullTotalMark / 100)).Count() > 0)
                            //{
                            //    classStudentLevel.TotalLevelCount = studentTotalList.Where(d => (b.TotalLavelMin * d.FullTotalMark / 100) <= d.TotalMark && d.TotalMark <= (b.TotalLavelMax * d.FullTotalMark / 100)).Count();
                            //}
                            //else
                            //{
                            //    classStudentLevel.TotalLevelCount = 0;
                            //}
                            if (studentTotalList.Where(d => d.ExamLevelId == b.TotalLevelId).ToList().Count() > 0)
                            {
                                classStudentLevel.TotalLevelCount = studentTotalList.Where(d => d.ExamLevelId == b.TotalLevelId).ToList().Count();
                            }
                            else
                            {
                                classStudentLevel.TotalLevelCount = 0;
                            }
                            vm.SNtClassStudentLevelList.Add(classStudentLevel);
                        }
                    }
                }
                #endregion
                #endregion
                #endregion

                #region 导出
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = hssfworkbook.CreateCellStyle();//设置垂直居中格式
                cellstyle.VerticalAlignment = VerticalAlignment.Center;//垂直居中
                cellstyle.Alignment = HorizontalAlignment.Center;//居中
                cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("等级分析") as HSSFSheet;//建立Sheet1

                int rowindex = 0;
                int cellindex = 0;
                //IRow row = sheet1.CreateRow(rowindex);
                //ICell cell = row.CreateCell(cellindex);
                //cell.SetCellValue("等级分析");
                //cell.CellStyle = cellstyle;
                //sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowindex, rowindex, cellindex, 3 + vm.TotalLevelList.Where(d => d.IsTotal == true).ToList().Count() * 2));
                //rowindex++;

                #region 总分
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
                cell.SetCellValue(year.ExamName + "等级情况分析-总分");
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = null;
                if (vm.TotalLevelList.Where(d => d.IsTotal == true).ToList().Count() > 0)
                {
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 5 + vm.TotalLevelList.Where(d => d.IsTotal == true).ToList().Count() * 2);
                }
                else
                {
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 3 + vm.TotalLevelList.Where(d => d.IsTotal == true).ToList().Count() * 2);
                }
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                rowindex++;

                #region 总分表头
                cellindex = 0;
                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("基本情况");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 3);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                if (vm.LsTotalLevelList.Count() > 0)
                {
                    cellindex = cellindex + 4;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("本次考试");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + vm.LsTotalLevelList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + vm.LsTotalLevelList.Count;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("近两次考试各等级人数增减情况");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + vm.LsTotalLevelList.Count() + 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                }
                rowindex++;

                cellindex = 0;
                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("学科");
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("教师");
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("人数");
                cell.CellStyle = cellstyle;
                cellindex++;

                var atitle = "";
                var ctitle = "";
                var i = 0;
                foreach (var b in vm.LsTotalLevelList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(b.ExamLevelName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                }
                if (vm.TotalLevelList.Count() > 0)
                {
                    foreach (var b in vm.NtTotalLevelList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(b.ExamLevelName);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                    }
                    foreach (var b in vm.NtTotalLevelList)
                    {
                        if (i == 0)
                        {
                            atitle += b.ExamLevelName + "与";
                        }
                        else if (i == 1)
                        {
                            atitle += b.ExamLevelName;
                        }
                        else if (i == vm.NtTotalLevelList.Count - 2)
                        {
                            ctitle += b.ExamLevelName + "与";
                        }
                        else if (i == vm.NtTotalLevelList.Count - 1)
                        {
                            ctitle += b.ExamLevelName;
                        }
                        i++;
                    }
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(atitle);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(ctitle);
                    cell.CellStyle = cellstyle;
                    setBorder(cellRangeAddress, sheet1, hssfworkbook);
                }
                rowindex++;
                #endregion

                var classStudentTotal = 0;
                var classIdList = vm.ClassStudentList.Select(d => d.ClassId).ToList();
                var twoResult = 0;
                var ftwoResult = 0;
                foreach (var a in vm.ClassStudentList)
                {
                    twoResult = 0;
                    ftwoResult = 0;
                    i = 0;
                    cellindex = 0;
                    classStudentTotal += vm.StudentList.Where(d => d.ClassId == a.ClassId).Distinct().Count();

                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("总分");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(a.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(a.TeacherName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(vm.StudentList.Where(d => d.ClassId == a.ClassId).Select(d => d.StudentId).Distinct().Count());
                    cell.CellStyle = cellstyle;
                    cellindex++;

                    foreach (var b in vm.LsTotalLevelList)
                    {
                        var classStudentLevel = vm.LsClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.TotalLevelId == b.Id).FirstOrDefault();
                        if (classStudentLevel != null)
                        {

                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(classStudentLevel.TotalLevelCount);
                            cell.CellStyle = cellstyle;
                        }
                        else
                        {
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(0);
                            cell.CellStyle = cellstyle;
                        }
                        cellindex++;
                    }
                    if (vm.TotalLevelList.Count() > 0)
                    {
                        foreach (var b in vm.NtTotalLevelList)
                        {
                            var lsTotalLevelCount = vm.LsClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.TotalLevelId == b.Id).FirstOrDefault() != null ? vm.LsClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.TotalLevelId == b.Id).FirstOrDefault().TotalLevelCount : 0;
                            var ntTotalLevelCount = vm.NtClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.TotalLevelId == b.Id).FirstOrDefault() != null ? vm.NtClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.TotalLevelId == b.Id).FirstOrDefault().TotalLevelCount : 0;
                            var count = lsTotalLevelCount - ntTotalLevelCount;
                            if (i == 0 || i == 1)
                            {
                                twoResult += count;
                            }
                            if (i == vm.NtTotalLevelList.Count - 2 || i == vm.NtTotalLevelList.Count - 1)
                            {
                                ftwoResult += count;
                            }
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(count);
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            i++;
                        }
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(twoResult);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(ftwoResult);
                        cell.CellStyle = cellstyle;
                    }
                    rowindex++;
                }

                cellindex = 0;
                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("年级");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 2);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex = 3;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue(classStudentTotal);
                cell.CellStyle = cellstyle;
                cellindex++;

                foreach (var b in vm.LsTotalLevelList)
                {
                    var classStudentLevelList = vm.LsClassStudentLevelList.Where(d => classIdList.Contains(d.ClassId) && d.TotalLevelId == b.Id).ToList();
                    var levelTotal = 0;
                    foreach (var classStudentLevel in classStudentLevelList)
                    {
                        levelTotal += classStudentLevel.TotalLevelCount;
                    }
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(levelTotal);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                }
                i = 0;
                twoResult = 0;
                ftwoResult = 0;
                if (vm.TotalLevelList.Count() > 0)
                {
                    foreach (var b in vm.NtTotalLevelList)
                    {
                        var lsTotalLevelCount = vm.LsClassStudentLevelList.Where(d => classIdList.Contains(d.ClassId) && d.TotalLevelId == b.Id).Sum(d => d.TotalLevelCount);
                        var ntTotalLevelCount = vm.NtClassStudentLevelList.Where(d => classIdList.Contains(d.ClassId) && d.TotalLevelId == b.Id).Sum(d => d.TotalLevelCount);
                        var count = lsTotalLevelCount - ntTotalLevelCount;
                        if (i == 0 || i == 1)
                        {
                            twoResult += count;
                        }
                        if (i == vm.NtTotalLevelList.Count - 2 || i == vm.NtTotalLevelList.Count - 1)
                        {
                            ftwoResult += count;
                        }
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(count);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        i++;
                    }
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(twoResult);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(ftwoResult);
                    cell.CellStyle = cellstyle;
                }
                rowindex++;
                #endregion

                rowindex++;
                foreach (var s in vm.selectSubjectList)
                {

                    #region 科目表头
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(year.ExamName + "等级情况分析-" + s.Text);
                    cell.CellStyle = cellstyle;
                    if (vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() > 0)
                    {
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 5 + vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() * 2);
                    }
                    else
                    {
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 3 + vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() * 2);
                    }
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    rowindex++;

                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("基本情况");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 3);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    if (vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() > 0)
                    {
                        cellindex = cellindex + 4;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("本次考试");
                        cell.CellStyle = cellstyle;
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() - 1);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                        cellindex = cellindex + vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count();
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("近两次考试各等级人数增减情况");
                        cell.CellStyle = cellstyle;
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() + 1);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                    }
                    rowindex++;

                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("学科");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("班级");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("教师");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("人数");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    foreach (var b in vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList())
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(b.TotalLavelName);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                    }
                    foreach (var b in vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList())
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(b.TotalLavelName);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                    }
                    if (vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().Count() > 0)
                    {
                        atitle = "";
                        ctitle = "";
                        i = 0;
                        foreach (var b in vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList())
                        {
                            if (i == 0)
                            {
                                atitle += b.TotalLavelName + "与";
                            }
                            else if (i == 1)
                            {
                                atitle += b.TotalLavelName;
                            }
                            else if (i == vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() - 2)
                            {
                                ctitle += b.TotalLavelName + "与";
                            }
                            else if (i == vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() - 1)
                            {
                                ctitle += b.TotalLavelName;
                            }
                            i++;
                        }
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(atitle);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(ctitle);
                        cell.CellStyle = cellstyle;
                        setBorder(cellRangeAddress, sheet1, hssfworkbook);
                    }
                    rowindex++;
                    #endregion

                    classStudentTotal = 0;
                    foreach (var a in vm.ClassOrgStudentList)
                    {
                        var teacherName = string.Join(",", orgTeacherList.Where(d => d.ClassId == a.ClassId && d.SubjectId == s.Value.ConvertToInt()).Select(d => d.TeacherName).Distinct().ToArray());
                        classStudentTotal += vm.StudentList.Where(d => d.ClassId == a.ClassId).Distinct().Count();
                        cellindex = 0;
                        row = sheet1.CreateRow(rowindex);
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(s.Text);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(a.ClassName);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(teacherName);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(vm.StudentList.Where(d => d.ClassId == a.ClassId).Distinct().Count());
                        cell.CellStyle = cellstyle;
                        cellindex++;

                        foreach (var b in vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList())
                        {
                            var classStudentLevel = vm.SClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.TotalLevelId == b.TotalLevelId && d.SubjectId == s.Value.ConvertToInt()).FirstOrDefault();
                            if (classStudentLevel != null)
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(classStudentLevel.TotalLevelCount);
                                cell.CellStyle = cellstyle;
                            }
                            else
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(0);
                                cell.CellStyle = cellstyle;
                            }
                            cellindex++;
                        }
                        i = 0;
                        twoResult = 0;
                        ftwoResult = 0;
                        if (vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().Count() > 0)
                        {
                            foreach (var b in vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList())
                            {
                                var lsTotalLevelCount = vm.SClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.TotalLevelId == b.TotalLevelId && d.SubjectId == s.Value.ConvertToInt()).FirstOrDefault() != null ? vm.SClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.TotalLevelId == b.TotalLevelId && d.SubjectId == s.Value.ConvertToInt()).FirstOrDefault().TotalLevelCount : 0;
                                var ntTotalLevelCount = vm.SNtClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.TotalLevelId == b.TotalLevelId && d.SubjectId == s.Value.ConvertToInt()).FirstOrDefault() != null ? vm.SNtClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.TotalLevelId == b.TotalLevelId && d.SubjectId == s.Value.ConvertToInt()).FirstOrDefault().TotalLevelCount : 0;
                                var count = lsTotalLevelCount - ntTotalLevelCount;
                                if (i == 0 || i == 1)
                                {
                                    twoResult += count;
                                }
                                else if (i == vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() - 2 || i == vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() - 1)
                                {
                                    ftwoResult += count;
                                }
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(count);
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                i++;
                            }
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(twoResult);
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(ftwoResult);
                            cell.CellStyle = cellstyle;
                        }
                        rowindex++;
                    }
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("年级");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 2);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = 3;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(classStudentTotal);
                    cell.CellStyle = cellstyle;
                    cellindex++;

                    i = 0;
                    twoResult = 0;
                    ftwoResult = 0;
                    foreach (var b in vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList())
                    {
                        var classStudentLevelList = vm.SClassStudentLevelList.Where(d => classIdList.Contains(d.ClassId) && d.TotalLevelId == b.TotalLevelId && d.SubjectId == s.Value.ConvertToInt()).ToList();
                        var levelTotal = 0;
                        foreach (var classStudentLevel in classStudentLevelList)
                        {
                            levelTotal += classStudentLevel.TotalLevelCount;
                        }
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(levelTotal);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                    }
                    if (vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().Count() > 0)
                    {
                        foreach (var b in vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList())
                        {
                            var lsTotalLevelCount = vm.SClassStudentLevelList.Where(d => classIdList.Contains(d.ClassId) && d.TotalLevelId == b.TotalLevelId && d.SubjectId == s.Value.ConvertToInt()).Sum(d => d.TotalLevelCount);
                            var ntTotalLevelCount = vm.SNtClassStudentLevelList.Where(d => classIdList.Contains(d.ClassId) && d.TotalLevelId == b.TotalLevelId && d.SubjectId == s.Value.ConvertToInt()).Sum(d => d.TotalLevelCount);
                            var count = lsTotalLevelCount - ntTotalLevelCount;
                            if (i == 0 || i == 1)
                            {
                                twoResult += count;
                            }
                            else if (i == vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() - 2 || i == vm.SubjectTotalLevelList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Distinct().ToList().Count() - 1)
                            {
                                ftwoResult += count;
                            }
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(count);
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            i++;
                        }
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(twoResult);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(ftwoResult);
                        cell.CellStyle = cellstyle;
                    }
                    rowindex++;
                    rowindex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("分数段报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }

        public ActionResult ExportAllStudent(int examId, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();
                #region 数据统计

                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == gradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && courseIdList.Contains(p.tbCourse.Id)
                                       orderby p.tbCourse.tbSubject.No
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id,
                                           p.FullTotalMark
                                       }).Distinct().ToList();

                var SubjectList = (from p in examSubjectList
                                   select new System.Web.Mvc.SelectListItem
                                   {
                                       Text = p.SubjectName,
                                       Value = p.SubjectId.ToString()
                                   }).ToList();

                SubjectList.Add(new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });

                //获取分数等级
                var TotalLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                     .Include(d => d.tbExamLevelGroup)
                                      where p.IsDeleted == false && p.IsTotal == true
                                      orderby p.No
                                      select p).ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where ((p.tbStudent.StudentCode.Contains(searchText) || p.tbStudent.StudentName.Contains(searchText)) || searchText == null)
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbExam.Id == examId
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
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullTotalMark
                          }).ToList();
                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == examId
                            && p.tbYear.IsDeleted == false
                            select new
                            { p.tbYear.Id, p.ExamName, YearId = p.tbYear.tbYearParent.tbYearParent.Id }).FirstOrDefault();

                if (year == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                //学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                    && p.tbClass.tbGrade.Id == gradeId
                                    && ((p.tbStudent.StudentCode.Contains(searchText) || p.tbStudent.StudentName.Contains(searchText)) || searchText == null)
                                    select new Dto.ExamReport.List
                                    {
                                        ClassId = p.tbClass.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName
                                    }).ToList();

                var classIdList = classStudent.Select(d => d.ClassId).Distinct().ToList();

                var ClassStudentList = classStudent;

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
                              TotalMark = p.TotalMark,
                              FullTotalMark = p.FullTotalMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              ExamLevelName = p.ExamLevelName,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName
                          }).ToList();

                //var totalFullMark = examSubjectList.Sum(d => d.FullTotalMark);

                var studentMarkList = (from p in tb
                                       group p by new
                                       {
                                           p.StudentId,
                                           StudentCode = p.StudentCode,
                                           StudentName = p.StudentName,
                                           p.ClassId
                                       } into g
                                       select new Dto.ExamReport.List
                                       {
                                           ClassId = g.Key.ClassId,
                                           StudentId = g.Key.StudentId,
                                           StudentCode = g.Key.StudentCode,
                                           StudentName = g.Key.StudentName,
                                           TotalMark = g.Sum(d => d.TotalMark),
                                           TotalClassRank = 0,
                                           TotalGradeRank = 0,
                                           SubjectId = 0,
                                           ExamLevelName = string.Empty,
                                           ClassName = string.Empty
                                       }).ToList();

                var lst = studentMarkList;

                //年级排名和等级
                var rank = 0;
                decimal? mark = null;
                var count = 1;
                foreach (var t in lst.OrderByDescending(d => d.TotalMark))
                {
                    var tk = (from p in TotalLevelList
                              where p.MaxScore >= t.TotalMark && p.MinScore <= t.TotalMark
                              select p).FirstOrDefault();
                    if (tk != null)
                    {
                        t.ExamLevelName = tk.ExamLevelName;
                    }
                    //年级排名
                    if (mark != t.TotalMark)
                    {
                        mark = t.TotalMark;
                        rank = rank + count;
                        count = 1;
                    }
                    else
                    {
                        count = count + 1;
                    }
                    var tt = (from p in lst
                              where p.StudentId == t.StudentId
                              select p).FirstOrDefault();
                    if (tt != null)
                    {
                        tt.TotalGradeRank = rank;
                    }
                }

                rank = 0;
                mark = null;
                count = 1;
                //班级排名
                foreach (var classId in classIdList)
                {
                    foreach (var t in lst.Where(d => d.ClassId == classId).OrderByDescending(d => d.TotalMark))
                    {
                        if (mark != t.TotalMark)
                        {
                            mark = t.TotalMark;
                            rank = rank + count;
                            count = 1;
                        }
                        else
                        {
                            count = count + 1;
                        }
                        var tt = (from p in lst.Where(d => d.ClassId == classId)
                                  where p.StudentId == t.StudentId
                                  select p).FirstOrDefault();
                        if (tt != null)
                        {
                            tt.TotalClassRank = rank;
                        }
                    }
                }

                var ExamTotalMarkList = lst;

                var tm = (from p in tb
                          select new Dto.ExamReport.List
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              StudentCode = p.StudentCode,
                              StudentName = p.StudentName,
                              TotalMark = p.TotalMark,
                              TotalClassRank = p.TotalClassRank,
                              TotalGradeRank = p.TotalGradeRank,
                              ExamLevelName = p.ExamLevelName,
                              ClassName = p.ClassName
                          }).ToList();

                var ExamMarkList = (from p in tm.Union(lst)
                                    select new Dto.ExamReport.List
                                    {
                                        StudentId = p.StudentId,
                                        SubjectId = p.SubjectId,
                                        StudentCode = p.StudentCode,
                                        StudentName = p.StudentName,
                                        TotalMark = p.TotalMark,
                                        TotalClassRank = p.TotalClassRank,
                                        TotalGradeRank = p.TotalGradeRank,
                                        ExamLevelName = p.ExamLevelName,
                                        ClassName = p.ClassName
                                    }).ToList();
                #endregion
                #region 导出
                //分数项目
                var OptionList = new List<string>() { "考试成绩 ", "班名", "级名", "等级" };
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = hssfworkbook.CreateCellStyle();//设置垂直居中格式
                cellstyle.VerticalAlignment = VerticalAlignment.Center;//垂直居中
                cellstyle.Alignment = HorizontalAlignment.Center;//居中
                cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                //缩小字体填充  
                cellstyle.ShrinkToFit = false;

                var sheetName = year.ExamName + "班级成绩册";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;

                IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                //表头
                ICell cell = cellHeader.CreateCell(0);
                cell.SetCellValue(sheetName);
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, 0, SubjectList.Count() * 4 + 2);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);

                //第二行
                rowStartIndex++;
                cellHeader = sheet1.CreateRow(rowStartIndex);
                cell = cellHeader.CreateCell(0);
                sheet1.SetColumnWidth(0, 15 * 256);
                cell.SetCellValue("学生学号");
                cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 0, 0);
                sheet1.AddMergedRegion(cellRangeAddress);
                cell.CellStyle = cellstyle;

                cell = cellHeader.CreateCell(1);
                sheet1.SetColumnWidth(1, 15 * 256);
                cell.SetCellValue("学生姓名");
                cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 1, 1);
                sheet1.AddMergedRegion(cellRangeAddress);
                cell.CellStyle = cellstyle;

                cell = cellHeader.CreateCell(2);
                sheet1.SetColumnWidth(2, 15 * 256);
                cell.SetCellValue("班级");
                cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 2, 2);
                sheet1.AddMergedRegion(cellRangeAddress);
                cell.CellStyle = cellstyle;

                var No = 0;
                foreach (var subject in SubjectList)
                {
                    cell = cellHeader.CreateCell(No + 3);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(No + 3, 15 * 256);
                    cell.SetCellValue(subject.Text);
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, No + 3, No + OptionList.Count() + 2);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    No += OptionList.Count();
                }
                //第三行
                No = 0;
                cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                foreach (var subject in SubjectList)
                {
                    for (var i = 0; i < OptionList.Count(); i++)
                    {
                        cell = cellHeader.CreateCell(No + 3);
                        cell.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(No + 3, 15 * 256);
                        cell.SetCellValue(OptionList[i]);
                        No++;
                    }
                }

                cellHeader.CreateCell(0).SetCellValue(string.Empty);
                cellHeader.GetCell(0).CellStyle = cellstyle;

                cellHeader.CreateCell(1).SetCellValue(string.Empty);
                cellHeader.GetCell(1).CellStyle = cellstyle;

                cellHeader.CreateCell(2).SetCellValue(string.Empty);
                cellHeader.GetCell(2).CellStyle = cellstyle;

                //数据行
                rowStartIndex++;
                foreach (var s in ClassStudentList)
                {
                    cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                    cell = cellHeader.CreateCell(0);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(s.StudentCode);

                    cell = cellHeader.CreateCell(1);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(1, 15 * 256);
                    cell.SetCellValue(s.StudentName);

                    cell = cellHeader.CreateCell(2);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(2, 15 * 256);
                    cell.SetCellValue(s.ClassName);

                    #region  //数据行
                    No = 0;
                    foreach (var subject in SubjectList)
                    {
                        var examMark = ExamMarkList.Where(d => d.SubjectId == subject.Value.ConvertToInt() && d.StudentId == s.StudentId).Select(d => d).FirstOrDefault();
                        if (examMark != null)
                        {
                            for (var i = 0; i < OptionList.Count(); i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        cell = cellHeader.CreateCell(No + 3);
                                        cell.CellStyle = cellstyle;
                                        cell.SetCellValue(examMark.TotalMark.ToString());
                                        No++;
                                        break;
                                    case 1:
                                        cell = cellHeader.CreateCell(No + 3);
                                        cell.CellStyle = cellstyle;
                                        cell.SetCellValue(examMark.TotalClassRank.ToString());
                                        No++;
                                        break;
                                    case 2:
                                        cell = cellHeader.CreateCell(No + 3);
                                        cell.CellStyle = cellstyle;
                                        cell.SetCellValue(examMark.TotalGradeRank.ToString());
                                        No++;
                                        break;
                                    case 3:
                                        cell = cellHeader.CreateCell(No + 3);
                                        cell.CellStyle = cellstyle;
                                        cell.SetCellValue(examMark.ExamLevelName);
                                        No++;
                                        break;
                                }

                            }
                        }
                        else
                        {
                            cell = cellHeader.CreateCell(No + 3);
                            cell.CellStyle = cellstyle;
                            cell.SetCellValue(string.Empty);
                            No++;
                            cell = cellHeader.CreateCell(No + 3);
                            cell.CellStyle = cellstyle;
                            cell.SetCellValue(string.Empty);
                            No++;
                            cell = cellHeader.CreateCell(No + 3);
                            cell.CellStyle = cellstyle;
                            cell.SetCellValue(string.Empty);
                            No++;
                            cell = cellHeader.CreateCell(No + 3);
                            cell.CellStyle = cellstyle;
                            cell.SetCellValue(string.Empty);
                            No++;
                        }

                    }
                    #endregion

                    rowStartIndex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("班级成绩册报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportAdvance(int examId, int lastexamId, int gradeId, string chkSubject, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamAnalyze.List();

                if (string.IsNullOrEmpty(chkSubject)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chksubjectList = vm.chkSubject.Split(',');

                //选中的科目
                var selectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                         where chksubjectList.Contains(p.Id.ToString())
                                         orderby p.No, p.SubjectName
                                         select new
                                         {
                                             p.Id,
                                             p.SubjectName,
                                         }).Distinct().ToList();

                vm.selectSubjectList = (from p in selectSubjectList
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.SubjectName,
                                        }).Distinct().ToList();

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == examId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                TopYearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();
                if (year == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var syear = (from p in db.Table<Exam.Entity.tbExam>()
                             where p.Id == lastexamId
                             && p.tbYear.IsDeleted == false
                             select new
                             {
                                 p.tbYear.Id,
                                 TopYearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                 p.ExamName
                             }).FirstOrDefault();

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == gradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.TopYearId
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                    }).ToList();

                vm.ClassStudentList = (from p in classStudent
                                       select new Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           ClassName = p.ClassName,
                                           StudentId = p.StudentId,
                                           StudentCode = p.StudentCode,
                                           StudentName = p.StudentName,
                                       }).Distinct().ToList();

                #region 本次考试
                var lsExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                      where p.tbExamCourse.IsDeleted == false
                                       && p.tbStudent.IsDeleted == false
                                       && p.tbExamCourse.tbExam.Id == examId
                                       && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                      select new
                                      {
                                          p.TotalMark,
                                          ExamId = p.tbExamCourse.tbExam.Id,
                                          StudentId = p.tbStudent.Id,
                                          SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                          p.tbExamCourse.FullTotalMark,
                                          p.TotalGradeRank,
                                      }).ToList();

                vm.SClassStudentLevelList = (from p in lsExamMarkList
                                             join t in classStudent
                                             on p.StudentId equals t.StudentId
                                             select new Dto.ExamAnalyze.List
                                             {
                                                 ExamId = p.ExamId,
                                                 StudentId = p.StudentId,
                                                 SubjectId = p.SubjectId,
                                                 TotalMark = p.TotalMark,
                                                 ClassId = t.ClassId,
                                                 ClassName = t.ClassName,
                                                 GradeRank = p.TotalGradeRank,
                                             }).ToList();

                //获取班级学生总分
                var examLsStudentTotalList = (from p in vm.SClassStudentLevelList
                                              group p by new { p.ClassId, p.StudentId } into g
                                              select new Dto.ExamAnalyze.List
                                              {
                                                  ClassId = g.Key.ClassId,
                                                  StudentId = g.Key.StudentId,
                                                  TotalMark = g.Sum(d => d.TotalMark),
                                              }).ToList();

                var gradeRank = decimal.Zero;
                decimal? gradeMark = null;
                var gradeCount = decimal.One;
                foreach (var t in examLsStudentTotalList.OrderByDescending(d => d.TotalMark))
                {
                    if (gradeMark != t.TotalMark)
                    {
                        gradeMark = t.TotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.TotalGradeRank = gradeRank;
                }
                vm.ExamLsStudentList = (from p in examLsStudentTotalList
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = p.ClassId,
                                            StudentId = p.StudentId,
                                            TotalMark = p.TotalMark,
                                            TotalGradeRank = p.TotalGradeRank,
                                        }).ToList();
                #endregion

                #region 上次考试
                var ntExamMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                      where p.tbExamCourse.IsDeleted == false
                                       && p.tbStudent.IsDeleted == false
                                       && p.tbExamCourse.tbExam.Id == lastexamId
                                       && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                      select new
                                      {
                                          p.TotalMark,
                                          ExamId = p.tbExamCourse.tbExam.Id,
                                          StudentId = p.tbStudent.Id,
                                          SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                          p.tbExamCourse.FullTotalMark,
                                          p.TotalGradeRank,
                                      }).ToList();

                vm.SNtClassStudentLevelList = (from p in ntExamMarkList
                                               join t in classStudent
                                               on p.StudentId equals t.StudentId
                                               select new Dto.ExamAnalyze.List
                                               {
                                                   ExamId = p.ExamId,
                                                   StudentId = p.StudentId,
                                                   SubjectId = p.SubjectId,
                                                   TotalMark = p.TotalMark,
                                                   ClassId = t.ClassId,
                                                   ClassName = t.ClassName,
                                                   GradeRank = p.TotalGradeRank,
                                               }).ToList();

                //获取班级学生总分
                var examNtStudentTotalList = (from p in vm.SNtClassStudentLevelList
                                              group p by new { p.ClassId, p.StudentId } into g
                                              select new Dto.ExamAnalyze.List
                                              {
                                                  ClassId = g.Key.ClassId,
                                                  StudentId = g.Key.StudentId,
                                                  TotalMark = g.Sum(d => d.TotalMark),
                                              }).ToList();
                gradeRank = decimal.Zero;
                gradeMark = null;
                gradeCount = decimal.One;
                foreach (var t in examNtStudentTotalList.OrderByDescending(d => d.TotalMark))
                {
                    if (gradeMark != t.TotalMark)
                    {
                        gradeMark = t.TotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.TotalGradeRank = gradeRank;
                }

                //获取班级学生总分
                vm.ExamNtStudentList = (from p in examNtStudentTotalList
                                        select new Dto.ExamAnalyze.List
                                        {
                                            ClassId = p.ClassId,
                                            StudentId = p.StudentId,
                                            TotalMark = p.TotalMark,
                                            TotalGradeRank = p.TotalGradeRank,
                                        }).ToList();
                #endregion
                #endregion

                #region 导出
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = hssfworkbook.CreateCellStyle();//设置垂直居中格式
                cellstyle.VerticalAlignment = VerticalAlignment.Center;//垂直居中
                cellstyle.Alignment = HorizontalAlignment.Center;//居中
                cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("与上次学生进退步") as HSSFSheet;//建立Sheet1

                #region 表头
                var rowindex = 0;
                var cellindex = 0;
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
                cell.SetCellValue("学生本次考试与上次考试名次进退情况");
                sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowindex, rowindex, cellindex, 4));
                rowindex++;
                rowindex++;

                cellindex = 0;
                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("上次考试:" + syear.ExamName);
                sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowindex, rowindex, cellindex, 8));
                rowindex++;
                rowindex++;

                cellindex = 0;
                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("本次考试:" + year.ExamName);
                sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowindex, rowindex, cellindex, 8));
                rowindex++;
                rowindex++;

                cellindex = 0;
                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("学号");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("姓名");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("总分");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 5);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex = cellindex + 6;

                foreach (var s in vm.selectSubjectList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(s.Text);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 5);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 6;
                }
                rowindex++;

                cellindex = 3;
                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("成绩");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex = cellindex + 3;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("名次");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex = cellindex + 3;

                foreach (var s in vm.selectSubjectList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("成绩");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 3;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("名次");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 3;
                }
                rowindex++;

                cellindex = 3;
                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("上次");
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("本次");
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("进退");
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("上次");
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("本次");
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("进退");
                cell.CellStyle = cellstyle;
                cellindex++;

                foreach (var s in vm.selectSubjectList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("上次");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("本次");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("进退");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("上次");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("本次");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("进退");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                for (var i = 7; i < 9; i++)
                {

                    sheet1.GetRow(i).CreateCell(0).SetCellValue(string.Empty);
                    sheet1.GetRow(i).GetCell(0).CellStyle = cellstyle;
                    sheet1.GetRow(i).CreateCell(1).SetCellValue(string.Empty);
                    sheet1.GetRow(i).GetCell(1).CellStyle = cellstyle;
                    sheet1.GetRow(i).CreateCell(2).SetCellValue(string.Empty);
                    sheet1.GetRow(i).GetCell(2).CellStyle = cellstyle;
                }
                rowindex++;
                #endregion


                foreach (var a in vm.ClassStudentList)
                {
                    cellindex = 0;
                    var ntModel = vm.ExamNtStudentList.Where(d => d.ClassId == a.ClassId && d.StudentId == a.StudentId).FirstOrDefault();
                    var lsModel = vm.ExamLsStudentList.Where(d => d.ClassId == a.ClassId && d.StudentId == a.StudentId).FirstOrDefault();

                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(a.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(a.StudentCode);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(a.StudentName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(ntModel != null ? ntModel.TotalMark.ToString() : "0");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(lsModel != null ? lsModel.TotalMark.ToString() : "0");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(lsModel != null ? (ntModel != null ? (lsModel.TotalMark - ntModel.TotalMark).ToString() : lsModel.TotalMark.ToString()) : (ntModel != null ? (0 - ntModel.TotalMark).ToString() : "0"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(ntModel != null ? ntModel.TotalGradeRank.ToString() : "0");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(lsModel != null ? lsModel.TotalGradeRank.ToString() : "0");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(ntModel != null ? (lsModel != null ? (ntModel.TotalGradeRank - lsModel.TotalGradeRank).ToString() : ntModel.TotalGradeRank.ToString()) : (lsModel != null ? (0 - lsModel.TotalGradeRank).ToString() : "0"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    foreach (var s in vm.selectSubjectList)
                    {
                        var ntSubjectModel = vm.SNtClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.StudentId == a.StudentId && d.SubjectId == s.Value.ConvertToInt()).FirstOrDefault();
                        var lsSubjectModel = vm.SClassStudentLevelList.Where(d => d.ClassId == a.ClassId && d.StudentId == a.StudentId && d.SubjectId == s.Value.ConvertToInt()).FirstOrDefault();

                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(ntSubjectModel != null ? ntSubjectModel.TotalMark.ToString() : "0");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(lsSubjectModel != null ? lsSubjectModel.TotalMark.ToString() : "0");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(lsSubjectModel != null ? (ntSubjectModel != null ? (lsSubjectModel.TotalMark - ntSubjectModel.TotalMark).ToString() : lsSubjectModel.TotalMark.ToString()) : (ntSubjectModel != null ? (0 - ntSubjectModel.TotalMark).ToString() : "0"));
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(ntSubjectModel != null ? ntSubjectModel.GradeRank.ToString() : "0");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(lsSubjectModel != null ? lsSubjectModel.GradeRank.ToString() : "0");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(ntSubjectModel != null ? (lsSubjectModel != null ? (ntSubjectModel.GradeRank - lsSubjectModel.GradeRank).ToString() : ntSubjectModel.GradeRank.ToString()) : (lsSubjectModel != null ? (0 - lsSubjectModel.GradeRank).ToString() : "0"));
                        cell.CellStyle = cellstyle;
                        cellindex++;
                    }
                    rowindex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("与上次考试学生进退步" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }

        public ActionResult ExportTopRank(int examId, int gradeId, string chkSubject, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamAnalyze.List();

                if (string.IsNullOrEmpty(chkSubject)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chksubjectList = vm.chkSubject.Split(',');

                //选中的科目
                var selectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                         where chksubjectList.Contains(p.Id.ToString())
                                         orderby p.No, p.SubjectName
                                         select new
                                         {
                                             p.Id,
                                             p.SubjectName,
                                         }).Distinct().ToList();

                vm.selectSubjectList = (from p in selectSubjectList
                                        select new System.Web.Mvc.SelectListItem
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.SubjectName,
                                        }).Distinct().ToList();

                //获取分数等级
                vm.TotalLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                     .Include(d => d.tbExamLevelGroup)
                                     where p.IsDeleted == false
                                     orderby p.No
                                     select p).ToList();

                //学年
                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == vm.ExamId
                            && p.tbYear.IsDeleted == false
                            select new
                            {
                                p.tbYear.Id,
                                YearId = p.tbYear.tbYearParent.tbYearParent.Id,
                                p.ExamName
                            }).FirstOrDefault();
                if (year == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == gradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year.YearId
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                vm.ClassStudentList = (from p in classStudent
                                       group p by new { p.ClassId, p.ClassName } into g
                                       select new Dto.ExamAnalyze.List
                                       {
                                           ClassId = g.Key.ClassId,
                                           ClassName = g.Key.ClassName,
                                           TotalCount = g.Where(d => d.ClassId == g.Key.ClassId).Count(),
                                       }).Distinct().ToList();

                #region 本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        p.TotalGradeRank,
                                    }).ToList();

                vm.StudentExamMarkList = (from p in examMarkList
                                          join t in classStudent
                                          on p.StudentId equals t.StudentId
                                          select new Dto.ExamAnalyze.List
                                          {
                                              ExamId = p.ExamId,
                                              StudentId = p.StudentId,
                                              SubjectId = p.SubjectId,
                                              TotalMark = p.TotalMark,
                                              ClassId = t.ClassId,
                                              ClassName = t.ClassName,
                                              GradeRank = p.TotalGradeRank,
                                          }).ToList();

                //获取班级学生总分
                vm.TopTotalExamMarkList = (from p in vm.StudentExamMarkList
                                           group p by new { p.ClassId, p.StudentId } into g
                                           select new Dto.ExamAnalyze.List
                                           {
                                               ClassId = g.Key.ClassId,
                                               StudentId = g.Key.StudentId,
                                               TotalMark = g.Sum(d => d.TotalMark),
                                               TotalGradeRank = 0,
                                           }).ToList();

                vm.DownTotalExamMarkList = (from p in vm.StudentExamMarkList
                                            group p by new { p.ClassId, p.StudentId } into g
                                            select new Dto.ExamAnalyze.List
                                            {
                                                ClassId = g.Key.ClassId,
                                                StudentId = g.Key.StudentId,
                                                TotalMark = g.Sum(d => d.TotalMark),
                                                TotalGradeRank = 0,
                                            }).ToList();

                //按总分成绩从高到低
                var gradeRank = decimal.Zero;
                decimal? gradeMark = null;
                var gradeCount = decimal.One;
                foreach (var t in vm.TopTotalExamMarkList.OrderByDescending(d => d.TotalMark))
                {
                    if (gradeMark != t.TotalMark)
                    {
                        gradeMark = t.TotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.TotalGradeRank = gradeRank;
                }

                //按总分从低到高
                gradeRank = decimal.Zero;
                gradeMark = null;
                gradeCount = decimal.One;
                foreach (var t in vm.DownTotalExamMarkList.OrderBy(d => d.TotalMark))
                {
                    if (gradeMark != t.TotalMark)
                    {
                        gradeMark = t.TotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.TotalGradeRank = gradeRank;
                }
                #endregion
                #endregion

                #region 导出
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = hssfworkbook.CreateCellStyle();//设置垂直居中格式
                cellstyle.VerticalAlignment = VerticalAlignment.Center;//垂直居中
                cellstyle.Alignment = HorizontalAlignment.Center;//居中
                cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                cellstyle.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                cellstyle.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("各科前若干名统计") as HSSFSheet;//建立Sheet1

                #region 表头
                var rowindex = 0;
                var cellindex = 0;
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
                cell.SetCellValue(year.ExamName + "-总分前若干名统计表");
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 13);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                rowindex++;

                cellindex = 0;
                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("人数");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("前10");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex = cellindex + 1;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("前20");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex = cellindex + 1;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("前50");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex = cellindex + 1;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("前100");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex = cellindex + 1;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("前200");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex = cellindex + 1;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("后100");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                rowindex++;

                cellindex = 2;
                row = sheet1.CreateRow(rowindex);
                for (var i = 0; i < 6; i++)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("人数");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("比例");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex++;
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                sheet1.GetRow(2).CreateCell(0).SetCellValue(string.Empty);
                sheet1.GetRow(2).GetCell(0).CellStyle = cellstyle;
                sheet1.GetRow(2).CreateCell(1).SetCellValue(string.Empty);
                sheet1.GetRow(2).GetCell(1).CellStyle = cellstyle;
                rowindex++;

                foreach (var a in vm.ClassStudentList)
                {
                    var topExamMarkList = vm.TopTotalExamMarkList.Where(d => d.ClassId == a.ClassId).ToList();
                    var downExamMarkList = vm.DownTotalExamMarkList.Where(d => d.ClassId == a.ClassId).ToList();

                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(a.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(a.TotalCount);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(topExamMarkList.Where(d => d.TotalGradeRank <= 10).Count());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (topExamMarkList.Where(d => d.TotalGradeRank <= 10).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(topExamMarkList.Where(d => d.TotalGradeRank <= 20).Count());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (topExamMarkList.Where(d => d.TotalGradeRank <= 20).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(topExamMarkList.Where(d => d.TotalGradeRank <= 50).Count());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (topExamMarkList.Where(d => d.TotalGradeRank <= 50).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(topExamMarkList.Where(d => d.TotalGradeRank <= 100).Count());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (topExamMarkList.Where(d => d.TotalGradeRank <= 100).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(topExamMarkList.Where(d => d.TotalGradeRank <= 200).Count());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (topExamMarkList.Where(d => d.TotalGradeRank <= 200).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(downExamMarkList.Where(d => d.TotalGradeRank <= 100).Count());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (downExamMarkList.Where(d => d.TotalGradeRank <= 100).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                    cell.CellStyle = cellstyle;
                    rowindex++;
                }
                decimal studentCount = 0;
                decimal gradeRank10 = 0;
                decimal gradeRank20 = 0;
                decimal gradeRank50 = 0;
                decimal gradeRank100 = 0;
                decimal gradeRank200 = 0;
                decimal downGradeRank100 = 0;
                foreach (var a in vm.ClassStudentList)
                {
                    studentCount += a.TotalCount;
                    var topExamMarkList = vm.TopTotalExamMarkList.Where(d => d.ClassId == a.ClassId).ToList();
                    gradeRank10 += topExamMarkList.Where(d => d.TotalGradeRank <= 10).Count();
                    gradeRank20 += topExamMarkList.Where(d => d.TotalGradeRank <= 20).Count();
                    gradeRank50 += topExamMarkList.Where(d => d.TotalGradeRank <= 50).Count();
                    gradeRank100 += topExamMarkList.Where(d => d.TotalGradeRank <= 100).Count();
                    gradeRank200 += topExamMarkList.Where(d => d.TotalGradeRank <= 200).Count();
                    var downExamMarkList = vm.DownTotalExamMarkList.Where(d => d.ClassId == a.ClassId).ToList();
                    downGradeRank100 += downExamMarkList.Where(d => d.TotalGradeRank <= 100).Count();
                }

                cellindex = 0;
                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("年级");
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue(studentCount.ToString());
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue(gradeRank10.ToString());
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue((studentCount > 0 ? (gradeRank10.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue(gradeRank20.ToString());
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue((studentCount > 0 ? (gradeRank20.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue(gradeRank50.ToString());
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue((studentCount > 0 ? (gradeRank50.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue(gradeRank100.ToString());
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue((studentCount > 0 ? (gradeRank100.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue(gradeRank200.ToString());
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue((studentCount > 0 ? (gradeRank200.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue(downGradeRank100.ToString());
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue((studentCount > 0 ? (downGradeRank100.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                cell.CellStyle = cellstyle;
                rowindex++;
                rowindex++;

                var rowList = new List<int>();
                foreach (var s in vm.selectSubjectList)
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(year.ExamName + "-" + s.Text + "前若干名统计表");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 13);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    rowindex++;

                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("班级");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("人数");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    rowList.Add(rowindex + 1);
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("前10");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 1;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("前20");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 1;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("前50");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 1;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("前100");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 1;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("前200");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 1;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("后100");
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    rowindex++;

                    cellindex = 2;
                    row = sheet1.CreateRow(rowindex);
                    for (var i = 0; i < 6; i++)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("人数");
                        cell.CellStyle = cellstyle;
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("比例");
                        cell.CellStyle = cellstyle;
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                        cellindex++;
                    }
                    setBorder(cellRangeAddress, sheet1, hssfworkbook);
                    foreach (var r in rowList)
                    {
                        sheet1.GetRow(r).CreateCell(0).SetCellValue(string.Empty);
                        sheet1.GetRow(r).GetCell(0).CellStyle = cellstyle;
                        sheet1.GetRow(r).CreateCell(1).SetCellValue(string.Empty);
                        sheet1.GetRow(r).GetCell(1).CellStyle = cellstyle;
                    }
                    rowindex++;

                    foreach (var a in vm.ClassStudentList)
                    {
                        var topExamMarkList = vm.StudentExamMarkList.Where(d => d.ClassId == a.ClassId && d.SubjectId == s.Value.ConvertToInt()).OrderBy(d => d.GradeRank).ToList();
                        //后100名
                        var downExamMarkList = new List<XkSystem.Areas.Exam.Dto.ExamAnalyze.List>();
                        var downExamMark = vm.StudentExamMarkList.Where(d => d.SubjectId == s.Value.ConvertToInt()).OrderByDescending(d => d.GradeRank).FirstOrDefault();
                        if (downExamMark != null && downExamMark.GradeRank < 100)
                        {
                            downExamMarkList = vm.StudentExamMarkList.Where(d => d.ClassId == a.ClassId && d.SubjectId == s.Value.ConvertToInt()).OrderByDescending(d => d.GradeRank).ToList();
                        }
                        else if (downExamMark != null && downExamMark.GradeRank >= 100)
                        {
                            downExamMarkList = vm.StudentExamMarkList.Where(d => d.ClassId == a.ClassId && d.SubjectId == s.Value.ConvertToInt()).Where(d => d.GradeRank >= (downExamMark.GradeRank - 100)).ToList();
                        }

                        cellindex = 0;
                        row = sheet1.CreateRow(rowindex);
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(a.ClassName);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(a.TotalCount);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(topExamMarkList.Where(d => d.GradeRank <= 10).Count());
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (topExamMarkList.Where(d => d.GradeRank <= 10).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(topExamMarkList.Where(d => d.GradeRank <= 20).Count());
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (topExamMarkList.Where(d => d.GradeRank <= 20).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(topExamMarkList.Where(d => d.TotalGradeRank <= 50).Count());
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (topExamMarkList.Where(d => d.GradeRank <= 50).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(topExamMarkList.Where(d => d.TotalGradeRank <= 100).Count());
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (topExamMarkList.Where(d => d.GradeRank <= 100).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(topExamMarkList.Where(d => d.GradeRank <= 200).Count());
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (topExamMarkList.Where(d => d.GradeRank <= 200).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(downExamMarkList.Where(d => d.GradeRank <= 100).Count());
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue((a.TotalCount > 0 ? (a.TotalCount > 0 ? (downExamMarkList.Where(d => d.GradeRank <= 100).Count().ConvertToDecimal() / a.TotalCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%") : "0%"));
                        cell.CellStyle = cellstyle;
                        rowindex++;
                    }

                    studentCount = 0;
                    gradeRank10 = 0;
                    gradeRank20 = 0;
                    gradeRank50 = 0;
                    gradeRank100 = 0;
                    gradeRank200 = 0;
                    downGradeRank100 = 0;
                    foreach (var a in vm.ClassStudentList)
                    {
                        studentCount += a.TotalCount;
                        var topExamMarkList = vm.StudentExamMarkList.Where(d => d.ClassId == a.ClassId && d.SubjectId == s.Value.ConvertToInt()).OrderBy(d => d.GradeRank).ToList();

                        gradeRank10 += topExamMarkList.Where(d => d.GradeRank <= 10).Count();
                        gradeRank20 += topExamMarkList.Where(d => d.GradeRank <= 20).Count();
                        gradeRank50 += topExamMarkList.Where(d => d.GradeRank <= 50).Count();
                        gradeRank100 += topExamMarkList.Where(d => d.GradeRank <= 100).Count();
                        gradeRank200 += topExamMarkList.Where(d => d.GradeRank <= 200).Count();
                        //后100名
                        var downExamMarkList = new List<XkSystem.Areas.Exam.Dto.ExamAnalyze.List>();
                        var downExamMark = vm.StudentExamMarkList.Where(d => d.SubjectId == s.Value.ConvertToInt()).OrderByDescending(d => d.GradeRank).FirstOrDefault();
                        if (downExamMark != null && downExamMark.GradeRank < 100)
                        {
                            downExamMarkList = vm.StudentExamMarkList.Where(d => d.ClassId == a.ClassId && d.SubjectId == s.Value.ConvertToInt()).OrderByDescending(d => d.GradeRank).ToList();
                        }
                        else if (downExamMark != null && downExamMark.GradeRank >= 100)
                        {
                            downExamMarkList = vm.StudentExamMarkList.Where(d => d.ClassId == a.ClassId && d.SubjectId == s.Value.ConvertToInt()).Where(d => d.GradeRank >= (downExamMark.GradeRank - 100)).ToList();
                        }
                        downGradeRank100 += downExamMarkList.Count();
                    }

                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("年级");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(studentCount.ToString());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(gradeRank10.ToString());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((studentCount > 0 ? (gradeRank10.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(gradeRank20.ToString());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((studentCount > 0 ? (gradeRank20.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(gradeRank50.ToString());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((studentCount > 0 ? (gradeRank50.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(gradeRank100.ToString());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((studentCount > 0 ? (gradeRank100.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(gradeRank200.ToString());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((studentCount > 0 ? (gradeRank200.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(downGradeRank100.ToString());
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((studentCount > 0 ? (downGradeRank100.ConvertToDecimal() / studentCount.ConvertToDecimal() * 100).ToString("0.00") + "%" : "0%"));
                    cell.CellStyle = cellstyle;
                    rowindex++;
                    rowindex++;
                }
                #endregion

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("与上次考试学生进退步" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }

        public ActionResult ExportTotalMarkTopN(int examId, int gradeId, string chkSubject, string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == gradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                #region 数据统计
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && courseIdList.Contains(p.tbCourse.Id)
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

                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == examId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                //班级
                var tbClassStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                      where p.tbStudent.IsDeleted == false
                                      && p.tbClass.tbGrade.Id == gradeId
                                      && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                      orderby p.tbClass.No, p.tbClass.ClassName
                                      select new
                                      {
                                          CiassId = p.tbClass.Id,
                                          p.tbClass.ClassName
                                      }).Distinct().ToList(); ;

                var ClassList = (from p in tbClassStudent
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Value = p.CiassId.ToString(),
                                     Text = p.ClassName,
                                 }).Distinct().ToList();

                var rank = searchText.ConvertToDecimal();

                if (chkClass == null || chkSubject == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList = chkClass.Split(',');
                var chksubjectList = chkSubject.Split(',');

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == gradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && chkclassList.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

                //班级
                var tbselctClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                        where chkclassList.Contains(p.Id.ToString())
                                        orderby p.No
                                        select new
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.ClassName,
                                        }).ToList();

                var selctClassList = (from p in tbselctClassList
                                      select new System.Web.Mvc.SelectListItem
                                      {
                                          Value = p.Value,
                                          Text = p.Text,
                                      }).ToList();

                var tbselectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                           where chksubjectList.Contains(p.Id.ToString())
                                           orderby p.No, p.SubjectName
                                           select new
                                           {
                                               SubjectId = p.Id,
                                               p.SubjectName,
                                           }).Distinct().ToList();

                var selectSubjectList = (from p in tbselectSubjectList
                                         select new System.Web.Mvc.SelectListItem
                                         {
                                             Value = p.SubjectId.ToString(),
                                             Text = p.SubjectName,
                                         }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == examId || examId == 0)
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalClassRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              ClassRank = p.TotalClassRank,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in selectSubjectList)
                {
                    //班级人数
                    var subjectId = o.Value.ConvertToInt();
                    var totalStudetCount = (from p in tg
                                            where p.SubjectId == subjectId
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                TotalCount = g.Count()
                                            }).ToList();
                    //前N名人数
                    var tm = (from p in tg
                              where p.SubjectId == subjectId
                              && p.ClassRank > decimal.Zero && p.ClassRank <= rank
                              group p by new
                              {
                                  p.ClassId
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  StudentCount = g.Count(),
                                  TotalCount = totalStudetCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = subjectId,
                                  SubjectName = o.Text,
                                  StudentCount = p.StudentCount > decimal.Zero ? p.StudentCount.ToString() : string.Empty,
                                  Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentCount * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                              }).ToList();
                    lst.AddRange(tb);
                }

                #endregion

                #region 导出
                var OptionList = new List<string>() { "人数", "比率 " };
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = SetCellStyle(hssfworkbook);

                var sheetName = db.Table<Exam.Entity.tbExam>().Where(d => d.Id == examId).Select(d => d.ExamName).FirstOrDefault() + "考试成绩前" + searchText + "名";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                //表头第一行
                ICell cell = cellHeader.CreateCell(0);
                cell.SetCellValue("班级");
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 0, 0);
                sheet1.AddMergedRegion(cellRangeAddress);
                cell.CellStyle = cellstyle;

                var No = 0;
                foreach (var subject in selectSubjectList)
                {
                    //表头
                    cell = cellHeader.CreateCell(OptionList.Count() * No + 1);
                    cell.SetCellValue(subject.Text);
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count() * No + 1, OptionList.Count() * (No + 1));
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    No++;
                }

                //第二行
                cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                No = 0;
                foreach (var subject in selectSubjectList)
                {
                    for (var i = 0; i < OptionList.Count(); i++)
                    {
                        cell = cellHeader.CreateCell(No + 1);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(OptionList[i]);
                        No++;
                    }
                }
                //跨行边框无显示
                cellHeader.CreateCell(0).SetCellValue("班级");
                cellHeader.GetCell(0).CellStyle = cellstyle;

                rowStartIndex++;

                //数据行
                foreach (var t in selctClassList)
                {
                    cellHeader = sheet1.CreateRow(rowStartIndex + 1);

                    cell = cellHeader.CreateCell(0);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(t.Text);

                    No = 0;
                    foreach (var subject in selectSubjectList)
                    {
                        var mark = lst.Where(d => d.SubjectId == subject.Value.ConvertToInt() && d.ClassId == t.Value.ConvertToInt()
                                                                             ).Select(d => d).FirstOrDefault();
                        if (mark != null)
                        {
                            cell = cellHeader.CreateCell(No + 1);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(0, 15 * 256);
                            cell.SetCellValue(mark.StudentCount);
                            No++;
                            cell = cellHeader.CreateCell(No + 1);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(0, 15 * 256);
                            cell.SetCellValue(mark.Rate);

                            No++;
                        }
                        else
                        {
                            cell = cellHeader.CreateCell(No + 1);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(0, 15 * 256);
                            cell.SetCellValue(string.Empty);
                            No++;
                            cell = cellHeader.CreateCell(No + 1);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(0, 15 * 256);
                            cell.SetCellValue(string.Empty);
                            No++;
                        }
                    }

                    rowStartIndex++;
                }
                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("考试成绩前N报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public void setRegionStyle(HSSFSheet sheet, CellRangeAddress region, ICellStyle cs)
        {
            for (int i = region.FirstRow; i <= region.LastRow; i++)
            {
                IRow row = HSSFCellUtil.GetRow(i, sheet);
                for (int j = region.FirstColumn; j <= region.LastColumn; j++)
                {
                    ICell singleCell = HSSFCellUtil.GetCell(row, (short)j);
                    singleCell.CellStyle = cs;
                }
            }
        }
        public void setBorder(CellRangeAddress cellRangeAddress, HSSFSheet sheet, HSSFWorkbook wb)
        {
            RegionUtil.SetBorderLeft(1, cellRangeAddress, sheet, wb);
            RegionUtil.SetBorderBottom(1, cellRangeAddress, sheet, wb);
            RegionUtil.SetBorderRight(1, cellRangeAddress, sheet, wb);
            RegionUtil.SetBorderTop(1, cellRangeAddress, sheet, wb);
        }

        /// <summary>
        /// 单元格设置统一样式(如背景色等)
        /// </summary>
        /// <param name="workbook">工作表</param>
        /// <returns>返回新样式</returns>
        public ICellStyle SetCellStyle(HSSFWorkbook hssfworkbook)
        {
            ICellStyle cellstyle = hssfworkbook.CreateCellStyle();//设置垂直居中格式
            cellstyle.VerticalAlignment = VerticalAlignment.Center;//垂直居中
            cellstyle.Alignment = HorizontalAlignment.Center;//居中
            cellstyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellstyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellstyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            cellstyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cellstyle.WrapText = true;//自动换行
            //缩小字体填充  
            cellstyle.ShrinkToFit = false;
            return cellstyle;
        }

        /// <summary>
        /// 根据Excel列类型获取列的值
        /// </summary>
        /// <param name="cell">Excel列</param>
        /// <returns></returns>
        public string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                    if (DateUtil.IsValidExcelDate(cell.NumericCellValue))
                    {
                        return cell.DateCellValue.ToString();
                    }
                    else
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Unknown:
                default:
                    return cell.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }

        public void getSubjectClassList(int examId, int? gradeId, string searchText, out List<System.Web.Mvc.SelectListItem> classList, out List<System.Web.Mvc.SelectListItem> subjectList, out int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                //根据年级获取教学班课程
                var courseIdList = (from p in db.Table<Course.Entity.tbOrg>()
                                    where p.tbCourse.IsDeleted == false
                                    && p.tbGrade.IsDeleted == false
                                    && p.tbGrade.Id == gradeId
                                    select p.tbCourse.Id).Distinct().ToList();

                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && courseIdList.Contains(p.tbCourse.Id)
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(searchText) || searchText == null)
                                       orderby p.tbCourse.tbSubject.No
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id
                                       }).Distinct().ToList();

                subjectList = (from p in examSubjectList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.SubjectName,
                                   Value = p.SubjectId.ToString()
                               }).ToList();

                var year = (from p in db.Table<Exam.Entity.tbExam>()
                            where p.Id == examId
                            && p.tbYear.IsDeleted == false
                            select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();

                yearId = year;

                var tbClassStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                      where p.tbStudent.IsDeleted == false
                                      && p.tbClass.tbGrade.Id == gradeId
                                      && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == year
                                      orderby p.tbClass.No, p.tbClass.ClassName
                                      select new
                                      {
                                          CiassId = p.tbClass.Id,
                                          p.tbClass.ClassName
                                      }).Distinct().ToList(); ;

                classList = (from p in tbClassStudent
                             select new System.Web.Mvc.SelectListItem
                             {
                                 Value = p.CiassId.ToString(),
                                 Text = p.ClassName,
                             }).Distinct().ToList();

            }
        }

        public void getSelectSubjectClassList(string[] chkclassList, string[] chksubjectList, out List<System.Web.Mvc.SelectListItem> selctClassList, out List<System.Web.Mvc.SelectListItem> selectSubjectList)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var tbSelctClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                        where chkclassList.Contains(p.Id.ToString())
                                        orderby p.No
                                        select new
                                        {
                                            Value = p.Id.ToString(),
                                            Text = p.ClassName,
                                        }).ToList();

                selctClassList = (from p in tbSelctClassList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Value = p.Value,
                                      Text = p.Text,
                                  }).ToList();

                var selectExamSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                             where chksubjectList.Contains(p.Id.ToString())
                                             orderby p.No, p.SubjectName
                                             select new
                                             {
                                                 p.Id,
                                                 p.SubjectName,
                                             }).Distinct().ToList();

                selectSubjectList = (from p in selectExamSubjectList
                                     select new System.Web.Mvc.SelectListItem
                                     {
                                         Value = p.Id.ToString(),
                                         Text = p.SubjectName,
                                     }).Distinct().ToList();

            }
        }
        #endregion
    }
}