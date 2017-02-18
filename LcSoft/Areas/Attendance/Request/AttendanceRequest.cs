using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace XkSystem.Areas.Attendance.Request
{
    [DataContract]
    public class AttendanceRequest
    {
        [DataMember]
        public int idwYear { get; set; }

        [DataMember]
        public int idwMonth { get; set; }

        [DataMember]
        public int idwDay { get; set; }

        [DataMember]
        public int idwHour { get; set; }

        [DataMember]
        public int idwMinute { get; set; }

        [DataMember]
        public int idwSecond { get; set; }

        [DataMember]
        public string sdwEnrollNumber { get; set; }

        [DataMember]
        public string iMachineNumber { get; set; }
    }

    [DataContract]
    public class AttendanceRequestModel
    {
        [DataMember]
        public List<AttendanceRequest> AttendanceRequest { get; set; }


        [DataMember]
        public string TenantName { get; set; }
    }

    [DataContract]
    public class DealAttendanceRequestModel
    {
        [DataMember]
        public string TenantName { get; set; }
    }
}