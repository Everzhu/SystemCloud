using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamCourseRate
{
    public class List
    {
        public List<Dto.ExamCourseRate.List> ExamCourseRateList { get; set; } = new List<Dto.ExamCourseRate.List>();

        public List<Dto.ExamCourseRate.List> ExamCourseUnionList { get; set; } = new List<Dto.ExamCourseRate.List>();

        public List<System.Web.Mvc.SelectListItem> ExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ExamCourseId { get; set; } = System.Web.HttpContext.Current.Request["ExamCourseId"].ConvertToInt();

        public int ExamUionId { get; set; } = System.Web.HttpContext.Current.Request["ExamUionId"].ConvertToInt();

        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();

        public int SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();
    }
}