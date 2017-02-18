using System;
using System.Collections.Generic;
using System.Web;

namespace XkSystem.Areas.Disk.Models.DiskFolder
{
    public class Edit
    {
        public Dto.DiskFolder.Edit DiskFolderEdit { get; set; } = new Dto.DiskFolder.Edit();

        public List<Dto.DiskType.Info> DiskTypeList { get; set; } = new List<Dto.DiskType.Info>();

        public int FolderId { get; set; } = System.Web.HttpContext.Current.Request["FolderId"].ConvertToInt();

        public int DiskTypeId { get; set; } = System.Web.HttpContext.Current.Request["DiskTypeId"].ConvertToInt();

        public int UserId { get; set; } = System.Web.HttpContext.Current.Request["UserId"].ConvertToInt();
    }
}