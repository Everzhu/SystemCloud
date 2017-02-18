using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherGrade
{
    public class Edit
    {
        [Display(Name = "教师"), Required]
        public int TeacherId { get; set; }

        public List<int> TeacherGradeList { get; set; } = new List<int>();

        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}