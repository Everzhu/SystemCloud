using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Attendance.Dto.Attendance
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 对应学生
        /// </summary>
        [Display(Name = "对应学生"), Required]
        public int StudentId { get; set; }

        public string StudentName { get; set; }

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
        /// 对应课程
        /// </summary>
        [Display(Name = "对应课程"), Required]
        public int CourseId { get; set; }
        public int OrgId { get; internal set; }
    }
}
