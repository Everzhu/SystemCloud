using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Sys.Models.SysIndex
{
    public class Index
    {
        /// <summary>
        /// 菜单
        /// </summary>
        public List<Dto.SysMenu.Info> MenuInfo { get; set; } = new List<Dto.SysMenu.Info>();

        /// <summary>
        /// 通知公告
        /// </summary>
        public List<Dto.SysMessage.List> SysMessageList { get; set; } = new List<Dto.SysMessage.List>();

        /// <summary>
        /// 常用功能
        /// </summary>
        public List<Dto.SysMenu.Info> SysShortcutList { get; set; } = new List<Dto.SysMenu.Info>();

        /// <summary>
        /// 个人信息
        /// </summary>
        public Dto.SysUser.Info UserInfo { get; set; }

        //Add,Harvey,xxzx,for demo,20161031
        /// <summary>
        /// 微信端调用PC端菜单列表
        /// </summary>
        public List<Dto.SysMenu.List> SysWechatMenuList { get; set; }
        //Add End
    }
}
