using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Entity
{
    /// <summary>
    /// 学生考勤
    /// </summary>
    public class tbAttendance : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属学生
        /// </summary>
        [Required]
        [Display(Name = "所属学生")]
        public Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 考勤类型
        /// </summary>
        [Required]
        [Display(Name = "考勤类型")]
        public virtual tbAttendanceType tbAttendanceType { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        [Required]
        [Display(Name = "考勤时间")]
        public DateTime AttendanceDate { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Required]
        [Display(Name = "节次")]
        public virtual Basis.Entity.tbPeriod tbPeriod { get; set; }

        /// <summary>
        /// 对应班级
        /// </summary>
        [Required]
        [Display(Name = "对应班级")]
        public virtual Course.Entity.tbOrg tbOrg { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Required]
        [Display(Name = "录入人员")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Required]
        [Display(Name = "录入时间")]
        public DateTime InputDate { get; set; }
    }
}