using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Dto.AttendanceLog
{
    public class Export
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 考勤机编号
        /// </summary>
        [Display(Name = "考勤机编号")]
        public string MachineCode { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        [Display(Name = "考勤时间")]
        public DateTime AttendanceDate { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        [Display(Name = "卡号")]
        public string CardNumber { get; set; }

        /// <summary>
        /// 处理状态（默认未处理，已处理的则进行了考勤计算）
        /// </summary>
        [Display(Name = "处理状态")]
        public bool Status { get; set; }

        public string StatusName { get; set; }
    }
}