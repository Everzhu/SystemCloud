using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformItem
{
    public class Edit
    {
        public Dto.PerformItem.Edit PerformItemEdit { get; set; } = new Dto.PerformItem.Edit();

        public List<System.Web.Mvc.SelectListItem> PerformGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<Dto.PerformOption.List> PerformOptionList { get; set; } = new List<Dto.PerformOption.List>();
        public int PerformId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();
    }
}