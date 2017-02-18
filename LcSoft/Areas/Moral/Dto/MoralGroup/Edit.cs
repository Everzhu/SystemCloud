using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralGroup
{
    public class Edit
    {
        public int Id { get; set; }

        [Display(Name ="排序")]
        public int? No { get; set; }

        [Display(Name ="评价组"),Required]
        public string MoralGroupName { get; set; }

        /// <summary>
        /// 评价
        /// </summary>
        [Display(Name ="德育"), Required]
        public int MoralId { get; set; }

    }
}