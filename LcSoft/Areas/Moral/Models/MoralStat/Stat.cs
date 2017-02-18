using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralStat
{
    public class Stat
    {

        public int MoralId { get; set; } = System.Web.HttpContext.Current.Request["MoralId"].ConvertToInt();

        public DateTime MoralFromDate { get; set; }

        public DateTime MoralToDate { get; set; }

        //public List<Dto.MoralStat.List> MoralStatList { get; set; } = new List<Dto.MoralStat.List>();

        //public List<Dto.MoralStat.List> MoralScoreList { get; set; } = new List<Dto.MoralStat.List>();

        public List<SelectListItem> MoralList { get; set; } = new List<SelectListItem>();

        public List<Dto.MoralItem.Info> MoralItemList { get; set; } = new List<Dto.MoralItem.Info>();

        //public List<XkSystem.Areas.Student.Dto.Student.Info> StudentList { get; set; } = new List<Student.Dto.Student.Info>();

        public List<SelectListItem> MoralClassList { get; set; } = new List<SelectListItem>();


    }
}