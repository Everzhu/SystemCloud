using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualitySelf
{
    public class Input
    {
        public List<Dto.QualitySelf.Input> QualitySelfList { get; set; } = new List<Dto.QualitySelf.Input>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public bool YearDefault = false;
    }
}