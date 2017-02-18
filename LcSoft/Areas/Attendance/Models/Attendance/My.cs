using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.Attendance
{
    public class My
    {
        public List<Dto.Attendance.My> MyAttendanceList { get; set; } = new List<Dto.Attendance.My>();

        public DateTime DateSearchFrom { get; set; } = HttpContext.Current.Request["DateSearchFrom"].ConvertToDateTime();

        public DateTime DateSearchTo { get; set; } = HttpContext.Current.Request["DateSearchTo"].ConvertToDateTime();

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}