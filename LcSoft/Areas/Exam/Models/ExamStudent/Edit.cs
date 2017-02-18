using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamStudent
{
    public class Edit
    {
        public Dto.ExamStudent.Edit ExamStudentEdit { get; set; } = new Dto.ExamStudent.Edit();

        public List<System.Web.Mvc.SelectListItem> StudentList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}