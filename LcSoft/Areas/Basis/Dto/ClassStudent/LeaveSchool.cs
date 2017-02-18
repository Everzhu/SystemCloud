using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.ClassStudent
{
    public class LeaveSchool
    {
        [Display(Name = "学生ID")]
        public int StudentId { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "学生调动")]
        public string StudentChangeName { get; set; }
    }
}