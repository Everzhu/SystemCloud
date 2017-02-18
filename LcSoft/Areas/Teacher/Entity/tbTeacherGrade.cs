using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Entity
{
    /// <summary>
    /// 年级组长
    /// </summary>
    public class tbTeacherGrade : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级"), Required]
        public virtual Basis.Entity.tbGrade tbGrade { get; set; }

        /// <summary>
        /// 年级组长
        /// </summary>
        [Display(Name = "年级组长"), Required]
        public virtual tbTeacher tbTeacher { get; set; }
    }
}