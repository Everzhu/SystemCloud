using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformItem
{
    public class List
    {
        public List<Dto.PerformItem.List> PerformItemList { get; set; } = new List<Dto.PerformItem.List>();
        public string PerformName { get; set; }
        public List<System.Web.Mvc.SelectListItem> PerformGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int PerformId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();
    }
}