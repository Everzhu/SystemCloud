using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.Quality
{
    public class Edit
    {
        public Dto.Quality.Edit QualityEdit { get; set; } = new Dto.Quality.Edit();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public List<System.Web.Mvc.SelectListItem> QualityList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string CreateWay { get; set; }

        public int CopyQualityId { get; set; }
    }
}