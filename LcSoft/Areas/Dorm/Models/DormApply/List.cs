using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormApply
{
    public class List
    {
        public List<Dto.DormApply.List> DormApplyList { get; set; } = new List<Dto.DormApply.List>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? CheckStatusId { get; set; } = System.Web.HttpContext.Current.Request["CheckStatusId"].ConvertToInt();

        public List<System.Web.Mvc.SelectListItem> CheckStatusList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}