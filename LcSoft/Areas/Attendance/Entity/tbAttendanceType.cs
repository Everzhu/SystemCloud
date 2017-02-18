using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Entity
{
    /// <summary>
    /// 学生考勤类型
    /// </summary>
    public class tbAttendanceType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考勤类型
        /// </summary>
        [Required]
        [Display(Name = "考勤类型")]
        public string AttendanceTypeName { get; set; }

        /// <summary>
        /// 考勤分值
        /// </summary>
        [Required]
        [Display(Name = "考勤分值")]
        public decimal AttendanceValue { get; set; }
    }
}