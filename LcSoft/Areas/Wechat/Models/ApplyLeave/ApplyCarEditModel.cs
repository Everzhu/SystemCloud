using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Models.ApplyLeave
{
    public class ApplyLeaveEditModel
    {
        public Dto.ApplyLeave.ApplyLeaveEditDto ApplyLeaveEditDto { get; set; } = new Dto.ApplyLeave.ApplyLeaveEditDto();

        public string DepartListJson { get; set; }

        public string LeaveTypeListJson { get; set; }

        public string ErrorMsg { get; set; }
    }
}