using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyReport
{
    public class ClassList
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
        [Display(Name = "行政班")]
        public int ClassId { get; set; }
        /// <summary>
        /// 参评班级
        /// </summary>
        [Display(Name = "行政班")]
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
        [Display(Name = "班主任")]
        public int TeacherId { get; set; }
        /// <summary>
        /// 班主任
        /// </summary>
        [Display(Name = "班主任")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 班主任
        /// </summary>
        [Display(Name = "班主任")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "任课教师")]
        public string ClassTeacherName { get; set; }
        /// <summary>
        /// 全部人数
        /// </summary>
        [Display(Name = "全部人数")]
        public int StudentAllCount { get; set; }
        /// <summary>
        /// 已选人数
        /// </summary>
        [Display(Name = "已评人数")]
        public int SelectedCount { get; set; }
        /// <summary>
        /// 未选人数
        /// </summary>
        [Display(Name = "未评人数")]
        public int UnSelectedCount { get; set; }

    }

    public class StudentList
    {
        public int OrgId { get; set; }

        public int StudentId { get; set; }
    }
}