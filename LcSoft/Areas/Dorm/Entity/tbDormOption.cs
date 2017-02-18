using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Entity
{
    /// <summary>
    /// 表现选项
    /// </summary>
    public class tbDormOption : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 表现名称
        /// </summary>
        [Required]
        [Display(Name = "表现名称")]
        public string DormOptionName { get; set; }

        /// <summary>
        /// 表现分
        /// </summary>
        [Required]
        [Display(Name = "表现分")]
        public decimal DormOptionValue { get; set; }
    }
}