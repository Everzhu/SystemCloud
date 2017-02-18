using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveInputBySchedule
{
    public class List
    {
        public int Id { get; set; }

        public string OrgName { get; set; }

        public string TeacherName { get; set; }

        public string RoomName { get; set; }

        public int MaxCount { get; set; }

        public int RemainCount { get; set; }       

        public int WeekId { get; set; }

        public int PeriodId { get; set; }        

        public bool IsSelected { get; set; }

        public bool IsChecked { get; set; }

        public bool IsFixed { get; set; }

    }
}