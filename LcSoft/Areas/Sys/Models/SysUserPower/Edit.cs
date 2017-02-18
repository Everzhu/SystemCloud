using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysUserPower
{
    public class Edit
    {
        public string Power { get; set; }

        public int UserId { get; set; } = System.Web.HttpContext.Current.Request["UserId"].ConvertToInt();
    }
}