using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Entity
{
    /// <summary>
    /// 消息（增加手机、RTX、短消息等推送）
    /// </summary>
    public class tbSysMessage : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 消息标题
        /// </summary>
        [Display(Name = "消息标题"), Required]
        public string MessageTitle { get; set; }

        /// <summary>
        /// 所属程序
        /// </summary>
        [Display(Name = "所属程序"), Required]
        public virtual Admin.Entity.tbProgram tbProgram { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [Display(Name = "消息内容")]
        public string MessageContent { get; set; }

        /// <summary>
        /// 是否授权
        /// </summary>
        [Display(Name = "是否授权"), Required]
        public bool IsPermit { get; set; }

        /// <summary>
        /// 短信通知
        /// </summary>
        [Display(Name = "短信通知"), Required]
        public bool IsSms { get; set; }

        /// <summary>
        /// 邮件通知
        /// </summary>
        [Display(Name = "邮件通知"), Required]
        public bool IsEmail { get; set; }

        /// <summary>
        /// 链接地址（可直接查看用）
        /// </summary>
        [Display(Name = "链接地址")]
        public string Url { get; set; }

        /// <summary>
        /// 提交人员
        /// </summary>
        [Display(Name = "提交人员"), Required]
        public virtual tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Display(Name = "提交时间"), Required]
        public DateTime InputDate { get; set; }

        [Display(Name = "是否公开"), Required]
        public bool IsPublic { get; set; } = true;
    }
}