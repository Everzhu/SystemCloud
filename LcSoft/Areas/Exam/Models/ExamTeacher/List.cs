using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamTeacher
{
    public class List
    {
        public List<Dto.ExamTeacher.List> ExamTeacherList { get; set; } = new List<Dto.ExamTeacher.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ExamId { get; set; } = System.Web.HttpContext.Current.Request["ExamId"].ConvertToInt();

        public int ScheduleId { get; set; } = System.Web.HttpContext.Current.Request["ScheduleId"].ConvertToInt();

        public int ExamRoomId { get; set; } = System.Web.HttpContext.Current.Request["ExamRoomId"].ConvertToInt();

        public int ExamCourseId { get; set; } = System.Web.HttpContext.Current.Request["ExamCourseId"].ConvertToInt();
    }
}