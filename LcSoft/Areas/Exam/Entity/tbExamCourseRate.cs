using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考试等级
    /// </summary>
    public class tbExamCourseRate : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 比例
        /// </summary>
        [Required]
        [Display(Name = "比例")]
        public decimal Rate { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Display(Name = "考试课程")]
        public virtual Entity.tbExamCourse tbExamCourse { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Display(Name = "考试课程")]
        public virtual Entity.tbExamCourse tbExamCourse1 { get; set; }
    }
}