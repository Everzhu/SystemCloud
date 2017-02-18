using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralPower
{
    public class Import
    {
        [Display(Name = "德育选项")]
        public string MoralItemName { get; set; }

        [Display(Name = "评分日期")]
        public string MoralDate { get; set; }

        [Display(Name = "评分人")]
        public string TeacherName { get; set; }

        [Display(Name ="评分班级")]
        public string MoralClass { get; internal set; }

        [Display(Name = "导入结果")]
        public string ImportError { get; set; }

    }
}