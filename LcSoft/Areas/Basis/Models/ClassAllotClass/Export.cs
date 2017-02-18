using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassAllotClass
{
    public class Export
    {
        public List<Dto.ClassAllotClass.Export> ExportList { get; set; } = new List<Dto.ClassAllotClass.Export>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}