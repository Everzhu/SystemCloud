using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysMessage
{
    public class PrivateMyMessageList
    {
        public List<Dto.SysMessage.PrivateMyMessageList> PrivateMessageMyList { get; set; } = new List<Dto.SysMessage.PrivateMyMessageList>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public string IsRead { get; set; } = System.Web.HttpContext.Current.Request["IsRead"].ConvertToString();
    }
}