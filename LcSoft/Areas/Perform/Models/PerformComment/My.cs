using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformComment
{
    public class My
    {
        public Dto.PerformComment.Info PerformCommentInfo { get; set; } = new Dto.PerformComment.Info();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = HttpContext.Current.Request["YearId"].ConvertToInt();
    }
}