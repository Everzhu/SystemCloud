using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysShortcut
{
    public class List
    {
        public List<Dto.SysShortcut.List> SysShortcutList { get; set; } = new List<Dto.SysShortcut.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}