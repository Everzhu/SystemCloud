using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamSchedule
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 场次名称
        /// </summary>
        [Display(Name = "场次名称")]
        public string ExamScheduleName { get; set; }

        /// <summary>
        /// 考试名称
        /// </summary>
        [Display(Name = "考试名称")]
        public string ExamName { get; set; }

        /// <summary>
        /// 考试日期
        /// </summary>
        [Display(Name = "考试日期")]
        public DateTime ScheduleDate { get; set; }

        /// <summary>
        /// 场次号
        /// </summary>
        [Display(Name = "场次号")]
        public int  ScheduleNo { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime ToDate { get; set; }

        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        [Display(Name = "考试课程")]
        public string CourseName { get; set; }
    }
}
