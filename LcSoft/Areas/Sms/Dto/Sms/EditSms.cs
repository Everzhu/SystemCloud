using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Dto.Sms
{
    public class EditSms
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        [Display(Name = "短信内容"), Required]
        public string SmsTitle { get; set; }

        /// <summary>
        /// 计划发送日期
        /// </summary>
        [Display(Name = "计划发送日期"), Required]
        public DateTime PlanDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 提交人员
        /// </summary>
        [Display(Name = "提交人员"), Required]
        public int SysUserId { get; set; }
    }
}