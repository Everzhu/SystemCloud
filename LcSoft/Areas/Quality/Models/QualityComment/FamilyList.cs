using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityComment
{
    public class FamilyList
    {
        public List<Dto.QualityComment.FamilyList> QualityFamilyList { get; set; } = new List<Dto.QualityComment.FamilyList>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public bool YearDefault = false;
    }
}