using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 德育扣分原因 
    /// </summary>
    public class tbMoralDataReason: Code.EntityHelper.EntityBase
    {

        /// <summary>
        /// 评分原因
        /// </summary>
        [Required]
        public string Reason { get; set; }



        /// <summary>
        /// 所属德育选项
        /// </summary>
        [Required]
        public virtual tbMoralItem tbMoralItem { get; set; }
    }
}