using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityReport
{
    public class GradeReport
    {
        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public List<Dto.QualityReport.GradeReport> ClassList { get; set; } = new List<Dto.QualityReport.GradeReport>();

        public List<Dto.QualityReport.GradeReport> ClassExamMarkList { get; set; } = new List<Dto.QualityReport.GradeReport>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();
    }
}