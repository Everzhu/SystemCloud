using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassAllotResult
{
    public class Export
    {
        public List<Dto.ClassAllotResult.Export> ClassAllotResultExport { get; set; } = new List<Dto.ClassAllotResult.Export>();

        public int? ClassAllotClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassAllotClassId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}