using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormApply
{
    public class Edit
    {
        public Dto.DormApply.Edit DormApplyEdit { get; set; } = new Dto.DormApply.Edit();
        
        public List<System.Web.Mvc.SelectListItem> DormList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}