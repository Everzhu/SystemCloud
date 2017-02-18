using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Entity
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class tbSysMenu : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        [Display(Name = "菜单名称"), Required]
        public string MenuName { get; set; }

        /// <summary>
        /// 所属程序
        /// </summary>
        [Display(Name = "所属程序"), Required]
        public virtual Admin.Entity.tbProgram tbProgram { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        [Display(Name = "菜单地址")]
        public string MenuUrl { get; set; }

        /// <summary>
        /// 上级菜单ID
        /// </summary>
        [Display(Name = "上级菜单ID")]
        public virtual tbSysMenu tbMenuParent { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Display(Name = "图标")]
        public string Icon { get; set; }

        /// <summary>
        /// 是否关闭
        /// </summary>
        [Display(Name = "是否关闭"), Required]
        public bool IsDisable { get; set; }

        /// <summary>
        /// 是否快捷菜单
        /// </summary>
        [Display(Name = "是否快捷菜单"), Required]
        public bool IsShortcut { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }
    }
}