using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Teacher.Entity
{
    public class tbTeacherHonor : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属教师
        /// </summary>
        [Display(Name = "所属教师"), Required]
        public virtual tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 荣誉名称
        /// </summary>
        [Display(Name = "荣誉名称"), Required]
        public string HonorName { get; set; }

        /// <summary>
        /// 荣誉类型
        /// </summary>
        [Display(Name = "荣誉类型"), Required]
        public virtual tbTeacherHonorType tbTeacherHonorType { get; set; }

        /// <summary>
        /// 获奖级别
        /// </summary>
        [Display(Name = "获奖级别"), Required]
        public virtual tbTeacherHonorLevel tbTeacherHonorLevel { get; set; }

        /// <summary>
        /// 荣誉证书
        /// </summary>
        [Display(Name = "荣誉证书")]
        public string HonorFile { get; set; }
    }
}
