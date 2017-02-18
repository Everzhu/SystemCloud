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
    public class tbExamImportSegmentMark : Code.EntityHelper.EntityBase
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
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public virtual Basis.Entity.tbGrade tbGrade { get; set; }

    }
}