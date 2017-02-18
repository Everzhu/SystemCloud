using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.Student
{
    public class Edit
    {
        public Dto.Student.Edit StudentEdit { get; set; } = new Dto.Student.Edit();

        public List<System.Web.Mvc.SelectListItem> StudentTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentStudyTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentSessionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SexList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> BloodTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> DictNationList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> DictPartyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string StudentHonorJson { get; set; }

        public string StudentFamilyJson { get; set; }
    }
}