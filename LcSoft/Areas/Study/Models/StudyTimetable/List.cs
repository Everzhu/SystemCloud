using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyTimetable
{
    public class List
    {
        public List<Dto.StudyTimetable.List> StudyTimetableList { get; set; } = new List<Dto.StudyTimetable.List>();
        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public string DateSearchFrom { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchFrom"]);
        public string DateSearchTo { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchTo"]);
        public int GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();
    }
}