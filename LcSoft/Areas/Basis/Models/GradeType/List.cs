using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.GradeType
{
    public class List
    {
        public List<Entity.tbGradeType> GradeTypeList { get; set; } = new List<Entity.tbGradeType>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}