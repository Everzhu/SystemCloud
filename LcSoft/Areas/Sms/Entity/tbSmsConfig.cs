using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Entity
{
    /// <summary>
    /// 短信参数
    /// </summary>
    public class tbSmsConfig : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 短信服务
        /// </summary>
        [Display(Name = "短信服务"), Required]
        public string SmsServer { get; set; }

        /// <summary>
        /// 短信服务类型
        /// </summary>
        [Display(Name = "短信服务类型"), Required]
        public Code.EnumHelper.SmsServerType SmsServerType { get; set; }

        /// <summary>
        /// 短信账户
        /// </summary>
        [Display(Name = "短信账户"), Required]
        public string SmsAccount { get; set; }

        /// <summary>
        /// 短信密码
        /// </summary>
        [Display(Name = "短信密码"), Required]
        public string SmsPassword { get; set; }

        /// <summary>
        /// 短信地址
        /// </summary>
        [Display(Name = "短信地址"), Required]
        public string SmsUrl { get; set; }

        /// <summary>
        /// 短信签名
        /// </summary>
        [Display(Name = "短信签名"), Required]
        public string SmsFreeSignName { get; set; }

        /// <summary>
        /// 短信模版
        /// </summary>
        [Display(Name = "短信模版"), Required]
        public string SmsTemplateCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态"), Required]
        public bool Status { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        [Display(Name = "是否默认"), Required]
        public bool IsDefault { get; set; }
    }
}