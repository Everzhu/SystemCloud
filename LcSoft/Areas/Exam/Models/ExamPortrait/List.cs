using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamPortrait
{
    public class List
    {
        public List<Dto.ExamPortrait.List> ExamPortraitList { get; set; } = new List<Dto.ExamPortrait.List>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamIds { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassIds { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectIds { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();

        public int? SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();

        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public string chkExam { get; set; } = System.Web.HttpContext.Current.Request["chkExam"];

        public string chkSubject { get; set; } = System.Web.HttpContext.Current.Request["chkSubject"];

        public string chkClass { get; set; } = System.Web.HttpContext.Current.Request["chkClass"];

        public string ReportScore { get; set; }

        public string ReportScoreClass { get; set; }

        public string ReportScoreGrade { get; set; }

        public string ReportAvgClass { get; set; }

        public string ReportAvgClassRank { get; set; }

        public decimal goodCount { get; set; }
        public decimal passCount { get; set; }
    }
}