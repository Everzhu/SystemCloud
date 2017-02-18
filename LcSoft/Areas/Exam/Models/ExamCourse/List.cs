using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace XkSystem.Areas.Exam.Models.ExamCourse
{
    public class List
    {
        public List<Dto.ExamCourse.List> ExamCourseList { get; set; } = new List<Dto.ExamCourse.List>();

        public List<Dto.ExamCourse.List> IdentifiedCourseList { get; set; } = new List<Dto.ExamCourse.List>();

        public List<System.Web.Mvc.SelectListItem> CourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamLevelGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string ExamName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();

        public int? CourseTypeId { get; set; } = System.Web.HttpContext.Current.Request["CourseTypeId"].ConvertToInt();

        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();
    }
}