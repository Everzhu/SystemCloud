using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.Class
{
    public class Info
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "班级名称"), Required]
        public string ClassName { get; set; }
    }
}