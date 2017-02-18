using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Dto.SysMessage
{
    public class PrivateMessageEdit
    {
        public int Id { get; set; }

        /// <summary>
        /// 私信标题
        /// </summary>
        [Display(Name = "私信标题"), Required]
        public string MessageTitle { get; set; }

        /// <summary>
        /// 私信内容
        /// </summary>
        [Display(Name = "私信内容")]
        public string MessageContent { get; set; }

        /// <summary>
        /// 接收人员
        /// </summary>
        [Display(Name = "接收人员")]
        public string ReceiverUserIds { get; set; }

        /// <summary>
        /// 短信通知
        /// </summary>
        [Display(Name = "是否手机短信"), Required]
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
        public int SysUserId { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Display(Name = "提交时间"), Required]
        public DateTime InputDate { get; set; }
    }
}