using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Entity
{
    /// <summary>
    /// 课表
    /// </summary>
    public class tbOrgSchedule : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教学班
        /// </summary>
        [Required]
        [Display(Name = "教学班")]
        public virtual tbOrg tbOrg { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        [Required]
        [Display(Name = "星期")]
        public virtual Basis.Entity.tbWeek tbWeek { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Required]
        [Display(Name = "节次")]
        public virtual Basis.Entity.tbPeriod tbPeriod { get; set; }

        /// <summary>
        /// 单双周
        /// </summary>
        [Required]
        [Display(Name = "单双周")]
        public Code.EnumHelper.CourseScheduleType ScheduleType { get; set; }
    }
}