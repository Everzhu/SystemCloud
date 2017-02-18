using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Teacher.Entity
{
    /// <summary>
    /// 教师类型
    /// </summary>
    public class tbTeacherType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教师类型
        /// </summary>
        [Display(Name = "教师类型"), Required]
        public string TeacherTypeName { get; set; }
    }
}
