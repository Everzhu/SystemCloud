using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Entity
{
    /// <summary>
    /// 选课规则
    /// </summary>
    public class tbElectiveRule : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属选课
        /// </summary>
        [Required]
        [Display(Name = "所属选课")]
        public virtual tbElective tbElective { get; set; }

        /// <summary>
        /// 所属课程
        /// </summary>
        [Required]
        [Display(Name = "所属课程")]
        public virtual Course.Entity.tbCourse tbCourse { get; set; }

        /// <summary>
        /// 规则课程
        /// </summary>
        [Required]
        [Display(Name = "规则课程")]
        public virtual Course.Entity.tbCourse tbCourseTarget { get; set; }

        /// <summary>
        /// 选课规则
        /// </summary>
        [Required]
        [Display(Name = "选课规则")]
        public Code.EnumHelper.ElectiveRule ElectiveRule { get; set; }
    }
}