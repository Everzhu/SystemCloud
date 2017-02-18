using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Models.ElectiveChange
{
    public class List
    {
        public List<SelectListItem> ElectiveList { get; set; } = new List<SelectListItem>();

        public bool IsWeekPeriod { get; set; }

        public bool IsHiddenSection { get; set; }

        public bool IsHiddenGroup { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public List<Dto.ElectiveChange.List> ElectiveChangeList { get; set; } = new List<Dto.ElectiveChange.List>();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public XkSystem.Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}