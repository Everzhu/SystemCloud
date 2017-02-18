using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Survey.Dto.Survey
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 评价名称
        /// </summary>
        [Display(Name = "评价名称")]
        public string SurveyName { get; set; }

        /// <summary>
        /// 学段
        /// </summary>
        [Display(Name = "学段")]
        public string YearSectionName { get; set; }

        /// <summary>
        /// 评价开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// 评价结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public bool IsOpen { get; set; }
    }
}
