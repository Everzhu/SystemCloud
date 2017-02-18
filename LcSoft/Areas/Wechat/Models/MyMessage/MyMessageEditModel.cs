using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Models.MyMessage
{
    public class MyMessageEditModel
    {
        public Dto.MyMessage.MyMessageEditDto MyMessageEditDto { get; set; } = new Dto.MyMessage.MyMessageEditDto();

        public List<System.Web.Mvc.SelectListItem> RoleList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}