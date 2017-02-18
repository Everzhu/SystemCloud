using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyData
{
    public class Import
    {
        /// <summary>
        /// 晚自习名称
        /// </summary>
        [Display(Name = "晚自习名称")]
        public string StudyName { get; set; }
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
        /// 自习日期
        /// </summary>
        [Display(Name = "自习日期")]
        public string InputDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 表现名称
        /// </summary>
        [Display(Name = "表现名称")]
        public string StudyOptionName { get; set; }
        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}