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
//光高
namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamAnalyzeGGController : Controller
    {
        #region 单科最高
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                vm.ClumnList = new List<string>() { "科目", "分数", "姓名", "班级" };

                //本次考试
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
                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
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
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          CiassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList(); ;

                vm.ClassList = (from p in tbClassStudent
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Value = p.CiassId.ToString(),
                                    Text = p.ClassName,
                                }).Distinct().ToList();

                if (vm.chkClass == null || vm.chkSubject == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');
                var chksubjectList = vm.chkSubject.Split(',');

                //班级
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
                                     }).ToList();

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
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == vm.ExamId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();


                #region  成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in selectClassStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();

                #region 年级班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           SubjectId = g.Key.SubjectId,
                                           MaxMark = g.Max(d => d.TotalMark),
                                       }).ToList();
                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.StudentId
                                            } into g
                                            select new
                                            {
                                                g.Key.StudentId,
                                                SubjectId = 0,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();

                //各班级总分最高分学生
                var MaxTotalMark = totalStudentMarkList.OrderByDescending(d => d.StudentTotalMark).FirstOrDefault();

                var lst = new List<Dto.ExamAnalyze.List>();
                foreach (var s in vm.selectSubjectList)
                {
                    var model = new Dto.ExamAnalyze.List();
                    var subjectId = s.Value.ConvertToInt();
                    if (subjectId == 0)//总分
                    {
                        model.SubjectName = s.Text;
                        var studentTotalMark = MaxTotalMark != null ? MaxTotalMark.StudentTotalMark : null;
                        model.TotalMark = studentTotalMark;
                        var studentIds = totalStudentMarkList.Where(d => d.StudentTotalMark == studentTotalMark).Select(d => d.StudentId).Distinct().ToList();
                        model.StudentName = string.Join(",", classExamMarkList.Where(d => studentIds.Contains(d.StudentId)).Select(d => d.StudentName).Distinct().ToArray());
                        model.ClassName = string.Join(",", classExamMarkList.Where(d => studentIds.Contains(d.StudentId)).Select(d => d.ClassName).Distinct().ToArray());
                    }
                    else
                    {
                        model.SubjectName = s.Text;
                        var maxMark = subjectMarkList.Where(d => d.SubjectId == subjectId).Select(d => d.MaxMark).FirstOrDefault();
                        model.TotalMark = maxMark;
                        model.StudentName = string.Join(",", classExamMarkList.Where(d => d.SubjectId == subjectId && d.TotalMark == maxMark).Select(d => d.StudentName).Distinct().ToArray());
                        model.ClassName = string.Join(",", classExamMarkList.Where(d => d.SubjectId == subjectId && d.TotalMark == maxMark).Select(d => d.ClassName).ToArray());
                    }
                    lst.Add(model);
                }
                vm.ExamAnalyzeList = lst;
                #endregion


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
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("List", new
            {
                ExamId = vm.ExamId,
                GradeId = vm.GradeId,
                chkSubject = chksubjectList,
                chkClass = chkclassList,
                checkedAll = CheckedAll,
                chkClassAll = chkClassAll,
                searchText = vm.SearchText
            }));
        }
        #endregion

        #region 各班尖子生
        public ActionResult ClassStudentTopList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                //本次考试
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
                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && (p.tbCourse.tbSubject.SubjectName.Contains(vm.SearchText) || vm.SearchText == null)
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id,
                                       CourseId=p.tbCourse.Id
                                   }).Distinct().ToList();

                var SubjectList = (from p in examSubjectList
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.SubjectId,
                                   }).Distinct().ToList();

                vm.SubjectList = (from p in SubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();

                vm.SubjectList.Add(new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });

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
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tbclassList = (from p in classStudent
                                   select new
                                   {
                                       p.ClassId,
                                       p.ClassName
                                   }).Distinct().ToList();

                vm.ClassList = (from p in tbclassList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Value = p.ClassId.ToString(),
                                    Text = p.ClassName,
                                }).ToList();

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == vm.ExamId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                #region  成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in classStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();
                #region  单个班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           MaxMark = g.Max(d => d.TotalMark),
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
                                                g.Key.StudentId,
                                                SubjectId = 0,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();

                var lst = new List<Dto.ExamAnalyze.List>();
                foreach (var c in vm.ClassList)
                {
                    foreach (var s in vm.SubjectList)
                    {
                        var model = new Dto.ExamAnalyze.List();
                        var subjectId = s.Value.ConvertToInt();
                        var classId = c.Value.ConvertToInt();
                        if (subjectId == 0)//总分
                        {
                            model.ClassId = classId;
                            model.SubjectId = subjectId;
                            //总分最高分学生
                            var MaxTotalMark = totalStudentMarkList.Where(d => d.ClassId == classId && d.SubjectId == subjectId).OrderByDescending(d => d.StudentTotalMark).FirstOrDefault();
                            var studentTotalMark = MaxTotalMark != null ? MaxTotalMark.StudentTotalMark : null;
                            model.TotalMark = studentTotalMark;
                            var studentIds = totalStudentMarkList.Where(d => d.ClassId == classId && d.SubjectId == subjectId && d.StudentTotalMark == studentTotalMark).Select(d => d.StudentId).Distinct().ToList();
                            model.StudentName = string.Join(",", classExamMarkList.Where(d => studentIds.Contains(d.StudentId)).Select(d => d.StudentName).Distinct().ToArray());
                        }
                        else
                        {
                            model.ClassId = classId;
                            model.SubjectId = subjectId;
                            var maxMark = subjectMarkList.Where(d => d.ClassId == classId && d.SubjectId == subjectId).Select(d => d.MaxMark).FirstOrDefault();
                            model.TotalMark = maxMark;
                            model.StudentName = string.Join(",", classExamMarkList.Where(d => d.ClassId == classId && d.SubjectId == subjectId && d.TotalMark == maxMark).Select(d => d.StudentName).Distinct().ToArray());
                        }
                        lst.Add(model);
                    }
                }
                vm.ExamAnalyzeList = lst;
                #endregion 

                #region  年级各科目总分最高分
                var gradeSubjectMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.SubjectId
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                SubjectId = g.Key.SubjectId,
                                                MaxMark = g.Max(d => d.TotalMark),
                                            }).ToList();
                //班级学生总成绩
                var gradeTotalStudentMarkList = (from p in classExamMarkList
                                                 group p by new
                                                 {
                                                     p.StudentId
                                                 } into g
                                                 select new
                                                 {
                                                     g.Key.StudentId,
                                                     SubjectId = 0,
                                                     StudentTotalMark = g.Sum(d => d.TotalMark)
                                                 }).ToList();

                var gradeMaxTotalMark = gradeTotalStudentMarkList.OrderByDescending(d => d.StudentTotalMark).FirstOrDefault();

                var lstGrade = new List<Dto.ExamAnalyze.List>();
                foreach (var s in vm.SubjectList)
                {
                    var model = new Dto.ExamAnalyze.List();
                    var subjectId = s.Value.ConvertToInt();
                    if (subjectId == 0)//总分
                    {
                        model.ClassId = 0;
                        model.SubjectId = subjectId;
                        var studentTotalMark = gradeMaxTotalMark != null ? gradeMaxTotalMark.StudentTotalMark : null;
                        model.TotalMark = studentTotalMark;
                        var studentIds = gradeTotalStudentMarkList.Where(d => d.SubjectId == subjectId && d.StudentTotalMark == studentTotalMark).Select(d => d.StudentId).Distinct().ToList();
                        model.StudentName = string.Join(",", classExamMarkList.Where(d => studentIds.Contains(d.StudentId)).Select(d => d.StudentName).Distinct().ToArray());
                        model.ClassName = string.Join(",", classExamMarkList.Where(d => studentIds.Contains(d.StudentId)).Select(d => d.ClassName).Distinct().ToArray());
                    }
                    else
                    {
                        model.ClassId = 0;
                        model.SubjectId = subjectId;
                        var maxMark = gradeSubjectMarkList.Where(d => d.SubjectId == subjectId).Select(d => d.MaxMark).FirstOrDefault();
                        model.TotalMark = maxMark;
                        model.StudentName = string.Join(",", classExamMarkList.Where(d => d.SubjectId == subjectId && d.TotalMark == maxMark).Select(d => d.StudentName).Distinct().ToArray());
                        model.ClassName = string.Join(",", classExamMarkList.Where(d => d.SubjectId == subjectId && d.TotalMark == maxMark).Select(d => d.ClassName).ToArray());
                    }
                    lstGrade.Add(model);
                }

                vm.ExamGradeAnalyzeList = lstGrade;
                #endregion

                #endregion

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassStudentTopList(Models.ExamAnalyze.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("ClassStudentTopList", new { ExamId = vm.ExamId, GradeId = vm.GradeId, searchText = vm.SearchText }));
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

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id,
                                       SubjectNo= p.tbCourse.tbSubject.No,
                                       p.FullTotalMark
                                   }).OrderBy(d=>d.SubjectNo).Distinct().ToList();

                var examSubjectList = (from p in SubjectList
                                   orderby p.SubjectNo
                                  select new
                                  {
                                      SubjectName = p.SubjectName,
                                      SubjectId = p.SubjectId
                                  }).Distinct().ToList();

                vm.SubjectList = (from p in examSubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).Distinct().ToList();

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
                        if ((double)t.TotalMark >= (double)totalFullMark * 0.6)
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

        #region 年级各班前十名
        public ActionResult ClassMarkTopNList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                vm.RankList = new List<SelectListItem>()
                             {
                                new System.Web.Mvc.SelectListItem { Text = "第一名", Value ="1" },
                                new System.Web.Mvc.SelectListItem { Text = "第二名", Value = "2" },
                                new System.Web.Mvc.SelectListItem { Text = "第三名", Value = "3" },
                                new System.Web.Mvc.SelectListItem { Text = "第四名", Value = "4" },
                                new System.Web.Mvc.SelectListItem { Text = "第五名", Value = "5" },
                                new System.Web.Mvc.SelectListItem { Text = "第六名", Value = "6" },
                                new System.Web.Mvc.SelectListItem { Text = "第七名", Value = "7" },
                                new System.Web.Mvc.SelectListItem { Text = "第八名", Value = "8" },
                                new System.Web.Mvc.SelectListItem { Text = "第九名", Value = "9" },
                                new System.Web.Mvc.SelectListItem { Text = "第十名", Value = "10" }
                              };

                //本次考试
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
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          ClassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList(); ;

                vm.ClassList = (from p in tbClassStudent
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Value = p.ClassId.ToString(),
                                    Text = p.ClassName,
                                }).Distinct().ToList();

                if (vm.chkClass == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');

                //班级
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
                                     }).ToList();

                vm.selctClassList.Add(new System.Web.Mvc.SelectListItem { Text = "年级", Value = "0" });

                //年级学生班级
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();
                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == vm.ExamId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                #region  成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in selectClassStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();

                #region 年级班级科目成绩
                //班级学生总成绩
                var lstRank = (from p in classExamMarkList
                               group p by new
                               {
                                   p.StudentId,
                                   p.StudentName,
                                   p.ClassId,
                                   p.ClassName
                               } into g
                               select new Dto.ExamAnalyze.List
                               {
                                   StudentId = g.Key.StudentId,
                                   StudentName = g.Key.StudentName,
                                   ClassId = g.Key.ClassId,
                                   ClassName = g.Key.ClassName,
                                   TotalMark = g.Sum(d => d.TotalMark),
                                   GradeRank = 0,
                                   ClassRank = 0
                               }).ToList();
                //排名
                var rank = 0;
                decimal? mark = null;
                var count = 1;
                foreach (var t in lstRank.OrderByDescending(d => d.TotalMark))
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
                    var tt = (from p in lstRank
                              where p.StudentId == t.StudentId
                              select p).FirstOrDefault();
                    if (tt != null)
                    {
                        tt.GradeRank = rank;
                    }
                }

                foreach (var c in tbselctClassList)
                {
                    rank = 0;
                    mark = null;
                    count = 1;
                    foreach (var t in lstRank.Where(d => d.ClassId == c.Value.ConvertToInt()).OrderByDescending(d => d.TotalMark))
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
                        var tt = (from p in lstRank.Where(d => d.ClassId == c.Value.ConvertToInt())
                                  where p.StudentId == t.StudentId
                                  select p).FirstOrDefault();
                        if (tt != null)
                        {
                            tt.ClassRank = rank;
                        }
                    }
                }
                vm.ExamAnalyzeList = lstRank;
                #endregion

                #endregion

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassMarkTopNList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("ClassMarkTopNList", new
            {
                ExamId = vm.ExamId,
                GradeId = vm.GradeId,
                chkClass = chkclassList,
                chkClassAll = chkClassAll,
                searchText = vm.SearchText
            }));
        }
        #endregion

        #region 各科情况统计报表
        public ActionResult ClassSubjectMarkList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                vm.OptionList = new List<string>() { "平均分", "及格率(%)", "优秀率(%)", "标准差", "最高分", "平均相对分" };

                //本次考试
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
                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
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
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          CiassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList(); ;

                vm.ClassList = (from p in tbClassStudent
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Value = p.CiassId.ToString(),
                                    Text = p.ClassName,
                                }).Distinct().ToList();

                if (vm.chkClass == null || vm.chkSubject == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');
                var chksubjectList = vm.chkSubject.Split(',');

                //班级
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
                                     }).ToList();

                vm.selctClassList.Add(new System.Web.Mvc.SelectListItem { Text = "年级", Value = "0" });

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

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject)
                                       where p.tbGrade.Id == vm.GradeId && p.IsTotal == false
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
                                           SubjectId = p.tbSubject != null ? p.tbSubject.Id : 0
                                       }).ToList();

                //年级学生班级
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == vm.ExamId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.TotalMark != null
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                var classExamMarkList = (from p in examMarkList
                                         join t in selectClassStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();

                #region 各班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           StudentCount = g.Count().ToString(),
                                           MaxMark = g.Max(d => d.TotalMark),
                                           AvgMark = g.Average(d => d.TotalMark)
                                       }).ToList();

                //年级科目成绩
                var gradeSubjectMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.SubjectId
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                SubjectId = g.Key.SubjectId,
                                                AvgMark = Math.Round(g.Average(d => d.TotalMark).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                            }).ToList();

                //当前考试：某科在班级中的标准差
                var lstDiff = new List<Exam.Dto.ExamAnalyze.List>();
                var StandardMark = decimal.Zero;
                double standardGap = 0;
                foreach (var c in tbselctClassList)
                {
                    foreach (var s in selectSubjectList)
                    {
                        StandardMark = decimal.Zero;
                        standardGap = 0;
                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.ClassId = c.Value.ConvertToInt();
                        var classMark = classExamMarkList.Where(d => d.ClassId == c.Value.ConvertToInt() && d.SubjectId == s.Id).ToList();
                        var gradeAvg = gradeSubjectMarkList.Where(d => d.SubjectId == s.Id).Select(d => d.AvgMark).FirstOrDefault();
                        foreach (var o in classMark)
                        {
                            var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                            var sd = (o.TotalMark - avgMark) * (o.TotalMark - avgMark);
                            StandardMark += sd.ConvertToDecimal();
                        }
                        if (classMark.Count > decimal.Zero)
                        {
                            standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / classMark.Count);
                        }
                        model.SubjectId = s.Id;
                        model.StandardDiff = standardGap;
                        lstDiff.Add(model);
                    }
                }
                var lstSegment = new List<Exam.Dto.ExamAnalyze.List>();
                //优秀及格率
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;
                    var isNormal = o.IsNormal;
                    //优秀科目人数
                    if (isGood)
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

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                    if (isNormal)//良好人数
                    {
                        //分数段人数
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = 2,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                    if (isPass)//及格人数
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

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = 3,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                }

                var tk = (from p in lstSegment
                          group p by new
                          {
                              p.ClassId,
                              p.SubjectId,
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.ClassId,
                              g.Key.SubjectId,
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum)
                          }).ToList();


                var ExamAnalyzeList = (from p in subjectMarkList
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           SubjectId = p.SubjectId,
                                           AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                           MaxMark = p.MaxMark,
                                           StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                           GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                           PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                       }).ToList();


                #endregion

                #region  年级成绩
                var gradeAvgMarkList = (from p in classExamMarkList
                                        group p by new
                                        {
                                            p.SubjectId
                                        } into g
                                        select new Exam.Dto.ExamAnalyze.List
                                        {
                                            SubjectId = g.Key.SubjectId,
                                            StudentCount = g.Count().ToString(),
                                            MaxMark = g.Max(d => d.TotalMark),
                                            AvgMark = g.Average(d => d.TotalMark)
                                        }).ToList();

                //当前考试：某科在班级中的标准差
                lstDiff = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var s in selectSubjectList)
                {
                    StandardMark = decimal.Zero;
                    standardGap = 0;
                    var gradeModel = new Exam.Dto.ExamAnalyze.List();
                    gradeModel.ClassId = 0;
                    var gradeMark = classExamMarkList.Where(d => d.SubjectId == s.Id).ToList();
                    var gradeAvg = gradeAvgMarkList.Where(d => d.SubjectId == s.Id).Select(d => d.AvgMark).FirstOrDefault();
                    foreach (var o in gradeMark)
                    {
                        var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                        var sd = (o.TotalMark - avgMark) * (o.TotalMark - avgMark);
                        StandardMark += sd.ConvertToDecimal();
                    }
                    if (gradeMark.Count > decimal.Zero)
                    {
                        standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / gradeMark.Count);
                    }
                    gradeModel.SubjectId = s.Id;
                    gradeModel.StandardDiff = standardGap;
                    lstDiff.Add(gradeModel);
                }

                lstSegment = new List<Exam.Dto.ExamAnalyze.List>();
                //优秀及格率
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;
                    var isNormal = o.IsNormal;
                    //优秀科目人数
                    if (isGood)
                    {
                        //分数段人数
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.SubjectId
                                  } into g
                                  select new
                                  {
                                      g.Key.SubjectId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = 0,
                                      SubjectId = o.SubjectId,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                    if (isNormal)//良好人数
                    {
                        //分数段人数
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.SubjectId,
                                  } into g
                                  select new
                                  {
                                      g.Key.SubjectId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = 0,
                                      SubjectId = o.SubjectId,
                                      Status = 2,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                    if (isPass)//及格人数
                    {
                        //分数段人数
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.SubjectId
                                  } into g
                                  select new
                                  {
                                      g.Key.SubjectId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = 0,
                                      SubjectId = o.SubjectId,
                                      Status = 3,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                }

                tk = (from p in lstSegment
                      group p by new
                      {
                          p.ClassId,
                          p.SubjectId,
                          p.Status
                      } into g
                      select new
                      {
                          g.Key.ClassId,
                          g.Key.SubjectId,
                          g.Key.Status,
                          StudentNum = g.Sum(d => d.StudentNum)
                      }).ToList();

                var garadeExamAnalyzeList = (from p in gradeSubjectMarkList
                                             select new Exam.Dto.ExamAnalyze.List
                                             {
                                                 ClassId = p.ClassId,
                                                 SubjectId = p.SubjectId,
                                                 AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                                 MaxMark = p.MaxMark,
                                                 StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                                 GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                 PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                             }).ToList();
                #endregion

                vm.ExamAnalyzeList = (from p in ExamAnalyzeList.Union(garadeExamAnalyzeList)
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ClassId = p.ClassId,
                                          SubjectId = p.SubjectId,
                                          StandardDiff = System.Math.Round(p.StandardDiff, 1, MidpointRounding.AwayFromZero),
                                          AvgMark = p.AvgMark,
                                          MaxMark = p.MaxMark,
                                          GoodRate = p.GoodRate,
                                          PassRate = p.PassRate
                                      }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClassSubjectMarkList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("ClassSubjectMarkList", new
            {
                ExamId = vm.ExamId,
                GradeId = vm.GradeId,
                chkSubject = chksubjectList,
                chkClass = chkclassList,
                checkedAll = CheckedAll,
                chkClassAll = chkClassAll,
                searchText = vm.SearchText
            }));
        }
        #endregion

        #region 成绩综合报表
        public ActionResult CompMarkReportList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

                vm.OptionList = new List<string>() { "班级", "应考数", "实考数", "平均分", "最高分", "最低分", "标准差" };
                vm.ClumnList = new List<string>() { "优秀率%", "良好率%", "及格率%" };
                //本次考试
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
                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == vm.ExamId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
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
                //任课老师
                var courseIds = examSubjectList.Select(d => d.CourseId).Distinct().ToList();
                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                     .Include(d => d.tbOrg.tbClass)
                                      where p.tbOrg.IsDeleted == false
                                      && courseIds.Contains(p.tbOrg.tbCourse.Id)
                                      && p.tbOrg.tbYear.Id == year.Id
                                      && p.tbTeacher.IsDeleted == false
                                      && p.tbOrg.tbCourse.IsDeleted == false
                                      && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
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
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          CiassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList();

                vm.ClassList = (from p in tbClassStudent
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Value = p.CiassId.ToString(),
                                    Text = p.ClassName,
                                }).Distinct().ToList();

                if (vm.chkClass == null || vm.chkSubject == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');
                var chksubjectList = vm.chkSubject.Split(',');

                //选中班级和科目
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
                                     }).ToList();

                vm.selctClassList.Add(new System.Web.Mvc.SelectListItem { Text = "年级", Value = "0" });

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

                //重要分数段
                var ImportSegmentMarkList = (from p in db.Table<Exam.Entity.tbExamImportSegmentMark>()
                                             where p.tbGrade.Id == vm.GradeId
                                             orderby p.No
                                             select new
                                             {
                                                 SegmentId = p.Id,
                                                 p.SegmentName,
                                                 p.MinMark,
                                                 p.MaxMark,
                                             }).ToList();

                vm.ImortSegmentList = (from p in ImportSegmentMarkList
                                       select new Dto.ExamAnalyze.SegmentList
                                       {
                                           SegmentId = p.SegmentId,
                                           SegmentName = p.SegmentName
                                       }).ToList();

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

                vm.SegmentList = (from p in SegmentMarkList
                                  select new Dto.ExamAnalyze.SegmentList
                                  {
                                      SegmentId = p.SegmentId,
                                      SegmentName = p.SegmentName,
                                      SubjectId = p.SubjectId,
                                      IsTotal = p.IsTotal
                                  }).ToList();

                //选中年级班级学生
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          orderby p.StudentId
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == vm.ExamId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                     && p.TotalMark != null
                                    orderby p.tbStudent.Id
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                var classExamMarkList = (from p in examMarkList
                                         join t in selectClassStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();

                #region 科目成绩
                //科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           StudentCount = g.Count().ToString(),//实考人数
                                           StudentNum = classStudent.Where(d => d.ClassId == g.Key.ClassId).Count(),//应考人数
                                           MaxMark = g.Max(d => d.TotalMark),
                                           MinMark = g.Min(d => d.TotalMark),
                                           AvgMark = g.Average(d => d.TotalMark)
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
                                                g.Key.StudentId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();
                var ty = (from p in totalStudentMarkList
                          group p by new
                          {
                              p.ClassId,
                          } into g
                          select new
                          {
                              g.Key.ClassId,
                              StudentCount = g.Count()
                          }).ToList();
                var totalMarkList = (from p in totalStudentMarkList
                                     group p by new { p.ClassId } into g
                                     select new Exam.Dto.ExamAnalyze.List
                                     {
                                         ClassId = g.Key.ClassId,
                                         StudentCount = ty.Where(d => d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                         StudentNum = classStudent.Where(d => d.ClassId == g.Key.ClassId).Count(),//应考人数
                                         AvgMark = g.Average(d => d.StudentTotalMark),
                                         MaxMark = g.Max(d => d.StudentTotalMark),
                                         MinMark = g.Min(d => d.StudentTotalMark)
                                     }).ToList();

                var totalGrdeAvg = totalStudentMarkList.Average(d => d.StudentTotalMark);


                //年级科目成绩
                var gradeSubjectMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.SubjectId
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                SubjectId = g.Key.SubjectId,
                                                ClassId = 0,
                                                StudentNum = selectClassStudent.Count(),
                                                StudentCount = g.Count().ToString(),
                                                AvgMark = Math.Round(g.Average(d => d.TotalMark).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                                MaxMark = g.Max(d => d.TotalMark),
                                                MinMark = g.Min(d => d.TotalMark)
                                            }).ToList();
                //年级总分
                var totalGradeStudentMarkList = (from p in classExamMarkList
                                                 group p by new
                                                 {
                                                     p.StudentId
                                                 } into g
                                                 select new
                                                 {
                                                     ClassId = 0,
                                                     SubjectId = 0,
                                                     g.Key.StudentId,
                                                     StudentTotalMark = g.Sum(d => d.TotalMark)
                                                 }).ToList();

                var totalGradeMarkList = (from p in totalGradeStudentMarkList
                                          group p by new { p.ClassId } into g
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ClassId = g.Key.ClassId,
                                              StudentCount = totalGradeStudentMarkList.Where(d => d.ClassId == g.Key.ClassId).Count().ToString(),
                                              StudentNum = selectClassStudent.Count(),//应考总人数
                                              AvgMark = decimal.Round(g.Average(d => d.StudentTotalMark).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                              MaxMark = g.Max(d => d.StudentTotalMark),
                                              MinMark = g.Min(d => d.StudentTotalMark)
                                          }).ToList();

                //当前考试：某科在班级中的标准差
                var lstDiff = new List<Exam.Dto.ExamAnalyze.List>();
                var StandardMark = decimal.Zero;
                double standardGap = 0;
                foreach (var c in tbselctClassList)
                {
                    foreach (var s in vm.selectSubjectList)
                    {
                        StandardMark = decimal.Zero;
                        standardGap = 0;
                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.ClassId = c.Value.ConvertToInt();
                        var subjectId = s.Value.ConvertToInt();
                        if (subjectId != 0)
                        {
                            var classMark = classExamMarkList.Where(d => d.ClassId == c.Value.ConvertToInt() && d.SubjectId == subjectId).ToList();
                            var gradeAvg = gradeSubjectMarkList.Where(d => d.SubjectId == subjectId).Select(d => d.AvgMark).FirstOrDefault();
                            foreach (var o in classMark)
                            {
                                var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                                var sd = (o.TotalMark - avgMark) * (o.TotalMark - avgMark);
                                StandardMark += sd.ConvertToDecimal();
                            }
                            if (classMark.Count > decimal.Zero)
                            {
                                standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / classMark.Count);
                            }
                            model.SubjectId = subjectId;
                            model.StandardDiff = standardGap;
                            lstDiff.Add(model);
                        }
                        else //总分
                        {
                            var classMark = totalStudentMarkList.Where(d => d.ClassId == c.Value.ConvertToInt()).ToList();
                            var gradeAvg = totalGrdeAvg;
                            foreach (var o in classMark)
                            {
                                var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                                var sd = (o.StudentTotalMark - avgMark) * (o.StudentTotalMark - avgMark);
                                StandardMark += sd.ConvertToDecimal();
                            }
                            if (classMark.Count > decimal.Zero)
                            {
                                standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / classMark.Count);
                            }
                            model.SubjectId = subjectId;
                            model.StandardDiff = standardGap;
                            lstDiff.Add(model);
                        }
                    }
                }
                //良好及格优秀
                var lstSegment = new List<Exam.Dto.ExamAnalyze.List>();
                //各分数段人数
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                //优秀,良好及格率以及各分数段人数
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;
                    var isNormal = o.IsNormal;
                    var isTotal = o.IsTotal;
                    if (isTotal && o.SubjectId == 0)//总分
                    {
                        var tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count()
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SegmentId = o.SegmentId,
                                      SubjectId = 0,
                                      StudentNum = p.StudentCount
                                  }).ToList();
                        lst.AddRange(tb);

                        if (isGood)
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count()
                                  }).ToList();

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = 0,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count()
                                  }).ToList();

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = 0,
                                      Status = 2,
                                      StudentNum = p.StudentCount
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count()
                                  }).ToList();

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = 0,
                                      Status = 3,
                                      StudentNum = p.StudentCount
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                    }
                    else
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

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      SegmentId = o.SegmentId,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lst.AddRange(tb);
                        //优秀科目人数
                        if (isGood)
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
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

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = 2,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
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

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = 3,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                    }
                }
                //重要分数段人数
                var lstImport = new List<Dto.ExamAnalyze.List>();
                foreach (var o in ImportSegmentMarkList)
                {
                    var tm = (from p in totalStudentMarkList
                              where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                              group p by new
                              {
                                  p.ClassId
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  StudentCount = g.Count()
                              }).ToList();

                    var tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SegmentId = o.SegmentId,
                                  SubjectId = 0,
                                  StudentNum = p.StudentCount
                              }).ToList();
                    lstImport.AddRange(tb);

                    foreach (var subject in selectSubjectList)
                    {
                        tm = (from p in classExamMarkList
                              where p.SubjectId == subject.Id
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

                        tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = subject.Id,
                                  SegmentId = o.SegmentId,
                                  StudentNum = p.StudentCount,
                              }).ToList();
                        lstImport.AddRange(tb);
                    }
                }

                var tk = (from p in lstSegment
                          group p by new
                          {
                              p.ClassId,
                              p.SubjectId,
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.ClassId,
                              g.Key.SubjectId,
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum)
                          }).ToList();

                //各科目项目
                var examSubjectAnalyzeList = (from p in subjectMarkList
                                              select new
                                              {
                                                  ClassId = p.ClassId,
                                                  SubjectId = p.SubjectId,
                                                  StudentNum = p.StudentNum,
                                                  StudentCount = p.StudentCount,
                                                  AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                                  GradeAvg = gradeSubjectMarkList.Where(d => d.SubjectId == p.SubjectId).Select(d => d.AvgMark).FirstOrDefault(),
                                                  MaxMark = p.MaxMark,
                                                  MinMark = p.MinMark,
                                                  StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                                  GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                  NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                  PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                              }).ToList();

                var ExamAnalyzeList = (from p in examSubjectAnalyzeList
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           SubjectId = p.SubjectId,
                                           StudentNum = p.StudentNum,
                                           StudentCount = p.StudentCount,
                                           AvgMark = p.AvgMark,
                                           MaxMark = p.MaxMark,
                                           MinMark = p.MinMark,
                                           StandardDiff = p.StandardDiff,
                                           StandardMark = p.StandardDiff != 0 ? Math.Round(((p.AvgMark - p.GradeAvg) * 10 / (decimal)p.StandardDiff + 50).ConvertToDecimal(), 1, MidpointRounding.AwayFromZero) : 50,
                                           GoodRate = p.GoodRate,
                                           NormalRate = p.NormalRate,
                                           PassRate = p.PassRate,
                                       }).ToList();

                //总分项目
                var examTotalSubjectAnalyzeList = (from p in totalMarkList
                                                   select new
                                                   {
                                                       ClassId = p.ClassId,
                                                       SubjectId = p.SubjectId,
                                                       StudentNum = p.StudentNum,
                                                       StudentCount = p.StudentCount,
                                                       AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                                       GradeAvg = totalGrdeAvg,
                                                       MaxMark = p.MaxMark,
                                                       MinMark = p.MinMark,
                                                       StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                                       GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                       NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                       PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                                   }).ToList();

                var ExamTotalAnalyzeList = (from p in examTotalSubjectAnalyzeList
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                ClassId = p.ClassId,
                                                SubjectId = p.SubjectId,
                                                StudentNum = p.StudentNum,
                                                StudentCount = p.StudentCount,
                                                AvgMark = p.AvgMark,
                                                MaxMark = p.MaxMark,
                                                MinMark = p.MinMark,
                                                StandardDiff = p.StandardDiff,
                                                StandardMark = p.StandardDiff != 0 ? Math.Round(((p.AvgMark - p.GradeAvg) * 10 / (decimal)p.StandardDiff + 50).ConvertToDecimal(), 1, MidpointRounding.AwayFromZero) : 50,
                                                GoodRate = p.GoodRate,
                                                NormalRate = p.NormalRate,
                                                PassRate = p.PassRate,
                                            }).ToList();

                #endregion

                #region  年级项目成绩
                //当前考试：某科在班级中的标准差
                lstDiff = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var s in vm.selectSubjectList)
                {
                    StandardMark = decimal.Zero;
                    standardGap = 0;
                    var subjectId = s.Value.ConvertToInt();
                    var gradeModel = new Exam.Dto.ExamAnalyze.List();
                    gradeModel.ClassId = 0;
                    if (subjectId != 0)
                    {
                        var gradeMark = classExamMarkList.Where(d => d.SubjectId == subjectId).ToList();
                        var gradeAvg = gradeSubjectMarkList.Where(d => d.SubjectId == subjectId).Select(d => d.AvgMark).FirstOrDefault();
                        foreach (var o in gradeMark)
                        {
                            var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                            var sd = (o.TotalMark - avgMark) * (o.TotalMark - avgMark);
                            StandardMark += sd.ConvertToDecimal();
                        }
                        if (gradeMark.Count > decimal.Zero)
                        {
                            standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / gradeMark.Count);
                        }
                        gradeModel.SubjectId = subjectId;
                        gradeModel.StandardDiff = standardGap;
                        lstDiff.Add(gradeModel);
                    }
                    else
                    {
                        var gradeMark = totalGradeStudentMarkList.Where(d => d.SubjectId == subjectId).ToList();
                        var gradeAvg = totalGradeMarkList.Where(d => d.SubjectId == subjectId).Select(d => d.AvgMark).FirstOrDefault();
                        foreach (var o in gradeMark)
                        {
                            var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                            var sd = (o.StudentTotalMark - avgMark) * (o.StudentTotalMark - avgMark);
                            StandardMark += sd.ConvertToDecimal();
                        }
                        if (gradeMark.Count > decimal.Zero)
                        {
                            standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / gradeMark.Count);
                        }
                        gradeModel.SubjectId = subjectId;
                        gradeModel.StandardDiff = standardGap;
                        lstDiff.Add(gradeModel);
                    }
                }

                //良好及格优秀
                var lstGradeSegment = new List<Exam.Dto.ExamAnalyze.List>();
                //各分数段人数
                var lstGrade = new List<Exam.Dto.ExamAnalyze.List>();
                //优秀及格率
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;
                    var isNormal = o.IsNormal;
                    var isTotal = o.IsTotal;
                    //优秀科目人数
                    if (isTotal && o.SubjectId == 0)
                    {
                        var tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  select p).ToList();

                        var tb = new Exam.Dto.ExamAnalyze.List()
                        {
                            ClassId = 0,
                            SegmentId = o.SegmentId,
                            SubjectId = 0,
                            StudentNum = tm.Count()
                        };
                        lstGrade.Add(tb);
                        if (isGood)
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  select p).ToList();

                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SegmentId = o.SegmentId,
                                SubjectId = 0,
                                Status = 1,
                                StudentNum = tm.Count()
                            };
                            lstGradeSegment.Add(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  select p).ToList();

                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SegmentId = o.SegmentId,
                                SubjectId = 0,
                                Status = 2,
                                StudentNum = tm.Count()
                            };
                            lstGradeSegment.Add(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  select p).ToList();

                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SegmentId = o.SegmentId,
                                SubjectId = 0,
                                Status = 3,
                                StudentNum = tm.Count()
                            };
                            lstGradeSegment.Add(tb);
                        }
                    }
                    else
                    {
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  select p).ToList();

                        var tb = new Exam.Dto.ExamAnalyze.List()
                        {
                            ClassId = 0,
                            SubjectId = o.SubjectId,
                            SegmentId = o.SegmentId,
                            StudentNum = tm.Count(),
                        };
                        lstGrade.Add(tb);
                        if (isGood)
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  select p).ToList();

                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SubjectId = o.SubjectId,
                                Status = decimal.One,
                                StudentNum = tm.Count(),
                            };
                            lstGradeSegment.Add(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  select p).ToList();

                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SubjectId = o.SubjectId,
                                Status = 2,
                                StudentNum = tm.Count(),
                            };
                            lstGradeSegment.Add(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  select p).ToList();
                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SubjectId = o.SubjectId,
                                Status = 3,
                                StudentNum = tm.Count(),
                            };
                            lstGradeSegment.Add(tb);
                        }
                    }
                }

                //重要分数段人数
                var lstGradeImport = new List<Dto.ExamAnalyze.List>();
                foreach (var o in ImportSegmentMarkList)
                {
                    var tm = (from p in totalStudentMarkList
                              where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                              select p).ToList();

                    var tb = new Exam.Dto.ExamAnalyze.List()
                    {
                        ClassId = 0,
                        SegmentId = o.SegmentId,
                        SubjectId = 0,
                        StudentNum = tm.Count()
                    };
                    lstGradeImport.Add(tb);

                    foreach (var subject in selectSubjectList)
                    {
                        var tg = (from p in classExamMarkList
                                  where p.SubjectId == subject.Id
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  select p).ToList();

                        tb = new Exam.Dto.ExamAnalyze.List()
                        {
                            ClassId = 0,
                            SegmentId = o.SegmentId,
                            SubjectId = subject.Id,
                            StudentNum = tg.Count()
                        };
                        lstGradeImport.Add(tb);
                    }
                }

                tk = (from p in lstGradeSegment
                      group p by new
                      {
                          p.ClassId,
                          p.SubjectId,
                          p.Status
                      } into g
                      select new
                      {
                          g.Key.ClassId,
                          g.Key.SubjectId,
                          g.Key.Status,
                          StudentNum = g.Sum(d => d.StudentNum)
                      }).ToList();

                var garadeExamAnalyzeList = (from p in gradeSubjectMarkList
                                             select new Exam.Dto.ExamAnalyze.List
                                             {
                                                 ClassId = p.ClassId,
                                                 SubjectId = p.SubjectId,
                                                 StudentNum = p.StudentNum,
                                                 StudentCount = p.StudentCount,
                                                 AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                                 MaxMark = p.MaxMark,
                                                 MinMark = p.MinMark,
                                                 StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                                 StandardMark = 50,
                                                 GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                 NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                 PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                             }).ToList();

                var garadeTotalExamAnalyzeList = (from p in totalGradeMarkList
                                                  select new Exam.Dto.ExamAnalyze.List
                                                  {
                                                      ClassId = p.ClassId,
                                                      SubjectId = p.SubjectId,
                                                      StudentNum = p.StudentNum,
                                                      StudentCount = p.StudentCount,
                                                      AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                                      MaxMark = p.MaxMark,
                                                      MinMark = p.MinMark,
                                                      StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                                      StandardMark = 50,
                                                      GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                      NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                      PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                                  }).ToList();
                #endregion

                //标准分各科目和总分班级年级排名
                var lstRank = (from p in ExamAnalyzeList.Union(ExamTotalAnalyzeList)
                               select new Exam.Dto.ExamAnalyze.List
                               {
                                   ClassId = p.ClassId,
                                   SubjectId = p.SubjectId,
                                   StudentNum = p.StudentNum,
                                   StudentCount = p.StudentCount,
                                   StandardDiff = System.Math.Round(p.StandardDiff, 1, MidpointRounding.AwayFromZero),
                                   StandardMark = Math.Round(p.StandardMark.ConvertToDecimal(), 1, MidpointRounding.AwayFromZero),
                                   AvgMark = p.AvgMark,
                                   MaxMark = p.MaxMark,
                                   MinMark = p.MinMark,
                                   GoodRate = p.GoodRate,
                                   NormalRate = p.NormalRate,
                                   PassRate = p.PassRate,
                                   TeacherName = p.TeacherName,
                                   GradeRank = 0
                               }).ToList();

                foreach (var s in vm.selectSubjectList)
                {
                    var rank = 0;
                    decimal? mark = null;
                    var count = 1;
                    foreach (var t in lstRank.Where(d => d.SubjectId == s.Value.ConvertToInt()).OrderByDescending(d => d.StandardMark))
                    {
                        if (mark != t.StandardMark)
                        {
                            mark = t.StandardMark;
                            rank = rank + count;
                            count = 1;
                        }
                        else
                        {
                            count = count + 1;
                        }
                        var tt = (from p in lstRank
                                  where p.ClassId == t.ClassId && p.SubjectId == s.Value.ConvertToInt()
                                  select p).FirstOrDefault();
                        if (tt != null)
                        {
                            tt.GradeRank = rank;
                        }
                    }
                }

                vm.ExamAnalyzeList = (from p in lstRank.Union(garadeExamAnalyzeList).Union(garadeTotalExamAnalyzeList)
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ClassId = p.ClassId,
                                          SubjectId = p.SubjectId,
                                          StudentNum = p.StudentNum,
                                          StudentCount = p.StudentCount,
                                          StandardDiff = System.Math.Round(p.StandardDiff, 1, MidpointRounding.AwayFromZero),
                                          StandardMark = Math.Round(p.StandardMark.ConvertToDecimal(), 1, MidpointRounding.AwayFromZero),
                                          AvgMark = p.AvgMark,
                                          MaxMark = p.MaxMark,
                                          MinMark = p.MinMark,
                                          GoodRate = p.GoodRate,
                                          NormalRate = p.NormalRate,
                                          PassRate = p.PassRate,
                                          TeacherName = p.TeacherName,
                                          GradeRank = p.GradeRank
                                      }).ToList();

                vm.ExamAnalyzeImportSegmentList = (from p in lstImport.Union(lstGradeImport)
                                                   select new Exam.Dto.ExamAnalyze.List
                                                   {
                                                       ClassId = p.ClassId,
                                                       SegmentId = p.SegmentId,
                                                       SubjectId = p.SubjectId,
                                                       StudentNum = p.StudentNum
                                                   }).ToList();

                vm.ExamAnalyzeSegmentList = (from p in lst.Union(lstGrade)
                                             select new Exam.Dto.ExamAnalyze.List
                                             {
                                                 ClassId = p.ClassId,
                                                 SegmentId = p.SegmentId,
                                                 SubjectId = p.SubjectId,
                                                 StudentNum = p.StudentNum
                                             }).ToList();
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompMarkReportList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("CompMarkReportList", new
            {
                ExamId = vm.ExamId,
                GradeId = vm.GradeId,
                chkSubject = chksubjectList,
                chkClass = chkclassList,
                checkedAll = CheckedAll,
                chkClassAll = chkClassAll,
                searchText = vm.SearchText
            }));
        }
        #endregion

        #region 总分对比名次进退
        public ActionResult TotalScoreAdvanceList()
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

                vm.ExamTotalList.Add((from p in db.Table<Exam.Entity.tbExam>()
                                      where p.IsDeleted == false
                                      && p.Id == vm.ExamId
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          ExamId = p.Id,
                                          ExamName = p.ExamName,
                                      }).FirstOrDefault());
                //上次考试
                vm.LastExamList = Areas.Exam.Controllers.ExamController.SelectList();
                if (vm.LastExamId == 0 && vm.LastExamList.Count > 0)
                {
                    vm.LastExamId = vm.LastExamList.FirstOrDefault().Value.ConvertToInt();
                }
                ntExam.Add(vm.LastExamId);
                vm.ExamTotalList.Add((from p in db.Table<Exam.Entity.tbExam>()
                                      where p.IsDeleted == false
                                      && p.Id == vm.LastExamId
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          LastExamId = p.Id,
                                          ExamName = p.ExamName,
                                      }).FirstOrDefault());

                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }
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
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName,
                                        p.tbStudent.StudentCode,
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          CiassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList(); ;

                vm.ClassList = (from p in tbClassStudent
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Value = p.CiassId.ToString(),
                                    Text = p.ClassName,
                                }).Distinct().ToList();

                if (vm.chkClass == null || vm.chkSubject == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');
                var chksubjectList = vm.chkSubject.Split(',');

                //班级XkSystemMessage.DTO
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
                                     }).ToList();

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

                //年级学生班级
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == vm.ExamId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                //上次考试考试
                var examLastMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                        where p.tbExamCourse.IsDeleted == false
                                         && p.tbStudent.IsDeleted == false
                                         && p.tbExamCourse.tbExam.Id == vm.LastExamId
                                         && p.tbExamCourse.tbCourse.IsDeleted == false
                                         && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                        select new
                                        {
                                            p.TotalMark,
                                            ExamId = p.tbExamCourse.tbExam.Id,
                                            StudentId = p.tbStudent.Id,
                                            SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        }).ToList();

                #region  成绩分析
                var lst = new List<Dto.ExamAnalyze.List>();
                #region 本次考试
                var classExamMarkList = (from p in examMarkList
                                         join t in selectClassStudent
                                         on p.StudentId equals t.StudentId
                                         where (t.StudentName.Contains(vm.SearchText) || vm.SearchText == null)
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();

                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.StudentId,
                                                p.ExamId,
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                ExamId = g.Key.ExamId,
                                                StudentId = g.Key.StudentId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark),
                                                GradeRank = 0,
                                            }).ToList();

                //年级排名
                var gradeRank = decimal.Zero;
                decimal? gradeMark = null;
                var gradeCount = decimal.One;
                foreach (var t in totalStudentMarkList.OrderByDescending(d => d.StudentTotalMark))
                {
                    if (gradeMark != t.StudentTotalMark)
                    {
                        gradeMark = t.StudentTotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.GradeRank = gradeRank;
                }
                #endregion

                #region 上次考试
                var classLastExamMarkList = (from p in examLastMarkList
                                             join t in selectClassStudent
                                             on p.StudentId equals t.StudentId
                                             where (t.StudentName.Contains(vm.SearchText) || vm.SearchText == null)
                                             select new
                                             {
                                                 ExamId = p.ExamId,
                                                 StudentId = p.StudentId,
                                                 StudentName = t.StudentName,
                                                 SubjectId = p.SubjectId,
                                                 TotalMark = p.TotalMark,
                                                 ClassId = t.ClassId,
                                                 ClassName = t.ClassName,
                                             }).ToList();

                #region 获取学生成绩总分
                //班级学生总成绩
                var totalLastStudentMarkList = (from p in classLastExamMarkList
                                                group p by new
                                                {
                                                    p.StudentId,
                                                    p.ExamId,
                                                } into g
                                                select new Exam.Dto.ExamAnalyze.List
                                                {
                                                    ExamId = g.Key.ExamId,
                                                    StudentId = g.Key.StudentId,
                                                    StudentTotalMark = g.Sum(d => d.TotalMark),
                                                    GradeRank = 0,
                                                }).ToList();

                //年级排名
                gradeRank = decimal.Zero;
                gradeMark = null;
                gradeCount = decimal.One;
                foreach (var t in totalLastStudentMarkList.OrderByDescending(d => d.StudentTotalMark))
                {
                    if (gradeMark != t.StudentTotalMark)
                    {
                        gradeMark = t.StudentTotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.GradeRank = gradeRank;
                }

                var result = (from p in classStudent
                              join t in totalStudentMarkList on p.StudentId equals t.StudentId
                              join c in totalLastStudentMarkList on p.StudentId equals c.StudentId
                              where chkclassList.Contains(p.ClassId.ToString())
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ExamId = t.ExamId,
                                  LastExamId = c.ExamId,
                                  StudentId = t.StudentId,
                                  StudentCode = p.StudentCode,
                                  StudentName = p.StudentName,
                                  StudentTotalMark = t.StudentTotalMark,
                                  StudentLastTotalMark = c.StudentTotalMark,
                                  GradeRank = t.GradeRank,
                                  GradeLastRank = c.GradeRank,
                                  ClassName = p.ClassName,
                              }).ToList();
                foreach (var r in result)
                {
                    r.GradeAdvanceRank = r.GradeRank != null ? (r.GradeLastRank != null ? r.GradeRank.Value - r.GradeLastRank.Value : r.GradeRank.Value) : (r.GradeLastRank != null ? 0 - r.GradeLastRank.Value : 0);
                }

                vm.ExamAnalyzeList = result;
                #endregion

                #endregion

                #endregion

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TotalScoreAdvanceList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("TotalScoreAdvanceList", new
            {
                ExamId = vm.ExamId,
                LastExamId = vm.LastExamId,
                GradeId = vm.GradeId,
                chkSubject = chksubjectList,
                chkClass = chkclassList,
                checkedAll = CheckedAll,
                chkClassAll = chkClassAll,
                searchText = vm.SearchText
            }));
        }
        #endregion

        #region 总分细化段
        public ActionResult TotalSegmentList()
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
                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   //&& (p.tbCourse.tbSubject.SubjectName.Contains(vm.SearchText) || vm.SearchText == null)
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
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName,
                                        p.tbStudent.StudentCode,
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          CiassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList();

                vm.ClassList = (from p in tbClassStudent
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Value = p.CiassId.ToString(),
                                    Text = p.ClassName,
                                }).Distinct().ToList();

                if (vm.chkClass == null || vm.chkSubject == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');
                var chksubjectList = vm.chkSubject.Split(',');

                //班级
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
                                     }).ToList();

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

                //年级学生班级
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();

                var selectStudentIds = selectClassStudent.Select(d => d.StudentId).Distinct().ToList();


                //获取课程ID
                var courseIds = (from p in db.Table<Course.Entity.tbOrg>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.IsDeleted == false
                                  && p.tbGrade.Id == vm.GradeId
                                  && p.tbCourse.IsDeleted == false
                                  && p.tbYear.Id == year.Id
                                  && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
                                 select p.tbCourse).Select(d => d.Id).Distinct().ToList();
                //获取考试各科满分总分
                vm.MaxTotalMark = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.IsDeleted == false
                                    && p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false
                                    && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
                                    && courseIds.Contains(p.tbCourse.Id)
                                   select p).Distinct().Sum(d => d.FullTotalMark);

                if (vm.SearchText.ConvertToDecimal() > vm.MaxTotalMark)
                {
                    vm.SearchText = vm.MaxTotalMark.ToString();
                }

                //考试成绩
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == vm.ExamId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                //获取学生总分
                vm.ExamAnalyzeList = (from p in examMarkList
                                      join t in selectClassStudent
                                      on p.StudentId equals t.StudentId
                                      group p by new { t.ClassId, t.StudentId } into g
                                      select new Dto.ExamAnalyze.List
                                      {
                                          TotalMark = g.Sum(d => d.TotalMark),
                                          ClassId = g.Key.ClassId,
                                          StudentId = g.Key.StudentId,
                                      }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TotalSegmentList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("TotalSegmentList", new
            {
                ExamId = vm.ExamId,
                GradeId = vm.GradeId,
                chkSubject = chksubjectList,
                chkClass = chkclassList,
                checkedAll = CheckedAll,
                chkClassAll = chkClassAll,
                searchText = vm.SearchText
            }));
        }
        #endregion

        #region  导出
        public ActionResult ExportSubjectMax(int examId, int gradeId, string chkSubject, string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计

                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(searchText) || searchText == null)
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

                SubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });

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
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          CiassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList(); ;

                var ClassList = (from p in tbClassStudent
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Value = p.CiassId.ToString(),
                                     Text = p.ClassName,
                                 }).Distinct().ToList();

                if (chkClass == null || chkSubject == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList = chkClass.Split(',');
                var chksubjectList = chkSubject.Split(',');

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
                                               p.Id,
                                               p.SubjectName,
                                           }).Distinct().ToList();

                var selectSubjectList = (from p in tbselectSubjectList
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
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();


                #region  成绩分析
                var classExamMarkList = (from p in examMarkList
                                         join t in selectClassStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();
                //单个班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           SubjectId = g.Key.SubjectId,
                                           MaxMark = g.Max(d => d.TotalMark),
                                       }).ToList();
                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.StudentId
                                            } into g
                                            select new
                                            {
                                                g.Key.StudentId,
                                                SubjectId = 0,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();

                //总分最高分学生
                var MaxTotalMark = totalStudentMarkList.OrderByDescending(d => d.StudentTotalMark).FirstOrDefault();

                var lst = new List<Dto.ExamAnalyze.List>();
                foreach (var s in selectSubjectList)
                {
                    var model = new Dto.ExamAnalyze.List();
                    var subjectId = s.Value.ConvertToInt();
                    if (subjectId == 0)//总分
                    {
                        model.SubjectName = s.Text;
                        var studentTotalMark = MaxTotalMark != null ? MaxTotalMark.StudentTotalMark : null;
                        model.TotalMark = studentTotalMark;
                        var studentIds = totalStudentMarkList.Where(d => d.StudentTotalMark == studentTotalMark).Select(d => d.StudentId).Distinct().ToList();
                        model.StudentName = string.Join(",", classExamMarkList.Where(d => studentIds.Contains(d.StudentId)).Select(d => d.StudentName).Distinct().ToArray());
                        model.ClassName = string.Join(",", classExamMarkList.Where(d => studentIds.Contains(d.StudentId)).Select(d => d.ClassName).Distinct().ToArray());
                    }
                    else
                    {
                        model.SubjectName = s.Text;
                        var maxMark = subjectMarkList.Where(d => d.SubjectId == subjectId).Select(d => d.MaxMark).FirstOrDefault();
                        model.TotalMark = maxMark;
                        model.StudentName = string.Join(",", classExamMarkList.Where(d => d.SubjectId == subjectId && d.TotalMark == maxMark).Select(d => d.StudentName).Distinct().ToArray());
                        model.ClassName = string.Join(",", classExamMarkList.Where(d => d.SubjectId == subjectId && d.TotalMark == maxMark).Select(d => d.ClassName).ToArray());
                    }
                    lst.Add(model);
                }

                #endregion

                #endregion

                #region 导出
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = SetCellStyle(hssfworkbook);

                var sheetName = year.ExamName + "最高分花名册";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                //表头
                ICell cell = cellHeader.CreateCell(0);
                cell.SetCellValue("科目");
                cell.CellStyle = cellstyle;

                cell = cellHeader.CreateCell(1);
                cell.SetCellValue("分数");
                cell.CellStyle = cellstyle;

                cell = cellHeader.CreateCell(2);
                cell.SetCellValue("姓名");
                cell.CellStyle = cellstyle;

                cell = cellHeader.CreateCell(3);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;

                rowStartIndex++;
                //数据行
                foreach (var t in lst)
                {
                    cellHeader = sheet1.CreateRow(rowStartIndex);

                    cell = cellHeader.CreateCell(0);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(t.SubjectName);

                    cell = cellHeader.CreateCell(1);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(1, 15 * 256);
                    cell.SetCellValue(t.TotalMark.ToString());

                    cell = cellHeader.CreateCell(2);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(2, 45 * 256);
                    cell.SetCellValue(t.StudentName);

                    cell = cellHeader.CreateCell(3);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(3, 45 * 256);
                    cell.SetCellValue(t.ClassName);
                    rowStartIndex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("单科最高分报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportClassStudentTop(int examId, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(searchText) || searchText == null)
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

                SubjectList.Add(new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });

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
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tbclassList = (from p in classStudent
                                   select new
                                   {
                                       p.ClassId,
                                       p.ClassName
                                   }).Distinct().ToList();

                var ClassList = (from p in tbclassList
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Value = p.ClassId.ToString(),
                                     Text = p.ClassName,
                                 }).ToList();

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                var classExamMarkList = (from p in examMarkList
                                         join t in classStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();
                #region  单个班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           MaxMark = g.Max(d => d.TotalMark),
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
                                                g.Key.StudentId,
                                                SubjectId = 0,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();

                var lst = new List<Dto.ExamAnalyze.List>();
                foreach (var c in ClassList)
                {
                    foreach (var s in SubjectList)
                    {
                        var model = new Dto.ExamAnalyze.List();
                        var subjectId = s.Value.ConvertToInt();
                        var classId = c.Value.ConvertToInt();
                        if (subjectId == 0)//总分
                        {
                            model.ClassId = classId;
                            model.SubjectId = subjectId;
                            //总分最高分学生
                            var MaxTotalMark = totalStudentMarkList.Where(d => d.ClassId == classId && d.SubjectId == subjectId).OrderByDescending(d => d.StudentTotalMark).FirstOrDefault();
                            var studentTotalMark = MaxTotalMark != null ? MaxTotalMark.StudentTotalMark : null;
                            model.TotalMark = studentTotalMark;
                            var studentIds = totalStudentMarkList.Where(d => d.ClassId == classId && d.SubjectId == subjectId && d.StudentTotalMark == studentTotalMark).Select(d => d.StudentId).Distinct().ToList();
                            model.StudentName = string.Join(",", classExamMarkList.Where(d => studentIds.Contains(d.StudentId)).Select(d => d.StudentName).Distinct().ToArray());
                        }
                        else
                        {
                            model.ClassId = classId;
                            model.SubjectId = subjectId;
                            var maxMark = subjectMarkList.Where(d => d.ClassId == classId && d.SubjectId == subjectId).Select(d => d.MaxMark).FirstOrDefault();
                            model.TotalMark = maxMark;
                            model.StudentName = string.Join(",", classExamMarkList.Where(d => d.ClassId == classId && d.SubjectId == subjectId && d.TotalMark == maxMark).Select(d => d.StudentName).Distinct().ToArray());
                        }
                        lst.Add(model);
                    }
                }

                #endregion

                #region  年级各科目总分最高分
                var gradeSubjectMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.SubjectId
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                SubjectId = g.Key.SubjectId,
                                                MaxMark = g.Max(d => d.TotalMark),
                                            }).ToList();
                //班级学生总成绩
                var gradeTotalStudentMarkList = (from p in classExamMarkList
                                                 group p by new
                                                 {
                                                     p.StudentId
                                                 } into g
                                                 select new
                                                 {
                                                     g.Key.StudentId,
                                                     SubjectId = 0,
                                                     StudentTotalMark = g.Sum(d => d.TotalMark)
                                                 }).ToList();

                var gradeMaxTotalMark = gradeTotalStudentMarkList.OrderByDescending(d => d.StudentTotalMark).FirstOrDefault();

                var lstGrade = new List<Dto.ExamAnalyze.List>();
                foreach (var s in SubjectList)
                {
                    var model = new Dto.ExamAnalyze.List();
                    var subjectId = s.Value.ConvertToInt();
                    if (subjectId == 0)//总分
                    {
                        model.ClassId = 0;
                        model.SubjectId = subjectId;
                        var studentTotalMark = gradeMaxTotalMark != null ? gradeMaxTotalMark.StudentTotalMark : null;
                        model.TotalMark = studentTotalMark;
                        var studentIds = gradeTotalStudentMarkList.Where(d => d.SubjectId == subjectId && d.StudentTotalMark == studentTotalMark).Select(d => d.StudentId).Distinct().ToList();
                        model.StudentName = string.Join(",", classExamMarkList.Where(d => studentIds.Contains(d.StudentId)).Select(d => d.StudentName).Distinct().ToArray());
                        model.ClassName = string.Join(",", classExamMarkList.Where(d => studentIds.Contains(d.StudentId)).Select(d => d.ClassName).Distinct().ToArray());
                    }
                    else
                    {
                        model.ClassId = 0;
                        model.SubjectId = subjectId;
                        var maxMark = gradeSubjectMarkList.Where(d => d.SubjectId == subjectId).Select(d => d.MaxMark).FirstOrDefault();
                        model.TotalMark = maxMark;
                        model.StudentName = string.Join(",", classExamMarkList.Where(d => d.SubjectId == subjectId && d.TotalMark == maxMark).Select(d => d.StudentName).Distinct().ToArray());
                        model.ClassName = string.Join(",", classExamMarkList.Where(d => d.SubjectId == subjectId && d.TotalMark == maxMark).Select(d => d.ClassName).ToArray());
                    }
                    lstGrade.Add(model);
                }

                #endregion

                #endregion

                #region 导出
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = SetCellStyle(hssfworkbook);

                var sheetName = year.ExamName + "各班各科最高分花名册";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                //表头
                ICell cell = cellHeader.CreateCell(0);
                cellHeader.Height = 20 * 20;
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                var No = 0;
                foreach (var s in SubjectList)
                {
                    cell = cellHeader.CreateCell(No + 1);
                    cell.SetCellValue(s.Text);
                    cell.CellStyle = cellstyle;
                    No++;
                }
                rowStartIndex++;
                //数据行
                foreach (var a in ClassList)
                {
                    cellHeader = sheet1.CreateRow(rowStartIndex);
                    cellHeader.Height = 30 * 20;
                    cell = cellHeader.CreateCell(0);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(a.Text);

                    No = 0;
                    foreach (var subject in SubjectList)
                    {
                        var mark = lst.Where(d => d.ClassId == a.Value.ConvertToInt() && d.SubjectId == subject.Value.ConvertToInt()).FirstOrDefault();
                        if (mark != null)
                        {
                            cell = cellHeader.CreateCell(No + 1);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 1, 35 * 256);
                            cell.SetCellValue(mark.StudentName + "\r\n" + mark.TotalMark);
                            No++;
                        }
                        else
                        {
                            cell = cellHeader.CreateCell(No + 1);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 1, 35 * 256);
                            cell.SetCellValue(string.Empty);
                            No++;
                        }
                    }
                    rowStartIndex++;
                }

                cellHeader = sheet1.CreateRow(rowStartIndex);
                cellHeader.Height = 30 * 20;
                cell = cellHeader.CreateCell(0);
                cell.CellStyle = cellstyle;
                sheet1.SetColumnWidth(0, 15 * 256);
                cell.SetCellValue("年级");

                No = 0;
                foreach (var subject in SubjectList)
                {
                    var mark = lstGrade.Where(d => d.SubjectId == subject.Value.ConvertToInt()).FirstOrDefault();
                    if (mark != null)
                    {
                        cell = cellHeader.CreateCell(No + 1);
                        cell.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(No + 1, 35 * 256);
                        var className = !string.IsNullOrEmpty(mark.ClassName) ? "(" + mark.ClassName + ")" : string.Empty;
                        cell.SetCellValue(mark.StudentName + "\r\n" + className + mark.TotalMark);
                        No++;
                    }
                    else
                    {
                        cell = cellHeader.CreateCell(No + 1);
                        cell.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(No + 1, 35 * 256);
                        cell.SetCellValue(string.Empty);
                        No++;
                    }
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("各班各科最高分报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

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
                //考试科目
                var examSubjectCourseList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id,
                                           SubjectNo=p.tbCourse.tbSubject.No,
                                           p.FullTotalMark
                                       }).OrderBy(d=>d.SubjectNo).Distinct().ToList();

                var examSubjectList=(from p in examSubjectCourseList
                                   orderby p.SubjectNo
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.SubjectId
                                   }).Distinct().ToList();

                var SubjectList = (from p in examSubjectList
                                   select new System.Web.Mvc.SelectListItem
                                   {
                                       Text = p.SubjectName,
                                       Value = p.SubjectId.ToString()
                                   }).Distinct().ToList();

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

        public ActionResult ExportClassMarkTopN(int examId, int gradeId, string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();
                #region 数据统计
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
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          CiassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList();

                if (chkClass == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList = chkClass.Split(',');

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

                selctClassList.Add(new System.Web.Mvc.SelectListItem { Text = "年级", Value = "0" });

                //年级学生班级
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();
                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                var classExamMarkList = (from p in examMarkList
                                         join t in selectClassStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();

                #region 年级班级科目成绩
                //班级学生总成绩
                var lstRank = (from p in classExamMarkList
                               group p by new
                               {
                                   p.StudentId,
                                   p.StudentName,
                                   p.ClassId,
                                   p.ClassName
                               } into g
                               select new Dto.ExamAnalyze.List
                               {
                                   StudentId = g.Key.StudentId,
                                   StudentName = g.Key.StudentName,
                                   ClassId = g.Key.ClassId,
                                   ClassName = g.Key.ClassName,
                                   TotalMark = g.Sum(d => d.TotalMark),
                                   GradeRank = 0,
                                   ClassRank = 0
                               }).ToList();
                //排名
                var rank = 0;
                decimal? mark = null;
                var count = 1;
                foreach (var t in lstRank.OrderByDescending(d => d.TotalMark))
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
                    var tt = (from p in lstRank
                              where p.StudentId == t.StudentId
                              select p).FirstOrDefault();
                    if (tt != null)
                    {
                        tt.GradeRank = rank;
                    }
                }

                foreach (var c in tbselctClassList)
                {
                    rank = 0;
                    mark = null;
                    count = 1;
                    foreach (var t in lstRank.Where(d => d.ClassId == c.Value.ConvertToInt()).OrderByDescending(d => d.TotalMark))
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
                        var tt = (from p in lstRank.Where(d => d.ClassId == c.Value.ConvertToInt())
                                  where p.StudentId == t.StudentId
                                  select p).FirstOrDefault();
                        if (tt != null)
                        {
                            tt.ClassRank = rank;
                        }
                    }
                }

                #endregion
                #endregion
                #region 导出
                //分数项目
                var RankList = new List<SelectListItem>()
                             {
                                new System.Web.Mvc.SelectListItem { Text = "第一名", Value ="1" },
                                new System.Web.Mvc.SelectListItem { Text = "第二名", Value = "2" },
                                new System.Web.Mvc.SelectListItem { Text = "第三名", Value = "3" },
                                new System.Web.Mvc.SelectListItem { Text = "第四名", Value = "4" },
                                new System.Web.Mvc.SelectListItem { Text = "第五名", Value = "5" },
                                new System.Web.Mvc.SelectListItem { Text = "第六名", Value = "6" },
                                new System.Web.Mvc.SelectListItem { Text = "第七名", Value = "7" },
                                new System.Web.Mvc.SelectListItem { Text = "第八名", Value = "8" },
                                new System.Web.Mvc.SelectListItem { Text = "第九名", Value = "9" },
                                new System.Web.Mvc.SelectListItem { Text = "第十名", Value = "10" }
                              };
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = SetCellStyle(hssfworkbook);

                var sheetName = year.ExamName + "年级班级成绩前十名册";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;

                IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                //表头
                ICell cell = cellHeader.CreateCell(0);
                cell.SetCellValue(sheetName);
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, 0, selctClassList.Count());
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);

                //第二行
                rowStartIndex++;
                cellHeader = sheet1.CreateRow(rowStartIndex);
                cell = cellHeader.CreateCell(0);
                sheet1.SetColumnWidth(0, 15 * 256);
                cell.SetCellValue("名次");
                cell.CellStyle = cellstyle;
                var No = 0;
                foreach (var s in selctClassList)
                {
                    cell = cellHeader.CreateCell(No + 1);
                    sheet1.SetColumnWidth(No + 1, 15 * 256);
                    cell.SetCellValue(s.Text);
                    cell.CellStyle = cellstyle;
                    No++;
                }

                //数据行
                foreach (var c in RankList)
                {
                    cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                    No = 0;
                    cell = cellHeader.CreateCell(No);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(c.Text);
                    foreach (var a in selctClassList)
                    {
                        var classId = a.Value.ConvertToInt();
                        if (classId != 0)
                        {
                            var lst = lstRank.Where(d => d.ClassId == classId && d.ClassRank == c.Value.ConvertToInt()).ToList();
                            var totalMark = lst.Select(d => d.TotalMark).FirstOrDefault();
                            var studentName = string.Join(",", lst.Select(d => d.StudentName).Distinct().ToArray());
                            cell = cellHeader.CreateCell(No + 1);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(0, 15 * 256);
                            cell.SetCellValue(studentName + "\r\n" + totalMark);
                        }
                        else
                        {
                            var lst = lstRank.Where(d => d.GradeRank == c.Value.ConvertToInt()).ToList();
                            var totalMark = lst.Select(d => d.TotalMark).FirstOrDefault();
                            var studentName = string.Join(",", lst.Select(d => d.StudentName).Distinct().ToArray());
                            cell = cellHeader.CreateCell(No + 1);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(0, 15 * 256);
                            cell.SetCellValue(studentName + "\r\n" + totalMark);

                        }
                        No++;
                    }
                    rowStartIndex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode(sheetName + DateTime.Now.ToString("yyyyMMdd") + ".xls"));
                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportClassSubjectMark(int examId, int gradeId, string chkSubject, string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(searchText) || searchText == null)
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id,
                                           SubjectNo=p.tbCourse.tbSubject.No
                                       }).OrderBy(d=>d.SubjectNo).Distinct().ToList();

                var SubjectList = (from p in examSubjectList
                                   select new System.Web.Mvc.SelectListItem
                                   {
                                       Text = p.SubjectName,
                                       Value = p.SubjectId.ToString()
                                   }).ToList();

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
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          CiassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList(); ;

                var ClassList = (from p in tbClassStudent
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Value = p.CiassId.ToString(),
                                     Text = p.ClassName,
                                 }).Distinct().ToList();

                if (chkClass == null || chkSubject == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList = chkClass.Split(',');
                var chksubjectList = chkSubject.Split(',');

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

                selctClassList.Add(new System.Web.Mvc.SelectListItem { Text = "年级", Value = "0" });

                var tbselectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                           where chksubjectList.Contains(p.Id.ToString())
                                           orderby p.No, p.SubjectName
                                           select new
                                           {
                                               p.Id,
                                               p.SubjectName,
                                           }).Distinct().ToList();

                var selectSubjectList = (from p in tbselectSubjectList
                                         select new System.Web.Mvc.SelectListItem
                                         {
                                             Value = p.Id.ToString(),
                                             Text = p.SubjectName,
                                         }).Distinct().ToList();

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject)
                                       where p.tbGrade.Id == gradeId && p.IsTotal == false
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
                                           SubjectId = p.tbSubject != null ? p.tbSubject.Id : 0
                                       }).ToList();

                //年级学生班级
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.TotalMark != null
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                var classExamMarkList = (from p in examMarkList
                                         join t in selectClassStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();

                #region 各班级科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           StudentCount = g.Count().ToString(),
                                           MaxMark = g.Max(d => d.TotalMark),
                                           AvgMark = g.Average(d => d.TotalMark)
                                       }).ToList();

                var gradeAvgMarkList = (from p in classExamMarkList
                                        group p by new
                                        {
                                            p.SubjectId
                                        } into g
                                        select new Exam.Dto.ExamAnalyze.List
                                        {
                                            SubjectId = g.Key.SubjectId,
                                            AvgMark = Math.Round(g.Average(d => d.TotalMark).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                        }).ToList();

                //当前考试：某科在班级中的标准差
                var lstDiff = new List<Exam.Dto.ExamAnalyze.List>();
                var StandardMark = decimal.Zero;
                double standardGap = 0;
                foreach (var c in tbselctClassList)
                {
                    foreach (var s in selectSubjectList)
                    {
                        StandardMark = decimal.Zero;
                        standardGap = 0;
                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.ClassId = c.Value.ConvertToInt();
                        var classMark = classExamMarkList.Where(d => d.ClassId == c.Value.ConvertToInt() && d.SubjectId == s.Value.ConvertToInt()).ToList();
                        var gradeAvg = gradeAvgMarkList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Select(d => d.AvgMark).FirstOrDefault();
                        foreach (var o in classMark)
                        {
                            var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                            var sd = (o.TotalMark - avgMark) * (o.TotalMark - avgMark);
                            StandardMark += sd.ConvertToDecimal();
                        }
                        if (classMark.Count > decimal.Zero)
                        {
                            standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / classMark.Count);
                        }
                        model.SubjectId = s.Value.ConvertToInt();
                        model.StandardDiff = standardGap;
                        lstDiff.Add(model);
                    }
                }
                var lstSegment = new List<Exam.Dto.ExamAnalyze.List>();
                //优秀及格率
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;
                    var isNormal = o.IsNormal;
                    //优秀科目人数
                    if (isGood)
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

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                    if (isNormal)//良好人数
                    {
                        //分数段人数
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = 2,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                    if (isPass)//及格人数
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

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = 3,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                }

                var tk = (from p in lstSegment
                          group p by new
                          {
                              p.ClassId,
                              p.SubjectId,
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.ClassId,
                              g.Key.SubjectId,
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum)
                          }).ToList();


                var ExamAnalyzeList = (from p in subjectMarkList
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           SubjectId = p.SubjectId,
                                           AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                           MaxMark = p.MaxMark,
                                           StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                           GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                           PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                       }).ToList();


                #endregion

                #region  年级成绩
                var gradeSubjectMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.SubjectId
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                SubjectId = g.Key.SubjectId,
                                                StudentCount = g.Count().ToString(),
                                                MaxMark = g.Max(d => d.TotalMark),
                                                AvgMark = g.Average(d => d.TotalMark)
                                            }).ToList();

                //当前考试：某科在班级中的标准差
                lstDiff = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var s in selectSubjectList)
                {
                    StandardMark = decimal.Zero;
                    standardGap = 0;
                    var gradeModel = new Exam.Dto.ExamAnalyze.List();
                    gradeModel.ClassId = 0;
                    var gradeMark = classExamMarkList.Where(d => d.SubjectId == s.Value.ConvertToInt()).ToList();
                    var gradeAvg = gradeSubjectMarkList.Where(d => d.SubjectId == s.Value.ConvertToInt()).Select(d => d.AvgMark).FirstOrDefault();
                    foreach (var o in gradeMark)
                    {
                        var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                        var sd = (o.TotalMark - avgMark) * (o.TotalMark - avgMark);
                        StandardMark += sd.ConvertToDecimal();
                    }
                    if (gradeMark.Count > decimal.Zero)
                    {
                        standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / gradeMark.Count);
                    }
                    gradeModel.SubjectId = s.Value.ConvertToInt();
                    gradeModel.StandardDiff = standardGap;
                    lstDiff.Add(gradeModel);
                }

                lstSegment = new List<Exam.Dto.ExamAnalyze.List>();
                //优秀及格率
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;
                    var isNormal = o.IsNormal;
                    //优秀科目人数
                    if (isGood)
                    {
                        //分数段人数
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.SubjectId
                                  } into g
                                  select new
                                  {
                                      g.Key.SubjectId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = 0,
                                      SubjectId = o.SubjectId,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                    if (isNormal)//良好人数
                    {
                        //分数段人数
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.SubjectId,
                                  } into g
                                  select new
                                  {
                                      g.Key.SubjectId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = 0,
                                      SubjectId = o.SubjectId,
                                      Status = 2,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                    if (isPass)//及格人数
                    {
                        //分数段人数
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.SubjectId
                                  } into g
                                  select new
                                  {
                                      g.Key.SubjectId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = 0,
                                      SubjectId = o.SubjectId,
                                      Status = 3,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lstSegment.AddRange(tb);
                    }
                }

                tk = (from p in lstSegment
                      group p by new
                      {
                          p.ClassId,
                          p.SubjectId,
                          p.Status
                      } into g
                      select new
                      {
                          g.Key.ClassId,
                          g.Key.SubjectId,
                          g.Key.Status,
                          StudentNum = g.Sum(d => d.StudentNum)
                      }).ToList();

                var garadeExamAnalyzeList = (from p in gradeSubjectMarkList
                                             select new Exam.Dto.ExamAnalyze.List
                                             {
                                                 ClassId = p.ClassId,
                                                 SubjectId = p.SubjectId,
                                                 AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                                 MaxMark = p.MaxMark,
                                                 StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                                 GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                 PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                             }).ToList();
                #endregion

                var lst = (from p in ExamAnalyzeList.Union(garadeExamAnalyzeList)
                           select new Exam.Dto.ExamAnalyze.List
                           {
                               ClassId = p.ClassId,
                               SubjectId = p.SubjectId,
                               StandardDiff = System.Math.Round(p.StandardDiff, 1, MidpointRounding.AwayFromZero),
                               AvgMark = p.AvgMark,
                               MaxMark = p.MaxMark,
                               GoodRate = p.GoodRate,
                               PassRate = p.PassRate
                           }).ToList();

                #endregion

                #region 导出
                var OptionList = new List<string>() { "平均分", "及格率(%)", "优秀率(%)", "标准差", "最高分", "平均相对分" };
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = SetCellStyle(hssfworkbook);

                var sheetName = year.ExamName + "各学科情况统计表";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                //表头
                ICell cell = cellHeader.CreateCell(0);
                cell.SetCellValue("科目");
                cell.CellStyle = cellstyle;

                cell = cellHeader.CreateCell(1);
                cell.SetCellValue("项目");
                cell.CellStyle = cellstyle;
                var No = 0;
                foreach (var c in selctClassList)
                {
                    cell = cellHeader.CreateCell(No + 2);
                    cell.SetCellValue(c.Text);
                    cell.CellStyle = cellstyle;
                    No++;
                }

                //数据行
                rowStartIndex++;
                foreach (var s in selectSubjectList)
                {
                    for (var i = 0; i < OptionList.Count; i++)
                    {
                        cellHeader = sheet1.CreateRow(rowStartIndex);
                        cell = cellHeader.CreateCell(0);
                        cell.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(0, 15 * 256);
                        cell.SetCellValue(s.Text);

                        cell = cellHeader.CreateCell(1);
                        cell.CellStyle = cellstyle;
                        sheet1.SetColumnWidth(1, 15 * 256);
                        cell.SetCellValue(OptionList[i]);

                        No = 0;
                        foreach (var c in selctClassList)
                        {
                            var mark = lst.Where(d => d.ClassId == c.Value.ConvertToInt() && d.SubjectId == s.Value.ConvertToInt()).FirstOrDefault();
                            var gradeAvgMark = lst.Where(d => d.ClassId == 0 && d.SubjectId == s.Value.ConvertToInt()).FirstOrDefault();
                            if (mark != null)
                            {
                                var AvgRelative = mark.AvgMark - gradeAvgMark.AvgMark;
                                switch (i)
                                {
                                    case 0:
                                        cell = cellHeader.CreateCell(No + 2);
                                        cell.CellStyle = cellstyle;
                                        cell.SetCellValue(mark.AvgMark.ToString());
                                        No++;
                                        break;
                                    case 1:
                                        cell = cellHeader.CreateCell(No + 2);
                                        cell.CellStyle = cellstyle;
                                        cell.SetCellValue(mark.PassRate.ToString());
                                        No++;
                                        break;
                                    case 2:
                                        cell = cellHeader.CreateCell(No + 2);
                                        cell.CellStyle = cellstyle;
                                        cell.SetCellValue(mark.GoodRate.ToString());
                                        No++;
                                        break;
                                    case 3:
                                        cell = cellHeader.CreateCell(No + 2);
                                        cell.CellStyle = cellstyle;
                                        cell.SetCellValue(mark.StandardDiff.ToString());
                                        No++;
                                        break;
                                    case 4:
                                        cell = cellHeader.CreateCell(No + 2);
                                        cell.CellStyle = cellstyle;
                                        cell.SetCellValue(mark.MaxMark.ToString());
                                        No++;
                                        break;
                                    case 5:
                                        cell = cellHeader.CreateCell(No + 2);
                                        cell.CellStyle = cellstyle;
                                        cell.SetCellValue(AvgRelative.ToString());
                                        No++;
                                        break;
                                }
                            }
                            else
                            {
                                cell = cellHeader.CreateCell(No + 2);
                                cell.CellStyle = cellstyle;
                                cell.SetCellValue(string.Empty);
                                No++;
                            }
                        }
                        rowStartIndex++;
                    }
                }

                //合并数据第一行格式重新创建
                var oldvalue = sheet1.GetRow(1).GetCell(0).StringCellValue;
                sheet1.GetRow(1).CreateCell(0).SetCellValue(oldvalue);
                var cellRangeAddress = new CellRangeAddress(1, OptionList.Count(), 0, 0);
                sheet1.AddMergedRegion(cellRangeAddress);
                sheet1.GetRow(1).GetCell(0).CellStyle = cellstyle;
                //以下放后面避免第一个科目合并导致单元格样式问题
                this.MergeColumn(hssfworkbook, 0, 1);

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("各学科情况统计表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportTotalScoreAdvance(int examId, int lastexamId, int gradeId, string chkClass, string chkSubject, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
                var vm = new Models.ExamAnalyze.List();

                //获取本次考试信息
                var bExam = (from p in db.Table<Exam.Entity.tbExam>()
                             where p.IsDeleted == false
                             && p.Id == examId
                             select new Exam.Dto.ExamAnalyze.List
                             {
                                 ExamId = p.Id,
                                 ExamName = p.ExamName,
                             }).FirstOrDefault();
                vm.ExamTotalList.Add(bExam);

                //获取上次考试信息
                var sExam = (from p in db.Table<Exam.Entity.tbExam>()
                             where p.IsDeleted == false
                             && p.Id == lastexamId
                             select new Exam.Dto.ExamAnalyze.List
                             {
                                 LastExamId = p.Id,
                                 ExamName = p.ExamName,
                             }).FirstOrDefault();
                vm.ExamTotalList.Add(sExam);

                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
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
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName,
                                        p.tbStudent.StudentCode,
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          CiassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList(); ;

                var ClassList = (from p in tbClassStudent
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Value = p.CiassId.ToString(),
                                     Text = p.ClassName,
                                 }).Distinct().ToList();

                if (chkClass == null || chkSubject == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList = chkClass.Split(',');
                var chksubjectList = chkSubject.Split(',');

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
                                               p.Id,
                                               p.SubjectName,
                                           }).Distinct().ToList();

                var selectSubjectList = (from p in tbselectSubjectList
                                         select new System.Web.Mvc.SelectListItem
                                         {
                                             Value = p.Id.ToString(),
                                             Text = p.SubjectName,
                                         }).Distinct().ToList();

                //年级学生班级
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                //上次考试考试
                var examLastMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                        where p.tbExamCourse.IsDeleted == false
                                         && p.tbStudent.IsDeleted == false
                                         && p.tbExamCourse.tbExam.Id == lastexamId
                                         && p.tbExamCourse.tbCourse.IsDeleted == false
                                         && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                        select new
                                        {
                                            p.TotalMark,
                                            ExamId = p.tbExamCourse.tbExam.Id,
                                            StudentId = p.tbStudent.Id,
                                            SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                        }).ToList();

                #region  成绩分析
                var lst = new List<Dto.ExamAnalyze.List>();
                #region 本次考试
                var classExamMarkList = (from p in examMarkList
                                         join t in selectClassStudent
                                         on p.StudentId equals t.StudentId
                                         where (t.StudentName.Contains(vm.SearchText) || vm.SearchText == null)
                                         select new
                                         {
                                             ExamId = p.ExamId,
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();

                //班级学生总成绩
                var totalStudentMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.StudentId,
                                                p.ExamId,
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                ExamId = g.Key.ExamId,
                                                StudentId = g.Key.StudentId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark),
                                                GradeRank = 0,
                                            }).ToList();

                //年级排名
                var gradeRank = decimal.Zero;
                decimal? gradeMark = null;
                var gradeCount = decimal.One;
                foreach (var t in totalStudentMarkList.OrderByDescending(d => d.StudentTotalMark))
                {
                    if (gradeMark != t.StudentTotalMark)
                    {
                        gradeMark = t.StudentTotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.GradeRank = gradeRank;
                }
                #endregion

                #region 上次考试
                var classLastExamMarkList = (from p in examLastMarkList
                                             join t in selectClassStudent
                                             on p.StudentId equals t.StudentId
                                             where (t.StudentName.Contains(vm.SearchText) || vm.SearchText == null)
                                             select new
                                             {
                                                 ExamId = p.ExamId,
                                                 StudentId = p.StudentId,
                                                 StudentName = t.StudentName,
                                                 SubjectId = p.SubjectId,
                                                 TotalMark = p.TotalMark,
                                                 ClassId = t.ClassId,
                                                 ClassName = t.ClassName,
                                             }).ToList();

                #region 获取学生成绩总分
                //班级学生总成绩
                var totalLastStudentMarkList = (from p in classLastExamMarkList
                                                group p by new
                                                {
                                                    p.StudentId,
                                                    p.ExamId,
                                                } into g
                                                select new Exam.Dto.ExamAnalyze.List
                                                {
                                                    ExamId = g.Key.ExamId,
                                                    StudentId = g.Key.StudentId,
                                                    StudentTotalMark = g.Sum(d => d.TotalMark),
                                                    GradeRank = 0,
                                                }).ToList();

                //年级排名
                gradeRank = decimal.Zero;
                gradeMark = null;
                gradeCount = decimal.One;
                foreach (var t in totalLastStudentMarkList.OrderByDescending(d => d.StudentTotalMark))
                {
                    if (gradeMark != t.StudentTotalMark)
                    {
                        gradeMark = t.StudentTotalMark;
                        gradeRank = gradeRank + gradeCount;
                        gradeCount = decimal.One;
                    }
                    else
                    {
                        gradeCount = gradeCount + decimal.One;
                    }

                    t.GradeRank = gradeRank;
                }

                var result = (from p in classStudent
                              join t in totalStudentMarkList on p.StudentId equals t.StudentId
                              join c in totalLastStudentMarkList on p.StudentId equals c.StudentId
                              where chkclassList.Contains(p.ClassId.ToString())
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ExamId = t.ExamId,
                                  LastExamId = c.ExamId,
                                  StudentId = t.StudentId,
                                  StudentCode = p.StudentCode,
                                  StudentName = p.StudentName,
                                  StudentTotalMark = t.StudentTotalMark,
                                  StudentLastTotalMark = c.StudentTotalMark,
                                  GradeRank = t.GradeRank,
                                  GradeLastRank = c.GradeRank,
                                  ClassName = p.ClassName,
                              }).ToList();
                foreach (var r in result)
                {
                    r.GradeAdvanceRank = r.GradeRank != null ? (r.GradeLastRank != null ? r.GradeRank.Value - r.GradeLastRank.Value : r.GradeRank.Value) : (r.GradeLastRank != null ? 0 - r.GradeLastRank.Value : 0);
                }
                #endregion

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

                //var sheetName = "总分对比名次进退";
                var sheetName = (bExam != null ? bExam.ExamName + "与" : string.Empty) + (sExam != null ? sExam.ExamName : string.Empty) + "对比名次进退";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                //表头
                int rowindex = 0;
                int cellindex = 0;
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(sheetName);
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 3 + vm.ExamTotalList.Count() * 2 + 1);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                rowindex++;

                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("序号");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("学号");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("姓名");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                foreach (var exam in vm.ExamTotalList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(exam.ExamName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 2;
                }
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("名次进退");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);

                rowindex++;
                cellindex = 4;
                row = sheet1.CreateRow(rowindex);
                foreach (var exam in vm.ExamTotalList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("总分");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("名次");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                }

                rowindex = 3;
                var j = 1;
                foreach (var r in result)
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(j);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(r.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(r.StudentCode);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(r.StudentName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    foreach (var exam in vm.ExamTotalList)
                    {
                        if (r.ExamId == exam.ExamId)
                        {
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(r.StudentTotalMark.ToString());
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(r.GradeRank.ToString());
                            cell.CellStyle = cellstyle;
                            cellindex++;
                        }
                        else if (r.LastExamId == exam.LastExamId)
                        {
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(r.StudentLastTotalMark.ToString());
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(r.GradeLastRank.ToString());
                            cell.CellStyle = cellstyle;
                            cellindex++;
                        }
                        else
                        {
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue("0");
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue("0");
                            cell.CellStyle = cellstyle;
                            cellindex++;
                        }
                    }
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue((r.GradeAdvanceRank != null ? r.GradeAdvanceRank.ToString() : "0"));
                    cell.CellStyle = cellstyle;
                    rowindex++;
                    j++;
                }

                //前四列表头增加样式
                cellindex = 0;
                for (var i = 0; i < 4; i++)
                {
                    sheet1.GetRow(2).CreateCell(i).SetCellValue(string.Empty);
                    sheet1.GetRow(2).GetCell(i).CellStyle = cellstyle;
                }
                //增加名次进退样式
                cellindex = 3 + vm.ExamTotalList.Count() * 2 + 1;
                sheet1.GetRow(2).CreateCell(cellindex).SetCellValue(string.Empty);
                sheet1.GetRow(2).GetCell(cellindex).CellStyle = cellstyle;

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode(sheetName + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportTotalSegment(int examId, int gradeId, string chkClass, string chkSubject, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
                var vm = new Models.ExamAnalyze.List();

                //获取本次考试信息
                var bExam = (from p in db.Table<Exam.Entity.tbExam>()
                             where p.IsDeleted == false
                             && p.Id == examId
                             select new Exam.Dto.ExamAnalyze.List
                             {
                                 ExamId = p.Id,
                                 ExamName = p.ExamName,
                             }).FirstOrDefault();
                vm.ExamTotalList.Add(bExam);

                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(searchText) || searchText == null)
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
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName,
                                        p.tbStudent.StudentCode,
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          CiassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList(); ;

                var ClassList = (from p in tbClassStudent
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Value = p.CiassId.ToString(),
                                     Text = p.ClassName,
                                 }).Distinct().ToList();

                if (chkClass == null || chkSubject == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList = chkClass.Split(',');
                var chksubjectList = chkSubject.Split(',');

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
                                               p.Id,
                                               p.SubjectName,
                                           }).Distinct().ToList();

                var selectSubjectList = (from p in tbselectSubjectList
                                         select new System.Web.Mvc.SelectListItem
                                         {
                                             Value = p.Id.ToString(),
                                             Text = p.SubjectName,
                                         }).Distinct().ToList();

                //年级学生班级
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();

                var selectStudentIds = selectClassStudent.Select(d => d.StudentId).Distinct().ToList();

                //获取课程ID
                var courseIds = (from p in db.Table<Course.Entity.tbOrg>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.IsDeleted == false
                                  && p.tbGrade.Id == gradeId
                                  && p.tbCourse.IsDeleted == false
                                  && p.tbYear.Id == year.Id
                                  && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
                                 select p.tbCourse).Select(d => d.Id).Distinct().ToList();
                //获取考试各科满分总分
                vm.MaxTotalMark = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.IsDeleted == false
                                    && p.tbExam.Id == examId
                                    && p.tbCourse.IsDeleted == false
                                    && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
                                    && courseIds.Contains(p.tbCourse.Id)
                                   select p).Distinct().Sum(d => d.FullTotalMark);

                //考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                //获取学生总分
                vm.ExamAnalyzeList = (from p in examMarkList
                                      join t in selectClassStudent
                                      on p.StudentId equals t.StudentId
                                      group p by new { t.ClassId, t.StudentId } into g
                                      select new Dto.ExamAnalyze.List
                                      {
                                          TotalMark = g.Sum(d => d.TotalMark),
                                          ClassId = g.Key.ClassId,
                                          StudentId = g.Key.StudentId,
                                      }).ToList();

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

                //var sheetName = "总分细化段";
                var sheetName = "总分细化段";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                //表头
                int rowindex = 0;
                int cellindex = 0;
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(sheetName);
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 1 + selctClassList.Count() + 1);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                rowindex++;

                row = sheet1.CreateRow(rowindex);
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("分数段");
                cell.CellStyle = cellstyle;
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("段均分");
                cell.CellStyle = cellstyle;
                cellindex++;
                foreach (var selctClass in selctClassList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(selctClass.Text);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                }
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("年级");
                cell.CellStyle = cellstyle;
                cellindex++;

                rowindex++;
                var fractional = searchText.ConvertToDecimal();
                if (fractional > 0)
                {
                    decimal Begin = 0;
                    decimal End = 0;
                    for (decimal i = 1; i <= (int)Math.Ceiling(vm.MaxTotalMark / fractional); i++)
                    {
                        if (i == 1)
                        {
                            Begin = vm.MaxTotalMark;
                            End = vm.MaxTotalMark - fractional * i;
                        }
                        else
                        {
                            Begin = vm.MaxTotalMark - fractional * (i - 1) - 1 + ("0.99").ConvertToDecimal();
                            End = (Begin < fractional ? ("0.00").ConvertToDecimal() : vm.MaxTotalMark - fractional * i);
                        }
                        var rankTotalList = vm.ExamAnalyzeList.Where(d => End <= d.TotalMark && d.TotalMark <= Begin).ToList();
                        var avg = rankTotalList.Average(d => d.TotalMark);
                        row = sheet1.CreateRow(rowindex);
                        cellindex = 0;
                        row = sheet1.CreateRow(rowindex);
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(Begin.ToString() + "~" + End.ToString());
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue((avg != null ? avg.Value.ToString("0.00") : "0.00"));
                        cell.CellStyle = cellstyle;
                        cellindex++;

                        var gradeTotal = 0;
                        foreach (var selctClass in selctClassList)
                        {
                            var studentCount = rankTotalList.Where(d => d.ClassId.ToString() == selctClass.Value).ToList();
                            gradeTotal += studentCount.Count();
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(studentCount.Count() > 0 ? studentCount.Count().ToString() : "");
                            cell.CellStyle = cellstyle;
                            cellindex++;
                        }
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(gradeTotal > 0 ? gradeTotal.ToString() : "");
                        cell.CellStyle = cellstyle;

                        rowindex++;
                    }
                }


                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode(sheetName + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportCompMarkReport(int examId, int gradeId, string chkClass, string chkSubject, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计

                //考试科目
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == examId
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                        && (p.tbCourse.tbSubject.SubjectName.Contains(searchText) || searchText == null)
                                       orderby p.tbCourse.tbSubject.No
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id,
                                           CourseId = p.tbCourse.Id
                                       }).Distinct().ToList();
                var examSubject = (from p in examSubjectList
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.SubjectId
                                   }).Distinct().ToList();

                var SubjectList = (from p in examSubject
                                   select new System.Web.Mvc.SelectListItem
                                   {
                                       Text = p.SubjectName,
                                       Value = p.SubjectId.ToString()
                                   }).ToList();

                SubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });
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

                //任课老师
                var courseIds = examSubjectList.Select(d => d.CourseId).Distinct().ToList();
                var orgTeacherList = (from p in db.Table<Course.Entity.tbOrgTeacher>()
                                     .Include(d => d.tbOrg.tbClass)
                                      where p.tbOrg.IsDeleted == false
                                      && courseIds.Contains(p.tbOrg.tbCourse.Id)
                                      && p.tbOrg.tbYear.Id == year.Id
                                      && p.tbTeacher.IsDeleted == false
                                      && p.tbOrg.tbCourse.IsDeleted == false
                                      && p.tbOrg.tbCourse.tbSubject.IsDeleted == false
                                      select new
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
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tbClassStudent = (from p in classStudent
                                      select new
                                      {
                                          CiassId = p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList(); ;

                var ClassList = (from p in tbClassStudent
                                 select new System.Web.Mvc.SelectListItem
                                 {
                                     Value = p.CiassId.ToString(),
                                     Text = p.ClassName,
                                 }).Distinct().ToList();

                if (chkClass == null || chkSubject == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList = chkClass.Split(',');
                var chksubjectList = chkSubject.Split(',');

                //选中班级和科目
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

                selctClassList.Add(new System.Web.Mvc.SelectListItem { Text = "年级", Value = "0" });

                var examSelectSubjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                             where chksubjectList.Contains(p.Id.ToString())
                                             orderby p.No, p.SubjectName
                                             select new
                                             {
                                                 p.Id,
                                                 p.SubjectName,
                                             }).Distinct().ToList();

                var selectSubjectList = (from p in examSelectSubjectList
                                         select new System.Web.Mvc.SelectListItem
                                         {
                                             Value = p.Id.ToString(),
                                             Text = p.SubjectName,
                                         }).Distinct().ToList();
                if (chksubjectList.Contains("0"))
                {
                    selectSubjectList.Insert(0, new System.Web.Mvc.SelectListItem { Text = "总分", Value = "0" });
                }

                //重要分数段
                var ImportSegmentMarkList = (from p in db.Table<Exam.Entity.tbExamImportSegmentMark>()
                                             where p.tbGrade.Id == gradeId
                                             orderby p.No
                                             select new
                                             {
                                                 SegmentId = p.Id,
                                                 p.SegmentName,
                                                 p.MinMark,
                                                 p.MaxMark,
                                             }).ToList();

                var ImortSegmentList = (from p in ImportSegmentMarkList
                                        select new Dto.ExamAnalyze.SegmentList
                                        {
                                            SegmentId = p.SegmentId,
                                            SegmentName = p.SegmentName
                                        }).ToList();

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>().Include(d => d.tbSubject)
                                       where p.tbGrade.Id == gradeId
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

                var SegmentList = (from p in SegmentMarkList
                                   select new Dto.ExamAnalyze.SegmentList
                                   {
                                       SegmentId = p.SegmentId,
                                       SegmentName = p.SegmentName,
                                       SubjectId = p.SubjectId,
                                       IsTotal = p.IsTotal
                                   }).ToList();

                //选中年级班级学生
                var selectClassStudent = (from p in classStudent
                                          where chkclassList.Contains(p.ClassId.ToString())
                                          orderby p.StudentId
                                          select new
                                          {
                                              ClassId = p.ClassId,
                                              p.ClassName,
                                              StudentId = p.StudentId,
                                              p.StudentName
                                          }).ToList();

                //本次考试
                var examMarkList = (from p in db.Table<Exam.Entity.tbExamMark>()
                                    where p.tbExamCourse.IsDeleted == false
                                     && p.tbStudent.IsDeleted == false
                                     && p.tbExamCourse.tbExam.Id == examId
                                     && p.tbExamCourse.tbCourse.IsDeleted == false
                                     && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                                     && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                                    orderby p.tbStudent.Id
                                    select new
                                    {
                                        p.TotalMark,
                                        ExamId = p.tbExamCourse.tbExam.Id,
                                        StudentId = p.tbStudent.Id,
                                        SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                                    }).ToList();

                var classExamMarkList = (from p in examMarkList
                                         join t in selectClassStudent
                                         on p.StudentId equals t.StudentId
                                         select new
                                         {
                                             StudentId = p.StudentId,
                                             StudentName = t.StudentName,
                                             SubjectId = p.SubjectId,
                                             TotalMark = p.TotalMark,
                                             ClassId = t.ClassId,
                                             ClassName = t.ClassName,
                                         }).ToList();

                #region 科目成绩
                //科目成绩
                var subjectMarkList = (from p in classExamMarkList
                                       group p by new
                                       {
                                           p.ClassId,
                                           p.SubjectId
                                       } into g
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ClassId = g.Key.ClassId,
                                           SubjectId = g.Key.SubjectId,
                                           StudentCount = g.Count().ToString(),//实考人数
                                           StudentNum = classStudent.Where(d => d.ClassId == g.Key.ClassId).Count(),//应考人数
                                           MaxMark = g.Max(d => d.TotalMark),
                                           MinMark = g.Min(d => d.TotalMark),
                                           AvgMark = g.Average(d => d.TotalMark)
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
                                                g.Key.StudentId,
                                                StudentTotalMark = g.Sum(d => d.TotalMark)
                                            }).ToList();
                var ty = (from p in totalStudentMarkList
                          group p by new
                          {
                              p.ClassId,
                          } into g
                          select new
                          {
                              g.Key.ClassId,
                              StudentCount = g.Count()
                          }).ToList();
                var totalMarkList = (from p in totalStudentMarkList
                                     group p by new { p.ClassId } into g
                                     select new Exam.Dto.ExamAnalyze.List
                                     {
                                         ClassId = g.Key.ClassId,
                                         StudentCount = ty.Where(d => d.ClassId == g.Key.ClassId).Select(d => d.StudentCount).FirstOrDefault().ToString(),
                                         StudentNum = classStudent.Where(d => d.ClassId == g.Key.ClassId).Count(),//应考人数
                                         AvgMark = g.Average(d => d.StudentTotalMark),
                                         MaxMark = g.Max(d => d.StudentTotalMark),
                                         MinMark = g.Min(d => d.StudentTotalMark)
                                     }).ToList();

                var totalGrdeAvg = totalStudentMarkList.Average(d => d.StudentTotalMark);


                //年级科目成绩
                var gradeSubjectMarkList = (from p in classExamMarkList
                                            group p by new
                                            {
                                                p.SubjectId
                                            } into g
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                SubjectId = g.Key.SubjectId,
                                                ClassId = 0,
                                                StudentNum = selectClassStudent.Count(),
                                                StudentCount = g.Count().ToString(),
                                                AvgMark = Math.Round(g.Average(d => d.TotalMark).ConvertToDecimal(), 1, MidpointRounding.AwayFromZero),
                                                MaxMark = g.Max(d => d.TotalMark),
                                                MinMark = g.Min(d => d.TotalMark)
                                            }).ToList();
                //年级总分
                var totalGradeStudentMarkList = (from p in classExamMarkList
                                                 group p by new
                                                 {
                                                     p.StudentId
                                                 } into g
                                                 select new
                                                 {
                                                     ClassId = 0,
                                                     SubjectId = 0,
                                                     g.Key.StudentId,
                                                     StudentTotalMark = g.Sum(d => d.TotalMark)
                                                 }).ToList();

                var totalGradeMarkList = (from p in totalGradeStudentMarkList
                                          group p by new { p.ClassId } into g
                                          select new Exam.Dto.ExamAnalyze.List
                                          {
                                              ClassId = g.Key.ClassId,
                                              StudentCount = totalGradeStudentMarkList.Where(d => d.ClassId == g.Key.ClassId).Count().ToString(),
                                              StudentNum = selectClassStudent.Count(),//应考总人数
                                              AvgMark = decimal.Round(g.Average(d => d.StudentTotalMark).ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                              MaxMark = g.Max(d => d.StudentTotalMark),
                                              MinMark = g.Min(d => d.StudentTotalMark)
                                          }).ToList();

                //当前考试：某科在班级中的标准差
                var lstDiff = new List<Exam.Dto.ExamAnalyze.List>();
                var StandardMark = decimal.Zero;
                double standardGap = 0;
                foreach (var c in tbselctClassList)
                {
                    foreach (var s in selectSubjectList)
                    {
                        StandardMark = decimal.Zero;
                        standardGap = 0;
                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.ClassId = c.Value.ConvertToInt();
                        var subjectId = s.Value.ConvertToInt();
                        if (subjectId != 0)
                        {
                            var classMark = classExamMarkList.Where(d => d.ClassId == c.Value.ConvertToInt() && d.SubjectId == subjectId).ToList();
                            var gradeAvg = gradeSubjectMarkList.Where(d => d.SubjectId == subjectId).Select(d => d.AvgMark).FirstOrDefault();
                            foreach (var o in classMark)
                            {
                                var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                                var sd = (o.TotalMark - avgMark) * (o.TotalMark - avgMark);
                                StandardMark += sd.ConvertToDecimal();
                            }
                            if (classMark.Count > decimal.Zero)
                            {
                                standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / classMark.Count);
                            }
                            model.SubjectId = subjectId;
                            model.StandardDiff = standardGap;
                            lstDiff.Add(model);
                        }
                        else //总分
                        {
                            var classMark = totalStudentMarkList.Where(d => d.ClassId == c.Value.ConvertToInt()).ToList();
                            var gradeAvg = totalGrdeAvg;
                            foreach (var o in classMark)
                            {
                                var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                                var sd = (o.StudentTotalMark - avgMark) * (o.StudentTotalMark - avgMark);
                                StandardMark += sd.ConvertToDecimal();
                            }
                            if (classMark.Count > decimal.Zero)
                            {
                                standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / classMark.Count);
                            }
                            model.SubjectId = subjectId;
                            model.StandardDiff = standardGap;
                            lstDiff.Add(model);
                        }
                    }
                }
                //良好及格优秀
                var lstSegment = new List<Exam.Dto.ExamAnalyze.List>();
                //各分数段人数
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                //优秀,良好及格率以及各分数段人数
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;
                    var isNormal = o.IsNormal;
                    var isTotal = o.IsTotal;
                    if (isTotal && o.SubjectId == 0)//总分
                    {
                        var tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count()
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SegmentId = o.SegmentId,
                                      SubjectId = 0,
                                      StudentNum = p.StudentCount
                                  }).ToList();
                        lst.AddRange(tb);

                        if (isGood)
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count()
                                  }).ToList();

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = 0,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count()
                                  }).ToList();

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = 0,
                                      Status = 2,
                                      StudentNum = p.StudentCount
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count()
                                  }).ToList();

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = 0,
                                      Status = 3,
                                      StudentNum = p.StudentCount
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                    }
                    else
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

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      SegmentId = o.SegmentId,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lst.AddRange(tb);
                        //优秀科目人数
                        if (isGood)
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
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

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      StudentCount = g.Count(),
                                  }).ToList();

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = 2,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
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

                            tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      SubjectId = o.SubjectId,
                                      Status = 3,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                            lstSegment.AddRange(tb);
                        }
                    }
                }
                //重要分数段人数
                var lstImport = new List<Dto.ExamAnalyze.List>();
                foreach (var o in ImportSegmentMarkList)
                {
                    var tm = (from p in totalStudentMarkList
                              where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                              group p by new
                              {
                                  p.ClassId
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  StudentCount = g.Count()
                              }).ToList();

                    var tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SegmentId = o.SegmentId,
                                  SubjectId = 0,
                                  StudentNum = p.StudentCount
                              }).ToList();
                    lstImport.AddRange(tb);

                    foreach (var subject in selectSubjectList)
                    {
                        tm = (from p in classExamMarkList
                              where p.SubjectId == subject.Value.ConvertToInt()
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

                        tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = subject.Value.ConvertToInt(),
                                  SegmentId = o.SegmentId,
                                  StudentNum = p.StudentCount,
                              }).ToList();
                        lstImport.AddRange(tb);
                    }
                }

                var tk = (from p in lstSegment
                          group p by new
                          {
                              p.ClassId,
                              p.SubjectId,
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.ClassId,
                              g.Key.SubjectId,
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum)
                          }).ToList();

                //各科目项目
                var examSubjectAnalyzeList = (from p in subjectMarkList
                                              select new
                                              {
                                                  ClassId = p.ClassId,
                                                  SubjectId = p.SubjectId,
                                                  StudentNum = p.StudentNum,
                                                  StudentCount = p.StudentCount,
                                                  AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                                  GradeAvg = gradeSubjectMarkList.Where(d => d.SubjectId == p.SubjectId).Select(d => d.AvgMark).FirstOrDefault(),
                                                  MaxMark = p.MaxMark,
                                                  MinMark = p.MinMark,
                                                  StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                                  GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                  NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                  PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                              }).ToList();

                var ExamAnalyzeList = (from p in examSubjectAnalyzeList
                                       select new Exam.Dto.ExamAnalyze.List
                                       {
                                           ClassId = p.ClassId,
                                           SubjectId = p.SubjectId,
                                           StudentNum = p.StudentNum,
                                           StudentCount = p.StudentCount,
                                           AvgMark = p.AvgMark,
                                           MaxMark = p.MaxMark,
                                           MinMark = p.MinMark,
                                           StandardDiff = p.StandardDiff,
                                           StandardMark = p.StandardDiff != 0 ? Math.Round(((p.AvgMark - p.GradeAvg) * 10 / (decimal)p.StandardDiff + 50).ConvertToDecimal(), 1, MidpointRounding.AwayFromZero) : 50,
                                           GoodRate = p.GoodRate,
                                           NormalRate = p.NormalRate,
                                           PassRate = p.PassRate,
                                       }).ToList();

                //总分项目
                var examTotalSubjectAnalyzeList = (from p in totalMarkList
                                                   select new
                                                   {
                                                       ClassId = p.ClassId,
                                                       SubjectId = p.SubjectId,
                                                       StudentNum = p.StudentNum,
                                                       StudentCount = p.StudentCount,
                                                       AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                                       GradeAvg = totalGrdeAvg,
                                                       MaxMark = p.MaxMark,
                                                       MinMark = p.MinMark,
                                                       StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                                       GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                       NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                       PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                                   }).ToList();

                var ExamTotalAnalyzeList = (from p in examTotalSubjectAnalyzeList
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                ClassId = p.ClassId,
                                                SubjectId = p.SubjectId,
                                                StudentNum = p.StudentNum,
                                                StudentCount = p.StudentCount,
                                                AvgMark = p.AvgMark,
                                                MaxMark = p.MaxMark,
                                                MinMark = p.MinMark,
                                                StandardDiff = p.StandardDiff,
                                                StandardMark = p.StandardDiff != 0 ? Math.Round(((p.AvgMark - p.GradeAvg) * 10 / (decimal)p.StandardDiff + 50).ConvertToDecimal(), 1, MidpointRounding.AwayFromZero) : 50,
                                                GoodRate = p.GoodRate,
                                                NormalRate = p.NormalRate,
                                                PassRate = p.PassRate,
                                            }).ToList();

                #endregion

                #region  年级项目成绩
                //当前考试：某科在班级中的标准差
                lstDiff = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var s in selectSubjectList)
                {
                    StandardMark = decimal.Zero;
                    standardGap = 0;
                    var subjectId = s.Value.ConvertToInt();
                    var gradeModel = new Exam.Dto.ExamAnalyze.List();
                    gradeModel.ClassId = 0;
                    if (subjectId != 0)
                    {
                        var gradeMark = classExamMarkList.Where(d => d.SubjectId == subjectId).ToList();
                        var gradeAvg = gradeSubjectMarkList.Where(d => d.SubjectId == subjectId).Select(d => d.AvgMark).FirstOrDefault();
                        foreach (var o in gradeMark)
                        {
                            var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                            var sd = (o.TotalMark - avgMark) * (o.TotalMark - avgMark);
                            StandardMark += sd.ConvertToDecimal();
                        }
                        if (gradeMark.Count > decimal.Zero)
                        {
                            standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / gradeMark.Count);
                        }
                        gradeModel.SubjectId = subjectId;
                        gradeModel.StandardDiff = standardGap;
                        lstDiff.Add(gradeModel);
                    }
                    else
                    {
                        var gradeMark = totalGradeStudentMarkList.Where(d => d.SubjectId == subjectId).ToList();
                        var gradeAvg = totalGradeMarkList.Where(d => d.SubjectId == subjectId).Select(d => d.AvgMark).FirstOrDefault();
                        foreach (var o in gradeMark)
                        {
                            var avgMark = gradeAvg != null ? gradeAvg : decimal.Zero;
                            var sd = (o.StudentTotalMark - avgMark) * (o.StudentTotalMark - avgMark);
                            StandardMark += sd.ConvertToDecimal();
                        }
                        if (gradeMark.Count > decimal.Zero)
                        {
                            standardGap = Math.Sqrt(double.Parse(StandardMark.ToString("N2")) * 1.0 / gradeMark.Count);
                        }
                        gradeModel.SubjectId = subjectId;
                        gradeModel.StandardDiff = standardGap;
                        lstDiff.Add(gradeModel);
                    }
                }

                //良好及格优秀
                var lstGradeSegment = new List<Exam.Dto.ExamAnalyze.List>();
                //各分数段人数
                var lstGrade = new List<Exam.Dto.ExamAnalyze.List>();
                //优秀及格率
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;
                    var isNormal = o.IsNormal;
                    var isTotal = o.IsTotal;
                    //优秀科目人数
                    if (isTotal && o.SubjectId == 0)
                    {
                        var tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  select p).ToList();

                        var tb = new Exam.Dto.ExamAnalyze.List()
                        {
                            ClassId = 0,
                            SegmentId = o.SegmentId,
                            SubjectId = 0,
                            StudentNum = tm.Count()
                        };
                        lstGrade.Add(tb);
                        if (isGood)
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  select p).ToList();

                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SegmentId = o.SegmentId,
                                SubjectId = 0,
                                Status = 1,
                                StudentNum = tm.Count()
                            };
                            lstGradeSegment.Add(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  select p).ToList();

                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SegmentId = o.SegmentId,
                                SubjectId = 0,
                                Status = 2,
                                StudentNum = tm.Count()
                            };
                            lstGradeSegment.Add(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            tm = (from p in totalStudentMarkList
                                  where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                                  select p).ToList();

                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SegmentId = o.SegmentId,
                                SubjectId = 0,
                                Status = 3,
                                StudentNum = tm.Count()
                            };
                            lstGradeSegment.Add(tb);
                        }
                    }
                    else
                    {
                        var tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  select p).ToList();

                        var tb = new Exam.Dto.ExamAnalyze.List()
                        {
                            ClassId = 0,
                            SubjectId = o.SubjectId,
                            SegmentId = o.SegmentId,
                            StudentNum = tm.Count(),
                        };
                        lstGrade.Add(tb);
                        if (isGood)
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  select p).ToList();

                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SubjectId = o.SubjectId,
                                Status = decimal.One,
                                StudentNum = tm.Count(),
                            };
                            lstGradeSegment.Add(tb);
                        }
                        if (isNormal)//良好人数
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  select p).ToList();

                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SubjectId = o.SubjectId,
                                Status = 2,
                                StudentNum = tm.Count(),
                            };
                            lstGradeSegment.Add(tb);
                        }
                        if (isPass)//及格人数
                        {
                            //分数段人数
                            tm = (from p in classExamMarkList
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  select p).ToList();
                            tb = new Exam.Dto.ExamAnalyze.List()
                            {
                                ClassId = 0,
                                SubjectId = o.SubjectId,
                                Status = 3,
                                StudentNum = tm.Count(),
                            };
                            lstGradeSegment.Add(tb);
                        }
                    }
                }

                //重要分数段人数
                var lstGradeImport = new List<Dto.ExamAnalyze.List>();
                foreach (var o in ImportSegmentMarkList)
                {
                    var tm = (from p in totalStudentMarkList
                              where p.StudentTotalMark >= o.MinMark && p.StudentTotalMark <= o.MaxMark
                              select p).ToList();

                    var tb = new Exam.Dto.ExamAnalyze.List()
                    {
                        ClassId = 0,
                        SegmentId = o.SegmentId,
                        SubjectId = 0,
                        StudentNum = tm.Count()
                    };
                    lstGradeImport.Add(tb);

                    foreach (var subject in selectSubjectList)
                    {
                        var tg = (from p in classExamMarkList
                                  where p.SubjectId == subject.Value.ConvertToInt()
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  select p).ToList();

                        tb = new Exam.Dto.ExamAnalyze.List()
                        {
                            ClassId = 0,
                            SegmentId = o.SegmentId,
                            SubjectId = subject.Value.ConvertToInt(),
                            StudentNum = tg.Count()
                        };
                        lstGradeImport.Add(tb);
                    }
                }

                tk = (from p in lstGradeSegment
                      group p by new
                      {
                          p.ClassId,
                          p.SubjectId,
                          p.Status
                      } into g
                      select new
                      {
                          g.Key.ClassId,
                          g.Key.SubjectId,
                          g.Key.Status,
                          StudentNum = g.Sum(d => d.StudentNum)
                      }).ToList();

                var garadeExamAnalyzeList = (from p in gradeSubjectMarkList
                                             select new Exam.Dto.ExamAnalyze.List
                                             {
                                                 ClassId = p.ClassId,
                                                 SubjectId = p.SubjectId,
                                                 StudentNum = p.StudentNum,
                                                 StudentCount = p.StudentCount,
                                                 AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                                 MaxMark = p.MaxMark,
                                                 MinMark = p.MinMark,
                                                 StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                                 StandardMark = 50,
                                                 GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                 NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                 PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                             }).ToList();

                var garadeTotalExamAnalyzeList = (from p in totalGradeMarkList
                                                  select new Exam.Dto.ExamAnalyze.List
                                                  {
                                                      ClassId = p.ClassId,
                                                      SubjectId = p.SubjectId,
                                                      StudentNum = p.StudentNum,
                                                      StudentCount = p.StudentCount,
                                                      AvgMark = decimal.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                                      MaxMark = p.MaxMark,
                                                      MinMark = p.MinMark,
                                                      StandardDiff = lstDiff.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId).Select(d => d.StandardDiff).FirstOrDefault(),
                                                      StandardMark = 50,
                                                      GoodRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                      NormalRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 2).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0,
                                                      PassRate = p.StudentCount.ConvertToInt() > decimal.Zero ? decimal.Round((decimal)tk.Where(d => d.ClassId == p.ClassId && d.SubjectId == p.SubjectId && d.Status == 3).Select(d => d.StudentNum).FirstOrDefault() / p.StudentCount.ConvertToInt() * 100, 2, MidpointRounding.AwayFromZero) : 0
                                                  }).ToList();
                #endregion

                //标准分各科目和总分班级年级排名
                var lstRank = (from p in ExamAnalyzeList.Union(ExamTotalAnalyzeList)
                               select new Exam.Dto.ExamAnalyze.List
                               {
                                   ClassId = p.ClassId,
                                   SubjectId = p.SubjectId,
                                   StudentNum = p.StudentNum,
                                   StudentCount = p.StudentCount,
                                   StandardDiff = System.Math.Round(p.StandardDiff, 1, MidpointRounding.AwayFromZero),
                                   StandardMark = Math.Round(p.StandardMark.ConvertToDecimal(), 1, MidpointRounding.AwayFromZero),
                                   AvgMark = p.AvgMark,
                                   MaxMark = p.MaxMark,
                                   MinMark = p.MinMark,
                                   GoodRate = p.GoodRate,
                                   NormalRate = p.NormalRate,
                                   PassRate = p.PassRate,
                                   TeacherName = p.TeacherName,
                                   GradeRank = 0
                               }).ToList();

                foreach (var s in selectSubjectList)
                {
                    var rank = 0;
                    decimal? mark = null;
                    var count = 1;
                    foreach (var t in lstRank.Where(d => d.SubjectId == s.Value.ConvertToInt()).OrderByDescending(d => d.StandardMark))
                    {
                        if (mark != t.StandardMark)
                        {
                            mark = t.StandardMark;
                            rank = rank + count;
                            count = 1;
                        }
                        else
                        {
                            count = count + 1;
                        }
                        var tt = (from p in lstRank
                                  where p.ClassId == t.ClassId && p.SubjectId == s.Value.ConvertToInt()
                                  select p).FirstOrDefault();
                        if (tt != null)
                        {
                            tt.GradeRank = rank;
                        }
                    }
                }

                var ExamGradeAnalyzeList = (from p in lstRank.Union(garadeExamAnalyzeList).Union(garadeTotalExamAnalyzeList)
                                            select new Exam.Dto.ExamAnalyze.List
                                            {
                                                ClassId = p.ClassId,
                                                SubjectId = p.SubjectId,
                                                StudentNum = p.StudentNum,
                                                StudentCount = p.StudentCount,
                                                StandardDiff = System.Math.Round(p.StandardDiff, 1, MidpointRounding.AwayFromZero),
                                                StandardMark = Math.Round(p.StandardMark.ConvertToDecimal(), 1, MidpointRounding.AwayFromZero),
                                                AvgMark = p.AvgMark,
                                                MaxMark = p.MaxMark,
                                                MinMark = p.MinMark,
                                                GoodRate = p.GoodRate,
                                                NormalRate = p.NormalRate,
                                                PassRate = p.PassRate,
                                                TeacherName = p.TeacherName,
                                                GradeRank = p.GradeRank
                                            }).ToList();

                var ExamAnalyzeImportSegmentList = (from p in lstImport.Union(lstGradeImport)
                                                    select new Exam.Dto.ExamAnalyze.List
                                                    {
                                                        ClassId = p.ClassId,
                                                        SegmentId = p.SegmentId,
                                                        SubjectId = p.SubjectId,
                                                        StudentNum = p.StudentNum
                                                    }).ToList();

                var ExamAnalyzeSegmentList = (from p in lst.Union(lstGrade)
                                              select new Exam.Dto.ExamAnalyze.List
                                              {
                                                  ClassId = p.ClassId,
                                                  SegmentId = p.SegmentId,
                                                  SubjectId = p.SubjectId,
                                                  StudentNum = p.StudentNum
                                              }).ToList();

                #endregion

                #region 导出
                var OptionList = new List<string>() { "班级", "应考数", "实考数", "平均分", "最高分", "最低分", "标准差" };
                var ClumnList = new List<string>() { "优秀率%", "良好率%", "及格率%" };
                var PgList = new List<string>() { "教师", "标准分", "名次" };
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

                var sheetName = year.ExamName + "综合报表";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                foreach (var subject in selectSubjectList)
                {
                    var segmentCount = SegmentList.Where(d => d.SubjectId == subject.Value.ConvertToInt()).Count();
                    IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                    //表头
                    ICell cell = cellHeader.CreateCell(0);
                    cell.SetCellValue(subject.Text);
                    CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, 0, OptionList.Count() + ClumnList.Count() + ImortSegmentList.Count() + segmentCount + 2);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    rowStartIndex++;
                    cellHeader = sheet1.CreateRow(rowStartIndex);
                    cell = cellHeader.CreateCell(0);
                    cell.SetCellValue("基本情况");
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, 0, OptionList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    cell = cellHeader.CreateCell(OptionList.Count());
                    cell.SetCellValue("各项比率");
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count(), OptionList.Count() + ClumnList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    if (ImortSegmentList.Count() > 0)
                    {
                        cell = cellHeader.CreateCell(OptionList.Count() + ClumnList.Count());
                        cell.SetCellValue("重要段人数");
                        cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count() + ClumnList.Count(), OptionList.Count() + ClumnList.Count() + ImortSegmentList.Count - 1);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                    }

                    if (segmentCount > 0)
                    {
                        cell = cellHeader.CreateCell(OptionList.Count() + ClumnList.Count() + ImortSegmentList.Count());
                        cell.SetCellValue("各分数段人数");
                        cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count() + ClumnList.Count(), OptionList.Count() + ClumnList.Count() + ImortSegmentList.Count + segmentCount - 1);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                    }

                    cell = cellHeader.CreateCell(OptionList.Count() + ClumnList.Count() + ImortSegmentList.Count() + segmentCount);
                    cell.SetCellValue("评估");
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count() + ClumnList.Count()+ImortSegmentList.Count() + segmentCount, OptionList.Count() + ClumnList.Count() + ImortSegmentList.Count + segmentCount + 2);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    //表头第三行
                    IRow cellHeaderR = sheet1.CreateRow(rowStartIndex + 1);
                    var Order = 0;
                    foreach (var option in OptionList)
                    {
                        ICell cell2 = cellHeaderR.CreateCell(Order);
                        cell2.CellStyle = cellstyle;
                        cell2.SetCellValue(option);
                        Order++;
                    }
                    Order = 0;
                    foreach (var option in ClumnList)
                    {
                        ICell cell2 = cellHeaderR.CreateCell(OptionList.Count() + Order);
                        cell2.CellStyle = cellstyle;
                        cell2.SetCellValue(option);
                        Order++;
                    }
                    Order = 0;
                    foreach (var imortSegment in ImortSegmentList)
                    {
                        ICell cell2 = cellHeaderR.CreateCell(OptionList.Count() + ClumnList.Count() + Order);
                        cell2.CellStyle = cellstyle;
                        cell2.SetCellValue(imortSegment.SegmentName);
                        Order++;
                    }
                    Order = 0;
                    foreach (var segment in SegmentList.Where(d => d.SubjectId == subject.Value.ConvertToInt()))
                    {
                        ICell cell2 = cellHeaderR.CreateCell(OptionList.Count() + ClumnList.Count() + ImortSegmentList.Count() + Order);
                        cell2.CellStyle = cellstyle;
                        cell2.SetCellValue(segment.SegmentName);
                        Order++;
                    }
                    Order = 0;
                    foreach (var pg in PgList)
                    {
                        ICell cell2 = cellHeaderR.CreateCell(OptionList.Count() + ClumnList.Count() + ImortSegmentList.Count() + segmentCount + Order);
                        cell2.CellStyle = cellstyle;
                        cell2.SetCellValue(pg);
                        Order++;
                    }
                    setBorder(cellRangeAddress, sheet1, hssfworkbook);

                    rowStartIndex++;
                    //数据行
                    foreach (var t in selctClassList)
                    {
                        //本次考试
                        var classId = t.Value.ConvertToInt();
                        var teacherName = string.Empty;
                        var subjectId = subject.Value.ConvertToInt();
                        if (subjectId != 0)
                        {
                            var teacher = string.Join(",", orgTeacherList.Where(d => d.ClassId == classId && d.SubjectId == subjectId).Select(d => d.TeacherName).Distinct().ToArray());
                            teacherName = teacher;
                        }
                        else
                        {
                            var teacher = string.Join(",",classTeacherList.Where(d => d.ClassId == classId).Select(d => d.TeacherName).Distinct().ToArray());
                            teacherName = teacher;
                        }
                        var mark = ExamGradeAnalyzeList.Where(d => d.ClassId == classId && d.SubjectId == subjectId
                                                                                       ).Select(d => d).FirstOrDefault();

                        cellHeader = sheet1.CreateRow(rowStartIndex + 1);

                        cell = cellHeader.CreateCell(0);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(t.Text);

                        cell = cellHeader.CreateCell(1);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.StudentNum.ToString() : string.Empty);

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
                        cell.SetCellValue(mark != null ? mark.StandardDiff.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(7);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.GoodRate.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(8);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.NormalRate.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(9);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.PassRate.ToString() : string.Empty);

                        Order = 0;
                        foreach (var imortSegment in ImortSegmentList)
                        {
                            var segmentId = imortSegment.SegmentId;
                            var imortMark = ExamAnalyzeImportSegmentList.Where(d => d.SegmentId == segmentId
                            && d.ClassId == classId && d.SubjectId == subject.Value.ConvertToInt()).FirstOrDefault();
                            cell = cellHeader.CreateCell(10 + Order);
                            cell.CellStyle = cellstyle;
                            cell.SetCellValue(imortMark != null ? imortMark.StudentNum.ToString() : string.Empty);
                            Order++;
                        }

                        Order = 0;
                        foreach (var segment in SegmentList.Where(d => d.SubjectId == subject.Value.ConvertToInt()))
                        {
                            var segmentId = segment.SegmentId;
                            var segmentMark = ExamAnalyzeSegmentList.Where(d => d.SegmentId == segmentId
                                                 && d.ClassId == classId && d.SubjectId == subject.Value.ConvertToInt()).FirstOrDefault();
                            cell = cellHeader.CreateCell(10 + ImortSegmentList.Count() + Order);
                            cell.CellStyle = cellstyle;
                            cell.SetCellValue(segmentMark != null ? segmentMark.StudentNum.ToString() : string.Empty);
                            Order++;
                        }

                        cell = cellHeader.CreateCell(OptionList.Count() + ClumnList.Count() + ImortSegmentList.Count() + segmentCount);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(teacherName);

                        cell = cellHeader.CreateCell(OptionList.Count() + ClumnList.Count() + ImortSegmentList.Count() + segmentCount + 1);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.StandardMark.ToString() : string.Empty);

                        cell = cellHeader.CreateCell(OptionList.Count() + ClumnList.Count() + ImortSegmentList.Count() + segmentCount + 2);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(mark != null ? mark.GradeRank.ToString() : string.Empty);


                        rowStartIndex++;
                    }
                    rowStartIndex += 3;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode(year.ExamName + "综合报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

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
        /// 合并单元格
        /// </summary>
        /// <param name="book"></param>
        /// <param name="cellIndex">需要合并的单元索引</param>
        /// <param name="rowIndex">需合并单元格的起始行，前两行为标题，默认起始行是第三行</param>
        public void MergeColumn(HSSFWorkbook book, int cellIndex, int rowIndex)
        {
            ISheet sheet = book.GetSheetAt(0);
            int cunit = cellIndex;
            // 合并单元格跨越的行数
            int rspan = 0;
            // 需合并单元格的起始行，前两行为标题，默认起始行是第三行
            int srow = rowIndex;

            // 合并单元格
            string oldvalue = string.Empty;
            for (int r = 1; r < sheet.PhysicalNumberOfRows; r++)
            {
                if (sheet.GetRow(r).GetCell(cunit).StringCellValue == oldvalue)
                {
                    rspan++;
                }
                else
                {
                    // 添加合并区域
                    sheet.AddMergedRegion(new CellRangeAddress(srow, srow + rspan, cellIndex, cellIndex));
                    oldvalue = sheet.GetRow(r).GetCell(cunit).StringCellValue;
                    srow = r;
                    rspan = 0;
                }
            }

            // 对未合并的单元格进行合并
            if (rspan != 0)
            {
                sheet.AddMergedRegion(new CellRangeAddress(srow, srow + rspan, cellIndex, cellIndex));
            }
        }
        #endregion
    }
}