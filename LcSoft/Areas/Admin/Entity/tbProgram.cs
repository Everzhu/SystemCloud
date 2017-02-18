using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Admin.Entity
{
    /// <summary>
    /// 程序
    /// </summary>
    public class tbProgram : Code.EntityHelper.EntityRoot
    {
        /// <summary>
        /// 程序名
        /// </summary>
        [Display(Name = "程序名"), Required]
        public string ProgramName { get; set; }

        /// <summary>
        /// 程序标题
        /// </summary>
        [Display(Name = "程序标题"), Required]
        public string ProgramTitle { get; set; }

        /// <summary>
        /// 是否宽屏展示
        /// </summary>
        [Display(Name = "是否宽屏展示"), Required]
        public bool IsWide { get; set; }

        /// <summary>
        /// 默认项目
        /// </summary>
        [Display(Name = "默认项目"), Required]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 程序编码
        /// </summary>
        [Display(Name = "程序编码"), Required]
        public Code.EnumHelper.ProgramCode ProgramCode { get; set; }

        /// <summary>
        /// 启动页
        /// </summary>
        [Display(Name = "启动页")]
        public string Startup { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Display(Name = "图标"), Required]
        public string BgIcon { get; set; }

        /// <summary>
        /// 背景颜色
        /// </summary>
        [Display(Name = "背景颜色"), Required]
        public string BgColor { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        public string Remark { get; set; }
    }
}