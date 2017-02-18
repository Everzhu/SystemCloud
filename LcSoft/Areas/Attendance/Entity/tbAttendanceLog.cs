using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Entity
{
    /// <summary>
    /// 考勤日志表
    /// </summary>
    public class tbAttendanceLog : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考勤机编号
        /// </summary>
        [Required]
        [Display(Name = "考勤机编号")]
        public string MachineCode { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        [Required]
        [Display(Name = "考勤时间")]
        public DateTime AttendanceDate { get; set; }
        
        /// <summary>
        /// 卡号
        /// </summary>
        [Required]
        [Display(Name = "卡号")]
        public string CardNumber { get; set; }

        /// <summary>
        /// 处理状态（默认未处理，已处理的则进行了考勤计算）
        /// </summary>
        [Required]
        [Display(Name = "处理状态")]
        public bool Status { get; set; }
    }
}