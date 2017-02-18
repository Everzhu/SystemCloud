﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Entity
{
    /// <summary>
    /// 评价班级
    /// </summary>
    public class tbPerformOrg : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价分组
        /// </summary>
        [Required]
        [Display(Name = "评价分组")]
        public virtual tbPerformGroup tbPerformGroup { get; set; }

        /// <summary>
        /// 评价科目
        /// </summary>
        [Required]
        [Display(Name = "评价科目")]
        public virtual Course.Entity.tbOrg tbOrg { get; set; }
    }
}