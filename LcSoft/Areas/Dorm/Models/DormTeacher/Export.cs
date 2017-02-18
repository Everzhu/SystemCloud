using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormTeacher
{
    public class Export
    {
        public List<Dto.DormTeacher.Export> ExportList { get; set; } = new List<Dto.DormTeacher.Export>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? BuildId { get; set; } = System.Web.HttpContext.Current.Request["BuildId"].ConvertToInt();
    }
}