using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgManager
{
    public class EditOrg
    {
        public Dto.OrgManager.EditOrg DataEdit { get; set; } = new Dto.OrgManager.EditOrg();

        public List<Dto.OrgManager.EditOrgList> OrgList { get; set; } = new List<Dto.OrgManager.EditOrgList>();

        public int TeacherId { get; set; }

        //public List<System.Web.Mvc.SelectListItem> OrgList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}