using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamSchedule
{
    public class List
    {
        public List<Dto.ExamSchedule.List> ExamScheduleList { get; set; } = new List<Dto.ExamSchedule.List>();

        public List<Dto.ExamSchedule.ScheduleRoomList> ScheduleRoomList { get; set; } = new List<Dto.ExamSchedule.ScheduleRoomList>();

        public List<Dto.ExamSchedule.ScheduleRoomList> ExamRoomCourseList { get; set; } = new List<Dto.ExamSchedule.ScheduleRoomList>();

        public List<System.Web.Mvc.SelectListItem> ExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ExamId { get; set; }= System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();

        public int ScheduleId { get; set; } = System.Web.HttpContext.Current.Request["ScheduleId"].ConvertToInt();

        public List<string> columnList { get; set; }
    }
}