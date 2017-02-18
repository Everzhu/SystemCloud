using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Models.DiskPower
{
    public class List
    {
        public List<Dto.DiskPower.List> DiskPowerList { get; set; } = new List<Dto.DiskPower.List>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int FolderId { get; set; } = System.Web.HttpContext.Current.Request["FolderId"].ConvertToInt();

        public string FolderName { get; set; }
    }
}