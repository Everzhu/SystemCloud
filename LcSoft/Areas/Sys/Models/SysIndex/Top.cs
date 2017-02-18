using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysIndex
{
    public class Top
    {
        public List<System.Web.Mvc.SelectListItem> ProgramInfo { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Dto.SysMenu.Info> MenuInfo { get; set; } = new List<Dto.SysMenu.Info>();

        public List<Dto.SysMessage.PrivateMyMessageList> PrivateMyMessageList { get; set; } = new List<Dto.SysMessage.PrivateMyMessageList>();

        /// <summary>
        /// 未读招生信息数量,用于导航菜单信封图标提示
        /// </summary>
        //public int AdmitUnReadPrivateMyMessageCount { get; set; }
    }
}