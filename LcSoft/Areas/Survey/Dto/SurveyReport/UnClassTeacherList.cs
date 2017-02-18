﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyReport
{
    public class UnClassTeacherList
    {
        public int Id { get; set; }
        /// <summary>
        /// 所属评教
        /// </summary>
        [Display(Name = "所属评教")]
        public int SurveyId { get; set; }
        /// <summary>
        /// 所属评教
        /// </summary>
        [Display(Name = "所属评教")]
        public string SurveyName { get; set; }
        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public int ClassId { get; set; }
        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public string ClassName { get; set; }
        /// <summary>
        /// 行政班序号
        /// </summary>
        [Display(Name = "行政班序号")]
        public int? ClassNo { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public int GradeId { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }
        /// <summary>
        /// 年级序号
        /// </summary>
        [Display(Name = "年级序号")]
        public int? GradeNo { get; set; }        
        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "班主任")]
        public int TeacherId { get; set; }
        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "班主任")]
        public string TeacherName { get; set; }
        /// <summary>
        /// 打分学生
        /// </summary>
        [Display(Name = "学生")]
        public int StudentId { get; set; }
        /// <summary>
        /// 打分学生
        /// </summary>
        [Display(Name = "学生")]
        public string StudentName { get; set; }        
        /// <summary>
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目")]
        public int SurveyItemId { get; set; }
        /// <summary>
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目")]
        public string SurveyItemName { get; set; }
        /// <summary>
        /// 评价选项
        /// </summary>
        [Display(Name = "评价选项")]
        public int SurveyOptionId { get; set; }
        /// <summary>
        /// 评价选项
        /// </summary>
        [Display(Name = "评价选项")]
        public string SurveyOptionName { get; set; }
        /// <summary>
        /// 评价分值
        /// </summary>
        [Display(Name = "评价分值")]
        public decimal SurveyOptionValue { get; set; }
        /// <summary>
        /// 选择此项数量
        /// </summary>
        [Display(Name = "选择此项数量")]
        public int SurveyOptionCount { get; set; }
        /// <summary>
        /// 选择此项总分
        /// </summary>
        [Display(Name = "选择此项总分")]
        public decimal SurveyOptionSum { get; set; }
        /// <summary>
        /// 获取项平均分
        /// </summary>
        [Display(Name = "获取项平均分")]
        public decimal SurveyOptionAvg { get; set; }
        /// <summary>
        /// 参评总人数
        /// </summary>
        [Display(Name = "参评总人数")]
        public int SurveyAllCount { get; set; }
        /// <summary>
        /// 项平均得分
        /// </summary>
        [Display(Name = "项平均得分")]
        public decimal SurveyAllAvg { get; set; }
        /// <summary>
        /// 已评人数
        /// </summary>
        [Display(Name = "已评人数")]
        public int SurveyCount { get; set; }
        /// <summary>
        /// 未评人数
        /// </summary>
        [Display(Name = "未评人数")]
        public int UnSurveyCount { get; set; }
    }
}