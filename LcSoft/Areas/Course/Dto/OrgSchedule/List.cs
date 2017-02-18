using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Course.Dto.OrgSchedule
{
    public class List
    {
        public int Id { get; set; }

        [Display(Name = "教学班Id")]
        public int OrgId { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        [Display(Name = "星期")]
        public int WeekId { get; set; }

        /// <summary>
        /// 单双周
        /// </summary>
        [Display(Name = "单双周")]
        public Code.EnumHelper.CourseScheduleType ScheduleType { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "节次")]
        public int PeriodId { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        [Display(Name = "课程名称")]
        public string CourseName { get; set; }

        /// <summary>
        /// 节次名称
        /// </summary>
        [Display(Name = "节次名称")]
        public string PeriodName { get; set; }

        [Display(Name = "课程Id")]
        public int CourseId { get; set; }

        /// <summary>
        /// 是否录入考勤数据
        /// </summary>
        public bool IsAttendance { get; set; } = false;
    }
}
