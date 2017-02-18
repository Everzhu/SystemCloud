﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Dto.PerformData
{
    public class StudentDetail
    {
        /// <summary>
        /// 评价学生
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目")]
        public string PerformItemName { get; set; }

        /// <summary>
        /// 评价分数
        /// </summary>
        [Display(Name = "评价分数")]
        public decimal Score { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Display(Name = "录入时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "录入人员")]
        public string SysUserName { get; set; }

        [Display(Name = "分数集合")]
        public List<Dto.PerformItem.List> PerformItemList { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "座位号")]
        public string No { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生Id
        /// </summary>
        [Display(Name = "学生Id")]
        public int StudentId { get; set; }

        /// <summary>
        /// 学生Id
        /// </summary>
        [Display(Name = "学生Id")]
        public int PerformItemId { get; set; }

        public int PerformId { get; set; }

        /// <summary>
        /// 学生Id
        /// </summary>
        [Display(Name = "学生Id")]
        public int CourseId { get; set; }

        public string CourseName { get; set; }
    }
}