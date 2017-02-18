using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentHonor
{
    public class HonorEdit
    {
        public int? Id { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号"), Required]
        public string StudentCode { get; set; }

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
        /// 荣誉证书
        /// </summary>
        [Display(Name = "荣誉证书")]
        public string HonorFile { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        [Display(Name = "审批意见")]
        public string CheckRemark { get; set; }
    }
}