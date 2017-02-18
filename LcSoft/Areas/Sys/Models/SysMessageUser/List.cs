using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysMessageUser
{
    public class List
    {
        public List<Dto.SysMessageUser.List> MessageUserList { get; set; } = new List<Dto.SysMessageUser.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}