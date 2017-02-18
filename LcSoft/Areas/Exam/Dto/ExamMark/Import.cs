using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Dto.ExamMark
{
    public class Import
    {
        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        [Display(Name = "唯一标识")]
        public string NameTypeNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程")]
        public string CourseName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public string ExamStatus { get; set; }

        /// <summary>
        /// 平时分
        /// </summary>
        [Display(Name = "平时分")]
        public decimal? AppraiseMark { get; set; }

        /// <summary>
        /// 考试成绩
        /// </summary>
        [Display(Name = "考试成绩")]
        public decimal? TotalMark { get; set; }

        /// <summary>
        /// 过程分
        /// </summary>
        [Display(Name = "过程分")]
        public decimal? SegmentMark { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [Display(Name = "等级")]
        public string LevelName { get; set; }

        /// <summary>
        /// 错误提示
        /// </summary>
        [Display(Name = "错误提示")]
        public string Error { get; set; }
    }
}