using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassAllotClass
{
    public class Batch
    {
        public Dto.ClassAllotClass.Batch BatchEdit { get; set; } = new Dto.ClassAllotClass.Batch();

        public List<System.Web.Mvc.SelectListItem> GradeList = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YearList = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassTypeList = new List<System.Web.Mvc.SelectListItem>();
    }
}