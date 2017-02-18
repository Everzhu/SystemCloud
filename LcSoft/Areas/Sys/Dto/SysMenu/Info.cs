using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Dto.SysMenu
{
    public class Info
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? No { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 菜单地址
        /// </summary>
        public string MenuUrl { get; set; }

        /// <summary>
        /// 上级菜单Id
        /// </summary>
        public int? MenuParentId { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// ProgramId
        /// </summary>
        public int ProgramId { get; set; }

        /// <summary>
        /// TenantId
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 快捷菜单
        /// </summary>
        public bool IsShortcut { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
    }
}