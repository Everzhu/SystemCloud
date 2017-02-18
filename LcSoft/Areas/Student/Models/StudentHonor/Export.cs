using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentHonor
{
    public class Export
    {
        public List<Dto.StudentHonor.Export> exportList { get; set; } = new List<Dto.StudentHonor.Export>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}