using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.Grade
{
    public class List
    {
        public List<Entity.tbGrade> GradeList { get; set; } = new List<Entity.tbGrade>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}