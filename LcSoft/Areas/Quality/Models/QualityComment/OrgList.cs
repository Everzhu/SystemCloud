using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityComment
{
    public class OrgList
    {
        public List<Dto.QualityComment.OrgList> QualityRemarkList { get; set; } = new List<Dto.QualityComment.OrgList>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YOrgList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();
    }
}