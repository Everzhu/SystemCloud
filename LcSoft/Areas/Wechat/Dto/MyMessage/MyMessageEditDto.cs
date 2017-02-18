using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Dto.MyMessage
{
    public class MyMessageEditDto
    {
        /// <summary>
        /// 消息标题
        /// </summary>
        [Display(Name = "消息标题"), Required]
        public string MessageTitle { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [Display(Name = "消息内容"), Required]
        public string MessageContent { get; set; }

        public string UserIds { get; set; }
    }
}