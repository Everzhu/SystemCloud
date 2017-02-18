using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Survey.Dto.Survey
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 评教名称
        /// </summary>
        [Display(Name = "评教名称"), Required]
        public string SurveyName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年"), Required]
        public int YearId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态"), Required]
        public bool IsOpen { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间"), Required]
        public DateTime FromDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间"), Required]
        public DateTime ToDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }
    }
}
