using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class MoralTeacherList
    {
        public List<Dto.MoralComment.MoralClassList> MTeacherList { get; set; } = new List<Dto.MoralComment.MoralClassList>();

        public List<SelectListItem> YearList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public string Week { get; set; } = System.Web.HttpContext.Current.Request["Week"] != null ? System.Web.HttpContext.Current.Request["Week"].ToString() : string.Empty;
    }
}