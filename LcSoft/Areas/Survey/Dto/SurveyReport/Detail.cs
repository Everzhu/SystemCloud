using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyReport
{
    public class Detail
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
        [Display(Name = "参评班级")]
        public bool IsClass { get; set; }
        /// <summary>
        /// 参评班级
        /// </summary>
        [Display(Name = "参评班级")]
        public int ClassId { get; set; }
        /// <summary>
        /// 参评班级
        /// </summary>
        [Display(Name = "参评班级")]
        public string ClassName { get; set; }
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
        /// 班主任
        /// </summary>
        [Display(Name = "教师姓名")]
        public int TeacherId { get; set; }
        /// <summary>
        /// 班主任
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 班主任
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "任课教师")]
        public string ClassTeacherName { get; set; }
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
        /// 是否已选
        /// </summary>
        [Display(Name = "是否已选")]
        public bool IsSelected { get; set; }
    }
}