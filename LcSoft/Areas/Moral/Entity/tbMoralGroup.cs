using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    public class tbMoralGroup : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价组
        /// </summary>
        [Required]
        [Display(Name = "评价组")]
        public string MoralGroupName { get; set; }

        /// <summary>
        /// 评价
        /// </summary>
        [Required]
        [Display(Name = "评价")]
        public virtual tbMoral tbMoral { get; set; }
    }
}