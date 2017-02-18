using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyRoom
{
    public class List
    {
        public List<Dto.StudyRoom.List> StudyRoomList { get; set; } = new List<Dto.StudyRoom.List>();
        public List<System.Web.Mvc.SelectListItem> RoomList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
        public List<System.Web.Mvc.SelectListItem> BuildList = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> RoomTypeList = new List<System.Web.Mvc.SelectListItem>();
        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
        public int? BuildId { get; set; } = System.Web.HttpContext.Current.Request["BuildId"].ConvertToInt();

        public int? RoomTypeId { get; set; } = System.Web.HttpContext.Current.Request["RoomTypeId"].ConvertToInt();
    }
}