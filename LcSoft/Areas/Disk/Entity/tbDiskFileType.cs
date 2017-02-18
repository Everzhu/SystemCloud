using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Disk.Entity
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public class tbDiskFileType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 文件类型
        /// </summary>
        [Required]
        [Display(Name = "文件类型")]
        public string DiskFileTypeName { get; set; }
    }
}
