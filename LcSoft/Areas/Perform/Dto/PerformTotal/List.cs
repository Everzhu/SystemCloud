using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Dto.PerformTotal
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
        /// 对应评价
        /// </summary>
        [Display(Name = "评价名称")]
        public string PerformName { get; set; }
        /// <summary>
        /// 评价课程
        /// </summary>
        [Display(Name = "评价课程")]
        public string CourseName { get; set; }
        /// <summary>
        /// 评价学生
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }
        /// <summary>
        /// 评价总分
        /// </summary>
        [Display(Name = "评价总分")]
        public decimal TotalScore { get; set; }
        /// <summary>
        /// 学生Id
        /// </summary>
        [Display(Name = "学生Id")]
        public int StudentId { get; set; }
        /// <summary>
        /// 课程Id
        /// </summary>
        [Display(Name = "课程Id")]
        public int CourseId { get; set; }
        /// <summary>
        /// 评价Id
        /// </summary>
        [Display(Name = "评价Id")]
        public int PerformId { get; set; }
    }
}