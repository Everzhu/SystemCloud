using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考试
    /// </summary>
    public class tbExamAppraiseMark : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学生
        /// </summary>
        [Display(Name = "学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程")]
        public virtual Course.Entity.tbCourse tbCourse { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年")]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 考勤分
        /// </summary>
        [Display(Name = "考勤分")]
        public decimal? Mark1 { get; set; }

        /// <summary>
        /// 过程分
        /// </summary>
        [Display(Name = "过程分")]
        public decimal? Mark2 { get; set; }
    }
}