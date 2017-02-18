using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamImportSegmentMark
{
    public class List
    {
        public List<Dto.ExamImportSegmentMark.List> ExamImportSegmentMarkList { get; set; } = new List<Dto.ExamImportSegmentMark.List>();

        public string SearchText { get; set; } = Convert.ToString(HttpContext.Current.Request["SearchText"]);

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}