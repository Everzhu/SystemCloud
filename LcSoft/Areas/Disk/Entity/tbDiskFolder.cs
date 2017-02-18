using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Entity
{
    /// <summary>
    /// 文件夹
    /// </summary>
    public class tbDiskFolder : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 文件夹
        /// </summary>
        [Required]
        [Display(Name = "文件夹")]
        public string DiskFolderName { get; set; }

        /// <summary>
        /// 文件夹类型
        /// </summary>
        [Required]
        [Display(Name = "文件夹类型")]
        public virtual tbDiskType tbDiskType { get; set; }

        /// <summary>
        /// 上级
        /// </summary>
        [Display(Name = "上级")]
        public virtual tbDiskFolder tbDiskFolderParent { get; set; }

        /// <summary>
        /// 文件夹权限
        /// </summary>
        [Required]
        [Display(Name = "文件夹权限")]
        public Code.EnumHelper.DiskPermit DiskPermit { get; set; }

        /// <summary>
        /// 所属人员
        /// </summary>
        [Required]
        [Display(Name = "所属人员")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
    }
}