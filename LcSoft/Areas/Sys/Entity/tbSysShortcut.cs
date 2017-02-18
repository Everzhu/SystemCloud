using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Entity
{
    /// <summary>
    /// 快捷菜单
    /// </summary>
    public class tbSysShortcut : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 对应用户
        /// </summary>
        [Display(Name = "对应用户")]
        public virtual tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 所属菜单
        /// </summary>
        [Display(Name = "所属菜单"), Required]
        public virtual tbSysMenu tbSysMenu { get; set; }
    }
}