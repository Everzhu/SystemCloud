using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysUser
{
    public class UserList
    {
        public List<Dto.SysUser.UserList> DataList { get; set; } = new List<Dto.SysUser.UserList>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        
        public Code.EnumHelper.SysUserType? UserType { get; set; }
    }
}