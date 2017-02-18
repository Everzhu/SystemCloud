using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysMessage
{
    public class PrivateMessageEdit
    {
        public Dto.SysMessage.PrivateMessageEdit PrivateMessageMyEdit { get; set; } = new Dto.SysMessage.PrivateMessageEdit();
        public List<Dto.SysUser.List> SysUserList { get; set; } = new List<Dto.SysUser.List>();
    }
}