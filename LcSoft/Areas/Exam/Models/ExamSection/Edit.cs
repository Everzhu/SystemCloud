using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamSection
{
    public class Edit
    {
        public Dto.ExamSection.Edit ExamSectionEdit { get; set; } = new Dto.ExamSection.Edit();

        public List<System.Web.Mvc.SelectListItem>  GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}