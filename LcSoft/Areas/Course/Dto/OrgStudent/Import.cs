using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.OrgStudent
{
    public class Import
    {
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        [Display(Name = "座位号")]
        public string No { get; set; }

        [Display(Name = "错误提示")]
        public string Error { get; set; }

    }
}