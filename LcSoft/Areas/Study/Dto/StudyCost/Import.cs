using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyCost
{
    public class Import
    {
        public int Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public string No { get; set; }
        /// <summary>
        /// 职工号
        /// </summary>
        [Display(Name = "职工号")]
        public string TeacherCode { get; set; }
        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherName { get; set; }
        /// <summary>
        /// 节次费用
        /// </summary>
        [Display(Name = "节次费用")]
        public string Cost { get; set; }
        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}