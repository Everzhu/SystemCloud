using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysConfig
{
    public class List
    {
        public List<Entity.tbSysConfig> SysConfigList = new List<Entity.tbSysConfig>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}