using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Disk.Models.DiskFileType
{
    public class List
    {
        public List<Entity.tbDiskFileType> DataList { get; set; } = new List<Entity.tbDiskFileType>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}