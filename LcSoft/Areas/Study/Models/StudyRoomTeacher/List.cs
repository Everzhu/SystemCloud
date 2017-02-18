using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyRoomTeacher
{
    public class List
    {
        public List<Dto.StudyRoomTeacher.List> StudyRoomTeacherList { get; set; } = new List<Dto.StudyRoomTeacher.List>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int RoomId { get; set; } = System.Web.HttpContext.Current.Request["RoomId"].ConvertToInt();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
        public List<System.Web.Mvc.SelectListItem> WeekList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string RoomName { get; set; }
    }
}