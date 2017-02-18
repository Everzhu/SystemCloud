using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyData
{
    public class Info
    {
        public int Id { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        [Display(Name = "总数")]
        public int TotalCount { get; set; }
        /// <summary>
        /// 评教
        /// </summary>
        [Display(Name = "评教")]
        public int SurveyId { get; set; }
        /// <summary>
        /// 教师
        /// </summary>
        [Display(Name = "教师")]
        public int TeacherId { get; set; }
        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public int OrgId { get; set; }
        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public int ClassId { get; set; }
        /// <summary>
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目")]
        public int SurveyItemId { get; set; }
        /// <summary>
        /// 项目类型
        /// </summary>
        [Display(Name = "项目类型")]
        public Code.EnumHelper.SurveyItemType SurveyItemType { get; set; }
        /// <summary>
        /// 评价选项
        /// </summary>
        [Display(Name = "评价选项")]
        public int SurveyOptionId { get; set; }
    }
}