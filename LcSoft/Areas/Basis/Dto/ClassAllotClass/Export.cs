using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.ClassAllotClass
{
    public class Export
    {
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称"), Required]
        public string ClassName { get; set; }

        /// <summary>
        /// 班级类型
        /// </summary>
        [Display(Name = "班级类型")]
        public string ClassTypeName { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }
    }
}