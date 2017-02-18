using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Sys.Models.SysIndex
{
    public class Forget
    {
        /// <summary>
        /// 是否租户
        /// </summary>
        public bool IsTenant { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        [Display(Name = "学校名称"), Required]
        public string SchoolName { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        [Display(Name = "用户账号")]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [Display(Name = "用户姓名"), Required]
        public string UserName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号")]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [RegularExpression(@"\d{11}", ErrorMessage = "手机格式不正确")]
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$", ErrorMessage = "邮箱格式不正确")]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "验证码"), Required]
        public string CheckCode { get; set; }

        public string CheckCodeRefer { get; set; }
    }
}
