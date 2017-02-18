using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysUserPower
{
    public class List
    {
        public List<Dto.SysUserPower.List> SysUserPowerList { get; set; } = new List<Dto.SysUserPower.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}