using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentChangeType
{
    public class List
    {
        public List<Dto.StudentChangeType.List> StudentChangeTypeList { get; set; } = new List<Dto.StudentChangeType.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}