using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Entity
{
    public class tbElectiveOrgStudent : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属选课开班
        /// </summary>
        [Required]
        [Display(Name = "所属选课开班")]
        public virtual tbElectiveOrg tbElectiveOrg { get; set; }

        /// <summary>
        /// 预选学生
        /// </summary>
        [Required]
        [Display(Name = "预选学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 允许学生修改
        /// </summary>
        [Required]
        [Display(Name = "允许学生修改")]
        public bool IsFixed { get; set; }

        /// <summary>
        /// 已选上
        /// </summary>
        [Required]
        [Display(Name = "已选上")]
        public bool IsChecked { get; set; }
    }
}