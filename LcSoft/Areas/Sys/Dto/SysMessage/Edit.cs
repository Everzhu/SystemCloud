using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Sys.Dto.SysMessage
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        [Display(Name = "消息标题"), Required]
        public string MessageTitle { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [Display(Name = "消息内容")]
        public string MessageContent { get; set; }

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


        [Display(Name = "是否公开"), Required]
        public bool IsPublic { get; set; } = true;


        [Display(Name ="接收角色")]
        public string RoleId { get; set; }
    }
}
