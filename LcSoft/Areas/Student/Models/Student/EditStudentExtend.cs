using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.Student
{
    public class EditStudentExtend
    {
        public Dto.Student.EditStudentExtend StudentExtend { get; set; } = new Dto.Student.EditStudentExtend();

        public string Step { get; set; } = System.Web.HttpContext.Current.Request["Step"].ConvertToString();

        public List<System.Web.Mvc.SelectListItem> DictBloodTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> DictPartyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> DictNationList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SchoolTransportationTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}