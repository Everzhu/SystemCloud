using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormData
{
    public class List
    {
        public List<Dto.DormData.List> DormDataList { get; set; } = new List<Dto.DormData.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? DormOptionId { get; set; } = System.Web.HttpContext.Current.Request["DormOptionId"].ConvertToInt();

        public List<System.Web.Mvc.SelectListItem> DormOptionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}