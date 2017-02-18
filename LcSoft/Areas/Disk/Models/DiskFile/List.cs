using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Models.DiskFile
{
    public class List
    {
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int FolderId { get; set; } = System.Web.HttpContext.Current.Request["FolderId"].ConvertToInt();

        public int DiskTypeId { get; set; } = System.Web.HttpContext.Current.Request["DiskTypeId"].ConvertToInt();

        public int UserId { get; set; } = System.Web.HttpContext.Current.Request["UserId"].ConvertToInt();

        public List<Dto.DiskFolder.Info> FolderPath { get; set; } = new List<Dto.DiskFolder.Info>();

        public List<Dto.DiskFile.List> DiskFileList { get; set; } = new List<Dto.DiskFile.List>();

        public List<int> AdminFolderIds { get; set; } = new List<int>();

        public List<int> ViewFolderIds { get; set; } = new List<int>();

        public List<int> AdminFileIds { get; set; } = new List<int>();

        public List<int> ViewFileIds { get; set; } = new List<int>();
    }
}