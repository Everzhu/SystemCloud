using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictCensus
{
    public class List
    {
        public List<Entity.tbDictCensus> DataList { get; set; } = new List<Entity.tbDictCensus>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}