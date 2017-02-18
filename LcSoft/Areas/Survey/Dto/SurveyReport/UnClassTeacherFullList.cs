using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyReport
{
    public class UnClassTeacherFullList
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
        [Display(Name = "行政班")]
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
        [Display(Name = "学号")]
        public string StudentCode { get; set; }
        /// <summary>
        /// 打分学生
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }
    }
}