using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyRoomStudent
{
    public class List
    {
        public List<Dto.StudyRoomStudent.List> StudyRoomStudentList { get; set; } = new List<Dto.StudyRoomStudent.List>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int RoomId { get; set; } = System.Web.HttpContext.Current.Request["RoomId"].ConvertToInt();
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();

        public string RoomName { get; set; }
    }
}