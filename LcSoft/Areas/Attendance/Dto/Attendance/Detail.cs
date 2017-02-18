using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Dto.Attendance
{
    public class Detail
    {
        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "行政班Id")]
        public int ClassId { get; set; }

        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public string ClassName { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "教学班Id")]
        public int OrgId { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "考勤Id")]
        public int AttendanceTypeId { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "考勤类型")]
        public string AttendanceTypeName { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        [Display(Name = "考勤日期")]
        public DateTime AttendanceDate { get; set; }

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

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "节次")]
        public string PeriodName { get; set; }
    }
}