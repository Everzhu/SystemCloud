using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralStat
{
    public class DayStat
    {

        public int MoralId { get; set; } = HttpContext.Current.Request["MoralId"].ConvertToInt();

        public int? ClassId { get; set; } = HttpContext.Current.Request["ClassId"].ConvertToInt();

        public List<Dto.MoralStat.List> StatList { get; set; } = new List<Dto.MoralStat.List>();

        public List<SelectListItem> MoralList { get; set; } = new List<SelectListItem>();

        public List<Student.Dto.Student.Info> MoralStudentList { get; set; } = new List<Student.Dto.Student.Info>();

        public List<SelectListItem> MoralClassList { get; internal set; } = new List<SelectListItem>();

        public List<Dto.MoralItem.Info> MoralItemList { get; internal set; } = new List<Dto.MoralItem.Info>();

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