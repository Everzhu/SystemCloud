using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysUser
{
    public class SelectUser
    {
        public List<Dto.SysUser.SelectUser> UserList { get; set; } = new List<Dto.SysUser.SelectUser>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}