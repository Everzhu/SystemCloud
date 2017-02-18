using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysRole
{
    public class MenuRoleList
    {
        public List<Dto.SysRole.MenuRoleList> DataList { get; set; } = new List<Dto.SysRole.MenuRoleList>();

        public int MenuId { get; set; }
    }
}