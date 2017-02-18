using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Course.Entity
{
    /// <summary>
    /// 教学班管理
    /// </summary>
    public class tbOrgManager : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教学班
        /// </summary>
        [Required]
        [Display(Name = "教学班")]
        public virtual tbOrg tbOrg { get; set; }

        /// <summary>
        /// 教师
        /// </summary>
        [Required]
        [Display(Name = "教师")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }
    }
}
