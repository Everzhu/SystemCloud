using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Dto.TeacherHonor
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 教师ID
        /// </summary>
        [Display(Name = "教师ID")]
        public int TeacherId { get; set; }

        /// <summary>
        /// 教师编号
        /// </summary>
        [Display(Name = "教师编号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 教师名称
        /// </summary>
        [Display(Name = "教师名称")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 荣誉名称
        /// </summary>
        [Display(Name = "荣誉名称")]
        public string HonorName { get; set; }

        /// <summary>
        /// 荣誉类型
        /// </summary>
        [Display(Name = "荣誉类型")]
        public string TeacherHonorTypeName { get; set; }

        /// <summary>
        /// 获奖级别
        /// </summary>
        [Display(Name = "获奖级别")]
        public string TeacherHonorLevelName { get; set; }

        /// <summary>
        /// 荣誉证书
        /// </summary>
        [Display(Name = "荣誉证书")]
        public string HonorFile { get; set; }
    }
}