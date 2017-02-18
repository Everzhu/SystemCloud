using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 获奖类型
    /// </summary>
    public class tbStudentHonorType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 获奖类型
        /// </summary>
        [Display(Name = "获奖类型"), Required]
        public string StudentHonorTypeName { get; set; }
    }
}