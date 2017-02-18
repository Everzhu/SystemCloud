using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Entity
{
    /// <summary>
    /// 课程
    /// </summary>
    public class tbCourse : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 课程
        /// </summary>
        [Required]
        [Display(Name = "课程")]
        public string CourseName { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        [Display(Name = "英文名")]
        public string CourseNameEn { get; set; }

        /// <summary>
        /// 课程编码
        /// </summary>
        [Display(Name = "课程编码")]
        public string CourseCode { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Required]
        [Display(Name = "科目")]
        public virtual tbSubject tbSubject { get; set; }

        /// <summary>
        /// 课程类型
        /// </summary>
        [Required]
        [Display(Name = "课程类型")]
        public virtual tbCourseType tbCourseType { get; set; }

        /// <summary>
        /// 课程领域
        /// </summary>
        [Display(Name = "课程领域")]
        public virtual tbCourseDomain tbCourseDomain { get; set; }

        /// <summary>
        /// 课程分组
        /// </summary>
        [Display(Name = "课程分组")]
        public virtual tbCourseGroup tbCourseGroup { get; set; }

        /// <summary>
        /// 学分
        /// </summary>
        [Required]
        [Display(Name = "学分")]
        public decimal Point { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        [Required]
        [Display(Name = "课时")]
        public int Hour { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [Required]
        [Display(Name = "是否等级录入")]
        public bool IsLevel { get; set; }
    }
}