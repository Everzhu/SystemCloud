using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 学生类型
    /// </summary>
    public class tbStudentType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学生类型
        /// </summary>
        [Display(Name = "学生类型"), Required]
        public string StudentTypeName { get; set; }
    }
}