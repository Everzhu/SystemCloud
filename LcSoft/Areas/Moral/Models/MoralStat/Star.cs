using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralStat
{
    public class Star
    {
        /// <summary>
        /// 德育Id
        /// </summary>
        public int MoralId { get; set; } = HttpContext.Current.Request["MoralId"].ConvertToInt();

        /// <summary>
        /// 德育集合
        /// </summary>
        public List<SelectListItem> MoralList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 德育选项集合
        /// </summary>
        public List<Dto.MoralItem.Info> MoralItemList { get; set; } = new List<Dto.MoralItem.Info>();

        public string Date { get; set; } = HttpContext.Current.Request["Date"].ConvertToString();

        public List<Dto.MoralStat.Star> StatList { get; set; } = new List<Dto.MoralStat.Star>();

        public List<Dto.MoralStat.StudentInfo> MoralStudentList { get; set; } = new List<Dto.MoralStat.StudentInfo>();

        public bool MoralIsNull { get; set; }

        public bool DataIsNull { get; set; }

    }
}