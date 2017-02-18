using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Dto.DiskFile
{
    public class Info
    {
        public int Id { get; set; }

        public string FileTitle { get; set; }

        public string FileName { get; set; }

        public int FolderId { get; set; }

        public string FolderPath { get; set; }
    }
}