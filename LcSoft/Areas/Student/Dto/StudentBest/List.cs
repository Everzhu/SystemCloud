using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentBest
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称")]
        public string ClassName { get; set; }

        /// <summary>
        /// 班级类型
        /// </summary>
        [Display(Name = "班级类型")]
        public string ClassTypeName { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        [Display(Name = "人数")]
        public int StudentCount { get; set; }

        /// <summary>
        /// 优生人数
        /// </summary>
        [Display(Name = "优生人数")]
        public int BestStudentCount { get; set; }

        /// <summary>
        /// 优生比
        /// </summary>
        [Display(Name = "优生比")]
        public string PercentAge { get; set; }
    }
}