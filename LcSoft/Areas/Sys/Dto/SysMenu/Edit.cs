using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Sys.Dto.SysMenu
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [Display(Name = "菜单名称"), Required]
        public string MenuName { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        [Display(Name = "菜单地址")]
        public string MenuUrl { get; set; }

        /// <summary>
        /// 上级菜单
        /// </summary>
        [Display(Name = "上级菜单")]
        public int? MenuParentId { get; set; }

        /// <summary>
        /// 上级菜单
        /// </summary>
        [Display(Name = "上级菜单")]
        public string MenuParentName { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [Display(Name = "菜单图标")]
        public string Icon { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态"), Required]
        public bool IsDisable { get; set; }

        /// <summary>
        /// 快捷菜单
        /// </summary>
        [Display(Name = "快捷菜单"), Required]
        public bool IsShortcut { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }
    }
}
