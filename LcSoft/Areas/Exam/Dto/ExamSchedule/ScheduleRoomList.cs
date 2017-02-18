using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamSchedule
{
    public class ScheduleRoomList
    {
        /// <summary>
        /// 场次
        /// </summary>
        public int ScheduleId { get; set; }

        [Display(Name = "考试课程")]
        public string CourseName { get; set; }

        public int ExamCourseId { get; set; }

        /// <summary>
        /// 考场名称
        /// </summary>
        [Display(Name = "考场名称")]
        public string ExamRoomName { get; set; }

        public int ExamRoomId { get; set; }

        /// <summary>
        /// 考场名称
        /// </summary>
        [Display(Name = "考试教室")]
        public string RoomName { get; set; }

        public DateTime ScheduleDate { get; set; }

        public int ScheduleNo { get; set; }

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
    }
}
