using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamImportSegmentMark
{
    public class Edit
    {
        public Dto.ExamImportSegmentMark.Edit ExamImportSegmentMarkEdit { get; set; } = new Dto.ExamImportSegmentMark.Edit();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

    }
}