using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralData
{
    public class DayEdit:OnceEdit
    {

        public DateTime MoralDate { get; set; } = HttpContext.Current.Request["MoralDate"].ConvertToDateTime();

        public string FromDate { get; set; }

        public string ToDate { get; set; }

    }
}