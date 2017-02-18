using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Quality.Dto.QualityItemGroup
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
        /// 评价分组
        /// </summary>
        [Display(Name = "评价分组")]
        public string QualityItemGroupName { get; set; }

        /// <summary>
        /// 评价
        /// </summary>
        [Display(Name = "评价")]
        public string QualityName { get; set; }
    }
}
