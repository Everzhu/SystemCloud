using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherType
{
    public class List
    {
        public List<Entity.tbTeacherType> DataList { get; set; } = new List<Entity.tbTeacherType>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}