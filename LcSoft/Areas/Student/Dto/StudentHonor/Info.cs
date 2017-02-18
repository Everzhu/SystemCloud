using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentHonor
{
    public class Info
    {
        public int? Id { get; set; }

        /// <summary>
        /// 荣誉名称
        /// </summary>
        [Display(Name = "荣誉名称"), Required]
        public string HonorName { get; set; }

        /// <summary>
        /// 获奖级别
        /// </summary>
        [Display(Name = "获奖级别"), Required]
        public string StudentHonorLevel { get; set; }

        /// <summary>
        /// 获奖类型
        /// </summary>
        [Display(Name = "获奖类型"), Required]
        public string StudentHonorType { get; set; }

        /// <summary>
        /// 荣誉证书
        /// </summary>
        [Display(Name = "荣誉证书")]
        public string HonorFile { get; set; }
    }
}