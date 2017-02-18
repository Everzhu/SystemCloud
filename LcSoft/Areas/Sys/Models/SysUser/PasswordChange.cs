using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Sys.Models.SysUser
{
    public class PasswordChange
    {
        /// <summary>
        /// 原密码
        /// </summary>
        [Display(Name = "原密码"), Required]
        public string Password { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Display(Name = "新密码"), Required, MinLength(6)]
        public string PasswordNew { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        [Display(Name = "确认密码"), Required, MinLength(6)]
        [Compare("PasswordNew")]
        public string PasswordAgain { get; set; }
    }
}
