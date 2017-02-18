using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamReport
{
    public class List
    {
        public List<Dto.ExamReport.List> ExamMarkList { get; set; } = new List<Dto.ExamReport.List>();

        public List<Dto.ExamReport.List> ExamTotalMarkList { get; set; } = new List<Dto.ExamReport.List>();

        public List<Dto.ExamReport.List> ClassStudentList { get; set; } = new List<Dto.ExamReport.List>();

        public List<System.Web.Mvc.SelectListItem> ExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> OrgList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamStatusList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamLevelList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> OptionEnChList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Dto.ExamReport.SubjectList> SectionSubjectList { get; set; } = new List<Dto.ExamReport.SubjectList>();

        public List<Perform.Dto.PerformComment.Info> CommentList { get; set; } = new List<Perform.Dto.PerformComment.Info>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();

        public int? CourseId { get; set; } = System.Web.HttpContext.Current.Request["CourseId"].ConvertToInt();

        public int? SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();

        public int? OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();

        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public int? GraduationTypeId { get; set; } = System.Web.HttpContext.Current.Request["GraduationTypeId"].ConvertToInt();

        public string DateSearchFrom { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchFrom"]);

        public List<string> OptionList { get; set; }

        public List<string> GradeOptionList { get; set; }

        public string SubjectName { get; set; }

        public decimal  KindType { get; set; }

        public string ExamName { get; set; }

        public string ReportScoreGrade { get; set; }

        public int? YearId { get; set; }=System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}