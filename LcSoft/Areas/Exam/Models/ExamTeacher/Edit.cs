using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamTeacher
{
    public class Edit
    {
        public Dto.ExamTeacher.Edit  ExamTeacherEdit { get; set; } = new Dto.ExamTeacher.Edit();

        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}