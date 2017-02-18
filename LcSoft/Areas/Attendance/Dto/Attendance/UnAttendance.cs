using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Attendance.Dto.Attendance
{
    public class UnAttendance
    {
        public int Id { get; set; }

        /// <summary>
        /// 教师Id
        /// </summary>
        [Display(Name = "教师Id")]
        public int TeacherId { get; set; }

        /// <summary>
        /// 教职工号
        /// </summary>
        [Display(Name = "教职工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// OrgId
        /// </summary>
        public int OrgId { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }

        /// <summary>
        /// 考勤时间
        /// </summary>
        [Display(Name = "考勤时间")]
        public DateTime CalendarDate { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "节次")]
        public string PeriodName { get; set; }
    }
}
