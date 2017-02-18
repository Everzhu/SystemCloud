using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyReport
{
    public class SubjectTextList
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
        /// 参评班级
        /// </summary>
        [Display(Name = "教学班")]
        public int OrgId { get; set; }
        /// <summary>
        /// 参评班级
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }
        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "任课教师")]
        public int TeacherId { get; set; }
        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "任课教师")]
        public string TeacherName { get; set; }
        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目")]
        public int SubjectId { get; set; }
        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目")]
        public string SubjectName { get; set; }
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
        /// 问答结果
        /// </summary>
        [Display(Name = "问答结果")]
        public string SurveyText { get; set; }
    }
}