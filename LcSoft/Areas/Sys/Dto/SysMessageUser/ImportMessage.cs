using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Dto.SysMessageUser
{
    public class ImportMessage
    {
        public int Id { get; set; }

        /// <summary>
        /// 所属消息
        /// </summary>
        [Display(Name = "用户账号")]
        public string UserCode { get; set; }

        /// <summary>
        /// 所属消息
        /// </summary>
        [Display(Name = "用户姓名")]
        public string UserName { get; set; }

        /// <summary>
        /// 所属消息
        /// </summary>
        [Display(Name = "私信标题")]
        public string MessageTitle { get; set; }

        /// <summary>
        /// 所属消息
        /// </summary>
        [Display(Name = "私信内容")]
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
        /// 是否查阅
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// 查阅时间
        /// </summary>
        public DateTime ReadDate { get; set; }
    }
}