using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Models.Sms
{
    public class SelectUser
    {
        public List<Dto.Sms.SelectUser> SelectUserList { get; set; } = new List<Dto.Sms.SelectUser>();
        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public Code.EnumHelper.SysUserType UserType { get; set; } = Code.EnumHelper.SysUserType.Teacher;

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}