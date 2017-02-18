using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Admin.Models.Tenant
{
    public class Edit
    {
        public Dto.Tenant.Edit TenantEdit { get; set; } = new Dto.Tenant.Edit();

        public List<System.Web.Mvc.SelectListItem> ProgramList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> TenantProgramList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}