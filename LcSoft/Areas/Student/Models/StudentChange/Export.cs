using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentChange
{
    public class Export
    {
        public List<Dto.StudentChange.Export> ExportList { get; set; } = new List<Dto.StudentChange.Export>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}