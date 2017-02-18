using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Dto.DiskFolder
{
    public class Info
    {
        public int Id { get; set; }

        public int? No { get; set; }

        public string DiskFolderName { get; set; }

        public int ParentId { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// 网盘类型ID:0-私有,1-公共
        /// </summary>
        public int DiskTypeId { get; set; }

        public string FolderPath { get; set; }

        public Code.EnumHelper.DiskPermit DiskPermit { get; set; }
    }
}