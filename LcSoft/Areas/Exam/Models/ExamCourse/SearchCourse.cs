using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace XkSystem.Areas.Exam.Models.ExamCourse
{
    public class SearchCourse
    {
        public List<Dto.ExamCourse.List> ExamCourseList { get; set; } = new List<Dto.ExamCourse.List>();

        public List<Areas.Course.Dto.Course.List> SubjectCourseList { get; set; } = new List<Areas.Course.Dto.Course.List>();

        public List<System.Web.Mvc.SelectListItem> CourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> FieldList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamLevelGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();

        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["examId"].ConvertToInt();

        public int FieldId { get; set; } = System.Web.HttpContext.Current.Request["FieldId"].ConvertToInt();
    }
}