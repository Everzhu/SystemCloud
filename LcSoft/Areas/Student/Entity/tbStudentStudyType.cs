using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 就读方式
    /// </summary>
    public class tbStudentStudyType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 就读方式
        /// </summary>
        [Display(Name = "就读方式"), Required]
        public string StudyTypeName { get; set; }
    }
}