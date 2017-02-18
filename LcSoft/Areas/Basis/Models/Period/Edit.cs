using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.Period
{
    public class Edit
    {
        public Dto.Period.Edit PeriodEdit { get; set; } = new Dto.Period.Edit();

        public List<System.Web.Mvc.SelectListItem> PeriodTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}