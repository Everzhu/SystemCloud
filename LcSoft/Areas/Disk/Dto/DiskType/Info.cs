using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Dto.DiskType
{
    public class Info
    {
        public int Id { get; set; }

        /// <summary>
        /// 网盘类型
        /// </summary>
        public string DiskTypeName { get; set; }

        public Code.EnumHelper.DiskType DiskType { get; set; }
    }
}