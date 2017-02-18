using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Entity
{
    /// <summary>
    /// 用户日志
    /// </summary>
    public class tbSysUserLog : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属用户
        /// </summary>
        [Display(Name = "所属用户"), Required]
        public virtual tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        [Display(Name = "操作内容"), Required]
        public string ActionContent { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [Display(Name = "操作时间"), Required]
        public DateTime ActionDate { get; set; }

        /// <summary>
        /// 操作IP
        /// </summary>
        [Display(Name = "操作IP"), Required]
        public string ActionIp { get; set; }
    }
}