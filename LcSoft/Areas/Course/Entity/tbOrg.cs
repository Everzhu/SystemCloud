using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Entity
{
    /// <summary>
    /// 教学班
    /// </summary>
    public class tbOrg : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教学班
        /// </summary>
        [Required]
        [Display(Name = "教学班")]
        public string OrgName { get; set; }

        /// <summary>
        /// 所属校区
        /// </summary>
        [Display(Name = "所属校区")]
        public virtual Basis.Entity.tbSchool tbSchool { get; set; }

        /// <summary>
        /// 班级模式
        /// </summary>
        [Required]
        [Display(Name = "班级模式")]
        public bool IsClass { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Required]
        [Display(Name = "学年")]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Required]
        [Display(Name = "课程")]
        public virtual tbCourse tbCourse { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Required]
        [Display(Name = "年级")]
        public virtual Basis.Entity.tbGrade tbGrade { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public virtual Basis.Entity.tbRoom tbRoom { get; set; }

        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public virtual Basis.Entity.tbClass tbClass { get; set; }

        /// <summary>
        /// 自动考勤
        /// </summary>
        [Required]
        [Display(Name = "自动考勤")]
        public bool IsAutoAttendance { get; set; }
    }
}