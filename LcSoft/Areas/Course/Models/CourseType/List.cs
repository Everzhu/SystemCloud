using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.CourseType
{
    public class List
    {
        public List<Entity.tbCourseType> CourseTypeList { get; set; } = new List<Entity.tbCourseType>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}