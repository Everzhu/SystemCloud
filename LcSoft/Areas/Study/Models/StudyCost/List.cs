using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyCost
{
    public class List
    {
        public List<Dto.StudyCost.List> StudyCostList { get; set; } = new List<Dto.StudyCost.List>();
        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int TeacherId { get; set; } = System.Web.HttpContext.Current.Request["TeacherId"].ConvertToInt();
    }
}