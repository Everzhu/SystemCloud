using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class MoralStudentList
    {
        public List<Dto.MoralComment.MoralStudentList> MStudentList { get; set; } = new List<Dto.MoralComment.MoralStudentList>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public bool YearDefault = false;

        public string Week { get; set; } = System.Web.HttpContext.Current.Request["Week"] != null ? System.Web.HttpContext.Current.Request["Week"].ToString() : string.Empty;
    }
}