using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Entity
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class tbSysUser : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        [Display(Name = "用户账号"), Required]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [Display(Name = "用户姓名"), Required]
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        [Display(Name = "用户密码"), Required]
        public string Password { get; set; }

        /// <summary>
        /// 用户密码(MD5)
        /// </summary>
        [Display(Name = "用户密码(MD5)")]
        public string PasswordMd5 { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public virtual Dict.Entity.tbDictSex tbSex { get; set; }

        /// <summary>
        /// 用户类别
        /// </summary>
        [Display(Name = "用户类别"), Required]
        public Code.EnumHelper.SysUserType UserType { get; set; }

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
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        [Display(Name = "QQ")]
        public string Qq { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [Display(Name = "头像")]
        public string Photo { get; set; }

        /// <summary>
        /// 一卡通
        /// </summary>
        [Display(Name = "一卡通")]
        public string CardNo { get; set; }

        /// <summary>
        /// 是否需要修改密码提醒（首次登陆、重置密码后首次登陆时，目前只做了标记，未做弹框提醒）
        /// </summary>
        [Display(Name = "密码提醒"), Required]
        public bool NeedAlert { get; set; } = true;
    }
}