using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考生状态
    /// </summary>
    public class tbExamStatus : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考生状态
        /// </summary>
        [Required]
        [Display(Name = "考生状态")]
        public string ExamStatusName { get; set; }
    }
}