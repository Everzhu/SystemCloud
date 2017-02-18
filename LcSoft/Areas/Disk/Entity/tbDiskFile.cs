using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Entity
{
    /// <summary>
    /// 网盘文件
    /// </summary>
    public class tbDiskFile : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 文件标题
        /// </summary>
        [Required]
        [Display(Name = "文件标题")]
        public string FileTitle { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [Required]
        [Display(Name = "文件名")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Required]
        [Display(Name = "文件大小")]
        public long FileLength { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [Required]
        [Display(Name = "文件类型")]
        public string FileContent { get; set; }

        /// <summary>
        /// 文件夹
        /// </summary>
        [Display(Name = "文件夹")]
        public virtual tbDiskFolder tbDiskFolder { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Required]
        [Display(Name = "提交时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        [Required]
        [Display(Name = "上传人")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }
    }
}