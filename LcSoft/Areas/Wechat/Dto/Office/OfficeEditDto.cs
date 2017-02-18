using XkSystem.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Dto.Office
{
    public class OfficeEditDto
    {
        [Display(Name = "来文(电)单位")]
        [Required]
        public string OfficeFileFrom { get; set; }

        [Display(Name = "收文时间")]
        [Required]
        public DateTime ReceiveFileTime { get; set; }

        [Display(Name = "标题")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "办理时限")]
        [Required]
        [DateTimeNotLessThan("ReceiveFileTime", "收文时间")]
        public DateTime LimitDateTo { get; set; }

        [Display(Name = "附件或摘要")]
        [Required]
        public string OfficeFileName { get; set; }

        [Display(Name = "附件或摘要")]
        [Required]
        public string FileContent { get; set; }

        /// <summary>
        /// 指定审批人ID
        /// </summary>
        [Display(Name = "指定审批人")]
        [Required]
        public int NextApproveUserId { get; set; }

        /// <summary>
        /// 指定审批人名称
        /// </summary>
        [Display(Name = "指定审批人")]
        [Required]
        public string NextApproveUserName { get; set; }

    }
}