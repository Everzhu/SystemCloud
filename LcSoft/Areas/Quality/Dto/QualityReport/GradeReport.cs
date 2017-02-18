using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Dto.QualityReport
{
    public class GradeReport
    {
        /// <summary>
        /// 班级ID
        /// </summary>
        [Display(Name = "班级ID")]
        public int ClassId { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级")]
        public string ClassName { get; set; }

        /// <summary>
        /// 学生ID
        /// </summary>
        [Display(Name = "学生ID")]
        public string StudentId { get; set; }

        /// <summary>
        /// 积点
        /// </summary>
        [Display(Name = "积点")]
        public decimal LevleValue { get; set; }
    }
}