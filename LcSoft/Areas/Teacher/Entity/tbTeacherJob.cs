using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Teacher.Entity
{
    /// <summary>
    /// 教师职务
    /// </summary>
    public class tbTeacherJob : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教师职务
        /// </summary>
        [Display(Name = "教师职务"), Required]
        public string TeacherJobName { get; set; }
    }
}
