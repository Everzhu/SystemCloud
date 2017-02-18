using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassAllotStudent
{
    public class Export
    {
        public List<Dto.ClassAllotStudent.Export> exportList { get; set; } = new List<Dto.ClassAllotStudent.Export>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();


    }
}