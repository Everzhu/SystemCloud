using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考试等级分组
    /// </summary>
    public class tbExamLevelGroup : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考试等级分组
        /// </summary>
        [Required]
        [Display(Name = "考试等级分组")]
        public string ExamLevelGroupName { get; set; }

        /// <summary>
        /// 是否总分等级组
        /// </summary>
        [Required]
        [Display(Name = "是否总分等级组")]
        public bool IsTotal { get; set; }

        /// <summary>
        /// 百分比或分数段
        /// </summary>
        [Required]
        [Display(Name = "百分比或分数段")]
        public bool IsGenerate { get; set; }
    }
}