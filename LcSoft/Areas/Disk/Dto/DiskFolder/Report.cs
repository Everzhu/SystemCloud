using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Dto.DiskFolder
{
    public class Report
    {
        public int Id { get; set; }

        public string DiskFolderName { get; set; }

        public Code.EnumHelper.DiskPermit DiskPermit { get; set; }
    }
}