using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Entity
{
    /// <summary>
    /// 网盘类型
    /// </summary>
    public class tbDiskType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 网盘类型
        /// </summary>
        [Required]
        [Display(Name = "网盘类型")]
        public string DiskTypeName { get; set; }

        /// <summary>
        /// 类型编码
        /// </summary>
        [Required]
        [Display(Name = "类型编码")]
        public Code.EnumHelper.DiskType DiskType { get; set; }
    }
}