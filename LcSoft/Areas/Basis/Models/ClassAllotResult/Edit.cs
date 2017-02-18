using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassAllotResult
{
    public class Edit
    {
        public Dto.ClassAllotResult.Edit ClassAllotResultEdit { get; set; } = new Dto.ClassAllotResult.Edit();

        public List<System.Web.Mvc.SelectListItem> ClassAllotClassList = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();
    }
}