using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Dto.SmsConfig
{
    public class Info
    {
        public int Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }
        /// <summary>
        /// 短信服务
        /// </summary>
        [Display(Name = "短信服务")]
        public string SmsServer { get; set; }
        /// <summary>
        /// 短信服务类型
        /// </summary>
        [Display(Name = "服务类型")]
        public XkSystem.Code.EnumHelper.SmsServerType SmsServerType { get; set; }
        /// <summary>
        /// 短信服务类型
        /// </summary>
        [Display(Name = "服务类型")]
        public string SmsServerTypeName
        {
            get
            {
                return this.SmsServerType.GetDescription();
            }
        }
        /// <summary>
        /// 短信账户
        /// </summary>
        [Display(Name = "短信账户")]
        public string SmsAccount { get; set; }
        /// <summary>
        /// 短信密码
        /// </summary>
        [Display(Name = "短信密码")]
        public string SmsPassword { get; set; }
        /// <summary>
        /// 短信地址
        /// </summary>
        [Display(Name = "短信地址(阿里)")]
        public string SmsUrl { get; set; }
        /// <summary>
        /// 短信签名
        /// </summary>
        [Display(Name = "短信签名(阿里)")]
        public string SmsFreeSignName { get; set; }
        /// <summary>
        /// 短信模版
        /// </summary>
        [Display(Name = "短信模版(阿里)")]
        public string SmsTemplateCode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public bool Status { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        [Display(Name = "是否默认")]
        public bool IsDefault { get; set; }
    }
}