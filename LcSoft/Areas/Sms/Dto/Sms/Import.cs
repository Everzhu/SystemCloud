using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Dto.Sms
{
    public class Import
    {
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "Id")]
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
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        /// <summary>
        /// 计划发送日期
        /// </summary>
        [Display(Name = "计划发送时间")]
        public string PlanDate { get; set; }

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
    }
}