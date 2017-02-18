using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherLevel
{
    public class List
    {
        public List<Entity.tbTeacherLevel> DataList { get; set; } = new List<Entity.tbTeacherLevel>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}