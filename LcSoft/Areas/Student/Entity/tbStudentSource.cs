using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 学生来源
    /// </summary>
    public class tbStudentSource : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学生来源
        /// </summary>
        [Display(Name = "学生来源"), Required]
        public string StudentSourceName { get; set; }
    }
}