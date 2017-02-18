using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamPower
{
    public class List
    {
        public List<Dto.ExamPower.List> ExamPowerList { get; set; } = new List<Dto.ExamPower.List>();

        public List<System.Web.Mvc.SelectListItem> CourseTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();

        public int? CourseTypeId { get; set; } = System.Web.HttpContext.Current.Request["CourseTypeId"].ConvertToInt();

        public string ExamName { get; set; }
        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();
        public int ExamCourseId { get; set; } = System.Web.HttpContext.Current.Request["ExamCourseId"].ConvertToInt();

        public string FromDate { get; set; }

        public string ToDate { get; set; }
    }
}