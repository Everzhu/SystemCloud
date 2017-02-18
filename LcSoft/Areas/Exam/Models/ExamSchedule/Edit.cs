using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamSchedule
{
    public class Edit
    {
        public Dto.ExamSchedule.Edit ExamScheduleEdit { get; set; } = new Dto.ExamSchedule.Edit();

        public List<System.Web.Mvc.SelectListItem> ExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}