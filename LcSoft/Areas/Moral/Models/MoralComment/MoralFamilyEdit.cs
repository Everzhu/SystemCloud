using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class MoralFamilyEdit
    {
        public Dto.MoralComment.MoralFamilyEdit MFamilyEdit { get; set; } = new Dto.MoralComment.MoralFamilyEdit();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public string StudentCode = string.Empty;

        public string StudentName = string.Empty;
    }
}