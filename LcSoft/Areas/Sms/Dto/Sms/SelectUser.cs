using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Dto.Sms
{
    public class SelectUser
    {
        public int Id { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        [Display(Name = "账号")]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string UserName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }

        /// <summary>
        /// 用户类别
        /// </summary>
        [Display(Name = "类别")]
        public Code.EnumHelper.SysUserType UserType { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }
    }
}