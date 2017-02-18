using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Entity
{
    /// <summary>
    /// 菜单权限
    /// </summary>
    public class tbSysUserPower : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属菜单
        /// </summary>
        [Display(Name = "所属菜单"), Required]
        public virtual tbSysMenu tbSysMenu { get; set; }

        /// <summary>
        /// 对应用户
        /// </summary>
        [Display(Name = "对应用户")]
        public virtual tbSysUser tbSysUser { get; set; }
    }
}