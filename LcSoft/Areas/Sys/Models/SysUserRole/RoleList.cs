using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysUserRole
{
    public class RoleList
    {

        public List<Dto.SysUserRole.RoleList> DataList { get; set; } = new List<Dto.SysUserRole.RoleList>();

        public int UserId { get; set; }

        public string UserName { get; set; }
    }
}