using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Subject
{
    public class List
    {
        public List<Entity.tbSubject> SubjectList { get; set; } = new List<Entity.tbSubject>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}