using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyClassTeacher
{
    public class Edit
    {
        [Display(Name = "教师"), Required]
        public int TeacherId { get; set; }
        [Display(Name = "班级"), Required]
        public int ClassId { get; set; }
        public Dto.StudyClassTeacher.Edit StudyClassTeacherEdit { get; set; } = new Dto.StudyClassTeacher.Edit();
        public List<int> WeekIdList = new List<int>();
        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> WeekList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
    }
}