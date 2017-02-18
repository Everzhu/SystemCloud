using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentHonor
{
    public class EditStudentHonor
    {
        public Dto.StudentHonor.EditStudentHonor StudentHonorEdit { get; set; } = new Dto.StudentHonor.EditStudentHonor();

        public int StudentId { get; set; }

        public List<System.Web.Mvc.SelectListItem> StudentHonorLevelList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentHonorTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentHonorSourceList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}