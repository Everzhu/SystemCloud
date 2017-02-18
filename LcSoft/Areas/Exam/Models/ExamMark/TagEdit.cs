using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamMark
{
    public class TagEdit
    {
        public Dto.ExamMark.TagEdit ExamTagEdit { get; set; } = new Dto.ExamMark.TagEdit();

        public List<System.Web.Mvc.SelectListItem> ExamCourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamStatusList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int ExamId { get; set; } = HttpContext.Current.Request["ExamId"].ConvertToInt();
    }
}