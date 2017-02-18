using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class HappeningStudentView
    {
        public Dto.MoralComment.HappeningStudentView HStudentView { get; set; } = new Dto.MoralComment.HappeningStudentView();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public int HappeningId { get; set; } = System.Web.HttpContext.Current.Request["HappeningId"].ConvertToInt();

        public string InputDate { get; set; } = System.Web.HttpContext.Current.Request["InputDate"].ToString();

        public List<Dto.MoralComment.HappeningStudentView> HStudentViewList { get; set; } = new List<Dto.MoralComment.HappeningStudentView>();

        public string StudentCode = string.Empty;

        public string StudentName = string.Empty;
    }
}