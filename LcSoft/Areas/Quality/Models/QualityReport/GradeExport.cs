using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityReport
{
    public class GradeExport
    {
        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public List<Dto.QualityReport.GradeExport> ClassList { get; set; } = new List<Dto.QualityReport.GradeExport>();

        public List<Dto.QualityReport.GradeExport> ClassExamMarkList { get; set; } = new List<Dto.QualityReport.GradeExport>();

        public int? ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();

        public int? YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();
    }
}