using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Wechat.Controllers
{

    public class AttendanceController : Controller
    {
        /// <summary>
        /// 查询考勤记录接口
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetAttendanceList(string TenantName,string UserCode, string FromDate, string ToDate,int PerPage,int CurrentPage)
        {
            if (FromDate == "开始时间")
            {
                FromDate = "";
            }
            if (ToDate == "结束时间")
            {
                ToDate = "";
            }
            var list = Areas.Attendance.Controllers.AttendanceController.AttendanceList(TenantName, UserCode, FromDate, ToDate);
            list = list.Skip(PerPage * (CurrentPage - 1)).Take(PerPage).ToList();
            return Json(new { Result = "Success", Message = "", AttendanceList = list });
        }
    }
}