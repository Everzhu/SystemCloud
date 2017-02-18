using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Admin.Dto.Program
{
    public class Info
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Display(Name = "序号"),]
        public int? No { get; set; }

        /// <summary>
        /// 程序名称
        /// </summary>
        [Display(Name = "程序名称")]
        public string ProgramName { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        [Display(Name = "是否默认")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 程序标题
        /// </summary>
        [Display(Name = "程序标题")]
        public string ProgramTitle { get; set; }

        /// <summary>
        /// 是否宽屏
        /// </summary>
        [Display(Name = "是否宽屏")]
        public bool IsWide { get; set; }

        /// <summary>
        /// 程序类型
        /// </summary>
        [Display(Name = "程序类型")]
        public int ProgramTypeId { get; set; }

        /// <summary>
        /// 启动页
        /// </summary>
        public string Startup { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Display(Name = "图标")]
        public string BgIcon { get; set; }

        /// <summary>
        /// 背景颜色
        /// </summary>
        [Display(Name = "背景颜色")]
        public string BgColor { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        public string Remark { get; set; }
    }
}