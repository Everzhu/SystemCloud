using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class MoralFamilyList
    {
        public List<Dto.MoralComment.MoralFamilyList> MFamilyList { get; set; } = new List<Dto.MoralComment.MoralFamilyList>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public bool YearDefault = false;
    }
}