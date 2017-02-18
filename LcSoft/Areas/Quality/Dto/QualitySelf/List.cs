using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Dto.QualitySelf
{
    public class List
    {
        public int Id { get; set; }

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
        /// 自评
        /// </summary>
        [Display(Name = "自评")]
        public string SelfName { get; set; }

        /// <summary>
        /// 学期期待
        /// </summary>
        [Display(Name = "学期期待")]
        public string PlanName { get; set; }

        /// <summary>
        /// 学期总结
        /// </summary>
        [Display(Name = "学期总结")]
        public string SummaryName { get; set; }
    }
}