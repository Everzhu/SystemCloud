using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Models.SmsConfig
{
    public class Edit
    {
        public Dto.SmsConfig.Edit SmsConfigEdit { get; set; } = new Dto.SmsConfig.Edit();
        public List<System.Web.Mvc.SelectListItem> SmsServerTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}