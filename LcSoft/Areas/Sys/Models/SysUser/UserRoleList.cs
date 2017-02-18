using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysUser
{
    public class UserRoleList
    {
        public List<Dto.SysUser.UserRoleList> DataList { get; set; } = new List<Dto.SysUser.UserRoleList>();

        //public int UserId { get; set; } = HttpContext.Current.Request["UserId"].ConvertToInt();
    }
}