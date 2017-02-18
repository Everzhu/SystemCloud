using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Models.DiskFolder
{
    public class Report
    {
        public List<Dto.DiskFolder.Report> FolderList { get; set; }

        public List<Dto.DiskFolder.ReportUser> DiskPowerUserList { get; set; }

        public List<Dto.DiskFolder.ReportUser> DiskFileUserList { get; set; }

        public int ParentId { get; set; } = System.Web.HttpContext.Current.Request["ParentId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}