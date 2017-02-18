using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Entity
{
    public class tbElectiveOrgClass : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属选课开班
        /// </summary>
        [Required]
        [Display(Name = "所属选课开班")]
        public virtual tbElectiveOrg tbElectiveOrg { get; set; }

        /// <summary>
        /// 选课班级
        /// </summary>
        [Required]
        [Display(Name = "选课班级")]
        public virtual Basis.Entity.tbClass tbClass { get; set; }

        /// <summary>
        /// 最大限度（设置每个班的可选名额）
        /// </summary>
        [Required]
        [Display(Name = "最大限度")]
        public int MaxLimit { get; set; }

        /// <summary>
        /// 剩余名额
        /// </summary>
        [Required]
        [Display(Name = "剩余名额")]
        public int RemainCount { get; set; }
    }
}