using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysUserLog
{
    public class List
    {
        public List<Entity.tbSysUserLog> UserLogList { get; set; } = new List<Entity.tbSysUserLog>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public string DateSearchFrom { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchFrom"]);

        public string DateSearchTo { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchTo"]);

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}