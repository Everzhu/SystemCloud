using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamPortraitController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        #region 考试成绩多次考试成绩分数
        public ActionResult List()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDisable == false
                                    && p.YearType == Code.EnumHelper.YearType.Section
                                group p by new { p.tbYearParent.tbYearParent.Id, p.tbYearParent.tbYearParent.YearName, p.IsDefault } into g
                                select new
                                {
                                    g.Key.Id,
                                    g.Key.YearName,
                                    g.Key.IsDefault
                                }).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //默认当前激活学年
                if (vm.YearId == 0 && vm.YearList.Count() > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id;
                }

                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.tbYear.tbYearParent.tbYearParent.Id == vm.YearId
                                 && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString()
                               }).ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && p.tbYear.Id == vm.YearId
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                vm.ClassList = (from p in ClassList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ClassName,
                                    Value = p.ClassId.ToString()
                                }).ToList();

                //考试科目
                var chkExams = (vm.chkExam != null && vm.chkExam != string.Empty) ? vm.chkExam.Split(',') : new string[] { };
                foreach (var chkExam in chkExams)
                {
                    if (vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Value;
                        listItem.Text = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Text;
                        vm.ExamIds.Add(listItem);
                    }
                }

                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkExams.Contains(p.tbExam.Id.ToString())
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();
                foreach (var Subject in SubjectList)
                {
                    if (vm.SubjectList.Where(c => c.Value == Subject.SubjectId.ToString()).FirstOrDefault() == null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Text = Subject.SubjectName;
                        listItem.Value = Subject.SubjectId.ToString();
                        vm.SubjectList.Add(listItem);
                    }
                }

                var chkSubjects = (vm.chkSubject != null && vm.chkSubject != string.Empty) ? vm.chkSubject.Split(',') : new string[] { };
                foreach (var chkSubject in chkSubjects)
                {
                    if (chkSubject == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkSubject;
                        vm.SubjectIds.Add(listItem);
                    }
                    if (vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Value;
                        listItem.Text = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Text;
                        vm.SubjectIds.Add(listItem);
                    }
                }

                //年级学生班级
                var chkClasss = (vm.chkClass != null && vm.chkClass != string.Empty) ? vm.chkClass.Split(',') : new string[] { };
                foreach (var chkClass in chkClasss)
                {
                    if (chkClass == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                    if (vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                }

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    && (string.IsNullOrEmpty(vm.SearchText) ? true : (p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)))
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName
                                    }).Distinct().ToList();

                vm.ExamPortraitList = (from p in tf
                                       join t in classStudent
                                       on p.StudentId equals t.StudentId
                                       select new Dto.ExamPortrait.List
                                       {
                                           ExamId = p.ExamId.ToString(),
                                           StudentId = p.StudentId.ToString(),
                                           ClassName = t.ClassName,
                                           StudentCode = t.StudentCode,
                                           StudentName = t.StudentName,
                                           SubjectId = p.SubjectId.ToString(),
                                           TotalMark = p.TotalMark.ToString(),
                                           TotalClassRank = p.TotalClassRank.ToString(),
                                           TotalGradeRank = p.TotalGradeRank.ToString(),
                                       }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(Models.ExamPortrait.List vm)
        {
            var arrystr = string.Empty;
            var chkExamList = Request.Form["chkExam"] != null ? Request.Form["chkExam"].ToString() : arrystr;
            var chkClassList = Request.Form["chkClass"] != null ? Request.Form["chkClass"].ToString() : arrystr;
            var chkSubjectList = Request.Form["chkSubject"] != null ? Request.Form["chkSubject"].ToString() : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("List", new { YearId = vm.YearId, GradeId = vm.GradeId, chkExam = chkExamList, chkSubject = chkSubjectList, chkClass = chkClassList, searchText = vm.SearchText }));
        }
        #endregion

        #region 考试分多次考试各科分数报表
        public ActionResult TotalExamReportList(string chkExam, string chkSubject, string chkClass, int StudentId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var chkexamList = chkExam.Split(',');
                var chksubjectList = chkSubject.Split(',');
                var chkclassList = chkClass.Split(',');

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkexamList.Contains(p.Id.ToString())
                              && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.chkExam = ToJSONString(ExamList.Select(d => d.ExamName).ToList());

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkexamList.Contains(p.tbExam.Id.ToString())
                                   && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();


                vm.chkSubject = ToJSONString(SubjectList.Select(d => d.SubjectName).ToList());

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkclassList.Contains(p.tbClass.Id.ToString())
                                    && p.tbStudent.Id == StudentId
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentCode,
                                        p.tbStudent.StudentName
                                    }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && chkexamList.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                           && p.tbStudent.Id == StudentId
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                var totalList = new List<object>();
                var totalClassList = new List<object>();
                var totalGradeList = new List<object>();
                foreach (var subject in SubjectList)
                {
                    var scoreList = new List<object>();
                    var scoreClassList = new List<object>();
                    var scoreGradeList = new List<object>();
                    foreach (var exam in ExamList)
                    {
                        var examScore = tf.Where(d => d.SubjectId == subject.SubjectId && d.ExamId == exam.ExamId && d.StudentId == StudentId).FirstOrDefault();
                        if (examScore != null)
                        {
                            scoreList.Add(examScore.TotalMark);
                            scoreClassList.Add(examScore.TotalClassRank);
                            scoreGradeList.Add(examScore.TotalGradeRank);
                        }
                        //else
                        //{
                        //    scoreList.Add(0);
                        //    scoreClassList.Add(0);
                        //    scoreGradeList.Add(0);
                        //}
                    }
                    totalList.Add(new { name = subject.SubjectName, type = "line", data = scoreList });
                    totalClassList.Add(new { name = subject.SubjectName, type = "line", data = scoreClassList });
                    totalGradeList.Add(new { name = subject.SubjectName, type = "line", data = scoreGradeList });
                }
                vm.ReportScore = ToJSONString(totalList);
                vm.ReportScoreClass = ToJSONString(totalClassList);
                vm.ReportScoreGrade = ToJSONString(totalGradeList);
                return View(vm);
            }

        }
        #endregion

        #region 综合分多次考试各科分数
        public ActionResult SegmentExamList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDisable == false
                                    && p.YearType == Code.EnumHelper.YearType.Section
                                group p by new { p.tbYearParent.tbYearParent.Id, p.tbYearParent.tbYearParent.YearName, p.IsDefault } into g
                                select new
                                {
                                    g.Key.Id,
                                    g.Key.YearName,
                                    g.Key.IsDefault
                                }).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //默认当前激活学年
                if (vm.YearId == 0 && vm.YearList.Count() > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id;
                }
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.tbYear.tbYearParent.tbYearParent.Id == vm.YearId
                                 && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString()
                               }).ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && p.tbYear.Id == vm.YearId
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                vm.ClassList = (from p in ClassList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ClassName,
                                    Value = p.ClassId.ToString()
                                }).ToList();

                //考试科目
                var chkExams = (vm.chkExam != null && vm.chkExam != string.Empty) ? vm.chkExam.Split(',') : new string[] { };
                foreach (var chkExam in chkExams)
                {
                    if (vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Value;
                        listItem.Text = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Text;
                        vm.ExamIds.Add(listItem);
                    }
                }

                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkExams.Contains(p.tbExam.Id.ToString())
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();

                foreach (var Subject in SubjectList)
                {
                    if (vm.SubjectList.Where(c => c.Value == Subject.SubjectId.ToString()).FirstOrDefault() == null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Text = Subject.SubjectName;
                        listItem.Value = Subject.SubjectId.ToString();
                        vm.SubjectList.Add(listItem);
                    }
                }

                var chkSubjects = (vm.chkSubject != null && vm.chkSubject != string.Empty) ? vm.chkSubject.Split(',') : new string[] { };
                foreach (var chkSubject in chkSubjects)
                {
                    if (chkSubject == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkSubject;
                        vm.SubjectIds.Add(listItem);
                    }
                    if (vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Value;
                        listItem.Text = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Text;
                        vm.SubjectIds.Add(listItem);
                    }
                }

                //年级学生班级
                var chkClasss = (vm.chkClass != null && vm.chkClass != string.Empty) ? vm.chkClass.Split(',') : new string[] { };
                foreach (var chkClass in chkClasss)
                {
                    var listItem = new System.Web.Mvc.SelectListItem();
                    listItem.Value = chkClass;
                    vm.ClassIds.Add(listItem);
                }

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.TotalMark,
                              p.SegmentMark,
                              p.SegmentClassRank,
                              p.SegmentGradeRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                //获取班级学生
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    && (string.IsNullOrEmpty(vm.SearchText) ? true : (p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)))
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName
                                    }).Distinct().ToList();

                vm.ExamPortraitList = (from p in tf
                                       join t in classStudent
                                       on p.StudentId equals t.StudentId
                                       select new Dto.ExamPortrait.List
                                       {
                                           ExamId = p.ExamId.ToString(),
                                           StudentId = p.StudentId.ToString(),
                                           ClassName = t.ClassName,
                                           StudentCode = t.StudentCode,
                                           StudentName = t.StudentName,
                                           SubjectId = p.SubjectId.ToString(),
                                           SegmentMark = p.SegmentMark.ToString(),
                                           SegmentClassRank = p.SegmentClassRank.ToString(),
                                           SegmentGradeRank = p.SegmentGradeRank.ToString(),
                                       }).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SegmentExamList(Models.ExamPortrait.List vm)
        {
            var arrystr = string.Empty;
            var chkExamList = Request.Form["chkExam"] != null ? Request.Form["chkExam"].ToString() : arrystr;
            var chkClassList = Request.Form["chkClass"] != null ? Request.Form["chkClass"].ToString() : arrystr;
            var chkSubjectList = Request.Form["chkSubject"] != null ? Request.Form["chkSubject"].ToString() : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("SegmentExamList", new { YearId = vm.YearId, GradeId = vm.GradeId, chkExam = chkExamList, chkSubject = chkSubjectList, chkClass = chkClassList, searchText = vm.SearchText }));
        }
        #endregion

        #region 综合分多次考试各科分数报表
        public ActionResult SegmentExamReportList(string chkExam, string chkSubject, string chkClass, int StudentId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var chkexamList = chkExam.Split(',');
                var chksubjectList = chkSubject.Split(',');
                var chkclassList = chkClass.Split(',');


                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkexamList.Contains(p.Id.ToString())
                              && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.chkExam = ToJSONString(ExamList.Select(d => d.ExamName).ToList());

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkexamList.Contains(p.tbExam.Id.ToString())
                                   && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();


                vm.chkSubject = ToJSONString(SubjectList.Select(d => d.SubjectName).ToList());

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkclassList.Contains(p.tbClass.Id.ToString())
                                    && p.tbStudent.Id == StudentId
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentCode,
                                        p.tbStudent.StudentName
                                    }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && chkexamList.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                           && p.tbStudent.Id == StudentId
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.SegmentMark,
                              p.SegmentClassRank,
                              p.SegmentGradeRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                var totalList = new List<object>();
                var totalClassList = new List<object>();
                var totalGradeList = new List<object>();

                foreach (var subject in SubjectList)
                {
                    var scoreList = new List<object>();
                    var scoreClassList = new List<object>();
                    var scoreGradeList = new List<object>();
                    foreach (var exam in ExamList)
                    {
                        var examScore = tf.Where(d => d.SubjectId == subject.SubjectId && d.ExamId == exam.ExamId && d.StudentId == StudentId).FirstOrDefault();
                        if (examScore != null)
                        {
                            scoreList.Add(examScore.SegmentMark);
                            scoreClassList.Add(examScore.SegmentClassRank);
                            scoreGradeList.Add(examScore.SegmentGradeRank);
                        }
                    }
                    totalList.Add(new { name = subject.SubjectName, type = "line", data = scoreList });
                    totalClassList.Add(new { name = subject.SubjectName, type = "line", data = scoreClassList });
                    totalGradeList.Add(new { name = subject.SubjectName, type = "line", data = scoreGradeList });
                }
                vm.ReportScore = ToJSONString(totalList);
                vm.ReportScoreClass = ToJSONString(totalClassList);
                vm.ReportScoreGrade = ToJSONString(totalGradeList);
                return View(vm);
            }

        }
        #endregion

        #region 考试分多次考试班级平均分
        public ActionResult TotalMarkExamAvgList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDisable == false
                                    && p.YearType == Code.EnumHelper.YearType.Section
                                group p by new { p.tbYearParent.tbYearParent.Id, p.tbYearParent.tbYearParent.YearName, p.IsDefault } into g
                                select new
                                {
                                    g.Key.Id,
                                    g.Key.YearName,
                                    g.Key.IsDefault
                                }).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //默认当前激活学年
                if (vm.YearId == 0 && vm.YearList.Count() > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id;
                }
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.tbYear.tbYearParent.tbYearParent.Id == vm.YearId
                                 && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString()
                               }).ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && p.tbYear.Id == vm.YearId
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                vm.ClassList = (from p in ClassList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ClassName,
                                    Value = p.ClassId.ToString()
                                }).ToList();

                //考试科目
                var chkExams = (vm.chkExam != null && vm.chkExam != string.Empty) ? vm.chkExam.Split(',') : new string[] { };
                foreach (var chkExam in chkExams)
                {
                    if (vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Value;
                        listItem.Text = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Text;
                        vm.ExamIds.Add(listItem);
                    }
                }

                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkExams.Contains(p.tbExam.Id.ToString())
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();
                foreach (var Subject in SubjectList)
                {
                    if (vm.SubjectList.Where(c => c.Value == Subject.SubjectId.ToString()).FirstOrDefault() == null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Text = Subject.SubjectName;
                        listItem.Value = Subject.SubjectId.ToString();
                        vm.SubjectList.Add(listItem);
                    }
                }

                var chkSubjects = (vm.chkSubject != null && vm.chkSubject != string.Empty) ? vm.chkSubject.Split(',') : new string[] { };
                foreach (var chkSubject in chkSubjects)
                {
                    if (chkSubject == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkSubject;
                        vm.SubjectIds.Add(listItem);
                    }
                    if (vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Value;
                        listItem.Text = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Text;
                        vm.SubjectIds.Add(listItem);
                    }
                }

                //年级学生班级
                var chkClasss = (vm.chkClass != null && vm.chkClass != string.Empty) ? vm.chkClass.Split(',') : new string[] { };
                foreach (var chkClass in chkClasss)
                {
                    if (chkClass == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                    if (vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Value;
                        listItem.Text = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Text;
                        vm.ClassIds.Add(listItem);
                    }
                }

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              TotalMark = p.TotalMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              ExamId = p.ExamId,
                          }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in vm.SubjectIds)
                {
                    foreach (var exam in vm.ExamIds)
                    {
                        var classAvgList = (from p in tg
                                            where p.SubjectId.ToString() == subject.Value
                                            && p.ExamId.ToString() == exam.Value
                                            group p by new
                                            {
                                                p.ClassId,
                                                p.ClassName
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                g.Key.ClassName,
                                                AvgMark = g.Average(d => d.TotalMark)
                                            }).ToList();

                        var tb = (from p in classAvgList
                                  select new Exam.Dto.ExamPortrait.List
                                  {
                                      ClassId = p.ClassId.ToString(),
                                      SubjectId = subject.Value,
                                      ClassName = p.ClassName,
                                      ExamId = exam.Value,
                                      AvgMark = p.AvgMark != null ? decimal.Round((decimal)p.AvgMark, 2, MidpointRounding.AwayFromZero) : 0,
                                      ClassRank = decimal.Zero
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }

                //排名
                foreach (var subject in vm.SubjectIds)
                {
                    foreach (var exam in vm.ExamIds)
                    {
                        var rank = decimal.Zero;
                        decimal? mark = null;
                        var count = decimal.One;
                        foreach (var t in lst.Where(d => d.SubjectId == subject.Value && d.ExamId == exam.Value).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = decimal.One;
                            }
                            else
                            {
                                count = count + decimal.One;
                            }

                            t.ClassRank = rank;
                        }
                    }
                }
                vm.ExamPortraitList = (from p in lst
                                       where (string.IsNullOrEmpty(vm.SearchText) ? true : (p.ClassName.Contains(vm.SearchText)))
                                       select p).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TotalMarkExamAvgList(Models.ExamPortrait.List vm)
        {
            var arrystr = string.Empty;
            var chkExamList = Request.Form["chkExam"] != null ? Request.Form["chkExam"].ToString() : arrystr;
            var chkClassList = Request.Form["chkClass"] != null ? Request.Form["chkClass"].ToString() : arrystr;
            var chkSubjectList = Request.Form["chkSubject"] != null ? Request.Form["chkSubject"].ToString() : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("TotalMarkExamAvgList", new { YearId = vm.YearId, GradeId = vm.GradeId, chkExam = chkExamList, chkSubject = chkSubjectList, chkClass = chkClassList, searchText = vm.SearchText }));
        }
        #endregion

        #region 考试分多次考试班级平均分报表
        public ActionResult TotalExamAvgReportList(string chkExam, string chkSubject, string chkClass, int ClassId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var chkexamList = chkExam.Split(',');
                var chksubjectList = chkSubject.Split(',');
                var chkclassList = chkClass.Split(',');

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkexamList.Contains(p.Id.ToString())
                              && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.chkExam = ToJSONString(ExamList.Select(d => d.ExamName).ToList());

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkexamList.Contains(p.tbExam.Id.ToString())
                                   && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();


                vm.chkSubject = ToJSONString(SubjectList.Select(d => d.SubjectName).ToList());

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkexamList.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkclassList.Contains(p.tbClass.Id.ToString())
                                    && (string.IsNullOrEmpty(vm.SearchText) ? true : (p.tbClass.ClassName.Contains(vm.SearchText)))
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              TotalMark = p.TotalMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              ExamId = p.ExamId,
                          }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in SubjectList)
                {
                    foreach (var exam in ExamList)
                    {
                        var classAvgList = (from p in tg
                                            where p.SubjectId == subject.SubjectId
                                            && p.ExamId == exam.ExamId
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                AvgMark = g.Average(d => d.TotalMark)
                                            }).ToList();

                        var tb = (from p in classAvgList
                                  select new Exam.Dto.ExamPortrait.List
                                  {
                                      ClassId = p.ClassId.ToString(),
                                      SubjectId = subject.SubjectId.ToString(),
                                      ExamId = exam.ExamId.ToString(),
                                      AvgMark = p.AvgMark != null ? decimal.Round((decimal)p.AvgMark, 2, MidpointRounding.AwayFromZero) : 0,
                                      ClassRank = decimal.Zero
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }

                //排名
                foreach (var subject in SubjectList)
                {
                    foreach (var exam in ExamList)
                    {
                        var rank = decimal.Zero;
                        decimal? mark = null;
                        var count = decimal.One;
                        foreach (var t in lst.Where(d => d.SubjectId == subject.SubjectId.ToString() && d.ExamId == exam.ExamId.ToString()).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = decimal.One;
                            }
                            else
                            {
                                count = count + decimal.One;
                            }

                            t.ClassRank = rank;
                        }
                    }
                }

                var totalAvgList = new List<object>();
                var totalRankList = new List<object>();
                foreach (var subject in SubjectList)
                {
                    var totalClassAvgList = new List<object>();
                    var totalClassRankList = new List<object>();
                    foreach (var exam in ExamList)
                    {
                        var examScore = lst.Where(d => d.SubjectId == subject.SubjectId.ToString() && d.ExamId == exam.ExamId.ToString() && d.ClassId == ClassId.ToString()).FirstOrDefault();
                        if (examScore != null)
                        {
                            totalClassAvgList.Add(examScore.AvgMark);
                            totalClassRankList.Add(examScore.ClassRank);
                        }
                    }
                    totalAvgList.Add(new { name = subject.SubjectName, type = "line", data = totalClassAvgList });
                    totalRankList.Add(new { name = subject.SubjectName, type = "line", data = totalClassRankList });
                }
                vm.ReportAvgClass = ToJSONString(totalAvgList);
                vm.ReportAvgClassRank = ToJSONString(totalRankList);
                return View(vm);
            }
        }
        #endregion

        #region 综合分多次考试班级平均分报表
        public ActionResult SegmentExamAvgReportList(string chkExam, string chkSubject, string chkClass, int ClassId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var chkexamList = chkExam.Split(',');
                var chksubjectList = chkSubject.Split(',');
                var chkclassList = chkClass.Split(',');

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkexamList.Contains(p.Id.ToString())
                              && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.chkExam = ToJSONString(ExamList.Select(d => d.ExamName).ToList());

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkexamList.Contains(p.tbExam.Id.ToString())
                                   && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();


                vm.chkSubject = ToJSONString(SubjectList.Select(d => d.SubjectName).ToList());

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkexamList.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkclassList.Contains(p.tbClass.Id.ToString())
                                    && (string.IsNullOrEmpty(vm.SearchText) ? true : (p.tbClass.ClassName.Contains(vm.SearchText)))
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                //获取班级学生综合成绩
                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              SegmentMark = p.SegmentMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              ExamId = p.ExamId,
                          }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in SubjectList)
                {
                    foreach (var exam in ExamList)
                    {
                        var classAvgList = (from p in tg
                                            where p.SubjectId == subject.SubjectId
                                            && p.ExamId == exam.ExamId
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                AvgMark = g.Average(d => d.SegmentMark)
                                            }).ToList();

                        var tb = (from p in classAvgList
                                  select new Exam.Dto.ExamPortrait.List
                                  {
                                      ClassId = p.ClassId.ToString(),
                                      SubjectId = subject.SubjectId.ToString(),
                                      ExamId = exam.ExamId.ToString(),
                                      AvgMark = p.AvgMark != null ? decimal.Round((decimal)p.AvgMark, 2, MidpointRounding.AwayFromZero) : 0,
                                      ClassRank = decimal.Zero
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }

                //排名
                foreach (var subject in SubjectList)
                {
                    foreach (var exam in ExamList)
                    {
                        var rank = decimal.Zero;
                        decimal? mark = null;
                        var count = decimal.One;
                        foreach (var t in lst.Where(d => d.SubjectId == subject.SubjectId.ToString() && d.ExamId == exam.ExamId.ToString()).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = decimal.One;
                            }
                            else
                            {
                                count = count + decimal.One;
                            }

                            t.ClassRank = rank;
                        }
                    }
                }

                var totalAvgList = new List<object>();
                var totalRankList = new List<object>();
                foreach (var subject in SubjectList)
                {
                    var totalClassAvgList = new List<object>();
                    var totalClassRankList = new List<object>();
                    foreach (var exam in ExamList)
                    {
                        var examScore = lst.Where(d => d.SubjectId == subject.SubjectId.ToString() && d.ExamId == exam.ExamId.ToString() && d.ClassId == ClassId.ToString()).FirstOrDefault();
                        if (examScore != null)
                        {
                            totalClassAvgList.Add(examScore.AvgMark);
                            totalClassRankList.Add(examScore.ClassRank);
                        }
                    }
                    totalAvgList.Add(new { name = subject.SubjectName, type = "line", data = totalClassAvgList });
                    totalRankList.Add(new { name = subject.SubjectName, type = "line", data = totalClassRankList });
                }
                vm.ReportAvgClass = ToJSONString(totalAvgList);
                vm.ReportAvgClassRank = ToJSONString(totalRankList);
                return View(vm);
            }
        }
        #endregion

        #region 综合分多次考试班级平均分
        public ActionResult SegmentMarkExamAvgList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDisable == false
                                    && p.YearType == Code.EnumHelper.YearType.Section
                                group p by new { p.tbYearParent.tbYearParent.Id, p.tbYearParent.tbYearParent.YearName, p.IsDefault } into g
                                select new
                                {
                                    g.Key.Id,
                                    g.Key.YearName,
                                    g.Key.IsDefault
                                }).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //默认当前激活学年
                if (vm.YearId == 0 && vm.YearList.Count() > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id;
                }
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.tbYear.tbYearParent.tbYearParent.Id == vm.YearId
                                 && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString()
                               }).ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && p.tbYear.Id == vm.YearId
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                vm.ClassList = (from p in ClassList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ClassName,
                                    Value = p.ClassId.ToString()
                                }).ToList();

                //考试科目
                var chkExams = (vm.chkExam != null && vm.chkExam != string.Empty) ? vm.chkExam.Split(',') : new string[] { };
                foreach (var chkExam in chkExams)
                {
                    if (vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Value;
                        listItem.Text = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Text;
                        vm.ExamIds.Add(listItem);
                    }
                }

                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkExams.Contains(p.tbExam.Id.ToString())
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();
                foreach (var Subject in SubjectList)
                {
                    if (vm.SubjectList.Where(c => c.Value == Subject.SubjectId.ToString()).FirstOrDefault() == null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Text = Subject.SubjectName;
                        listItem.Value = Subject.SubjectId.ToString();
                        vm.SubjectList.Add(listItem);
                    }
                }

                var chkSubjects = (vm.chkSubject != null && vm.chkSubject != string.Empty) ? vm.chkSubject.Split(',') : new string[] { };
                foreach (var chkSubject in chkSubjects)
                {
                    if (chkSubject == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkSubject;
                        vm.SubjectIds.Add(listItem);
                    }
                    if (vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Value;
                        listItem.Text = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Text;
                        vm.SubjectIds.Add(listItem);
                    }
                }

                //年级学生班级
                var chkClasss = (vm.chkClass != null && vm.chkClass != string.Empty) ? vm.chkClass.Split(',') : new string[] { };
                foreach (var chkClass in chkClasss)
                {
                    if (chkClass == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                    if (vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Value;
                        listItem.Text = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Text;
                        vm.ClassIds.Add(listItem);
                    }
                }

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              SegmentMark = p.SegmentMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              ExamId = p.ExamId,
                          }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in vm.SubjectIds)
                {
                    foreach (var exam in vm.ExamIds)
                    {
                        var classAvgList = (from p in tg
                                            where p.SubjectId.ToString() == subject.Value
                                            && p.ExamId.ToString() == exam.Value
                                            group p by new
                                            {
                                                p.ClassId,
                                                p.ClassName
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                g.Key.ClassName,
                                                AvgMark = g.Average(d => d.SegmentMark)
                                            }).ToList();

                        var tb = (from p in classAvgList
                                  select new Exam.Dto.ExamPortrait.List
                                  {
                                      ClassId = p.ClassId.ToString(),
                                      SubjectId = subject.Value,
                                      ClassName = p.ClassName,
                                      ExamId = exam.Value,
                                      AvgMark = p.AvgMark != null ? decimal.Round((decimal)p.AvgMark, 2, MidpointRounding.AwayFromZero) : 0,
                                      ClassRank = decimal.Zero
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }

                //排名
                foreach (var subject in vm.SubjectIds)
                {
                    foreach (var exam in vm.ExamIds)
                    {
                        var rank = decimal.Zero;
                        decimal? mark = null;
                        var count = decimal.One;
                        foreach (var t in lst.Where(d => d.SubjectId == subject.Value && d.ExamId == exam.Value).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = decimal.One;
                            }
                            else
                            {
                                count = count + decimal.One;
                            }

                            t.ClassRank = rank;
                        }
                    }
                }
                vm.ExamPortraitList = (from p in lst
                                       where (string.IsNullOrEmpty(vm.SearchText) ? true : (p.ClassName.Contains(vm.SearchText)))
                                       select p).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SegmentMarkExamAvgList(Models.ExamPortrait.List vm)
        {
            var arrystr = string.Empty;
            var chkExamList = Request.Form["chkExam"] != null ? Request.Form["chkExam"].ToString() : arrystr;
            var chkClassList = Request.Form["chkClass"] != null ? Request.Form["chkClass"].ToString() : arrystr;
            var chkSubjectList = Request.Form["chkSubject"] != null ? Request.Form["chkSubject"].ToString() : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("SegmentMarkExamAvgList", new { YearId = vm.YearId, GradeId = vm.GradeId, chkExam = chkExamList, chkSubject = chkSubjectList, chkClass = chkClassList, searchText = vm.SearchText }));
        }
        #endregion

        #region 考试分多次考试各科前N名
        public ActionResult TotalMarkExamTopList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDisable == false
                                    && p.YearType == Code.EnumHelper.YearType.Section
                                group p by new { p.tbYearParent.tbYearParent.Id, p.tbYearParent.tbYearParent.YearName, p.IsDefault } into g
                                select new
                                {
                                    g.Key.Id,
                                    g.Key.YearName,
                                    g.Key.IsDefault
                                }).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //默认当前激活学年
                if (vm.YearId == 0 && vm.YearList.Count() > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id;
                }
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                var rank = vm.SearchText.ConvertToDecimal();

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.tbYear.tbYearParent.tbYearParent.Id == vm.YearId
                                 && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString()
                               }).ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && p.tbYear.Id == vm.YearId
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                vm.ClassList = (from p in ClassList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ClassName,
                                    Value = p.ClassId.ToString()
                                }).ToList();

                //考试科目
                var chkExams = (vm.chkExam != null && vm.chkExam != string.Empty) ? vm.chkExam.Split(',') : new string[] { };
                foreach (var chkExam in chkExams)
                {
                    if (vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Value;
                        listItem.Text = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Text;
                        vm.ExamIds.Add(listItem);
                    }
                }

                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkExams.Contains(p.tbExam.Id.ToString())
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();
                foreach (var Subject in SubjectList)
                {
                    if (vm.SubjectList.Where(c => c.Value == Subject.SubjectId.ToString()).FirstOrDefault() == null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Text = Subject.SubjectName;
                        listItem.Value = Subject.SubjectId.ToString();
                        vm.SubjectList.Add(listItem);
                    }
                }

                var chkSubjects = (vm.chkSubject != null && vm.chkSubject != string.Empty) ? vm.chkSubject.Split(',') : new string[] { };
                foreach (var chkSubject in chkSubjects)
                {
                    if (chkSubject == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkSubject;
                        vm.SubjectIds.Add(listItem);
                    }
                    if (vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Value;
                        listItem.Text = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Text;
                        vm.SubjectIds.Add(listItem);
                    }
                }

                //年级学生班级
                var chkClasss = (vm.chkClass != null && vm.chkClass != string.Empty) ? vm.chkClass.Split(',') : new string[] { };
                foreach (var chkClass in chkClasss)
                {
                    if (chkClass == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                    if (vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Value;
                        listItem.Text = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Text;
                        vm.ClassIds.Add(listItem);
                    }
                }

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalClassRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              ExamId = p.ExamId,
                              ClassRank = p.TotalClassRank,
                          }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in vm.SubjectIds)
                {
                    foreach (var exam in vm.ExamIds)
                    {
                        var totalStudetCount = (from p in tg
                                                where p.SubjectId.ToString() == subject.Value
                                             && p.ExamId.ToString() == exam.Value
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
                                  where p.SubjectId.ToString() == subject.Value
                                           && p.ExamId.ToString() == exam.Value
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
                                  select new Exam.Dto.ExamPortrait.List
                                  {
                                      ClassId = p.ClassId.ToString(),
                                      SubjectId = subject.Value,
                                      ExamId = exam.Value,
                                      StudentCount = p.StudentCount > decimal.Zero ? p.StudentCount.ToString() : string.Empty,
                                      Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentCount * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }
                vm.ExamPortraitList = lst;

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TotalMarkExamTopList(Models.ExamPortrait.List vm)
        {
            var arrystr = string.Empty;
            var chkExamList = Request.Form["chkExam"] != null ? Request.Form["chkExam"].ToString() : arrystr;
            var chkClassList = Request.Form["chkClass"] != null ? Request.Form["chkClass"].ToString() : arrystr;
            var chkSubjectList = Request.Form["chkSubject"] != null ? Request.Form["chkSubject"].ToString() : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("TotalMarkExamTopList", new { YearId = vm.YearId, GradeId = vm.GradeId, chkExam = chkExamList, chkSubject = chkSubjectList, chkClass = chkClassList, searchText = vm.SearchText }));
        }

        #endregion

        #region 综合分多次考试各科前N名
        public ActionResult SegmentMarkExamTopList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDisable == false
                                    && p.YearType == Code.EnumHelper.YearType.Section
                                group p by new { p.tbYearParent.tbYearParent.Id, p.tbYearParent.tbYearParent.YearName, p.IsDefault } into g
                                select new
                                {
                                    g.Key.Id,
                                    g.Key.YearName,
                                    g.Key.IsDefault
                                }).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //默认当前激活学年
                if (vm.YearId == 0 && vm.YearList.Count() > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id;
                }
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                var rank = vm.SearchText.ConvertToDecimal();

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.tbYear.tbYearParent.tbYearParent.Id == vm.YearId
                                 && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString()
                               }).ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && p.tbYear.Id == vm.YearId
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                vm.ClassList = (from p in ClassList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ClassName,
                                    Value = p.ClassId.ToString()
                                }).ToList();

                //考试科目
                var chkExams = (vm.chkExam != null && vm.chkExam != string.Empty) ? vm.chkExam.Split(',') : new string[] { };
                foreach (var chkExam in chkExams)
                {
                    if (vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Value;
                        listItem.Text = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Text;
                        vm.ExamIds.Add(listItem);
                    }
                }

                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkExams.Contains(p.tbExam.Id.ToString())
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();
                foreach (var Subject in SubjectList)
                {
                    if (vm.SubjectList.Where(c => c.Value == Subject.SubjectId.ToString()).FirstOrDefault() == null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Text = Subject.SubjectName;
                        listItem.Value = Subject.SubjectId.ToString();
                        vm.SubjectList.Add(listItem);
                    }
                }

                var chkSubjects = (vm.chkSubject != null && vm.chkSubject != string.Empty) ? vm.chkSubject.Split(',') : new string[] { };
                foreach (var chkSubject in chkSubjects)
                {
                    if (chkSubject == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkSubject;
                        vm.SubjectIds.Add(listItem);
                    }
                    if (vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Value;
                        listItem.Text = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Text;
                        vm.SubjectIds.Add(listItem);
                    }
                }

                //年级学生班级
                var chkClasss = (vm.chkClass != null && vm.chkClass != string.Empty) ? vm.chkClass.Split(',') : new string[] { };
                foreach (var chkClass in chkClasss)
                {
                    if (chkClass == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                    if (vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Value;
                        listItem.Text = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Text;
                        vm.ClassIds.Add(listItem);
                    }
                }

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.SegmentClassRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              ExamId = p.ExamId,
                              ClassRank = p.SegmentClassRank,
                          }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in vm.SubjectIds)
                {
                    foreach (var exam in vm.ExamIds)
                    {
                        var totalStudetCount = (from p in tg
                                                where p.SubjectId.ToString() == subject.Value
                                             && p.ExamId.ToString() == exam.Value
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
                                  where p.SubjectId.ToString() == subject.Value
                                           && p.ExamId.ToString() == exam.Value
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
                                  select new Exam.Dto.ExamPortrait.List
                                  {
                                      ClassId = p.ClassId.ToString(),
                                      SubjectId = subject.Value,
                                      ExamId = exam.Value,
                                      StudentCount = p.StudentCount > decimal.Zero ? p.StudentCount.ToString() : string.Empty,
                                      Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentCount * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }
                vm.ExamPortraitList = lst;

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SegmentMarkExamTopList(Models.ExamPortrait.List vm)
        {
            var arrystr = string.Empty;
            var chkExamList = Request.Form["chkExam"] != null ? Request.Form["chkExam"].ToString() : arrystr;
            var chkClassList = Request.Form["chkClass"] != null ? Request.Form["chkClass"].ToString() : arrystr;
            var chkSubjectList = Request.Form["chkSubject"] != null ? Request.Form["chkSubject"].ToString() : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("SegmentMarkExamTopList", new { YearId = vm.YearId, GradeId = vm.GradeId, chkExam = chkExamList, chkSubject = chkSubjectList, chkClass = chkClassList, searchText = vm.SearchText }));
        }

        #endregion

        #region 考试分多次考试各科前N名明细
        public ActionResult TotalMarkExamTopDetailList(string chkExam, string chkSubject, string chkClass, int ClassId, string rank)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var chkexamList = chkExam.Split(',');
                var chksubjectList = chkSubject.Split(',');
                var chkclassList = chkClass.Split(',');

                var Rank = rank.ConvertToDecimal();

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkexamList.Contains(p.Id.ToString())
                              && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkexamList.Contains(p.tbExam.Id.ToString())
                                   && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();

                //考试成绩
                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkexamList.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalClassRank,
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.Id == ClassId
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

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
                              p.TotalMark,
                              p.ExamId,
                          }).ToList();

                var List = new List<object>();
                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in SubjectList)
                {
                    var totalCountList = new List<object>();
                    foreach (var exam in ExamList)
                    {
                        //分数段人数
                        var tm = (from p in tg
                                  where p.SubjectId == subject.SubjectId
                                  && p.ExamId == exam.ExamId
                                  && p.ClassRank > decimal.Zero && p.ClassRank <= Rank
                                  select new
                                  {
                                      SubjectId = p.SubjectId,
                                      ExamId = p.ExamId,
                                      SubjectName = subject.SubjectName + "(前" + Rank + "名)",
                                      ExamName = exam.ExamName,
                                      p.StudentCode,
                                      p.StudentName,
                                      p.TotalMark
                                  }).ToList();
                        var studentCount = tm.Count();
                        if (studentCount > decimal.Zero)
                        {
                            var tb = (from p in tm
                                      select new Exam.Dto.ExamPortrait.List
                                      {
                                          SubjectId = p.SubjectId.ToString(),
                                          ExamId = exam.ExamId.ToString(),
                                          StudentCode = p.StudentCode,
                                          StudentName = p.StudentName,
                                          SubjectName = p.SubjectName,
                                          ExamName = exam.ExamName,
                                          Mark = p.TotalMark.ToString(),
                                          TotalCount = studentCount
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                        else
                        {
                            var model = new Exam.Dto.ExamPortrait.List()
                            {
                                SubjectId = subject.SubjectId.ToString(),
                                ExamId = exam.ExamId.ToString(),
                                StudentCode = string.Empty,
                                StudentName = string.Empty,
                                SubjectName = subject.SubjectName + "(前" + Rank + "名)",
                                ExamName = exam.ExamName,
                                Mark = string.Empty,
                                TotalCount = 0
                            };
                            lst.Add(model);
                        }
                        totalCountList.Add(studentCount);
                    }
                    List.Add(new { name = subject.SubjectName, type = "bar", data = totalCountList });
                }
                vm.ExamPortraitList = lst;
                vm.chkSubject = ToJSONString(SubjectList.Select(d => d.SubjectName).ToList());
                vm.chkExam = ToJSONString(ExamList.Select(d => d.ExamName).ToList());
                vm.ReportScore = ToJSONString(List);
                return View(vm);
            }
        }
        #endregion

        #region 综合分多次考试各科前N名明细
        public ActionResult SegmentExamTopDetailList(string chkExam, string chkSubject, string chkClass, int ClassId, string rank)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var chkexamList = chkExam.Split(',');
                var chksubjectList = chkSubject.Split(',');
                var chkclassList = chkClass.Split(',');

                var Rank = rank.ConvertToDecimal();

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkexamList.Contains(p.Id.ToString())
                              && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkexamList.Contains(p.tbExam.Id.ToString())
                                   && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();

                //考试成绩
                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkexamList.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.SegmentClassRank,
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.Id == ClassId
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              t.StudentCode,
                              t.StudentName,
                              SubjectId = p.SubjectId,
                              ClassRank = p.SegmentClassRank,
                              p.SegmentMark,
                              p.ExamId,
                          }).ToList();

                var List = new List<object>();
                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in SubjectList)
                {
                    var totalCountList = new List<object>();
                    foreach (var exam in ExamList)
                    {
                        //分数段人数
                        var tm = (from p in tg
                                  where p.SubjectId == subject.SubjectId
                                  && p.ExamId == exam.ExamId
                                  && p.ClassRank > decimal.Zero && p.ClassRank <= Rank
                                  select new
                                  {
                                      ExamId = p.ExamId,
                                      SubjectName = subject.SubjectName + "(前" + Rank + "名)",
                                      ExamName = exam.ExamName,
                                      p.StudentCode,
                                      p.StudentName,
                                      p.SegmentMark
                                  }).ToList();
                        var studentCount = tm.Count();
                        if (studentCount > decimal.Zero)
                        {
                            var tb = (from p in tm
                                      select new Exam.Dto.ExamPortrait.List
                                      {
                                          ExamId = exam.ExamId.ToString(),
                                          StudentCode = p.StudentCode,
                                          StudentName = p.StudentName,
                                          SubjectName = p.SubjectName,
                                          ExamName = exam.ExamName,
                                          Mark = p.SegmentMark.ToString(),
                                          TotalCount = studentCount
                                      }).ToList();
                            lst.AddRange(tb);
                        }
                        else
                        {
                            var model = new Exam.Dto.ExamPortrait.List()
                            {
                                ExamId = exam.ExamId.ToString(),
                                StudentCode = string.Empty,
                                StudentName = string.Empty,
                                SubjectName = subject.SubjectName + "(前" + Rank + "名)",
                                ExamName = exam.ExamName,
                                Mark = string.Empty,
                                TotalCount = 0
                            };
                            lst.Add(model);
                        }
                        totalCountList.Add(studentCount);
                    }
                    List.Add(new { name = subject.SubjectName, type = "bar", data = totalCountList });
                }
                vm.ExamPortraitList = lst;
                vm.chkSubject = ToJSONString(SubjectList.Select(d => d.SubjectName).ToList());
                vm.chkExam = ToJSONString(ExamList.Select(d => d.ExamName).ToList());
                vm.ReportScore = ToJSONString(List);
                return View(vm);
            }
        }
        #endregion

        #region 考试分总分分数
        public ActionResult TotalMarkTotalHistory()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDisable == false
                                    && p.YearType == Code.EnumHelper.YearType.Section
                                group p by new { p.tbYearParent.tbYearParent.Id, p.tbYearParent.tbYearParent.YearName, p.IsDefault } into g
                                select new
                                {
                                    g.Key.Id,
                                    g.Key.YearName,
                                    g.Key.IsDefault
                                }).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //默认当前激活学年
                if (vm.YearId == 0 && vm.YearList.Count() > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id;
                }
                //获取年级
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.tbYear.tbYearParent.tbYearParent.Id == vm.YearId
                                 && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //绑定考试
                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString()
                               }).ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && p.tbYear.Id == vm.YearId
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                //绑定班级
                vm.ClassList = (from p in ClassList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ClassName,
                                    Value = p.ClassId.ToString()
                                }).ToList();

                //考试科目
                var chkExams = (vm.chkExam != null && vm.chkExam != string.Empty) ? vm.chkExam.Split(',') : new string[] { };
                foreach (var chkExam in chkExams)
                {
                    if (vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Value;
                        listItem.Text = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Text;
                        vm.ExamIds.Add(listItem);
                    }
                }

                //获取科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkExams.Contains(p.tbExam.Id.ToString())
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();
                foreach (var Subject in SubjectList)
                {
                    if (vm.SubjectList.Where(c => c.Value == Subject.SubjectId.ToString()).FirstOrDefault() == null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Text = Subject.SubjectName;
                        listItem.Value = Subject.SubjectId.ToString();
                        vm.SubjectList.Add(listItem);
                    }
                }

                var chkSubjects = (vm.chkSubject != null && vm.chkSubject != string.Empty) ? vm.chkSubject.Split(',') : new string[] { };
                foreach (var chkSubject in chkSubjects)
                {
                    if (chkSubject == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkSubject;
                        vm.SubjectIds.Add(listItem);
                    }
                    if (vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Value;
                        listItem.Text = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Text;
                        vm.SubjectIds.Add(listItem);
                    }
                }

                //年级学生班级
                var chkClasss = (vm.chkClass != null && vm.chkClass != string.Empty) ? vm.chkClass.Split(',') : new string[] { };
                foreach (var chkClass in chkClasss)
                {
                    if (chkClass == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                    if (vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                }

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var examScoreList = (from p in tf
                                     join t in classStudent
                                     on p.StudentId equals t.StudentId
                                     select new Dto.ExamPortrait.List
                                     {
                                         ExamId = p.ExamId.ToString(),
                                         StudentId = p.StudentId.ToString(),
                                         ClassName = t.ClassName,
                                         ClassId = t.ClassId.ToString(),
                                         StudentCode = t.StudentCode,
                                         StudentName = t.StudentName,
                                         SubjectId = p.SubjectId.ToString(),
                                         TotalMark = p.TotalMark.ToString(),
                                         TotalClassRank = p.TotalClassRank.ToString(),
                                         TotalGradeRank = p.TotalGradeRank.ToString(),
                                     }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                lst = (from p in examScoreList
                       group p by new { p.StudentId, p.StudentCode, p.StudentName, p.ClassId, p.ClassName, p.ExamId } into g
                       select new Dto.ExamPortrait.List
                       {
                           ExamId = g.Key.ExamId.ToString(),
                           StudentId = g.Key.StudentId.ToString(),
                           ClassName = g.Key.ClassName,
                           StudentCode = g.Key.StudentCode,
                           StudentName = g.Key.StudentName,
                           ClassId = g.Key.ClassId,
                           TotalHistory = g.Sum(d => d.TotalMark.ConvertToDecimal()),
                       }).ToList();

                //排名
                foreach (var exam in vm.ExamIds)
                {
                    //年级排名
                    var gradeRank = decimal.Zero;
                    decimal? gradeMark = null;
                    var gradeCount = decimal.One;
                    foreach (var t in lst.Where(d => d.ExamId == exam.Value).OrderByDescending(d => d.TotalHistory))
                    {
                        if (gradeMark != t.TotalHistory)
                        {
                            gradeMark = t.TotalHistory;
                            gradeRank = gradeRank + gradeCount;
                            gradeCount = decimal.One;
                        }
                        else
                        {
                            gradeCount = gradeCount + decimal.One;
                        }

                        t.GradeRank = gradeRank;
                    }
                    //班级排名
                    foreach (var classs in vm.ClassIds)
                    {
                        var classRank = decimal.Zero;
                        decimal? classMark = null;
                        var classCount = decimal.One;
                        foreach (var t in lst.Where(d => d.ClassId == classs.Value && d.ExamId == exam.Value).OrderByDescending(d => d.TotalHistory))
                        {
                            if (classMark != t.TotalHistory)
                            {
                                classMark = t.TotalHistory;
                                classRank = classRank + classCount;
                                classCount = decimal.One;
                            }
                            else
                            {
                                classCount = classCount + decimal.One;
                            }

                            t.ClassRank = classRank;
                        }
                    }
                }
                vm.ExamPortraitList = (from p in lst
                                       where (string.IsNullOrEmpty(vm.SearchText) ? true : (p.StudentCode.Contains(vm.SearchText) || p.StudentName.Contains(vm.SearchText)))
                                       select p).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TotalMarkTotalHistory(Models.ExamPortrait.List vm)
        {
            var arrystr = string.Empty;
            var chkExamList = Request.Form["chkExam"] != null ? Request.Form["chkExam"].ToString() : arrystr;
            var chkClassList = Request.Form["chkClass"] != null ? Request.Form["chkClass"].ToString() : arrystr;
            var chkSubjectList = Request.Form["chkSubject"] != null ? Request.Form["chkSubject"].ToString() : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("TotalMarkTotalHistory", new { YearId = vm.YearId, GradeId = vm.GradeId, chkExam = chkExamList, chkSubject = chkSubjectList, chkClass = chkClassList, searchText = vm.SearchText }));
        }
        #endregion

        #region 考试分总分分数明细
        public ActionResult TotalMarkTotalHistoryDetail(string chkExam, string chkSubject, string chkClass, string studentCode)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var chkexamList = chkExam.Split(',');
                var chksubjectList = chkSubject.Split(',');
                var chkclassList = chkClass.Split(',');

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkexamList.Contains(p.Id.ToString())
                              && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString(),
                               }).ToList();

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkexamList.Contains(p.tbExam.Id.ToString())
                                   && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
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
                                      Value = p.SubjectId.ToString(),
                                  }).ToList();

                //考试成绩
                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkexamList.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalClassRank,
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbStudent.StudentCode == studentCode
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                vm.ExamPortraitList = (from p in tf
                                       join t in classStudent
                                       on p.StudentId equals t.StudentId
                                       select new Dto.ExamPortrait.List
                                       {
                                           StudentId = p.StudentId.ToString(),
                                           StudentCode = t.StudentCode,
                                           StudentName = t.StudentName,
                                           SubjectId = p.SubjectId.ToString(),
                                           TotalMark = p.TotalMark.ToString(),
                                           ExamId = p.ExamId.ToString(),
                                       }).ToList();

                var examScoreList = (from p in vm.ExamPortraitList
                                     group p by new { p.ExamId } into g
                                     select new Dto.ExamPortrait.List
                                     {
                                         ExamId = g.Key.ExamId.ToString(),
                                         TotalHistory = g.Sum(d => d.TotalMark.ConvertToDecimal()),
                                     }).ToList();

                var totalExamList = new List<object>();
                var totalHistoryList = new List<object>();
                foreach (var exam in ExamList)
                {
                    var examScore = examScoreList.Where(d => d.ExamId == exam.ExamId.ToString()).FirstOrDefault();
                    if (examScore != null)
                    {
                        totalHistoryList.Add(examScore.TotalHistory);
                    }
                }
                totalExamList.Add(new { name = "总分", type = "line", data = totalHistoryList });
                vm.chkSubject = ToJSONString(SubjectList.Select(d => d.SubjectName).ToList());
                vm.chkExam = ToJSONString(ExamList.Select(d => d.ExamName).ToList());
                vm.ReportScore = ToJSONString(totalExamList);
                return View(vm);
            }
        }
        #endregion

        #region 考试分综合成绩分析
        public ActionResult SegmentTotalHistory()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDisable == false
                                    && p.YearType == Code.EnumHelper.YearType.Section
                                group p by new { p.tbYearParent.tbYearParent.Id, p.tbYearParent.tbYearParent.YearName, p.IsDefault } into g
                                select new
                                {
                                    g.Key.Id,
                                    g.Key.YearName,
                                    g.Key.IsDefault
                                }).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //默认当前激活学年
                if (vm.YearId == 0 && vm.YearList.Count() > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id;
                }
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.tbYear.tbYearParent.tbYearParent.Id == vm.YearId
                                 && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString()
                               }).ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && p.tbYear.Id == vm.YearId
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                vm.ClassList = (from p in ClassList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ClassName,
                                    Value = p.ClassId.ToString()
                                }).ToList();

                //考试科目
                var chkExams = (vm.chkExam != null && vm.chkExam != string.Empty) ? vm.chkExam.Split(',') : new string[] { };
                foreach (var chkExam in chkExams)
                {
                    if (vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Value;
                        listItem.Text = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Text;
                        vm.ExamIds.Add(listItem);
                    }
                }

                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkExams.Contains(p.tbExam.Id.ToString())
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();
                foreach (var Subject in SubjectList)
                {
                    if (vm.SubjectList.Where(c => c.Value == Subject.SubjectId.ToString()).FirstOrDefault() == null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Text = Subject.SubjectName;
                        listItem.Value = Subject.SubjectId.ToString();
                        vm.SubjectList.Add(listItem);
                    }
                }

                var chkSubjects = (vm.chkSubject != null && vm.chkSubject != string.Empty) ? vm.chkSubject.Split(',') : new string[] { };
                foreach (var chkSubject in chkSubjects)
                {
                    if (chkSubject == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkSubject;
                        vm.SubjectIds.Add(listItem);
                    }
                    if (vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Value;
                        listItem.Text = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Text;
                        vm.SubjectIds.Add(listItem);
                    }
                }

                //年级学生班级
                var chkClasss = (vm.chkClass != null && vm.chkClass != string.Empty) ? vm.chkClass.Split(',') : new string[] { };
                foreach (var chkClass in chkClasss)
                {
                    if (chkClass == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                    if (vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                }

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())

                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.SegmentMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())

                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var examScoreList = (from p in tf
                                     join t in classStudent
                                     on p.StudentId equals t.StudentId
                                     select new Dto.ExamPortrait.List
                                     {
                                         ExamId = p.ExamId.ToString(),
                                         StudentId = p.StudentId.ToString(),
                                         ClassName = t.ClassName,
                                         ClassId = t.ClassId.ToString(),
                                         StudentCode = t.StudentCode,
                                         StudentName = t.StudentName,
                                         SubjectId = p.SubjectId.ToString(),
                                         SegmentMark = p.SegmentMark.ToString(),
                                         TotalClassRank = p.TotalClassRank.ToString(),
                                         TotalGradeRank = p.TotalGradeRank.ToString(),
                                     }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                lst = (from p in examScoreList
                       group p by new { p.StudentId, p.StudentCode, p.StudentName, p.ClassId, p.ClassName, p.ExamId } into g
                       select new Dto.ExamPortrait.List
                       {
                           ExamId = g.Key.ExamId.ToString(),
                           StudentId = g.Key.StudentId.ToString(),
                           ClassName = g.Key.ClassName,
                           StudentCode = g.Key.StudentCode,
                           StudentName = g.Key.StudentName,
                           ClassId = g.Key.ClassId,
                           TotalHistory = g.Sum(d => d.SegmentMark != null ? d.SegmentMark.ConvertToDecimal() : 0),
                       }).ToList();

                //排名
                foreach (var exam in vm.ExamIds)
                {
                    //年级排名
                    var gradeRank = decimal.Zero;
                    decimal? gradeMark = null;
                    var gradeCount = decimal.One;
                    foreach (var t in lst.Where(d => d.ExamId == exam.Value).OrderByDescending(d => d.TotalHistory))
                    {
                        if (gradeMark != t.TotalHistory)
                        {
                            gradeMark = t.TotalHistory;
                            gradeRank = gradeRank + gradeCount;
                            gradeCount = decimal.One;
                        }
                        else
                        {
                            gradeCount = gradeCount + decimal.One;
                        }

                        t.GradeRank = gradeRank;
                    }
                    //班级排名
                    foreach (var classs in vm.ClassIds)
                    {
                        var classRank = decimal.Zero;
                        decimal? classMark = null;
                        var classCount = decimal.One;
                        foreach (var t in lst.Where(d => d.ClassId == classs.Value && d.ExamId == exam.Value).OrderByDescending(d => d.TotalHistory))
                        {
                            if (classMark != t.TotalHistory)
                            {
                                classMark = t.TotalHistory;
                                classRank = classRank + classCount;
                                classCount = decimal.One;
                            }
                            else
                            {
                                classCount = classCount + decimal.One;
                            }

                            t.ClassRank = classRank;
                        }
                    }
                }

                vm.ExamPortraitList = (from p in lst
                                       where (string.IsNullOrEmpty(vm.SearchText) ? true : (p.StudentCode.Contains(vm.SearchText) || p.StudentName.Contains(vm.SearchText)))
                                       select p).ToList();

                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SegmentTotalHistory(Models.ExamPortrait.List vm)
        {
            var arrystr = string.Empty;
            var chkExamList = Request.Form["chkExam"] != null ? Request.Form["chkExam"].ToString() : arrystr;
            var chkClassList = Request.Form["chkClass"] != null ? Request.Form["chkClass"].ToString() : arrystr;
            var chkSubjectList = Request.Form["chkSubject"] != null ? Request.Form["chkSubject"].ToString() : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("SegmentTotalHistory", new { YearId = vm.YearId, GradeId = vm.GradeId, chkExam = chkExamList, chkSubject = chkSubjectList, chkClass = chkClassList, searchText = vm.SearchText }));
        }
        #endregion

        #region 考试分综合成绩分析详细
        public ActionResult SegmentTotalHistoryDetail(string chkExam, string chkSubject, string chkClass, string studentCode)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var chkexamList = chkExam.Split(',');
                var chksubjectList = chkSubject.Split(',');
                var chkclassList = chkClass.Split(',');

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkexamList.Contains(p.Id.ToString())
                              && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString(),
                               }).ToList();

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkexamList.Contains(p.tbExam.Id.ToString())
                                   && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   && chksubjectList.Contains(p.tbCourse.tbSubject.Id.ToString())
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
                                      Value = p.SubjectId.ToString(),
                                  }).ToList();

                //考试成绩
                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkexamList.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbStudent.StudentCode == studentCode
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                vm.ExamPortraitList = (from p in tf
                                       join t in classStudent
                                       on p.StudentId equals t.StudentId
                                       select new Dto.ExamPortrait.List
                                       {
                                           StudentId = p.StudentId.ToString(),
                                           StudentCode = t.StudentCode,
                                           StudentName = t.StudentName,
                                           SubjectId = p.SubjectId.ToString(),
                                           SegmentMark = p.SegmentMark.ToString(),
                                           ExamId = p.ExamId.ToString(),
                                       }).ToList();

                var examScoreList = (from p in vm.ExamPortraitList
                                     group p by new { p.ExamId } into g
                                     select new Dto.ExamPortrait.List
                                     {
                                         ExamId = g.Key.ExamId.ToString(),
                                         TotalHistory = g.Sum(d => d.SegmentMark.ConvertToDecimal()),
                                     }).ToList();

                var totalExamList = new List<object>();
                var totalHistoryList = new List<object>();
                foreach (var exam in ExamList)
                {
                    var examScore = examScoreList.Where(d => d.ExamId == exam.ExamId.ToString()).FirstOrDefault();
                    if (examScore != null)
                    {
                        totalHistoryList.Add(examScore.TotalHistory);
                    }
                }
                totalExamList.Add(new { name = "总分", type = "line", data = totalHistoryList });
                vm.chkSubject = ToJSONString(SubjectList.Select(d => d.SubjectName).ToList());
                vm.chkExam = ToJSONString(ExamList.Select(d => d.ExamName).ToList());
                vm.ReportScore = ToJSONString(totalExamList);
                return View(vm);
            }
        }
        #endregion

        #region 考试成绩综合成绩析
        public ActionResult TotalMarkClassCompHistory()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDisable == false
                                    && p.YearType == Code.EnumHelper.YearType.Section
                                group p by new { p.tbYearParent.tbYearParent.Id, p.tbYearParent.tbYearParent.YearName, p.IsDefault } into g
                                select new
                                {
                                    g.Key.Id,
                                    g.Key.YearName,
                                    g.Key.IsDefault
                                }).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //默认当前激活学年
                if (vm.YearId == 0 && vm.YearList.Count() > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id;
                }
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.tbYear.tbYearParent.tbYearParent.Id == vm.YearId
                                 && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString()
                               }).ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && p.tbYear.Id == vm.YearId
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                vm.ClassList = (from p in ClassList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ClassName,
                                    Value = p.ClassId.ToString()
                                }).ToList();

                //考试科目
                var chkExams = (vm.chkExam != null && vm.chkExam != string.Empty) ? vm.chkExam.Split(',') : new string[] { };
                foreach (var chkExam in chkExams)
                {
                    if (vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Value;
                        listItem.Text = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Text;
                        vm.ExamIds.Add(listItem);
                    }
                }

                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkExams.Contains(p.tbExam.Id.ToString())
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();
                foreach (var Subject in SubjectList)
                {
                    if (vm.SubjectList.Where(c => c.Value == Subject.SubjectId.ToString()).FirstOrDefault() == null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Text = Subject.SubjectName;
                        listItem.Value = Subject.SubjectId.ToString();
                        vm.SubjectList.Add(listItem);
                    }
                }

                var chkSubjects = (vm.chkSubject != null && vm.chkSubject != string.Empty) ? vm.chkSubject.Split(',') : new string[] { };
                foreach (var chkSubject in chkSubjects)
                {
                    if (chkSubject == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkSubject;
                        vm.SubjectIds.Add(listItem);
                    }
                    if (vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Value;
                        listItem.Text = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Text;
                        vm.SubjectIds.Add(listItem);
                    }
                }

                //年级学生班级
                var chkClasss = (vm.chkClass != null && vm.chkClass != string.Empty) ? vm.chkClass.Split(',') : new string[] { };
                foreach (var chkClass in chkClasss)
                {
                    if (chkClass == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                    if (vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Value;
                        listItem.Text = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Text;
                        vm.ClassIds.Add(listItem);
                    }
                }

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              FullTotalMark = p.tbExamCourse.FullTotalMark,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    //&& (string.IsNullOrEmpty(vm.SearchText) ? true : (p.tbClass.ClassName.Contains(vm.SearchText) || p.tbClass.ClassName.Contains(vm.SearchText)))
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var examScoreList = (from p in tf
                                     join t in classStudent
                                     on p.StudentId equals t.StudentId
                                     select new Dto.ExamPortrait.List
                                     {
                                         ExamId = p.ExamId.ToString(),
                                         StudentId = p.StudentId.ToString(),
                                         ClassName = t.ClassName,
                                         ClassId = t.ClassId.ToString(),
                                         StudentCode = t.StudentCode,
                                         StudentName = t.StudentName,
                                         SubjectId = p.SubjectId.ToString(),
                                         TotalMark = p.TotalMark.ToString(),
                                         FullTotalMark = p.FullTotalMark,
                                     }).ToList();

                //获取分数段
                var examSegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                           where p.IsDeleted == false
                                           && p.tbSubject.IsDeleted == false
                                           && p.tbGrade.IsDeleted == false
                                           && p.tbGrade.Id == vm.GradeId
                                           && chkSubjects.Contains(p.tbSubject.Id.ToString())
                                           select new
                                           {
                                               SubjectId = p.tbSubject.Id.ToString(),
                                               MinMark = p.MinMark,
                                               MaxMark = p.MaxMark,
                                               IsGood = p.IsGood,
                                               IsPass = p.IsPass,
                                           }).ToList();


                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var cla in ClassList)
                {
                    foreach (var subject in SubjectList)
                    {
                        foreach (var exam in ExamList)
                        {
                            var list = new Exam.Dto.ExamPortrait.List();
                            //班级科目考试人数
                            decimal escCount = examScoreList.Where(d => d.ExamId == exam.ExamId.ToString() && d.SubjectId == subject.SubjectId.ToString() && d.ClassId == cla.ClassId.ToString()).Count();

                            //优秀及格人数
                            var gpList = (from p in examSegmentMarkList
                                          join t in examScoreList
                                          on p.SubjectId equals t.SubjectId
                                          where p.SubjectId == subject.SubjectId.ToString()
                                          && t.ClassId == cla.ClassId.ToString()
                                          && t.SubjectId == subject.SubjectId.ToString()
                                          && t.ExamId == exam.ExamId.ToString()
                                          && (p.MinMark * t.FullTotalMark / 100 <= t.TotalMark.ConvertToDecimal()
                                          && t.TotalMark.ConvertToDecimal() <= p.MaxMark * t.FullTotalMark / 100)
                                          select p).ToList();

                            //优秀人数比率
                            list.ExcellentCount = gpList.Where(d => d.IsGood == true).Count();
                            list.ExcellentRate = escCount == 0 ? "0" : decimal.Round(gpList.Where(d => d.IsGood == true).Count() / escCount * 100, 2, MidpointRounding.AwayFromZero) + "%";

                            //及格人数比率
                            list.PassingCount = gpList.Where(d => d.IsPass == true).Count();
                            list.PassingRate = escCount == 0 ? "0" : decimal.Round(gpList.Where(d => d.IsPass == true).Count() / escCount * 100, 2, MidpointRounding.AwayFromZero) + "%";

                            list.ClassId = cla.ClassId.ToString();
                            list.SubjectId = subject.SubjectId.ToString();
                            list.ExamId = exam.ExamId.ToString();
                            lst.Add(list);
                        }
                    }
                }
                vm.ExamPortraitList = lst;
                return View(vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TotalMarkClassCompHistory(Models.ExamPortrait.List vm)
        {
            var arrystr = string.Empty;
            var chkExamList = Request.Form["chkExam"] != null ? Request.Form["chkExam"].ToString() : arrystr;
            var chkClassList = Request.Form["chkClass"] != null ? Request.Form["chkClass"].ToString() : arrystr;
            var chkSubjectList = Request.Form["chkSubject"] != null ? Request.Form["chkSubject"].ToString() : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("TotalMarkClassCompHistory", new { YearId = vm.YearId, GradeId = vm.GradeId, chkExam = chkExamList, chkSubject = chkSubjectList, chkClass = chkClassList, searchText = vm.SearchText }));
        }
        #endregion

        #region 考试成绩综合成绩析明细
        public ActionResult TotalClassCompHistoryDetail(string chkExam, int SubjectId, int GradeId, int ClassId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var chkexamList = chkExam.Split(',');

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkexamList.Contains(p.Id.ToString())
                              && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.chkExam = ToJSONString(ExamList.Select(d => d.ExamName).ToList());
                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString(),
                               }).ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkexamList.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && p.tbExamCourse.tbCourse.tbSubject.Id == SubjectId
                          select new
                          {
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                              FullTotalMark = p.tbExamCourse.FullTotalMark,
                          }).ToList();

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.Id == ClassId
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new Dto.ExamPortrait.List
                          {
                              StudentId = p.StudentId.ToString(),
                              SubjectId = p.SubjectId.ToString(),
                              TotalMark = p.TotalMark.ToString(),
                              ClassId = t.ClassId.ToString(),
                              ClassName = t.ClassName,
                              ExamId = p.ExamId.ToString(),
                              FullTotalMark = p.FullTotalMark,
                              StudentCode = t.StudentCode,
                              StudentName = t.StudentName,
                          }).ToList();

                //获取分数段
                var examSegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                           where p.IsDeleted == false
                                           && p.tbSubject.IsDeleted == false
                                           && p.tbGrade.IsDeleted == false
                                           && p.tbGrade.Id == GradeId
                                           && p.tbSubject.Id == SubjectId
                                           select new
                                           {
                                               SubjectId = p.tbSubject.Id.ToString(),
                                               MinMark = p.MinMark,
                                               MaxMark = p.MaxMark,
                                               IsGood = p.IsGood,
                                               IsPass = p.IsPass,
                                           }).ToList();

                var lstTotal = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var exam in ExamList)
                {
                    //每个学生成绩优秀以及及格分析
                    foreach (var student in classStudent)
                    {
                        var goodCount = decimal.Zero;
                        var passCount = decimal.Zero;
                        var goodNo = decimal.Zero;
                        var passNo = decimal.Zero;
                        foreach (var o in examSegmentMarkList)
                        {
                            var isGood = o.IsGood;
                            var isPass = o.IsPass;
                            var ty = (from p in tg
                                      where p.ExamId == exam.ExamId.ToString() && p.StudentId == student.StudentId.ToString()
                                      && p.TotalMark.ConvertToDecimal() >= o.MinMark * p.FullTotalMark / 100 && p.TotalMark.ConvertToDecimal() <= o.MaxMark * p.FullTotalMark / 100
                                      orderby p.TotalMark descending
                                      select new
                                      {
                                          isGood,
                                          isPass,
                                          p.TotalMark
                                      }).ToList();

                            goodCount = ty.Where(d => d.isGood == true).Count();
                            passCount = ty.Where(d => d.isPass == true).Count();

                            if (goodCount > decimal.Zero)
                            {
                                goodNo++;
                            }

                            if (passCount > decimal.Zero)
                            {
                                passNo++;
                            }
                        }
                        var model = new Exam.Dto.ExamPortrait.List();
                        model.StudentCode = student.StudentCode;
                        model.StudentName = student.StudentName;
                        model.ExamId = exam.ExamId.ToString();
                        model.ExamName = exam.ExamName;
                        model.Mark = tg.Where(d => d.ExamId == exam.ExamId.ToString() && d.StudentId == student.StudentId.ToString()).Select(d => d.TotalMark).FirstOrDefault() != null ?
                            tg.Where(d => d.ExamId == exam.ExamId.ToString() && d.StudentId == student.StudentId.ToString()).Select(d => d.TotalMark).FirstOrDefault().ToString() : string.Empty;
                        model.IsGood = goodNo > decimal.Zero ? "是" : string.Empty;
                        model.IsPass = passNo > decimal.Zero ? "是" : string.Empty;

                        lstTotal.Add(model);
                    }

                    vm.ExamPortraitList = lstTotal;
                }
                return View(vm);
            }
        }
        #endregion

        #region 综合成绩综合成绩析
        public ActionResult SegmentClassCompHistory()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var yearList = (from p in db.Table<Basis.Entity.tbYear>()
                                where p.IsDisable == false
                                    && p.YearType == Code.EnumHelper.YearType.Section
                                group p by new { p.tbYearParent.tbYearParent.Id, p.tbYearParent.tbYearParent.YearName, p.IsDefault } into g
                                select new
                                {
                                    g.Key.Id,
                                    g.Key.YearName,
                                    g.Key.IsDefault
                                }).ToList();
                vm.YearList = (from p in yearList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.YearName,
                                   Value = p.Id.ToString(),
                               }).ToList();
                //默认当前激活学年
                if (vm.YearId == 0 && vm.YearList.Count() > 0)
                {
                    vm.YearId = yearList.Where(d => d.IsDefault == true).FirstOrDefault().Id;
                }
                vm.GradeList = Areas.Basis.Controllers.GradeController.SelectList();
                if (vm.GradeId == 0 && vm.GradeList.Count > 0)
                {
                    vm.GradeId = vm.GradeList.FirstOrDefault().Value.ConvertToInt();
                }

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where p.tbYear.tbYearParent.tbYearParent.Id == vm.YearId
                                 && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString()
                               }).ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && p.tbYear.Id == vm.YearId
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                vm.ClassList = (from p in ClassList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ClassName,
                                    Value = p.ClassId.ToString()
                                }).ToList();

                //考试科目
                var chkExams = (vm.chkExam != null && vm.chkExam != string.Empty) ? vm.chkExam.Split(',') : new string[] { };
                foreach (var chkExam in chkExams)
                {
                    if (vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Value;
                        listItem.Text = vm.ExamList.Where(d => d.Value == chkExam).FirstOrDefault().Text;
                        vm.ExamIds.Add(listItem);
                    }
                }

                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where chkExams.Contains(p.tbExam.Id.ToString())
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id
                                   }).Distinct().ToList();
                foreach (var Subject in SubjectList)
                {
                    if (vm.SubjectList.Where(c => c.Value == Subject.SubjectId.ToString()).FirstOrDefault() == null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Text = Subject.SubjectName;
                        listItem.Value = Subject.SubjectId.ToString();
                        vm.SubjectList.Add(listItem);
                    }
                }

                var chkSubjects = (vm.chkSubject != null && vm.chkSubject != string.Empty) ? vm.chkSubject.Split(',') : new string[] { };
                foreach (var chkSubject in chkSubjects)
                {
                    if (chkSubject == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkSubject;
                        vm.SubjectIds.Add(listItem);
                    }
                    if (vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Value;
                        listItem.Text = vm.SubjectList.Where(d => d.Value == chkSubject).FirstOrDefault().Text;
                        vm.SubjectIds.Add(listItem);
                    }
                }

                //年级学生班级
                var chkClasss = (vm.chkClass != null && vm.chkClass != string.Empty) ? vm.chkClass.Split(',') : new string[] { };
                foreach (var chkClass in chkClasss)
                {
                    if (chkClass == "-1")
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = chkClass;
                        vm.ClassIds.Add(listItem);
                    }
                    if (vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault() != null)
                    {
                        var listItem = new System.Web.Mvc.SelectListItem();
                        listItem.Value = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Value;
                        listItem.Text = vm.ClassList.Where(d => d.Value == chkClass).FirstOrDefault().Text;
                        vm.ClassIds.Add(listItem);
                    }
                }

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              FullSegmentMark = p.tbExamCourse.FullSegmentMark,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    //&& (string.IsNullOrEmpty(vm.SearchText) ? true : (p.tbClass.ClassName.Contains(vm.SearchText) || p.tbClass.ClassName.Contains(vm.SearchText)))
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var examScoreList = (from p in tf
                                     join t in classStudent
                                     on p.StudentId equals t.StudentId
                                     select new Dto.ExamPortrait.List
                                     {
                                         ExamId = p.ExamId.ToString(),
                                         StudentId = p.StudentId.ToString(),
                                         ClassName = t.ClassName,
                                         ClassId = t.ClassId.ToString(),
                                         StudentCode = t.StudentCode,
                                         StudentName = t.StudentName,
                                         SubjectId = p.SubjectId.ToString(),
                                         SegmentMark = p.SegmentMark.ToString(),
                                         FullSegmentMark = p.FullSegmentMark,
                                     }).ToList();

                //获取分数段
                var examSegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                           where p.IsDeleted == false
                                           && p.tbSubject.IsDeleted == false
                                           && p.tbGrade.IsDeleted == false
                                           && p.tbGrade.Id == vm.GradeId
                                           && chkSubjects.Contains(p.tbSubject.Id.ToString())
                                           select new
                                           {
                                               SubjectId = p.tbSubject.Id.ToString(),
                                               MinMark = p.MinMark,
                                               MaxMark = p.MaxMark,
                                               IsGood = p.IsGood,
                                               IsPass = p.IsPass,
                                           }).ToList();



                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var cla in ClassList)
                {
                    foreach (var subject in SubjectList)
                    {
                        foreach (var exam in ExamList)
                        {
                            var list = new Exam.Dto.ExamPortrait.List();
                            //班级科目考试人数
                            decimal escCount = examScoreList.Where(d => d.ExamId == exam.ExamId.ToString() && d.SubjectId == subject.SubjectId.ToString() && d.ClassId == cla.ClassId.ToString()).Count();

                            //优秀及格人数
                            var gpList = (from p in examSegmentMarkList
                                          join t in examScoreList
                                          on p.SubjectId equals t.SubjectId
                                          where p.SubjectId == subject.SubjectId.ToString()
                                          && t.ClassId == cla.ClassId.ToString()
                                          && t.SubjectId == subject.SubjectId.ToString()
                                          && t.ExamId == exam.ExamId.ToString()
                                          && (p.MinMark * t.FullSegmentMark / 100 <= t.SegmentMark.ConvertToDecimal()
                                          && t.SegmentMark.ConvertToDecimal() <= p.MaxMark * t.FullSegmentMark / 100)
                                          select p).ToList();

                            //优秀人数比率
                            list.ExcellentCount = gpList.Where(d => d.IsGood == true).Count();
                            list.ExcellentRate = escCount == 0 ? "0" : decimal.Round(gpList.Where(d => d.IsGood == true).Count() / escCount * 100, 2, MidpointRounding.AwayFromZero) + "%";

                            //及格人数比率
                            list.PassingCount = gpList.Where(d => d.IsPass == true).Count();
                            list.PassingRate = escCount == 0 ? "0" : decimal.Round(gpList.Where(d => d.IsPass == true).Count() / escCount * 100, 2, MidpointRounding.AwayFromZero) + "%";

                            list.ClassId = cla.ClassId.ToString();
                            list.SubjectId = subject.SubjectId.ToString();
                            list.ExamId = exam.ExamId.ToString();
                            lst.Add(list);
                        }
                    }
                }
                vm.ExamPortraitList = lst;
                return View(vm);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SegmentClassCompHistory(Models.ExamPortrait.List vm)
        {
            var arrystr = string.Empty;
            var chkExamList = Request.Form["chkExam"] != null ? Request.Form["chkExam"].ToString() : arrystr;
            var chkClassList = Request.Form["chkClass"] != null ? Request.Form["chkClass"].ToString() : arrystr;
            var chkSubjectList = Request.Form["chkSubject"] != null ? Request.Form["chkSubject"].ToString() : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("SegmentClassCompHistory", new { YearId = vm.YearId, GradeId = vm.GradeId, chkExam = chkExamList, chkSubject = chkSubjectList, chkClass = chkClassList, searchText = vm.SearchText }));
        }
        #endregion

        #region 综合成绩综合成绩析明细
        public ActionResult SegmentCompHistoryDetail(string chkExam, int SubjectId, int GradeId, int ClassId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamPortrait.List();
                var chkexamList = chkExam.Split(',');

                //考试
                var ExamList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkexamList.Contains(p.Id.ToString())
                              && p.IsPublish == true && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                vm.chkExam = ToJSONString(ExamList.Select(d => d.ExamName).ToList());
                vm.ExamList = (from p in ExamList
                               select new System.Web.Mvc.SelectListItem
                               {
                                   Text = p.ExamName,
                                   Value = p.ExamId.ToString(),
                               }).ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkexamList.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && p.tbExamCourse.tbCourse.tbSubject.Id == SubjectId
                          select new
                          {
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                              FullSegmentMark = p.tbExamCourse.FullSegmentMark,
                          }).ToList();

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.Id == ClassId
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new Dto.ExamPortrait.List
                          {
                              StudentId = p.StudentId.ToString(),
                              SubjectId = p.SubjectId.ToString(),
                              SegmentMark = p.SegmentMark.ToString(),
                              ClassId = t.ClassId.ToString(),
                              ClassName = t.ClassName,
                              ExamId = p.ExamId.ToString(),
                              FullSegmentMark = p.FullSegmentMark,
                              StudentCode = t.StudentCode,
                              StudentName = t.StudentName,
                          }).ToList();

                //获取分数段
                var examSegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                           where p.IsDeleted == false
                                           && p.tbSubject.IsDeleted == false
                                           && p.tbGrade.IsDeleted == false
                                           && p.tbGrade.Id == GradeId
                                           && p.tbSubject.Id == SubjectId
                                           select new
                                           {
                                               SubjectId = p.tbSubject.Id.ToString(),
                                               MinMark = p.MinMark,
                                               MaxMark = p.MaxMark,
                                               IsGood = p.IsGood,
                                               IsPass = p.IsPass,
                                           }).ToList();


                var lstTotal = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var exam in ExamList)
                {
                    //每个学生成绩优秀以及及格分析
                    foreach (var student in classStudent)
                    {
                        var goodCount = decimal.Zero;
                        var passCount = decimal.Zero;
                        var goodNo = decimal.Zero;
                        var passNo = decimal.Zero;
                        foreach (var o in examSegmentMarkList)
                        {
                            var isGood = o.IsGood;
                            var isPass = o.IsPass;
                            var ty = (from p in tg
                                      where p.ExamId == exam.ExamId.ToString() && p.StudentId == student.StudentId.ToString()
                                      && p.SegmentMark.ConvertToDecimal() >= o.MinMark * p.FullSegmentMark / 100 && p.SegmentMark.ConvertToDecimal() <= o.MaxMark * p.FullSegmentMark / 100
                                      orderby p.TotalMark descending
                                      select new
                                      {
                                          isGood,
                                          isPass,
                                          p.TotalMark
                                      }).ToList();

                            goodCount = ty.Where(d => d.isGood == true).Count();
                            passCount = ty.Where(d => d.isPass == true).Count();

                            if (goodCount > decimal.Zero)
                            {
                                goodNo++;
                            }

                            if (passCount > decimal.Zero)
                            {
                                passNo++;
                            }
                        }
                        var model = new Exam.Dto.ExamPortrait.List();
                        model.StudentCode = student.StudentCode;
                        model.StudentName = student.StudentName;
                        model.ExamId = exam.ExamId.ToString();
                        model.ExamName = exam.ExamName;
                        model.Mark = tg.Where(d => d.ExamId == exam.ExamId.ToString() && d.StudentId == student.StudentId.ToString()).Select(d => d.SegmentMark).FirstOrDefault() != null ?
                            tg.Where(d => d.ExamId == exam.ExamId.ToString() && d.StudentId == student.StudentId.ToString()).Select(d => d.SegmentMark).FirstOrDefault().ToString() : string.Empty;
                        model.IsGood = goodNo > decimal.Zero ? "是" : string.Empty;
                        model.IsPass = passNo > decimal.Zero ? "是" : string.Empty;

                        lstTotal.Add(model);
                    }

                    vm.ExamPortraitList = lstTotal;
                }
                return View(vm);
            }
        }
        #endregion

        #region 导出
        #region 考试分多次考试各科分数 
        public ActionResult TotalExamExport(string chkExam, string chkSubject, string chkClass, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamPortrait.List();

                if (string.IsNullOrEmpty(chkExam) || string.IsNullOrEmpty(chkSubject) || string.IsNullOrEmpty(chkClass)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var chkExams = chkExam.Split(',');
                var chkSubjects = chkSubject.Split(',');
                var chkClasss = chkClass.Split(',');

                //获取考试
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkExams.Contains(p.Id.ToString())
                                && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //获取科目
                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   where chkSubject.Contains(p.Id.ToString())
                                    && p.IsDeleted == false
                                   orderby p.No
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.Id
                                   }).Distinct().ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && chkClasss.Contains(p.Id.ToString())
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    && (string.IsNullOrEmpty(vm.SearchText) ? true : (p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)))
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName
                                    }).Distinct().ToList();

                vm.ExamPortraitList = (from p in tf
                                       join t in classStudent
                                       on p.StudentId equals t.StudentId
                                       select new Dto.ExamPortrait.List
                                       {
                                           ExamId = p.ExamId.ToString(),
                                           StudentId = p.StudentId.ToString(),
                                           ClassName = t.ClassName,
                                           StudentCode = t.StudentCode,
                                           StudentName = t.StudentName,
                                           SubjectId = p.SubjectId.ToString(),
                                           TotalMark = p.TotalMark.ToString(),
                                           TotalClassRank = p.TotalClassRank.ToString(),
                                           TotalGradeRank = p.TotalGradeRank.ToString(),
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

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("考试分多次考试各科分数") as HSSFSheet;//建立Sheet1

                var rowindex = 0;
                var cellindex = 0;
                #region 表头
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
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
                var m = 1;
                foreach (var subject in subjectList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(subject.SubjectName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex,3*examList.Count()*m+2);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = 3 * examList.Count() * m + 3;
                    m++;
                }
                rowindex++;

                cellindex = 3;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(exam.ExamName);
                        cell.CellStyle = cellstyle;
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                        cellindex = cellindex + 3;
                    }
                }
                rowindex++;

                cellindex = 3;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("考试成绩");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("班级排名");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("年级排名");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                    }
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                for (var i = 1; i < 3; i++)
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

                foreach (var student in vm.ExamPortraitList.Select(d => new { d.StudentId, d.ClassName, d.StudentCode, d.StudentName }).Distinct().ToList())
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.StudentCode);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.StudentName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    foreach (var subject in subjectList)
                    {
                        foreach (var exam in examList)
                        {
                            var examScore = vm.ExamPortraitList.Where(d => d.StudentCode == student.StudentCode && d.ExamId == exam.ExamId.ToString() && d.SubjectId == subject.SubjectId.ToString()).FirstOrDefault();
                            if (examScore != null)
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(examScore.TotalMark);
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(examScore.TotalClassRank);
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(examScore.TotalGradeRank);
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                            else
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                        }
                    }
                    rowindex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("考试分多次考试各科分数" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }
        #endregion

        #region 综合分多次考试各科分数 
        public ActionResult SegmentExamExport(string chkExam, string chkSubject, string chkClass, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamPortrait.List();

                if (string.IsNullOrEmpty(chkExam) || string.IsNullOrEmpty(chkSubject) || string.IsNullOrEmpty(chkClass)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var chkExams = chkExam.Split(',');
                var chkSubjects = chkSubject.Split(',');
                var chkClasss = chkClass.Split(',');

                //获取考试
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkExams.Contains(p.Id.ToString())
                                && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //获取科目
                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   where chkSubject.Contains(p.Id.ToString())
                                    && p.IsDeleted == false
                                   orderby p.No
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.Id
                                   }).Distinct().ToList();

                //年级学生班级
                var ClassList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && chkClasss.Contains(p.Id.ToString())
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.TotalMark,
                              p.SegmentMark,
                              p.SegmentClassRank,
                              p.SegmentGradeRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                //获取班级学生
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    && (string.IsNullOrEmpty(vm.SearchText) ? true : (p.tbStudent.StudentCode.Contains(vm.SearchText) || p.tbStudent.StudentName.Contains(vm.SearchText)))
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName
                                    }).Distinct().ToList();

                vm.ExamPortraitList = (from p in tf
                                       join t in classStudent
                                       on p.StudentId equals t.StudentId
                                       select new Dto.ExamPortrait.List
                                       {
                                           ExamId = p.ExamId.ToString(),
                                           StudentId = p.StudentId.ToString(),
                                           ClassName = t.ClassName,
                                           StudentCode = t.StudentCode,
                                           StudentName = t.StudentName,
                                           SubjectId = p.SubjectId.ToString(),
                                           SegmentMark = p.SegmentMark.ToString(),
                                           SegmentClassRank = p.SegmentClassRank.ToString(),
                                           SegmentGradeRank = p.SegmentGradeRank.ToString(),
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

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("综合分多次考试各科分数") as HSSFSheet;//建立Sheet1

                var rowindex = 0;
                var cellindex = 0;
                #region 表头
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
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
                var m = 1;
                foreach (var subject in subjectList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(subject.SubjectName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, 3 * examList.Count() * m + 2);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = 3 * examList.Count() * m + 3;
                    m++;
                }
                rowindex++;

                cellindex = 3;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(exam.ExamName);
                        cell.CellStyle = cellstyle;
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                        cellindex = cellindex + 3;
                    }
                }
                rowindex++;

                cellindex = 3;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("综合分成绩");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("班级排名");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("年级排名");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                    }
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                for (var i = 1; i < 3; i++)
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

                foreach (var student in vm.ExamPortraitList.Select(d => new { d.StudentId, d.ClassName, d.StudentCode, d.StudentName }).Distinct().ToList())
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.StudentCode);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.StudentName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    foreach (var subject in subjectList)
                    {
                        foreach (var exam in examList)
                        {
                            var examScore = vm.ExamPortraitList.Where(d => d.StudentCode == student.StudentCode && d.ExamId == exam.ExamId.ToString() && d.SubjectId == subject.SubjectId.ToString()).FirstOrDefault();
                            if (examScore != null)
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(examScore.SegmentMark);
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(examScore.SegmentClassRank);
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(examScore.SegmentGradeRank);
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                            else
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                        }
                    }
                    rowindex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("综合分多次考试各科分数" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }
        #endregion

        #region 考试分多次考试班级平均分 
        public ActionResult TotalExamAvgExport(string chkExam, string chkSubject, string chkClass, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamPortrait.List();

                if (string.IsNullOrEmpty(chkExam) || string.IsNullOrEmpty(chkSubject) || string.IsNullOrEmpty(chkClass)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var chkExams = chkExam.Split(',');
                var chkSubjects = chkSubject.Split(',');
                var chkClasss = chkClass.Split(',');

                //获取考试
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkExams.Contains(p.Id.ToString())
                                && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //获取科目
                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   where chkSubject.Contains(p.Id.ToString())
                                    && p.IsDeleted == false
                                   orderby p.No
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.Id
                                   }).Distinct().ToList();

                //年级学生班级
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && chkClasss.Contains(p.Id.ToString())
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              TotalMark = p.TotalMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              ExamId = p.ExamId,
                          }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        var classAvgList = (from p in tg
                                            where p.SubjectId.ToString() == subject.SubjectId.ToString()
                                            && p.ExamId.ToString() == exam.ExamId.ToString()
                                            group p by new
                                            {
                                                p.ClassId,
                                                p.ClassName
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                g.Key.ClassName,
                                                AvgMark = g.Average(d => d.TotalMark)
                                            }).ToList();

                        var tb = (from p in classAvgList
                                  select new Exam.Dto.ExamPortrait.List
                                  {
                                      ClassId = p.ClassId.ToString(),
                                      SubjectId = subject.SubjectId.ToString(),
                                      ClassName = p.ClassName,
                                      ExamId = exam.ExamId.ToString(),
                                      AvgMark = p.AvgMark != null ? decimal.Round((decimal)p.AvgMark, 2, MidpointRounding.AwayFromZero) : 0,
                                      ClassRank = decimal.Zero
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }

                //排名
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        var rank = decimal.Zero;
                        decimal? mark = null;
                        var count = decimal.One;
                        foreach (var t in lst.Where(d => d.SubjectId == subject.SubjectId.ToString() && d.ExamId == exam.ExamId.ToString()).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = decimal.One;
                            }
                            else
                            {
                                count = count + decimal.One;
                            }

                            t.ClassRank = rank;
                        }
                    }
                }
                vm.ExamPortraitList = (from p in lst
                                       where (string.IsNullOrEmpty(vm.SearchText) ? true : (p.ClassName.Contains(vm.SearchText)))
                                       select p).ToList();
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

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("考试分多次考试班级平均分") as HSSFSheet;//建立Sheet1

                var rowindex = 0;
                var cellindex = 0;
                #region 表头
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                foreach (var subject in subjectList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(subject.SubjectName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2 * examList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 2 * examList.Count();
                }
                rowindex++;

                cellindex = 1;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(exam.ExamName);
                        cell.CellStyle = cellstyle;
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                        cellindex = cellindex + 2;
                    }
                }
                rowindex++;

                cellindex = 1;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("平均分");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("多班排名");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                    }
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                for (var i = 1; i < 3; i++)
                {
                    sheet1.GetRow(i).CreateCell(0).SetCellValue(string.Empty);
                    sheet1.GetRow(i).GetCell(0).CellStyle = cellstyle;
                }
                rowindex++;
                #endregion

                foreach (var a in classList.Where(d => vm.ExamPortraitList.Select(c => c.ClassId).ToList().Contains(d.ClassId.ToString())).ToList())
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(a.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    foreach (var subject in subjectList)
                    {
                        foreach (var exam in examList)
                        {
                            var mark = vm.ExamPortraitList.Where(d => d.SubjectId == subject.SubjectId.ToString() && d.ClassId == a.ClassId.ToString() && d.ExamId == exam.ExamId.ToString()
                                                                                                ).Select(d => d).FirstOrDefault();

                            if (mark != null)
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(mark.AvgMark.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(mark.ClassRank.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                            else
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                        }
                    }
                    rowindex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("考试分多次考试班级平均分" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }
        #endregion

        #region 综合分多次考试班级平均分
        public ActionResult SegmentExamAvgExport(string chkExam, string chkSubject, string chkClass, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamPortrait.List();

                if (string.IsNullOrEmpty(chkExam) || string.IsNullOrEmpty(chkSubject) || string.IsNullOrEmpty(chkClass)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var chkExams = chkExam.Split(',');
                var chkSubjects = chkSubject.Split(',');
                var chkClasss = chkClass.Split(',');

                //获取考试
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkExams.Contains(p.Id.ToString())
                                && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //获取科目
                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   where chkSubject.Contains(p.Id.ToString())
                                    && p.IsDeleted == false
                                   orderby p.No
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.Id
                                   }).Distinct().ToList();

                //年级学生班级
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && chkClasss.Contains(p.Id.ToString())
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              SegmentMark = p.SegmentMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              ExamId = p.ExamId,
                          }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        var classAvgList = (from p in tg
                                            where p.SubjectId.ToString() == subject.SubjectId.ToString()
                                            && p.ExamId.ToString() == exam.ExamId.ToString()
                                            group p by new
                                            {
                                                p.ClassId,
                                                p.ClassName
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                g.Key.ClassName,
                                                AvgMark = g.Average(d => d.SegmentMark)
                                            }).ToList();

                        var tb = (from p in classAvgList
                                  select new Exam.Dto.ExamPortrait.List
                                  {
                                      ClassId = p.ClassId.ToString(),
                                      SubjectId = subject.SubjectId.ToString(),
                                      ClassName = p.ClassName,
                                      ExamId = exam.ExamId.ToString(),
                                      AvgMark = p.AvgMark != null ? decimal.Round((decimal)p.AvgMark, 2, MidpointRounding.AwayFromZero) : 0,
                                      ClassRank = decimal.Zero
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }

                //排名
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        var rank = decimal.Zero;
                        decimal? mark = null;
                        var count = decimal.One;
                        foreach (var t in lst.Where(d => d.SubjectId == subject.SubjectId.ToString() && d.ExamId == exam.ExamId.ToString()).OrderByDescending(d => d.AvgMark))
                        {
                            if (mark != t.AvgMark)
                            {
                                mark = t.AvgMark;
                                rank = rank + count;
                                count = decimal.One;
                            }
                            else
                            {
                                count = count + decimal.One;
                            }

                            t.ClassRank = rank;
                        }
                    }
                }
                vm.ExamPortraitList = (from p in lst
                                       where (string.IsNullOrEmpty(vm.SearchText) ? true : (p.ClassName.Contains(vm.SearchText)))
                                       select p).ToList();
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

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("综合分多次考试班级平均分") as HSSFSheet;//建立Sheet1

                var rowindex = 0;
                var cellindex = 0;
                #region 表头
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                foreach (var subject in subjectList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(subject.SubjectName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2 * examList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 2 * examList.Count();
                }
                rowindex++;

                cellindex = 1;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(exam.ExamName);
                        cell.CellStyle = cellstyle;
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                        cellindex = cellindex + 2;
                    }
                }
                rowindex++;

                cellindex = 1;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("平均分");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("多班排名");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                    }
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                for (var i = 1; i < 3; i++)
                {
                    sheet1.GetRow(i).CreateCell(0).SetCellValue(string.Empty);
                    sheet1.GetRow(i).GetCell(0).CellStyle = cellstyle;
                }
                rowindex++;
                #endregion

                foreach (var a in classList.Where(d => vm.ExamPortraitList.Select(c => c.ClassId).ToList().Contains(d.ClassId.ToString())).ToList())
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(a.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    foreach (var subject in subjectList)
                    {
                        foreach (var exam in examList)
                        {
                            var mark = vm.ExamPortraitList.Where(d => d.SubjectId == subject.SubjectId.ToString() && d.ClassId == a.ClassId.ToString() && d.ExamId == exam.ExamId.ToString()
                                                                                                ).Select(d => d).FirstOrDefault();

                            if (mark != null)
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(mark.AvgMark.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(mark.ClassRank.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                            else
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                        }
                    }
                    rowindex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("综合分多次考试班级平均分" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }
        #endregion

        #region 考试分多次考试各科前N名
        public ActionResult TotalExamTopExport(string chkExam, string chkSubject, string chkClass, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamPortrait.List();

                if (string.IsNullOrEmpty(chkExam) || string.IsNullOrEmpty(chkSubject) || string.IsNullOrEmpty(chkClass)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var chkExams = chkExam.Split(',');
                var chkSubjects = chkSubject.Split(',');
                var chkClasss = chkClass.Split(',');

                //获取考试
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkExams.Contains(p.Id.ToString())
                                && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //获取科目
                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   where chkSubject.Contains(p.Id.ToString())
                                    && p.IsDeleted == false
                                   orderby p.No
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.Id
                                   }).Distinct().ToList();

                //年级学生班级
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && chkClasss.Contains(p.Id.ToString())
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                var rank = searchText.ConvertToDecimal();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalClassRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              ExamId = p.ExamId,
                              ClassRank = p.TotalClassRank,
                          }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        var totalStudetCount = (from p in tg
                                                where p.SubjectId.ToString() == subject.SubjectId.ToString()
                                             && p.ExamId.ToString() == exam.ExamId.ToString()
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
                                  where p.SubjectId.ToString() == subject.SubjectId.ToString()
                                           && p.ExamId.ToString() == exam.ExamId.ToString()
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
                                  select new Exam.Dto.ExamPortrait.List
                                  {
                                      ClassId = p.ClassId.ToString(),
                                      SubjectId = subject.SubjectId.ToString(),
                                      ExamId = exam.ExamId.ToString(),
                                      StudentCount = p.StudentCount > decimal.Zero ? p.StudentCount.ToString() : string.Empty,
                                      Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentCount * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }
                vm.ExamPortraitList = lst;
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

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("考试分多次考试各科前N名") as HSSFSheet;//建立Sheet1

                var rowindex = 0;
                var cellindex = 0;
                #region 表头
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                foreach (var subject in subjectList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(subject.SubjectName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2 * examList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 2 * examList.Count();
                }
                rowindex++;

                cellindex = 1;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(exam.ExamName);
                        cell.CellStyle = cellstyle;
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                        cellindex = cellindex + 2;
                    }
                }
                rowindex++;

                cellindex = 1;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("人数");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("比率");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                    }
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                for (var i = 1; i < 3; i++)
                {
                    sheet1.GetRow(i).CreateCell(0).SetCellValue(string.Empty);
                    sheet1.GetRow(i).GetCell(0).CellStyle = cellstyle;
                }
                rowindex++;
                #endregion

                foreach (var a in classList.Where(d => vm.ExamPortraitList.Select(c => c.ClassId).ToList().Contains(d.ClassId.ToString())).ToList())
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(a.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    foreach (var subject in subjectList)
                    {
                        foreach (var exam in examList)
                        {
                            var mark = vm.ExamPortraitList.Where(d => d.SubjectId == subject.SubjectId.ToString() && d.ClassId == a.ClassId.ToString() && d.ExamId == exam.ExamId.ToString()
                                                                                                ).Select(d => d).FirstOrDefault();

                            if (mark != null)
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(mark.StudentCount);
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(mark.Rate);
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                            else
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                        }
                    }
                    rowindex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("考试分多次考试各科前N名" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }
        #endregion

        #region 综合分多次考试各科前N名
        public ActionResult SegmentExamTopExport(string chkExam, string chkSubject, string chkClass, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamPortrait.List();

                if (string.IsNullOrEmpty(chkExam) || string.IsNullOrEmpty(chkSubject) || string.IsNullOrEmpty(chkClass)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var chkExams = chkExam.Split(',');
                var chkSubjects = chkSubject.Split(',');
                var chkClasss = chkClass.Split(',');

                //获取考试
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkExams.Contains(p.Id.ToString())
                                && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //获取科目
                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   where chkSubject.Contains(p.Id.ToString())
                                    && p.IsDeleted == false
                                   orderby p.No
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.Id
                                   }).Distinct().ToList();

                //年级学生班级
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && chkClasss.Contains(p.Id.ToString())
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                var rank = searchText.ConvertToDecimal();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.SegmentClassRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              ExamId = p.tbExamCourse.tbExam.Id,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              ExamId = p.ExamId,
                              ClassRank = p.SegmentClassRank,
                          }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        var totalStudetCount = (from p in tg
                                                where p.SubjectId.ToString() == subject.SubjectId.ToString()
                                             && p.ExamId.ToString() == exam.ExamId.ToString()
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
                                  where p.SubjectId.ToString() == subject.SubjectId.ToString()
                                           && p.ExamId.ToString() == exam.ExamId.ToString()
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
                                  select new Exam.Dto.ExamPortrait.List
                                  {
                                      ClassId = p.ClassId.ToString(),
                                      SubjectId = subject.SubjectId.ToString(),
                                      ExamId = exam.ExamId.ToString(),
                                      StudentCount = p.StudentCount > decimal.Zero ? p.StudentCount.ToString() : string.Empty,
                                      Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentCount * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }
                vm.ExamPortraitList = lst;
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

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("综合分多次考试各科前N名") as HSSFSheet;//建立Sheet1

                var rowindex = 0;
                var cellindex = 0;
                #region 表头
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 2, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                foreach (var subject in subjectList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(subject.SubjectName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2 * examList.Count() - 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 2 * examList.Count();
                }
                rowindex++;

                cellindex = 1;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(exam.ExamName);
                        cell.CellStyle = cellstyle;
                        cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 1);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                        cellindex = cellindex + 2;
                    }
                }
                rowindex++;

                cellindex = 1;
                row = sheet1.CreateRow(rowindex);
                foreach (var subject in subjectList)
                {
                    foreach (var exam in examList)
                    {
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("人数");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue("比率");
                        cell.CellStyle = cellstyle;
                        cellindex++;
                    }
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                for (var i = 1; i < 3; i++)
                {
                    sheet1.GetRow(i).CreateCell(0).SetCellValue(string.Empty);
                    sheet1.GetRow(i).GetCell(0).CellStyle = cellstyle;
                }
                rowindex++;
                #endregion

                foreach (var a in classList.Where(d => vm.ExamPortraitList.Select(c => c.ClassId).ToList().Contains(d.ClassId.ToString())).ToList())
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(a.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    foreach (var subject in subjectList)
                    {
                        foreach (var exam in examList)
                        {
                            var mark = vm.ExamPortraitList.Where(d => d.SubjectId == subject.SubjectId.ToString() && d.ClassId == a.ClassId.ToString() && d.ExamId == exam.ExamId.ToString()
                                                                                                ).Select(d => d).FirstOrDefault();

                            if (mark != null)
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(mark.StudentCount);
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(mark.Rate);
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                            else
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                        }
                    }
                    rowindex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("综合分多次考试各科前N名" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }
        #endregion

        #region 考试分总分分数 
        public ActionResult TotalExamlHistorypExport(string chkExam, string chkSubject, string chkClass, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamPortrait.List();

                if (string.IsNullOrEmpty(chkExam) || string.IsNullOrEmpty(chkSubject) || string.IsNullOrEmpty(chkClass)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var chkExams = chkExam.Split(',');
                var chkSubjects = chkSubject.Split(',');
                var chkClasss = chkClass.Split(',');

                //获取考试
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkExams.Contains(p.Id.ToString())
                                && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //获取科目
                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   where chkSubject.Contains(p.Id.ToString())
                                    && p.IsDeleted == false
                                   orderby p.No
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.Id
                                   }).Distinct().ToList();

                //年级学生班级
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && chkClasss.Contains(p.Id.ToString())
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.TotalMark,
                              p.TotalClassRank,
                              p.TotalGradeRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var examScoreList = (from p in tf
                                     join t in classStudent
                                     on p.StudentId equals t.StudentId
                                     select new Dto.ExamPortrait.List
                                     {
                                         ExamId = p.ExamId.ToString(),
                                         StudentId = p.StudentId.ToString(),
                                         ClassName = t.ClassName,
                                         ClassId = t.ClassId.ToString(),
                                         StudentCode = t.StudentCode,
                                         StudentName = t.StudentName,
                                         SubjectId = p.SubjectId.ToString(),
                                         TotalMark = p.TotalMark.ToString(),
                                         TotalClassRank = p.TotalClassRank.ToString(),
                                         TotalGradeRank = p.TotalGradeRank.ToString(),
                                     }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                lst = (from p in examScoreList
                       group p by new { p.StudentId, p.StudentCode, p.StudentName, p.ClassId, p.ClassName, p.ExamId } into g
                       select new Dto.ExamPortrait.List
                       {
                           ExamId = g.Key.ExamId.ToString(),
                           StudentId = g.Key.StudentId.ToString(),
                           ClassName = g.Key.ClassName,
                           StudentCode = g.Key.StudentCode,
                           StudentName = g.Key.StudentName,
                           ClassId = g.Key.ClassId,
                           TotalHistory = g.Sum(d => d.TotalMark.ConvertToDecimal()),
                       }).ToList();

                //排名
                foreach (var exam in examList)
                {
                    //年级排名
                    var gradeRank = decimal.Zero;
                    decimal? gradeMark = null;
                    var gradeCount = decimal.One;
                    foreach (var t in lst.Where(d => d.ExamId == exam.ExamId.ToString()).OrderByDescending(d => d.TotalHistory))
                    {
                        if (gradeMark != t.TotalHistory)
                        {
                            gradeMark = t.TotalHistory;
                            gradeRank = gradeRank + gradeCount;
                            gradeCount = decimal.One;
                        }
                        else
                        {
                            gradeCount = gradeCount + decimal.One;
                        }

                        t.GradeRank = gradeRank;
                    }
                    //班级排名
                    foreach (var classs in classList)
                    {
                        var classRank = decimal.Zero;
                        decimal? classMark = null;
                        var classCount = decimal.One;
                        foreach (var t in lst.Where(d => d.ClassId == classs.ClassId.ToString() && d.ExamId == exam.ExamId.ToString()).OrderByDescending(d => d.TotalHistory))
                        {
                            if (classMark != t.TotalHistory)
                            {
                                classMark = t.TotalHistory;
                                classRank = classRank + classCount;
                                classCount = decimal.One;
                            }
                            else
                            {
                                classCount = classCount + decimal.One;
                            }

                            t.ClassRank = classRank;
                        }
                    }
                }
                vm.ExamPortraitList = (from p in lst
                                       where (string.IsNullOrEmpty(vm.SearchText) ? true : (p.StudentCode.Contains(vm.SearchText) || p.StudentName.Contains(vm.SearchText)))
                                       select p).ToList();
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

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("考试分总分分数") as HSSFSheet;//建立Sheet1

                var rowindex = 0;
                var cellindex = 0;
                #region 表头
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("学号");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("姓名");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                foreach (var exam in examList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(exam.ExamName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 3;
                }
                rowindex++;

                cellindex = 3;
                row = sheet1.CreateRow(rowindex);
                foreach (var exam in examList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("总分");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("班级排名");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("年级排名");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                sheet1.GetRow(1).CreateCell(0).SetCellValue(string.Empty);
                sheet1.GetRow(1).GetCell(0).CellStyle = cellstyle;
                sheet1.GetRow(1).CreateCell(1).SetCellValue(string.Empty);
                sheet1.GetRow(1).GetCell(1).CellStyle = cellstyle;
                sheet1.GetRow(1).CreateCell(2).SetCellValue(string.Empty);
                sheet1.GetRow(1).GetCell(2).CellStyle = cellstyle;
                rowindex++;
                #endregion

                foreach (var student in vm.ExamPortraitList.Select(d => new { d.StudentId, d.ClassName, d.StudentCode, d.StudentName }).Distinct().ToList())
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.StudentCode);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.StudentName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    foreach (var exam in examList)
                    {
                        var examScore = vm.ExamPortraitList.Where(d => d.StudentCode == student.StudentCode && d.ExamId == exam.ExamId.ToString()).FirstOrDefault();
                        if (examScore != null)
                        {
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(examScore.TotalHistory.ToString());
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(examScore.ClassRank.ToString());
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(examScore.GradeRank.ToString());
                            cell.CellStyle = cellstyle;
                            cellindex++;
                        }
                        else
                        {
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue("");
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue("");
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue("");
                            cell.CellStyle = cellstyle;
                            cellindex++;
                        }
                    }
                    rowindex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("考试分总分分数" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }
        #endregion

        #region 综合分总分分数 
        public ActionResult SegmentExamlHistorypExport(string chkExam, string chkSubject, string chkClass, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamPortrait.List();

                if (string.IsNullOrEmpty(chkExam) || string.IsNullOrEmpty(chkSubject) || string.IsNullOrEmpty(chkClass)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var chkExams = chkExam.Split(',');
                var chkSubjects = chkSubject.Split(',');
                var chkClasss = chkClass.Split(',');

                //获取考试
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkExams.Contains(p.Id.ToString())
                                && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //获取科目
                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   where chkSubject.Contains(p.Id.ToString())
                                    && p.IsDeleted == false
                                   orderby p.No
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.Id
                                   }).Distinct().ToList();

                //年级学生班级
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && chkClasss.Contains(p.Id.ToString())
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.SegmentMark,
                              p.SegmentClassRank,
                              p.SegmentGradeRank,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var examScoreList = (from p in tf
                                     join t in classStudent
                                     on p.StudentId equals t.StudentId
                                     select new Dto.ExamPortrait.List
                                     {
                                         ExamId = p.ExamId.ToString(),
                                         StudentId = p.StudentId.ToString(),
                                         ClassName = t.ClassName,
                                         ClassId = t.ClassId.ToString(),
                                         StudentCode = t.StudentCode,
                                         StudentName = t.StudentName,
                                         SubjectId = p.SubjectId.ToString(),
                                         SegmentMark = p.SegmentMark.ToString(),
                                         SegmentClassRank = p.SegmentClassRank.ToString(),
                                         SegmentGradeRank = p.SegmentGradeRank.ToString(),
                                     }).ToList();

                var lst = new List<Exam.Dto.ExamPortrait.List>();
                lst = (from p in examScoreList
                       group p by new { p.StudentId, p.StudentCode, p.StudentName, p.ClassId, p.ClassName, p.ExamId } into g
                       select new Dto.ExamPortrait.List
                       {
                           ExamId = g.Key.ExamId.ToString(),
                           StudentId = g.Key.StudentId.ToString(),
                           ClassName = g.Key.ClassName,
                           StudentCode = g.Key.StudentCode,
                           StudentName = g.Key.StudentName,
                           ClassId = g.Key.ClassId,
                           TotalHistory = g.Sum(d => d.SegmentMark != null ? d.SegmentMark.ConvertToDecimal() : 0),
                       }).ToList();

                //排名
                foreach (var exam in examList)
                {
                    //年级排名
                    var gradeRank = decimal.Zero;
                    decimal? gradeMark = null;
                    var gradeCount = decimal.One;
                    foreach (var t in lst.Where(d => d.ExamId == exam.ExamId.ToString()).OrderByDescending(d => d.TotalHistory))
                    {
                        if (gradeMark != t.TotalHistory)
                        {
                            gradeMark = t.TotalHistory;
                            gradeRank = gradeRank + gradeCount;
                            gradeCount = decimal.One;
                        }
                        else
                        {
                            gradeCount = gradeCount + decimal.One;
                        }

                        t.GradeRank = gradeRank;
                    }
                    //班级排名
                    foreach (var classs in classList)
                    {
                        var classRank = decimal.Zero;
                        decimal? classMark = null;
                        var classCount = decimal.One;
                        foreach (var t in lst.Where(d => d.ClassId == classs.ClassId.ToString() && d.ExamId == exam.ExamId.ToString()).OrderByDescending(d => d.TotalHistory))
                        {
                            if (classMark != t.TotalHistory)
                            {
                                classMark = t.TotalHistory;
                                classRank = classRank + classCount;
                                classCount = decimal.One;
                            }
                            else
                            {
                                classCount = classCount + decimal.One;
                            }

                            t.ClassRank = classRank;
                        }
                    }
                }
                vm.ExamPortraitList = (from p in lst
                                       where (string.IsNullOrEmpty(vm.SearchText) ? true : (p.StudentCode.Contains(vm.SearchText) || p.StudentName.Contains(vm.SearchText)))
                                       select p).ToList();
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

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("综合分总分分数") as HSSFSheet;//建立Sheet1

                var rowindex = 0;
                var cellindex = 0;
                #region 表头
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("学号");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("姓名");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                foreach (var exam in examList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(exam.ExamName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 2);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 3;
                }
                rowindex++;

                cellindex = 3;
                row = sheet1.CreateRow(rowindex);
                foreach (var exam in examList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("总分");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("班级排名");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("年级排名");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                sheet1.GetRow(1).CreateCell(0).SetCellValue(string.Empty);
                sheet1.GetRow(1).GetCell(0).CellStyle = cellstyle;
                sheet1.GetRow(1).CreateCell(1).SetCellValue(string.Empty);
                sheet1.GetRow(1).GetCell(1).CellStyle = cellstyle;
                sheet1.GetRow(1).CreateCell(2).SetCellValue(string.Empty);
                sheet1.GetRow(1).GetCell(2).CellStyle = cellstyle;
                rowindex++;
                #endregion

                foreach (var student in vm.ExamPortraitList.Select(d => new { d.StudentId, d.ClassName, d.StudentCode, d.StudentName }).Distinct().ToList())
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.ClassName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.StudentCode);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(student.StudentName);
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    foreach (var exam in examList)
                    {
                        var examScore = vm.ExamPortraitList.Where(d => d.StudentCode == student.StudentCode && d.ExamId == exam.ExamId.ToString()).FirstOrDefault();
                        if (examScore != null)
                        {
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(examScore.TotalHistory.ToString());
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(examScore.ClassRank.ToString());
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue(examScore.GradeRank.ToString());
                            cell.CellStyle = cellstyle;
                            cellindex++;
                        }
                        else
                        {
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue("");
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue("");
                            cell.CellStyle = cellstyle;
                            cellindex++;
                            cell = row.CreateCell(cellindex);
                            cell.SetCellValue("");
                            cell.CellStyle = cellstyle;
                            cellindex++;
                        }
                    }
                    rowindex++;
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("综合分总分分数" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }
        #endregion

        #region 考试分综合成绩分析 
        public ActionResult TotalExamlCompExport(string chkExam, string chkSubject, string chkClass, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamPortrait.List();

                if (string.IsNullOrEmpty(chkExam) || string.IsNullOrEmpty(chkSubject) || string.IsNullOrEmpty(chkClass)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var chkExams = chkExam.Split(',');
                var chkSubjects = chkSubject.Split(',');
                var chkClasss = chkClass.Split(',');

                //获取考试
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkExams.Contains(p.Id.ToString())
                                && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //获取科目
                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   where chkSubject.Contains(p.Id.ToString())
                                    && p.IsDeleted == false
                                   orderby p.No
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.Id
                                   }).Distinct().ToList();

                //年级学生班级
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && chkClasss.Contains(p.Id.ToString())
                                 && (string.IsNullOrEmpty(vm.SearchText)? true :p.ClassName.Contains(vm.SearchText))
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              FullTotalMark = p.tbExamCourse.FullTotalMark,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    //&& (string.IsNullOrEmpty(vm.SearchText) ? true : (p.tbClass.ClassName.Contains(vm.SearchText) || p.tbClass.ClassName.Contains(vm.SearchText)))
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var examScoreList = (from p in tf
                                     join t in classStudent
                                     on p.StudentId equals t.StudentId
                                     select new Dto.ExamPortrait.List
                                     {
                                         ExamId = p.ExamId.ToString(),
                                         StudentId = p.StudentId.ToString(),
                                         ClassName = t.ClassName,
                                         ClassId = t.ClassId.ToString(),
                                         StudentCode = t.StudentCode,
                                         StudentName = t.StudentName,
                                         SubjectId = p.SubjectId.ToString(),
                                         TotalMark = p.TotalMark.ToString(),
                                         FullTotalMark = p.FullTotalMark,
                                     }).ToList();

                //获取分数段
                var examSegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                           where p.IsDeleted == false
                                           && p.tbSubject.IsDeleted == false
                                           && p.tbGrade.IsDeleted == false
                                           && p.tbGrade.Id == vm.GradeId
                                           && chkSubjects.Contains(p.tbSubject.Id.ToString())
                                           select new
                                           {
                                               SubjectId = p.tbSubject.Id.ToString(),
                                               MinMark = p.MinMark,
                                               MaxMark = p.MaxMark,
                                               IsGood = p.IsGood,
                                               IsPass = p.IsPass,
                                           }).ToList();


                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var cla in classList)
                {
                    foreach (var subject in subjectList)
                    {
                        foreach (var exam in examList)
                        {
                            var list = new Exam.Dto.ExamPortrait.List();
                            //班级科目考试人数
                            decimal escCount = examScoreList.Where(d => d.ExamId == exam.ExamId.ToString() && d.SubjectId == subject.SubjectId.ToString() && d.ClassId == cla.ClassId.ToString()).Count();

                            //优秀及格人数
                            var gpList = (from p in examSegmentMarkList
                                          join t in examScoreList
                                          on p.SubjectId equals t.SubjectId
                                          where p.SubjectId == subject.SubjectId.ToString()
                                          && t.ClassId == cla.ClassId.ToString()
                                          && t.SubjectId == subject.SubjectId.ToString()
                                          && t.ExamId == exam.ExamId.ToString()
                                          && (p.MinMark * t.FullTotalMark / 100 <= t.TotalMark.ConvertToDecimal()
                                          && t.TotalMark.ConvertToDecimal() <= p.MaxMark * t.FullTotalMark / 100)
                                          select p).ToList();

                            //优秀人数比率
                            list.ExcellentCount = gpList.Where(d => d.IsGood == true).Count();
                            list.ExcellentRate = escCount == 0 ? "0" : decimal.Round(gpList.Where(d => d.IsGood == true).Count() / escCount * 100, 2, MidpointRounding.AwayFromZero) + "%";

                            //及格人数比率
                            list.PassingCount = gpList.Where(d => d.IsPass == true).Count();
                            list.PassingRate = escCount == 0 ? "0" : decimal.Round(gpList.Where(d => d.IsPass == true).Count() / escCount * 100, 2, MidpointRounding.AwayFromZero) + "%";

                            list.ClassId = cla.ClassId.ToString();
                            list.SubjectId = subject.SubjectId.ToString();
                            list.ExamId = exam.ExamId.ToString();
                            lst.Add(list);
                        }
                    }
                }
                vm.ExamPortraitList = lst;
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

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("考试分综合成绩分析") as HSSFSheet;//建立Sheet1

                var rowindex = 0;
                var cellindex = 0;
                #region 表头
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("科目");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                foreach (var exam in examList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(exam.ExamName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 3);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 4;
                }
                rowindex++;

                cellindex = 2;
                row = sheet1.CreateRow(rowindex);
                foreach (var exam in examList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("优秀人数");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("优秀比率");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("及格人数");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("及格比率");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                sheet1.GetRow(1).CreateCell(0).SetCellValue(string.Empty);
                sheet1.GetRow(1).GetCell(0).CellStyle = cellstyle;
                sheet1.GetRow(1).CreateCell(1).SetCellValue(string.Empty);
                sheet1.GetRow(1).GetCell(1).CellStyle = cellstyle;
                rowindex++;
                #endregion

                foreach (var cla in classList.Where(d => string.IsNullOrEmpty(searchText) ? true : d.ClassName.Contains(searchText)))
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(cla.ClassName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex + subjectList.Count() - 1, cellindex, cellindex);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    var i = 0;
                    foreach (var subject in subjectList)
                    {
                        if (i > 0)
                        {
                            row = sheet1.CreateRow(rowindex);
                        }
                        cellindex = 1;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(subject.SubjectName);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        foreach (var exam in examList)
                        {
                            var result = vm.ExamPortraitList.Where(d => d.ClassId == cla.ClassId.ToString() && d.SubjectId == subject.SubjectId.ToString() && d.ExamId == exam.ExamId.ToString()).FirstOrDefault();
                            if (result != null)
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(result.ExcellentCount.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(result.ExcellentRate.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(result.PassingCount.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(result.PassingRate.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                            else
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                        }
                        rowindex++;
                        i++;
                    }
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                rowindex = 3;
                foreach (var cla in classList.Where(d => string.IsNullOrEmpty(searchText) ? true : d.ClassName.Contains(searchText)))
                {
                    for (var j = rowindex; j <= (rowindex + subjectList.Count() - 2); j++)
                    {
                        sheet1.GetRow(j).CreateCell(0).SetCellValue(string.Empty);
                        sheet1.GetRow(j).GetCell(0).CellStyle = cellstyle;
                    }
                    rowindex = rowindex + subjectList.Count();
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("考试分综合成绩分析" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }
        #endregion

        #region 综合分综合成绩分析 
        public ActionResult SegmentExamlCompExport(string chkExam, string chkSubject, string chkClass, int gradeId, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 统计分析
                var vm = new Models.ExamPortrait.List();

                if (string.IsNullOrEmpty(chkExam) || string.IsNullOrEmpty(chkSubject) || string.IsNullOrEmpty(chkClass)) return Content("<script>alert('暂无数据!');history.go(-1);</script>");

                var chkExams = chkExam.Split(',');
                var chkSubjects = chkSubject.Split(',');
                var chkClasss = chkClass.Split(',');

                //获取考试
                var examList = (from p in db.Table<Exam.Entity.tbExam>()
                                where chkExams.Contains(p.Id.ToString())
                                && p.IsDeleted == false
                                select new
                                {
                                    ExamName = p.ExamName,
                                    ExamId = p.Id,
                                }).Distinct().ToList();

                //获取科目
                var subjectList = (from p in db.Table<Course.Entity.tbSubject>()
                                   where chkSubject.Contains(p.Id.ToString())
                                    && p.IsDeleted == false
                                   orderby p.No
                                   select new
                                   {
                                       SubjectName = p.SubjectName,
                                       SubjectId = p.Id
                                   }).Distinct().ToList();

                //年级学生班级
                var classList = (from p in db.Table<Basis.Entity.tbClass>()
                                 where p.IsDeleted == false
                                 && p.tbGrade.Id == vm.GradeId
                                 && chkClasss.Contains(p.Id.ToString())
                                 && (string.IsNullOrEmpty(vm.SearchText) ? true : p.ClassName.Contains(vm.SearchText))
                                 select new
                                 {
                                     ClassName = p.ClassName,
                                     ClassId = p.Id,
                                 }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && p.tbExamCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.IsDeleted == false
                           && p.tbExamCourse.tbExam.IsDeleted == false
                           && chkExams.Contains(p.tbExamCourse.tbExam.Id.ToString())
                           && chkSubjects.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              ExamId = p.tbExamCourse.tbExam.Id,
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              FullSegmentMark = p.tbExamCourse.FullSegmentMark,
                          }).ToList();

                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && chkClasss.Contains(p.tbClass.Id.ToString())
                                    //&& (string.IsNullOrEmpty(vm.SearchText) ? true : (p.tbClass.ClassName.Contains(vm.SearchText) || p.tbClass.ClassName.Contains(vm.SearchText)))
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        ClassName = p.tbClass.ClassName,
                                        StudentCode = p.tbStudent.StudentCode,
                                        StudentName = p.tbStudent.StudentName,
                                        ClassId = p.tbClass.Id,
                                    }).Distinct().ToList();

                var examScoreList = (from p in tf
                                     join t in classStudent
                                     on p.StudentId equals t.StudentId
                                     select new Dto.ExamPortrait.List
                                     {
                                         ExamId = p.ExamId.ToString(),
                                         StudentId = p.StudentId.ToString(),
                                         ClassName = t.ClassName,
                                         ClassId = t.ClassId.ToString(),
                                         StudentCode = t.StudentCode,
                                         StudentName = t.StudentName,
                                         SubjectId = p.SubjectId.ToString(),
                                         SegmentMark = p.SegmentMark.ToString(),
                                         FullSegmentMark = p.FullSegmentMark,
                                     }).ToList();

                //获取分数段
                var examSegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                           where p.IsDeleted == false
                                           && p.tbSubject.IsDeleted == false
                                           && p.tbGrade.IsDeleted == false
                                           && p.tbGrade.Id == vm.GradeId
                                           && chkSubjects.Contains(p.tbSubject.Id.ToString())
                                           select new
                                           {
                                               SubjectId = p.tbSubject.Id.ToString(),
                                               MinMark = p.MinMark,
                                               MaxMark = p.MaxMark,
                                               IsGood = p.IsGood,
                                               IsPass = p.IsPass,
                                           }).ToList();


                var lst = new List<Exam.Dto.ExamPortrait.List>();
                foreach (var cla in classList)
                {
                    foreach (var subject in subjectList)
                    {
                        foreach (var exam in examList)
                        {
                            var list = new Exam.Dto.ExamPortrait.List();
                            //班级科目考试人数
                            decimal escCount = examScoreList.Where(d => d.ExamId == exam.ExamId.ToString() && d.SubjectId == subject.SubjectId.ToString() && d.ClassId == cla.ClassId.ToString()).Count();

                            //优秀及格人数
                            var gpList = (from p in examSegmentMarkList
                                          join t in examScoreList
                                          on p.SubjectId equals t.SubjectId
                                          where p.SubjectId == subject.SubjectId.ToString()
                                          && t.ClassId == cla.ClassId.ToString()
                                          && t.SubjectId == subject.SubjectId.ToString()
                                          && t.ExamId == exam.ExamId.ToString()
                                          && (p.MinMark * t.FullSegmentMark / 100 <= t.SegmentMark.ConvertToDecimal()
                                          && t.SegmentMark.ConvertToDecimal() <= p.MaxMark * t.FullSegmentMark / 100)
                                          select p).ToList();

                            //优秀人数比率
                            list.ExcellentCount = gpList.Where(d => d.IsGood == true).Count();
                            list.ExcellentRate = escCount == 0 ? "0" : decimal.Round(gpList.Where(d => d.IsGood == true).Count() / escCount * 100, 2, MidpointRounding.AwayFromZero) + "%";

                            //及格人数比率
                            list.PassingCount = gpList.Where(d => d.IsPass == true).Count();
                            list.PassingRate = escCount == 0 ? "0" : decimal.Round(gpList.Where(d => d.IsPass == true).Count() / escCount * 100, 2, MidpointRounding.AwayFromZero) + "%";

                            list.ClassId = cla.ClassId.ToString();
                            list.SubjectId = subject.SubjectId.ToString();
                            list.ExamId = exam.ExamId.ToString();
                            lst.Add(list);
                        }
                    }
                }
                vm.ExamPortraitList = lst;
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

                HSSFSheet sheet1 = hssfworkbook.CreateSheet("综合分综合成绩分析") as HSSFSheet;//建立Sheet1

                var rowindex = 0;
                var cellindex = 0;
                #region 表头
                IRow row = sheet1.CreateRow(rowindex);
                ICell cell = row.CreateCell(cellindex);
                cell.SetCellValue("班级");
                cell.CellStyle = cellstyle;
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                cell = row.CreateCell(cellindex);
                cell.SetCellValue("科目");
                cell.CellStyle = cellstyle;
                cellRangeAddress = new CellRangeAddress(rowindex, rowindex + 1, cellindex, cellindex);
                setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                sheet1.AddMergedRegion(cellRangeAddress);
                cellindex++;
                foreach (var exam in examList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(exam.ExamName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex, cellindex, cellindex + 3);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    cellindex = cellindex + 4;
                }
                rowindex++;

                cellindex = 2;
                row = sheet1.CreateRow(rowindex);
                foreach (var exam in examList)
                {
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("优秀人数");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("优秀比率");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("及格人数");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue("及格比率");
                    cell.CellStyle = cellstyle;
                    cellindex++;
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                sheet1.GetRow(1).CreateCell(0).SetCellValue(string.Empty);
                sheet1.GetRow(1).GetCell(0).CellStyle = cellstyle;
                sheet1.GetRow(1).CreateCell(1).SetCellValue(string.Empty);
                sheet1.GetRow(1).GetCell(1).CellStyle = cellstyle;
                rowindex++;
                #endregion

                foreach (var cla in classList.Where(d => string.IsNullOrEmpty(searchText) ? true : d.ClassName.Contains(searchText)))
                {
                    cellindex = 0;
                    row = sheet1.CreateRow(rowindex);
                    cell = row.CreateCell(cellindex);
                    cell.SetCellValue(cla.ClassName);
                    cell.CellStyle = cellstyle;
                    cellRangeAddress = new CellRangeAddress(rowindex, rowindex + subjectList.Count() - 1, cellindex, cellindex);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);

                    var i = 0;
                    foreach (var subject in subjectList)
                    {
                        if (i > 0)
                        {
                            row = sheet1.CreateRow(rowindex);
                        }
                        cellindex = 1;
                        cell = row.CreateCell(cellindex);
                        cell.SetCellValue(subject.SubjectName);
                        cell.CellStyle = cellstyle;
                        cellindex++;
                        foreach (var exam in examList)
                        {
                            var result = vm.ExamPortraitList.Where(d => d.ClassId == cla.ClassId.ToString() && d.SubjectId == subject.SubjectId.ToString() && d.ExamId == exam.ExamId.ToString()).FirstOrDefault();
                            if (result != null)
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(result.ExcellentCount.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(result.ExcellentRate.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(result.PassingCount.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue(result.PassingRate.ToString());
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                            else
                            {
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                                cell = row.CreateCell(cellindex);
                                cell.SetCellValue("");
                                cell.CellStyle = cellstyle;
                                cellindex++;
                            }
                        }
                        rowindex++;
                        i++;
                    }
                }
                setBorder(cellRangeAddress, sheet1, hssfworkbook);
                rowindex = 3;
                foreach (var cla in classList.Where(d => string.IsNullOrEmpty(searchText) ? true : d.ClassName.Contains(searchText)))
                {
                    for (var j = rowindex; j <= (rowindex + subjectList.Count() - 2); j++)
                    {
                        sheet1.GetRow(j).CreateCell(0).SetCellValue(string.Empty);
                        sheet1.GetRow(j).GetCell(0).CellStyle = cellstyle;
                    }
                    rowindex = rowindex + subjectList.Count();
                }

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("综合分综合成绩分析" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }
                #endregion
            }
        }
        #endregion
        #endregion

        public ActionResult SelectSubjectByExamId(string examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var arrystr = new string[] { };
                var chkExams = examId != null ? examId.Split(',') : arrystr;
                var listItems = new List<System.Web.Mvc.SelectListItem>();
                foreach (var chkExam in chkExams)
                {
                    var id = (chkExam == string.Empty || chkExam == null) ? 0 : int.Parse(chkExam);
                    //考试科目
                    var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                       where p.tbExam.Id == id
                                        && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                       orderby p.tbCourse.tbSubject.No
                                       select new
                                       {
                                           SubjectName = p.tbCourse.tbSubject.SubjectName,
                                           SubjectId = p.tbCourse.tbSubject.Id
                                       }).Distinct().ToList();
                    foreach (var Subject in SubjectList)
                    {
                        if (listItems.Where(c => c.Value == Subject.SubjectId.ToString()).FirstOrDefault() == null)
                        {
                            var listItem = new System.Web.Mvc.SelectListItem();
                            listItem.Text = Subject.SubjectName;
                            listItem.Value = Subject.SubjectId.ToString();
                            listItems.Add(listItem);
                        }
                    }
                }

                return Json(listItems, JsonRequestBehavior.AllowGet);
            }
        }

        #region 班级前N名
        public ActionResult TotalMarkTopNList()
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                return View();
            }
        }
        #endregion

        #region 对象转JSON字符串
        public static string ToJSONString(object obj)
        {
            if (obj == null) return "[]";

            var js = new System.Web.Script.Serialization.JavaScriptSerializer();
            return js.Serialize(obj);
        }
        #endregion

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
    }
}