using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyData
{
    public class InputHor
    {
        public int Id { get; set; }
        /// <summary>
        /// 评教
        /// </summary>
        [Display(Name = "评教")]
        public int SurveyId { get; set; }
        /// <summary>
        /// 评教
        /// </summary>
        [Display(Name = "评教")]
        public string SurveyName { get; set; }
        /// <summary>
        /// 教师
        /// </summary>
        [Display(Name = "教师")]
        public int TeacherId { get; set; }
        /// <summary>
        /// 教师
        /// </summary>
        [Display(Name = "教师")]
        public string TeacherCode { get; set; }
        /// <summary>
        /// 教师
        /// </summary>
        [Display(Name = "教师")]
        public string TeacherName { get; set; }
        /// <summary>
        /// 学生
        /// </summary>
        [Display(Name = "学生")]
        public int StudentId { get; set; }
        /// <summary>
        /// 学生
        /// </summary>
        [Display(Name = "学生")]
        public string StudentCode { get; set; }
        /// <summary>
        /// 学生
        /// </summary>
        [Display(Name = "学生")]
        public string StudentName { get; set; }
        /// <summary>
        /// 问答结果
        /// </summary>
        [Display(Name = "问答结果")]
        public string SurveyText { get; set; }
        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public int OrgId { get; set; }
        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }
        /// <summary>
        /// 教学班排序
        /// </summary>
        [Display(Name = "教学班排序")]
        public int? OrgNo { get; set; }
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
        /// 行政班排序
        /// </summary>
        [Display(Name = "行政班排序")]
        public int? ClassNo { get; set; }
        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程")]
        public int CourseId { get; set; }
        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程")]
        public string CourseName { get; set; }
        /// <summary>
        /// 课程排序
        /// </summary>
        [Display(Name = "课程排序")]
        public int? CourseNo { get; set; }
        /// <summary>
        /// 评价分组
        /// </summary>
        [Display(Name = "评价分组")]
        public int SurveyGroupId { get; set; }
        /// <summary>
        /// 评价分组
        /// </summary>
        [Display(Name = "评价分组")]
        public string SurveyGroupName { get; set; }
        /// <summary>
        /// 是否任课教师
        /// </summary>
        [Display(Name = "是否任课教师")]
        public bool SurveyGroupIsOrg { get; set; }
        /// <summary>
        /// 评价分组排序
        /// </summary>
        [Display(Name = "评价分组排序")]
        public int? SurveyGroupNo { get; set; }
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
        /// 项目类型
        /// </summary>
        [Display(Name = "项目类型")]
        public Code.EnumHelper.SurveyItemType SurveyItemType { get; set; }
        /// <summary>
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目")]
        public int? SurveyItemNo { get; set; }
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
    }
}