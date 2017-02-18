using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 学届
    /// </summary>
    public class tbStudentSession : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学生类型
        /// </summary>
        [Display(Name = "学生类型"), Required]
        public string StudentSessionName { get; set; }
    }
}