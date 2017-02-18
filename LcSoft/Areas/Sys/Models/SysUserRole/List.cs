using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysUserRole
{
    public class List
    {
        public Dto.SysUserRole.List SysUserRole { get; set; } = new Dto.SysUserRole.List();

        public List<Dto.SysUserRole.List> SysUserRoleList { get; set; } = new List<Dto.SysUserRole.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int RoleId { get; set; } = System.Web.HttpContext.Current.Request["RoleId"].ConvertToInt();

        public string RoleName { get; set; }

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}