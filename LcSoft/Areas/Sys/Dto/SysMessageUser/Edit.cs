using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Sys.Dto.SysMessageUser
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
        /// 所属消息
        /// </summary>
        [Display(Name = "所属消息"), Required]
        public string SysMessageId { get; set; }

        /// <summary>
        /// 接收人员
        /// </summary>
        [Display(Name = "接收人员"), Required]
        public string SysUserId { get; set; }

        /// <summary>
        /// 是否查阅
        /// </summary>
        [Display(Name = "是否查阅"), Required]
        public bool IsRead { get; set; }

        /// <summary>
        /// 查阅时间
        /// </summary>
        public DateTime ReadDate { get; set; }
    }
}
