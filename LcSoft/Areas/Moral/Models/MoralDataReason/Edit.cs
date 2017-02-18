using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralDataReason
{
    public class Edit
    {

        public int MoralId { get; set; } = HttpContext.Current.Request["MoralId"].ConvertToInt();

        public List<SelectListItem> MoralItemList { get; set; } = new List<SelectListItem>();

        public Dto.MoralDataReason.Edit MoralDataReasonEdit { get; set; } = new Dto.MoralDataReason.Edit();

    }
}