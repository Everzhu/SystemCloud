using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralStat
{
    public class ModelBase
    {
        /// <summary>
        /// 德育Id
        /// </summary>
        public int MoralId { get; set; } = HttpContext.Current.Request["MoralId"].ConvertToInt();

        public int MoralClassId { get; set; } = HttpContext.Current.Request["ClassId"].ConvertToInt();

        /// <summary>
        /// 德育集合
        /// </summary>
        public List<SelectListItem> MoralList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 德育选项集合
        /// </summary>
        public List<Dto.MoralItem.Info> MoralItemList { get; set; } = new List<Dto.MoralItem.Info>();

        /// <summary>
        /// 德育班级
        /// </summary>
        public List<SelectListItem> MoralClassList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 德语选项评价对象集合
        /// </summary>
        public List<SelectListItem> MoralItemKind { get; set; } = typeof(Code.EnumHelper.MoralItemKind).ToItemList();

        /// <summary>
        /// 统计类别(m=月、w=周、d=天、s=自定义)
        /// </summary>
        public string StatDate { get; set; } = HttpContext.Current.Request["StatDate"].ConvertToString();

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime FromDate { get; set; } = HttpContext.Current.Request["fd"].ConvertToDateTime();

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime ToDate { get; set; } = HttpContext.Current.Request["td"].ConvertToDateTime();


        public bool DataIsNull { get; set; }
    }
}