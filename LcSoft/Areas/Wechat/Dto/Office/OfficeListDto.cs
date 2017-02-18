using XkSystem.Areas.Wechat.Dto.CommDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Dto.Office
{
    public class OfficeListDto : WeOAApproveFlowListDto
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 来文(电)单位
        /// </summary>
        public string OfficeFileFrom { get; set; }

        /// <summary>
        /// 收文时间
        /// </summary>
        public DateTime ReceiveFileTime { get; set; }

        /// <summary>
        /// 办理时限
        /// </summary>
        public DateTime LimitDateTo { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public string OfficeFileName { get; set; }

        /// <summary>
        /// 附件，序列化名称
        /// </summary>
        public string OfficeFileNameSeq { get; set; }

        /// <summary>
        /// 内容摘要
        /// </summary>
        public string FileContent { get; set; }
    }
}