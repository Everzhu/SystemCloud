using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Quality.Dto.QualityItem
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
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容")]
        [Required]
        public string QualityItemName { get; set; }

        /// <summary>
        /// 评价类型
        /// </summary>
        [Display(Name = "评价类型")]
        [Required]
        public Code.EnumHelper.QualityItemType QualityItemType { get; set; }

        /// <summary>
        /// 是否纵向
        /// </summary>
        [Display(Name = "是否纵向")]
        public bool IsVertical { get; set; }

        /// <summary>
        /// 所属分组
        /// </summary>
        [Display(Name = "所属分组"), Required]
        public int QualityItemGroupId { get; set; }
    }
}
