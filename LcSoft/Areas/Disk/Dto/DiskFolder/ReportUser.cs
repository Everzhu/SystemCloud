using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Dto.DiskFolder
{
    public class ReportUser
    {
        public int FolderId { get; set; }

        public int UserId { get; set; }

        public string UserCode { get; set; }

        public string UserName { get; set; }
    }
}