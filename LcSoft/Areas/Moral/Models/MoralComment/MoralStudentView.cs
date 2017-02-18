using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class MoralStudentView
    {
        public Dto.MoralComment.MoralStudentView MStudentView { get; set; } = new Dto.MoralComment.MoralStudentView();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public string Week { get; set; } = System.Web.HttpContext.Current.Request["Week"].ToString();
    }
}