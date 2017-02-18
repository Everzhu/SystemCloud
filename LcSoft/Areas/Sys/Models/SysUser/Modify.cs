using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Sys.Models.SysUser
{
    public class Modify
    {
        public Dto.SysUser.Modify UserModify { get; set; } = new Dto.SysUser.Modify();

        public List<System.Web.Mvc.SelectListItem> UserTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SexList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public bool Status { get; set; }
    }
}
