using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class MoralClassEdit
    {
        public Dto.MoralComment.MoralClassEdit MClassEdit { get; set; } = new Dto.MoralComment.MoralClassEdit();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public string InputMonth { get; set; }=System.Web.HttpContext.Current.Request["InputMonth"].ToString();

        public string StudentCode = string.Empty;

        public string StudentName = string.Empty;
    }
}