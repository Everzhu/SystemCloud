using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Models.DormOption
{
    public class List
    {
        public List<Entity.tbDormOption> DormOptionList { get; set; } = new List<Entity.tbDormOption>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}