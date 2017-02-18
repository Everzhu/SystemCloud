using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamCourse
{
    public class Edit
    {
        public Dto.ExamCourse.Edit ExamCourseEdit { get; set; } = new Dto.ExamCourse.Edit();

        public List<System.Web.Mvc.SelectListItem> ExamLevelGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamSectionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public bool IsInputType { get; set; }

        public List<int> TeacherIdS { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? YearId { get; set; }

        public int? SubjectId { get; set; }

        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["examId"].ConvertToInt();
    }
}