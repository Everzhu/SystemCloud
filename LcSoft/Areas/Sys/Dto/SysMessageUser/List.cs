using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Sys.Dto.SysMessageUser
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 所属消息
        /// </summary>
        [Display(Name = "所属消息")]
        public string SysMessageName { get; set; }

        /// <summary>
        /// 接收人员
        /// </summary>
        [Display(Name = "接收人员")]
        public string SysUserName { get; set; }

        /// <summary>
        /// 是否查阅
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// 查阅时间
        /// </summary>
        public DateTime ReadDate { get; set; }
    }
}
