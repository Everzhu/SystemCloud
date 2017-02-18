using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentHonorLevel
{
    public class List
    {
        public List<Entity.tbStudentHonorLevel> StudentHonorLevelList { get; set; } = new List<Entity.tbStudentHonorLevel>();

        public List<System.Web.Mvc.SelectListItem> StudentTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}