using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Attendance.Models.Attendance
{
    public class Edit
    {
        public Dto.Attendance.Edit AttendanceEdit { get; set; } = new Dto.Attendance.Edit();

        //studentId=a.StudentId,periodId=a.PeriodId,orgId=Model.OrgId

        public int StudentId { get; set; } = HttpContext.Current.Request["StudentId"].ConvertToInt();

        public int PeriodId { get; set; } = HttpContext.Current.Request["PeriodId"].ConvertToInt();

        public int OrgId { get; set; } = HttpContext.Current.Request["OrgId"].ConvertToInt();

        public int DayWeekId { get; set; } = HttpContext.Current.Request["DayWeekId"].ConvertToInt();

        [Display(Name ="考勤")]
        public List<SelectListItem> AttendanceTypeList { get; set; } = new List<SelectListItem>();

        [Display(Name ="短信模板")]
        public string SmsTemplet { get; set; }

        
        public string SmsToUserIds { get; set; }

        [Display(Name = "短信接收人")]
        public string Mobiles { get; set; } = string.Empty;

        public List<SmsUserInfo> MobileList { get; set; } = new List<SmsUserInfo>();

    }
}