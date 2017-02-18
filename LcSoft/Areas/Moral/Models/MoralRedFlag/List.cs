using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralRedFlag
{
    public class List
    {
        /// <summary>
        /// 周(按周统计，该参数表示统计本年度的第多少周)
        /// </summary>
        public int? WeekNum { get; set; } = HttpContext.Current.Request["WeekNum"].ConvertToIntWithNull();

        /// <summary>
        /// 德育Id
        /// </summary>
        public int MoralId { get; set; } = HttpContext.Current.Request["MoralId"].ConvertToInt();

        /// <summary>
        /// 德育集合
        /// </summary>
        public List<System.Web.Mvc.SelectListItem> MoralList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        /// <summary>
        /// 德育选项集合
        /// </summary>
        public List<Dto.MoralItem.Info> MoralItemList { get; set; } = new List<Dto.MoralItem.Info>();

        public string Date { get; set; } = HttpContext.Current.Request["Date"].ConvertToString();

        public List<Dto.MoralStat.RedFlag> StatList { get; set; } = new List<Dto.MoralStat.RedFlag>();

        public List<Dto.MoralRedFlag.ClassInfo> MoralClassInfo { get; set; } = new List<Dto.MoralRedFlag.ClassInfo>();

        public List<System.Web.Mvc.SelectListItem> WeekList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public bool DataIsNull { get; set; }
    }
}