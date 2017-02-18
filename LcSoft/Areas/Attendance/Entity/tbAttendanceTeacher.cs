using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Attendance.Entity
{
    /// <summary>
    /// 考勤录入情况
    /// </summary>
    public class tbAttendanceTeacher : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教学班
        /// </summary>
        [Required]
        [Display(Name = "教学班")]
        public virtual Course.Entity.tbOrg tbOrg { get; set; }

        /// <summary>
        /// 录入教师
        /// </summary>
        [Required]
        [Display(Name = "录入教师")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }
        
        /// <summary>
        /// 考勤日期
        /// </summary>
        [Required]
        [Display(Name = "考勤日期")]
        public DateTime AttendanceDate { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Required]
        [Display(Name = "录入时间")]
        public DateTime InputDate { get; set; }
    }
}
