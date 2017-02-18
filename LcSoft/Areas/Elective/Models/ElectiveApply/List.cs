using XkSystem.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Models.ElectiveApply
{
    public class List
    {
        public List<Dto.ElectiveApply.List> ElectiveApplyList { get; set; } = new List<Dto.ElectiveApply.List>();


        public List<SelectListItem> ElectiveList { get; set; } = new List<SelectListItem>();

        public int ElectiveId { get; set; } = HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();

        public PageHelper Page { get; set; } = new PageHelper();

    }
}