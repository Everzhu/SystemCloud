using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherJob
{
    public class List
    {
        public List<Entity.tbTeacherJob> DataList { get; set; } = new List<Entity.tbTeacherJob>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}