using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Sys.Dto.SysRolePower
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
        /// 所属菜单
        /// </summary>
        [Display(Name = "所属菜单"), Required]
        public string SysMenuId { get; set; }

        /// <summary>
        /// 对应角色
        /// </summary>
        [Display(Name = "对应角色")]
        public string SysRoleId { get; set; }
    }
}
