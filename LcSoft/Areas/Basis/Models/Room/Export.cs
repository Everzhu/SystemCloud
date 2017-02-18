using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.Room
{
    public class Export
    {
        public List<Dto.Room.Export> ExportList { get; set; } = new List<Dto.Room.Export>();

        public int? BuildId { get; set; } = System.Web.HttpContext.Current.Request["BuildId"].ConvertToInt();

        public int? RoomTypeId { get; set; } = System.Web.HttpContext.Current.Request["RoomTypeId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}