using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Dto.QualitySummary
{
    public class List
    {
        public int ClassId { get; set; }

        public int TeacherId { get; set; }

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
        /// 班主任
        /// </summary>
        [Display(Name = "班主任")]
        public string ClassTeacherName { get; set; }

        /// <summary>
        /// 班级人数
        /// </summary>
        [Display(Name = "班级人数")]
        public int ClassStudentCount { get; set; }

        /// <summary>
        /// 自评
        /// </summary>
        [Display(Name = "自评")]
        public string QualitySelf { get; set; }

        /// <summary>
        /// 学期期待
        /// </summary>
        [Display(Name = "学期期待")]
        public string QualityPlan { get; set; }

        /// <summary>
        /// 学期总结
        /// </summary>
        [Display(Name = "学期总结")]
        public string QualitySummary { get; set; }

        /// <summary>
        /// 评价评语
        /// </summary>
        [Display(Name = "评价评语")]
        public string QualityComment { get; set; }

        /// <summary>
        /// 同学评价
        /// </summary>
        [Display(Name = "同学评价")]
        public string QualityStudent { get; set; }
    }
}