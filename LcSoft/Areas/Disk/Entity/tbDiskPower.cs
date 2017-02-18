using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Entity
{
    /// <summary>
    /// 网盘权限
    /// </summary>
    public class tbDiskPower : Code.EntityHelper.EntityBase 
    {
        /// <summary>
        /// 文件夹
        /// </summary>
        [Required]
        [Display(Name = "文件夹")]
        public virtual tbDiskFolder tbDiskFolder { get; set; }

        /// <summary>
        /// 人员
        /// </summary>
        [Required]
        [Display(Name = "人员")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 管理权限
        /// </summary>
        [Required]
        [Display(Name = "管理权限")]
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 上传权限
        /// </summary>
        [Required]
        [Display(Name = "上传权限")]
        public bool IsInput { get; set; }

        /// <summary>
        /// 查看权限
        /// </summary>
        [Required]
        [Display(Name = "查看权限")]
        public bool IsView { get; set; }
    }
}