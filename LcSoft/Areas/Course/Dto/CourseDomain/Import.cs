using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.CourseDomain
{
    public class Import
    {
        [Display(Name = "排序")]
        public int? No { get; set; }

        [Display(Name = "课程领域")]
        public string CourseDomainName { get; set; }

        [Display(Name = "领域类型")]
        public string CourseDomainTypeName { get; set; }

        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}