using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 成绩权限
    /// </summary>
    public class tbExamPower : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属考试
        /// </summary>
        [Required]
        [Display(Name = "所属考试")]
        public virtual tbExamCourse tbExamCourse { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Required]
        [Display(Name = "录入人员")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 是否任课教师
        /// </summary>
        [Required]
        [Display(Name = "是否任课教师")]
        public bool IsOrgTeacher { get; set; }
    }
}