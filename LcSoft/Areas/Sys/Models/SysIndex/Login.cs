using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Sys.Models.SysIndex
{
    /// <summary>
    /// 登录
    /// </summary>
    public class Login
    {
        public List<Admin.Dto.Program.Info> ProgramList { get; set; }

        /// <summary>
        /// 是否选择租户
        /// </summary>
        public bool IsTenant { get; set; }

        /// <summary>
        /// 系统Id
        /// </summary>
        public int ProgramId { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        [Display(Name = "学校名称"), Required]
        public string SchoolName { get; set; }

        /// <summary>
        /// 用户帐号
        /// </summary>
        [Display(Name = "用户帐号"), Required]
        public string UserCode { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [Display(Name = "登录密码"), Required]
        public string Password { get; set; }

        /// <summary>
        /// 记住账号和密码
        /// </summary>
        [Display(Name = "记住账号和密码")]
        public bool Remember { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Display(Name = "验证码"), Required]
        public string CheckCode { get; set; }

        /// <summary>
        /// 确认验证码
        /// </summary>
        public string CheckCodeRefer { get; set; }

        /// <summary>
        /// 登录后指定的返回地址，非单点
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// logo
        /// </summary>
        public string Logo { get; set; }
    }
}
