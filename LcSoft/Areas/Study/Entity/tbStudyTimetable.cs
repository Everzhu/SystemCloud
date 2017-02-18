using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Entity
{
    /// <summary>
    /// 晚自习值班记录
    /// </summary>
    public class tbStudyTimetable : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教师
        /// </summary>
        [Display(Name = "教师"), Required]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级")]
        public virtual Basis.Entity.tbClass tbClass { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public virtual Basis.Entity.tbRoom tbRoom { get; set; }

        /// <summary>
        /// 录入日期
        /// </summary>
        [Display(Name = "录入日期"), Required]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 录入人员(自动生成时为NULL)
        /// </summary>
        [Display(Name = "录入人员")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
    }
}