using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassAllotResult
{
    public class List
    {
        public List<Dto.ClassAllotResult.List> ClassAllotResultList { get; set; } = new List<Dto.ClassAllotResult.List>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassAllotClassList = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int YearId { get; set; }

        public int? ClassAllotClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassAllotClassId"].ConvertToInt();
    }
}