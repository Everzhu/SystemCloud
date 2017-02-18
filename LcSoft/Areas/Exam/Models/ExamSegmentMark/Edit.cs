using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamSegmentMark
{
    public class Edit
    {
        public Dto.ExamSegmentMark.Edit  ExamSegmentMarkEdit { get; set; } = new Dto.ExamSegmentMark.Edit();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SegmentGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

    }
}