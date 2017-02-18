using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysRole
{
    public class MenuList
    {
        //public List<Dto.SysRole.MenuList> DataList { get; set; } = new List<Dto.SysRole.MenuList>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}