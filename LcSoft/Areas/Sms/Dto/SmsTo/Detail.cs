using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Dto.SmsTo
{
    public class Detail
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
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [Display(Name = "发送时间")]
        public DateTime SendDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public decimal Status { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        [Display(Name = "重试次数")]
        public int Retry { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "备注信息")]
        public string Remark { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "所属短信")]
        public int SmsId { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "短信内容")]
        public string SmsTitle { get; set; }

        /// <summary>
        /// 接收人员
        /// </summary>
        [Display(Name = "接收人员")]
        public string SysUserName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime FromDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime ToDate { get; set; } = DateTime.Now;
    }
}