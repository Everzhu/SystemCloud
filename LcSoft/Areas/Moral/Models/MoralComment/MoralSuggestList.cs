using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class MoralSuggestList
    {
        public List<Dto.MoralComment.MoralSuggestList> MSuggestList { get; set; } = new List<Dto.MoralComment.MoralSuggestList>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public bool YearDefault = false;

        public string InputDate { get; set; } = System.Web.HttpContext.Current.Request["InputDate"] != null ? System.Web.HttpContext.Current.Request["InputDate"].ToString() : string.Empty;

        public int SuggestId { get; set; } = System.Web.HttpContext.Current.Request["SuggestId"].ConvertToInt();

        public List<Dto.MoralComment.SuggestStudentView> SStudentViewList { get; set; } = new List<Dto.MoralComment.SuggestStudentView>();
    }
}