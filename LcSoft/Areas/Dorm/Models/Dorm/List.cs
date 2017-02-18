using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.Dorm
{
    public class List
    {
        public List<Dto.Dorm.List> DormList { get; set; } = new List<Dto.Dorm.List>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}