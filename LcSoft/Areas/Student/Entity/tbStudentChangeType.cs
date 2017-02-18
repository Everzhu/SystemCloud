using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 学生调动类型
    /// </summary>
    public class tbStudentChangeType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学生调动类型名称
        /// </summary>
        [Display(Name = "学生调动类型名称"), Required]
        public string StudentChangeTypeName { get; set; }

        /// <summary>
        /// 异动类型
        /// </summary>
        [Display(Name = "异动类型")]
        public Code.EnumHelper.StudentChangeType StudentChangeType { get; set; }
    }
}