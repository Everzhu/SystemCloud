using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralClass
{
    public class List
    {
        public string MoralName { get; set; }

        public int MoralId { get; set; } = System.Web.HttpContext.Current.Request["MoralId"].ConvertToInt();

        public string ClassIds { get; set; }
    }
}