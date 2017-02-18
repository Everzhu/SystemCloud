using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysRole
{
    public class List
    {
        public List<Entity.tbSysRole> RoleList { get; set; } = new List<Entity.tbSysRole>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}