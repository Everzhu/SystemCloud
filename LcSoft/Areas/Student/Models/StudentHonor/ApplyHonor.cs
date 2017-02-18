using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentHonor
{
    public class ApplyHonor
    {
        public Dto.StudentHonor.ApplyHonor ApplyHonorEdit { get; set; } = new Dto.StudentHonor.ApplyHonor();

        public List<System.Web.Mvc.SelectListItem> StudentHonorLevelList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentHonorTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}