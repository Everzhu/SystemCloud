using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.Dorm
{
    public class Edit
    {
        public Dto.Dorm.Edit DormEdit { get; set; } = new Dto.Dorm.Edit();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}