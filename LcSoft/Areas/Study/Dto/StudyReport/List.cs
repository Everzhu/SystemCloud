using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyReport
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
        /// 班级Id
        /// </summary>
        [Display(Name = "班级Id")]
        public int ClassId { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称")]
        public string ClassName { get; set; }
        /// <summary>
        /// 异常人数
        /// </summary>
        [Display(Name = "异常人数")]
        public int StudentCount { get; set; }
    }
}