using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Entity
{
    /// <summary>
    /// 角色
    /// </summary>
    public class tbSysUserRole : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 对应用户
        /// </summary>
        [Display(Name = "对应用户"), Required]
        public virtual tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 所属角色
        /// </summary>
        [Display(Name = "所属角色"), Required]
        public virtual tbSysRole tbSysRole { get; set; }
    }
}