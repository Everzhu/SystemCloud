using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Sys.Models.SysUser
{
    public class Edit
    {
        public Dto.SysUser.Edit UserEdit { get; set; } = new Dto.SysUser.Edit();

        public List<System.Web.Mvc.SelectListItem> SexList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}
