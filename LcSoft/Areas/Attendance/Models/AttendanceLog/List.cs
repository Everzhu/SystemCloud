using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.AttendanceLog
{
    public class List
    {
        public List<Entity.tbAttendanceLog> DataList { get; set; } = new List<Entity.tbAttendanceLog>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();

        public string FromTime { get; set; } = HttpContext.Current.Request["FromTime"].ConvertToString();

        public string ToTime { get; set; } = HttpContext.Current.Request["ToTime"].ConvertToString();
    }
}