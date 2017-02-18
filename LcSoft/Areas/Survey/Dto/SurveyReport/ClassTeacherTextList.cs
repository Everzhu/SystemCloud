using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyReport
{
    public class ClassTeacherTextList
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
        /// 班主任
        /// </summary>
        [Display(Name = "班主任")]
        public int TeacherId { get; set; }
        /// <summary>
        /// 班主任
        /// </summary>
        [Display(Name = "班主任")]
        public string TeacherName { get; set; }
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