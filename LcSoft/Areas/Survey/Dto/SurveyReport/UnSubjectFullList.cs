﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyReport
{
    public class UnSubjectFullList
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
        /// 打分学生
        /// </summary>
        [Display(Name = "学生")]
        public int StudentId { get; set; }
        /// <summary>
        /// 打分学生
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }
        /// <summary>
        /// 打分学生
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }
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
        /// 科目序号
        /// </summary>
        [Display(Name = "科目序号")]
        public int? SubjectNo { get; set; }
        /// <summary>
        /// 课程序号
        /// </summary>
        [Display(Name = "课程序号")]
        public int? CourseNo { get; set; }
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
    }
}