using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralDataReason
{
    public class List
    {
        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();

        public int MoralId { get; set; } = HttpContext.Current.Request["MoralId"].ConvertToInt();

        public List<Dto.MoralDataReason.List> MoralDataReasonList { get; set; } = new List<Dto.MoralDataReason.List>();
    }
}