using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityComment
{
    public class ClassList
    {
        public List<Dto.QualityComment.ClassList> QualityCommentList { get; set; } = new List<Dto.QualityComment.ClassList>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();
    }
}