using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考试等级
    /// </summary>
    public class tbExamLevel : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考试等级
        /// </summary>
        [Required]
        [Display(Name = "考试等级")]
        public string ExamLevelName { get; set; }

        /// <summary>
        /// 等级积点
        /// </summary>
        [Required]
        [Display(Name = "等级积点")]
        public decimal ExamLevelValue { get; set; }

        /// <summary>
        /// 比例
        /// </summary>
        [Required]
        [Display(Name = "比例")]
        public decimal Rate { get; set; }

        /// <summary>
        /// 最高百分数
        /// </summary>
        [Required]
        [Display(Name = "最高百分数")]
        public decimal MaxScore { get; set; }

        /// <summary>
        /// 最低分百分数
        /// </summary>
        [Required]
        [Display(Name = "最低分百分数")]
        public decimal MinScore { get; set; }

        /// <summary>
        /// 考试等级组
        /// </summary>
        //[Required]
        [Display(Name = "考试等级组")]
        public virtual Entity.tbExamLevelGroup tbExamLevelGroup { get; set; }

        /// <summary>
        /// 是否总分等级
        /// </summary>
        [Display(Name = "是否总分等级")]
        public bool IsTotal { get; set; }
    }
}