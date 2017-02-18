using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.Study
{
    public class Export
    {
        public List<Dto.Study.Export> ExportList { get; set; } = new List<Dto.Study.Export>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int? YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();
        public int? IsRoomId { get; set; } = System.Web.HttpContext.Current.Request["IsRoomId"].ConvertToInt();
    }
}