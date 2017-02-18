using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentHonor
{
    public class Edit
    {
        public Dto.StudentHonor.Edit StudentHonorEdit { get; set; } = new Dto.StudentHonor.Edit();

        public int StudnetId { get; set; }

        public List<System.Web.Mvc.SelectListItem> StudentHonorLevelList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentHonorTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}