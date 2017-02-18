using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.Study
{
    public class Import
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public string No { get; set; }

        /// <summary>
        /// 晚自习名称
        /// </summary>
        [Display(Name = "晚自习名称")]
        public string StudyName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年")]
        public string YearName { get; set; }

        /// <summary>
        /// 晚自习模式（班级、教室）
        /// </summary>
        [Display(Name = "晚自习模式")]
        public string IsRoom { get; set; }

        /// <summary>
        /// 开放申请
        /// </summary>
        [Display(Name = "开放申请")]
        public string IsApply { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "申请开始时间")]
        public string ApplyFrom { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "申请结束时间")]
        public string ApplyTo { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}