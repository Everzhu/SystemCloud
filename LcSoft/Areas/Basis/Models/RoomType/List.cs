using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.RoomType
{
    public class List
    {
        public List<Entity.tbRoomType> RoomTypeList { get; set; } = new List<Entity.tbRoomType>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}