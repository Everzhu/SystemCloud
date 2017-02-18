﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Entity
{
    /// <summary>
    /// 评价课程
    /// </summary>
    public class tbPerformCourse : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价分组
        /// </summary>
        [Required]
        [Display(Name = "评价分组")]
        public virtual tbPerformGroup tbPerformGroup { get; set; }

        /// <summary>
        /// 评价课程
        /// </summary>
        [Required]
        [Display(Name = "评价课程")]
        public virtual Course.Entity.tbCourse tbCourse { get; set; }
    }
}