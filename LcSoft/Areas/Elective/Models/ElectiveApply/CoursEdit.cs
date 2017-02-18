using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Models.ElectiveApply
{
    public class CourseEdit
    {
        public Areas.Course.Dto.Course.Edit ApplyCourse { get; set; } = new Course.Dto.Course.Edit();

        public int ElectiveId { get; set; } = HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public string CourseName { get; set; } = HttpContext.Current.Request["CourseName"].ConvertToString();

        public bool IsError { get; set; }

        public List<SelectListItem> CourseSubjectList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CourseTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CourseDomainList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> CourseGroupList { get; set; } = new List<SelectListItem>();
    }
}