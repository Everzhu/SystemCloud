using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Admin.Models.Tenant
{
    public class List
    {
        public List<Entity.tbTenant> TenantList { get; set; } = new List<Entity.tbTenant>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}