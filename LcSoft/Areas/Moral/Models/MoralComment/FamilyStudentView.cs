using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class FamilyStudentView
    {
        public Dto.MoralComment.FamilyStudentView FStudentView { get; set; } = new Dto.MoralComment.FamilyStudentView();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public int YearTermId { get; set; } = System.Web.HttpContext.Current.Request["YearTermId"].ConvertToInt();
    }
}