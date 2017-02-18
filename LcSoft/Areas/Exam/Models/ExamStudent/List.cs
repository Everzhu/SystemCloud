using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamStudent
{
    public class List
    {
        public List<Dto.ExamStudent.List> ExamStudentList { get; set; } = new List<Dto.ExamStudent.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();

        public int ScheduleId { get; set; } = System.Web.HttpContext.Current.Request["ScheduleId"].ConvertToInt();

        public int ExamRoomId { get; set; } = System.Web.HttpContext.Current.Request["ExamRoomId"].ConvertToInt();

        public int ExamCourseId { get; set; } = System.Web.HttpContext.Current.Request["ExamCourseId"].ConvertToInt();
    }
}