using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Dto.Attendance
{
    public class Info
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Display(Name = "Id")]
        public int Id { get; set; }
        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "考勤类型")]
        public string AttendanceTypeName { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "班级Id")]
        public int ClassId { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "班级名称")]
        public string ClassName { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        [Display(Name = "考勤")]
        public int AttendanceTypeId { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        [Display(Name = "考勤时间")]
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
        /// 人数
        /// </summary>
        [Display(Name = "人数")]
        public int CountNum { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        [Display(Name = "学生Id")]
        public int StudentId { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "节次")]
        public int PeriodId { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "班级")]
        public int OrgId { get; set; }
    }
}