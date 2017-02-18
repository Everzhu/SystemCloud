using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace XkSystem.Areas.Disk.Dto.DiskFolder
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
        /// 文件夹名称
        /// </summary>
        [Display(Name = "文件夹名称"), Required]
        public string DiskFolderName { get; set; }

        /// <summary>
        /// 文件夹类型
        /// </summary>
        [Display(Name = "文件夹类型"), Required]
        public int DiskTypeId { get; set; }

        /// <summary>
        /// 文件夹权限
        /// </summary>
        [Display(Name = "文件夹权限"), Required]
        public Code.EnumHelper.DiskPermit DiskPermit { get; set; }

        public int ParentId { get; set; }
    }
}