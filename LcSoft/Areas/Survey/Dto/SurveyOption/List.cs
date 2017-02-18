using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Survey.Dto.SurveyOption
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
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容")]
        public string SurveyItemName { get; set; }

        /// <summary>
        /// 评价选项
        /// </summary>
        [Display(Name = "评价选项")]
        public string OptionName { get; set; }

        /// <summary>
        /// 选项分值
        /// </summary>
        [Display(Name = "选项分值")]
        public decimal OptionValue { get; set; }
    }
}
