using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Entity
{
    /// <summary>
    /// 晚自习教师
    /// </summary>
    public class tbStudyRoomTeacher : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 晚自习
        /// </summary>
        [Display(Name = "晚自习"), Required]
        public virtual tbStudy tbStudy { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级"), Required]
        public virtual Basis.Entity.tbRoom tbRoom { get; set; }

        /// <summary>
        /// 教师
        /// </summary>
        [Display(Name = "教师"), Required]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 主要责任人
        /// </summary>
        [Display(Name = "主要责任人"), Required]
        public bool IsMaster { get; set; }
        /// <summary>
        /// 星期
        /// </summary>
        [Display(Name = "星期"), Required]
        public virtual Basis.Entity.tbWeek tbWeek { get; set; }
    }
}