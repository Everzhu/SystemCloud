using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentType
{
    public class List
    {
        public List<Entity.tbStudentType> StudentTypeList { get; set; } = new List<Entity.tbStudentType>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}