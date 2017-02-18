using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherHonor
{
    public class Edit
    {
        public Dto.TeacherHonor.Edit DataEdit { get; set; } = new Dto.TeacherHonor.Edit();

        public List<System.Web.Mvc.SelectListItem> TeacherHonorLevelList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> TeacherHonorTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}