using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysMenu
{
    public class Edit
    {
        public Dto.SysMenu.Edit MenuEdit { get; set; } = new Dto.SysMenu.Edit();

        public List<System.Web.Mvc.SelectListItem> ParentMenuList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int ParentId { get; set; } = System.Web.HttpContext.Current.Request["ParentId"].ConvertToInt();
    }
}