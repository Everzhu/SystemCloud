using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.Study
{
    public class List
    {
        public List<Dto.Study.List> StudyList { get; set; } = new List<Dto.Study.List>();
        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> IsRoomList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();
        public int IsRoomId { get; set; } = System.Web.HttpContext.Current.Request["IsRoomId"].ConvertToInt();
        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}