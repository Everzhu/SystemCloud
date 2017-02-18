using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考试类型
    /// </summary>
    public class tbExamType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考试类型
        /// </summary>
        [Required]
        [Display(Name = "考试类型")]
        public string ExamTypeName { get; set; }
    }
}