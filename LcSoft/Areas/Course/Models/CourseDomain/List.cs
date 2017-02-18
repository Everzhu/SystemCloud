using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.CourseDomain
{
    public class List
    {
        public List<Entity.tbCourseDomain> CourseDomainList { get; set; } = new List<Entity.tbCourseDomain>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}