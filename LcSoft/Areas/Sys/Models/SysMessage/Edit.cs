using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysMessage
{
    public class Edit
    {
        public Dto.SysMessage.Edit MessageEdit { get; set; } = new Dto.SysMessage.Edit();

        public List<System.Web.Mvc.SelectListItem> RoleList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string RoleIds { get; set; } = HttpContext.Current.Request["RoleIds"].ConvertToString();
    }
}