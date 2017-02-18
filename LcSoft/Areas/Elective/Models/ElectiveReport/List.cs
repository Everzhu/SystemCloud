using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static XkSystem.Code.PageHelper;

namespace XkSystem.Areas.Elective.Models.ElectiveReport
{
    public class UnElective
    {
        public List<System.Web.Mvc.SelectListItem> ElectiveList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();
    }
}