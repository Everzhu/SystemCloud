using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamRoom
{
    public class List
    {
        public List<Dto.ExamRoom.List> ExamRoomList { get; set; } = new List<Dto.ExamRoom.List>();
        public List<Dto.ExamRoom.List> StudentList { get; set; } = new List<Dto.ExamRoom.List>();
        public List<Dto.ExamRoom.List> ExamTeacherList { get; set; } = new List<Dto.ExamRoom.List>();


        public List<System.Web.Mvc.SelectListItem> RoomList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();
        public List<System.Web.Mvc.SelectListItem> ExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int ScheduleId { get; set; } = System.Web.HttpContext.Current.Request["ScheduleId"].ConvertToInt();

        public int ExamRoomId { get; set; } = System.Web.HttpContext.Current.Request["ExamRoomId"].ConvertToInt();
        public int ExamCourseId { get; set; } = System.Web.HttpContext.Current.Request["ExamCourseId"].ConvertToInt();
    }
}