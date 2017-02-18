using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace XkSystem.Areas.Disk.Dto.DiskFile
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [Display(Name = "文件名称"), Required]
        public string FileTitle { get; set; }

        public int FolderId { get; set; }
    }
}