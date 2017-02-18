using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 获奖级别
    /// </summary>
    public class tbStudentHonorLevel : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 获奖级别
        /// </summary>
        [Display(Name = "获奖级别"), Required]
        public string StudentHonorLevelName { get; set; }
    }
}