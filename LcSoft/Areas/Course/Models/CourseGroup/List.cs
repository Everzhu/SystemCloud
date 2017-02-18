using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.CourseGroup
{
    public class List
    {
        public List<Entity.tbCourseGroup> CourseGroupList { get; set; } = new List<Entity.tbCourseGroup>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}