using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Student.Dto.StudentHonor
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 荣誉名称
        /// </summary>
        [Display(Name = "荣誉名称")]
        public string HonorName { get; set; }

        /// <summary>
        /// 获奖级别
        /// </summary>
        [Display(Name = "获奖级别")]
        public string StudentHonorLevelName { get; set; }

        /// <summary>
        /// 荣誉类型
        /// </summary>
        [Display(Name = "荣誉类型")]
        public string StudentHonorTypeName { get; set; }

        /// <summary>
        /// 荣誉来源
        /// </summary>
        [Display(Name = "荣誉来源")]
        public Code.EnumHelper.StudentHonorSource HonorSource { get; set; }

        /// <summary>
        /// 荣誉证书
        /// </summary>
        [Display(Name = "荣誉证书")]
        public string HonorFile { get; set; }
    }
}
