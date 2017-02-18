using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 德育图片
    /// </summary>
    public class tbMoralPhoto : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属德育记录
        /// </summary> 
        [Required]
        [Display(Name = "所属德育记录")]
        public virtual tbMoralData tbMoralData { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [Required]
        [Display(Name = "文件名")]
        public string FileName { get; set; }
    }
}