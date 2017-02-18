using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormStudent
{
    public class Export
    {
        public List<Dto.DormStudent.Export> ExportList { get; set; } = new List<Dto.DormStudent.Export>();
        
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? SexId { get; set; } = System.Web.HttpContext.Current.Request["SexId"].ConvertToInt();

        public int? BuildId { get; set; } = System.Web.HttpContext.Current.Request["BuildId"].ConvertToInt();

        public int? RoomId { get; set; } = System.Web.HttpContext.Current.Request["RoomId"].ConvertToInt();
    }
}