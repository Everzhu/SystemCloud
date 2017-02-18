using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysUser
{
    public class List
    {
        public List<Dto.SysUser.List> UserList { get; set; } = new List<Dto.SysUser.List>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public Code.EnumHelper.SysUserType? UserType { get; set; }
    }
}
