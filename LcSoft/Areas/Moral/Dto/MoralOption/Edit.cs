using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralOption
{
    public class Edit
    {
        public int Id { get; set; }

        [Display(Name ="排序")]
        public int? No { get; set; }

        /// <summary>
        /// 选项名称
        /// </summary>
        [Display(Name="选项名称"),Required]
        public string MoralOptionName { get; set; }

        /// <summary>
        /// 分值
        /// </summary>
        [Display(Name ="分值"),Required]
        [RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "分值只能为正整数或浮点数")]
        public decimal MoralOptionValue { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        [Display(Name ="所属项目"),Required]
        public int tbMoralItemId  { get; set; }


    }
}