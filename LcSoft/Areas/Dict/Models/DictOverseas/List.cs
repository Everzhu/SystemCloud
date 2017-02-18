using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictOverseas
{
    public class List
    {
        public List<Entity.tbDictOverseas> DataList { get; set; } = new List<Entity.tbDictOverseas>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}