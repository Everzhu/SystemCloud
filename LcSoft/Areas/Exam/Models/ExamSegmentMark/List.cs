using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamSegmentMark
{
    public class List
    {
        public List<Dto.ExamSegmentMark.List> ExamSegmentMarkList { get; set; } = new List<Dto.ExamSegmentMark.List>();

        public string SearchText { get; set; } = Convert.ToString(HttpContext.Current.Request["SearchText"]);

        public List<System.Web.Mvc.SelectListItem> SegmentGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int? SubjectId { get; set; }= HttpContext.Current.Request["SubjectId"].ConvertToInt();

        public int? SegmentGroupId { get; set; } = HttpContext.Current.Request["SegmentGroupId"].ConvertToInt();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}