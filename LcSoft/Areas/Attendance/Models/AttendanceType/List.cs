using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.AttendanceType
{
    public class List
    {
        public List<Entity.tbAttendanceType> AttendanceTypeList { get; set; } = new List<Entity.tbAttendanceType>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}