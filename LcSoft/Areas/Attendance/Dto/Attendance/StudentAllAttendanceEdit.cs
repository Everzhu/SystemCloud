using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Dto.Attendance
{
    public class StudentAllAttendanceEdit
    {

        public int Id { get; set; }

        /// <summary>
        /// 对应学生
        /// </summary>
        [Display(Name = "对应学生"), Required]
        public int StudentId { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "考勤类型"), Required]
        public int AttendanceTypeId { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        [Display(Name = "考勤时间"), Required]
        public DateTime AttendanceDate { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "节次"), Required]
        public int PeriodId { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "开始节次"), Required]
        public int PeriodFormId { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "结束节次"), Required]
        public int PeriodToId { get; set; }
        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "节次"), Required]
        public int PeriodName { get; set; }

        /// <summary>
        /// 对应课程
        /// </summary>
        [Display(Name = "对应课程"), Required]
        public int CourseId { get; set; }

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