using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace XkSystem.Areas.Disk.Dto.DiskFile
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 文件图标：文件夹|文件
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 网盘类型ID:0-私有,1-公共
        /// </summary>
        public int DiskTypeId { get; set; }

        public Code.EnumHelper.DiskPermit DiskPermit { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// 文件标题
        /// </summary>
        [Display(Name = "文件标题")]
        public string FileTitle { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Display(Name = "文件大小")]
        public long? FileLength { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Display(Name = "提交时间")]
        public DateTime? InputDate { get; set; }

        public bool IsAdmin { get; set; }
    }
}