using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class ClassSetImportWordSchedule
    {
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 班级课表
        /// </summary>
        public DataTable ClassScheduleList { get; set; } = new DataTable();
    }
}