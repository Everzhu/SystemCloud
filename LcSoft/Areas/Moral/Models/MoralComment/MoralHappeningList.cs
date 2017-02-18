using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class MoralHappeningList
    {
        public List<Dto.MoralComment.MoralHappeningList> MHappeningList { get; set; } = new List<Dto.MoralComment.MoralHappeningList>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public int HappeningId { get; set; } = System.Web.HttpContext.Current.Request["HappeningId"].ConvertToInt();

        public bool YearDefault = false;

        public string InputDate { get; set; } = System.Web.HttpContext.Current.Request["InputDate"] != null ? System.Web.HttpContext.Current.Request["InputDate"].ToString() : string.Empty;

        public List<Dto.MoralComment.HappeningStudentView> HStudentViewList { get; set; } = new List<Dto.MoralComment.HappeningStudentView>();
    }
}