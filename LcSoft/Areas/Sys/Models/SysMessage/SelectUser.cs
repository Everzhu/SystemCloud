using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysMessage
{
    public class SelectUser
    {
        public List<Dto.SysMessage.SelectUser> SelectUserList { get; set; } = new List<Dto.SysMessage.SelectUser>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public Code.EnumHelper.SysUserType? UserType { get; set; }
    }
}