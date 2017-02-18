using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Course.Entity
{
    /// <summary>
    /// 教学班日历
    /// </summary>
    public class tbOrgCalendar : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教学班
        /// </summary>
        [Required]
        [Display(Name = "教学班")]
        public virtual tbOrg tbOrg { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Required]
        [Display(Name = "任课教师")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 日历
        /// </summary>
        [Required]
        [Display(Name = "日历")]
        public DateTime CalendarDate { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Required]
        [Display(Name = "节次")]
        public virtual Basis.Entity.tbPeriod tbPeriod { get; set; }
    }
}
