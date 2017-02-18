using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamChange
{
    public class List
    {
        public List<Dto.ExamChange.List> ExamMarkList { get; set; } = new List<Dto.ExamChange.List>();

        public List<System.Web.Mvc.SelectListItem> ExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> OrgList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamStatusList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ExamLevelList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();

        public int CourseId { get; set; } = System.Web.HttpContext.Current.Request["CourseId"].ConvertToInt();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();
    }
}