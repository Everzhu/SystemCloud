using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Dto.Sms
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        [Display(Name = "短信内容")]
        public string SmsTitle { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Display(Name = "验证码")]
        public string SmsPIN { get; set; }

        /// <summary>
        /// 计划发送日期
        /// </summary>
        [Display(Name = "计划发送时间")]
        public DateTime PlanDate { get; set; }

        /// <summary>
        /// 提交日期
        /// </summary>
        [Display(Name = "提交时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 提交人员
        /// </summary>
        [Display(Name = "提交人员")]
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