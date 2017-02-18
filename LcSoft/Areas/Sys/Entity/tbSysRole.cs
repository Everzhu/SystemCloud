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
    public class tbSysRole : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Display(Name = "角色名称"), Required]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        [Display(Name = "角色编码")]
        public Code.EnumHelper.SysRoleCode RoleCode { get; set; }
    }
}