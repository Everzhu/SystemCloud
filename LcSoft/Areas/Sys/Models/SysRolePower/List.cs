using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysRolePower
{
    public class List
    {
        public List<Dto.SysRolePower.List> SysRolePowerList { get; set; } = new List<Dto.SysRolePower.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}