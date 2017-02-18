using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Models.DiskFile
{
    public class Edit
    {
        public Dto.DiskFile.Edit DiskFileEdit { get; set; } = new Dto.DiskFile.Edit();

        public int FolderId { get; set; } = System.Web.HttpContext.Current.Request["FolderId"].ConvertToInt();

        public int UserId { get; set; } = System.Web.HttpContext.Current.Request["UserId"].ConvertToInt();
    }
}