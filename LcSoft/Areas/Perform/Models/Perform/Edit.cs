using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.Perform
{
    public class Edit
    {
        public Dto.Perform.Edit PerformEdit { get; set; } = new Dto.Perform.Edit();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> PerformList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string CreateWay { get; set; }

        public int CopyPerformId { get; set; }
    }
}