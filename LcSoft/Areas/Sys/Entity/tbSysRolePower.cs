using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Entity
{
    /// <summary>
    /// 菜单权限（角色授权分按角色、按用户、按用户类型，任选一种模式）
    /// </summary>
    public class tbSysRolePower : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属菜单
        /// </summary>
        [Display(Name = "所属菜单"), Required]
        public virtual tbSysMenu tbSysMenu { get; set; }

        /// <summary>
        /// 对应角色
        /// </summary>
        [Display(Name = "对应角色")]
        public virtual tbSysRole tbSysRole { get; set; }
    }
}