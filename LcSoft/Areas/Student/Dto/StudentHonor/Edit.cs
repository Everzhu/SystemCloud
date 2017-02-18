using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Student.Dto.StudentHonor
{
    public class Edit
    {
        /// <summary>
        /// Id
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 用于数据存入数据库前标识数据的唯一性
        /// </summary>
        public Guid? guid { get; set; }

        /// <summary>
        /// 所属学生
        /// </summary>
        [Display(Name = "所属学生"), Required]
        public int StudentId { get; set; }

        /// <summary>
        /// 荣誉名称
        /// </summary>
        [Display(Name = "荣誉名称"), Required]
        public string HonorName { get; set; }

        /// <summary>
        /// 获奖学年
        /// </summary>
        [Display(Name = "获奖学年")]
        public int? YearId { get; set; }

        /// <summary>
        /// 获奖级别
        /// </summary>
        [Display(Name = "获奖级别"), Required]
        public string StudentHonorLevelId { get; set; }

        /// <summary>
        /// 获奖类型
        /// </summary>
        [Display(Name = "获奖类型"), Required]
        public string StudentHonorTypeId { get; set; }

        /// <summary>
        /// 获奖来源
        /// </summary>
        [Display(Name = "获奖来源"), Required]
        public Code.EnumHelper.StudentHonorSource HonorSource { get; set; }

        /// <summary>
        /// 荣誉证书
        /// </summary>
        [Display(Name = "荣誉证书")]
        public string HonorFile { get; set; }
    }
}
