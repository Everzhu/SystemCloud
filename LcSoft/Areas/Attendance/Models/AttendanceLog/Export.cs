using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.AttendanceLog
{
    public class Export
    {
        public List<Dto.AttendanceLog.Export> DataList { get; set; } = new List<Dto.AttendanceLog.Export>();

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();

        public DateTime FromTime { get; set; } = HttpContext.Current.Request["FromTime"].ConvertToDateTime();

        public DateTime ToTime { get; set; } = HttpContext.Current.Request["ToTime"].ConvertToDateTime();
    }
}