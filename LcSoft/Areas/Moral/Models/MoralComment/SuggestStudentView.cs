using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralComment
{
    public class SuggestStudentView
    {
        public Dto.MoralComment.SuggestStudentView SStudentView { get; set; } = new Dto.MoralComment.SuggestStudentView();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public int SuggestId { get; set; } = System.Web.HttpContext.Current.Request["SuggestId"].ConvertToInt();

        public string InputDate { get; set; } = System.Web.HttpContext.Current.Request["InputDate"].ToString();

        public List<Dto.MoralComment.HappeningStudentView> SStudentViewList { get; set; } = new List<Dto.MoralComment.HappeningStudentView>();

        public string StudentCode = string.Empty;

        public string StudentName = string.Empty;

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}