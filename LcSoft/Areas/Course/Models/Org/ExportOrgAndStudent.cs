using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Org
{
    public class ExportOrgAndStudent
    {
        public List<Dto.Org.ExportOrgAndStudent> DataList { get; set; } = new List<Dto.Org.ExportOrgAndStudent>();

        public int? YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? GradeId { get; set; } = HttpContext.Current.Request["GradeId"].ConvertToInt();

        public int? SubjectId { get; set; } = HttpContext.Current.Request["SubjectId"].ConvertToInt();

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}