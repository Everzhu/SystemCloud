using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class MoralStudentEdit
    {
        public Dto.MoralComment.MoralStudentEdit MStudentEdit { get; set; } = new Dto.MoralComment.MoralStudentEdit();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public string Week { get; set; } = System.Web.HttpContext.Current.Request["Week"].ToString();

        public string StudentCode = string.Empty;

        public string StudentName = string.Empty;
    }
}