using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Quality.Dto.QualityItem
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
        public string QualityItemName { get; set; }

        /// <summary>
        /// 评价分组
        /// </summary>
        [Display(Name = "评价分组")]
        public string QualityItemGroupName { get; set; }

        /// <summary>
        /// 评价类型
        /// </summary>
        [Display(Name = "评价类型")]
        public Code.EnumHelper.QualityItemType QualityItemType { get; set; }

        /// <summary>
        /// 选项布局
        /// </summary>
        [Display(Name = "选项布局")]
        public bool IsVertical { get; set; }

        /// <summary>
        /// 评价选项Id
        /// </summary>
        [Display(Name = "评价选项Id")]
        public string QualityOptionIds { get; set; }

        /// <summary>
        /// 评价选项
        /// </summary>
        [Display(Name = "评价选项")]
        public string QualityOptionNames { get; set; }
    }
}
