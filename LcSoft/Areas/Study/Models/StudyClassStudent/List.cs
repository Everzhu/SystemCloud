using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyClassStudent
{
    public class List
    {
        public List<Dto.StudyClassStudent.List> StudyClassStudentList { get; set; } = new List<Dto.StudyClassStudent.List>();
        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
    }
}