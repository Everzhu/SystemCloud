using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.Teacher
{
    public class Edit
    {
        public Dto.Teacher.Edit TeacherEdit { get; set; } = new Dto.Teacher.Edit();

        [Display(Name = "部门"), Required]
        [MinLength(1, ErrorMessage = "请至少选择一个部门")]
        public string TeacherDeptIds { get; set; }

        [Display(Name = "部门"), Required]
        public string TeacherDeptNameJson { get; set; }

        public List<System.Web.Mvc.SelectListItem> EducationList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> TeacherTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> TeacherDeptList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SexList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> DictPartyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> DictNationList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> DictMarriageList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> DictRegionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> DictHealthList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}