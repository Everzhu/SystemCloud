using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Dto.QualityReport
{
    public class ClassStudent
    {
        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string Comment { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Display(Name = "录入时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        [Display(Name = "录入人")]
        public string UserName { get; set; }

        /// <summary>
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目")]
        public string QualityItemName { get; set; }

        /// <summary>
        /// 评价结果
        /// </summary>
        [Display(Name = "评价结果")]
        public decimal OptionAvg { get; set; }

        /// <summary>
        /// 荣誉名称
        /// </summary>
        [Display(Name = "荣誉名称")]
        public string HonorName { get; set; }

        /// <summary>
        /// 荣誉级别
        /// </summary>
        [Display(Name = "荣誉级别")]
        public string StudentHonorLevelName { get; set; }

        /// <summary>
        /// 荣誉类型
        /// </summary>
        [Display(Name = "荣誉类型")]
        public string StudentHonorTypeName { get; set; }

        /// <summary>
        /// 领域ID
        /// </summary>
        public int CourseDemainId { get; set; }

        /// <summary>
        /// 课程ID
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// 领域
        /// </summary>
        public string CourseDemainName { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// 考试ID
        /// </summary>
        public int ExamId { get; set; }

        /// <summary>
        /// 学生ID
        /// </summary>
        public int StudentId { get; set; }

        [Display(Name = "成绩")]
        public decimal? TotalMark { get; set; }

        [Display(Name = "评价")]
        public string LevelName { get; set; }

        [Display(Name = "积分")]
        public decimal LevelScore { get; set; }

        [Display(Name = "年级成绩排名")]
        public string TotalGradeRank { get; set; }
    }
}