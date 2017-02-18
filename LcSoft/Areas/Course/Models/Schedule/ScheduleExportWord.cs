using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class ScheduleExportWord
    {
        public string HeaderText { get; set; }

        public string TableCaption { get; set; }

        public DataTable ScheduleList { get; set; } = new DataTable();
    }
}