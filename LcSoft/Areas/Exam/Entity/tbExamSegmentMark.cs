using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 分数段
    /// </summary>
    public class tbExamSegmentMark : Code.EntityHelper.EntityBase
    {
        /// <summary>
        ///分数段名称
        /// </summary>
        [Required]
        [Display(Name = "分数段名称")]
        public string SegmentName { get; set; }

        /// <summary>
        /// 最小值百分数
        /// </summary>
        [Required]
        [Display(Name = "最小值百分数")]
        public decimal MinMark { get; set; }

        /// <summary>
        /// 最大值百分数
        /// </summary>
        [Required]
        [Display(Name = "最大值百分数")]
        public decimal MaxMark { get; set; }

        /// <summary>
        /// 是否优秀
        /// </summary>
        [Required]
        [Display(Name = "是否优秀")]
        public bool IsGood { get; set; }

        /// <summary>
        /// 是否及格
        /// </summary>
        [Required]
        [Display(Name = "是否及格")]
        public bool IsPass { get; set; }

        /// <summary>
        /// 是否良好
        /// </summary>
        [Required]
        [Display(Name = "是否良好")]
        public bool IsNormal { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目")]
        public virtual Course.Entity.tbSubject tbSubject { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public virtual Basis.Entity.tbGrade tbGrade { get; set; }

        /// <summary>
        /// 考试分数段分组
        /// </summary>
        [Display(Name = "考试分数段分组")]
        public virtual Entity.tbExamSegmentGroup tbExamSegmentGroup { get; set; }

        /// <summary>
        /// 是否总分等级
        /// </summary>
        [Required]
        [Display(Name = "是否总分等级")]
        public bool IsTotal { get; set; }

        /// <summary>
        /// 比例
        /// </summary>
        [Required]
        [Display(Name = "比例")]
        public decimal Rate { get; set; }

        /// <summary>
        /// 百分比或分数段
        /// </summary>
        [Required]
        [Display(Name = "百分比或分数段")]
        public bool IsGenerate { get; set; }
    }
}