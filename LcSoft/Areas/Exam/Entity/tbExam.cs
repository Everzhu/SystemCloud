using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考试
    /// </summary>
    public class tbExam : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考试名称
        /// </summary>
        [Required]
        [Display(Name = "考试名称")]
        public string ExamName { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        [Required]
        [Display(Name = "是否显示")]
        public bool IsPublish { get; set; }

        /// <summary>
        /// 考试类型
        /// </summary>
        [Required]
        [Display(Name = "考试类型")]
        public virtual tbExamType tbExamType { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年")]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 考试等级组
        /// </summary>
        [Display(Name = "考试等级组")]
        public virtual tbExamLevelGroup tbExamLevelGroup { get; set; }

        /// <summary>
        /// 考试分数段分组
        /// </summary>
        [Display(Name = "考试分数段分组")]
        public virtual Entity.tbExamSegmentGroup tbExamSegmentGroup { get; set; }
    }
}