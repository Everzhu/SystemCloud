using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.ClassGroup
{
    public class Info
    {
        public int Id { get; set; }

        [Display(Name = "小组名称")]
        public string ClassGroupName { get; set; }

        [Display(Name = "班级Id")]
        public int ClassId { get; set; }

        [Display(Name ="班级名称")]
        public string ClassName { get; set; }
    }
}