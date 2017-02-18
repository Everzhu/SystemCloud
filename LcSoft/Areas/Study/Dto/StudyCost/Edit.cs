using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyCost
{
    public class Edit
    {
        public int Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }
        /// <summary>
        /// 教师
        /// </summary>
        [Display(Name = "教师"), Required]
        public int  TeacherId { get; set; }
        /// <summary>
        /// 费用
        /// </summary>
        [Display(Name = "节次费用"), Required]
        public decimal Cost { get; set; }
    }
}