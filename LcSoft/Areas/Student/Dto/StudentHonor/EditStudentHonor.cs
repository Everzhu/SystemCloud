using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentHonor
{
    public class EditStudentHonor
    {
        public int Id { get; set; }

        /// <summary>
        /// 荣誉名称
        /// </summary>
        [Display(Name = "荣誉名称"), Required]
        public string HonorName { get; set; }

        /// <summary>
        /// 获奖级别
        /// </summary>
        [Display(Name = "获奖级别"), Required]
        public int StudentHonorLevelId { get; set; }

        /// <summary>
        /// 获奖类型
        /// </summary>
        [Display(Name = "获奖类型"), Required]
        public int StudentHonorTypeId { get; set; }

        /// <summary>
        /// 荣誉来源
        /// </summary>
        [Display(Name = "荣誉来源"), Required]
        public Code.EnumHelper.StudentHonorSource HonorSource { get; set; }

        /// <summary>
        /// 荣誉证书
        /// </summary>
        [Display(Name = "荣誉证书"), Required]
        public string HonorFile { get; set; }
    }
}