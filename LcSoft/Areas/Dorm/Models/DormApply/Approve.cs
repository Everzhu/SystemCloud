using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormApply
{
    public class Approve
    {
        public Dto.DormApply.Approve DormApplyApprove { get; set; } = new Dto.DormApply.Approve();

        public List<System.Web.Mvc.SelectListItem> CheckStatusList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}