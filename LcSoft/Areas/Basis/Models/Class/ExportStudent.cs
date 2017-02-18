using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.Class
{
    public class ExportStudent
    {
        public List<Dto.Class.ExportStudent> ExportStudentList { get; set; } = new List<Dto.Class.ExportStudent>();

        public int? YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public int? ClassTypeId { get; set; } = System.Web.HttpContext.Current.Request["ClassTypeId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}