using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Entity
{
    /// <summary>
    /// 消息接收人
    /// </summary>
    public class tbSysMessageUser : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属消息
        /// </summary>
        [Display(Name = "所属消息"), Required]
        public virtual tbSysMessage tbSysMessage { get; set; }

        /// <summary>
        /// 接收人员
        /// </summary>
        [Display(Name = "接收人员"), Required]
        public virtual tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 是否查阅
        /// </summary>
        [Display(Name = "是否查阅"), Required]
        public bool IsRead { get; set; }

        /// <summary>
        /// 查阅时间
        /// </summary>
        [Display(Name = "查阅时间")]
        public DateTime ReadDate { get; set; }
    }
}