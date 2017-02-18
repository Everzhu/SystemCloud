using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyClassStudent
{
    public class List
    {
        public int Id { get; set; }
        /// <summary>
        /// 晚自习Id
        /// </summary>
        [Display(Name = "晚自习Id")]
        public int StudyId { get; set; }
        /// <summary>
        /// 晚自习名称
        /// </summary>
        [Display(Name = "晚自习名称")]
        public string StudyName { get; set; }
        /// <summary>
        /// 班级ID
        /// </summary>
        [Display(Name = "班级ID")]
        public int ClassId { get; set; }

        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public string ClassName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生Id
        /// </summary>
        [Display(Name = "学生Id")]
        public int StudentId { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }
        /// <summary>
        /// 是否参加晚自习
        /// </summary>
        [Display(Name = "是否参加晚自习")]
        public bool IsApplyStudy { get; set; } = false;
    }
}