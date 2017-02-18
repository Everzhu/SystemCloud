using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Dto.PerformDataDay
{
    public class InputMultipleEdit
    {
        public int Id { get; set; }
        /// <summary>
        /// 对应评价
        /// </summary>
        [Display(Name = "对应评价")]
        public int PerformId { get; set; }
        /// <summary>
        /// 对应评价
        /// </summary>
        [Display(Name = "对应评价")]
        public string PerformName { get; set; }
        /// <summary>
        /// 评价课程
        /// </summary>
        [Display(Name = "评价课程")]
        public int CourseId { get; set; }
        /// <summary>
        /// 评价课程
        /// </summary>
        [Display(Name = "评价课程")]
        public string CourseName { get; set; }
        /// <summary>
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目")]
        public int PerformItemId { get; set; }
        /// <summary>
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目")]
        public string PerformItemName { get; set; }
        /// <summary>
        /// 评价选项
        /// </summary>
        [Display(Name = "评价选项")]
        public int PerformOptionId { get; set; }
        /// <summary>
        /// 评价选项
        /// </summary>
        [Display(Name = "评价选项")]
        public string PerformOptionName { get; set; }
        /// <summary>
        /// 选项分值
        /// </summary>
        [Display(Name = "选项分值")]
        public decimal PerformOptionValue { get; set; }
        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "座位号")]
        public string StudentNo { get; set; }
        /// <summary>
        /// 学生Id
        /// </summary>
        [Display(Name = "学生Id")]
        public int StudentId { get; set; }
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
        /// 评价分数
        /// </summary>
        [Display(Name = "评价分数")]
        public decimal Score { get; set; }
        /// <summary>
        /// 评价总分
        /// </summary>
        [Display(Name = "评价总分")]
        public decimal TotalScore { get; set; }
        /// <summary>
        /// 录入时间
        /// </summary>
        [Display(Name = "录入时间")]
        public DateTime InputDate { get; set; }
        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "录入人员")]
        public int SysUserId { get; set; }
        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "录入人员")]
        public string SysUserName { get; set; }
    }
}