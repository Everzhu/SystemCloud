using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考试安排
    /// </summary>
    public class tbExamSchedule : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 场次名称
        /// </summary>
        [Required]
        [Display(Name = "场次名称")]
        public string ExamScheduleName { get; set; }

        /// <summary>
        /// 所属考试
        /// </summary>
        [Required]
        [Display(Name = "所属考试")]
        public virtual tbExam tbExam { get; set; }

        /// <summary>
        /// 考试日期
        /// </summary>
        [Required]
        [Display(Name = "考试日期")]
        public DateTime ScheduleDate { get; set; }

        /// <summary>
        /// 场次
        /// </summary>
        [Required]
        [Display(Name = "场次")]
        public int ScheduleNo { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        [Display(Name = "开始时间")]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Required]
        [Display(Name = "结束时间")]
        public DateTime ToDate { get; set; }
    }
}