using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamSchedule
{
    public class Edit
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 考试名称
        /// </summary>
        [Display(Name = "考试名称"), Required]
        public int ExamId { get; set; }

        /// <summary>
        /// 场次名称
        /// </summary>
        [Display(Name = "场次名称"), Required]
        public string ExamScheduleName { get; set; }

        /// <summary>
        /// 考试日期
        /// </summary>
        [Display(Name = "考试日期"), Required]
        public DateTime ScheduleDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 场次号
        /// </summary>
        [Display(Name = "场次号"), Required]
        public int ScheduleNo { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间"), Required]
        public DateTime FromDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间"), Required]
        public DateTime ToDate { get; set; } = DateTime.Now;
    }
}
