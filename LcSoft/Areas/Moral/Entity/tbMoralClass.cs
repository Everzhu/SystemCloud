using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 参评班级
    /// </summary>
    public class tbMoralClass : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属德育
        /// </summary>
        [Required]
        [Display(Name = "所属德育")]
        public virtual tbMoral tbMoral { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Required]
        [Display(Name = "班级")]
        public virtual Basis.Entity.tbClass tbClass { get; set; }
    }
}