using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysIndex
{
    public class Left
    {
        public List<Dto.SysMenu.Info> MenuInfo { get; set; } = new List<Dto.SysMenu.Info>();
    }
}