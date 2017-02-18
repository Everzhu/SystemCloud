using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamRoom
{
    public class Edit
    {
        public Dto.ExamRoom.Edit  ExamRoomEdit { get; set; } = new Dto.ExamRoom.Edit();

        public List<System.Web.Mvc.SelectListItem> RoomList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}