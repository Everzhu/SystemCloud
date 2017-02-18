using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Dto.SysRole
{
    public class MenuRoleList
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [Display(Name = "角色名称")]
        public string RoleName { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        [Display(Name = "角色编码")]
        public Code.EnumHelper.SysRoleCode RoleCode { get; set; }

        /// <summary>
        /// 是否授权
        /// </summary>
        [Display(Name = "是否授权")]
        public bool IsEnabled { get; set; }
    }
}