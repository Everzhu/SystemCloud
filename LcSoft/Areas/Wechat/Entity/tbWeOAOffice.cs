using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Entity
{
    /// <summary>
    /// 办公发文审批对象
    /// </summary>
    public class tbWeOAOffice : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 来文(电)单位
        /// </summary>
        [Required]
        [Display(Name = "来文(电)单位")]
        public string OfficeFileFrom { get; set; }

        /// <summary>
        /// 收文时间
        /// </summary>
        [Required]
        [Display(Name = "收文时间")]
        public DateTime ReceiveFileTime { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        [Display(Name = "标题")]
        public string Title { get; set; }

        /// <summary>
        /// 办理时限
        /// </summary>
        [Display(Name = "办理时限")]
        public DateTime LimitDateTo { get; set; }

        /// <summary>
        /// 办公文件,附件
        /// </summary>
        [Display(Name = "办公文件,附件")]
        public string OfficeFileName { get; set; }

        /// <summary>
        /// 办公文件，附件，序列化名称
        /// </summary>
        [Display(Name = "办公文件，附件，序列化名称")]
        public string OfficeFileNameSeq { get; set; }

        /// <summary>
        /// 内容摘要
        /// </summary>
        [Display(Name = "内容摘要")]
        public string FileContent { get; set; }

    }
}