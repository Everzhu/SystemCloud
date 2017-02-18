using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.Room
{
    public class SelectRoom
    {
        public List<Dto.Room.SelectRoom> SelectRoomList { get; set; } = new List<Dto.Room.SelectRoom>();

        public List<System.Web.Mvc.SelectListItem> BuildList = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> RoomTypeList = new List<System.Web.Mvc.SelectListItem>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? BuildId { get; set; } = System.Web.HttpContext.Current.Request["BuildId"].ConvertToInt();

        public int? RoomTypeId { get; set; } = System.Web.HttpContext.Current.Request["RoomTypeId"].ConvertToInt();
    }
}