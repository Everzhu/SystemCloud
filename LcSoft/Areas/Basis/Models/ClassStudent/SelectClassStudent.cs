using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassStudent
{
    public class SelectClassStudent
    {
        public List<Dto.ClassStudent.SelectClassStudent> ClassStudentList { get; set; } = new List<Dto.ClassStudent.SelectClassStudent>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public string ClassName { get; set; }
    }
}