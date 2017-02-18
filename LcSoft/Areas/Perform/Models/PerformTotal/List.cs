using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformTotal
{
    public class List
    {
        public List<Dto.PerformTotal.List> PerformTotalList { get; set; } = new List<Dto.PerformTotal.List>();
        public List<System.Web.Mvc.SelectListItem> PerformList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<Dto.PerformItem.List> PerformItemList { get; set; } = new List<Dto.PerformItem.List>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int PerformId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();        
    }
}