using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Teacher.Entity
{
    /// <summary>
    /// 教师级别
    /// </summary>
    public class tbTeacherLevel : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教师级别
        /// </summary>
        [Display(Name = "教师级别"), Required]
        public string TeacherLevelName { get; set; }
    }
}
