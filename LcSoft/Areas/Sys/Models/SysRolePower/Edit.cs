using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysRolePower
{
    public class Edit
    {
        public string Power { get; set; }

        public int RoleId { get; set; } = System.Web.HttpContext.Current.Request["RoleId"].ConvertToInt();
    }
}