using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Dto.SysMessage
{
    public class PrivateMessageList
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        [Display(Name = "消息标题")]
        public string MessageTitle { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [Display(Name = "消息内容")]
        public string MessageContent { get; set; }

        /// <summary>
        /// 短信通知
        /// </summary>
        [Display(Name = "是否手机短信")]
        public bool IsSms { get; set; }

        /// <summary>
        /// 邮件通知
        /// </summary>
        [Display(Name = "邮件通知")]
        public bool IsEmail { get; set; }

        /// <summary>
        /// 链接地址（可直接查看用）
        /// </summary>
        [Display(Name = "链接地址")]
        public string Url { get; set; }

        /// <summary>
        /// 提交人员
        /// </summary>
        [Display(Name = "提交人员")]
        public string SysUserName { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Display(Name = "提交时间")]
        public DateTime InputDate { get; set; }
        /// <summary>
        /// 消息人员
        /// </summary>
        public List<Dto.SysMessageUser.List> SysMessageUserList { get; set; }
    }
}