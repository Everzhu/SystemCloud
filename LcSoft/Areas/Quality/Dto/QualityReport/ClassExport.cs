using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Dto.QualityReport
{
    public class ClassExport
    {
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
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 积点
        /// </summary>
        [Display(Name = "积点")]
        public decimal LevleValue { get; set; }

        /// <summary>
        /// 星级
        /// </summary>
        [Display(Name = "星级")]
        public string LevelName { get; set; }
    }
}