using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Entity
{
    /// <summary>
    /// 晚自习表现
    /// </summary>
    public class tbStudyOption : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 表现名称
        /// </summary>
        [Display(Name = "表现名称"), Required]
        public string StudyOptionName { get; set; }

        /// <summary>
        /// 表现分值
        /// </summary>
        [Display(Name = "表现分值"), Required]
        public decimal StudyOptionValue { get; set; }
    }
}