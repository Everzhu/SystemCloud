using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 孩子家中情况回复
    /// </summary>
    public class tbMoralHappeningReply : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 回复对象
        /// </summary>
        [Required]
        [Display(Name = "回复对象")]
        public virtual Entity.tbMoralHappening tbMoralHappening { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        [Display(Name = "内容")]
        public string Content { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Required]
        [Display(Name = "提交时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 提交人员
        /// </summary>
        [Required]
        [Display(Name = "提交人员")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
    }
}