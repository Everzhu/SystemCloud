using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Entity
{
    /// <summary>
    /// 科组长
    /// </summary>
    public class tbTeacherSubject : Code.EntityHelper.EntityBase
    {

        /// <summary>
        /// 科组
        /// </summary>
        [Display(Name = "科组"), Required]
        public virtual Course.Entity.tbSubject tbSubject { get; set; }

        /// <summary>
        /// 科组长
        /// </summary>
        [Display(Name = "科组长"), Required]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级"), Required]
        public virtual Basis.Entity.tbGrade tbGrade { get; set; }
    }
}