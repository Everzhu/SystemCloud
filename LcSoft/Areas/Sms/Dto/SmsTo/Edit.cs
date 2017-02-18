using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Dto.SmsTo
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码"), Required]
        public string Mobile { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态"), Required]
        public decimal Status { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        [Display(Name = "重试次数"), Required]
        public int Retry { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "备注信息"), Required]
        public string Remark { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "所属短信"), Required]
        public int SmsId { get; set; }

        /// <summary>
        /// 接收人员
        /// </summary>
        [Display(Name = "接收人员"), Required]
        public int SysUserId { get; set; }
    }
}