using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考试课程
    /// </summary>
    public class tbExamCourse : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属考试
        /// </summary>
        [Required]
        [Display(Name = "所属考试")]
        public virtual tbExam tbExam { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Required]
        [Display(Name = "考试课程")]
        public virtual Course.Entity.tbCourse tbCourse { get; set; }

        /// <summary>
        /// 过程分折算
        /// </summary>
        [Required]
        [Display(Name = "过程分折算")]
        public decimal AppraiseRate { get; set; } = 100;

        /// <summary>
        /// 过程分满分值
        /// </summary>
        [Required]
        [Display(Name = "过程分满分值")]
        public decimal FullAppraiseMark { get; set; } = 100;

        /// <summary>
        /// 考试成绩折算
        /// </summary>
        [Required]
        [Display(Name = "考试成绩折算")]
        public decimal TotalRate { get; set; } = 100;

        /// <summary>
        /// 考试成绩满分值
        /// </summary>
        [Required]
        [Display(Name = "考试成绩满分值")]
        public decimal FullTotalMark { get; set; } = 100;

        /// <summary>
        /// 综合成绩满分值
        /// </summary>
        [Required]
        [Display(Name = "综合成绩满分值")]
        public decimal FullSegmentMark { get; set; } = 100;

        /// <summary>
        /// 等级组
        /// </summary>
        [Required]
        [Display(Name = "等级组")]
        public virtual tbExamLevelGroup tbExamLevelGroup { get; set; }

        /// <summary>
        /// 学习时间段
        /// </summary>
        [Display(Name = "学习时间段")]
        public virtual tbExamSection tbExamSection { get; set; }

        /// <summary>
        /// 所属考场安排
        /// </summary>
        [Display(Name = "所属考场安排")]
        public virtual tbExamSchedule tbExamSchedule { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// 模块认定
        /// </summary>
        [Required]
        [Display(Name = "模块认定")]
        public bool Identified { get; set; }
    }
}