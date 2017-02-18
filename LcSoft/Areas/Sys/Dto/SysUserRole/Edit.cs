using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Sys.Dto.SysUserRole
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 对应用户
        /// </summary>
        [Display(Name = "对应用户"), Required]
        public int SysUserId { get; set; }

        /// <summary>
        /// 所属角色
        /// </summary>
        [Display(Name = "所属角色"), Required]
        public int SysRoleId { get; set; }
    }
}
