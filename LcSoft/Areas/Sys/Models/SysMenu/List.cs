using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysMenu
{
    public class List
    {
        public List<Dto.SysMenu.List> MenuList { get; set; } = new List<Dto.SysMenu.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ParentId { get; set; } = System.Web.HttpContext.Current.Request["ParentId"].ConvertToInt();
    }
}