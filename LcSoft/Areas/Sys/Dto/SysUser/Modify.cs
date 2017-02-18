using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Sys.Dto.SysUser
{
    public class Modify
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        [Display(Name = "用户账号")]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [Display(Name = "用户姓名")]
        public string UserName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public int? SexId { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [Display(Name = "是否禁用"), Required]
        public bool IsDisable { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        [Display(Name = "是否锁定"), Required]
        public bool IsLock { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [Display(Name = "身份证号")]
        public string IdentityNumber { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码"), RegularExpression(@"\d{11}", ErrorMessage = "手机格式不正确")]
        public string Mobile { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "邮箱"), RegularExpression(@"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$", ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [Display(Name = "QQ号码")]
        public string Qq { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [Display(Name = "头像")]
        public string Photo { get; set; }
    }
}
