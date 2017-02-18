using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace XkSystem.Areas.Exam.Controllers
{
    public class ExamAnalyzeController : Controller
    {
        #region 分数段统计
        public ActionResult List()
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
                //科目和班级
                var classList =new  List<System.Web.Mvc.SelectListItem>();
                var subjectList = new List<System.Web.Mvc.SelectListItem>();
                var yearId = 0;
                this.getSubjectClassList(vm.ExamId, vm.GradeId, vm.SearchText,out classList, out subjectList,out yearId);
                vm.SubjectList = subjectList;
                vm.ClassList = classList;

                //选中班级和科目
                if (vm.chkClass == null || vm.chkSubject == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');
                var chksubjectList = vm.chkSubject.Split(',');

                var selectclassList = new List<System.Web.Mvc.SelectListItem>();
                var selectsubjectList = new List<System.Web.Mvc.SelectListItem>();
                this.getSelectSubjectClassList(chkclassList,chksubjectList, out selectclassList, out selectsubjectList);
                vm.selctClassList = selectclassList;
                vm.selectSubjectList = selectsubjectList;

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id == vm.GradeId
                                        && p.tbSubject.IsDeleted == false
                                        && chksubjectList.Contains(p.tbSubject.Id.ToString())
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           SubjectId = p.tbSubject.Id
                                       }).ToList();

                vm.SegmentList = (from p in SegmentMarkList
                                  select new Exam.Dto.ExamAnalyze.SegmentList
                                  {
                                      SegmentName = p.SegmentName,
                                      SegmentId = p.SegmentId,
                                      SubjectId = p.SubjectId
                                  }).Distinct().ToList();

                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && chkclassList.Contains(p.tbClass.Id.ToString())
                                    orderby p.tbClass.No
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
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullTotalMark
                          }).ToList();

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
                              FullTotalMark = p.FullTotalMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    //班级人数
                    var totalStudetCount = (from p in tg
                                            where p.SubjectId == o.SubjectId
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                TotalCount = g.Count()
                                            }).ToList();
                    //分数段人数
                    var tm = (from p in tg
                              where p.SubjectId == o.SubjectId
                              && p.TotalMark >= o.MinMark  && p.TotalMark <= o.MaxMark
                              group p by new
                              {
                                  p.ClassId
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  SegmentId = o.SegmentId,
                                  StudentCount = g.Count(),
                                  TotalCount = totalStudetCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = o.SubjectId,
                                  SegmentId = p.SegmentId,
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
        public ActionResult List(Models.ExamAnalyze.List vm)
        {
            var arrystr =string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"]: arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("List", 
                new { ExamId = vm.ExamId, GradeId = vm.GradeId,
                    chkSubject = chksubjectList, chkClass= chkclassList,
                    checkedAll = CheckedAll,
                    chkClassAll= chkClassAll, searchText = vm.SearchText }));
        }

        public ActionResult SegmentList()
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
                //科目和班级
                var classList = new List<System.Web.Mvc.SelectListItem>();
                var subjectList = new List<System.Web.Mvc.SelectListItem>();
                var yearId = 0;
                this.getSubjectClassList(vm.ExamId, vm.GradeId, vm.SearchText, out classList, out subjectList, out yearId);
                vm.SubjectList = subjectList;
                vm.ClassList = classList;

                if (vm.chkClass == null || vm.chkSubject == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');
                var chksubjectList = vm.chkSubject.Split(',');

                var selectclassList = new List<System.Web.Mvc.SelectListItem>();
                var selectsubjectList = new List<System.Web.Mvc.SelectListItem>();
                this.getSelectSubjectClassList(chkclassList, chksubjectList, out selectclassList, out selectsubjectList);
                vm.selctClassList = selectclassList;
                vm.selectSubjectList = selectsubjectList;

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id == vm.GradeId
                                        && p.tbSubject.IsDeleted == false
                                        && chksubjectList.Contains(p.tbSubject.Id.ToString())
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           SubjectId = p.tbSubject.Id
                                       }).ToList();

                vm.SegmentList = (from p in SegmentMarkList
                                  select new Exam.Dto.ExamAnalyze.SegmentList
                                  {
                                      SegmentName = p.SegmentName,
                                      SegmentId = p.SegmentId,
                                      SubjectId = p.SubjectId
                                  }).Distinct().ToList();

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
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullSegmentMark
                          }).ToList();

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
                              p.FullSegmentMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    //班级人数
                    var totalStudetCount = (from p in tg
                                            where p.SubjectId == o.SubjectId
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                TotalCount = g.Count()
                                            }).ToList();
                    //分数段人数
                    var tm = (from p in tg
                              where p.SubjectId == o.SubjectId
                              && p.SegmentMark >= o.MinMark && p.SegmentMark <= o.MaxMark
                              group p by new
                              {
                                  p.ClassId
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  SegmentId = o.SegmentId,
                                  StudentCount = g.Count(),
                                  TotalCount = totalStudetCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = o.SubjectId,
                                  SegmentId = p.SegmentId,
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
        public ActionResult SegmentList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("SegmentList", new { ExamId = vm.ExamId, GradeId = vm.GradeId,
                chkSubject = chksubjectList, chkClass = chkclassList,
                checkedAll = CheckedAll,
                chkClassAll = chkClassAll,
                searchText = vm.SearchText }));
        }

        public ActionResult TotalMarkDetailList(int examId, int gradeId, int classId, string chkSubject)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();
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
                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id == gradeId
                                        && p.tbSubject.IsDeleted == false
                                        && chksubjectList.Contains(p.tbSubject.Id.ToString())
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           SubjectId = p.tbSubject.Id,
                                           p.tbSubject.SubjectName
                                       }).ToList();

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
                              TotalMark = p.TotalMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    //分数段人数
                    var tm = (from p in tg
                              where p.SubjectId == o.SubjectId
                              && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                              select new
                              {
                                  SubjectName = o.SubjectName,
                                  SegmentId = o.SegmentId,
                                  SegmentName = o.SegmentName,
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
                                      SegmentName = p.SegmentName,
                                      Mark = p.TotalMark.ToString(),
                                      TotalCount = studentCount
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                    else
                    {
                        var model = new Exam.Dto.ExamAnalyze.List()
                        {
                            StudentCode = string.Empty,
                            StudentName = string.Empty,
                            SubjectName = o.SubjectName,
                            SegmentName = o.SegmentName,
                            Mark = string.Empty,
                            TotalCount = 0
                        };
                        lst.Add(model);
                    }
                }
                vm.ExamAnalyzeList = lst;

                return View(vm);
            }
        }

        public ActionResult SegmentMarkDetailList(int examId, int gradeId, int classId, string chkSubject)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();
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
                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id == gradeId
                                        && p.tbSubject.IsDeleted == false
                                        && chksubjectList.Contains(p.tbSubject.Id.ToString())
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           SubjectId = p.tbSubject.Id,
                                           p.tbSubject.SubjectName
                                       }).ToList();

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
                              p.SegmentMark,
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
                              SegmentMark = p.SegmentMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    //分数段人数
                    var tm = (from p in tg
                              where p.SubjectId == o.SubjectId
                              && p.SegmentMark >= o.MinMark && p.SegmentMark <= o.MaxMark
                              select new
                              {
                                  SubjectName = o.SubjectName,
                                  SegmentId = o.SegmentId,
                                  SegmentName = o.SegmentName,
                                  p.StudentCode,
                                  p.StudentName,
                                  p.SegmentMark
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
                                      SegmentName = p.SegmentName,
                                      Mark = p.SegmentMark.ToString(),
                                      TotalCount = studentCount
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                    else
                    {
                        var model = new Exam.Dto.ExamAnalyze.List()
                        {
                            StudentCode = string.Empty,
                            StudentName = string.Empty,
                            SubjectName = o.SubjectName,
                            SegmentName = o.SegmentName,
                            Mark = string.Empty,
                            TotalCount = 0
                        };
                        lst.Add(model);
                    }
                }
                vm.ExamAnalyzeList = lst;

                return View(vm);
            }
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
                                  SubjectName=o.Text,
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
            return Code.MvcHelper.Post(null, Url.Action("TotalMarkTopNList", new { ExamId = vm.ExamId, GradeId = vm.GradeId, chkSubject = chksubjectList,
                chkClass = chkclassList,
                CheckedAll= CheckedAll,
                chkClassAll= chkClassAll,
                searchText = vm.SearchText }));
        }

        public ActionResult SegmentTopNList()
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

                if (vm.chkClass == null || vm.chkSubject == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');
                var chksubjectList = vm.chkSubject.Split(',');

                var selectclassList = new List<System.Web.Mvc.SelectListItem>();
                var selectsubjectList = new List<System.Web.Mvc.SelectListItem>();
                this.getSelectSubjectClassList(chkclassList, chksubjectList, out selectclassList, out selectsubjectList);
                vm.selctClassList = selectclassList;
                vm.selectSubjectList = selectsubjectList;


                //年级班级学生
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

                var rank = vm.SearchText.ConvertToDecimal();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == vm.ExamId || vm.ExamId == 0)
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.SegmentClassRank,
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
                              ClassRank = p.SegmentClassRank,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in selectsubjectList)
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
        public ActionResult SegmentTopNList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("SegmentTopNList", new { ExamId = vm.ExamId, GradeId = vm.GradeId, chkSubject = chksubjectList,
                chkClass = chkclassList,
                CheckedAll = CheckedAll,
                chkClassAll = chkClassAll,
                searchText = vm.SearchText }));
        }

        public ActionResult TotalMarkTopNDetailList(int examId, int gradeId, int classId, string chkSubject,decimal? rank)
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
                var lsttotal =new List<string>();
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
                //图形数据
                vm.chkSubject = ToJSONString(SubjectList.Select(d => d.SubjectName).ToList());
            
                vm.totalCommData = ToJSONString(lsttotal);

                return View(vm);
            }
        }

        public ActionResult SegmentTopNDetailList(int examId, int gradeId, int classId, string chkSubject, decimal? rank)
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
                              p.SegmentClassRank,
                              p.SegmentMark,
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
                              ClassRank = p.SegmentClassRank,
                              p.SegmentMark
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
                                  p.SegmentMark
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
                                      Mark = p.SegmentMark.ToString(),
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

                //图形数据
                vm.chkSubject = ToJSONString(SubjectList.Select(d => d.SubjectName).ToList());

                vm.totalCommData = ToJSONString(lsttotal);

                return View(vm);
            }
        }

        #endregion

        #region 班级平均分
        public ActionResult TotalMarkAvgList()
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
                vm.OptionList = new List<string>() { "平均分", "多班排名" };

                //科目和班级
                var classList = new List<System.Web.Mvc.SelectListItem>();
                var subjectList = new List<System.Web.Mvc.SelectListItem>();
                var yearId = 0;
                this.getSubjectClassList(vm.ExamId, vm.GradeId, vm.SearchText, out classList, out subjectList, out yearId);
                vm.SubjectList = subjectList;
                vm.ClassList = classList;

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
                              SubjectId = p.SubjectId,
                              TotalMark = p.TotalMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in selectsubjectList)
                {
                    //班级人数
                    var subjectId = o.Value.ConvertToInt();
                    var classAvgList = (from p in tg
                                        where p.SubjectId == subjectId
                                        group p by new
                                        {
                                            p.ClassId,
                                            p.ClassName
                                        } into g
                                        select new
                                        {
                                            g.Key.ClassId,
                                            AvgMark = g.Average(d => d.TotalMark)
                                        }).ToList();

                    var tb = (from p in classAvgList
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = subjectId,
                                  AvgMark = Math.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                  ClassRank = decimal.Zero
                              }).ToList();
                    lst.AddRange(tb);
                }
                //排名
                foreach (var subject in selectsubjectList)
                {
                    var rank = decimal.Zero;
                    decimal? mark = null;
                    var count = decimal.One;
                    foreach (var t in lst.Where(d => d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.AvgMark))
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
                vm.ExamAnalyzeList = lst;
                var lstSubject =new  List<object>();
                //图形数据
                vm.chkSelectClass = ToJSONString(vm.selctClassList.Select(d => d.Text).ToList());
                foreach(var s in vm.selectSubjectList)
                {
                    lstSubject.Add(new { name = s.Text });
                }
                vm.chkSelectSubject = ToJSONString(lstSubject);

                var lstData = new List<object>();
                foreach (var o in vm.selctClassList)
                {
                    var lstAvg = new List<object>();
                    foreach (var k in lst.Where(d=>d.ClassId==o.Value.ConvertToInt()))
                    {
                        lstAvg.Add(k.AvgMark);
                    }
                    lstData.Add(new { value = lstAvg,name = o.Text});
                }
                vm.totalCommData = ToJSONString(lstData);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TotalMarkAvgList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("TotalMarkAvgList", new { ExamId = vm.ExamId, GradeId = vm.GradeId,
                chkSubject = chksubjectList, chkClass = chkclassList,
                CheckedAll = CheckedAll,
                chkClassAll = chkClassAll,
                searchText = vm.SearchText }));
        }

        public ActionResult SegmentAvgList()
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
                vm.OptionList = new List<string>() { "平均分", "多班排名" };

                //科目和班级
                var classList = new List<System.Web.Mvc.SelectListItem>();
                var subjectList = new List<System.Web.Mvc.SelectListItem>();
                var yearId = 0;
                this.getSubjectClassList(vm.ExamId, vm.GradeId, vm.SearchText, out classList, out subjectList, out yearId);
                vm.SubjectList = subjectList;
                vm.ClassList = classList;

                //选中班级和科目
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
                              p.SegmentMark,
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
                              SegmentMark = p.SegmentMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in selectsubjectList)
                {
                    //班级人数
                    var subjectId = o.Value.ConvertToInt();
                    var classAvgList = (from p in tg
                                        where p.SubjectId == subjectId
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
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = subjectId,
                                  AvgMark =Math.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                  ClassRank = decimal.Zero
                              }).ToList();
                    lst.AddRange(tb);
                }
                //排名
                foreach (var subject in selectsubjectList)
                {
                    var rank = decimal.Zero;
                    decimal? mark = null;
                    var count = decimal.One;
                    foreach (var t in lst.Where(d => d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.AvgMark))
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
                vm.ExamAnalyzeList = lst;
                var lstSubject = new List<object>();
                //图形数据
                vm.chkSelectClass = ToJSONString(vm.selctClassList.Select(d => d.Text).ToList());
                foreach (var s in vm.selectSubjectList)
                {
                    lstSubject.Add(new { name = s.Text });
                }
                vm.chkSelectSubject = ToJSONString(lstSubject);

                var lstData = new List<object>();
                foreach (var o in vm.selctClassList)
                {
                    var lstAvg = new List<object>();
                    foreach (var k in lst.Where(d => d.ClassId == o.Value.ConvertToInt()))
                    {
                        lstAvg.Add(k.AvgMark);
                    }
                    lstData.Add(new { value = lstAvg, name = o.Text });
                }
                vm.totalCommData = ToJSONString(lstData);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SegmentAvgList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("SegmentAvgList", new { ExamId = vm.ExamId, GradeId = vm.GradeId,
                chkSubject = chksubjectList, chkClass = chkclassList,
                CheckedAll= CheckedAll,
                chkClassAll= chkClassAll,
                searchText = vm.SearchText }));
        }

        #endregion

        #region 单科等级
        public ActionResult SegmentLevelList()
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
                vm.OptionList = new List<string>() { "人数", "比率" };

                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && p.tbExamLevelGroup.IsDeleted == false
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id,
                                       LevelGroupId=p.tbExamLevelGroup.Id
                                   }).Distinct().ToList();

                vm.SubjectList = (from p in SubjectList
                                  select new System.Web.Mvc.SelectListItem
                                  {
                                      Text = p.SubjectName,
                                      Value = p.SubjectId.ToString()
                                  }).ToList();
                if(vm.SubjectList.Count>0 && vm.SubjectId==0)
                {
                    vm.SubjectId = vm.SubjectList.FirstOrDefault().Value.ConvertToInt();
                }

                var LevelGroupId = (from p in SubjectList
                                    where p.SubjectId == vm.SubjectId
                                    select p.LevelGroupId).FirstOrDefault();
                
                var LevelList=(from p in db.Table<Exam.Entity.tbExamLevel>()
                              where p.tbExamLevelGroup.Id==LevelGroupId
                              orderby p.No,p.ExamLevelName
                              select new
                              {
                                  ExamLevelName = p.ExamLevelName,
                                  LevelId = p.Id
                              }).ToList();

                vm.LevelList = (from p in LevelList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ExamLevelName,
                                    Value = p.LevelId.ToString()
                                }).ToList();

                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == vm.ExamId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    orderby p.tbClass.No
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();

               var tbclassStudent = (from p in classStudent
                                select new 
                                {
                                    p.ClassId,
                                    p.ClassName
                                }).Distinct().ToList();
                //班级
                vm.ClassList = (from p in tbclassStudent
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Value = p.ClassId.ToString(),
                                    Text = p.ClassName,
                                }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == vm.ExamId || vm.ExamId == 0)
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.Id==vm.SubjectId
                           && (p.tbExamLevel.IsDeleted == false || p.tbExamLevel==null)
                          select new Exam.Dto.ExamAnalyze.List
                          {
                              LevelId= p.tbExamLevel.Id,
                              StudentId = p.tbStudent.Id,
                          }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              LevelId = p.LevelId,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in LevelList)
                {
                    //班级等级人数
                    var levelId = o.LevelId;
                    var classTotalCount =(from p in tg
                                        group p by new
                                        {
                                            p.ClassId
                                        } into g
                                        select new
                                        {
                                            g.Key.ClassId,
                                            TotalCount = g.Count()
                                        }).ToList();

                    var tm = (from p in tg
                              where p.LevelId == levelId
                              group p by new
                              {
                                  p.ClassId
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  StudentCount = g.Count(),
                                  TotalCount = classTotalCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  LevelId = o.LevelId,
                                  StudentCount=p.StudentCount.ToString(),
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
        public ActionResult SegmentLevelList(Models.ExamAnalyze.List vm)
        {
            return Code.MvcHelper.Post(null, Url.Action("SegmentLevelList", new { ExamId = vm.ExamId, GradeId = vm.GradeId, SubjectId=vm.SubjectId,searchText = vm.SearchText }));
        }

        public ActionResult SegmentLevelDetailList(int examId, int gradeId, int classId, int subjectId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();
                //考试科目等级
                var LevelGroup = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == examId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && p.tbCourse.tbSubject.Id==subjectId
                                    && p.tbExamLevelGroup.IsDeleted == false
                                  select new
                                   {
                                       p.tbCourse.tbSubject.SubjectName,
                                       p.tbExamLevelGroup.Id
                                   }).FirstOrDefault();

                if (LevelGroup != null)
                {
                    var LevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                     where p.tbExamLevelGroup.Id == LevelGroup.Id
                                     orderby p.No, p.ExamLevelName
                                     select new
                                     {
                                         ExamLevelName = p.ExamLevelName,
                                         LevelId = p.Id
                                     }).ToList();

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
                               && p.tbExamCourse.tbCourse.IsDeleted == false
                               && p.tbExamCourse.tbCourse.tbSubject.Id == subjectId
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  Mark = p.SegmentMark.ToString(),
                                  StudentId = p.tbStudent.Id,
                                  LevelId = p.tbExamLevel.Id
                              }).ToList();

                    var tg = (from p in tf
                              join t in classStudent
                              on p.StudentId equals t.StudentId
                              select new
                              {
                                  StudentId = p.StudentId,
                                  t.StudentCode,
                                  t.StudentName,
                                  p.LevelId,
                                  Mark = p.Mark
                              }).ToList();
                    var lst = new List<Exam.Dto.ExamAnalyze.List>();
                    var lsttotal = new List<string>();
                    foreach (var o in LevelList)
                    {
                        //分数段人数
                        var tm = (from p in tg
                                  where p.LevelId == o.LevelId
                                  select new
                                  {
                                      p.StudentCode,
                                      p.StudentName,
                                      p.Mark
                                  }).ToList();
                        var studentCount = tm.Count();

                        if (studentCount > decimal.Zero)
                        {
                            var tb = (from p in tm
                                      select new Exam.Dto.ExamAnalyze.List
                                      {
                                          SubjectName = LevelGroup.SubjectName,
                                          StudentCode = p.StudentCode,
                                          StudentName = p.StudentName,
                                          LevelName = o.ExamLevelName,
                                          Mark = p.Mark,
                                          TotalCount = studentCount
                                      }).ToList();
                            lst.AddRange(tb);
                            lsttotal.Add(studentCount.ToString());
                        }
                        else
                        {
                            var model = new Exam.Dto.ExamAnalyze.List()
                            {
                                SubjectName = LevelGroup.SubjectName,
                                StudentCode = string.Empty,
                                StudentName = string.Empty,
                                LevelName = o.ExamLevelName,
                                Mark = string.Empty,
                                TotalCount = 0
                            };
                            lst.Add(model);
                            lsttotal.Add(studentCount.ToString());
                        }
                    }
                    vm.ExamAnalyzeList = lst;

                    //图形数据
                    vm.chkSubject = ToJSONString(LevelList.Select(d => d.ExamLevelName).ToList());

                    vm.totalCommData = ToJSONString(lsttotal);
                }
                return View(vm);
            }
        }

        #endregion

        #region 综合成绩析
        public ActionResult CompreList()
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
                vm.GoodPassList= new List<string>() { "优秀", "及格"};
                vm.OptionList = new List<string>() { "人数","比率","排名"};
                //科目和班级
                var classList = new List<System.Web.Mvc.SelectListItem>();
                var subjectList = new List<System.Web.Mvc.SelectListItem>();
                var yearId = 0;
                this.getSubjectClassList(vm.ExamId, vm.GradeId, vm.SearchText, out classList, out subjectList, out yearId);
                vm.SubjectList = subjectList;
                vm.ClassList = classList;

                //选中班级和科目
                if (vm.chkClass == null || vm.chkSubject == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');
                var chksubjectList = vm.chkSubject.Split(',');

                var selectclassList = new List<System.Web.Mvc.SelectListItem>();
                var selectsubjectList = new List<System.Web.Mvc.SelectListItem>();
                this.getSelectSubjectClassList(chkclassList, chksubjectList, out selectclassList, out selectsubjectList);
                vm.selctClassList = selectclassList;
                vm.selectSubjectList = selectsubjectList;

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id == vm.GradeId
                                        && p.tbSubject.IsDeleted == false
                                        && chksubjectList.Contains(p.tbSubject.Id.ToString())
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsPass,
                                           SubjectId = p.tbSubject.Id,
                                           p.tbSubject.SubjectName
                                       }).ToList();

                vm.SegmentList = (from p in SegmentMarkList
                                  select new Exam.Dto.ExamAnalyze.SegmentList
                                  {
                                      SegmentName = p.SegmentName,
                                      SegmentId = p.SegmentId,
                                      SubjectId = p.SubjectId
                                  }).Distinct().ToList();

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
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullSegmentMark
                          }).ToList();

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
                              FullSegmentMark = p.FullSegmentMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                var lstTotal = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;
                   
                    //优秀科目人数
                    if (isGood)
                    {
                        //分数段人数
                        var tm = (from p in tg
                                  where p.SubjectId == o.SubjectId
                                  && p.SegmentMark >= o.MinMark && p.SegmentMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                      p.ClassName
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      g.Key.ClassName,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      ClassName=p.ClassName,
                                      SubjectId = o.SubjectId,
                                      SubjectName=o.SubjectName,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                    if(isPass)//及格人数
                    {
                        //分数段人数
                        var tm = (from p in tg
                                  where p.SubjectId == o.SubjectId
                                  && p.SegmentMark >= o.MinMark * p.FullSegmentMark / 100 && p.SegmentMark <= o.MaxMark * p.FullSegmentMark / 100
                                  group p by new
                                  {
                                      p.ClassId,
                                      p.ClassName
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      g.Key.ClassName,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      ClassName=p.ClassName,
                                      SubjectId = o.SubjectId,
                                      SubjectName=o.SubjectName,
                                      Status =2,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }
                //统计人数
                foreach(var subject in selectsubjectList)
                {
                    //班级科目人数
                    var totalStudetCount = (from p in tg
                                            where p.SubjectId == subject.Value.ConvertToInt()
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                TotalCount = g.Count()
                                            }).ToList();
                    var tt = (from p in lst
                              where p.SubjectId == subject.Value.ConvertToInt()
                              group p by new
                              {
                                  p.ClassId,
                                  p.ClassName,
                                  p.Status,
                                  p.SubjectId,
                                  p.SubjectName
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  g.Key.ClassName,
                                  g.Key.Status,
                                  g.Key.SubjectId,
                                  g.Key.SubjectName,
                                  StudentNum=g.Sum(d=>d.StudentNum),
                                  TotalCount = totalStudetCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tk =(from p in tt
                           select new Exam.Dto.ExamAnalyze.List
                           {
                               ClassId=p.ClassId,
                               ClassName=p.ClassName,
                               SubjectId=p.SubjectId,
                               SubjectName=p.SubjectName,
                               Status=p.Status,
                               StudentNum=p.StudentNum,
                               ClassRank=0,
                               Percent= p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentNum * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero)) : decimal.Zero,
                               Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentNum * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                           }).ToList();

                    lstTotal.AddRange(tk);
                }
                //分组班级科目
                foreach (var c in vm.selctClassList)
                {
                    foreach (var s in selectsubjectList)
                    {
                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.ClassId = c.Value.ConvertToInt();
                        model.ClassName = c.Text;
                        model.SubjectId = s.Value.ConvertToInt();
                        model.SubjectName = s.Text;

                        vm.CompreList.Add(model);
                    }
                }

                //优秀率排名
                foreach (var s in selectsubjectList)
                {
                    var rank = decimal.Zero;
                    decimal? mark = null;
                    var count = decimal.One;
                    foreach (var t in lstTotal.Where(d => d.Status == decimal.One && d.SubjectId==s.Value.ConvertToInt()).OrderByDescending(d => d.Percent))
                    {
                        if (mark != t.Percent)
                        {
                            mark = t.Percent;
                            rank = rank + count;
                            count = decimal.One;
                        }
                        else
                        {
                            count = count + decimal.One;
                        }

                        t.ClassRank = rank;
                    }

                    rank = decimal.Zero;
                    mark = null;
                    count = decimal.One;
                    foreach (var t in lstTotal.Where(d => d.Status == 2 && d.SubjectId==s.Value.ConvertToInt()).OrderByDescending(d => d.Percent))
                    {
                        if (mark != t.Percent)
                        {
                            mark = t.Percent;
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
                vm.ExamAnalyzeList = lstTotal;
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompreList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("CompreList", new { ExamId = vm.ExamId, GradeId = vm.GradeId,
                chkSubject = chksubjectList, chkClass = chkclassList,
                CheckedAll= CheckedAll,
                chkClassAll= chkClassAll,
                searchText = vm.SearchText }));
        }

        public ActionResult CompreTotalMarkList()
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
                vm.GoodPassList = new List<string>() { "优秀", "及格" };
                vm.OptionList = new List<string>() { "人数", "比率", "排名" };
                //科目和班级
                var classList = new List<System.Web.Mvc.SelectListItem>();
                var subjectList = new List<System.Web.Mvc.SelectListItem>();
                var yearId = 0;
                this.getSubjectClassList(vm.ExamId, vm.GradeId, vm.SearchText, out classList, out subjectList, out yearId);
                vm.SubjectList = subjectList;
                vm.ClassList = classList;

                //选中班级和科目
                if (vm.chkClass == null || vm.chkSubject == null) return View(vm);
                var chkclassList = vm.chkClass.Split(',');
                var chksubjectList = vm.chkSubject.Split(',');

                var selectclassList = new List<System.Web.Mvc.SelectListItem>();
                var selectsubjectList = new List<System.Web.Mvc.SelectListItem>();
                this.getSelectSubjectClassList(chkclassList, chksubjectList, out selectclassList, out selectsubjectList);
                vm.selctClassList = selectclassList;
                vm.selectSubjectList = selectsubjectList;

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id == vm.GradeId
                                        && p.tbSubject.IsDeleted == false
                                        && chksubjectList.Contains(p.tbSubject.Id.ToString())
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsPass,
                                           SubjectId = p.tbSubject.Id,
                                           p.tbSubject.SubjectName
                                       }).ToList();

                vm.SegmentList = (from p in SegmentMarkList
                                  select new Exam.Dto.ExamAnalyze.SegmentList
                                  {
                                      SegmentName = p.SegmentName,
                                      SegmentId = p.SegmentId,
                                      SubjectId = p.SubjectId
                                  }).Distinct().ToList();

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
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullTotalMark
                          }).ToList();

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
                              FullTotalMark = p.FullTotalMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                var lstTotal = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;

                    //优秀科目人数
                    if (isGood)
                    {
                        //分数段人数
                        var tm = (from p in tg
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                      p.ClassName
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      g.Key.ClassName,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      ClassName = p.ClassName,
                                      SubjectId = o.SubjectId,
                                      SubjectName = o.SubjectName,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                    if (isPass)//及格人数
                    {
                        //分数段人数
                        var tm = (from p in tg
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark * p.FullTotalMark / 100 && p.TotalMark <= o.MaxMark * p.FullTotalMark / 100
                                  group p by new
                                  {
                                      p.ClassId,
                                      p.ClassName
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      g.Key.ClassName,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      ClassName = p.ClassName,
                                      SubjectId = o.SubjectId,
                                      SubjectName = o.SubjectName,
                                      Status = 2,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }
                //统计人数
                foreach (var subject in selectsubjectList)
                {
                    //班级科目人数
                    var totalStudetCount = (from p in tg
                                            where p.SubjectId == subject.Value.ConvertToInt()
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                TotalCount = g.Count()
                                            }).ToList();
                    var tt = (from p in lst
                              where p.SubjectId == subject.Value.ConvertToInt()
                              group p by new
                              {
                                  p.ClassId,
                                  p.ClassName,
                                  p.Status,
                                  p.SubjectId,
                                  p.SubjectName
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  g.Key.ClassName,
                                  g.Key.Status,
                                  g.Key.SubjectId,
                                  g.Key.SubjectName,
                                  StudentNum = g.Sum(d => d.StudentNum),
                                  TotalCount = totalStudetCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tk = (from p in tt
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  ClassName = p.ClassName,
                                  SubjectId = p.SubjectId,
                                  SubjectName = p.SubjectName,
                                  Status = p.Status,
                                  StudentNum = p.StudentNum,
                                  ClassRank = 0,
                                  Percent = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentNum * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero)) : decimal.Zero,
                                  Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentNum * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                              }).ToList();

                    lstTotal.AddRange(tk);
                }
                //分组班级科目
                foreach (var c in vm.selctClassList)
                {
                    foreach (var s in selectsubjectList)
                    {
                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.ClassId = c.Value.ConvertToInt();
                        model.ClassName = c.Text;
                        model.SubjectId = s.Value.ConvertToInt();
                        model.SubjectName = s.Text;

                        vm.CompreList.Add(model);
                    }
                }

                //优秀率排名
                foreach (var s in selectsubjectList)
                {
                    var rank = decimal.Zero;
                    decimal? mark = null;
                    var count = decimal.One;
                    foreach (var t in lstTotal.Where(d => d.Status == decimal.One && d.SubjectId==s.Value.ConvertToInt()).OrderByDescending(d => d.Percent))
                    {
                        if (mark != t.Percent)
                        {
                            mark = t.Percent;
                            rank = rank + count;
                            count = decimal.One;
                        }
                        else
                        {
                            count = count + decimal.One;
                        }

                        t.ClassRank = rank;
                    }

                    rank = decimal.Zero;
                    mark = null;
                    count = decimal.One;
                    foreach (var t in lstTotal.Where(d => d.Status == 2 && d.SubjectId==s.Value.ConvertToInt()).OrderByDescending(d => d.Percent))
                    {
                        if (mark != t.Percent)
                        {
                            mark = t.Percent;
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
                vm.ExamAnalyzeList = lstTotal;
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompreTotalMarkList(Models.ExamAnalyze.List vm)
        {
            var arrystr = string.Empty;
            var chksubjectList = Request["chkSubject"] != null ? Request.Form["chkSubject"] : arrystr;
            var chkclassList = Request["chkClass"] != null ? Request.Form["chkClass"] : arrystr;
            var CheckedAll = Request["chkSubjectAll"] != null ? Request.Form["chkSubjectAll"] : arrystr;
            var chkClassAll = Request["chkClassAll"] != null ? Request.Form["chkClassAll"] : arrystr;
            return Code.MvcHelper.Post(null, Url.Action("CompreTotalMarkList", new { ExamId = vm.ExamId, GradeId = vm.GradeId, chkSubject = chksubjectList,
                chkClass = chkclassList,
                CheckedAll = CheckedAll,
                chkClassAll = chkClassAll,
                searchText = vm.SearchText }));
        }

        public ActionResult CompreDetailList(int examId, int gradeId, int classId, int  subjectId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();
                
                //考试科目
                var SubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id == vm.ExamId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && p.tbCourse.tbSubject.Id==subjectId
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
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == vm.ExamId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id == vm.GradeId
                                        && p.tbSubject.IsDeleted == false
                                        && p.tbSubject.Id==subjectId
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsPass
                                       }).ToList();


                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && p.tbClass.Id==classId
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentCode,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == vm.ExamId || vm.ExamId == 0)
                           && p.tbExamCourse.tbCourse.tbSubject.Id==subjectId
                          select new
                          {
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullSegmentMark
                          }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              t.StudentCode,
                              t.StudentName,
                              SegmentMark = p.SegmentMark,
                              FullSegmentMark = p.FullSegmentMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                var lstTotal = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;

                    //优秀科目人数
                    if (isGood)
                    {
                        //分数段人数
                        var StudentCount = (from p in tg
                                            where p.SegmentMark >= o.MinMark * p.FullSegmentMark / 100 && p.SegmentMark <= o.MaxMark * p.FullSegmentMark / 100
                                            select decimal.One).Count(); 

                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.Status = decimal.One;
                        model.StudentNum = StudentCount;
                        lst.Add(model);
                    }
                    if (isPass)//及格人数
                    {
                        //分数段人数
                        var StudentCount = (from p in tg
                                            where p.SegmentMark >= o.MinMark * p.FullSegmentMark / 100 && p.SegmentMark <= o.MaxMark * p.FullSegmentMark / 100
                                            select decimal.One).Count(); 

                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.Status =2;
                        model.StudentNum = StudentCount;
                        lst.Add(model);
                    }
                  
                }
                //每个学生成绩优秀以及及格分析
                foreach (var student in classStudent)
                {
                    var goodCount = decimal.Zero;
                    var passCount = decimal.Zero;
                    var goodNo = decimal.Zero;
                    var passNo = decimal.Zero;
                    foreach (var o in SegmentMarkList)
                    {
                        var isGood = o.IsGood;
                        var isPass = o.IsPass;
                        var ty = (from p in tg
                                  where p.StudentId == student.StudentId
                                  && p.SegmentMark >= o.MinMark * p.FullSegmentMark / 100 && p.SegmentMark <= o.MaxMark * p.FullSegmentMark / 100
                                  select new
                                  {
                                      isGood,
                                      isPass,
                                      p.SegmentMark
                                  }).ToList();

                        goodCount = ty.Where(d => d.isGood == true).Count();
                        passCount = ty.Where(d => d.isPass == true).Count();

                        if(goodCount>decimal.Zero)
                        {
                            goodNo++;
                        }

                        if (passCount > decimal.Zero)
                        {
                            passNo++;
                        }
                    }
                    var model = new Exam.Dto.ExamAnalyze.List();
                    model.StudentCode = student.StudentCode;
                    model.StudentName = student.StudentName;
                    model.Mark = tg.Where(d => d.StudentId == student.StudentId).Select(d => d.SegmentMark).FirstOrDefault()!=null?
                        tg.Where(d => d.StudentId == student.StudentId).Select(d => d.SegmentMark).FirstOrDefault().ToString():string.Empty;
                    model.IsGood = goodNo > decimal.Zero ? "是" : string.Empty;
                    model.IsPass = passNo > decimal.Zero ? "是" : string.Empty;

                    lstTotal.Add(model);
                }
                //优秀及格人数 status  1:优秀  2:及格
                var tw = (from p in lst
                          group p by new
                          {
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum),
                          }).ToList();

                vm.goodCount = tw.Where(d => d.Status == decimal.One).Select(d=>d.StudentNum).FirstOrDefault();
                vm.passCount = tw.Where(d => d.Status == 2).Select(d => d.StudentNum).FirstOrDefault();

                vm.ExamAnalyzeList = lstTotal;
                return View(vm);
            }
        }

        public ActionResult CompreTotalMarkDetailList(int examId, int gradeId, int classId, int subjectId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var vm = new Models.ExamAnalyze.List();

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

                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == vm.ExamId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id == vm.GradeId
                                        && p.tbSubject.IsDeleted == false
                                        && p.tbSubject.Id == subjectId
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsPass
                                       }).ToList();


                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == vm.GradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    && p.tbClass.Id == classId
                                    select new
                                    {
                                        StudentId = p.tbStudent.Id,
                                        p.tbStudent.StudentCode,
                                        p.tbStudent.StudentName
                                    }).ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == vm.ExamId || vm.ExamId == 0)
                           && p.tbExamCourse.tbCourse.tbSubject.Id == subjectId
                          select new
                          {
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullTotalMark
                          }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              t.StudentCode,
                              t.StudentName,
                              TotalMark = p.TotalMark,
                              FullTotalMark = p.FullTotalMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                var lstTotal = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;

                    //优秀科目人数
                    if (isGood)
                    {
                        //分数段人数
                        var StudentCount = (from p in tg
                                            where p.TotalMark >= o.MinMark * p.FullTotalMark / 100 && p.TotalMark <= o.MaxMark * p.FullTotalMark / 100
                                            select decimal.One).Count();

                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.Status = decimal.One;
                        model.StudentNum = StudentCount;
                        lst.Add(model);
                    }
                    if (isPass)//及格人数
                    {
                        //分数段人数
                        var StudentCount = (from p in tg
                                            where p.TotalMark >= o.MinMark * p.FullTotalMark / 100 && p.TotalMark <= o.MaxMark * p.FullTotalMark / 100
                                            select decimal.One).Count();

                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.Status = 2;
                        model.StudentNum = StudentCount;
                        lst.Add(model);
                    }

                }
                //每个学生成绩优秀以及及格分析
                foreach (var student in classStudent)
                {
                    var goodCount = decimal.Zero;
                    var passCount = decimal.Zero;
                    var goodNo = decimal.Zero;
                    var passNo = decimal.Zero;
                    foreach (var o in SegmentMarkList)
                    {
                        var isGood = o.IsGood;
                        var isPass = o.IsPass;
                        var ty = (from p in tg
                                  where p.StudentId == student.StudentId
                                  && p.TotalMark >= o.MinMark * p.FullTotalMark / 100 && p.TotalMark <= o.MaxMark * p.FullTotalMark / 100
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
                    var model = new Exam.Dto.ExamAnalyze.List();
                    model.StudentCode = student.StudentCode;
                    model.StudentName = student.StudentName;
                    model.Mark = tg.Where(d => d.StudentId == student.StudentId).Select(d => d.TotalMark).FirstOrDefault() != null ?
                        tg.Where(d => d.StudentId == student.StudentId).Select(d => d.TotalMark).FirstOrDefault().ToString() : string.Empty;
                    model.IsGood = goodNo > decimal.Zero ? "是" : string.Empty;
                    model.IsPass = passNo > decimal.Zero ? "是" : string.Empty;

                    lstTotal.Add(model);
                }
                //优秀及格人数 status  1:优秀  2:及格
                var tw = (from p in lst
                          group p by new
                          {
                              p.Status
                          } into g
                          select new
                          {
                              g.Key.Status,
                              StudentNum = g.Sum(d => d.StudentNum),
                          }).ToList();
                vm.goodCount = tw.Where(d => d.Status == decimal.One).Select(d => d.StudentNum).FirstOrDefault();
                vm.passCount = tw.Where(d => d.Status == 2).Select(d => d.StudentNum).FirstOrDefault();

                vm.ExamAnalyzeList = lstTotal;
                return View(vm);
            }
        }

        #endregion

        #region 导出
        public ActionResult Export(int examId,int gradeId, string chkSubject,string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
                //学年
                var yearId = this.getYear(examId);

                if (chkClass == null || chkSubject == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList =chkClass.Split(',');
                var chksubjectList =chkSubject.Split(',');

                var selectclassList = new List<System.Web.Mvc.SelectListItem>();
                var selectsubjectList = new List<System.Web.Mvc.SelectListItem>();
                this.getSelectSubjectClassList(chkclassList, chksubjectList, out selectclassList, out selectsubjectList);
               

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id ==gradeId
                                        && p.tbSubject.IsDeleted == false
                                        && chksubjectList.Contains(p.tbSubject.Id.ToString())
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           SubjectId = p.tbSubject.Id
                                       }).ToList();

                var SegmentList = (from p in SegmentMarkList
                                  select new Exam.Dto.ExamAnalyze.SegmentList
                                  {
                                      SegmentName = p.SegmentName,
                                      SegmentId = p.SegmentId,
                                      SubjectId = p.SubjectId
                                  }).Distinct().ToList();


               

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

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id ==examId || examId == 0)
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullTotalMark
                          }).ToList();

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
                              FullTotalMark = p.FullTotalMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    //班级人数
                    var totalStudetCount = (from p in tg
                                            where p.SubjectId == o.SubjectId
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                TotalCount = g.Count()
                                            }).ToList();
                    //分数段人数
                    var tm = (from p in tg
                              where p.SubjectId == o.SubjectId
                              && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                              group p by new
                              {
                                  p.ClassId
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  SegmentId = o.SegmentId,
                                  StudentCount = g.Count(),
                                  TotalCount = totalStudetCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = o.SubjectId,
                                  SegmentId = p.SegmentId,
                                  Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentCount * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                              }).ToList();
                    lst.AddRange(tb);
                }

                #endregion

                #region 导出
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = SetCellStyle(hssfworkbook); 

                var sheetName =db.Table<Exam.Entity.tbExam>().Where(d => d.Id == examId).Select(d => d.ExamName).FirstOrDefault() + "多科考试成绩分数段";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                //表头第一行
                ICell cell = cellHeader.CreateCell(0);
                cell.SetCellValue("班级");
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex+1, 0,0);
                sheet1.AddMergedRegion(cellRangeAddress);
                cell.CellStyle = cellstyle;

                var No = 0;
                var count = 0;
                foreach (var subject in selectsubjectList)
                {
                    var segmentCount = SegmentList.Where(d => d.SubjectId == subject.Value.ConvertToInt()).ToList().Count();
                    if (segmentCount > 0)
                    {
                        count += segmentCount;

                        //表头
                        cell = cellHeader.CreateCell(count-segmentCount + 1);
                        cell.SetCellValue(subject.Text);
                        cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, count - segmentCount + 1, count);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                        No++;
                    }
                }
                //第二行
                cellHeader = sheet1.CreateRow(rowStartIndex+1);
                No = 0;
                foreach (var subject in selectsubjectList)
                {
                    foreach (var k in SegmentList.Where(d => d.SubjectId == subject.Value.ConvertToInt()))
                    {
                        cell = cellHeader.CreateCell(No + 1);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(k.SegmentName);
                        No++;
                    }
                }
                //跨行边框无显示
                cellHeader.CreateCell(0).SetCellValue("班级");
                cellHeader.GetCell(0).CellStyle = cellstyle;

                rowStartIndex++;

                //数据行
                foreach (var t in selectclassList)
                {
                    cellHeader = sheet1.CreateRow(rowStartIndex+1);

                    cell = cellHeader.CreateCell(0);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(t.Text);

                    No = 0;
                    foreach (var subject in selectsubjectList)
                    {
                        foreach (var s in SegmentList.Where(d => d.SubjectId == subject.Value.ConvertToInt()))
                        {
                            var mark = lst.Where(d => d.SubjectId == subject.Value.ConvertToInt() && d.ClassId == t.Value.ConvertToInt()
                                                                                && d.SegmentId == s.SegmentId).Select(d => d).FirstOrDefault();
                            if(mark!=null)
                            {
                                cell = cellHeader.CreateCell(No+1);
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
                            }
                        }
                    }

                    rowStartIndex++;
                }
                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("多科分数段报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportSegment(int examId, int gradeId, string chkSubject, string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
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
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == examId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                //班级
                var ClassStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == gradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    orderby p.tbClass.No, p.tbClass.ClassName
                                    select new
                                    {
                                        CiassId = p.tbClass.Id,
                                        p.tbClass.ClassName
                                    }).Distinct().ToList(); ;

                var ClassList = (from p in ClassStudent
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Value = p.CiassId.ToString(),
                                    Text = p.ClassName,
                                }).Distinct().ToList();

                if (chkClass == null || chkSubject == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList = chkClass.Split(',');
                var chksubjectList = chkSubject.Split(',');

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id == gradeId
                                        && p.tbSubject.IsDeleted == false
                                        && chksubjectList.Contains(p.tbSubject.Id.ToString())
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           SubjectId = p.tbSubject.Id
                                       }).ToList();

                var SegmentList = (from p in SegmentMarkList
                                   select new Exam.Dto.ExamAnalyze.SegmentList
                                   {
                                       SegmentName = p.SegmentName,
                                       SegmentId = p.SegmentId,
                                       SubjectId = p.SubjectId
                                   }).Distinct().ToList();


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

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == examId || examId == 0)
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullTotalMark
                          }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              TotalMark = p.SegmentMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              FullTotalMark = p.FullTotalMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    //班级人数
                    var totalStudetCount = (from p in tg
                                            where p.SubjectId == o.SubjectId
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                TotalCount = g.Count()
                                            }).ToList();
                    //分数段人数
                    var tm = (from p in tg
                              where p.SubjectId == o.SubjectId
                              && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                              group p by new
                              {
                                  p.ClassId
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  SegmentId = o.SegmentId,
                                  StudentCount = g.Count(),
                                  TotalCount = totalStudetCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = o.SubjectId,
                                  SegmentId = p.SegmentId,
                                  Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentCount * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                              }).ToList();
                    lst.AddRange(tb);
                }

                #endregion

                #region 导出
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = SetCellStyle(hssfworkbook);

                var sheetName = db.Table<Exam.Entity.tbExam>().Where(d => d.Id == examId).Select(d => d.ExamName).FirstOrDefault() + "多科综合成绩分数段";

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
                var count = 0;
                foreach (var subject in selectSubject)
                {
                    var segmentCount = SegmentList.Where(d => d.SubjectId == subject.Value.ConvertToInt()).ToList().Count();
                    if (segmentCount > 0)
                    {
                        count += segmentCount;

                        //表头
                        cell = cellHeader.CreateCell(count - segmentCount + 1);
                        cell.SetCellValue(subject.Text);
                        cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, count - segmentCount + 1, count);
                        setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                        sheet1.AddMergedRegion(cellRangeAddress);
                        No++;
                    }
                }
                //第二行
                cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                No = 0;
                foreach (var subject in selectSubject)
                {
                    foreach (var k in SegmentList.Where(d => d.SubjectId == subject.Value.ConvertToInt()))
                    {
                        cell = cellHeader.CreateCell(No + 1);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(k.SegmentName);
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
                    foreach (var subject in selectSubject)
                    {
                        foreach (var s in SegmentList.Where(d => d.SubjectId == subject.Value.ConvertToInt()))
                        {
                            var mark = lst.Where(d => d.SubjectId == subject.Value.ConvertToInt() && d.ClassId == t.Value.ConvertToInt()
                                                                                && d.SegmentId == s.SegmentId).Select(d => d).FirstOrDefault();
                            if (mark != null)
                            {
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
                            }
                        }
                    }

                    rowStartIndex++;
                }
                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("多科综合分分数段报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

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

                #region 数据统计
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id ==examId
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
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id ==examId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                //班级
                var tbClassStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                      where p.tbStudent.IsDeleted == false
                                      && p.tbClass.tbGrade.Id ==gradeId
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
                var chksubjectList =chkSubject.Split(',');

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

                var sheetName = db.Table<Exam.Entity.tbExam>().Where(d => d.Id == examId).Select(d => d.ExamName).FirstOrDefault() + "考试成绩前"+searchText+"名";

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
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count()*No + 1, OptionList.Count() * (No + 1));
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    No++;
                }

                //第二行
                cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                No = 0;
                foreach (var subject in selectSubjectList)
                {
                    for (var i = 0; i <OptionList.Count(); i++)
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
                        var mark = lst.Where(d => d.SubjectId == subject.Value.ConvertToInt() && d.ClassId ==t.Value.ConvertToInt()
                                                                             ).Select(d => d).FirstOrDefault();
                        if (mark != null)
                        {
                            cell = cellHeader.CreateCell(No + 1);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(0, 15 * 256);
                            cell.SetCellValue(mark.StudentCount);
                            No++;
                            cell = cellHeader.CreateCell(No +1);
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

        public ActionResult ExportSegmentMarkTopN(int examId, int gradeId, string chkSubject, string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
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
                              p.SegmentClassRank,
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
                              ClassRank = p.SegmentClassRank,
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

        public ActionResult ExportTotalMarkAvg(int examId, int gradeId, string chkSubject, string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
                //考试科目
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

                if (chkClass == null || chkSubject == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList = chkClass.Split(',');
                var chksubjectList =chkSubject.Split(',');

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

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id ==examId|| examId == 0)
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
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
                              SubjectId = p.SubjectId,
                              TotalMark = p.TotalMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in selectSubjectList)
                {
                    //班级人数
                    var subjectId = o.Value.ConvertToInt();
                    var classAvgList = (from p in tg
                                        where p.SubjectId == subjectId
                                        group p by new
                                        {
                                            p.ClassId,
                                            p.ClassName
                                        } into g
                                        select new
                                        {
                                            g.Key.ClassId,
                                            AvgMark = g.Average(d => d.TotalMark)
                                        }).ToList();

                    var tb = (from p in classAvgList
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = subjectId,
                                  AvgMark =Math.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                  ClassRank = decimal.Zero
                              }).ToList();
                    lst.AddRange(tb);
                }
                //排名
                foreach (var subject in selectSubjectList)
                {
                    var rank = decimal.Zero;
                    decimal? mark = null;
                    var count = decimal.One;
                    foreach (var t in lst.Where(d => d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.AvgMark))
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

                #endregion

                #region 导出
                var OptionList = new List<string>() { "平均分", "多班排名" };
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = SetCellStyle(hssfworkbook);

                var sheetName = db.Table<Exam.Entity.tbExam>().Where(d => d.Id == examId).Select(d => d.ExamName).FirstOrDefault() + "考试成绩平均分";

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
                            cell.SetCellValue(mark.AvgMark.ToString());
                            No++;
                            cell = cellHeader.CreateCell(No + 1);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(0, 15 * 256);
                            cell.SetCellValue(mark.ClassRank.ToString());

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
                    return File(filePath, "application/octet-stream", Server.UrlEncode("考试成绩平均分报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportSegmentMarkAvg(int examId, int gradeId, string chkSubject, string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
                //考试科目
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
                                               SubjectId = p.Id,
                                               p.SubjectName,
                                           }).Distinct().ToList();

                var selectSubjectList = (from p in tbselectSubjectList
                                         select new System.Web.Mvc.SelectListItem
                                         {
                                             Value = p.SubjectId.ToString(),
                                             Text = p.SubjectName,
                                         }).Distinct().ToList();


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

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == examId || examId == 0)
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.SegmentMark,
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
                              TotalMark = p.SegmentMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in selectSubjectList)
                {
                    //班级人数
                    var subjectId = o.Value.ConvertToInt();
                    var classAvgList = (from p in tg
                                        where p.SubjectId == subjectId
                                        group p by new
                                        {
                                            p.ClassId,
                                            p.ClassName
                                        } into g
                                        select new
                                        {
                                            g.Key.ClassId,
                                            AvgMark = g.Average(d => d.TotalMark)
                                        }).ToList();

                    var tb = (from p in classAvgList
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  SubjectId = subjectId,
                                  AvgMark = Math.Round(p.AvgMark.ConvertToDecimal(), 2, MidpointRounding.AwayFromZero),
                                  ClassRank = decimal.Zero
                              }).ToList();
                    lst.AddRange(tb);
                }
                //排名
                foreach (var subject in selectSubjectList)
                {
                    var rank = decimal.Zero;
                    decimal? mark = null;
                    var count = decimal.One;
                    foreach (var t in lst.Where(d => d.SubjectId == subject.Value.ConvertToInt()).OrderByDescending(d => d.AvgMark))
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

                #endregion

                #region 导出
                var OptionList = new List<string>() { "平均分", "多班排名" };
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

                var sheetName = db.Table<Exam.Entity.tbExam>().Where(d => d.Id == examId).Select(d => d.ExamName).FirstOrDefault() + "考试成绩平均分";

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
                            cell.SetCellValue(mark.AvgMark.ToString());
                            No++;
                            cell = cellHeader.CreateCell(No + 1);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(0, 15 * 256);
                            cell.SetCellValue(mark.ClassRank.ToString());

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
                    return File(filePath, "application/octet-stream", Server.UrlEncode("考试成绩平均分报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportSegmentLevel(int examId, int gradeId, int subjectId,string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id ==examId
                                    && p.tbCourse.IsDeleted == false && p.tbCourse.tbSubject.IsDeleted == false
                                    && p.tbExamLevelGroup.IsDeleted == false
                                    && p.tbCourse.tbSubject.Id==subjectId
                                   orderby p.tbCourse.tbSubject.No
                                   select new
                                   {
                                       SubjectName = p.tbCourse.tbSubject.SubjectName,
                                       SubjectId = p.tbCourse.tbSubject.Id,
                                       LevelGroupId = p.tbExamLevelGroup.Id
                                   }).FirstOrDefault();


                var LevelGroupId = examSubjectList.LevelGroupId;

                var examLevelList = (from p in db.Table<Exam.Entity.tbExamLevel>()
                                 where p.tbExamLevelGroup.Id == LevelGroupId
                                 orderby p.No, p.ExamLevelName
                                 select new
                                 {
                                     ExamLevelName = p.ExamLevelName,
                                     LevelId = p.Id
                                 }).ToList();

                var LevelList = (from p in examLevelList
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Text = p.ExamLevelName,
                                    Value = p.LevelId.ToString()
                                }).ToList();

                //学年
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == examId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                //年级学生班级
                var classStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                    where p.tbStudent.IsDeleted == false
                                    && p.tbClass.tbGrade.Id == gradeId
                                    && p.tbClass.IsDeleted == false && p.tbClass.tbYear.Id == yearId
                                    orderby p.tbClass.No
                                    select new
                                    {
                                        ClassId = p.tbClass.Id,
                                        p.tbClass.ClassName,
                                        StudentId = p.tbStudent.Id
                                    }).ToList();
                //班级
                var tbclassStudent = (from p in classStudent
                                      select new
                                      {
                                          p.ClassId,
                                          p.ClassName
                                      }).Distinct().ToList();
                //班级
                var ClassList = (from p in tbclassStudent
                                select new System.Web.Mvc.SelectListItem
                                {
                                    Value = p.ClassId.ToString(),
                                    Text = p.ClassName,
                                }).Distinct().ToList();

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == examId|| examId == 0)
                           && p.tbExamCourse.tbCourse.IsDeleted == false
                           && p.tbExamCourse.tbCourse.tbSubject.Id == subjectId
                           && (p.tbExamLevel.IsDeleted == false || p.tbExamLevel == null)
                          select new Exam.Dto.ExamAnalyze.List
                          {
                              LevelId = p.tbExamLevel.Id,
                              StudentId = p.tbStudent.Id,
                          }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              LevelId = p.LevelId,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in LevelList)
                {
                    //班级等级人数
                    var levelId = o.Value.ConvertToInt();
                    var classTotalCount = (from p in tg
                                           group p by new
                                           {
                                               p.ClassId
                                           } into g
                                           select new
                                           {
                                               g.Key.ClassId,
                                               TotalCount = g.Count()
                                           }).ToList();

                    var tm = (from p in tg
                              where p.LevelId == levelId
                              group p by new
                              {
                                  p.ClassId
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  StudentCount = g.Count(),
                                  TotalCount = classTotalCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tb = (from p in tm
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  LevelId = levelId,
                                  StudentCount = p.StudentCount.ToString(),
                                  Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentCount * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                              }).ToList();
                    lst.AddRange(tb);
                }

                #endregion

                #region 导出
                var OptionList = new List<string>() { "人数", "比率" };
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

                var sheetName = db.Table<Exam.Entity.tbExam>().Where(d => d.Id == examId).Select(d => d.ExamName).FirstOrDefault() + "单科等级";

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
                foreach (var lv in LevelList)
                {
                    //表头
                    cell = cellHeader.CreateCell(OptionList.Count() * No + 1);
                    cell.SetCellValue(lv.Text);
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count() * No + 1, OptionList.Count() * (No + 1));
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    No++;
                }

                //第二行
                cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                No = 0;
                foreach (var subject in LevelList)
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
                foreach (var t in ClassList)
                {
                    cellHeader = sheet1.CreateRow(rowStartIndex + 1);

                    cell = cellHeader.CreateCell(0);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(t.Text);

                    No = 0;
                    foreach (var lv in LevelList)
                    {
                        var mark =lst.Where(d => d.LevelId == lv.Value.ConvertToInt() && d.ClassId == t.Value.ConvertToInt()
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
                    return File(filePath, "application/octet-stream", Server.UrlEncode("单科等级报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportCompreTotalMark(int examId, int gradeId, string chkSubject, string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
                var examSubjectList = (from p in db.Table<Exam.Entity.tbExamCourse>()
                                   where p.tbExam.Id ==examId
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

                if (chkClass == null || chkSubject == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList = chkClass.Split(',');
                var chksubjectList =chkSubject.Split(',');

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id ==gradeId
                                        && p.tbSubject.IsDeleted == false
                                        && chksubjectList.Contains(p.tbSubject.Id.ToString())
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsPass,
                                           SubjectId = p.tbSubject.Id,
                                           p.tbSubject.SubjectName
                                       }).ToList();

                var SegmentList = (from p in SegmentMarkList
                                  select new Exam.Dto.ExamAnalyze.SegmentList
                                  {
                                      SegmentName = p.SegmentName,
                                      SegmentId = p.SegmentId,
                                      SubjectId = p.SubjectId
                                  }).Distinct().ToList();


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

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == examId || examId == 0)
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.TotalMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullTotalMark
                          }).ToList();

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
                              FullTotalMark = p.FullTotalMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                var lstTotal = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;

                    //优秀科目人数
                    if (isGood)
                    {
                        //分数段人数
                        var tm = (from p in tg
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                      p.ClassName
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      g.Key.ClassName,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      ClassName = p.ClassName,
                                      SubjectId = o.SubjectId,
                                      SubjectName = o.SubjectName,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                    if (isPass)//及格人数
                    {
                        //分数段人数
                        var tm = (from p in tg
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark  && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                      p.ClassName
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      g.Key.ClassName,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      ClassName = p.ClassName,
                                      SubjectId = o.SubjectId,
                                      SubjectName = o.SubjectName,
                                      Status = 2,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }
                //统计人数
                foreach (var subject in selectSubjectList)
                {
                    //班级科目人数
                    var totalStudetCount = (from p in tg
                                            where p.SubjectId == subject.Value.ConvertToInt()
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                TotalCount = g.Count()
                                            }).ToList();
                    var tt = (from p in lst
                              where p.SubjectId == subject.Value.ConvertToInt()
                              group p by new
                              {
                                  p.ClassId,
                                  p.ClassName,
                                  p.Status,
                                  p.SubjectId,
                                  p.SubjectName
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  g.Key.ClassName,
                                  g.Key.Status,
                                  g.Key.SubjectId,
                                  g.Key.SubjectName,
                                  StudentNum = g.Sum(d => d.StudentNum),
                                  TotalCount = totalStudetCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tk = (from p in tt
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  ClassName = p.ClassName,
                                  SubjectId = p.SubjectId,
                                  SubjectName = p.SubjectName,
                                  Status = p.Status,
                                  StudentNum = p.StudentNum,
                                  ClassRank = 0,
                                  Percent = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentNum * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero)) : decimal.Zero,
                                  Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentNum * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                              }).ToList();

                    lstTotal.AddRange(tk);
                }
                var CompreList = new List<Dto.ExamAnalyze.List>();
                //分组班级科目
                foreach (var c in selctClassList)
                {
                    foreach (var s in selectSubjectList)
                    {
                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.ClassId = c.Value.ConvertToInt();
                        model.ClassName = c.Text;
                        model.SubjectId = s.Value.ConvertToInt();
                        model.SubjectName = s.Text;
                        CompreList.Add(model);
                    }
                }

                //优秀率排名
                var rank = decimal.Zero;
                decimal? mark = null;
                var count = decimal.One;
                foreach (var t in lstTotal.Where(d => d.Status == decimal.One).OrderByDescending(d => d.Percent))
                {
                    if (mark != t.Percent)
                    {
                        mark = t.Percent;
                        rank = rank + count;
                        count = decimal.One;
                    }
                    else
                    {
                        count = count + decimal.One;
                    }

                    t.ClassRank = rank;
                }

                rank = decimal.Zero;
                mark = null;
                count = decimal.One;
                foreach (var t in lstTotal.Where(d => d.Status == 2).OrderByDescending(d => d.Percent))
                {
                    if (mark != t.Percent)
                    {
                        mark = t.Percent;
                        rank = rank + count;
                        count = decimal.One;
                    }
                    else
                    {
                        count = count + decimal.One;
                    }

                    t.ClassRank = rank;
                }

                #endregion

                #region 导出
                var GoodPassList = new List<string>() { "优秀", "及格" };
                var OptionList = new List<string>() { "人数", "比率", "排名" };
               
                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = SetCellStyle(hssfworkbook);

                var sheetName = db.Table<Exam.Entity.tbExam>().Where(d => d.Id == examId).Select(d => d.ExamName).FirstOrDefault() + "考试成绩综合分析";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                //表头第一行
                ICell cell = cellHeader.CreateCell(0);
                cell.SetCellValue("班级");
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 0, 0);
                sheet1.AddMergedRegion(cellRangeAddress);
                cell.CellStyle = cellstyle;

                cell.SetCellValue("科目");
                cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 1, 1);
                sheet1.AddMergedRegion(cellRangeAddress);
                cell.CellStyle = cellstyle;

                var No = 0;
                for (var i = 0; i <GoodPassList.Count(); i++)
                {
                    //表头
                    cell = cellHeader.CreateCell(OptionList.Count() * No + 2);
                    cell.SetCellValue(GoodPassList[i]);
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count() * No + 2, OptionList.Count() * (No + 1)+1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    No++;
                }

                //第二行
                cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                No = 0;
                for (var i = 0; i <GoodPassList.Count(); i++)
                {
                    for (var j = 0; j <OptionList.Count(); j++)
                    {
                        cell = cellHeader.CreateCell(No + 2);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(OptionList[j]);
                        No++;
                    }
                }

                //跨行边框无显示
                cellHeader.CreateCell(0).SetCellValue(string.Empty);
                cellHeader.GetCell(0).CellStyle = cellstyle;
                cellHeader.CreateCell(1).SetCellValue(string.Empty);
                cellHeader.GetCell(1).CellStyle = cellstyle;

                rowStartIndex++;

                //数据行
                foreach (var a in CompreList)
                {
                    cellHeader = sheet1.CreateRow(rowStartIndex + 1);

                    cell = cellHeader.CreateCell(0);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(a.ClassName);

                    cell = cellHeader.CreateCell(1);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(1, 15 * 256);
                    cell.SetCellValue(a.SubjectName);

                    No = 0;
                    for (var status = 1; status <= GoodPassList.Count(); status++)
                    {
                        var examMark = lstTotal.Where(d => d.SubjectId == a.SubjectId && d.ClassId == a.ClassId
                                                                              && d.Status == status).Select(d => d).FirstOrDefault();
                        if (examMark != null)
                        {
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(examMark.StudentNum);
                            No++;
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(examMark.Rate);
                            No++;
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(examMark.ClassRank.ToString());
                            No++;
                        }
                        else
                        {
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(string.Empty);
                            No++;
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(string.Empty);
                            No++;
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(string.Empty);
                            No++;
                        }
                    }

                    rowStartIndex++;
                }
                this.MergeColumn(hssfworkbook, 0, 2);
                //解决合并后覆盖
                sheet1.GetRow(0).CreateCell(0).SetCellValue("班级");
                sheet1.GetRow(0).GetCell(0).CellStyle = cellstyle;
                sheet1.GetRow(0).CreateCell(1).SetCellValue("科目");
                sheet1.GetRow(0).GetCell(1).CellStyle = cellstyle;

                //合并数据第一行格式重新创建
                var oldvalue = sheet1.GetRow(2).GetCell(0).StringCellValue;
                sheet1.GetRow(2).CreateCell(0).SetCellValue(oldvalue);
                cellRangeAddress = new CellRangeAddress(2, 1+selectSubjectList.Count(), 0, 0);
                sheet1.AddMergedRegion(cellRangeAddress);
                sheet1.GetRow(2).GetCell(0).CellStyle = cellstyle;

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("考试成绩综合成绩分析报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
            }
        }

        public ActionResult ExportCompreSegmentMark(int examId, int gradeId, string chkSubject, string chkClass, string searchText)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var filePath = System.IO.Path.GetTempFileName();

                #region 数据统计
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

                if (chkClass == null || chkSubject == null) return Content("<script>alert('暂无数据!');history.go(-1);</script>");
                var chkclassList = chkClass.Split(',');
                var chksubjectList = chkSubject.Split(',');

                //科目分数段
                var SegmentMarkList = (from p in db.Table<Exam.Entity.tbExamSegmentMark>()
                                       where p.tbGrade.Id == gradeId
                                        && p.tbSubject.IsDeleted == false
                                        && chksubjectList.Contains(p.tbSubject.Id.ToString())
                                       select new
                                       {
                                           SegmentId = p.Id,
                                           p.SegmentName,
                                           p.MinMark,
                                           p.MaxMark,
                                           p.IsGood,
                                           p.IsPass,
                                           SubjectId = p.tbSubject.Id,
                                           p.tbSubject.SubjectName
                                       }).ToList();

                var SegmentList = (from p in SegmentMarkList
                                   select new Exam.Dto.ExamAnalyze.SegmentList
                                   {
                                       SegmentName = p.SegmentName,
                                       SegmentId = p.SegmentId,
                                       SubjectId = p.SubjectId
                                   }).Distinct().ToList();


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

                var tf = (from p in db.Table<Exam.Entity.tbExamMark>()
                          where p.tbExamCourse.IsDeleted == false
                           && p.tbStudent.IsDeleted == false
                           && (p.tbExamCourse.tbExam.Id == examId || examId == 0)
                           && chksubjectList.Contains(p.tbExamCourse.tbCourse.tbSubject.Id.ToString())
                          select new
                          {
                              p.SegmentMark,
                              StudentId = p.tbStudent.Id,
                              SubjectId = p.tbExamCourse.tbCourse.tbSubject.Id,
                              p.tbExamCourse.FullTotalMark
                          }).ToList();

                var tg = (from p in tf
                          join t in classStudent
                          on p.StudentId equals t.StudentId
                          select new
                          {
                              StudentId = p.StudentId,
                              SubjectId = p.SubjectId,
                              TotalMark = p.SegmentMark,
                              ClassId = t.ClassId,
                              ClassName = t.ClassName,
                              FullTotalMark = p.FullTotalMark
                          }).ToList();
                var lst = new List<Exam.Dto.ExamAnalyze.List>();
                var lstTotal = new List<Exam.Dto.ExamAnalyze.List>();
                foreach (var o in SegmentMarkList)
                {
                    var isGood = o.IsGood;
                    var isPass = o.IsPass;

                    //优秀科目人数
                    if (isGood)
                    {
                        //分数段人数
                        var tm = (from p in tg
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                      p.ClassName
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      g.Key.ClassName,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      ClassName = p.ClassName,
                                      SubjectId = o.SubjectId,
                                      SubjectName = o.SubjectName,
                                      Status = decimal.One,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                    if (isPass)//及格人数
                    {
                        //分数段人数
                        var tm = (from p in tg
                                  where p.SubjectId == o.SubjectId
                                  && p.TotalMark >= o.MinMark && p.TotalMark <= o.MaxMark
                                  group p by new
                                  {
                                      p.ClassId,
                                      p.ClassName
                                  } into g
                                  select new
                                  {
                                      g.Key.ClassId,
                                      g.Key.ClassName,
                                      StudentCount = g.Count(),
                                  }).ToList();

                        var tb = (from p in tm
                                  select new Exam.Dto.ExamAnalyze.List
                                  {
                                      ClassId = p.ClassId,
                                      ClassName = p.ClassName,
                                      SubjectId = o.SubjectId,
                                      SubjectName = o.SubjectName,
                                      Status = 2,
                                      StudentNum = p.StudentCount,
                                  }).ToList();
                        lst.AddRange(tb);
                    }
                }
                //统计人数
                foreach (var subject in selectSubjectList)
                {
                    //班级科目人数
                    var totalStudetCount = (from p in tg
                                            where p.SubjectId == subject.Value.ConvertToInt()
                                            group p by new
                                            {
                                                p.ClassId
                                            } into g
                                            select new
                                            {
                                                g.Key.ClassId,
                                                TotalCount = g.Count()
                                            }).ToList();
                    var tt = (from p in lst
                              where p.SubjectId == subject.Value.ConvertToInt()
                              group p by new
                              {
                                  p.ClassId,
                                  p.ClassName,
                                  p.Status,
                                  p.SubjectId,
                                  p.SubjectName
                              } into g
                              select new
                              {
                                  g.Key.ClassId,
                                  g.Key.ClassName,
                                  g.Key.Status,
                                  g.Key.SubjectId,
                                  g.Key.SubjectName,
                                  StudentNum = g.Sum(d => d.StudentNum),
                                  TotalCount = totalStudetCount.Where(c => c.ClassId == g.Key.ClassId).Select(c => c.TotalCount).FirstOrDefault()
                              }).ToList();

                    var tk = (from p in tt
                              select new Exam.Dto.ExamAnalyze.List
                              {
                                  ClassId = p.ClassId,
                                  ClassName = p.ClassName,
                                  SubjectId = p.SubjectId,
                                  SubjectName = p.SubjectName,
                                  Status = p.Status,
                                  StudentNum = p.StudentNum,
                                  ClassRank = 0,
                                  Percent = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentNum * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero)) : decimal.Zero,
                                  Rate = p.TotalCount > decimal.Zero ? (decimal.Round((decimal)p.StudentNum * 100 / p.TotalCount, 2, MidpointRounding.AwayFromZero) + "%") : string.Empty
                              }).ToList();

                    lstTotal.AddRange(tk);
                }
                var CompreList = new List<Dto.ExamAnalyze.List>();
                //分组班级科目
                foreach (var c in selctClassList)
                {
                    foreach (var s in selectSubjectList)
                    {
                        var model = new Exam.Dto.ExamAnalyze.List();
                        model.ClassId = c.Value.ConvertToInt();
                        model.ClassName = c.Text;
                        model.SubjectId = s.Value.ConvertToInt();
                        model.SubjectName = s.Text;
                        CompreList.Add(model);
                    }
                }

                //优秀率排名
                var rank = decimal.Zero;
                decimal? mark = null;
                var count = decimal.One;
                foreach (var t in lstTotal.Where(d => d.Status == decimal.One).OrderByDescending(d => d.Percent))
                {
                    if (mark != t.Percent)
                    {
                        mark = t.Percent;
                        rank = rank + count;
                        count = decimal.One;
                    }
                    else
                    {
                        count = count + decimal.One;
                    }

                    t.ClassRank = rank;
                }

                rank = decimal.Zero;
                mark = null;
                count = decimal.One;
                foreach (var t in lstTotal.Where(d => d.Status == 2).OrderByDescending(d => d.Percent))
                {
                    if (mark != t.Percent)
                    {
                        mark = t.Percent;
                        rank = rank + count;
                        count = decimal.One;
                    }
                    else
                    {
                        count = count + decimal.One;
                    }

                    t.ClassRank = rank;
                }

                #endregion

                #region 导出
                var GoodPassList = new List<string>() { "优秀", "及格" };
                var OptionList = new List<string>() { "人数", "比率", "排名" };

                HSSFWorkbook hssfworkbook = new HSSFWorkbook();
                ICellStyle cellstyle = SetCellStyle(hssfworkbook);

                //缩小字体填充  
                cellstyle.ShrinkToFit = false;

                var sheetName = db.Table<Exam.Entity.tbExam>().Where(d => d.Id == examId).Select(d => d.ExamName).FirstOrDefault() + "综合成绩综合分析";

                HSSFSheet sheet1 = hssfworkbook.CreateSheet(sheetName) as HSSFSheet;//建立Sheet1
                var rowStartIndex = 0;
                IRow cellHeader = sheet1.CreateRow(rowStartIndex);
                //表头第一行
                ICell cell = cellHeader.CreateCell(0);
                cell.SetCellValue("班级");
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 0, 0);
                sheet1.AddMergedRegion(cellRangeAddress);
                cell.CellStyle = cellstyle;

                cell.SetCellValue("科目");
                cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex + 1, 1, 1);
                sheet1.AddMergedRegion(cellRangeAddress);
                cell.CellStyle = cellstyle;

                var No = 0;
                for (var i = 0; i < GoodPassList.Count(); i++)
                {
                    //表头
                    cell = cellHeader.CreateCell(OptionList.Count() * No + 2);
                    cell.SetCellValue(GoodPassList[i]);
                    cellRangeAddress = new CellRangeAddress(rowStartIndex, rowStartIndex, OptionList.Count() * No + 2, OptionList.Count() * (No + 1) + 1);
                    setRegionStyle(sheet1, cellRangeAddress, cellstyle);
                    sheet1.AddMergedRegion(cellRangeAddress);
                    No++;
                }

                //第二行
                cellHeader = sheet1.CreateRow(rowStartIndex + 1);
                No = 0;
                for (var i = 0; i < GoodPassList.Count(); i++)
                {
                    for (var j = 0; j < OptionList.Count(); j++)
                    {
                        cell = cellHeader.CreateCell(No + 2);
                        cell.CellStyle = cellstyle;
                        cell.SetCellValue(OptionList[j]);
                        No++;
                    }
                }
                //跨行边框无显示
                cellHeader.CreateCell(0).SetCellValue(string.Empty);
                cellHeader.GetCell(0).CellStyle = cellstyle;
                cellHeader.CreateCell(1).SetCellValue(string.Empty);
                cellHeader.GetCell(1).CellStyle = cellstyle;

                rowStartIndex++;

                //数据行
                foreach (var a in CompreList)
                {
                    cellHeader = sheet1.CreateRow(rowStartIndex + 1);

                    cell = cellHeader.CreateCell(0);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(0, 15 * 256);
                    cell.SetCellValue(a.ClassName);

                    cell = cellHeader.CreateCell(1);
                    cell.CellStyle = cellstyle;
                    sheet1.SetColumnWidth(1, 15 * 256);
                    cell.SetCellValue(a.SubjectName);

                    No = 0;
                    for (var status = 1; status <= GoodPassList.Count(); status++)
                    {
                        var examMark = lstTotal.Where(d => d.SubjectId == a.SubjectId && d.ClassId == a.ClassId
                                                                              && d.Status == status).Select(d => d).FirstOrDefault();
                        if (examMark != null)
                        {
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(examMark.StudentNum);
                            No++;
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(examMark.Rate);
                            No++;
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(examMark.ClassRank.ToString());
                            No++;
                        }
                        else
                        {
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(string.Empty);
                            No++;
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(string.Empty);
                            No++;
                            cell = cellHeader.CreateCell(No + 2);
                            cell.CellStyle = cellstyle;
                            sheet1.SetColumnWidth(No + 2, 15 * 256);
                            cell.SetCellValue(string.Empty);
                            No++;
                        }
                    }

                    rowStartIndex++;
                }
                this.MergeColumn(hssfworkbook, 0, 2);
                sheet1.GetRow(0).CreateCell(0).SetCellValue("班级");
                sheet1.GetRow(0).GetCell(0).CellStyle = cellstyle;
                sheet1.GetRow(0).CreateCell(1).SetCellValue("科目");
                sheet1.GetRow(0).GetCell(1).CellStyle = cellstyle;

                //合并数据第一行格式重新创建
                var oldvalue = sheet1.GetRow(2).GetCell(0).StringCellValue;
                sheet1.GetRow(2).CreateCell(0).SetCellValue(oldvalue);
                cellRangeAddress = new CellRangeAddress(2, 1 + selectSubjectList.Count(), 0, 0);
                sheet1.AddMergedRegion(cellRangeAddress);
                sheet1.GetRow(2).GetCell(0).CellStyle = cellstyle;

                var fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                hssfworkbook.Write(fs);
                fs.Close();

                if (string.IsNullOrEmpty(filePath) == false)
                {
                    return File(filePath, "application/octet-stream", Server.UrlEncode("考试成绩综合成绩分析报表" + DateTime.Now.ToString("yyyyMMdd") + ".xls"));

                }
                else
                {
                    return View();
                }

                #endregion
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
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="book"></param>
        /// <param name="cellIndex">需要合并的单元索引</param>
        /// <param name="rowIndex">需合并单元格的起始行，前两行为标题，默认起始行是第三行</param>
        public void MergeColumn(HSSFWorkbook book,int cellIndex,int rowIndex)
        {
            ISheet sheet = book.GetSheetAt(0);
            int cunit = cellIndex;
            // 合并单元格跨越的行数
            int rspan = 0;
            // 需合并单元格的起始行，前两行为标题，默认起始行是第三行
            int srow = rowIndex;

            // 合并单元格
            string oldvalue = string.Empty;
            for (int r =2; r < sheet.PhysicalNumberOfRows; r++)
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

        public int  getYear(int examId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
                var yearId = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == examId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();
                return yearId;
            }
        }
       
        public void getSubjectClassList(int examId,int? gradeId, string searchText,out List<System.Web.Mvc.SelectListItem> classList, out List<System.Web.Mvc.SelectListItem> subjectList,out int yearId)
        {
            using (var db = new XkSystem.Models.DbContext())
            {
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

                subjectList = (from p in examSubjectList
                          select new System.Web.Mvc.SelectListItem
                          {
                              Text = p.SubjectName,
                              Value = p.SubjectId.ToString()
                          }).ToList();

                var  year = (from p in db.Table<Exam.Entity.tbExam>()
                              where p.Id == examId
                              && p.tbYear.IsDeleted == false
                              select p.tbYear.tbYearParent.tbYearParent.Id).FirstOrDefault();

                yearId = year;

                var tbClassStudent = (from p in db.Table<Basis.Entity.tbClassStudent>()
                                      where p.tbStudent.IsDeleted == false
                                      && p.tbClass.tbGrade.Id ==gradeId
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
    }
}