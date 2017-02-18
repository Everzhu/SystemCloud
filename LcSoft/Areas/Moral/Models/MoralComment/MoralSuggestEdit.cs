using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class MoralSuggestEdit
    {
        public Dto.MoralComment.MoralSuggestEdit MSuggestEdit { get; set; } = new Dto.MoralComment.MoralSuggestEdit();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public string InputDate { get; set; } = System.Web.HttpContext.Current.Request["InputDate"].ToString();

        public string StudentCode = string.Empty;

        public string StudentName = string.Empty;
    }
}